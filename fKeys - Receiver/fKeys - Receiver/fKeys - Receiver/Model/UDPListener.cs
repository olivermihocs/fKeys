using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fKeys___Receiver.Model
{
    class UDPListener
    {
        private int _port;
        private volatile bool _listening;
        Thread _listeningThread;
        UdpClient _listener;
        public event EventHandler<UDPMessageArgs> NewMessageReceived;


        public bool isListening()
        {
            return _listening;
        }
        //constructor
        public UDPListener(int port)
        {
            this._port = port;
            this._listening = false;
        }

        public void StartListener(int exceptedMessageLength)
        {
            if (!this._listening)
            {
                _listeningThread = new Thread(ListenForUDPPackages);
                this._listening = true;
                _listeningThread.Start();  
            }
        }

        public void StopListener()
        {
            this._listening = false;
        }

        public void ShutDown()
        {
            StopListener();
            if (_listener != null)
                _listener.Close();
        }

        public void ListenForUDPPackages()
        {
            _listener = null;
            try
            {
                _listener = new UdpClient(_port);
            }
            catch (SocketException)
            {
                //do nothing
            }

            if (_listener != null)
            {
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, _port);

                try
                {
                    while (this._listening)
                    {
                        byte[] bytes = _listener.Receive(ref groupEP);

                        //raise event                        
                        OnNewMessageReceived(bytes);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    _listener.Close();
                }
            }
        }
        public void OnNewMessageReceived(byte[] bytes)
        {
            if (NewMessageReceived != null)
                NewMessageReceived.Invoke(this, new UDPMessageArgs(bytes));
        }
    }

    public class UDPMessageArgs : EventArgs
    {
        public byte[] data { get; set; }

        public UDPMessageArgs(byte[] newData)
        {
            data = newData;
        }
    }
}
