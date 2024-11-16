using NetworkLimiter.src.NetworkMonitorApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLimiter.src.NetworkMonitorApp.Platform.Linux
{
    public class LinuxNetworkUsageTracker : INetworkUsageTracker
    {
        public IEnumerable<NetworkUsage> GetCurrentUsages()
        {
            var networkUsages = new List<NetworkUsage>();
            string[] lines = File.ReadAllLines("/proc/net/dev");

            foreach (var line in lines.Skip(2)) // Skip headers
            {
                var parts = line.Split(':');
                if (parts.Length < 2) continue;

                string interfaceName = parts[0].Trim();
                string[] stats = parts[1].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                networkUsages.Add(new NetworkUsage
                {
                    InterfaceName = interfaceName,
                    BytesReceived = long.Parse(stats[0]),
                    BytesSent = long.Parse(stats[8])
                });
            }

            return networkUsages;
        }
    }
}
