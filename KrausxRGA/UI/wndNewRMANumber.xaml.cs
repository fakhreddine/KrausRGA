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
using System.Windows.Shapes;
using KrausRGA.Models;
using KrausRGA.DBLogics;
using KrausRGA.Views;
using System.Collections;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.Expression.Encoder.Devices;
using System.Windows.Controls.Primitives;
using KrausRGA.EntityModel;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput;
using System.Data;
using System.Reflection;
using System.Windows.Threading;



namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndNewRMANumber.xaml
    /// </summary>
    public partial class wndNewRMANumber : Window
    {

        mNewRMANumber _mNewRMA = new mNewRMANumber();

        mUser _mUser = clGlobal.mCurrentUser;

         DataTable dt = new DataTable();

         List<StatusAndPoints> listofstatus = new List<StatusAndPoints>();

         mPOnumberRMA _mponumber = new mPOnumberRMA();

         mUpdateForNewRMA _mUpdate;

         DispatcherTimer dtLoadUpdate;

         List<SkuReasonIDSequence> _lsReasonSKU = new List<SkuReasonIDSequence>();

        Guid ReturnDetailsID;

        string SKU,_SKU;
        string PName,_PName;
        string Qty,_Qty;
        string ProductID;
        string SalesPrices;

        string Cat, _Cat;

        DateTime eastern = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Eastern Standard Time");

      

        public wndNewRMANumber()
        {
            String[] FontSizes = File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[1].Split(new char[] { '-' });
            String HeaderSize = FontSizes[1];
            String ControlSize = FontSizes[2];
            String VeriableSize = FontSizes[0];

            Resources["FontSize"] = Convert.ToDouble(VeriableSize);
            Resources["HeaderSize"] = Convert.ToDouble(HeaderSize);
            Resources["ContactFontSize"] = Convert.ToDouble(ControlSize);

            InitializeComponent();
            FillRMAStausAndDecision();
            txtRMAReqDate.SelectedDate = DateTime.Now;

          
        }

        private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        
        public void FilldgReasons(String cat)
        {
            dgReasons.ItemsSource = _mNewRMA.GetReasons(cat);
        }
        
        private void cntItemStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            TextBlock cbk = (TextBlock)e.Source;
            Canvas cs = cbk.Parent as Canvas;
            TextBlock txtReasonGuids = cs.FindName("txtReasosnsGuid") as TextBlock;
            DataGridRow row = (DataGridRow)cbk.FindParent<DataGridRow>();

            int index = row.GetIndex();
          
            DataGridCell cell2 = GetCell(index, 0);
            ContentPresenter CntPersenter2 = cell2.Content as ContentPresenter;
            DataTemplate DataTemp2 = CntPersenter2.ContentTemplate;

            DataGridCell cell1 = GetCell(index, 1);
            ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
            DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;

            if (((TextBox)DataTemp2.FindName("txtSKU", CntPersenter2)).Text != "")// && ((TextBox)DataTemp1.FindName("txtProductName", CntPersenter1)).Text != "")
            {
                cvItemStatus.Visibility = System.Windows.Visibility.Visible;

                DataGridCell cell = GetCell(index, 0);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;

                string Sku = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text;

                List<String> NewRMAnumber = new List<string>();


                NewRMAnumber = _mNewRMA.NewRMAInfo(Sku);

                string Category;
                if (NewRMAnumber.Count > 0)
                {
                    string[] NewRMA = NewRMAnumber[0].Split(new char[] { '#' });

                    Category = NewRMA[2];

                    FilldgReasons(Category);
                }
                else
                {
                    Category = "";
                    FilldgReasons(Category);
                }
            }
        }

        private void ctlReasons_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TextBlock cbk = (TextBlock)e.Source;
                Canvas cvs = (Canvas)cbk.Parent;
                Border bdr = (Border)cvs.Parent;
                DataGridRow row = (DataGridRow)cbk.FindParent<DataGridRow>();
                ContentPresenter cp = dgReasons.Columns[0].GetCellContent(row) as ContentPresenter;
                DataTemplate Dt = cp.ContentTemplate;
                CheckBox ch = (CheckBox)Dt.FindName("cbReasons", cp);
                if (ch.IsChecked == true)
                {
                    ch.IsChecked = false;
                    bdr.Background = new SolidColorBrush(Colors.SkyBlue);
                }
                else
                {
                    ch.IsChecked = true;
                    bdr.Background = new SolidColorBrush(Colors.Black);
                }
            }
            catch { }

        }

        private void txtItemReason_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtItemReason_GotFocus(object sender, RoutedEventArgs e)
        {

        }

       

        private void btnHomeDone_Click(object sender, RoutedEventArgs e)
        {
            //if (cmbOtherReason.SelectedIndex != 0)
            //{
            //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.New_ReturnReason_Added.ToString(), DateTime.UtcNow.ToString());
            //    Guid reasonID = _mNewRMA.SetReasons(txtOtherReason.Text);
            //}
            //txtOtherReason.Text = "";
            txtItemReason.Text = "";

            Byte RMAStatus = Convert.ToByte(cmbRMAStatus.SelectedValue.ToString());
            Byte Decision = Convert.ToByte(cmbRMADecision.SelectedValue.ToString());

            List<Return> _lsreturn = new List<Return>();
            Return ret = new Return();
            ret.RMANumber = txtRMANumber.Text;
            ret.VendoeName = txtVendorName.Text;
            ret.VendorNumber = txtVendorNumber.Text;
            ret.ScannedDate = DateTime.UtcNow;
            ret.ExpirationDate = DateTime.UtcNow.AddDays(60);
            eastern = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(txtRMAReqDate.SelectedDate.Value, "Eastern Standard Time");
            ret.ReturnDate = eastern;
            ret.PONumber = txtPoNumber.Text;
            ret.CustomerName1 = txtName.Text;
            ret.Address1 = txtAddress.Text;
            ret.City = txtCustCity.Text;
            ret.Country = txtCountry.Text;
            ret.ZipCode = txtZipCode.Text;
            ret.State = txtState.Text;

            _lsreturn.Add(ret);

            //Save to RMA Master Table.
            Guid ReturnTblID = _mNewRMA.SetReturnTbl(_lsreturn, "", RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.WrongRMAFlag, Views.clGlobal.Warranty, 60, 0);

            if (Views.clGlobal.IsAlreadySaved)
            {
                MessageBox.Show("RMA number for this return is : " + _mUpdate._ReturnTbl1.RGAROWID);
            }
            else
            {
                MessageBox.Show("RMA number for this return is : " + _mNewRMA.GetNewROWID(ReturnTblID));
            }

            if (Views.clGlobal.IsAlreadySaved)
            {
                ReturnTblID = _mUpdate._ReturnTbl1.ReturnID;
                _lsreturn.Add(_mUpdate._ReturnTbl1);

                foreach (var ReturnDetailsID in _mUpdate._lsReturnDetails1)
                {
                    _mponumber.DeleteReturnDetails(ReturnDetailsID.ReturnDetailID);
                }
            }

            for (int i = 0; i < dgPackageInfo.Items.Count; i++)
            {

                DataGridCell cell = GetCell(i, 1);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;

                SKU = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text.ToString();


                DataGridCell cell1 = GetCell(i, 4);
                ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
                DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;
                PName = ((TextBlock)DataTemp1.FindName("tbDQyt", CntPersenter1)).Text.ToString();


                DataGridCell cell2 = GetCell(i, 2);
                ContentPresenter CntPersenter2 = cell2.Content as ContentPresenter;
                DataTemplate DataTemp2 = CntPersenter2.ContentTemplate;
                Qty = ((TextBox)DataTemp2.FindName("tbQty", CntPersenter2)).Text.ToString();

                DataGridCell cellforProductID = GetCell(i, 6);
                ContentPresenter CntPersenterforProductID = cellforProductID.Content as ContentPresenter;
                DataTemplate DataTempforProductID = CntPersenterforProductID.ContentTemplate;
                ProductID = ((TextBox)DataTempforProductID.FindName("txtProductID", CntPersenterforProductID)).Text;//= _mNewRMA.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                DataGridCell cellforSales = GetCell(i, 7);
                ContentPresenter CntPersenterforSales = cellforSales.Content as ContentPresenter;
                DataTemplate DataTempforSales = CntPersenterforSales.ContentTemplate;
                SalesPrices = ((TextBox)DataTempforSales.FindName("txtSalesPrice", CntPersenterforSales)).Text;// = "0"


                DataGridCell cell4 = GetCell(i, 3);
                ContentPresenter CntPersenter4 = cell4.Content as ContentPresenter;
                DataTemplate DataTemp4 = CntPersenter4.ContentTemplate;
                StackPanel SpImages = (StackPanel)DataTemp4.FindName("spProductImages", CntPersenter4);


                if (SKU != null) _SKU = SKU;
                if (PName != null) _PName = PName;
                if (Qty != null) _Qty = Qty;
                if (Cat != null) _Cat = Cat;

                //string SKUNewName = "";
                //if (listofstatus.Count > 0)
                //{
                //    for (int j = 0; j < listofstatus.Count; j++)
                //    {
                //        if (listofstatus[j].SKUName == _SKU)
                //        {
                //            SKUNewName = _SKU;
                //            Views.clGlobal.SKU_Staus = listofstatus[j].Status;
                //            Views.clGlobal.TotalPoints = listofstatus[j].Points;
                //            break;
                //        }
                //    }
                //}
                //else
                //{
                //    SKUNewName = _SKU;
                //    Views.clGlobal.SKU_Staus = "Reject";
                //    Views.clGlobal.TotalPoints = 0;
                //}

                string SKUNewName = "";
                Boolean checkflag = false;
                if (listofstatus.Count > 0)
                {
                    for (int k = listofstatus.Count - 1; k >= 0; k--)
                    {
                        if (listofstatus[k].SKUName == SKU && listofstatus[k].NewItemQuantity == Convert.ToInt16(PName))
                        {
                            SKUNewName = SKU;
                            Views.clGlobal.SKU_Staus = listofstatus[k].Status;
                            Views.clGlobal.TotalPoints = listofstatus[k].Points;
                            Views.clGlobal.IsScanned = listofstatus[k].IsScanned;
                            Views.clGlobal.IsManually = listofstatus[k].IsMannually;
                            Views.clGlobal.NewItemQty = listofstatus[k].NewItemQuantity;
                            Views.clGlobal._SKU_Qty_Seq = listofstatus[k].skusequence;

                            listofstatus.RemoveAt(k);
                            checkflag = true;

                            break;
                        }

                    }
                    if (!checkflag)
                    {
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.IsScanned = 1;//listofstatus[i].IsScanned;
                        Views.clGlobal.IsManually = 1;//listofstatus[i].IsMannually;
                        Views.clGlobal.NewItemQty = 1;
                        Views.clGlobal._SKU_Qty_Seq = 0;
                    }
                }
                else
                {
                    SKUNewName = SKU;
                    Views.clGlobal.SKU_Staus = "";
                    Views.clGlobal.TotalPoints = 0;
                    Views.clGlobal.IsScanned = 1;
                    Views.clGlobal.IsManually = 1;
                    Views.clGlobal.NewItemQty = 1;
                    Views.clGlobal._SKU_Qty_Seq = 0;

                }


                if (_SKU != "")
                {
                    ReturnDetailsID = _mNewRMA.SetReturnDetailTbl(ReturnTblID, _SKU, _PName, 0, 0, Convert.ToInt32(_Qty), _Cat, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints, 1, 1, Views.clGlobal.NewItemQty, Views.clGlobal._SKU_Qty_Seq, ProductID, Convert.ToDecimal(SalesPrices));
                }


                if (dt.Rows.Count > 0)
                {
                    for (int j = dt.Rows.Count - 1; j >= 0; j--)
                    {
                        DataRow d = dt.Rows[j];
                        if (d["SKU"].ToString() == SKU && d["ItemQuantity"].ToString() == PName)
                        {
                            Guid ReturnedSKUPoints = _mNewRMA.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[j][0].ToString(), dt.Rows[j][1].ToString(), dt.Rows[j][2].ToString(), Convert.ToInt16(dt.Rows[j][3].ToString()), Convert.ToInt16(dt.Rows[j][4].ToString()));
                            d.Delete();
                        }
                    }
                }


                if (_lsReasonSKU.Count > 0)
                {
                    for (int k = _lsReasonSKU.Count - 1; k >= 0; k--)
                    {
                        if (_lsReasonSKU[k].SKUName == SKU && _lsReasonSKU[k].SKU_sequence == Convert.ToInt16(PName))
                        {
                            _mNewRMA.SetTransaction(Guid.NewGuid(), _lsReasonSKU[k].ReasonID, ReturnDetailsID);
                            _lsReasonSKU.RemoveAt(k);
                        }
                    }
                }

                foreach (Image imageCaptured in SpImages.Children)
                {
                    String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + imageCaptured.Name.ToString() + ".jpg";

                    //Set Images table from model.
                    Guid ImageID = _mNewRMA.SetReturnedImages(ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                }

                if (_SKU != "")
                {

                    wndSlipPrint slip = new wndSlipPrint();

                    Views.clGlobal.lsSlipInfo = _mNewRMA.GetSlipInfo(_lsreturn, _SKU, _mNewRMA.GetENACodeByItem(_SKU), "", _mNewRMA.GetNewROWID(ReturnTblID), cmbRMAStatus.SelectedIndex.ToString(), Views.clGlobal.SKU_Staus);

                    slip.ShowDialog();
                }
                Views.clGlobal.SKU_Staus = "";
                Views.clGlobal.TotalPoints = 0;
            }
            wndBoxInformation wndBox = new wndBoxInformation();
            clGlobal.IsUserlogged = true;
            this.Close();
            //close wait screen.
           // WindowThread.Stop();
            wndBox.Show(); 

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
        
     
        public String GetGuidChecked(DataGridRow Row)
        {
            String _return = "";
            try
            {
                ContentPresenter chkCp = dgReasons.Columns[0].GetCellContent(Row) as ContentPresenter;
                DataTemplate chkDt = chkCp.ContentTemplate;
                Border bdrChec = chkDt.FindName("bdrCheck", chkCp) as Border;
                TextBlock ResonGuid = dgReasons.Columns[1].GetCellContent(Row) as TextBlock;
                if (bdrChec.Background.ToString() == Colors.Black.ToString()) _return = ResonGuid.Text.ToString();
            }
            catch (Exception)
            {
            }
            return _return;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cvItemStatus.Visibility = System.Windows.Visibility.Hidden;

            int selectedIndex = dgPackageInfo.SelectedIndex;
            if (selectedIndex != -1)
            {


                DataGridCell cell = GetCell(selectedIndex, 4);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;

                TextBlock txtReturnGuid = (TextBlock)DataTemp.FindName("txtReasosnsGuid", CntPersenter);
                TextBlock txtRCount = (TextBlock)DataTemp.FindName("txtCheckedCount", CntPersenter);
                int countReasons = 0;
                txtReturnGuid.Text = "";
                foreach (DataGridRow RowReason in GetDataGridRows(dgReasons))
                {
                    string RGuid = GetGuidChecked(RowReason);
                    if (RGuid != "")
                    {
                        txtReturnGuid.Text += "#" + RGuid;
                        countReasons++;
                    }
                }
                txtRCount.Text = countReasons.ToString() + " Reason.";

            }
        }
        
        private void ChangeColor(CheckBox Chk, TextBlock txt, Canvas can)
        {
            if (Chk.IsChecked == false)
            {
                Chk.IsChecked = true;
                txt.Background = new SolidColorBrush(Colors.Black);
                can.Background = new SolidColorBrush(Color.FromRgb(121, 216, 66));

            }
            else if (Chk.IsChecked == true)
            {
                Chk.IsChecked = false;
                txt.Background = new SolidColorBrush(Colors.SkyBlue);
                can.Background = new SolidColorBrush(Color.FromRgb(198, 122, 58));
            }
        }
        
     
        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            if (clGlobal.Redirect == "Processed")
            {
               
                wndProcessedReturn processed = new wndProcessedReturn();
                processed.Show();
                this.Close();
            }
            else
            {
                wndBoxInformation boxinfo = new wndBoxInformation();
                clGlobal.IsUserlogged = true;
                boxinfo.Show();
                this.Close();
            }
        }

        public void FillRMAStausAndDecision()
        {
            cmbRMADecision.ItemsSource = _mNewRMA.GetRMADecision();
            cmbRMADecision.SelectedIndex = 0;
            cmbRMAStatus.ItemsSource = _mNewRMA.GetRMAStatusList();
            cmbRMAStatus.SelectedIndex = 0;
        }

        /// <summary>
        /// This is to all return DataGridRows Object
        /// </summary>
        /// <param name="grid"> Grid View object</param>
        /// <returns>DataGridRow</returns>
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

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            List<Reason> lsReturn = _mNewRMA.GetReasons();

            //add reason select to the Combobox other reason.
            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";
            lsReturn.Insert(0, re);
            cmbSkuReasons.ItemsSource = lsReturn;

            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Reason", typeof(string));
            dt.Columns.Add("Reason_Value", typeof(string));
            dt.Columns.Add("Points", typeof(int));
            dt.Columns.Add("ItemQuantity", typeof(string));

            // cmbOtherReason.ItemsSource = lsReturn;

            if (Views.clGlobal.IsAlreadySaved)
            {
                //txtPoNumber.Text = Views.clGlobal.Ponumber;
                //InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
                //txtPoNumber_KeyDown_1(txtPoNumber, new KeyEventArgs { });

               // List<RMAInfo> lsCustomeronfo = _mNewRMA.GetCustomer(Views.clGlobal.Ponumber);
                lstponumber.Visibility = Visibility.Hidden;

                brdPrint.Visibility = Visibility.Visible;

                _mUpdate = new mUpdateForNewRMA(Views.clGlobal.NewRGANumber); //mReturn.lsRMAInformation[0].RMANumber);

                for (int i = 0; i < _mUpdate._lsskuandpoints.Count; i++)
                {
                    DataRow dr0 = dt.NewRow();
                    dr0["SKU"] = _mUpdate._lsskuandpoints[i].SKU;
                    dr0["Reason"] = _mUpdate._lsskuandpoints[i].Reason;
                    dr0["Reason_Value"] = _mUpdate._lsskuandpoints[i].Reason_Value;
                    dr0["Points"] = _mUpdate._lsskuandpoints[i].Points;
                    dr0["ItemQuantity"] = _mUpdate._lsskuandpoints[i].SkuSequence;
                    dt.Rows.Add(dr0);
                }
                for (int i = 0; i < _mUpdate._lsReturnDetails1.Count; i++)
                {
                    StatusAndPoints _lsstatusandpoints = new StatusAndPoints();
                    if (_mUpdate._lsReturnDetails1[i].SKU_Status != "")
                    {
                        _lsstatusandpoints.SKUName = _mUpdate._lsReturnDetails1[i].SKUNumber;
                        _lsstatusandpoints.Status = _mUpdate._lsReturnDetails1[i].SKU_Status;
                        _lsstatusandpoints.Points = _mUpdate._lsReturnDetails1[i].SKU_Reason_Total_Points;
                        _lsstatusandpoints.IsMannually = _mUpdate._lsReturnDetails1[i].IsManuallyAdded;
                        _lsstatusandpoints.IsScanned = _mUpdate._lsReturnDetails1[i].IsSkuScanned;
                        _lsstatusandpoints.NewItemQuantity = _mUpdate._lsReturnDetails1[i].SKU_Sequence;
                        _lsstatusandpoints.skusequence = _mUpdate._lsReturnDetails1[i].SKU_Qty_Seq;
                        listofstatus.Add(_lsstatusandpoints);
                    }

                }


                if (_mUpdate._lsReturnDetails1.Count > 0)
                {
                    txtPoNumber.Text = _mUpdate._ReturnTbl1.PONumber;

                    lstponumber.Visibility = Visibility.Hidden;

                    txtVendorName.Text = _mUpdate._ReturnTbl1.VendoeName;//lsCustomeronfo[0].VendorName;
                    txtVendorNumber.Text = _mUpdate._ReturnTbl1.VendorNumber;
                    txtAddress.Text = _mUpdate._ReturnTbl1.Address1;
                    txtCountry.Text = _mUpdate._ReturnTbl1.Country;
                    txtCustCity.Text = _mUpdate._ReturnTbl1.City;
                    txtState.Text = _mUpdate._ReturnTbl1.State;
                    txtZipCode.Text = _mUpdate._ReturnTbl1.ZipCode;
                    txtName.Text = _mUpdate._ReturnTbl1.CustomerName1;
                    txtRMANumber.Text = _mUpdate._ReturnTbl1.RGAROWID;

                    cmbRMAStatus.SelectedIndex = Convert.ToInt16(_mUpdate._ReturnTbl1.RMAStatus);
                    cmbRMADecision.SelectedIndex = Convert.ToInt16(_mUpdate._ReturnTbl1.Decision);

                    for (int i = 0; i < _mUpdate._lsReturnDetails1.Count; i++)
                    {
                        dgPackageInfo.Items.Add(_mUpdate._lsReturnDetails1[i]);
                    }
                    
                    txtbarcode.Focus();
                    // _mponumber = new mPOnumberRMA(txtPoNumber.Text.ToUpper());
                    txtbarcode.Focus();

                }

            }

            dtLoadUpdate = new DispatcherTimer();
            dtLoadUpdate.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dtLoadUpdate.Tick += dtLoadUpdate_Tick;
            //start the dispacher.
            dtLoadUpdate.Start();


            FillRMAStausAndDecision();

            var data = new RDetails { SKUNumber = "", ProductName = "", SKU_Qty_Seq = "1", SKU_Status = "", SKU_Sequence = "1", ProductID = "", SalesPrice="" };

            dgPackageInfo.Items.Add(data);
            txtbarcode.Focus();



        }
        void dtLoadUpdate_Tick(object sender, EventArgs e)
        {
            dtLoadUpdate.Stop();
            //_showBarcode();
            txtbarcode.Text = "";
            txtbarcode.Focus();
            //set the all setting from update model.
            SetGridChack(dgPackageInfo);

        }

        protected void SetGridChack(DataGrid Grid)
        {
            try
            {
                //SetReasons(_mUpdate._ReturnTbl.ReturnReason);
                for (int i = 0; i < dgPackageInfo.Items.Count; i++)
                {


                    
                    DataGridRow row = (DataGridRow)dgPackageInfo.ItemContainerGenerator.ContainerFromIndex(i);

                    for (int j = 0; j < _mUpdate._lsReturnDetails1.Count(); j++)
                    {
                        DataGridCell cell = GetCell(i, 1);
                        ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                        DataTemplate DataTemp = CntPersenter.ContentTemplate;

                        SKU = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text.ToString();
                     
                        Button btnGreen = (Button)DataTemp.FindName("btnGreen", CntPersenter);
                        Button btnRed = (Button)DataTemp.FindName("btnRed", CntPersenter);

                        if (_mUpdate._lsReturnDetails1[j].SKUNumber == SKU)// && btnGreen.Visibility == System.Windows.Visibility.Hidden)
                        {
                            GreenRowsNumber1.Add(row.GetIndex());
                            // btnGreen.Visibility = System.Windows.Visibility.Visible;
                           // btnRed.Visibility = System.Windows.Visibility.Visible;

                            if (_mUpdate._lsReturnDetails1[j].SKU_Qty_Seq == 1)
                            {
                                // btnGreen.Visibility = System.Windows.Visibility.Visible; //row.IsEnabled = false;
                                row.Background = Brushes.SkyBlue;
                            }

                            //Images Stack Panel.
                            ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                            DataTemplate DtImages = CntImag.ContentTemplate;
                            StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                            foreach (var Imgitem in _mUpdate._lsImages1)
                            {
                                if (Imgitem.ReturnDetailID == _mUpdate._lsReturnDetails1[j].ReturnDetailID)
                                {
                                    try
                                    {
                                        BitmapSource bs = new BitmapImage(new Uri(Imgitem.SKUImagePath));

                                        Image img = new Image();
                                        //Zoom image.
                                        img.MouseEnter += img_MouseEnter;

                                        img.Height = 62;
                                        img.Width = 74;
                                        img.Stretch = Stretch.Fill;
                                        String Name = Imgitem.SKUImagePath.Remove(0, Imgitem.SKUImagePath.IndexOf("SR"));
                                        img.Name = Name.ToString().Split(new char[] { '.' })[0];
                                        img.Source = bs;
                                        img.Margin = new Thickness(0.5);

                                        //Images added to the Row.
                                        _addToStackPanel(SpImages, img);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception)
            { }
        }

        public class RDetails
        {
            public string SKUNumber { get; set; }
            public string ProductName { get; set; }
            public String SKU_Qty_Seq { get; set; }
            public String SKU_Status { get; set; }
            public string SKU_Sequence { get; set; }
            public string ProductID { get; set; }
            public string SalesPrice { get; set; }

        }

        private void cmbRMAStatus_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void tbQty_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var data = new RDetails { SKUNumber = "", ProductName = "", SKU_Qty_Seq = "1", SKU_Status = "", SKU_Sequence = "1", ProductID = "", SalesPrice = "" };

                dgPackageInfo.Items.Add(data);
            }
        }

        private void txtSKU_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            List<String> _lsNewRMAnumber = new List<string>();

            string che = ((System.Windows.Controls.TextBox)(e.Source)).Text.ToUpper();

            _lsNewRMAnumber = _mNewRMA.NewRMAInfo(che);

           // lstSKU.Visibility = Visibility.Visible;

            lstSKU.ItemsSource = _lsNewRMAnumber;

        }

        private void lstSKU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ListBox itembox = (ListBox)sender;

            if (itembox.SelectedItem != null)
            {
                string item = lstSKU.SelectedItem.ToString();

                string[] NewRMA = item.Split(new char[] { '#' });

                string NewSKU = NewRMA[0];
                string NewPName = NewRMA[1];
                string Category = NewRMA[2];

                int index = dgPackageInfo.SelectedIndex;

                DataGridCell cell = GetCell(index, 0);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;
                ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text = NewSKU;

                DataGridCell cell1 = GetCell(index, 1);
                ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
                DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;
                ((TextBox)DataTemp1.FindName("txtProductName", CntPersenter1)).Text = NewPName;

                DataGridCell cell2 = GetCell(index, 2);
                ContentPresenter CntPersenter2 = cell2.Content as ContentPresenter;
                DataTemplate DataTemp2 = CntPersenter2.ContentTemplate;
                ((TextBox)DataTemp2.FindName("tbQty", CntPersenter2)).Focus();

                InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);

                DataGridCell cell7 = GetCell(index, 5);
                ContentPresenter CntPersenter7 = cell7.Content as ContentPresenter;
                DataTemplate DataTemp7 = CntPersenter7.ContentTemplate;
                ((TextBox)DataTemp7.FindName("txtcategory", CntPersenter7)).Text = Category;

                lstSKU.Visibility = Visibility.Hidden;

            }
        }

        protected void _addToStackPanel(StackPanel StackPanelName, Image CapImage)
        {
            try
            {
                StackPanelName.Children.Add(CapImage);
            }
            catch (Exception)
            { }
        }
        
        void img_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            bdrZoomImage.Visibility = System.Windows.Visibility.Hidden;
            imgZoom.Source = img.Source;
        }

        private void imgZoom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgZoom.Source = null;
            bdrZoomImage.Visibility = System.Windows.Visibility.Hidden;
        }

        //StackPanel spRowImages;
        private void ContentControl_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("Images Capture By Camera Press  -  Yes\n\nBrowse From System Press - No", "Confirmation", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                ContentControl cnt = (ContentControl)sender;
                DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();

                StackPanel spRowImages = cnt.FindName("spProductImages") as StackPanel;

                if (GreenRowsNumber1.Contains(row.GetIndex()))
                {
                    try
                    {
                        //Show Camera.
                        Barcode.Camera.Open();
                        foreach (String Nameitem in Views.clGlobal.lsImageList)
                        {
                            try
                            {
                                string path = "C:\\Images\\";

                                BitmapSource bs = new BitmapImage(new Uri(path + Nameitem));

                                Image img = new Image();
                                //Zoom image.
                                img.MouseEnter += img_MouseEnter;

                                img.Height = 62;
                                img.Width = 74;
                                img.Stretch = Stretch.Fill;
                                img.Name = Nameitem.ToString().Split(new char[] { '.' })[0];
                                img.Source = bs;
                                img.Margin = new Thickness(0.5);

                                //Images added to the Row.
                                _addToStackPanel(spRowImages, img);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.SelectItem__00.ToString(), DateTime.UtcNow.ToString());
                    ErrorMsg("Please select the item.", Color.FromRgb(185, 84, 0));
                }
            }
            else if (result == MessageBoxResult.No)
            {

                ContentControl cnt = (ContentControl)sender;
                DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();

                StackPanel spRowImages = cnt.FindName("spProductImages") as StackPanel;

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".png";
                dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All files (*.*)|*.*";


                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result1 = dlg.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result1 == true)
                {
                    // Open document 
                    string filename = dlg.FileName;

                    string originalfilename = dlg.SafeFileName;

                    // textBox1.Text = filename;
                    //string path = "C:\\Images\\";

                    BitmapSource bs = new BitmapImage(new Uri(filename));

                    Image img = new Image();
                    //Zoom image.
                    img.MouseEnter += img_MouseEnter;

                    img.Height = 62;
                    img.Width = 74;
                    img.Stretch = Stretch.Fill;
                    img.Name = originalfilename.ToString().Split(new char[] { '.' })[0];
                    img.Source = bs;
                    img.Margin = new Thickness(0.5);

                    //Images added to the Row.
                    _addToStackPanel(spRowImages, img);

                }
            }
            else
            {
                // Cancel code here
            } 
        }

        private void ErrorMsg(string Msg, Color BgColor)
        {
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            bdrMsg.Visibility = System.Windows.Visibility.Visible;
            bdrMsg.Background = new SolidColorBrush(BgColor);
            txtError.Text = Msg;
        }
        private void dgPackageInfo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
          
        }

        private void dgPackageInfo_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
          //  dgPackageInfo.Items.RemoveAt(dgPackageInfo.SelectedIndex);
        }

        private void txtPoNumber_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
              List<RMAInfo> lsCustomeronfo=_mNewRMA.GetCustomer(txtPoNumber.Text);
              lstponumber.Visibility = Visibility.Hidden;   

              if (lsCustomeronfo.Count>0)
              {
                  txtAddress.Text = lsCustomeronfo[0].Address1;
                  txtCountry.Text = lsCustomeronfo[0].Country;
                  txtCustCity.Text = lsCustomeronfo[0].City;
                  txtState.Text = lsCustomeronfo[0].State;
                  txtZipCode.Text = lsCustomeronfo[0].ZipCode;
                  txtName.Text = lsCustomeronfo[0].CustomerName1;     
              }
            }
        }

        private void txtPoNumber_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (txtPoNumber.Text == "")
            {
                lstponumber.Visibility = Visibility.Hidden;
                txtPoNumber.Text = "";
                txtAddress.Text = "";
                txtCountry.Text = "";
                txtCustCity.Text = "";
                txtState.Text = "";
                txtZipCode.Text = "";
                txtName.Text = "";
            }
            else
            {
                lstponumber.ItemsSource = _mNewRMA.GetPOnumber(txtPoNumber.Text);
                lstponumber.Visibility = Visibility.Visible;
            }
        }

        private void lstponumber_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (lstponumber.SelectedItem!=null)
            {
                List<RMAInfo> lsCustomeronfo = _mNewRMA.GetCustomer(lstponumber.SelectedItem.ToString());

                if (lsCustomeronfo.Count > 0)
                {
                    txtPoNumber.Text = lsCustomeronfo[0].PONumber;
                    txtAddress.Text = lsCustomeronfo[0].Address1;
                    txtCountry.Text = lsCustomeronfo[0].Country;
                    txtCustCity.Text = lsCustomeronfo[0].City;
                    txtState.Text = lsCustomeronfo[0].State;
                    txtZipCode.Text = lsCustomeronfo[0].ZipCode;
                    txtName.Text = lsCustomeronfo[0].CustomerName1;
                }
                lstponumber.Visibility = Visibility.Hidden;    
            }
        }

        private void txtVendorNumber_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (Key.Down==Key.Enter)
            {
                 txtVendorName.Text = _mNewRMA.GetVenderNameByVenderNumber(txtVendorNumber.Text);
            }
            lstVenderNumber.Visibility = Visibility.Hidden;   
        }

        private void txtVendorNumber_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (txtVendorNumber.Text == "")
            {
                lstVenderNumber.Visibility = Visibility.Hidden;
                txtVendorName.Text = "";
            }
            else
            {
                lstVenderNumber.ItemsSource = _mNewRMA.GetVanderNumber(txtVendorNumber.Text.ToUpper());
                lstVenderNumber.Visibility = Visibility.Visible;
            }
        }

        private void lstVenderNumber_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (lstVenderNumber.SelectedItem!=null)
            {
                txtVendorNumber.Text = lstVenderNumber.SelectedItem.ToString();
                txtVendorName.Text = _mNewRMA.GetVenderNameByVenderNumber(txtVendorNumber.Text);
            }
            lstVenderNumber.Visibility = Visibility.Hidden;
            lstVenderName.Visibility = Visibility.Hidden;
        }

        private void lstVenderName_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (lstVenderName.SelectedItem != null)
            {
                txtVendorName.Text = lstVenderName.SelectedItem.ToString();
                txtVendorNumber.Text = _mNewRMA.GetVenderNumberByVenderName(txtVendorName.Text);
            }
            lstVenderName.Visibility = Visibility.Hidden;
            lstVenderNumber.Visibility = Visibility.Hidden;
        }

        private void txtVendorName_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (Key.Down == Key.Enter)
            {
                txtVendorNumber.Text = _mNewRMA.GetVenderNumberByVenderName(txtVendorName.Text);
            }
            lstVenderName.Visibility = Visibility.Hidden;   
        }

        private void txtVendorName_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (txtVendorName.Text == "")
            {
                lstVenderName.Visibility = Visibility.Hidden;
                txtVendorNumber.Text = "";
            }
            else
            {
                lstVenderName.ItemsSource = _mNewRMA.GetVenderName(txtVendorName.Text.ToUpper());
                lstVenderName.Visibility = Visibility.Visible;
            }
        }

        private void txtVendorNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            lstVenderNumber.Visibility = Visibility.Hidden;
        }

        private void txtVendorNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtVendorNumber.Text == "")
            {
                lstVenderNumber.Visibility = Visibility.Hidden;
               // txtVendorName.Text = "";
            }
            else
            {
                lstVenderNumber.ItemsSource = _mNewRMA.GetVanderNumber(txtVendorNumber.Text.ToUpper());
                lstVenderNumber.Visibility = Visibility.Visible;
            }
        }

        private void txtVendorName_LostFocus(object sender, RoutedEventArgs e)
        {
            lstVenderName.Visibility = Visibility.Hidden;
        }

        private void txtVendorName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtVendorName.Text == "")
            {
                lstVenderName.Visibility = Visibility.Hidden;
               // txtVendorNumber.Text = "";
            }
            else
            {
                lstVenderName.ItemsSource = _mNewRMA.GetVenderName(txtVendorName.Text.ToUpper());
                lstVenderName.Visibility = Visibility.Visible;
            }
        }

        private void txtPoNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtPoNumber.Text == "")
            {
                lstponumber.Visibility = Visibility.Hidden;
                txtPoNumber.Text = "";
                txtAddress.Text = "";
                txtCountry.Text = "";
                txtCustCity.Text = "";
                txtState.Text = "";
                txtZipCode.Text = "";
                txtName.Text = "";
            }
            else
            {
                lstponumber.ItemsSource = _mNewRMA.GetPOnumber(txtPoNumber.Text);
                lstponumber.Visibility = Visibility.Visible;
            }
        }

        private void txtPoNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            lstponumber.Visibility = Visibility.Hidden;
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string SelectedskuName = "";
            string ItemQuantity = "";

            for (int i = 0; i < dgPackageInfo.Items.Count; i++)
            {

                DataGridCell cell = GetCell(i, 0);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DtQty = CntPersenter.ContentTemplate;

                DataGridCell cell2 = GetCell(i, 1);
                ContentPresenter CntPersenter2 = cell2.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter2.ContentTemplate;

                SKU = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter2)).Text.ToString();


                DataGridCell cell3 = GetCell(i, 4);
                ContentPresenter CntPersenter3 = cell3.Content as ContentPresenter;
                DataTemplate DataTemp3 = CntPersenter3.ContentTemplate;
                string skuname3 = ((TextBlock)DataTemp3.FindName("tbDQyt", CntPersenter3)).Text.ToString();


                Button txtRetutn = (Button)DtQty.FindName("btnGreen", CntPersenter);

                if (txtRetutn.Visibility == Visibility.Visible)
                {
                    SelectedskuName = SKU;
                    ItemQuantity = skuname3;
                    DataGridRow row = (DataGridRow)dgPackageInfo.ItemContainerGenerator.ContainerFromIndex(i);
                    row.IsEnabled = false;
                    //dgPackageInfo.Items[i] = IsEnabled = true;
                }
            }
               DataRow dr = dt.NewRow();
                dr["SKU"] = SelectedskuName;
                dr["ItemQuantity"] = ItemQuantity;

                if (btnBoxNew.IsChecked == true)
                {
                    dr["Reason"] = lblItemIsNew.Content;
                    dr["Reason_Value"] = "Yes";
                    dr["Points"] = 100;
                    dt.Rows.Add(dr);
                }
                else if (btnBoxNotNew.IsChecked == true)
                {
                    dr["Reason"] = lblItemIsNew.Content;
                    dr["Reason_Value"] = "No";
                    dr["Points"] = 0;
                    dt.Rows.Add(dr);
                }

                DataRow dr1 = dt.NewRow();
                dr1["SKU"] = SelectedskuName;
                dr1["ItemQuantity"] = ItemQuantity;

                if (btnInstalledYes.IsChecked == true)
                {
                    dr1["Reason"] = lblInstalled.Content;
                    dr1["Reason_Value"] = "Yes";
                    dr1["Points"] = 0;
                    dt.Rows.Add(dr1);
                }
                else if (btnInstalledNo.IsChecked == true)
                {
                    dr1["Reason"] = lblInstalled.Content;
                    dr1["Reason_Value"] = "No";
                    dr1["Points"] = 100;
                    dt.Rows.Add(dr1);
                }

                DataRow dr2 = dt.NewRow();
                dr2["SKU"] = SelectedskuName;
                dr2["ItemQuantity"] = ItemQuantity;

                if (btnStatusYes.IsChecked == true)
                {
                    dr2["Reason"] = lblStatus.Content;
                    dr2["Reason_Value"] = "Yes";
                    dr2["Points"] = 0;
                    dt.Rows.Add(dr2);
                }
                else if (btnStatusNo.IsChecked == true)
                {
                    dr2["Reason"] = lblStatus.Content;
                    dr2["Reason_Value"] = "No";
                    dr2["Points"] = 100;
                    dt.Rows.Add(dr2);
                }

                DataRow dr3 = dt.NewRow();
                dr3["SKU"] = SelectedskuName;
                dr3["ItemQuantity"] = ItemQuantity;

                if (btnManufacturerYes.IsChecked == true)
                {
                    dr3["Reason"] = lblManufacturer.Content;
                    dr3["Reason_Value"] = "Yes";
                    dr3["Points"] = 100;
                    dt.Rows.Add(dr3);
                }
                else if (btnManufacturerNo.IsChecked == true)
                {
                    dr3["Reason"] = lblManufacturer.Content;
                    dr3["Reason_Value"] = "No";
                    dr3["Points"] = 0;
                    dt.Rows.Add(dr3);
                }

                DataRow dr4 = dt.NewRow();
                dr4["SKU"] = SelectedskuName;
                dr4["ItemQuantity"] = ItemQuantity;

                if (btntransiteYes.IsChecked == true)
                {
                    dr4["Reason"] = lblDefectontea.Content;
                    dr4["Reason_Value"] = "Yes";
                    dr4["Points"] = 100;
                    dt.Rows.Add(dr4);
                }
                else if (btntransiteNo.IsChecked == true)
                {
                    dr4["Reason"] = lblDefectontea.Content;
                    dr4["Reason_Value"] = "No";
                    dr4["Points"] = 0;
                    dt.Rows.Add(dr4);
                }
          



                StatusAndPoints _lsstatusandpoints = new StatusAndPoints();
                _lsstatusandpoints.SKUName = SelectedskuName;
                _lsstatusandpoints.Status = Views.clGlobal.SKU_Staus;
                _lsstatusandpoints.Points = Convert.ToInt16(lblpoints.Content);//Views.clGlobal.TotalPoints;
                _lsstatusandpoints.NewItemQuantity = Convert.ToInt16(ItemQuantity);
                _lsstatusandpoints.skusequence = 1;

              
                if (Views.clGlobal.IsAlreadySaved)
                {
                    for (int i = listofstatus.Count - 1; i >= 0; i--)
                    {
                        if (listofstatus[i].SKUName == SelectedskuName && listofstatus[i].NewItemQuantity == Convert.ToInt16(ItemQuantity))
                        {
                            listofstatus.RemoveAt(i);
                        }
                    }
                }

                _lsstatusandpoints.IsMannually = Views.clGlobal.IsManually;

                listofstatus.Add(_lsstatusandpoints);

                lblpoints.Content = "";
                points = 0;
                itemnew = true;
                IsStatus = true;
                IsManufacture = true;
                IsDefectiveTransite = true;
                ISinstalled = true;

                int ro = dt.Rows.Count;
                UncheckAllButtons();
                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));

                txtbarcode.Text = "";
                txtbarcode.Focus();
                btnAdd.IsEnabled = false;
                CanvasConditions.IsEnabled = false;

                #region SaveReasons
                Guid SkuReasonID = Guid.NewGuid();
                if (txtskuReasons.Text != "")
                {
                    SkuReasonID = _mNewRMA.SetReasons(txtskuReasons.Text);
                }
                else
                {
                    SkuReasonID = new Guid(cmbSkuReasons.SelectedValue.ToString());
                }

                SkuReasonIDSequence lsskusequenceReasons = new SkuReasonIDSequence();
                lsskusequenceReasons.ReasonID = SkuReasonID;
                lsskusequenceReasons.SKU_sequence = Convert.ToInt16(ItemQuantity);
                lsskusequenceReasons.SKUName = SelectedskuName;
                _lsReasonSKU.Add(lsskusequenceReasons);

                fillComboBox();

                cmbSkuReasons.SelectedIndex = 0;
                txtskuReasons.Text = "";

                #endregion


        }
        private void fillComboBox()
        {
            List<Reason> lsReturn = _mNewRMA.GetReasons();

            //add reason select to the Combobox other reason.
            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";
            lsReturn.Insert(0, re);
            cmbSkuReasons.ItemsSource = lsReturn;
        }

        Boolean itemnew = true;
       
        int points = 0;
        private void btnBoxNew_Checked_1(object sender, RoutedEventArgs e)
        {
            if (itemnew)
            {
                points = points + 100;
                lblpoints.Content = points.ToString();
                Views.clGlobal.SKU_Staus = "Refund";
            }
            else
            {

            }
            itemnew = false;
            btnInstalledNo.IsEnabled = false;
            btnInstalledYes.IsEnabled = false;
            btnStatusNo.IsEnabled = false;
            btnStatusYes.IsEnabled = false;

            btnInstalledNo.IsChecked = false;
            btnInstalledYes.IsChecked = false;
            btnStatusNo.IsChecked = false;
            btnStatusYes.IsChecked = false;

            btnBoxNotNew.IsChecked = false;

        }

        private void UncheckAllButtons()
        {
            btnBoxNew.IsChecked = false;
            btnBoxNotNew.IsChecked = false;
            btnInstalledYes.IsChecked = false;
            btnInstalledNo.IsChecked = false;
            btnStatusNo.IsChecked = false;
            btnStatusYes.IsChecked = false;
            btnManufacturerNo.IsChecked = false;
            btnManufacturerYes.IsChecked = false;
            btntransiteNo.IsChecked = false;
            btntransiteYes.IsChecked = false;

            btnManufacturerNo.IsEnabled = false;
            btnManufacturerYes.IsEnabled = false;
            btntransiteNo.IsEnabled = false;
            btntransiteYes.IsEnabled = false;
        }

        private void btnBoxNotNew_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!itemnew)
            {
                points = points - 100;
                lblpoints.Content = points.ToString();
                Views.clGlobal.SKU_Staus = "Deny";
            }
            else
            {
                points = points + 0;
                lblpoints.Content = points.ToString();
                Views.clGlobal.SKU_Staus = "Deny";
            }
            itemnew = true;
            btnInstalledNo.IsEnabled = true;
            btnInstalledYes.IsEnabled = true;
            btnStatusNo.IsEnabled = true;
            btnStatusYes.IsEnabled = true;

            btnBoxNew.IsChecked = false;
        }
        Boolean IsDefectiveTransite = true;
        private void btntransiteYes_Checked_1(object sender, RoutedEventArgs e)
        {
            if (IsDefectiveTransite)
            {
                points = points + 100;
                lblpoints.Content = points.ToString();
                if (btnBoxNotNew.IsChecked == true)
                {
                    Views.clGlobal.SKU_Staus = "Deny";
                }
                else
                {
                    Views.clGlobal.SKU_Staus = "Refund";
                }
            }
            else
            {

            }
            IsDefectiveTransite = false;

            btntransiteNo.IsChecked = false;
        }

        private void btntransiteNo_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsDefectiveTransite)
            {
                points = points - 100;
                lblpoints.Content = points.ToString();
            }
            else
            {
                points = points + 0;
                lblpoints.Content = points.ToString();
            }
            IsDefectiveTransite = true;

            btntransiteYes.IsChecked = false;

            // Views.clGlobal.SKU_Staus = "";

            if (btnManufacturerNo.IsChecked == true && btntransiteNo.IsChecked == true)
            {
                Views.clGlobal.SKU_Staus = "Deny";
            }
        }
        Boolean ISinstalled = true;
        private void btnInstalledYes_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!ISinstalled)
            {
                points = points - 100;
                lblpoints.Content = points.ToString();
                Views.clGlobal.SKU_Staus = "Deny";
                btnStatusNo.IsEnabled = false;
                btnStatusYes.IsEnabled = false;

                btnManufacturerNo.IsEnabled = false;
                btnManufacturerYes.IsEnabled = false;
                btntransiteNo.IsEnabled = false;
                btntransiteYes.IsEnabled = false;
            }
            else
            {
                points = points + 0;
                lblpoints.Content = points.ToString();
                Views.clGlobal.SKU_Staus = "Deny";
                btnStatusNo.IsEnabled = false;
                btnStatusYes.IsEnabled = false;

                btnManufacturerNo.IsEnabled = false;
                btnManufacturerYes.IsEnabled = false;
                btntransiteNo.IsEnabled = false;
                btntransiteYes.IsEnabled = false;
            }
            ISinstalled = true;

            btnInstalledNo.IsChecked = false;

           // ErrorMsg("This Item is Rejected.", Color.FromRgb(185, 84, 0));
            //btnStatusNo.IsEnabled = false;
            // btnStatusYes.IsEnabled = false;
            btnAdd.IsEnabled = true;
        }
        private void btnInstalledNo_Checked_1(object sender, RoutedEventArgs e)
        {
            if (ISinstalled)
            {
                points = points + 100;
                lblpoints.Content = points.ToString();
            }
            else
            {

            }
            ISinstalled = false;

            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            // btnAdd.IsEnabled = false;
            btnStatusNo.IsEnabled = true;
            btnStatusYes.IsEnabled = true;

            btnInstalledYes.IsChecked = false;
        }
        private void btnStatusNo_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsManufacture)
            {
                points = points - 100;
                lblpoints.Content = points.ToString();
            }
            if (!IsDefectiveTransite)
            {
                points = points - 100;
                lblpoints.Content = points.ToString();
            }
            if (IsStatus)
            {
                points = points + 100;
                lblpoints.Content = points.ToString();
            }
            IsStatus = false;
            if (btnBoxNotNew.IsChecked == true)
            {
                Views.clGlobal.SKU_Staus = "Deny";
            }
            else
            {
                Views.clGlobal.SKU_Staus = "Refund";
            }
            btnManufacturerNo.IsEnabled = false;
            btnManufacturerYes.IsEnabled = false;
            btntransiteNo.IsEnabled = false;
            btntransiteYes.IsEnabled = false;
            btnManufacturerNo.IsChecked = false;
            btnManufacturerYes.IsChecked = false;
            btntransiteNo.IsChecked = false;
            btntransiteYes.IsChecked = false;
            btnAdd.IsEnabled = true;

            btnStatusYes.IsChecked = false;

        }

        private void btnManufacturerYes_Checked(object sender, RoutedEventArgs e)
        {

            if (IsManufacture)
            {
                points = points + 100;
                lblpoints.Content = points.ToString();
                if (btnBoxNotNew.IsChecked == true)
                {
                    Views.clGlobal.SKU_Staus = "Deny";
                }
                else
                {
                    Views.clGlobal.SKU_Staus = "Refund";
                }
            }
            else
            {

            }
            IsManufacture = false;

            btnManufacturerNo.IsChecked = false;
        }

        private void btnManufacturerNo_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsManufacture)
            {
                points = points - 100;
                lblpoints.Content = points.ToString();
            }
            else
            {
                points = points + 0;
                lblpoints.Content = points.ToString();
            }
            IsManufacture = true;

            btnManufacturerYes.IsChecked = false;
            // Views.clGlobal.SKU_Staus = "";

            if (btnManufacturerNo.IsChecked == true && btntransiteNo.IsChecked == true)
            {
                Views.clGlobal.SKU_Staus = "Deny";
            }
        }
        Boolean IsStatus = true;
        Boolean IsManufacture = true;
        private void btnStatusYes_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsStatus)
            {
                points = points - 100;
                lblpoints.Content = points.ToString();
            }
            IsStatus = true;
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            //  btnAdd.IsEnabled = false;
            btnManufacturerNo.IsEnabled = true;
            btnManufacturerYes.IsEnabled = true;
            btntransiteNo.IsEnabled = true;
            btntransiteYes.IsEnabled = true;
            btnManufacturerNo.IsChecked = true;
            btnManufacturerYes.IsChecked = false;
            btntransiteNo.IsChecked = true;
            btntransiteYes.IsChecked = false;
            IsManufacture = true;
            IsDefectiveTransite = true;

            btnStatusNo.IsChecked = false;

        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < dgPackageInfo.Items.Count; i++)
            {

                DataGridCell cell = GetCell(i, 0);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DtQty = CntPersenter.ContentTemplate;
                Button txtRetutn = (Button)DtQty.FindName("btnGreen", CntPersenter);
                txtRetutn.Visibility = System.Windows.Visibility.Hidden;
                Button txtRetutn2 = (Button)DtQty.FindName("btnRed", CntPersenter);
                txtRetutn2.Visibility = System.Windows.Visibility.Visible;
                txtbarcode.Focus();
            }

            //Button txtsku = (Button)e.Source;
            //Canvas Sptextblock = (Canvas)txtsku.Parent;
            //TextBox skutext = Sptextblock.FindName("txtSKU") as TextBox;

            int index = dgPackageInfo.SelectedIndex;

            DataGridCell cell1 = GetCell(index, 1);
            ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
            DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;

           // DataGridRow row1 = (DataGridRow)skutext.FindParent<DataGridRow>();
            if (((TextBox)DataTemp1.FindName("txtSKU", CntPersenter1)).Text != "")
            {
                CanvasConditions.IsEnabled = true;
                btnAdd.IsEnabled = true;
                Button btnRed = (Button)e.Source;
                Canvas SpButtons = (Canvas)btnRed.Parent;
                Button btnGreen = SpButtons.FindName("btnGreen") as Button;
                btnGreen.Visibility = System.Windows.Visibility.Visible;
                btnRed.Visibility = System.Windows.Visibility.Hidden;

                DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();

                ContentPresenter Cntskustatus = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                DataTemplate Dtskustatus = Cntskustatus.ContentTemplate;
                TextBlock txtskustatus = (TextBlock)Dtskustatus.FindName("tbskustatus", Cntskustatus);

                ContentPresenter CntQuantity2 = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                TextBox txtRetutn2 = (TextBox)DtQty2.FindName("tbDQyt", CntQuantity2);

                if (row.Background == Brushes.SkyBlue)
                {
                    CanvasConditions.IsEnabled = true;

                    btnInstalledNo.IsChecked = true;
                    btnBoxNotNew.IsChecked = true;
                    btnStatusNo.IsChecked = true;


                    btnGreen.Visibility = System.Windows.Visibility.Visible;
                    btnRed.Visibility = System.Windows.Visibility.Hidden;
                }
                if (row.Background == Brushes.SkyBlue && txtskustatus.Text != "")
                {
                    CanvasConditions.IsEnabled = false;
                    string msg = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (((TextBox)DataTemp1.FindName("txtSKU", CntPersenter1)).Text == dt.Rows[i][0].ToString() && txtRetutn2.Text == dt.Rows[i][4].ToString())
                        {
                          //  msg = dt.Rows[i][1].ToString() + " : " + dt.Rows[i][2].ToString() + "\n" + msg;
                            if (dt.Rows[i][1].ToString() == "Item is New" && dt.Rows[i][2].ToString() == "Yes")
                            {
                                btnBoxNew.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Item is New" && dt.Rows[i][2].ToString() == "No"))
                            {
                                btnBoxNotNew.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Installed" && dt.Rows[i][2].ToString() == "Yes"))
                            {
                                btnInstalledYes.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Installed" && dt.Rows[i][2].ToString() == "No"))
                            {
                                btnInstalledNo.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Chip/Bended/Scratch/Broken" && dt.Rows[i][2].ToString() == "Yes"))
                            {
                                btnStatusYes.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Chip/Bended/Scratch/Broken" && dt.Rows[i][2].ToString() == "No"))
                            {
                                btnStatusNo.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Manufacturer Defective" && dt.Rows[i][2].ToString() == "Yes"))
                            {
                                btnManufacturerYes.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Manufacturer Defective" && dt.Rows[i][2].ToString() == "No"))
                            {
                                btnManufacturerNo.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Defect in Transite" && dt.Rows[i][2].ToString() == "Yes"))
                            {
                                btntransiteYes.IsChecked = true;
                            }
                            else if ((dt.Rows[i][1].ToString() == "Defect in Transite" && dt.Rows[i][2].ToString() == "No"))
                            {
                                btntransiteNo.IsChecked = true;
                            }
                        }
                    }

                   // MessageBox.Show(msg);
                }

                GreenRowsNumber1.Add(row.GetIndex());
                bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                txtbarcode.Focus();

                mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
            }
            else
            {
                CanvasConditions.IsEnabled = false;
            }


           
        }
        List<int> GreenRowsNumber1 = new List<int>();
        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            CanvasConditions.IsEnabled = false;
            btnAdd.IsEnabled = false;
            Button btnGreen = (Button)e.Source;
            Canvas SpButtons = (Canvas)btnGreen.Parent;
            Button btnRed = SpButtons.FindName("btnRed") as Button;
            btnGreen.Visibility = System.Windows.Visibility.Hidden;
            btnRed.Visibility = System.Windows.Visibility.Visible;

            DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
            GreenRowsNumber1.Remove(row.GetIndex());
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            txtbarcode.Focus();

            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_UnChecked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");

        }
        int max;
        private void txtbarcode_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtbarcode.Text.Trim() != "")
            {
                List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                int count=0;

                if (dgPackageInfo.Items.Count>1)
                {
                    count = dgPackageInfo.Items.Count-1;
                }


                Boolean check = false;
                for (int i = 0; i < dgPackageInfo.Items.Count; i++)
                {

                    DataGridCell cell = GetCell(i, 1);
                    ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                    DataTemplate DataTemp = CntPersenter.ContentTemplate;
                    string skuname = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text.ToString();

                    DataGridCell cell3 = GetCell(i, 4);
                    ContentPresenter CntPersenter3 = cell3.Content as ContentPresenter;
                    DataTemplate DataTemp3 = CntPersenter3.ContentTemplate;
                    string skuname3 = ((TextBlock)DataTemp3.FindName("tbDQyt", CntPersenter3)).Text.ToString();

                    string Str = txtbarcode.Text.TrimStart('0').ToString();
                    string sku = _mNewRMA.GetENACodeByItem(skuname);
                    if (sku == Str)
                    {
                        check = true;
                        if (max < Convert.ToInt16(skuname3))
                        {
                            max = Convert.ToInt16(skuname3);
                        }
                    }

                }

                DataGridCell cell4 = GetCell(count, 1);
                ContentPresenter CntPersenter4 = cell4.Content as ContentPresenter;
                DataTemplate DataTemp4 = CntPersenter4.ContentTemplate;
                ((TextBox)DataTemp4.FindName("txtSKU", CntPersenter4)).Text = _mNewRMA.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());


                DataGridCell cellforProductID= GetCell(count, 6);
                ContentPresenter CntPersenterforProductID = cellforProductID.Content as ContentPresenter;
                DataTemplate DataTempforProductID = CntPersenterforProductID.ContentTemplate;
                ((TextBox)DataTempforProductID.FindName("txtProductID", CntPersenterforProductID)).Text = _mNewRMA.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                DataGridCell cellforSales = GetCell(count, 7);
                ContentPresenter CntPersenterforSales = cellforSales.Content as ContentPresenter;
                DataTemplate DataTempforSales = CntPersenterforSales.ContentTemplate;
                ((TextBox)DataTempforSales.FindName("txtSalesPrice", CntPersenterforSales)).Text = "0";//_mNewRMA.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];


                DataGridRow row = (DataGridRow)dgPackageInfo.ItemContainerGenerator.ContainerFromIndex(count);
                row.Background = Brushes.SkyBlue;

                if (check)
                {
                    DataGridCell cell1 = GetCell(count, 4);
                    ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
                    DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;
                    ((TextBlock)DataTemp1.FindName("tbDQyt", CntPersenter1)).Text = (max + 1).ToString();

                }

                max = 0;

                txtbarcode.Text = "";
                txtbarcode.Focus();


                var data = new RDetails { SKUNumber = "", ProductName = "", SKU_Qty_Seq = "1", SKU_Status = "", SKU_Sequence = "1", ProductID = "", SalesPrice = "" };

                dgPackageInfo.Items.Add(data);
            }
        }
        private void cmbSkuReasons_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSkuReasons.SelectedIndex != 0)
            {
                txtskuReasons.Text = "";
            }
        }

        private void txtskuReasons_KeyDown_1(object sender, KeyEventArgs e)
        {
            cmbSkuReasons.SelectedIndex = 0;
        }

        private void btnPrint_Click_1(object sender, RoutedEventArgs e)
        {
            wndRMAFormPrint slip = new wndRMAFormPrint();
            clGlobal.NewRGANumber = _mUpdate._ReturnTbl1.RGAROWID;
            slip.ShowDialog();
        }
       
    }
}
