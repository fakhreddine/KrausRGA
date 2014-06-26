using KrausRGA.Models;
using KrausRGA.Views;
using System;
using System.Collections;
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

        DispatcherTimer dtLoadUpdate1;

        double hei = 2000;

        public wndRMAFormPrint()
        {
            InitializeComponent();
         
            // txtTextToAdd.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            var retunbyrow = _mponumner.GetReturnByRowID(clGlobal.NewRGANumber)[0];

            if (retunbyrow.RMANumber == "N/A")
            {
                forgetdata = new mupdatedForPonumber(retunbyrow.PONumber);

                txtPonumber.Text = forgetdata._ReturnTbl1.PONumber;
                txtRMA.Text = forgetdata._ReturnTbl1.RMANumber;
                txtVendorName.Text = forgetdata._ReturnTbl1.VendoeName;
                txtName.Text = forgetdata._ReturnTbl1.VendoeName;
                txtAddress.Text = forgetdata._ReturnTbl1.Address1 + " " + forgetdata._ReturnTbl1.Address2 + " " + forgetdata._ReturnTbl1.Address3;
                txtRequestDate.Text = Convert.ToString(forgetdata._ReturnTbl1.CreatedDate);

                dgPackageInfo.ItemsSource = forgetdata._lsReturnDetails1;

                dtLoadUpdate1 = new DispatcherTimer();
                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                //start the dispacher.
                dtLoadUpdate1.Start();

                _threadPrint.Interval = new TimeSpan(0, 0, 5);
                _threadPrint.Start();
                _threadPrint.Tick += _threadPrint_Tick;


                              

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

                dtLoadUpdate1 = new DispatcherTimer();
                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                //start the dispacher.
                dtLoadUpdate1.Start();

                Canvas.SetTop(CanvasNote, height);

            }


        }

        void dtLoadUpdate1_Tick(object sender, EventArgs e)
        {
            dtLoadUpdate1.Stop();

            foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
            {

                string Reason = "";
                TextBlock SkuNumber = dgPackageInfo.Columns[0].GetCellContent(row1) as TextBlock;

                

                for (int i = 0; i < forgetdata._lsReturnDetails1.Count; i++)
                {
                    TextBlock Status = dgPackageInfo.Columns[2].GetCellContent(row1) as TextBlock;
                    Reason = "";
                    for (int j = 0; j < forgetdata._lsskuandpoints.Count; j++)
                    {
                        if (forgetdata._lsReturnDetails1[i].SKUNumber == SkuNumber.Text && forgetdata._lsReturnDetails1[i].ReturnDetailID == forgetdata._lsskuandpoints[j].ReturnDetailID)
                        {
                            Reason = Reason + forgetdata._lsskuandpoints[j].Reason + ", ";
                        }
                    }
                     Status.Text = Reason;
                }
              
            }
        

        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            if (itemsSource != null)
            {

                foreach (var item in itemsSource)
                {
                    var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (null != row) yield return row;
                }
            }
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
