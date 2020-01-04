﻿using PingData;
using SimplePingTool.HelperClasses;
using SimplePingTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimplePingTool
{

    //TODO: Validation for user input

    public class MainWindowModelView : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Property for storing of the currently displayed view model
        /// </summary>
        public BaseViewModel CurrentViewModel {
            get
            {
                return _currentViewModel;
            }
            set
            {
                    _currentViewModel = value;
                    RaisePropertyChange();
            }
        }
        private BaseViewModel _currentViewModel;

        /// <summary>
        /// ICommand for changing to Ping View
        /// </summary>
        public ICommand ChangeToPingView { get; set; }

        /// <summary>
        /// ICommand for changing to Port Scan View
        /// </summary>
        public ICommand ChangeToPortView { get; set; }


        /// <summary>
        /// ICommand for changing to Process Scan View
        /// </summary>
        public ICommand ChangeToMemoryView { get; set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// A list containing all ViewModels
        /// </summary>
        private readonly Dictionary<string, BaseViewModel> ViewModelsList = new Dictionary<string, BaseViewModel>()
            {
                {nameof(PingViewModel), new PingViewModel()},
                {nameof(PortScannerViewModel), new PortScannerViewModel()},
                {nameof(MemoryViewModel), new MemoryViewModel()},
            };

        #endregion


        public MainWindowModelView()
        {
            GoToPingView();

            ChangeToPingView = new RelayCommand<object>(GoToPingView);
            ChangeToPortView = new RelayCommand<object>(GoToPortView);
            ChangeToMemoryView = new RelayCommand<object>(GoToMemoryView);
        }

        #region Private Methods

        /// <summary>
        /// Change to the Ping View Model
        /// </summary>
        /// <param name="none"></param>
        private void GoToPingView(object none = null)
        {
            ViewModelsList.TryGetValue(nameof(PingViewModel), out BaseViewModel view);
            CurrentViewModel = view;
        }

        /// <summary>
        /// Change to the Port Scan view model
        /// </summary>
        /// <param name="none"></param>
        private void GoToPortView(object none = null)
        {
            ViewModelsList.TryGetValue(nameof(PortScannerViewModel), out BaseViewModel view);
            CurrentViewModel = view;
        }
        /// <summary>
        /// Change to the Port Scan view model
        /// </summary>
        /// <param name="none"></param>
        private void GoToMemoryView(object none = null)
        {

            
            ViewModelsList.TryGetValue(nameof(MemoryViewModel), out BaseViewModel view);
            CurrentViewModel = view;
        }



        #endregion
    }
}