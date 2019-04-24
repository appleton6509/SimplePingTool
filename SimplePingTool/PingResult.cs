using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePingTool
{
    public class PingResult
    {
        public string Latency { get; set; }
        public string AddressOrIp { get; set; }
        public string Status { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
