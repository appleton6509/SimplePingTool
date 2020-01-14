using ITBox.Converters;
using ITBox.HelperClasses;
using MemoryData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ITBox.ViewModel
{
    public class MemoryViewModel : BaseViewModel
    {
        #region Private Properties 

        private ObservableCollection<ProcessMemory> _allProcessMemoryUsage;
        private ProcessMemory _selectedProcess;

        #endregion Private Properties

        #region Public Properties

        /// <summary>
        /// A collection of currently running processes
        /// </summary>
        public ObservableCollection<ProcessMemory> AllProcessMemoryUsage
        {
            get
            {
                return _allProcessMemoryUsage;
            }
            set
            {
                _allProcessMemoryUsage = value;
            }
        }

        /// <summary>
        /// A Live Charts property that converts a value to calculate memory size and returns it
        /// </summary>
        public Func<double, string> MemorySizeFormatter { get; set; } = value =>
        {
            MemorySizeByteConverter size = new MemorySizeByteConverter();
            var converted = size.Convert(value, null, null, CultureInfo.CurrentCulture);
            return converted.ToString();
        };

        /// <summary>
        /// Total working(physical) memory currently in use, in bytes.
        /// </summary>
        public double TotalMemoryInUse
        {
            get
            {
                double totalMemory = AllProcessMemoryUsage.Sum(x => x.Memory);
                return totalMemory;
            }
        }

 

        /// <summary>
        /// a process currently selected
        /// </summary>
        public ProcessMemory SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                //if new selected process is null, do nothing and return
                if (value != null)
                {
                    _selectedProcess = value;
                    RaisePropertyChange();
                }
                else
                    return;
            }
        }

        #endregion Public Properties

        public MemoryViewModel()
        {

            AllProcessMemoryUsage = new ObservableCollection<ProcessMemory>();

            Task.Run(() =>
            {
                while (true)
                {
                    
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(UpdateProcessCollection));

                    System.Threading.Thread.Sleep(1000);
                }
            });
        }


        /// <summary>
        /// Updates a collection of processes with the most current processes.
        /// </summary>
        private void UpdateProcessCollection()
        {
            ProcessHandler.UpdateProcessList(AllProcessMemoryUsage);
            ProcessHandler.QuickSort(AllProcessMemoryUsage);
            RaisePropertyChange(nameof(TotalMemoryInUse));

        }

    }
}
