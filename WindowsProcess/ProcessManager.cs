using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsProcess
{
    public class ProcessManager
    {

        public ProcessManager()
        {

        }
        public Process[] GetRunningProcesses()
        {
            Process[] RunningProcesses = Process.GetProcesses();

            foreach (Process process in RunningProcesses)
            {
                Debug.WriteLine(process.ProcessName);
            }

            return RunningProcesses;
        }
    }
}
