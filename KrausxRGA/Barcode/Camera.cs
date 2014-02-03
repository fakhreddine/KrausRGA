using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using WindowsInput;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace KrausRGA.Barcode
{
    public static class Camera
    {
        /// <summary>
        /// Static pathe of images where we want to store the images. this is set though the properties of project.
        /// </summary>
        private static string imgPath = KrausRGA.Properties.Settings.Default.DrivePath + "\\";

        /// <summary>
        /// capture Photo from Open The Camera.
        /// </summary>
        public static void TakePhoto(System.Windows.Controls.Canvas surface)
        {
            try
            {
                String RAMNUmber = "NEWRMA00";
                try
                {
                    RAMNUmber = Views.clGlobal.mReturn.lsRMAInformation[0].RMANumber;
                }
                catch (Exception)
                {
                }
                CanvasExportToPng(new Uri("C:\\Images" + "\\" + RAMNUmber + "_" + DateTime.Now.ToString("ddMMHHmmssstt") + ".jpg"), surface);
                var player = new MediaPlayer();
                player.Open(new Uri(@"C:\Windows\Media\Windows Recycle.wav"));
                player.Play();
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// open Camera.
        /// </summary>
        public static void Open()
        {
            try
            {
                UI.wndCamera cam = new UI.wndCamera();
                cam.ShowDialog();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Close Camera.
        /// </summary>
        public static void Close()
        {
            try
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.MENU);
                InputSimulator.SimulateKeyPress(VirtualKeyCode.SPACE);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_C);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Latest Photo Captured in Camera Roll Folder Full Path.
        /// </summary>
        /// <returns></returns>
        public static String LastPhotoName()
        {

            String ImageName = "";
            try
            {
                var DirInfo = new DirectoryInfo("C:\\Images\\");
                DirInfo.Attributes &= ~FileAttributes.ReadOnly;
                ImageName = (from f in DirInfo.GetFiles()
                             orderby f.LastWriteTime descending
                             select f).First().Name.ToString();
            }
            catch (Exception)
            { }
            return ImageName;
        }

        /// <summary>
        /// Count Number Of Files int the Folder;
        /// </summary>
        /// <returns></returns>
        public static int CheckImageCount()
        {
            int _returnCount = 0;
            try
            {
                var DirInfo = new DirectoryInfo(imgPath);
                DirInfo.Attributes &= ~FileAttributes.ReadOnly;
                _returnCount = (from f in DirInfo.GetFiles()
                                select f).Count();
            }
            catch (Exception)
            { }
            return _returnCount;
        }

        /// <summary>
        /// This function convert the Canvas control to the Bitmap Image.
        /// </summary>
        /// <param name="surface">
        /// Canvas control Object want to convert.
        /// </param>
        /// <returns>
        /// Bitmap Object containing Canvas control Image.
        /// </returns>
        public static System.Drawing.Bitmap CanvasToBitmap(System.Windows.Controls.Canvas surface)
        {
            System.Drawing.Bitmap BmpOut = null;
            try
            {
                try
                {
                    File.Delete("C:\\Images\\Img.jpg");
                }
                catch (Exception)
                { }
                Uri path = new Uri("C:\\Images\\Img.jpg");
                // Save current canvas transform
                Transform transform = surface.LayoutTransform;
                // reset current transform (in case it is scaled or rotated)
                surface.LayoutTransform = null;

                // Get the size of canvas
                Size size = new Size(surface.ActualWidth, surface.ActualHeight);
                // Measure and arrange the surface
                // VERY IMPORTANT
                surface.Measure(size);
                surface.Arrange(new Rect(size));

                // Create a render bitmap and push the surface to it
                RenderTargetBitmap renderBitmap =
                  new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96d,
                    96d,
                    PixelFormats.Pbgra32);
                renderBitmap.Render(surface);

                // Create a file stream for saving image
                using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
                {
                    // Use png encoder for our data
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    // push the rendered bitmap to it
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    // save the data to the stream
                    encoder.Save(outStream);
                }

                // Restore previously saved layout
                surface.LayoutTransform = transform;
                Thread.Sleep(200);

                BmpOut = new System.Drawing.Bitmap("C:\\Images\\Img.jpg");
            }
            catch (Exception)
            { }
            return BmpOut;
        }

        /// <summary>
        /// Convert the Bitmap to Image source.
        /// </summary>
        /// <param name="bitmap">
        /// Bitmap Object.
        /// </param>
        /// <param name="imgFormat">
        /// 
        /// </param>
        /// <returns></returns>
        public static BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap, System.Drawing.Imaging.ImageFormat imgFormat)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, imgFormat);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static void CreateThumbnail(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                    stream5.Close();
                }

            }
        }

        public static void CanvasExportToPng(Uri path, System.Windows.Controls.Canvas surface)
        {
            
            if (path == null) return;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.ActualWidth, surface.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            // Restore previously saved layout
            surface.LayoutTransform = transform;
        }

        public static void Rotate90Degree(String ImageUri)
        {
            System.Drawing.Bitmap bitmap1 = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(ImageUri);
            bitmap1.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
            bitmap1.Save(ImageUri);
        }

        public static void CopytoNetwork(String Filename)
        {
            try
            {
                string updir = KrausRGA.Properties.Settings.Default.DrivePath;

                //AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                //WindowsIdentity identity = new WindowsIdentity(KrausRGA.Properties.Settings.Default.UserNameForImagesLogin, KrausRGA.Properties.Settings.Default.UserPasswordForImages);
                //WindowsImpersonationContext context = identity.Impersonate();
                Thread newWindowThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        //System.Net.NetworkCredential AccessPermissions = new System.Net.NetworkCredential(KrausRGA.Properties.Settings.Default.UserNameForImagesLogin, KrausRGA.Properties.Settings.Default.UserNameForImagesLogin);
                        //using (new NetworkConnection(updir, AccessPermissions))
                        //{
                            File.Copy(@"C:\Images\" + Filename, updir + "\\" + Filename, true);
                            // File.Delete(@"C:\Images\" + Filename);
                            // Start the Dispatcher Processing
                            System.Windows.Threading.Dispatcher.Run();
                       // }
                    }
                    catch (Exception)
                    {
                    }
                    
                }));
                // Set the apartment state
                newWindowThread.SetApartmentState(ApartmentState.STA);
                // Make the thread a background thread
                newWindowThread.IsBackground = true;
                // Start the thread
                newWindowThread.Start();  
               
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Delete Files from the local drive at once in a day.
        /// </summary>
        public static void DeleteLocalImages()
        {
            try
            {
                //Read Lines From the Text File.
                String[] FileLines = File.ReadAllLines(Environment.CurrentDirectory.ToString() + "\\VersionNumber.txt")[2].Split(new char[] { ':' });
                
                //Chack for the Datetime 
                if (Convert.ToDateTime(FileLines[1].ToString())<  DateTime.Now)
                {
                    if (FileLines[0]=="0")
                    {
                        Directory.Delete("C:\\Images", true);
                        File.WriteAllText(Environment.CurrentDirectory + "\\VersionNumber.txt", File.ReadAllText(Environment.CurrentDirectory + "\\VersionNumber.txt").Replace(File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[2].ToString(), "1:"+DateTime.Now.ToString("dd/MM/yyyy")).ToString(), Encoding.Default);
                    }
                }
                ///Ceate Diractory for Images.
                if (!Directory.Exists("C:\\Images"))
                {
                    Directory.CreateDirectory("C:\\Images");
                }
            }
            catch (Exception)
            {}
        }
    }
}