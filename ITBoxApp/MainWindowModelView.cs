using PingData;
using SimplePingTool.HelperClasses;
using SimplePingTool.ViewModel;
using System.Collections.Generic;
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
        /// Close the application
        /// </summary>
        public ICommand Close { get; set; }


        #endregion

        #region Private Properties

        /// <summary>
        /// A list containing all ViewModels
        /// </summary>
        private readonly Dictionary<string, BaseViewModel> ViewModelsList = new Dictionary<string, BaseViewModel>()
            {
                {nameof(PingViewModel), new PingViewModel()}
            };

        #endregion


        public MainWindowModelView()
        {
            GoToPingView();

            ChangeToPingView = new RelayCommand<object>(GoToPingView);
            Close = new RelayCommand<object>((CloseCommand));

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
        /// Close the application
        /// </summary>
        /// <param name="none"></param>
        private void CloseCommand(object none = null)
        {
            App.Current.Shutdown();
        }

        #endregion
    }
}
