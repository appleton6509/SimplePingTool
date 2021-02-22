using PingData.BusinessLogic;
using System;
using System.IO;


namespace PingData.Utils
{
    public static class LogUtil
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
                SetPath(value);
            }
        }

        /// <summary>
        /// Parse a string value into a valid directory string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ParseDirectoryPath(string path)
        {

            string parsedFilePath = path.Replace("/", @"\");

            if (parsedFilePath[parsedFilePath.Length-1] != '\\')
                parsedFilePath += @"\";

            if (!Directory.Exists(parsedFilePath))
            {
                try
                {
                    Directory.CreateDirectory(parsedFilePath);
                }
                catch
                {
                    return false;
                }
            }

            try
            {
                string fileName = DateTime.Today.ToShortDateString().Substring(0).Replace('/', '-') + ".log";

                using (StreamWriter fs = File.AppendText(parsedFilePath + fileName))
                    fs.WriteLine("");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Log the results of a ping to a text file
        /// </summary>
        /// <param name="pingResults"></param>
        /// <param name="path"></param>
        public static void LogToTextFile(Response pingResults, string path = null)
        {
            //path unset, set it
            if (String.IsNullOrEmpty(path))
                path = Path;

            //valid pingresult and directory exists
            if (Object.Equals(pingResults, null) && !Directory.Exists(path))
                return;

            string currentDate = DateTime.Today.ToShortDateString().Substring(0).Replace('/', '-');
            string fileName = currentDate + ".log";

            using (StreamWriter fs = File.AppendText(path + fileName))
                fs.WriteLine(pingResults.ToString());
        }

        private static void SetPath(string value)
        {
            if (ParseDirectoryPath(value))
                _path = value;
            else
                throw new Exception("Directory is invalid or user does not have the correct permissions");
        }
    }
}
