using NetworkLimiter.src.NetworkMonitorApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLimiter.src.NetworkMonitorApp.Platform.Windows
{
    public class WindowsNetworkUsageTracker : INetworkUsageTracker
    {
        public IEnumerable<NetworkUsage> GetCurrentUsages()
        {
            var networkUsages = new List<NetworkUsage>();
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var ni in interfaces)
            {
                var stats = ni.GetIPv4Statistics();
                networkUsages.Add(new NetworkUsage
                {
                    InterfaceName = ni.Name,
                    BytesReceived = stats.BytesReceived,
                    BytesSent = stats.BytesSent
                });
            }

            return networkUsages;
        }
    }
}
