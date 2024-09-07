using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace fKeys___Sender.HotkeyLogic
{

    /// <summary>
    /// Class responsible for Handling hotkey inputs
    /// </summary>
    public class HotkeyHandler
    {

        //Imports for registering and unregistering a global hotkey
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        //Hotkey ID and current active hotkey
        private const int Hotkey_ID = 9000;
        private Hotkey _currentHotkey;


        //Handle for our window, this must be updated when the class gets initialized
        private IntPtr _windowHandle;
        private HwndSource _source;

        //Current state for hotkey registration (Active/Inactive)
        private Boolean _registerState = false;


        //Constructor
        public HotkeyHandler(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);
        }


        //Hook for window, listen for the keybind with WM_Hotkey
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_Hotkey = 0x0312;
            switch (msg)
            {
                case WM_Hotkey:
                    switch (wParam.ToInt32())
                    {
                        case Hotkey_ID:
                            int vKey = (((int)lParam >> 16) & 0xFFFF);
                            if (vKey == _currentHotkey.GetVirtualKey())
                            {
                                OnKeyBindEvent(vKey);
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }


        //Shutdown handler, remove hooks and unregister keybinds
        public void ShutDown()
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, Hotkey_ID);
        }


        //Change hotkey state (Unregister and Register hotkey)
        public void ChangeHotkeyState(bool newVal)
        {
            _registerState = newVal;
            if (_currentHotkey != null && newVal)
                RegisterHotKey(_windowHandle, Hotkey_ID, _currentHotkey.GetVirtualModifiers(), (uint)_currentHotkey.GetVirtualKey());
            else
            {
                UnregisterHotKey(_windowHandle, Hotkey_ID);
            }

        }

        //Update current active hotkey, unregister old hotkey and register new if we are in active state
        public void ChangeActiveHotkey(Hotkey Hotkey)
        {
            _currentHotkey = Hotkey;
            OnNewHotkeyEvent(_currentHotkey);
            UnregisterHotKey(_windowHandle, Hotkey_ID);
            if (_registerState)
                RegisterHotKey(_windowHandle, Hotkey_ID, _currentHotkey.GetVirtualModifiers(), (uint)_currentHotkey.GetVirtualKey());
        }

        
        //Eventhandlers
        public event EventHandler<Hotkey> KeyBindEvent;
        public event EventHandler<Hotkey> NewHotkeyEvent;


        #region Events

        //Current hotkey was pressed event
        private void OnKeyBindEvent(int vKey)
        {
            if (KeyBindEvent != null)
                KeyBindEvent(this, _currentHotkey);
        }

        //Current has been updated event
        private void OnNewHotkeyEvent(Hotkey Hotkey)
        {
            if (NewHotkeyEvent != null)
                NewHotkeyEvent(this, _currentHotkey);
        }

        #endregion


    }
}
