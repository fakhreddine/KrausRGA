using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using KrausRGA.Views;
using KrausRGA.Barcode;

namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndSlipPrint.xaml
    /// </summary>
    public partial class wndSlipPrint : Window
    {

        DispatcherTimer _threadPrint = new DispatcherTimer();
        List<cSlipInfo> _lsInfoSlip = new List<cSlipInfo>();
        public UPCA upc = null;

       

        public wndSlipPrint()
        {
            InitializeComponent();
            _threadPrint.Interval = new TimeSpan(0, 0, 1);
            _threadPrint.Start();
            _threadPrint.Tick += _threadPrint_Tick;
            txtTextToAdd.Visibility = Visibility.Hidden;
        }

        void _threadPrint_Tick(object sender, EventArgs e)
        {
            //Print functions.
            _print();
            //Stop Double priting 
            _threadPrint.Stop();
            //Close this window.
            this.Close();
           
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();

            _lsInfoSlip = clGlobal.lsSlipInfo;

            string SRnumber = _lsInfoSlip[0].SRNumber;
            string SKUName = _lsInfoSlip[0].ProductName;
            string productname = _lsInfoSlip[0].EANCode;
            DateTime ReceivedDate = _lsInfoSlip[0].ReceivedDate;
            DateTime Expiration = _lsInfoSlip[0].Expiration;
            string UserName = _lsInfoSlip[0].ReceivedBY;
            string RMAStatusReal;
            String RMAStatus = _lsInfoSlip[0].RMAStatus;
            if (RMAStatus == "0")
            {
                RMAStatusReal = "Accepted";
            }
            else if (RMAStatus == "1")
            {
                RMAStatusReal = "Rejected";
            }
            else
            {
                RMAStatusReal = "N/A";
            }

            string ItemStatus = _lsInfoSlip[0].ItemStatus;

            string Reason = "N/A";

            if(_lsInfoSlip[0].Reason!="")
             Reason = _lsInfoSlip[0].Reason;

            var sBoxNumber = b.Encode(BarcodeLib.TYPE.CODE128, SRnumber, System.Drawing.Color.Black, System.Drawing.Color.Transparent, 1500, 550);
           // var sproductname = b.Encode(BarcodeLib.TYPE.UPCA, productname, System.Drawing.Color.Black, System.Drawing.Color.Transparent, 2000, 500);
            txtTextToAdd.Text = _lsInfoSlip[0].EANCode;
            if (_lsInfoSlip[0].EANCode == "" || _lsInfoSlip[0].EANCode == "N/A")
            {
                txtTextToAdd.Text = "000000000000";
                SKUName = "*[UPC Code Not Found] " + SKUName;
            }
            


            UPCA upca = new UPCA();
            if (this.txtTextToAdd.Text.Length == 12)
            {
                this.txtTextToAdd.Text = this.txtTextToAdd.Text.Substring(0, 11) + upca.GetCheckSum(this.txtTextToAdd.Text).ToString();
                System.Drawing.Image img;
                img = upca.CreateBarCode(this.txtTextToAdd.Text, 3);

                //this.image.Left = System.Convert.ToInt32((this.image.Width / 2) - (img.Width / 2));

                var imges = new System.Drawing.Bitmap(img);

                var newimag = Imaging.CreateBitmapSourceFromHBitmap(imges.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());


                image.Source = newimag;

                //this.pctBarCode.Image = img;
                this.txtTextToAdd.SelectAll();
            }
            else
            {
                this.image.Source = null;
            }

            var bitmapBox = new System.Drawing.Bitmap(sBoxNumber);
         //   var pbitmapBox = new System.Drawing.Bitmap(sproductname);

            var bBoxSource = Imaging.CreateBitmapSourceFromHBitmap(bitmapBox.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
           // var pproduct = Imaging.CreateBitmapSourceFromHBitmap(pbitmapBox.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            bitmapBox.Dispose();

            imageBarcode.Source = bBoxSource;
          //  image.Source = pproduct;

            txtExpiration.Text = Expiration.ToString("MMM dd, yyyy");
            txtReceivedDate.Text = ReceivedDate.ToString("MMM dd, yyyy");
            txtReceived.Text = UserName; 
            txtReason.Text = Reason;
            txtSRNumber.Text = SRnumber;
            txtproductName.Text = SKUName;
            txtRMAStatus.Text = RMAStatusReal;
            txtItemStatus.Text = ItemStatus;

           // txtEANcode.Text = _lsInfoSlip[0].EANCode;
        }

        private void _print()
        {
            try
            {

                PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
                printDlg.PrintTicket.PageMediaSize = new PageMediaSize((Double)395.0, (Double)820.0);
                //printDlg.ShowDialog();

                //get selected printer capabilities
                System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.Width, capabilities.PageImageableArea.ExtentHeight / this.Height);

                //Transform the Visual to scale
                this.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                this.Measure(sz);

                this.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.
                printDlg.PrintVisual(this, "BoxSlip_KrausUSA_A");
            }
            catch (Exception )
            {

            }
        }
    }
}
