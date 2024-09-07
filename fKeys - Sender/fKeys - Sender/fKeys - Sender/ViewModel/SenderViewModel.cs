using fKeys___Sender.Model;
using fKeys___Sender.HotkeyLogic;
using System;
using System.Windows.Input;

namespace fKeys___Sender.ViewModel
{
    
    public class SenderViewModel : ViewModelBase
    {
        #region Private variables

        private SenderModel _model;
        private string _localAddress;
        private string _targetAddress;
        private string _newAddress;
        private string _hotkeyStateText;
        private bool _hotkeyState;

        #endregion

        #region Initialization

        public SenderViewModel(SenderModel model)
        {
            
            _model = model;

            _model.TargetAddressChanged += new EventHandler<String>(Model_TargetAddressChanged);
            _model.LocalAddressChanged += new EventHandler<String>(Model_LocalAddressChanged);
            ChangeTargetAddressCommand = new DelegateCommand(x => OnChangeTargetAddress());
            OpenChangeWindowCommand = new DelegateCommand(x =>OnOpenChangeWindow());
            CloseChangeWindowCommand = new DelegateCommand(x => OnCloseChangeWindow());
            ChangeHotkeyStateCommand = new DelegateCommand(x => OnHotkeyStateChanged());

            SetHotkeyStateProperties(false);
        }

        //Loads application default state for view
        public void LoadDefaults()
        {
            _model.UpdateIP();
            try
            {
                string[] mvk = _model.LoadHotkey().Split("_");
                Hotkey hk = new((Key)(int.Parse(mvk[1])), (ModifierKeys)int.Parse(mvk[0]));

                if (hk == null || hk.Key==null || hk.Modifiers==null)
                {
                    hk = null;
                }
                else
                {
                    ActiveHotkey = hk;
                    OnActiveHotkeyChanged();
                }
            }
            catch
            {
                Console.WriteLine("Couldn't load saved Hotkey");
            }
            NewAddress = TargetAddress;
        }

        #endregion

        #region Properties

        //Local machine IP address
        public String LocalAddress
        {
            get { return _localAddress; }
            set
            {
                _localAddress = value;
                OnPropertyChanged();
            }

        }

        //Target machine IP address
        public String TargetAddress
        {
            get { return _targetAddress; }
            set
            {
                _targetAddress = value;
                OnPropertyChanged();
            }

        }

        //New target machine IP address field
        public String NewAddress
        {
            get { return _newAddress; }
            set
            {
                _newAddress = value;
                OnPropertyChanged();
            }

        }

        //Setter for changing active HotkeyState (Enabled or Disab
        private void SetHotkeyStateProperties(bool newVal)
        {
            _hotkeyState = newVal;
            if (newVal)
                HotkeyStateText = "Disable Hotkey";
            else
                HotkeyStateText = "Enable Hotkey";

        }

        //Text that displays on toggle button
        public String HotkeyStateText
        {
            get { return _hotkeyStateText; }
            set
            {
                _hotkeyStateText = value;
                OnPropertyChanged();
            }
        }

        //Currently active hotkey
        private Hotkey _activeHotkey;
        public Hotkey ActiveHotkey
        {
            get { return _activeHotkey; }
            set
            {
                _activeHotkey = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        //Hotkey turn ON/OFF toggle event
        private void OnHotkeyStateChanged()
        {
            SetHotkeyStateProperties(!_hotkeyState);
            if (HotkeyStateChanged != null)
                HotkeyStateChanged(this, _hotkeyState);
        }

        //Target machine IP address changed event
        private void OnChangeTargetAddress()
        {
            _model.UpdateTargetAddress(NewAddress);
            OnCloseChangeWindow();
        }

        //Open change target IP address window event
        private void OnOpenChangeWindow()
        {
            if (OpenChangeWindow != null)
                OpenChangeWindow(this,EventArgs.Empty);
        }

        //Change target IP address window closing event
        private void OnCloseChangeWindow()
        {
            if (CloseChangeWindow != null)
                CloseChangeWindow(this, EventArgs.Empty);
        }

        //New active hotkey event
        public void OnActiveHotkeyChanged()
        {
            if (ActiveHotkeyChanged != null)
                ActiveHotkeyChanged(this, ActiveHotkey);
        }

        #endregion

        #region Commands

        public DelegateCommand ChangeTargetAddressCommand { get; private set; }
        public DelegateCommand ChangeHotkeyStateCommand { get; private set; }
        public DelegateCommand OpenChangeWindowCommand { get; private set; }
        public DelegateCommand CloseChangeWindowCommand { get; private set; }

        #endregion

        #region Event Handlers

        public event EventHandler OpenChangeWindow;
        public event EventHandler CloseChangeWindow;
        public event EventHandler<bool> HotkeyStateChanged;
        public EventHandler<Hotkey> ActiveHotkeyChanged;

        //Event handler for changing target machine IP address
        private void Model_TargetAddressChanged(object sender, String ip)
        {
            TargetAddress = ip;
        }

        //Event handler for changing local machine IP address
        private void Model_LocalAddressChanged(object sender, String ip)
        {
            LocalAddress = ip;
        }

        //Event handler for hotkey recording
        public void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Don't let the event pass further
            // because we don't want standard textbox shortcuts working
            e.Handled = true;

            // Get modifiers and Key data
            var modifiers = Keyboard.Modifiers;
            var Key = e.Key;

            // When Alt is pressed, SystemKey is used instead
            if (Key == Key.System)
            {
                Key = e.SystemKey;
            }

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None &&
                (Key == Key.Delete || Key == Key.Back || Key == Key.Escape))
            {
                ActiveHotkey = null;
                return;
            }

            // If no actual Key was pressed - return
            if (Key == Key.LeftCtrl ||
                Key == Key.RightCtrl ||
                Key == Key.LeftAlt ||
                Key == Key.RightAlt ||
                Key == Key.LeftShift ||
                Key == Key.RightShift ||
                Key == Key.LWin ||
                Key == Key.RWin ||
                Key == Key.Clear ||
                Key == Key.OemClear ||
                Key == Key.Apps)
            {
                return;
            }

            // Update the value
            ActiveHotkey = new Hotkey(Key, modifiers);

            //Kill focus
            Keyboard.ClearFocus();

            //Notify logic
            OnActiveHotkeyChanged();
        }

        #endregion
    }
}
