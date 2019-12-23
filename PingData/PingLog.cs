using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingData
{
    public class PingLog
    {
        private string fileName = DateTime.Today.ToShortDateString().Substring(0).Replace('/', '-') + ".txt";

        public void LogToTextFile(PingResult pingResults)
        {
            using (StreamWriter fs = File.AppendText(fileName))
            {
                if (pingResults.StatusCode == 0) //Ping Successful
                {
                    fs.WriteLine($"{pingResults.TimeStamp}     Host: {pingResults.AddressOrIp} Status: {pingResults.Status} Latency: {pingResults.Latency} ms");
                } else //Ping Failed 
                {
                    fs.WriteLine($"{pingResults.TimeStamp}     Host: {pingResults.AddressOrIp} Status: {pingResults.Status} Error Code: {pingResults.StatusCode} Error Message:{pingResults.ErrorMessage}");
                }
            }
            
        }
    }
}
