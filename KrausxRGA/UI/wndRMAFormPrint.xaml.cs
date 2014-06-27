using KrausRGA.DBLogics;
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
using System.Windows.Controls.Primitives;
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

        mUpdateForNewRMA forgetdataNewRMA = new mUpdateForNewRMA();

        mUpdateModeRMA forSRnumber = new mUpdateModeRMA();

        protected DBLogics.cmdReasons cRtnreasons = new DBLogics.cmdReasons();

        mReturnDetails mreturn;

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
                if (retunbyrow.OrderNumber == "N/A")
                {
                    forgetdataNewRMA = new mUpdateForNewRMA(retunbyrow.RGAROWID);

                    txtPonumber.Text = forgetdataNewRMA._ReturnTbl1.PONumber;
                    txtRMA.Text = forgetdataNewRMA._ReturnTbl1.RMANumber;
                    txtVendorName.Text = forgetdataNewRMA._ReturnTbl1.VendoeName;
                    txtName.Text = forgetdataNewRMA._ReturnTbl1.VendoeName;
                    txtAddress.Text = forgetdataNewRMA._ReturnTbl1.Address1 + " " + forgetdata._ReturnTbl1.Address2 + " " + forgetdata._ReturnTbl1.Address3;
                    txtRequestDate.Text = Convert.ToString(forgetdataNewRMA._ReturnTbl1.CreatedDate);

                    dgPackageInfo.ItemsSource = forgetdataNewRMA._lsReturnDetails1;//.OrderByDescending(q => q.SKU_Sequence);

                    dtLoadUpdate1 = new DispatcherTimer();
                    dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                    dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                    //start the dispacher.
                    dtLoadUpdate1.Start();

                    double height = dgPackageInfo.DesiredSize.Height;

                    // Canvas.GetTop(CanvasGrid);

                    Canvas.SetTop(CanvasNote, height);

                    _threadPrint.Interval = new TimeSpan(0, 0, 5);
                    _threadPrint.Start();
                    _threadPrint.Tick += _threadPrint_Tick;
                }
                else
                {
                    forgetdata = new mupdatedForPonumber(retunbyrow.PONumber);

                    txtPonumber.Text = forgetdata._ReturnTbl1.PONumber;
                    txtRMA.Text = forgetdata._ReturnTbl1.RMANumber;
                    txtVendorName.Text = forgetdata._ReturnTbl1.VendoeName;
                    txtName.Text = forgetdata._ReturnTbl1.VendoeName;
                    txtAddress.Text = forgetdata._ReturnTbl1.Address1 + " " + forgetdata._ReturnTbl1.Address2 + " " + forgetdata._ReturnTbl1.Address3;
                    txtRequestDate.Text = Convert.ToString(forgetdata._ReturnTbl1.CreatedDate);

                    dgPackageInfo.ItemsSource = forgetdata._lsReturnDetails1;//.OrderByDescending(q => q.SKU_Sequence);

                    dtLoadUpdate1 = new DispatcherTimer();
                    dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                    dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                    //start the dispacher.
                    dtLoadUpdate1.Start();

                    double height = dgPackageInfo.DesiredSize.Height;

                    // Canvas.GetTop(CanvasGrid);

                    Canvas.SetTop(CanvasNote, height);

                    _threadPrint.Interval = new TimeSpan(0, 0, 5);
                    _threadPrint.Start();
                    _threadPrint.Tick += _threadPrint_Tick;
                }
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

                dgPackageInfo.ItemsSource = forSRnumber._lsReturnDetails;//.OrderByDescending(q => q.SKU_Sequence);

                double height = dgPackageInfo.DesiredSize.Height;
                Canvas.SetTop(CanvasNote, height);
                //  Canvas.GetTop(CanvasGrid);

                dtLoadUpdate1 = new DispatcherTimer();
                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                //start the dispacher.
                dtLoadUpdate1.Start();

                _threadPrint.Interval = new TimeSpan(0, 0, 5);
                _threadPrint.Start();
                _threadPrint.Tick += _threadPrint_Tick;
            }



        }

        void dtLoadUpdate1_Tick(object sender, EventArgs e)
        {
            dtLoadUpdate1.Stop();

            string Reason = "";


            #region ForStatus
             for (int i = 0; i < dgPackageInfo.Items.Count; i++)
            {

                DataGridRow rowContainer = GetRow(i);

                TextBlock SkuNumber = dgPackageInfo.Columns[0].GetCellContent(rowContainer) as TextBlock;

                TextBlock Status = dgPackageInfo.Columns[2].GetCellContent(rowContainer) as TextBlock;

                TextBlock ReasonFeild = dgPackageInfo.Columns[3].GetCellContent(rowContainer) as TextBlock;

                Reason = "";
                for (int j = 0; j < forgetdata._lsskuandpoints.Count; j++)
                {
                    if (forgetdata._lsReturnDetails1[i].SKUNumber == SkuNumber.Text && forgetdata._lsReturnDetails1[i].ReturnDetailID == forgetdata._lsskuandpoints[j].ReturnDetailID)
                    {
                        // Reason = Reason + forgetdata._lsskuandpoints[j].Reason + ", ";

                      

                        if (forgetdata._lsskuandpoints[j].Reason == "Item is New" && forgetdata._lsskuandpoints[j].Reason_Value == "Yes")
                        {
                            Reason = Reason + "New" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Item is New" && forgetdata._lsskuandpoints[j].Reason_Value == "No")
                        {
                            Reason = Reason + "Not New" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Installed" && forgetdata._lsskuandpoints[j].Reason_Value == "Yes")
                        {
                            Reason = Reason + "Installed" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Installed" && forgetdata._lsskuandpoints[j].Reason_Value == "No")
                        {
                            Reason = Reason + "Not Installed" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Chip/Bended/Scratch/Broken" && forgetdata._lsskuandpoints[j].Reason_Value == "Yes")
                        {
                            Reason = Reason + "Chip/Bended/Scratch/Broken" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Chip/Bended/Scratch/Broken" && forgetdata._lsskuandpoints[j].Reason_Value == "No")
                        {
                            Reason = Reason + "Not Chip/Bended/Scratch/Broken" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Manufacturer Defective" && forgetdata._lsskuandpoints[j].Reason_Value == "Yes")
                        {
                            Reason = Reason + "Manufacturer Defective" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Manufacturer Defective" && forgetdata._lsskuandpoints[j].Reason_Value == "No")
                        {
                            Reason = Reason + "Not Manufacturer Defective" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Defect in Transite" && forgetdata._lsskuandpoints[j].Reason_Value == "Yes")
                        {
                            Reason = Reason + "Defect in Transite" + ",";
                        }
                        else if (forgetdata._lsskuandpoints[j].Reason == "Defect in Transite" && forgetdata._lsskuandpoints[j].Reason_Value == "No")
                        {
                            Reason = Reason + "Not Defect in Transite" + ",";
                        }
                    }
                }
                string Reason1 = Reason.TrimEnd(',');
                Status.Text = Reason1;
                Reason1 = "";
            }
            #endregion

            #region For Reason

             for (int i = 0; i < dgPackageInfo.Items.Count; i++)
             {
                DataGridRow rowContainer = GetRow(i);

                TextBlock ReasonFeild = dgPackageInfo.Columns[3].GetCellContent(rowContainer) as TextBlock;

                for (int j = 0; j < forgetdata._lsReasons1.Count; j++)
                {
                    if (forgetdata._lsReturnDetails1[i].ReturnDetailID == forgetdata._lsReasons1[j].ReturnDetailID)
                    {
                        System.Guid ReturnID = forgetdata._lsReturnDetails1[i].ReturnDetailID;

                        string reas = cRtnreasons.GetReasonsByReturnDetailID(ReturnID);

                        ReasonFeild.Text = reas;
                    }
                }

             }


            #endregion



        }



        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    dgPackageInfo.ScrollIntoView(rowContainer, dgPackageInfo.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)dgPackageInfo.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dgPackageInfo.UpdateLayout();
                dgPackageInfo.ScrollIntoView(dgPackageInfo.Items[index]);
                row = (DataGridRow)dgPackageInfo.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
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
