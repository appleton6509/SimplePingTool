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
        private static string _path = Directory.GetCurrentDirectory() + @"\";

        /// <summary>
        /// Directory path of the log file
        /// </summary>
        public static string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (ValidateDirectoryPath(value))
                    _path = value;
                else
                {
                    throw new Exception("Directory is invalid or user does not have the correct permissions");
                }
                
            }
        }

        /// <summary>
        /// Parse a string value into a valid directory string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ValidateDirectoryPath(string value)
        {

            if (Directory.Exists(value))
            {
                try
                {
                    string fileName = DateTime.Today.ToShortDateString().Substring(0).Replace('/', '-') + ".log";

                    using (StreamWriter fs = File.AppendText(value + fileName))
                    {
                        fs.WriteLine(" ");
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;

        }
        /// <summary>
        /// Log the results of a ping to a text file
        /// </summary>
        /// <param name="pingResults"></param>
        /// <param name="path"></param>
        public static void LogToTextFile(PingResult pingResults, string path = null)
        {
            //path unset, set it
            if (String.IsNullOrEmpty(path))
                path = Path;

            //validate pingresult and directory exists
            if (Object.Equals(pingResults, null) && !Directory.Exists(path))
            {
                return;
            }


            string fileName = DateTime.Today.ToShortDateString().Substring(0).Replace('/', '-') + ".log";

            using (StreamWriter fs = File.AppendText(path + fileName))
            {
                fs.WriteLine(pingResults.ToString());
            }

        }
    }
}
