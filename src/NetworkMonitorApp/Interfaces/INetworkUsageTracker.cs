using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLimiter.src.NetworkMonitorApp.Interfaces
{
    public interface INetworkUsageTracker
    {
        IEnumerable<NetworkUsage> GetCurrentUsages();
    }

}
