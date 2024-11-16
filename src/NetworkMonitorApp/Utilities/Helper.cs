using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace NetworkMonitorApp.Utilities
{
    public static class Helper
    {
        // Add constants
        const string PasswordFilePath = "password.dat";

        public static string FormatBytesPerSecond(long bytesPerSecond)
        {
            if (bytesPerSecond < 1024)
            {
                return $"{bytesPerSecond} B/s";
            }
            else if (bytesPerSecond < 1024 * 1024)
            {
                double kbps = bytesPerSecond / 1024.0;
                return $"{kbps:F2} KB/s";
            }
            else if (bytesPerSecond < 1024 * 1024 * 1024)
            {
                double mbps = bytesPerSecond / (1024.0 * 1024.0);
                return $"{mbps:F2} MB/s";
            }
            else
            {
                double gbps = bytesPerSecond / (1024.0 * 1024.0 * 1024.0);
                return $"{gbps:F2} GB/s";
            }
        }

        public static string GetConnectionType(string interfaceName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                var ni = Array.Find(interfaces, i => i.Name == interfaceName);
                if (ni != null)
                {
                    switch (ni.NetworkInterfaceType)
                    {
                        case NetworkInterfaceType.Ethernet:
                        case NetworkInterfaceType.GigabitEthernet:
                        case NetworkInterfaceType.FastEthernetFx:
                        case NetworkInterfaceType.FastEthernetT:
                            return "Ethernet";
                        case NetworkInterfaceType.Wireless80211:
                            return "WiFi";
                        default:
                            return "Other";
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (interfaceName.StartsWith("eth"))
                    return "Ethernet";
                else if (interfaceName.StartsWith("wlan") || interfaceName.StartsWith("wifi"))
                    return "WiFi";
                else
                    return "Other";
            }

            return "Unknown";
        }

        public static string MakeValidFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Concat(name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).Replace(' ', '_');
        }

        // Administrator privilege check
        public static bool IsAdministrator()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetCurrentUserId() == 0; // Root user ID is 0
            }
            return false;
        }

        // Get current user ID on Linux
        private static int GetCurrentUserId()
        {
            try
            {
                return int.Parse(File.ReadAllText("/proc/self/status")
                    .Split('\n')
                    .FirstOrDefault(line => line.StartsWith("Uid:"))
                    ?.Split('\t')[1] ?? "1000"); // Default to 1000 if parsing fails
            }
            catch
            {
                return 1000; // Default to non-root user ID
            }
        }

        // Password management methods
        public static bool IsPasswordSet()
        {
            return File.Exists(PasswordFilePath);
        }

        public static void SetPassword(string password)
        {
            string hashedPassword = HashPassword(password);
            File.WriteAllText(PasswordFilePath, hashedPassword);
        }

        public static bool VerifyPassword(string password)
        {
            if (!IsPasswordSet())
                return false;

            string storedHash = File.ReadAllText(PasswordFilePath);
            string enteredHash = HashPassword(password);
            return storedHash == enteredHash;
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        // Secure password input
        public static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Length--;
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password.Append(info.KeyChar);
                    Console.Write("*");
                }
            }
            return password.ToString();
        }
    }
}
