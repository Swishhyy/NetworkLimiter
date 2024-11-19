using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetworkLimiter.src.NetworkMonitorApp.Interfaces;

namespace NetworkLimiter.src.NetworkMonitorApp.Core
{
    public class NetworkBandwidthLimiter
    {
        private readonly IBandwidthLimiter _platformBandwidthLimiter;

        public NetworkBandwidthLimiter()
        {
            // Use PlatformFactory to get the correct limiter based on OS
            _platformBandwidthLimiter = PlatformFactory.GetBandwidthLimiter();
        }

        public void SetLimit(string interfaceName, long bytesPerSecond)
        {
            _platformBandwidthLimiter.SetLimit(interfaceName, bytesPerSecond);
        }

        public void RemoveLimit(string interfaceName)
        {
            _platformBandwidthLimiter.RemoveLimit(interfaceName);
        }
    }
}
