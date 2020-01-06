using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MemoryData
{
    public static class ProcessHandler
    {

        /// <summary>
        /// Updates a list of processes with the current list of processes.
        /// </summary>
        /// <param name="listToUpdate">The list that will be updated</param>
        public static void UpdateProcessList(ObservableCollection<ProcessMemory> listToUpdate)
        {

            Hashtable comparableList = CreateCombinedProcessList();

            if (listToUpdate != null)
            {
                for (int i = 0; i < listToUpdate.Count; i++)
                {
                    if (comparableList.ContainsKey(listToUpdate[i].Name))
                    {
                        //update existing process list
                        (comparableList[listToUpdate[i].Name] as ProcessMemory).CopyTo(listToUpdate[i]);

                        //remove from compare process list
                        comparableList.Remove(listToUpdate[i].Name);
                    }
                    else
                    {
                        //remove from existing process list
                        listToUpdate.Remove(listToUpdate[i]);
                    }
                }
            }

            //Add new processes to the list
            foreach (DictionaryEntry process in comparableList)
            {
                listToUpdate.Add((process.Value as ProcessMemory));
            }
        }

        /// <summary>
        /// Creates a list of processes, with each process contained in list once, combining memory totals of additional process instances
        /// </summary>
        /// <returns>A Hashtable of MinimalProcess Objects</returns>
        internal static Hashtable CreateCombinedProcessList(Process[] processes = null)
        {
            //processes not provided, create them.
            if (processes == null)
                processes = Process.GetProcesses();

            Hashtable table = new Hashtable();

            //
            foreach (Process process in processes)
            {
                //process is null, skip it
                if (process == null)
                    continue;

                string key = process.ProcessName;

                if (table.ContainsKey(key))
                {
                    CombineProcessWithMinimalProcess((ProcessMemory)table[key], process);
                }
                else
                {
                    table.Add(key, ConvertProcessToMinimalProcess(process));
                }
            }

            return table;
        }

        /// <summary>
        /// Converts a process class object into a minimal process object
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static ProcessMemory ConvertProcessToMinimalProcess(Process process, ProcessMemory minProcess = null)
        {
            if (minProcess == null)
                minProcess = new ProcessMemory();

            minProcess.Memory = (double)process.WorkingSet64;
            minProcess.Name = process.ProcessName;
            minProcess.NonpagedSystemMemorySize64 = (double)process.NonpagedSystemMemorySize64;
            minProcess.PagedMemorySize64 = (double)process.PagedMemorySize64;
            minProcess.PagedSystemMemorySize64 = (double)process.PagedSystemMemorySize64;
            minProcess.PrivateMemorySize64 = (double)process.PrivateMemorySize64;
            minProcess.VirtualMemorySize64 = (double)process.VirtualMemorySize64;
            minProcess.PeakPagedMemorySize64 = (double)process.PeakPagedMemorySize64;
            minProcess.PeakVirtualMemorySize64 = (double)process.PeakVirtualMemorySize64;
            minProcess.PeakWorkingSet64 = (double)process.PeakWorkingSet64;
            minProcess.Id = process.Id.ToString();

            return minProcess;
        }

        /// <summary>
        /// Convert processes of the same name into a single minimal process
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static void CombineProcessWithMinimalProcess(ProcessMemory minProcess, Process process)
        {
            minProcess.Memory += (double)process.WorkingSet64;
            minProcess.NonpagedSystemMemorySize64 += (double)process.NonpagedSystemMemorySize64;
            minProcess.PagedMemorySize64 += (double)process.PagedMemorySize64;
            minProcess.PagedSystemMemorySize64 += (double)process.PagedSystemMemorySize64;
            minProcess.PrivateMemorySize64 += (double)process.PrivateMemorySize64;
            minProcess.VirtualMemorySize64 += (double)process.VirtualMemorySize64;
            minProcess.Id += "," + process.Id.ToString();

        }

        #region Quick Sort Algorithm

        private static void exchange(ObservableCollection<ProcessMemory> data, int m, int n)
        {
            ProcessMemory temporary;

            temporary = data[m];
            data[m] = data[n];
            data[n] = temporary;
        }

        private static void QuickSort(ObservableCollection<ProcessMemory> data, int l, int r)
        {
            int i, j;
            double x;

            i = l;
            j = r;


            x = data[(l + r) / 2].Memory; /* find pivot item */
            while (true)
            {
                while (data[i].Memory > x)
                    i++;
                while (x > data[j].Memory)
                    j--;
                if (i <= j)
                {
                    exchange(data, i, j);
                    i++;
                    j--;
                }
                if (i > j)
                    break;
            }
            if (l < j)
                QuickSort(data, l, j);
            if (i < r)
                QuickSort(data, i, r);
        }


        /// <summary>
        /// Sorts a collection of processes in ascending order using the Quick Sort Algorithm
        /// </summary>
        /// <param name="data">collection of MinimalProcess objects</param>
        public static void QuickSort(ObservableCollection<ProcessMemory> data)
        {
            QuickSort(data, 0, data.Count - 1);
        }

        #endregion
    }
}
