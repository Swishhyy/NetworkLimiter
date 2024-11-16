using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using NetworkMonitorApp.Core;
using NetworkMonitorApp.Utilities;
using NetworkLimiter.src.NetworkMonitorApp.Core;

namespace NetworkMonitorApp
{
    class Program
    {
        // Add global variables
        static NetworkManager networkManager;
        static string selectedInterface;
        static bool isPrivacyMode = false;
        static Dictionary<string, long> speedLimits = new Dictionary<string, long>();
        // Add after 'static bool isPrivacyMode = false;'
        static long previousBytesSent = 0;
        static long previousBytesReceived = 0;
        static System.Timers.Timer timer;

        static bool isRunning = false;
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Application started.");

                // Check for administrator privileges
                if (!Helper.IsAdministrator())
                {
                    Console.WriteLine("Please run the application as an administrator.");
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("Running as administrator.");

                // Password protection
                if (Helper.IsPasswordSet())
                {
                    Console.Write("Enter password: ");
                    string inputPassword = Helper.ReadPassword();
                    if (!Helper.VerifyPassword(inputPassword))
                    {
                        Console.WriteLine("Incorrect password. Exiting application.");
                        Console.ReadLine();
                        return;
                    }
                }
                Console.WriteLine("Password verification passed.");

                networkManager = new NetworkManager();

                // Network selection prompt
                SelectNetworkInterface();

                // Initialize previous byte counts
                InitializeNetworkStatistics();

                // Set up the timer
                SetupTimer();

                // Start the live updates
                isRunning = true;

                // Main loop to handle key presses
                while (isRunning)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true);
                        if (key.Key == ConsoleKey.M)
                        {
                            // Stop the timer to pause updates
                            timer.Stop();

                            // Clear the console
                            Console.Clear();

                            // Display the options menu
                            DisplayOptionsMenu();

                            // After returning from the options menu, resume updates if still running
                            if (isRunning)
                            {
                                Console.Clear();
                                SetupTimer();
                            }
                        }
                        else if (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                        {
                            // Exit on Ctrl+C
                            isRunning = false;
                        }
                    }

                    // Small delay to reduce CPU usage
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                // Stop and dispose of the timer when the application is exiting
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }

                Console.WriteLine("Exiting application...");
                // Removed Console.ReadLine(); to allow the application to exit immediately
            }
        }

        static void SetupTimer()
        {
            if (timer == null)
            {
                timer = new System.Timers.Timer(1000); // Interval in milliseconds
                timer.Elapsed += UpdateNetworkStatistics;
                timer.AutoReset = true;
            }
            timer.Start();
        }

        static void InitializeNetworkStatistics()
        {
            previousBytesSent = GetTotalBytesSent(selectedInterface);
            previousBytesReceived = GetTotalBytesReceived(selectedInterface);
        }

        /// <summary>
        /// Updates network statistics every second and displays live data.
        /// </summary>
        /// <param name="source">The timer source.</param>
        /// <param name="e">Elapsed event arguments.</param>
        static void UpdateNetworkStatistics(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!timer.Enabled)
                {
                    // If the timer is not enabled, skip updating
                    return;
                }

                long currentBytesSent = GetTotalBytesSent(selectedInterface);
                long currentBytesReceived = GetTotalBytesReceived(selectedInterface);

                long bytesSentPerSecond = currentBytesSent - previousBytesSent;
                long bytesReceivedPerSecond = currentBytesReceived - previousBytesReceived;

                double mbSentPerSecond = bytesSentPerSecond / (1024.0 * 1024.0);
                double mbReceivedPerSecond = bytesReceivedPerSecond / (1024.0 * 1024.0);

                // Update previous byte counts
                previousBytesSent = currentBytesSent;
                previousBytesReceived = currentBytesReceived;

                // Display live network statistics
                DisplayLiveNetworkStatistics(mbSentPerSecond, mbReceivedPerSecond);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in UpdateNetworkStatistics: " + ex.Message);
                // Optionally, log the exception or handle it as needed
            }
        }


        static void DisplayLiveNetworkStatistics(double mbSentPerSecond, double mbReceivedPerSecond)
        {
            // Clear console to refresh the display
            Console.Clear();

            // Display network information
            DisplayNetworkInformation();

            // Display live network statistics
            Console.WriteLine("=== Live Network Statistics ===\n");

            // Set title color
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0,-30} {1}", "Metric", "Value");
            Console.WriteLine(new string('-', 45));
            Console.ResetColor();

            // Display Bytes Sent per Second
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,-30}: ", "Bytes Sent per Second");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0:F2} MB/s", mbSentPerSecond);

            // Display Bytes Received per Second
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,-30}: ", "Bytes Received per Second");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0:F2} MB/s", mbReceivedPerSecond);

            Console.ResetColor();

            Console.WriteLine(); // Add an empty line for spacing

            // Instructions for the user
            Console.WriteLine("Press 'M' to open the options menu, or 'Ctrl+C' to exit.");
        }

        static void SelectNetworkInterface()
        {
            // Retrieve available network interfaces
            var interfaces = networkManager.GetUsages().Select(u => u.InterfaceName).Distinct().ToList();

            if (interfaces.Count == 0)
            {
                Console.WriteLine("No network interfaces found.");
                Environment.Exit(0);
            }

            // Display interfaces
            Console.WriteLine("Available Network Interfaces:");
            for (int i = 0; i < interfaces.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {interfaces[i]}");
            }

            // Prompt user for selection
            int selectedIndex = -1;
            while (selectedIndex < 0 || selectedIndex >= interfaces.Count)
            {
                Console.Write("Select the network interface you want to manage (enter the number): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int index) && index > 0 && index <= interfaces.Count)
                {
                    selectedIndex = index - 1;
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                }
            }

            selectedInterface = interfaces[selectedIndex];
        }
        static void DisplayNetworkInformation()
        {
            // Retrieve network interface details
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(ni => ni.Name == selectedInterface);

            if (networkInterface == null)
            {
                Console.WriteLine("Selected network interface not found.");
                Environment.Exit(0);
            }

            // Get IP properties
            var ipProperties = networkInterface.GetIPProperties();

            // Collect IP addresses
            var ipv4Addresses = ipProperties.UnicastAddresses
                .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(ip => ip.Address.ToString())
                .ToList();

            var ipv6Addresses = ipProperties.UnicastAddresses
                .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                .Select(ip => ip.Address.ToString())
                .ToList();

            // Get network statistics
            var statistics = networkInterface.GetIPv4Statistics();

            // Clear console (if desired)
            // You might want to remove this if the console is cleared elsewhere
            // Console.Clear();

            // Display section header with color
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== Network Interface Details ===\n");
            Console.ResetColor();

            // Display Name
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,-20}: ", "Name");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0}", networkInterface.Name);

            // Display Description
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,-20}: ", "Description");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0}", networkInterface.Description);

            // Display Type
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,-20}: ", "Type");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0}", networkInterface.NetworkInterfaceType);

            // Display Status
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,-20}: ", "Status");
            Console.ForegroundColor = networkInterface.OperationalStatus == OperationalStatus.Up ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine("{0}", networkInterface.OperationalStatus);

            // Display Speed
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,-20}: ", "Speed");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0} Mbps", networkInterface.Speed / 1_000_000);

            // Add an empty line for spacing
            Console.WriteLine();

            if (!isPrivacyMode)
            {
                // Display IPv4 Addresses
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("{0,-20}: ", "IPv4 Addresses");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("{0}", string.Join(", ", ipv4Addresses));

                // Display IPv6 Addresses
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("{0,-20}: ", "IPv6 Addresses");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("{0}", string.Join(", ", ipv6Addresses));

                // Display Bytes Received
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("{0,-20}: ", "Bytes Received");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}", statistics.BytesReceived);

                // Display Bytes Sent
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("{0,-20}: ", "Bytes Sent");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}", statistics.BytesSent);
            }
            else
            {
                // Display privacy mode message
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Vital data is hidden in privacy mode.");
            }

            // Add an empty line for spacing
            Console.WriteLine();

            // Display Speed Limit
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0,-20}: ", "Speed Limit");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0}", GetSpeedLimit(selectedInterface));

            // Reset console colors
            Console.ResetColor();

            // Add an empty line for spacing
            Console.WriteLine();
        }

        static void DisplayOptionsMenu()
        {
            bool inOptionsMenu = true;

            while (inOptionsMenu)
            {
                Console.WriteLine("\nOptions Menu:");
                Console.WriteLine("1. Limit Speeds");
                Console.WriteLine("2. Unlimit Speeds");
                Console.WriteLine("3. Toggle Privacy Mode");
                Console.WriteLine("4. Set/Change Password");
                Console.WriteLine("5. Return to Live Network Information");
                Console.WriteLine("6. Exit");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        LimitSpeeds(selectedInterface);
                        break;
                    case "2":
                        UnlimitSpeeds(selectedInterface);
                        break;
                    case "3":
                        TogglePrivacyMode();
                        break;
                    case "4":
                        ManagePassword();
                        break;
                    case "5":
                        // Return to live updates
                        inOptionsMenu = false;
                        break;
                    case "6":
                        // Exit the application
                        isRunning = false;
                        inOptionsMenu = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (inOptionsMenu)
                {
                    Console.WriteLine("\nPress Enter to return to the options menu...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            // After exiting the options menu, the timer will be restarted in the main loop
        }

        static void LimitSpeeds(string interfaceName)
        {
            Console.Write("Enter the speed limit in Mbps: ");
            string input = Console.ReadLine();
            if (double.TryParse(input, out double mbps))
            {
                long bytesPerSecond = (long)(mbps * 1024 * 1024 / 8); // Convert Mbps to Bytes per second
                networkManager.LimitBandwidth(interfaceName, bytesPerSecond);
                speedLimits[interfaceName] = bytesPerSecond;
                Console.WriteLine($"Speed limit of {mbps} Mbps applied to {interfaceName}.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a numeric value.");
            }
        }

        static void UnlimitSpeeds(string interfaceName)
        {
            networkManager.RemoveBandwidthLimit(interfaceName);
            speedLimits.Remove(interfaceName);
            Console.WriteLine($"Speed limit removed from {interfaceName}.");
        }

        static void TogglePrivacyMode()
        {
            isPrivacyMode = !isPrivacyMode;
            if (isPrivacyMode)
            {
                Console.WriteLine("Privacy mode enabled. Vital data will be hidden.");
            }
            else
            {
                Console.WriteLine("Privacy mode disabled. Vital data will be displayed.");
            }
        }
        static long GetTotalBytesSent(string interfaceName)
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(ni => ni.Name == interfaceName);

            if (networkInterface != null)
            {
                return networkInterface.GetIPv4Statistics().BytesSent;
            }
            return 0;
        }

        static long GetTotalBytesReceived(string interfaceName)
        {
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(ni => ni.Name == interfaceName);

            if (networkInterface != null)
            {
                return networkInterface.GetIPv4Statistics().BytesReceived;
            }
            return 0;
        }


        static void ManagePassword()
        {
            if (Helper.IsPasswordSet())
            {
                Console.Write("Enter current password: ");
                string currentPassword = Helper.ReadPassword();
                if (Helper.VerifyPassword(currentPassword))
                {
                    Console.Write("Enter new password: ");
                    string newPassword = Helper.ReadPassword();
                    Helper.SetPassword(newPassword);
                    Console.WriteLine("Password changed successfully.");
                }
                else
                {
                    Console.WriteLine("Incorrect password.");
                }
            }
            else
            {
                Console.Write("Set a new password: ");
                string newPassword = Helper.ReadPassword();
                Helper.SetPassword(newPassword);
                Console.WriteLine("Password set successfully.");
            }
        }

        static string GetSpeedLimit(string interfaceName)
        {
            if (speedLimits.TryGetValue(interfaceName, out long bytesPerSecond))
            {
                double mbps = bytesPerSecond * 8 / (1024.0 * 1024.0);
                return $"{mbps:F2} Mbps";
            }
            return "No limit";
        }
    }
}
