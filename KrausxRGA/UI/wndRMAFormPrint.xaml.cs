using KrausRGA.Models;
using KrausRGA.Views;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndRMAFormPrint.xaml
    /// </summary>
    public partial class wndRMAFormPrint : Window
    {
        DispatcherTimer _threadPrint = new DispatcherTimer();

        mPOnumberRMA _mponumner = new mPOnumberRMA();

        mupdatedForPonumber forgetdata = new mupdatedForPonumber();

        mUpdateModeRMA forSRnumber = new mUpdateModeRMA();

        double hei = 2000;

        public wndRMAFormPrint()
        {
            InitializeComponent();
            _threadPrint.Interval = new TimeSpan(0, 0, 1);
            _threadPrint.Start();
            _threadPrint.Tick += _threadPrint_Tick;
            // txtTextToAdd.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            var retunbyrow = _mponumner.GetReturnByRowID(clGlobal.NewRGANumber)[0];

            if (retunbyrow.RMANumber == "N/A")
            {
                this.Dispatcher.Invoke(new Action(() => { forgetdata = new mupdatedForPonumber(retunbyrow.PONumber); }));

                txtPonumber.Text = forgetdata._ReturnTbl1.PONumber;
                txtRMA.Text = forgetdata._ReturnTbl1.RMANumber;
                txtVendorName.Text = forgetdata._ReturnTbl1.VendoeName;
                txtName.Text = forgetdata._ReturnTbl1.VendoeName;
                txtAddress.Text = forgetdata._ReturnTbl1.Address1 + " " + forgetdata._ReturnTbl1.Address2 + " " + forgetdata._ReturnTbl1.Address3;
                txtRequestDate.Text = Convert.ToString(forgetdata._ReturnTbl1.CreatedDate);

                dgPackageInfo.ItemsSource = forgetdata._lsReturnDetails1;

                double height = dgPackageInfo.DesiredSize.Height;

               // Canvas.GetTop(CanvasGrid);

                Canvas.SetTop(CanvasNote, height);
            }
            else
            {
                this.Dispatcher.Invoke(new Action(() => { forSRnumber = new mUpdateModeRMA(retunbyrow.RMANumber); }));

                txtPonumber.Text = forSRnumber._ReturnTbl.PONumber;
                txtRMA.Text = forSRnumber._ReturnTbl.RMANumber;
                txtVendorName.Text = forSRnumber._ReturnTbl.VendoeName;
                txtName.Text = forSRnumber._ReturnTbl.VendoeName;
                txtAddress.Text = forSRnumber._ReturnTbl.Address1 + " " + forSRnumber._ReturnTbl.Address2 + " " + forSRnumber._ReturnTbl.Address3;
                txtRequestDate.Text = Convert.ToString(forSRnumber._ReturnTbl.CreatedDate);

                dgPackageInfo.ItemsSource = forSRnumber._lsReturnDetails;

                double height = dgPackageInfo.DesiredSize.Height;

              //  Canvas.GetTop(CanvasGrid);

                Canvas.SetTop(CanvasNote, height);

            }

            //_mponumner.mPOnumberRMA1(retunbyrow.PONumber);
            //forgetdata._ReturnTbl1

            //retunbyrow.ReturnID

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
        private void _print()
        {
            try
            {

                PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
                printDlg.PrintTicket.PageMediaSize = new PageMediaSize((Double)700.0, (Double)hei);
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
            catch (Exception)
            {

            }
        }
    }
}
