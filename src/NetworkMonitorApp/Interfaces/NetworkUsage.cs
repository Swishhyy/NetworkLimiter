using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLimiter.src.NetworkMonitorApp.Interfaces
{
    public class NetworkUsage
    {
        public string InterfaceName { get; set; }
        public long BytesReceived { get; set; }
        public long BytesSent { get; set; }
    }
}
