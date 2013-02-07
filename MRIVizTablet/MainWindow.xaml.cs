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
        //private string _ipAddress = "192.168.139.100";
        private int _port = 12345;

        private bool paused = false;


        #region iNetwork Methods

        private void InitializeConnection()
        {
            // connect to the server
            this._connection = new Connection(this._ipAddress, this._port);
            this._connection.Connected += new ConnectionEventHandler(OnConnected);
            this._connection.Start();

            this._connection.SendMessage(new Message("ae"));
        }

        private void sendPauseMsg()
        {
            this._connection.SendMessage(new Message("changeStateApp"));
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
                            int x = msg.GetIntField("x");
                            int y = msg.GetIntField("y");
                            setImageOnDisplay(index, x, y);
                            Console.WriteLine("Index Received: "+index + " X: "+ x + " Y:"+y);
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

        double imgD = 512;

        void setImageSizeByFactor(double f)
        {
            this.imgD = this.imgD * f;
            this.Dispatcher.Invoke(new Action(delegate()
            {
                image.Height = this.imgD;
                image.Width = this.imgD;
            }));
        }

        void setImageOnDisplay(int imageIndex, int x, int y)
        {
            if (!paused)
            {
                String imgUri = "MRIImages/IM-0001-0" + imageIndex + ".jpg";

                this.Dispatcher.Invoke(new Action(delegate()
                {
                    image.Source = new BitmapImage(new Uri(imgUri, UriKind.Relative));
                    double[] c = processCoordinates(x, y); 
                    scroll.ScrollToHorizontalOffset(c[0]);
                    scroll.ScrollToVerticalOffset(c[1]);
                }));
            }
        }

        double[] processCoordinates(int x, int y)
        {
            double[] coordinates = { 0.0, 0.0 };
            
            double tH = 2250;
            double tW = 3000;
            // processing X
            double imgXFactor = this.imgD / tW;
            coordinates[0] = imgXFactor * (double)x;
            // processing Y
            double imgYFactor = this.imgD / tH;
            coordinates[1] = scroll.ScrollableHeight - (imgYFactor * (double)y);
            Console.WriteLine(coordinates[0] + " "+coordinates[1] );
            return coordinates;
        }



        void changePauseState()
        {
            this.paused = this.paused == true ? false : true;
            this.Dispatcher.Invoke(new Action(delegate()
            {
                appStatus.Text = this.paused.ToString();
            }));
            sendPauseMsg();
        }

        #endregion

        private void changeAppState(object sender, TouchEventArgs e)
        {
            changePauseState();
        }

        private void changeImageSize(object sender, RoutedEventArgs e)
        {
            setImageSizeByFactor(0.5);
        }



    }
}
