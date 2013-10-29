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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Expression.Encoder.Devices;
using WebcamControl;
using System.IO;
using System.Drawing.Imaging;


namespace KrausRGA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();                    

            // Bind the Video and Audio device properties of the
            // Webcam control to the SelectedValue property of 
            // the necessary ComboBox.
            Binding bndg_1 = new Binding("SelectedValue");
            bndg_1.Source = VidDvcsComboBox;
            WebCamCtrl.SetBinding(Webcam.VideoDeviceProperty, bndg_1);

            Binding bndg_2 = new Binding("SelectedValue");
            bndg_2.Source = AudDvcsComboBox;
            WebCamCtrl.SetBinding(Webcam.AudioDeviceProperty, bndg_2);

            // Create directory for saving video files.
            string vidPath = @"C:\VideoClips";

            if (Directory.Exists(vidPath) == false)
            {
                Directory.CreateDirectory(vidPath);
            }

            // Create directory for saving image files.
            string imgPath = @"C:\WebcamSnapshots";

            if (Directory.Exists(imgPath) == false)
            {
                Directory.CreateDirectory(imgPath);
            }

            // Set some properties of the Webcam control
            WebCamCtrl.VideoDirectory = vidPath;
            WebCamCtrl.VidFormat = VideoFormat.mp4;

            WebCamCtrl.ImageDirectory = imgPath;
            WebCamCtrl.PictureFormat = ImageFormat.Jpeg;
            
            WebCamCtrl.FrameRate = 30;
            WebCamCtrl.FrameSize = new System.Drawing.Size(320, 240);

            // Find a/v devices connected to the machine.
            FindDevices();

            VidDvcsComboBox.SelectedIndex = 0;
            AudDvcsComboBox.SelectedIndex = 0;
        }

        private void FindDevices()
        {
            var vidDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            var audDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);

            int CameraNumber = KrausRGA.Properties.Settings.Default.CameraNumber;
            int i = 0;
            foreach (EncoderDevice dvc in vidDevices)
            {
                if (i==CameraNumber)
                VidDvcsComboBox.Items.Add(dvc.Name);
                i++;
            }

            foreach (EncoderDevice dvc in audDevices)
            {
                AudDvcsComboBox.Items.Add(dvc.Name);
            }

        }

        private void SnapshotButton_Click(object sender, RoutedEventArgs e)
        {
            // Take snapshot of webcam image.
            WebCamCtrl.TakeSnapshot();

            int panelWidth = Convert.ToInt32(WebCamCtrl.ActualWidth);
            int panelHeight = Convert.ToInt32(WebCamCtrl.ActualHeight);


            Point pt = WebCamCtrl.TranslatePoint(new Point(0, 0), WebCamCtrl);
            Point pnlPnt = WebCamCtrl.PointToScreen(pt);
            System.Drawing.Point pnl = new System.Drawing.Point(Convert.ToInt32(pnlPnt.X), Convert.ToInt32(pnlPnt.Y));
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(panelWidth, panelHeight);
            System.Drawing.Graphics gcs = System.Drawing.Graphics.FromImage(bmp);
            gcs.CopyFromScreen(pnl, System.Drawing.Point.Empty, new System.Drawing.Size(panelWidth, panelHeight));

            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero,
                System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));

            Image img = new Image();
            img.Height = 165;
            img.Width = 200;
            img.Stretch = Stretch.Fill;
            img.Name = "KrausGRA" + DateTime.Now.ToString("hhmmsstt");
            img.Source = bs;
            img.Margin = new Thickness(2.0);
            spPhotos.Children.Add(img);
            img.Focus();
            sclPh.ScrollToRightEnd();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Display webcam video on control.
            bdrStop.Visibility = System.Windows.Visibility.Hidden;
            bdrCapture.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnStartCapture_Click(object sender, RoutedEventArgs e)
        {
            
            bdrCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStartCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStop.Visibility = System.Windows.Visibility.Visible;

            
            WebCamCtrl.StartCapture(); 
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            WebCamCtrl.StopCapture();

            bdrCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStartCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStop.Visibility = System.Windows.Visibility.Hidden;

          
        }

        private void btnOpenCamera_Click(object sender, RoutedEventArgs e)
        {
            bdrCamera.Visibility = System.Windows.Visibility.Visible;

        }

        private void brnCloseCamera_Click(object sender, RoutedEventArgs e)
        {
            WebCamCtrl.StopCapture();
            WebCamCtrl.Dispose();
            bdrCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStartCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStop.Visibility = System.Windows.Visibility.Hidden;
            bdrCamera.Visibility = System.Windows.Visibility.Hidden;
            removeStackPanelChild(spPhotos);
        }

        /// <summary>
        /// remove child controles from Stackpanel
        /// </summary>
        /// <param name="stackPacnel">
        /// StackPanel UI Element
        /// </param>
        private void removeStackPanelChild(StackPanel stackPacnel)
        {
            try
            {
                while (stackPacnel.Children.Count>0)
                {
                    stackPacnel.Children.RemoveAt(stackPacnel.Children.Count-1);
                }
            }
            catch (Exception)
            {}
        }
    }
}
