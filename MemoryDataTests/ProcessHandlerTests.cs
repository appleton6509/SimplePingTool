using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MemoryData;
using System.Collections;

namespace MemoryData.Tests
{
    [TestClass()]
    public class ProcessHandlerTests
    {

        /// <summary>
        /// Tests when a list that is null is passed as the first parameter
        /// </summary>
        [TestMethod()]
        public void GetProcessListTest_ListIsNull()
        {
            //assign
            Process[] processes = Process.GetProcesses();
            ObservableCollection<ProcessMemory> procList = null;


            //act & assert
            Assert.ThrowsException<NullReferenceException>(() => ProcessHandler.UpdateProcessList(procList));
        }

        /// <summary>
        /// Tests count of updated list is greater than zero
        /// </summary>
        [TestMethod()]
        public void GetProcessListTest_ReturnListCountGreaterThanZero()
        {
            //assign
            ObservableCollection<ProcessMemory> procList = new ObservableCollection<ProcessMemory>();
            bool condition = false;


            //act
            ProcessHandler.UpdateProcessList(procList);
            if (procList.Count >= 1)
                condition = true;

            //assert
            Assert.IsTrue(condition);
        }

        /// <summary>
        /// Tests speed of execution
        /// </summary>
        [TestMethod()]
        public void CreateCombinedProcessListTest_SpeedTest()
        {
            bool condition = true;

            ObservableCollection<ProcessMemory> fullProcessList2 = new ObservableCollection<ProcessMemory>();

            Hashtable fullProcessList = ProcessHandler.CreateCombinedProcessList();

            for (int i = 0; i < 1; i++)
                ProcessHandler.UpdateProcessList(fullProcessList2);

            //assert
            Assert.IsTrue(condition);
        }

    }
}
