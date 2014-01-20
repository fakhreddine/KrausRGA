using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using WindowsInput;
using System.Diagnostics;

namespace KrausRGA.Barcode
{
   public static class Camera
    {
        private static string imgPath = "C:\\Users\\" + Environment.UserName + "\\Pictures\\Camera Roll\\";
       
       /// <summary>
       /// capture Photo from Open The Camera.
       /// </summary>
       public static void TakePhoto()
       {
           try
           {
               Thread.Sleep(2000);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.SPACE);
              
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
               InputSimulator.SimulateKeyPress(VirtualKeyCode.LWIN);
               Thread.Sleep(500);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_C);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_A);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_M);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_E);
               Thread.Sleep(300);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
               Thread.Sleep(1500);
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
               InputSimulator.SimulateKeyDown(VirtualKeyCode.LWIN);
               Thread.Sleep(600);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
               Thread.Sleep(600);
               InputSimulator.SimulateKeyUp(VirtualKeyCode.LWIN);
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
                var DirInfo = new DirectoryInfo(imgPath);
                DirInfo.Attributes &= ~FileAttributes.ReadOnly;
                ImageName = (from f in DirInfo.GetFiles()
                                    orderby f.LastWriteTime descending
                                    select f).First().Name.ToString();
           }
           catch (Exception)
           {}
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
                _returnCount= (from f in DirInfo.GetFiles()
                             select f).Count();
           }
           catch (Exception)
           {}
           return _returnCount;
       }

       /// <summary>
       /// Move Camera Roll Folder images to the Designation Folder.
       /// </summary>
       /// <param name="DesignationFolder_FullPath">
       /// Images Moved to this Location, Must be full path
       /// </param>
       public static void MoveImagesToImages()
       {
           try
           {
               String Path = Environment.CurrentDirectory+"\\Move.bat"; //Environment.CurrentDirectory + "\\CapturePhotos\\MoveFolder\\MoveFiles.exe";
               ProcessStartInfo myProcess = new ProcessStartInfo(Path);
               myProcess.WindowStyle = ProcessWindowStyle.Hidden;
               myProcess.UseShellExecute = true;
               Process.Start(myProcess);
           }
           catch (Exception)
           { }
       }

       public static System.Drawing.Bitmap CaptureDesktopWithCursor()
       {
           System.Drawing.Bitmap desktopBMP;
           desktopBMP = CaptureDesktop();
           if (desktopBMP != null)
           {
               return desktopBMP;
           }
           return null;
       }
       public struct SIZE
       {
           public int cx;
           public int cy;
       }
       static System.Drawing.Bitmap CaptureDesktop()
       {
           SIZE size;
           IntPtr hBitmap;
           IntPtr hDC = Win32Stuff.GetDC(Win32Stuff.GetDesktopWindow());
           IntPtr hMemDC = GDIStuff.CreateCompatibleDC(hDC);

           size.cx = Win32Stuff.GetSystemMetrics
                     (Win32Stuff.SM_CXSCREEN);

           size.cy = Win32Stuff.GetSystemMetrics
                     (Win32Stuff.SM_CYSCREEN);

           hBitmap = GDIStuff.CreateCompatibleBitmap(hDC, size.cx, size.cy);

           if (hBitmap != IntPtr.Zero)
           {
               IntPtr hOld = (IntPtr)GDIStuff.SelectObject
                                      (hMemDC, hBitmap);

               GDIStuff.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC,
                                              0, 0, GDIStuff.SRCCOPY);

               GDIStuff.SelectObject(hMemDC, hOld);
               GDIStuff.DeleteDC(hMemDC);
               Win32Stuff.ReleaseDC(Win32Stuff.GetDesktopWindow(), hDC);
               System.Drawing.Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
               GDIStuff.DeleteObject(hBitmap);
               GC.Collect();
               return bmp;
           }
           return null;

       }
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
    }
}
