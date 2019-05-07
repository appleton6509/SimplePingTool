using System;

namespace SimplePingTool
{
    public class PingResult
    {
        public int Latency { get; set; }
        public string AddressOrIp { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
