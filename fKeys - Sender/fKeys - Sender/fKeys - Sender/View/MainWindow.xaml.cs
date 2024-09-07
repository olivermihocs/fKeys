using System;
using System.Windows;
using System.Windows.Input;
namespace fKeys___Sender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
        }


        #region Hotkey recording
        
        public EventHandler<KeyEventArgs> PreviewKeyDown;

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if(PreviewKeyDown !=null)
                PreviewKeyDown(this, e);
        }
        #endregion
    }
}
