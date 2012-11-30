using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GroupLab.iNetwork;
using GroupLab.iNetwork.Tcp;

namespace MRIVizTablet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeConnection();
            InitializeComponent();
        }


        // iNetworking initialization
        private Connection _connection;
        // Tablet ipAddress
        //private string _ipAddress = "192.168.0.112";
        private string _ipAddress = "136.159.7.53";
        private int _port = 12345 ;


        #region iNetwork Methods

        private void InitializeConnection()
        {
            // connect to the server
            this._connection = new Connection(this._ipAddress, this._port);
            this._connection.Connected += new ConnectionEventHandler(OnConnected);
            this._connection.Start();

            this._connection.SendMessage(new Message("ae"));
        }

        void OnConnected(object sender, ConnectionEventArgs e)
        {
            this._connection.MessageReceived += new ConnectionMessageEventHandler(OnMessageReceived);
        }

        private void OnMessageReceived(object sender, Message msg)
        {
            try
            {
                if (msg != null)
                {
                    switch (msg.Name)
                    {
                        default:
                            // don't do anything
                            break;
                        case "ChangeImg":
                            int index = msg.GetIntField("index");
                            setImageOnDisplay(index);
                            Console.WriteLine(msg.ToString());
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.StackTrace);
            }

        }


        #endregion
      
        #region Display-related functions
        void setImageOnDisplay(int imageIndex)
        {
            String imgUri = "MRIImages/IM-0001-0" + imageIndex + ".jpg";

            this.Dispatcher.Invoke(new Action(delegate()
            {
                image.Source = new BitmapImage(new Uri(imgUri, UriKind.Relative));
            }));
        }

        #endregion



    }
}
