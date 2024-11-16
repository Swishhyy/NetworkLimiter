using NetworkLimiter.src.NetworkMonitorApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLimiter.src.NetworkMonitorApp.Core
{
    public class NetworkManager
    {
        private readonly INetworkUsageTracker _usageTracker;
        private readonly IBandwidthLimiter _bandwidthLimiter;

        public NetworkManager()
        {
            _usageTracker = PlatformFactory.GetNetworkUsageTracker();
            _bandwidthLimiter = PlatformFactory.GetBandwidthLimiter();
        }

        public IEnumerable<NetworkUsage> GetUsages()
        {
            return _usageTracker.GetCurrentUsages();
        }

        public void LimitBandwidth(string interfaceName, long bytesPerSecond)
        {
            _bandwidthLimiter.SetLimit(interfaceName, bytesPerSecond);
        }

        public void RemoveBandwidthLimit(string interfaceName)
        {
            _bandwidthLimiter.RemoveLimit(interfaceName);
        }
    }
}
