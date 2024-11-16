using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetworkMonitorApp.Core
{
    public static class PasswordManager
    {
        const string PasswordFilePath = "password.dat";

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
    }
}
