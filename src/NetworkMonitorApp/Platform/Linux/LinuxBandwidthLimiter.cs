using NetworkLimiter.src.NetworkMonitorApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLimiter.src.NetworkMonitorApp.Platform.Linux
{
    public class LinuxBandwidthLimiter : IBandwidthLimiter
    {
        public void SetLimit(string interfaceName, long bytesPerSecond)
        {
            string rate = $"{bytesPerSecond}bps";
            string command = $"sudo tc qdisc add dev {interfaceName} root tbf rate {rate} burst 32kbit latency 400ms";
            ExecuteCommand(command);
        }

        public void RemoveLimit(string interfaceName)
        {
            string command = $"sudo tc qdisc del dev {interfaceName} root";
            ExecuteCommand(command);
        }

        private void ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("/bin/bash", "-c \"" + command + "\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();
        }
    }
}
