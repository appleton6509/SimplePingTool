using MemoryData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using WindowsProcess;

namespace SimplePingTool.ViewModel
{
    public class MemoryViewModel : BaseViewModel
    {

        public ObservableCollection<MinimalProcess> AllProcesses { get; set; }

        public MemoryViewModel()
        {

            //create list
            AllProcesses = ProcessHandler.CreateCombinedProcessList(Process.GetProcesses());

            //sort list
            SortProcesses.ProcessArrayQuickSort(AllProcesses);



            System.Threading.Tasks.Task.Run(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(UpdateList));
                }
            });
        }

        private void UpdateList()
        {
            ProcessHandler.UpdateProcessList(AllProcesses, Process.GetProcesses());
        }

    }
}
