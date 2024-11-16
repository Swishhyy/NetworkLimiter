using NetworkLimiter.src.NetworkMonitorApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLimiter.src.NetworkMonitorApp.Platform.Windows
{
    public class WindowsBandwidthLimiter : IBandwidthLimiter
    {
        public void SetLimit(string interfaceName, long bytesPerSecond)
        {
            // Use 'netsh' command to set limit
            string command = $"netsh interface qos set interface \"{interfaceName}\" throttle={bytesPerSecond}";
            ExecuteCommand(command);
        }

        public void RemoveLimit(string interfaceName)
        {
            // Use 'netsh' command to remove limit
            string command = $"netsh interface qos delete interface \"{interfaceName}\"";
            ExecuteCommand(command);
        }

        private void ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/C " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Verb = "runas" // Run as administrator
            };
            Process.Start(processInfo)?.WaitForExit();
        }
    }
}
