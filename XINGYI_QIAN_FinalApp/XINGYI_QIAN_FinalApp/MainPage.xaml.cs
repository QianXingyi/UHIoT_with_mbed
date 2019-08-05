using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace XINGYI_QIAN_FinalApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TCPControl1 tcp = new TCPControl1();
        SerialControl serial = new SerialControl();
        UDPControl udp = new UDPControl();
        int temp = 1;
        public MainPage()
        {
            this.InitializeComponent();
            var view = ApplicationView.GetForCurrentView();

            // active  
            view.TitleBar.BackgroundColor = Color.FromArgb(255, 0, 122, 204);
            view.TitleBar.ForegroundColor = Colors.White;
            view.TitleBar.InactiveBackgroundColor = Color.FromArgb(255, 0, 122, 204);
            view.TitleBar.InactiveForegroundColor = Colors.Black;
            // button  
            view.TitleBar.ButtonBackgroundColor = Color.FromArgb(255, 0, 122, 204);
            view.TitleBar.ButtonForegroundColor = Colors.White;
            view.TitleBar.ButtonHoverBackgroundColor = Colors.LightBlue;
            view.TitleBar.ButtonHoverForegroundColor = Colors.White;
            view.TitleBar.ButtonPressedBackgroundColor = Colors.LightBlue;
            view.TitleBar.ButtonPressedForegroundColor = Colors.White;
            view.TitleBar.ButtonInactiveBackgroundColor = Colors.DarkGray;
            view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
        }

        private void expandButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }



        private void mbedClick(object sender, RoutedEventArgs e)
        {
            temp = 1;
            cmdBar.Children.Clear();
            splBar.Children.Clear();
            Grid.SetColumn(serial, 0);
            Grid.SetRow(serial, 0);
            cmdBar.Children.Clear();
            cmdBar.Children.Add(serial);
        }

        private void mbedClick_2(object sender, RoutedEventArgs e)
        {
            temp = 1;
            cmdBar.Children.Clear();
            splBar.Children.Clear();
            Grid.SetColumn(serial, 0);
            Grid.SetRow(serial, 0);
            splBar.Children.Clear();
            splBar.Children.Add(serial);
        }


        private void linkClick(object sender, RoutedEventArgs e)
        {
            temp = 2;
            cmdBar.Children.Clear();
            splBar.Children.Clear();
            Grid.SetColumn(udp, 0);
            Grid.SetRow(udp, 0);
            cmdBar.Children.Clear();
            cmdBar.Children.Add(udp);
        }

        private void linkClick_2(object sender, RoutedEventArgs e)
        {
            temp = 2;
            cmdBar.Children.Clear();
            splBar.Children.Clear();
            Grid.SetColumn(udp, 0);
            Grid.SetRow(udp, 0);
            Grid.SetColumn(tcp, 1);
            Grid.SetRow(tcp, 0);
            splBar.Children.Clear();
            splBar.Children.Add(udp);
            splBar.Children.Add(tcp);
        }


        private void dataClick(object sender, RoutedEventArgs e)
        {
            temp = 3;
            cmdBar.Children.Clear();
            splBar.Children.Clear();
            Grid.SetColumn(tcp, 0);
            Grid.SetRow(tcp, 0);
            cmdBar.Children.Clear();
            cmdBar.Children.Add(tcp);
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Bounds.Width >= 800)
            {
                cmdBar.Children.Clear();
                splBar.Children.Clear();
                Grid.SetColumn(serial, 0);
                Grid.SetRow(serial, 0);
                splBar.Children.Clear();
                splBar.Children.Add(serial);
                if (Window.Current.Bounds.Width >= 1200)
                {
                    mySplitView.IsPaneOpen = true;
                }
                else
                    mySplitView.IsPaneOpen = false;
            }
            else
            {
                mySplitView.IsPaneOpen = false;
                cmdBar.Children.Clear();
                splBar.Children.Clear();
                Grid.SetColumn(serial, 0);
                Grid.SetRow(serial, 0);
                cmdBar.Children.Clear();
                cmdBar.Children.Add(serial);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Width >= 800)
            {
                cmdBar.Children.Clear();
                splBar.Children.Clear();
                Grid.SetColumn(serial, 0);
                Grid.SetRow(serial, 0);
                splBar.Children.Clear();
                switch (temp)
                {
                    case 1:
                        splBar.Children.Add(serial);
                        break;

                    case 2:

                    case 3:
                        temp = 2;
                        Grid.SetColumn(udp, 0);
                        Grid.SetRow(udp, 0);
                        Grid.SetColumn(tcp, 1);
                        Grid.SetRow(tcp, 0);
                        splBar.Children.Clear();
                        splBar.Children.Add(udp);
                        splBar.Children.Add(tcp);
                        break;
                }
                if (Window.Current.Bounds.Width >= 1200)
                {
                    mySplitView.IsPaneOpen = true;
                }
                else
                    mySplitView.IsPaneOpen = false;
            }
            else
            {
                mySplitView.IsPaneOpen = false;
                cmdBar.Children.Clear();
                splBar.Children.Clear();
                Grid.SetColumn(serial, 0);
                Grid.SetRow(serial, 0);
                cmdBar.Children.Clear();
                switch (temp)
                {
                    case 1:
                        cmdBar.Children.Add(serial);
                        break;
                    case 2:
                        cmdBar.Children.Add(udp);
                        break;
                    case 3:
                        cmdBar.Children.Add(tcp);
                        break;
                }
            }
        }
    }
}
