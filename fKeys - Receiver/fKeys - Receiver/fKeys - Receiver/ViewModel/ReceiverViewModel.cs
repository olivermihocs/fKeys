using fKeys___Receiver.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fKeys___Receiver.ViewModel
{
    public class ReceiverViewModel : ViewModelBase
    {
        #region Private variables
        private ReceiverModel _model;
        private string _IPAddress;
        private string _port;
        #endregion

        #region

        public ReceiverViewModel(ReceiverModel model)
        {

            _model = model;
            _model.IPAddressChanged += new EventHandler<String>(Model_IPAddressChanged);
            _model.PortChanged += new EventHandler<int>(Model_PortChanged);
            _model.UpdateIP();
        }

        #endregion

        #region Properties
        public String IPAddress
        {
            get { return _IPAddress; }
            set
            {
                _IPAddress = value;
                OnPropertyChanged();
            }

        }

        public String Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged();
            }

        }

        #endregion

        #region Events
        private void Model_IPAddressChanged(object sender, String s)
        {
            IPAddress = s;
        }

        private void Model_PortChanged(object sender, int p)
        {
            Port = p.ToString();
        }
        #endregion



    }
}
