using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsProcess
{
    public class ProcessTree
    {
        public string Name { get; set; }
        public ObservableCollection<ProcessData> Data { get; set; } = new ObservableCollection<ProcessData>();

        public ProcessTree(string name, int processId)
        {
            this.Data.Add(new ProcessData() { Id = processId });
            this.Name = name;
        }

    }
    public class ProcessData
    {
        public int Id { get; set; }

    }
}
