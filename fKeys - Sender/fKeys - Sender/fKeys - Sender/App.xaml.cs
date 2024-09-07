using System;
using System.Windows;
using System.Windows.Interop;
using fKeys___Sender.Model;
using fKeys___Sender.View;
using fKeys___Sender.ViewModel;
using fKeys___Sender.HotkeyLogic;
using System.Windows.Input;

namespace fKeys___Sender
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Private variables

        private SenderModel _model;
        private SenderViewModel _viewModel;
        private MainWindow _view;
        private ChangeTargetWindow _ctw;
        private HotkeyHandler _hkh;

        #endregion

        #region Constructor & initialization

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            //Model
            _model = new SenderModel();
            
            //ViewModel
            _viewModel = new SenderViewModel(_model);
            _viewModel.OpenChangeWindow += new EventHandler(ViewModel_OpenChangeWindow);
            _viewModel.CloseChangeWindow += new EventHandler(ViewModel_CloseChangeWindow);
            _viewModel.HotkeyStateChanged += new EventHandler<bool>(ViewModel_HotkeyStateChanged);
            _viewModel.ActiveHotkeyChanged += new EventHandler<Hotkey>(ViewModel_ActiveHotkeyChanged);

            //View
            _view = new MainWindow();
            _view.PreviewKeyDown += new EventHandler<KeyEventArgs>(MainWindow_PreviewKeyDown);
            _view.DataContext = _viewModel;
            _view.Show();
            
            //HotkeyHandler
            _hkh = new HotkeyHandler(new WindowInteropHelper(_view).Handle);
            _hkh.KeyBindEvent += new EventHandler<Hotkey>(HotkeyHandler_HotkeyPressed);
            _hkh.NewHotkeyEvent += new EventHandler<Hotkey>(HotkeyHandler_NewHotkey);

            //Default state
            _viewModel.LoadDefaults();
        }
        #endregion

        #region Event handlers

        //Hotkey recording
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            _viewModel.HotkeyTextBox_PreviewKeyDown(sender, e);
        }

        //Open "Change Target IP" window
        private void ViewModel_OpenChangeWindow(object sender, EventArgs e)
        {
            _ctw = new ChangeTargetWindow();
            _ctw.DataContext = _viewModel;
            _ctw.ShowDialog();
        }

        //Close "Change Target IP" window
        private void ViewModel_CloseChangeWindow(object sender, EventArgs e)
        {
            _ctw.Close();
        }

        //Enable/Disable hotkey
        private void ViewModel_HotkeyStateChanged(object sender, bool newVal)
        {
            _hkh.ChangeHotkeyState(newVal);
        }

        //Change active hotkey
        private void ViewModel_ActiveHotkeyChanged(object sender, Hotkey Hotkey)
        {
            _hkh.ChangeActiveHotkey(Hotkey);
        }

        //Hotkey press
        private void HotkeyHandler_HotkeyPressed(object sender, Hotkey Hotkey)
        {
            if (Hotkey == null) return;
            _model.SendKeyBind(Hotkey.GetVirtualModifiers().ToString()+"_"+Hotkey.GetVirtualKey().ToString());
        }

        //Save new hotkey
        private void HotkeyHandler_NewHotkey(object sender, Hotkey Hotkey)
        {
            if (Hotkey == null) return;

            uint virtualMods = Hotkey.GetVirtualModifiers();
            int vk = Hotkey.GetVirtualKey();
            _model.SaveHotkey(Hotkey.GetVirtualModifiers().ToString() + "_" + ((int)Hotkey.Key).ToString());

        }

        #endregion

    }
}
