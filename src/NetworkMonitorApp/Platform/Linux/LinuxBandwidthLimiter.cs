using NetworkLimiter.src.NetworkMonitorApp.Interfaces;
using System.Diagnostics;

namespace NetworkLimiter.src.NetworkMonitorApp.Platform.Linux
{
    public class LinuxBandwidthLimiter : IBandwidthLimiter
    {
        public void SetLimit(string interfaceName, long bytesPerSecond)
        {
            long kbps = bytesPerSecond * 8 / 1000; // Convert bytes/sec to kbps
            string command = $"tc qdisc add dev {interfaceName} root tbf rate {kbps}kbit burst 32kbit latency 400ms";
            ExecuteCommand(command);
        }

        public void RemoveLimit(string interfaceName)
        {
            string command = $"tc qdisc del dev {interfaceName} root";
            ExecuteCommand(command);
        }

        private void ExecuteCommand(string command)
        {
            var process = new ProcessStartInfo("bash", $"-c \"{command}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var proc = Process.Start(process);
            proc?.WaitForExit();
        }
    }
}
