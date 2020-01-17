using System.Windows;

namespace SimplePingTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow app = new MainWindow();
            MainWindowModelView context = new MainWindowModelView();
            app.DataContext = context;
            app.Show();
        }
    }
}
