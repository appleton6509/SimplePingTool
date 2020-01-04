using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryData
{
    public static class SortProcesses
    {
        private static void exchange(ObservableCollection<MinimalProcess> data, int m, int n)
        {
            MinimalProcess temporary;

            temporary = data[m];
            data[m] = data[n];
            data[n] = temporary;
        }

        private static void ProcessArrayQuickSort(ObservableCollection<MinimalProcess> data, int l, int r)
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
                ProcessArrayQuickSort(data, l, j);
            if (i < r)
                ProcessArrayQuickSort(data, i, r);
        }

        public static void ProcessArrayQuickSort(ObservableCollection<MinimalProcess> data)
        {
            ProcessArrayQuickSort(data, 0, data.Count - 1);
        }
    }
}
