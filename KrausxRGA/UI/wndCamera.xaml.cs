using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using KrausRGA;
using System.Windows.Threading;
using KrausRGA.Barcode;
namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndCamera.xaml
    /// </summary>
    public partial class wndCamera : Window
    {
        DispatcherTimer CaptureTime;
        int TimerTickCount = 0;
        public wndCamera()
        {
            InitializeComponent();
            tbInfoText.Text = "";
            Views.clGlobal.BarcodeValueFound = "";
            Views.clGlobal.lsImageList = new List<string>();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(CaptureTime.IsEnabled)
            CaptureTime.Stop();
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Thread.Sleep(500);
            try
            {
                player.Dispose();
            }
            catch (Exception)
            { }
            finally
            {
                this.Close();
            }
        }

        private void Image_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            try
            {
                cvsControls.Visibility = System.Windows.Visibility.Hidden;
                bdrPreview.Visibility = System.Windows.Visibility.Hidden;
                String LastPhotoName = "";
                Barcode.Camera.TakePhoto(cvsCamera);
                LastPhotoName = Barcode.Camera.LastPhotoName();
                Views.clGlobal.lsImageList.Add(LastPhotoName);

                Barcode.Camera.Rotate90Degree(KrausRGA.Properties.Settings.Default.DrivePath + "\\" + LastPhotoName);
                tbInfoText.Text = "Image captured. " + LastPhotoName + "'";
                imgPreview.Source = new BitmapImage(new Uri(KrausRGA.Properties.Settings.Default.DrivePath + "\\" + LastPhotoName));
                bdrPreview.Visibility = System.Windows.Visibility.Visible;
                cvsControls.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception)
            {}
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            CaptureTime = new DispatcherTimer();
            CaptureTime.Interval = new TimeSpan(0, 0, 0, 0, 500);
            CaptureTime.Tick += CaptureTime_Tick;

        }

        private void CaptureTime_Tick(object sender, EventArgs e)
        {
            cvsControls.Visibility = System.Windows.Visibility.Hidden;
            bdrMarks.Visibility = System.Windows.Visibility.Visible;
            TimerTickCount++;

            this.Dispatcher.Invoke(new Action(() =>
            {

                if (TimerTickCount== 1)
                    bdrScanner.Visibility = System.Windows.Visibility.Visible;

                if (TimerTickCount == 10)
                    bdrScanner.Visibility = System.Windows.Visibility.Hidden;

                if (TimerTickCount == 1)
                    tbInfoText.Text = "Scanning Barcode.";
                tbInfoText.Text = tbInfoText.Text + ".";

            }));

            String _barcodeValue = Barcode.BarcodeRead.Read(cvsCamera);
            if (_barcodeValue != "")
            {
                CaptureTime.Stop();
                Views.clGlobal.FBCode.BarcodeValue = _barcodeValue;
                try
                {
                    player.Dispose();
                }
                catch (Exception)
                { }
                bdrMarks.Visibility = System.Windows.Visibility.Visible;
                this.Close();
            }
        }

        private void Image_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            CaptureTime.Start();
        }

        
    }
}
