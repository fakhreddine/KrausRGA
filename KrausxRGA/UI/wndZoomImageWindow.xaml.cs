using KrausRGA.Views;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndZoomImageWindow.xaml
    /// </summary>
    public partial class wndZoomImageWindow : Window
    {
        public wndZoomImageWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            imgzoom.Source = clGlobal.Zoomimages.Source;

           // Image img = clGlobal.Zoomimages.Source;
        }

        private void BtnSaveImage_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

         //   System.IO.FileStream fs = (System.IO.FileStream)dlg.OpenFile();
            //dlg.FileName = imgzoom.  // Default file name
            dlg.DefaultExt = ".jpg"; // Default file extension
            dlg.Filter = "Image documents (.jpg)|*.jpg"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

      
            // Process save file dialog box results
            if (result == true)
            {
           
                var bmp = imgzoom.Source as BitmapImage;

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using (var stream = dlg.OpenFile())
                    encoder.Save(stream);

            }

        }

    }
}
