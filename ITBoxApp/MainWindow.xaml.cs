using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using PingData;
using System.Collections.ObjectModel;

namespace SimplePingTool
{
    /// <summary>
    /// Main Application of the Simple Ping Tool
    /// </summary>
    /// test
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Event which controls the movement of the window when the title bar is pressed and held down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gdTitleBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
