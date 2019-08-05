using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace XINGYI_QIAN_FinalApp
{
    public sealed partial class SerialControl : UserControl
    {
        private SerialDevice serialPort = null;
        DataReader dataReaderObject = null;
        DataWriter dataWriteObject = null;
        private CancellationTokenSource ReadCancellationTokenSource;
        String mbedResived;
        App app = Application.Current as App;
        public SerialControl()
        {
            this.InitializeComponent();
            GetAvailablePort();
        }
        private async void GetAvailablePort()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelectorFromUsbVidPid(0x0D28, 0x0204));
                serialPort = await SerialDevice.FromIdAsync(dis[0].Id);
                try
                {
                    // Configure serial settings
                    serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                    serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    serialPort.BaudRate = 9600;
                    serialPort.Parity = SerialParity.None;
                    serialPort.StopBits = SerialStopBitCount.One;
                    serialPort.DataBits = 8;
                    serialPort.Handshake = SerialHandshake.None;
                    ReadCancellationTokenSource = new CancellationTokenSource();
                    Listen();
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }
        }
        private async void Listen()
        {
            try
            {
                if (serialPort != null)
                {
                    dataReaderObject = new DataReader(serialPort.InputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
        }


        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;

            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                mbedResived = dataReaderObject.ReadString(bytesRead);
                rcvdText.Text = mbedResived;
                app.s = mbedResived;
                string[] strInput = mbedResived.Split(',');
                try
                {
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
            }
        }

        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleSwitch.IsOn)
            {
                getGrid.Visibility = Visibility.Collapsed;
                sendGrid.Visibility = Visibility.Visible;
            }
            else {
                getGrid.Visibility = Visibility.Visible;
                sendGrid.Visibility = Visibility.Collapsed;
            }
        }
       
        private async Task WriteAsync(char c)
        {
            Task<UInt32> storeAsyncTask;

            if (sendBox.Text.Length != 0)
            {
                // Load the text from the sendText input text box to the dataWriter object
                dataWriteObject.WriteString(c+"");

                // Launch an async task to complete the write operation
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();

                UInt32 bytesWritten = await storeAsyncTask;
                if (bytesWritten > 0)
                {
                       stateBox.Text = "Success!";
                }
               
            }
            else
            {
                stateBox.Text = "Please try again!";
            }
        }

    
        private void sendTextButton_Click(object sender, RoutedEventArgs e)
        {
            methodSend('$');
            char[] chars = sendBox.Text.ToCharArray();
            for (int i = 0; i < sendBox.Text.Length; i++)
            {
                methodSend(chars[i]);
            }
        }
        private async void methodSend(char c)
        {
            try
            {
                if (serialPort != null)
                {
                    // Create the DataWriter object and attach to OutputStream
                    dataWriteObject = new DataWriter(serialPort.OutputStream);
                    
                    //Launch the WriteAsync task to perform the write
                        await WriteAsync(c);
                }
                else
                {
                    sendBox.Text = "Please try again!";
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                // Cleanup once complete
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }
    }
}
