using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MemoryData
{
    public static class ProcessHandler
    {
        internal class MinimalProcessComparer : IEqualityComparer<MinimalProcess>
        {
            public bool Equals(MinimalProcess x, MinimalProcess y)
            {
                if (string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                else
                {
                    return false;
                }

            }

            public int GetHashCode(MinimalProcess obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        /// <summary>
        /// Updates a list of processes with the current list of processes, updated and removing where necessary
        /// </summary>
        /// <param name="listToUpdate">The list that will be updated</param>
        public static void UpdateProcessList(ObservableCollection<MinimalProcess> listToUpdate, Process[] updatedList)
        {
            ObservableCollection<MinimalProcess> comparableList = CreateCombinedProcessList(updatedList);

            SortProcesses.ProcessArrayQuickSort(comparableList);

            listToUpdate.Clear();

            foreach (MinimalProcess process in comparableList)
                listToUpdate.Add(process);

        }
        /// <summary>
        /// Creates a list of processes, with each process contained in list once, combining memory totals of additional process instances
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<MinimalProcess> CreateCombinedProcessList(Process[] processes)
        {

            double TotalMemoryUsed = processes.Sum(x => x.WorkingSet64);

            Dictionary<string, MinimalProcess> dict = new Dictionary<string, MinimalProcess>();


            foreach (Process process in processes)
            {
                string key = process.ProcessName;
                double memory = (double)process.WorkingSet64;

                if (dict.ContainsKey(key))
                {
                    dict[key].Memory += memory;
                }
                else
                {
                    dict.Add(key, new MinimalProcess(key, memory, TotalMemoryUsed));
                }
            }

            return new ObservableCollection<MinimalProcess>(dict.Values);
        }
        public static MinimalProcess ConvertToMinimalProcess(Process process, double totalMemory)
        {
            MinimalProcess newProcess = new MinimalProcess()
            {
                Name = process.ProcessName,
                Memory = process.WorkingSet64,
                TotalMemory = totalMemory
            };

            return newProcess;
        }

    }
}
