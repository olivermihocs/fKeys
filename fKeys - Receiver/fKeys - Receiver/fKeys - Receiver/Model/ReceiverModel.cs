using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WindowsInput;

namespace fKeys___Receiver.Model
{
    
    public class ReceiverModel
    {
        #region Private Variables
        private string _IPAddress;
        private int _port;
        private UDPListener _udpListener;

        private InputSimulator _inputSimulator;
        private KeyboardSimulator _keyboard;

        private int[] _modifierBits = new int[] { 8,4,2,1 };
        private WindowsInput.Native.VirtualKeyCode[] _modifierKeys = new WindowsInput.Native.VirtualKeyCode[] {
            WindowsInput.Native.VirtualKeyCode.LWIN,WindowsInput.Native.VirtualKeyCode.LSHIFT,
            WindowsInput.Native.VirtualKeyCode.LCONTROL,WindowsInput.Native.VirtualKeyCode.MENU};

        #endregion

        #region Constructor & Initialization
        public ReceiverModel()
        {
            _inputSimulator = new InputSimulator();
            _keyboard = new KeyboardSimulator(_inputSimulator);

            _port = 5005;
            this._udpListener = new UDPListener(_port);
            _udpListener.NewMessageReceived += new EventHandler<UDPMessageArgs>(UDP_NewMessageReceived);
            _udpListener.StartListener(64);
        }

        #endregion

        #region Internet Info / UDP

        public void UpdateIP()
        {
            _IPAddress = GetLocalIPAddress();
            OnIPAddressChanged();
            OnPortChanged();
        }

        private static bool IsNetworkAvailable()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        public void ChangeUDPListenState(bool val)
        {
            if (val && !_udpListener.isListening())
                _udpListener.StartListener(64);
            if(!val && _udpListener.isListening())
                _udpListener.StopListener();
        }

        public void ShutDown()
        {
            _udpListener.ShutDown();
        }
        #endregion

        #region Event Handlers
        public EventHandler<String> IPAddressChanged;
        public EventHandler<int> PortChanged;
        #endregion

        #region Events
        public void OnIPAddressChanged()
        {
            if (IPAddressChanged != null)
                IPAddressChanged.Invoke(this, _IPAddress);
        }

        public void OnPortChanged()
        {
            if (PortChanged != null)
                PortChanged.Invoke(this, _port);
        }

        public void UDP_NewMessageReceived(object sender, UDPMessageArgs e)
        {

            String str = Encoding.ASCII.GetString(e.data);
            try
            {
                SimulateKeyPress(str);
            }
            catch
            {
                Console.WriteLine("Error processing package");
            }
        }

        private void SimulateKeyPress(String packageStr)
        {
            int[] mod_vKey = Array.ConvertAll(packageStr.Split('_'), s => int.Parse(s));
            if (mod_vKey[0] != 0)
            {
                List<WindowsInput.Native.VirtualKeyCode> activeModifiers = new List<WindowsInput.Native.VirtualKeyCode> ();

                int i = 0;
                while (mod_vKey[0] > 0 && i < _modifierBits.Length)
                {
                    if (mod_vKey[0] >= _modifierBits[i])
                    {
                        activeModifiers.Add(_modifierKeys[i]);
                        mod_vKey[0] -= _modifierBits[i];
                    }
                    ++i;
                }
                _keyboard.ModifiedKeyStroke(activeModifiers, (WindowsInput.Native.VirtualKeyCode)mod_vKey[1]);
            }
            else
            {
                OnPortChanged();
                _keyboard.KeyPress((WindowsInput.Native.VirtualKeyCode)mod_vKey[1]);
            }
        }

        public int getIntValue(String str)
        {
            try
            {
                int numVal = Int16.Parse(str);
                return numVal;
            }
            catch (FormatException e)
            {
                return 0;
            }
        }

        #endregion
    }
}
