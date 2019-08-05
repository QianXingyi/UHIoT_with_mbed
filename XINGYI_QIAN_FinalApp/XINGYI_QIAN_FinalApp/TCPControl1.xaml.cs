using System;
using System.IO;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace XINGYI_QIAN_FinalApp
{
    public sealed partial class TCPControl1 : UserControl
    {
        DispatcherTimer t = new DispatcherTimer();
        StreamSocketListener receiveSocket = new StreamSocketListener();
        StreamSocket sendSocket = new StreamSocket();
        string port = "1337";
        Boolean isRepeat = false;
        App app = Application.Current as App;
        int i = 0;
        public TCPControl1()
        {
            this.InitializeComponent();
            initSocket();
            t.Interval = TimeSpan.FromSeconds(1);
            t.Tick += T_Tick;

        }

        private void T_Tick(object sender, object e)
        {

            sendMbed();
        }

        public async void initSocket()
        //asynchronous
        {
            try
            {
                receiveSocket.ConnectionReceived += SocketLister_ConnectionRecevived;
                await receiveSocket.BindServiceNameAsync(port);
                //use the port
            }
            catch (Exception x)
            {
                ContentDialog dlg = new ContentDialog()
                {
                    Title = "TCP Error",
                    Content = "There was an error creating the socket, the APP will now close.",
                    PrimaryButtonText = "OK",
                    IsPrimaryButtonEnabled = true
                };
                await dlg.ShowAsync();
                Application.Current.Exit();
                //exit application

            }
        }
        public async void sendData(string recipientIP, string message)
        {
            i++;
            try
            {
                sendSocket = new StreamSocket();
                HostName recipient = new HostName(recipientIP);//reciver's IP
                await sendSocket.ConnectAsync(recipient, port);
                Stream streamOut = sendSocket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(streamOut);
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
                sendSocket.Dispose();
            }
            catch (Exception)
            {


            }
        }

        private void sendMbed()
        {
            string sendString = "$" + CryptoClass.AES_Ecrypt(app.s, app.passAES);
            sendData(app.partIP, sendString);
            showText.Text = "your IP" + app.localIP + "\npartner's IP"
                + app.partIP + "\nPartner's Key:"
                + app.passAESPart + "\n" + "LocalKey:" + app.passAES + "\n" + "You've sent " + i + " times";
        }

        private async void SocketLister_ConnectionRecevived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            //listen the port
            Stream inStream = args.Socket.InputStream.AsStreamForRead();
            //data got
            StreamReader reader = new StreamReader(inStream);
            string getString = await reader.ReadLineAsync();
            if (getString.Contains("@"))
            {
                app.passAESPart = CryptoClass.RSA_Decrypt(getString.Substring(1), app.keyPair);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, (() =>
                {
                    showText.Text = "your IP" + app.localIP + "\npartner's Ip" + app.partIP + "\nPartner's Key:" + app.passAESPart + "\n" + "LocalKey:" + app.passAES;
                }));
            }
            else if (getString.Contains("$"))
            {

                string getOString = CryptoClass.AES_Decrypt(getString.Substring(1), app.passAESPart);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, (() =>
                {


                    try
                    {
                        getText.Text = getOString;
                        string[] strInput = getOString.Split(',');
                        tempText.Text = strInput[0];
                        port1Text.Text = strInput[1];
                        port2Text.Text = strInput[2];
                        xText.Text = strInput[3];
                        yText.Text = strInput[4];
                        zText.Text = strInput[5];
                    }
                    catch (Exception)
                    {

                    }
                }));
            }

            //put message to UI
        }


        private void getBtn_Click(object sender, RoutedEventArgs e)
        {
            app.passAES = CryptoClass.GenerateRandomString();
            showText.Text = "your IP" + app.localIP + "\npartner's Ip" + app.partIP
                + "\nPartner's Key:"
                + app.passAESPart + "\n" + "LocalKey:" + app.passAES;
            string sendString = "@" + CryptoClass.RSA_Encrypt(app.passAES, app.publicKey);
            sendData(app.partIP, sendString);
        }


        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            sendMbed();
        }

        private void continueBtn_Click(object sender, RoutedEventArgs e)
        {
            isRepeat = !isRepeat;
            if (isRepeat)
            {
                t.Start();
                continueBtn.Content = "REPEATOR ON";
            }
            else
            {
                t.Stop();
                continueBtn.Content = "REPEATOR OFF";
            }
        }
    }
}
