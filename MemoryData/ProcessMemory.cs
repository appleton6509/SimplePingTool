using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MemoryData
{
    public class ProcessMemory : INotifyPropertyChanged
    {

        #region Private Properties

        private string _name;
        private double _memory;
        private string _id;
        private double _nonpagedSystemMemorySize64;
        private double _pagedMemorySize64;
        private double _pagedSystemMemorySize64;
        private double _privateMemorySize64;
        private double _virtualMemorySize64;
        private double _peakPagedMemorySize64;
        private double _peakVirtualMemorySize64;
        private double _peakWorkingSet64;

        #endregion

        #region Public Properties
        /// <summary>
        /// Process name
        /// </summary>
        public string Name {
        
            get { return _name; }
            set { 
                _name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The total physical memory(working set) for the given process, in bytes 
        /// </summary>
        public double Memory
        {

            get { return _memory; }
            set
            {
                _memory = value;
                RaisePropertyChanged();
            }
        }

        public string Id
        {

            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        public double NonpagedSystemMemorySize64
        {

            get { return _nonpagedSystemMemorySize64; }
            set
            {
                _nonpagedSystemMemorySize64 = value;
                RaisePropertyChanged();
            }
        }

        public double PagedMemorySize64
        {

            get { return _pagedMemorySize64; }
            set
            {
                _pagedMemorySize64 = value;
                RaisePropertyChanged();
            }
        }

        public double PagedSystemMemorySize64
        {

            get { return _pagedSystemMemorySize64; }
            set
            {
                _pagedSystemMemorySize64 = value;
                RaisePropertyChanged();
            }
        }

        public double PrivateMemorySize64
        {

            get { return _privateMemorySize64; }
            set
            {
                _privateMemorySize64 = value;
                RaisePropertyChanged();
            }
        }

        public double VirtualMemorySize64
        {

            get { return _virtualMemorySize64; }
            set
            {
                _virtualMemorySize64 = value;
                RaisePropertyChanged();
            }
        }

        public double PeakPagedMemorySize64
        {

            get { return _peakPagedMemorySize64; }
            set
            {
                _peakPagedMemorySize64 = value;
                RaisePropertyChanged();
            }
        }

        public double PeakVirtualMemorySize64
        {

            get { return _peakVirtualMemorySize64; }
            set
            {
                _peakVirtualMemorySize64 = value;
                RaisePropertyChanged();
            }
        }

        public double PeakWorkingSet64
        {

            get { return _peakWorkingSet64; }
            set
            {
                _peakWorkingSet64 = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// create a new process with minimal properties
        /// </summary>
        /// <param name="name">Name of the process</param>
        /// <param name="memoryInbytes">memory used, in bytes</param>
        public ProcessMemory(string name, double memoryInbytes)
        {
            this.Name = name;
            this.Memory = memoryInbytes;
        }

        public ProcessMemory() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Copies the properties of this object to another object
        /// </summary>
        /// <param name="process"></param>
        public void CopyTo(ProcessMemory process)
        {
            process.Id = this.Id;
            process.Memory = this.Memory;
            process.Name = this.Name;
            process.NonpagedSystemMemorySize64 = this.NonpagedSystemMemorySize64;
            process.PagedMemorySize64 = this.PagedSystemMemorySize64;
            process.PeakPagedMemorySize64 = this.PeakPagedMemorySize64;
            process.PeakVirtualMemorySize64 = this.PeakVirtualMemorySize64;
            process.PeakWorkingSet64 = this.PeakWorkingSet64;
            process.PrivateMemorySize64 = this.PrivateMemorySize64;
            process.VirtualMemorySize64 = this.VirtualMemorySize64;
        }

        #endregion

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged([CallerMemberName]string properetyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(properetyName));
        }

        #endregion

    }
}
