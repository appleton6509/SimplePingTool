using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MemoryData
{
    public class MinimalProcess : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public double TotalMemory { get; set; }
        private double _memory;
        public double Memory
        {
            get
            {
                return _memory;
            }
            set
            {
                _memory = value;
                OnPropertyChange();
            }
        }

        public MinimalProcess(string name, double memory, double totalMemory)
        {
            this.Name = name;
            this.Memory = memory;
            this.TotalMemory = totalMemory;
        }

        public MinimalProcess() { }


        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChange([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
