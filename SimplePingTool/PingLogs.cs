using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePingTool
{
    class PingLogs
    {
        public void LogToTextFile(PingResult pingResults)
        {
            string latency = pingResults.Latency;
            string addressOrIp = pingResults.AddressOrIp;
            string status = pingResults.Status;
            string timeStamp = pingResults.TimeStamp.ToString();


        }
    }
}
