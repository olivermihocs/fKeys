using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace fKeys___Sender.Model
{

    public class SenderModel
    {
        #region Private Variables
        private const string _HotkeyPath = @"keybinds.cfg";
        private const string _targetPath = @"target.cfg";
        private string _localAddress;
        private string _targetAddress;
        private int _targetPort = 5005;

        #endregion

        #region Constructor & Initialization

        public SenderModel()
        {
            _targetAddress = LoadTargetAddress();
        }

        #endregion


        #region Send Keybind via UDP

        //Send string via UDP to current IP + PORT
        public bool SendKeyBind(string bindStr)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            IPAddress serverAddr = IPAddress.Parse(_targetAddress);

            IPEndPoint endPoint = new IPEndPoint(serverAddr, _targetPort);

            byte[] send_buffer = Encoding.ASCII.GetBytes(bindStr);
            try
            {
                sock.SendTo(send_buffer, endPoint);
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
        #endregion

        #region File Handling

        //Creates an empty file if it doesn't already exists with name on path
        public static void CreateEmptyFile(String path)
        {
            if (!File.Exists(path))
            {
                using (File.Create(path));
            }
        }

        //Used for saving target machine IP address
        public void SaveTargetAddress()
        {
            using (FileStream fs = new FileStream(_targetPath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(_targetAddress);
            }
        }

        //Used for loading target machine IP address
        public static string LoadTargetAddress()
        {
            CreateEmptyFile(_targetPath);
            string ret = "";
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(_targetPath))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                var str = streamReader.ReadLine();
                if (str != null)
                    ret = str;
            }
            return ret;
        }

        //Used for saving a hotkey in string form
        public bool SaveHotkey(String str)
        {
            CreateEmptyFile(_HotkeyPath);
            //Add Keybind
            using (FileStream fs = new FileStream(_HotkeyPath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(str);
            }
            return true;
        }

        //Used for loading a hotkey in string form
        public string LoadHotkey()
        {
            CreateEmptyFile(_HotkeyPath);
            string ret = "";
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(_HotkeyPath))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                var str = streamReader.ReadLine();
                if (str != null)
                    ret = str;
            }
            return ret;
        }
        #endregion


        #region Getters and Setters for Internet Information


        //Updates target machine IP address
        public void UpdateTargetAddress(string newIP)
        {
            _targetAddress = newIP;
            SaveTargetAddress();
            OnTargetAddressChanged();
        }

        //Updates local machine IP address
        public void UpdateIP()
        {
            _localAddress = GetLocalIPAddress();
            OnLocalAddressChanged();
            OnTargetAddressChanged();
        }

        //Checks for available network connection
        private static bool IsNetworkAvailable()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        //Return local machine IP address
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

        #endregion

        #region Event Handlers
        public EventHandler<String> TargetAddressChanged;
        public EventHandler<String> LocalAddressChanged;
        #endregion

        #region Events

        //Target machine IP address changed event
        public void OnTargetAddressChanged()
        {
            if (TargetAddressChanged != null)
                TargetAddressChanged.Invoke(this, _targetAddress);
        }
        
        //Local machine IP address changed event
        public void OnLocalAddressChanged()
        {
            if (LocalAddressChanged != null)
                LocalAddressChanged.Invoke(this, _localAddress);
        }

        #endregion
    }

    

}


