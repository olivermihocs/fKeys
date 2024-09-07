using fKeys___Receiver.Model;
using fKeys___Receiver.ViewModel;
using fKeys___Receiver.View;
using System.Windows;

namespace fKeys___Receiver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Private Variables
        private ReceiverModel _model;
        private ReceiverViewModel _viewModel;
        private MainWindow _view;
        #endregion

        #region Constructor & Initialization

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
            Exit += new ExitEventHandler(ShutDown);
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new ReceiverModel();
            _viewModel = new ReceiverViewModel(_model);

            //Setting up view
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();

        }
        #endregion

        #region Events

        public void ShutDown(object sender, ExitEventArgs e)
        {
            _model.ShutDown();
        }

        #endregion
    }
}
