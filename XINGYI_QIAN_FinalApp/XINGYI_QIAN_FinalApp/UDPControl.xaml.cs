using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace XINGYI_QIAN_FinalApp
{
    public sealed partial class UDPControl : UserControl
    {
        DatagramSocket receiveUDP = new DatagramSocket();
        App app = Application.Current as App;
        string sessionKey;
        DispatcherTimer t = new DispatcherTimer();
        Boolean isOn = false;
        int i;


        public UDPControl()
        {
            this.InitializeComponent();
            app.keyPair = CryptoClass.generateSessionKey();
            sessionKey = CryptoClass.getPreGeneratedPublicKey(app.keyPair);
            initSocket();
            t.Interval = TimeSpan.FromSeconds(1);
            t.Tick += T_Tick;
        }

        private void T_Tick(object sender, object e)
        {
            try
            {
                if (!app.partIP.Equals(""))
                {
                    receiveUDP.Dispose();
                    receiveUDP = new DatagramSocket();
                    initSocket();
                    HostName ipAdress = new HostName("255.255.255.255");
                    sendUDPData(sessionKey, ipAdress, new DatagramSocket());
                    t.Stop();
                    sendButton.Content = "Linked";
                    isOn = false;
                }
            }
            catch (Exception)
            {
                i++;
                if (i == 1)
                    sendButton.Content = "Connecting";
                if (i == 2)
                    sendButton.Content = "Connecting.";
                if (i == 3)
                    sendButton.Content = "Connecting..";
                if (i == 4)
                {
                    sendButton.Content = "Connecting...";
                    i = 0;
                }

                receiveUDP.Dispose();
                receiveUDP = new DatagramSocket();
                initSocket();
                HostName ipAdress = new HostName("255.255.255.255");
                sendUDPData(sessionKey, ipAdress, new DatagramSocket());
            }



        }

        private async void initSocket()
        //asynchronous
        {
            var hosts = NetworkInformation.GetHostNames();
            // 筛选无线或以太网
            var host = hosts.FirstOrDefault(h =>
            {
                bool isIpaddr = (h.Type == Windows.Networking.HostNameType.Ipv4) || (h.Type == Windows.Networking.HostNameType.Ipv6);
                // 如果不是IP地址表示的名称，则忽略
                if (isIpaddr == false)
                {
                    return false;
                }
                IPInformation ipinfo = h.IPInformation;
                // 71表示无线，6表示以太网
                if (ipinfo.NetworkAdapter.IanaInterfaceType == 71 || ipinfo.NetworkAdapter.IanaInterfaceType == 6)
                {
                    return true;
                }
                return false;
            });
            if (host != null)
            {
                app.localIP = host.DisplayName; //显示IP
            }

            try
            {
                receiveUDP.MessageReceived += ReceiveUDP_MessageReceived;
                await receiveUDP.BindServiceNameAsync("1677");
            }
            catch (Exception x)
            {
                ContentDialog dlg = new ContentDialog()
                {
                    Title = "Socket Error",
                    Content = "There was an error creating the sockets, the App will now close!",
                    PrimaryButtonText = "OK",
                    IsPrimaryButtonEnabled = true
                };
                await dlg.ShowAsync();
                Application.Current.Exit();
            }
        }

        private async void ReceiveUDP_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            Stream inStream = args.GetDataStream().AsStreamForRead();
            String ipFrom = args.RemoteAddress.ToString();
            if (ipFrom.Equals(app.localIP))
            { }
            else
                app.partIP = ipFrom;
            //data got
            StreamReader reader = new StreamReader(inStream);
            string msg = await reader.ReadLineAsync();
            if (ipFrom.Equals(app.partIP))
            {
                app.publicKey = msg;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, (() =>
                {
                    keyBox.Text = sessionKey;
                    LocalIPBox.Text = app.localIP;
                    PartIPBox.Text = app.partIP;
                    PartkeyBox.Text = app.publicKey;
                }));
            }
        }
        public async void sendUDPData(string msg, HostName ipAdress, DatagramSocket sender)
        {
            Stream outStream;//data sent
            string port1 = string.Empty;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, (() => { port1 = "1677"; }));
            outStream = (await sender.GetOutputStreamAsync(ipAdress, port1)).AsStreamForWrite();

            StreamWriter writer = new StreamWriter(outStream);
            await writer.WriteLineAsync(msg);
            await writer.FlushAsync();
            sender.Dispose();
        }


        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isOn)
            {
                sendButton.Content = "Connect ON";
                t.Start();
            }

            else
            {
                t.Stop();
                i = 0;
                sendButton.Content = "Connect OFF";
            }
            isOn = !isOn;
        }


    }
}
