using PingData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimplePingTool.ModelView
{
    public class MainWindowModelView : INotifyPropertyChanged
    {
        public MainWindowModelView()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
