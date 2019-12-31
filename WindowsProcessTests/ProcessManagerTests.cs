using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WindowsProcess.Tests
{
    [TestClass()]
    public class ProcessManagerTests
    {
        [TestMethod()]
        public void GetRunningProcesses_ReturnsMoreThanZero()
        {

            //assign
            ProcessManager process = new ProcessManager();
            bool expectedResult = true;
            bool actualResult;

            //act
            Process[] myProcesses = process.GetRunningProcesses();

            if (myProcesses.Length > 0)
                actualResult = true;
            else
                actualResult = false;

            //assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}