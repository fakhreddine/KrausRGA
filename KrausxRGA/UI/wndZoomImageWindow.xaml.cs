using KrausRGA.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

           // string path = "C:\\images\\";

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

             //  // BitmapImage bi = new BitmapImage();
             //   bmp.BeginInit();
             ////   bi.UriSource = photoPath;
             //   bmp.DecodePixelWidth = 0;
             //   bmp.DecodePixelHeight = ;
             //   bmp.EndInit();





                using (var stream = dlg.OpenFile())
                {
                    //bmp.BeginInit();
                    // bmp.DecodePixelWidth = 100;
                    // bmp.DecodePixelHeight = 100; //Convert.ToInt16(0.01);
                    // bmp.CacheOption = BitmapCacheOption.OnLoad;
                    // bmp.StreamSource = stream;
                    // bmp.EndInit();
                    //var photoDecoder = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    //var photo = photoDecoder.Frames[0];

                    //var target = new TransformedBitmap(photo, new ScaleTransform(10 / photo.Width * 96 / photo.DpiX, 10 / photo.Height * 96 / photo.DpiY, 0, 0));
                    ////    var thumbnail = BitmapFrame.Create(target);
                    //var encoder = new PngBitmapEncoder();
                    //encoder.Frames.Add(BitmapFrame.Create(target));
                    //using (var stream1 = dlg.OpenFile())
                    //    encoder.Save(stream1);



                   // string strFileName = "Marshimaro.jpg";
                   // string strThumbnail = "MarshimaroThumb.png";

                    byte[] baSource = getJPGFromImageControl(bmp);

                   // byte[] baSource = File.ReadAllBytes(strFileName);
                    Stream streamPhoto = new MemoryStream(baSource);
                    BitmapFrame bfPhoto = ReadBitmapFrame(streamPhoto);
                    int nThumbnailSize = 400, nWidth, nHeight;
                    if (bfPhoto.Width > bfPhoto.Height)
                    {
                        nWidth = nThumbnailSize;
                        nHeight = (int)(bfPhoto.Height * nThumbnailSize / bfPhoto.Width);
                    }
                    else
                    {
                        nHeight = nThumbnailSize;
                        nWidth = (int)(bfPhoto.Width * nThumbnailSize / bfPhoto.Height);
                    }
                    BitmapFrame bfResize = FastResize(bfPhoto, nWidth, nHeight);
                   // byte[] baResize = ToByteArray(bfResize);
                    //File.WriteAllBytes(@"Thumbnails\" + Path.GetFileNameWithoutExtension(strThumbnail) + ".png", baResize);
                    //Console.WriteLine("Resize done!!!");

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bfResize));
                    //using (var stream1 = dlg.OpenFile())
                        encoder.Save(stream);

                }





             

                //System.Drawing.Bitmap resizedImage;

                //using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(filePath)) 
                //    resizedImage = new System.Drawing.Bitmap(originalImage,);

                //// Save resized picture
                //resizedImage.Save(resizedFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                //resizedImage.Dispose();
                //byte[] targetBytes = null;
                //using (var memoryStream = new MemoryStream())
                //{
                //    var targetEncoder = new PngBitmapEncoder();
                //    targetEncoder.Frames.Add(Resize(bmp, 10, 10, 1));
                //    targetEncoder.Save(memoryStream);
                //    targetBytes = memoryStream.ToArray();
                //}


                //var encoder = new PngBitmapEncoder();
                //encoder.Frames.Add(BitmapFrame.Create(bmp));
                //using (var stream1 = dlg.OpenFile())
                //    encoder.Save(stream1);

            }

        }

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.GetBuffer();
        }

        public static BitmapFrame Resize(BitmapFrame photo, int width, int height, BitmapScalingMode scalingMode)
        {

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(
                group, scalingMode);
            group.Children.Add(
                new ImageDrawing(photo,
                    new Rect(0, 0, width, height)));
            var targetVisual = new DrawingVisual();
            var targetContext = targetVisual.RenderOpen();
            targetContext.DrawDrawing(group);
            var target = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Default);
            targetContext.Close();
            target.Render(targetVisual);
            var targetFrame = BitmapFrame.Create(target);
            return targetFrame;
        }
        private void imgzoom_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
           // var bmp = imgzoom.Source as BitmapImage;
            //Process.Start(@"imgzoom.Source");

            //Process.Start(

        }

        private void ResizeImage(string inputPath, string outputPath, int width, int height)
        {
            var bitmap = new BitmapImage();

            using (var stream = new FileStream(inputPath, FileMode.Open))
            {
                bitmap.BeginInit();
                bitmap.DecodePixelWidth = width;
                bitmap.DecodePixelHeight = height;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
        private static BitmapFrame FastResize(BitmapFrame bfPhoto, int nWidth, int nHeight)
        {
            TransformedBitmap tbBitmap = new TransformedBitmap(bfPhoto, new ScaleTransform(nWidth / bfPhoto.Width, nHeight / bfPhoto.Height, 0, 0));
            return BitmapFrame.Create(tbBitmap);
        }

        private static byte[] ToByteArray(BitmapFrame bfResize)
        {

            MemoryStream msStream = new MemoryStream();
            PngBitmapEncoder pbdDecoder = new PngBitmapEncoder();
            pbdDecoder.Frames.Add(bfResize);
            pbdDecoder.Save(msStream);
            return msStream.ToArray();
        }

        private static BitmapFrame ReadBitmapFrame(Stream streamPhoto)
        {
            BitmapDecoder bdDecoder = BitmapDecoder.Create(streamPhoto, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            return bdDecoder.Frames[0];
        }

    }
}
