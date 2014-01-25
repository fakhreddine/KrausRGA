using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace KrausRGA.Barcode
{
   public static class BarcodeRead 
    {
       /// <summary>
       /// Adjust Image Cotrast 
       /// </summary>
       /// <param name="Image"></param>
       /// <param name="Value"></param>
       /// <returns></returns>
       public static Bitmap AdjustContrast(Bitmap Image, float Value)
       {
           Value = (100.0f + Value) / 100.0f;
           Value *= Value;
           Bitmap NewBitmap = (Bitmap)Image.Clone();
           BitmapData data = NewBitmap.LockBits(
               new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
               ImageLockMode.ReadWrite,
               NewBitmap.PixelFormat);
           int Height = NewBitmap.Height;
           int Width = NewBitmap.Width;

           unsafe
           {
               for (int y = 0; y < Height; ++y)
               {
                   byte* row = (byte*)data.Scan0 + (y * data.Stride);
                   int columnOffset = 0;
                   for (int x = 0; x < Width; ++x)
                   {
                       byte B = row[columnOffset];
                       byte G = row[columnOffset + 1];
                       byte R = row[columnOffset + 2];

                       float Red = R / 255.0f;
                       float Green = G / 255.0f;
                       float Blue = B / 255.0f;
                       Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                       Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                       Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                       int iR = (int)Red;
                       iR = iR > 255 ? 255 : iR;
                       iR = iR < 0 ? 0 : iR;
                       int iG = (int)Green;
                       iG = iG > 255 ? 255 : iG;
                       iG = iG < 0 ? 0 : iG;
                       int iB = (int)Blue;
                       iB = iB > 255 ? 255 : iB;
                       iB = iB < 0 ? 0 : iB;

                       row[columnOffset] = (byte)iB;
                       row[columnOffset + 1] = (byte)iG;
                       row[columnOffset + 2] = (byte)iR;

                       columnOffset += 4;
                   }
               }
           }

           NewBitmap.UnlockBits(data);

           return NewBitmap;
       }

       /// <summary>
       /// Read the Barcode from the Image.
       /// </summary>
       /// <param name="FileName"></param>
       /// <returns></returns>
       public static String Read(Bitmap bmpImage)
       {
           //Bitmap bmpImage = new Bitmap(FileName);
           bmpImage = AdjustContrast(bmpImage, (float)20.0);
           String _return = "";
           System.Collections.ArrayList barcodes= new System.Collections.ArrayList();
           int  iScans = 100;
           BarcodeScanner.FullScanPage(ref barcodes, bmpImage, iScans, BarcodeScanner.BarcodeType.All);
           foreach (var Str in barcodes)
           {
               _return = _return + Str.ToString();
           }
           return _return;
       }


       public static String Read(System.Windows.Controls.Canvas CanvasControl)
       {
           String _return = "";
           try
           {
               Bitmap Bmp = Camera.CanvasToBitmap(CanvasControl);
               _return = Read(Bmp);

           }
           catch (Exception)
           {
           }
           return _return;
                
       }
    }
}
