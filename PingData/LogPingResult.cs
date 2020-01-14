using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingData
{
    public static class LogPingResult
    {
        /// <summary>
        /// Directory path of the log file
        /// </summary>
        public static string Path = Directory.GetCurrentDirectory() + "\\";
        
        public static void LogToTextFile(PingResult pingResults, string path = null)
        {

            if (pingResults != null)
            {
                if (String.IsNullOrEmpty(path))
                    path = Path;


                string fileName = DateTime.Today.ToShortDateString().Substring(0).Replace('/', '-') + ".log";

                using (StreamWriter fs = File.AppendText(path + fileName))
                {
                    fs.WriteLine(pingResults.ToString());
                }
            }

        }
    }
}
