using NetworkLimiter.src.NetworkMonitorApp.Interfaces;
using NetworkLimiter.src.NetworkMonitorApp.Platform.Linux;
using NetworkLimiter.src.NetworkMonitorApp.Platform.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLimiter.src.NetworkMonitorApp.Core
{
    public static class PlatformFactory
    {
        public static INetworkUsageTracker GetNetworkUsageTracker()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsNetworkUsageTracker();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new LinuxNetworkUsageTracker();
            else
                throw new PlatformNotSupportedException("Only Windows and Linux are supported.");
        }

        public static IBandwidthLimiter GetBandwidthLimiter()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsBandwidthLimiter();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new LinuxBandwidthLimiter();
            else
                throw new PlatformNotSupportedException("Only Windows and Linux are supported.");
        }
    }
}
