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
    /// Interaction logic for wndPONumber.xaml
    /// </summary>
    public partial class wndPONumber : Window
    {

        mNewRMANumber _mNewRMA = new mNewRMANumber();

        mPOnumberRMA _mponumber = new mPOnumberRMA();



        //  mReturnDetails _mReturn = clGlobal.mReturn;

        mUser _mUser = clGlobal.mCurrentUser;

        List<int> GreenRowsNumber = new List<int>();

        // Guid ReturnDetailsID;

        List<RMAInfo> _lsRMAInfo = new List<RMAInfo>();

        ScrollViewer SvImagesScroll;

        List<int> GreenRowsNumber1 = new List<int>();

        List<StatusAndPoints> listofstatus = new List<StatusAndPoints>();

        List<SkuAndIsScanned> lsskuIsScanned = new List<SkuAndIsScanned>();

        List<SkuAndIsScanned> lsIsManually = new List<SkuAndIsScanned>();

        DataTable dt = new DataTable();

        DispatcherTimer dtLoadUpdate;

        DispatcherTimer dtLoadUpdate1;

        DispatcherTimer dtLoadnormal;

        List<SkuReasonIDSequence> _lsReasonSKU = new List<SkuReasonIDSequence>();

        mupdatedForPonumber _mUpdate;

        // mReturnDetails _mReturn =clGlobal.mReturn;



        Boolean check = true;

        DateTime eastern = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Eastern Standard Time");

        StackPanel spRowImages;
        public wndPONumber()
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
        void img_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            bdrZoomImage.Visibility = System.Windows.Visibility.Visible;
            imgZoom.Source = img.Source;
        }
        private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentControl cnt = (ContentControl)sender;
            DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();

            StackPanel spRowImages = cnt.FindName("spProductImages") as StackPanel;

            if (GreenRowsNumber.Contains(row.GetIndex()))
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

        /// <summary>
        /// Add child to the stackPanel
        /// </summary>
        /// <param name="StackPanelName">
        /// StackPanel Name on which controls you want to add.
        /// </param>
        /// <param name="CapImage">
        /// Image Image name you want to add to the Stackpanel
        /// </param>
        protected void _addToStackPanel(StackPanel StackPanelName, Image CapImage)
        {
            try
            {
                StackPanelName.Children.Add(CapImage);
                SvImagesScroll.ScrollToRightEnd();
            }
            catch (Exception)
            { }
        }

        public void FilldgReasons(String cat)
        {
            dgReasons.ItemsSource = _mponumber.GetReasons(cat);
        }

        private void cntItemStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock cbk = (TextBlock)e.Source;
            Canvas cs = cbk.Parent as Canvas;
            TextBlock txtReasonGuids = cs.FindName("txtReasosnsGuid") as TextBlock;
            DataGridRow row = (DataGridRow)cbk.FindParent<DataGridRow>();

            if (GreenRowsNumber.Contains(row.GetIndex()))
            {
                cvItemStatus.Visibility = System.Windows.Visibility.Visible;
                TextBlock tbSKUName = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                txtSKUname.Text = tbSKUName.Text.ToString();
                FilldgReasons(tbSKUName.Text.ToString());
            }
            else
            {
                mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.SelectItem__00.ToString(), DateTime.UtcNow.ToString());
                ErrorMsg("Please select the item.", Color.FromRgb(185, 84, 0));
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
                    bdr.Background = new SolidColorBrush(Colors.Gray);
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
            int refundcount = 0;
            int denycount = 0;
            int listcount = listofstatus.Count;

            for (int i = 0; i < listcount; i++)
            {
                if (listofstatus[i].Status == "Refund" && cmbRMAStatus.SelectedIndex == 1)
                {
                    refundcount++;
                }
                if (listofstatus[i].Status == "Deny" && cmbRMAStatus.SelectedIndex == 1)
                {
                    denycount++;
                }

            }

            if (listcount == refundcount)
            {
                cmbRMADecision.SelectedIndex = 2;
            }
            else if (listcount > refundcount)
            {
                cmbRMADecision.SelectedIndex = 3;
            }

            if (denycount == refundcount)
            {
                cmbRMADecision.SelectedIndex = 1;
            }

            if (cmbRMAStatus.SelectedIndex == 2)
            {
                cmbRMADecision.SelectedIndex = 1;
            }

            if (cmbRMAStatus.SelectedIndex == 0)
            {
                cmbRMADecision.SelectedIndex = 1;
            }


            Byte RMAStatus = Convert.ToByte(cmbRMAStatus.SelectedValue.ToString());
            Byte Decision = Convert.ToByte(cmbRMADecision.SelectedValue.ToString());
            DateTime ScannedDate = DateTime.UtcNow;
            DateTime ExpirationDate = DateTime.UtcNow.AddDays(60);

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
            ret.DeliveryDate = _mponumber.lsRMAInformationforponumner[0].DeliveryDate;
            ret.OrderDate = _mponumber.lsRMAInformationforponumner[0].OrderDate;
            ret.Address2 = _mponumber.lsRMAInformationforponumner[0].Address2;
            ret.Address3 = _mponumber.lsRMAInformationforponumner[0].Address3;
            ret.OrderNumber = _mponumber.lsRMAInformationforponumner[0].OrderNumber;


            _lsreturn.Add(ret);


            string wrongRMA = "";
            string Warranty = "";
            if (Views.clGlobal.ScenarioType == "Lowes")
            {
                wrongRMA = "N/A";
                Warranty = "N/A";
            }
            if (Views.clGlobal.ScenarioType == "HomeDepot")
            {
                wrongRMA = Views.clGlobal.WrongRMAFlag;
                Warranty = Views.clGlobal.Warranty;
            }
            if (Views.clGlobal.ScenarioType == "Others")
            {
                wrongRMA = Views.clGlobal.WrongRMAFlag;
                Warranty = Views.clGlobal.Warranty;
            }



            //Save to RMA Master Table.
            Guid ReturnTblID = _mponumber.SetReturnTbl(_lsreturn, "", RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID, wrongRMA, Warranty, 60, Views.clGlobal.ShipDate_ScanDate_Diff);

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

            foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
            {

                ContentPresenter CntPersenter = dgPackageInfo.Columns[0].GetCellContent(row) as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;
                Button btnGreen = (Button)DataTemp.FindName("btnGreen", CntPersenter);

                if (Views.clGlobal.ScenarioType == "Lowes")
                {

                    //if (btnGreen.Visibility == System.Windows.Visibility.Visible)
                    //{
                    // If item present in the return 
                    // item SKUNumber
                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                    //Product Name.
                    // TextBlock ProcutName = dgPackageInfo.Columns[2].GetCellContent(row) as TextBlock;

                    //item Returned Quantity.
                    ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty = CntQuantity.ContentTemplate;
                    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbDQyt", CntQuantity1);

                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                    TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row) as TextBlock;

                    TextBlock SalesPrice = dgPackageInfo.Columns[8].GetCellContent(row) as TextBlock;

                    TextBlock LineType = dgPackageInfo.Columns[9].GetCellContent(row) as TextBlock;

                    //Returned RMA Information.
                    RMAInfo rmaInfo = _mponumber.lsRMAInformationforponumner.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
                    int DeliveredQty;
                    int ExpectedQty;
                    string tck;
                    if (rmaInfo == null)
                    {
                        DeliveredQty = 0;//rmaInfo.DeliveredQty;
                        ExpectedQty = 0;//rmaInfo.ExpectedQty;
                        tck = "";
                    }
                    else
                    {
                        DeliveredQty = rmaInfo.DeliveredQty;
                        ExpectedQty = rmaInfo.ExpectedQty;
                        tck = rmaInfo.TCLCOD_0;
                    }

                    //Set returned details table.

                    for (int i = 0; i < lsskuIsScanned.Count; i++)
                    {
                        if (lsskuIsScanned[i].SKUName == SkuNumber.Text)
                        {
                            Views.clGlobal.IsScanned = lsskuIsScanned[i].IsScanned;
                            break;
                        }
                    }
                    for (int i = 0; i < lsIsManually.Count; i++)
                    {
                        if (lsIsManually[i].SKUName == SkuNumber.Text)
                        {
                            Views.clGlobal.IsManually = lsIsManually[i].IsScanned;
                            break;
                        }
                    }


                    string SKUNewName = "";
                    if (listofstatus.Count > 0)
                    {
                        for (int i = listofstatus.Count - 1; i >= 0; i--)
                        {
                            if (listofstatus[i].SKUName == SkuNumber.Text && listofstatus[i].NewItemQuantity == Convert.ToInt16(txtRetutn1.Text))
                            {
                                SKUNewName = SkuNumber.Text;
                                Views.clGlobal.SKU_Staus = listofstatus[i].Status;
                                Views.clGlobal.TotalPoints = listofstatus[i].Points;
                                Views.clGlobal.IsScanned = listofstatus[i].IsScanned;
                                Views.clGlobal.IsManually = listofstatus[i].IsMannually;
                                Views.clGlobal.NewItemQty = listofstatus[i].NewItemQuantity;
                                Views.clGlobal._SKU_Qty_Seq = listofstatus[i].skusequence;

                                listofstatus.RemoveAt(i);

                                break;
                            }
                        }
                    }
                    else
                    {
                        SKUNewName = SkuNumber.Text;
                        Views.clGlobal.SKU_Staus = "Reject";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.IsScanned = 1;
                        Views.clGlobal.IsManually = 1;
                        Views.clGlobal.NewItemQty = 1;
                        Views.clGlobal._SKU_Qty_Seq = 1;

                    }

                    if (row.Background == Brushes.Gray)
                    {
                        Views.clGlobal.SKU_Staus = "Refund";
                    }

                    if (LineType.Text == "6")
                    {
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.IsScanned = 1;
                        Views.clGlobal.IsManually = 1;
                        Views.clGlobal.NewItemQty = 1;
                        Views.clGlobal._SKU_Qty_Seq = 0;
                        txtRetutn.Text = "0";
                    }


                    Guid ReturnDetailsID = _mponumber.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, 0, Views.clGlobal.IsScanned, Views.clGlobal.IsManually, Convert.ToInt16(txtRetutn1.Text), Convert.ToInt16(txtRetutn.Text), ProductID.Text, Convert.ToDecimal(SalesPrice.Text), Convert.ToInt16(LineType.Text));
                    Views.clGlobal.IsScanned = 0;
                    Views.clGlobal.IsManually = 0;

                    Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0, 0);

                    

                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mponumber.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    //SKU Reasons Table
                    //foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                    //{
                    //    _mponumber.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                    //}

                    if (LineType.Text != "6")
                    {
                        wndSlipPrint slip = new wndSlipPrint();

                        Views.clGlobal.lsSlipInfo = _mponumber.GetSlipInfo(_lsreturn, SkuNumber.Text, _mNewRMA.GetENACodeByItem(SkuNumber.Text), "", _mNewRMA.GetNewROWID(ReturnTblID), cmbRMAStatus.SelectedIndex.ToString(), "Refund");

                        slip.ShowDialog();
                    }

                    

                    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                    Views.AuditType.lsaudit.Clear();
                    // }
                }

                if (Views.clGlobal.ScenarioType == "HomeDepot")
                {
                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                    //Product Name.
                    // TextBlock ProcutName = dgPackageInfo.Columns[2].GetCellContent(row) as TextBlock;

                    //item Returned Quantity.
                    ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty = CntQuantity.ContentTemplate;
                    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbDQyt", CntQuantity1);


                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                    TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row) as TextBlock;

                    TextBlock SalesPrice = dgPackageInfo.Columns[8].GetCellContent(row) as TextBlock;

                    TextBlock LineType = dgPackageInfo.Columns[9].GetCellContent(row) as TextBlock;

                    //item Status.k
                    //ContentPresenter CntStatus = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                    //DataTemplate DtStatus = CntStatus.ContentTemplate;
                    //TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;

                    //Returned RMA Information.
                    RMAInfo rmaInfo = _mponumber.lsRMAInformationforponumner.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
                    int DeliveredQty;
                    int ExpectedQty;
                    string tck;
                    if (rmaInfo == null)
                    {
                        DeliveredQty = 0;//rmaInfo.DeliveredQty;
                        ExpectedQty = 0;//rmaInfo.ExpectedQty;
                        tck = "";
                    }
                    else
                    {
                        DeliveredQty = rmaInfo.DeliveredQty;
                        ExpectedQty = rmaInfo.ExpectedQty;
                        tck = rmaInfo.TCLCOD_0;
                    }

                    string SKUNewName = "";
                    Boolean checkflag = false;
                    if (listofstatus.Count > 0)
                    {
                        for (int i = listofstatus.Count - 1; i >= 0; i--)
                        {
                            if (listofstatus[i].SKUName == SkuNumber.Text && listofstatus[i].NewItemQuantity == Convert.ToInt16(txtRetutn1.Text))
                            {
                                SKUNewName = SkuNumber.Text;
                                Views.clGlobal.SKU_Staus = listofstatus[i].Status;
                                Views.clGlobal.TotalPoints = listofstatus[i].Points;
                                Views.clGlobal.IsScanned = listofstatus[i].IsScanned;
                                Views.clGlobal.IsManually = listofstatus[i].IsMannually;
                                Views.clGlobal.NewItemQty = listofstatus[i].NewItemQuantity;
                                Views.clGlobal._SKU_Qty_Seq = listofstatus[i].skusequence;

                                listofstatus.RemoveAt(i);
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
                        SKUNewName = SkuNumber.Text;
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.IsScanned = 1;
                        Views.clGlobal.IsManually = 1;
                        Views.clGlobal.NewItemQty = 1;
                        Views.clGlobal._SKU_Qty_Seq = 0;

                    }

                    //Set returned details table.

                    if (Views.clGlobal.Warranty == "0")
                    {
                        Views.clGlobal.SKU_Staus = "Deny";
                    }

                    if (LineType.Text == "6")
                    {
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.IsScanned = 1;
                        Views.clGlobal.IsManually = 1;
                        Views.clGlobal.NewItemQty = 1;
                        Views.clGlobal._SKU_Qty_Seq = 0;
                        txtRetutn.Text = "0";
                    }

                    Guid ReturnDetailsID = _mponumber.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints, Views.clGlobal.IsScanned, Views.clGlobal.IsManually, Views.clGlobal.NewItemQty, Views.clGlobal._SKU_Qty_Seq, ProductID.Text, Convert.ToDecimal(SalesPrice.Text),Convert.ToInt16(LineType.Text));

                    Views.clGlobal.IsScanned = 0;
                    Views.clGlobal.IsManually = 0;

                    // j++;

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow d = dt.Rows[i];
                            if (d["SKU"].ToString() == SkuNumber.Text && d["ItemQuantity"].ToString() == txtRetutn1.Text)
                            {
                                Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), Convert.ToInt16(dt.Rows[i][3].ToString()), Convert.ToInt16(dt.Rows[i][4].ToString()));
                                d.Delete();
                            }
                        }
                    }
                    else
                    {
                        if (check)
                        {
                            Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0, 0);
                        }
                    }

                    if (_lsReasonSKU.Count > 0)
                    {
                        for (int i = _lsReasonSKU.Count - 1; i >= 0; i--)
                        {
                            if (_lsReasonSKU[i].SKUName == SkuNumber.Text && _lsReasonSKU[i].SKU_sequence == Convert.ToInt16(txtRetutn1.Text))
                            {
                                _mNewRMA.SetTransaction(Guid.NewGuid(), _lsReasonSKU[i].ReasonID, ReturnDetailsID);
                                _lsReasonSKU.RemoveAt(i);
                            }
                        }
                    }




                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mponumber.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    //SKU Reasons Table
                    //foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                    //{
                    //    _mponumber.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                    //}

                    if (LineType.Text != "6")
                    {
                        wndSlipPrint slip = new wndSlipPrint();

                        Views.clGlobal.lsSlipInfo = _mponumber.GetSlipInfo(_lsreturn, SkuNumber.Text, _mNewRMA.GetENACodeByItem(SkuNumber.Text), "", _mNewRMA.GetNewROWID(ReturnTblID), cmbRMAStatus.SelectedIndex.ToString(), Views.clGlobal.SKU_Staus);

                        slip.ShowDialog();


                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                    }

               
                    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                    Views.AuditType.lsaudit.Clear();
                }


                if (Views.clGlobal.ScenarioType == "Others")
                {
                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                    //Product Name.
                    // TextBlock ProcutName = dgPackageInfo.Columns[2].GetCellContent(row) as TextBlock;

                    //item Returned Quantity.
                    ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty = CntQuantity.ContentTemplate;
                    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbDQyt", CntQuantity1);



                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                    TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row) as TextBlock;

                    TextBlock SalesPrice = dgPackageInfo.Columns[8].GetCellContent(row) as TextBlock;

                    TextBlock LineType = dgPackageInfo.Columns[9].GetCellContent(row) as TextBlock;
                    //item Status.k
                    //ContentPresenter CntStatus = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                    //DataTemplate DtStatus = CntStatus.ContentTemplate;
                    //TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;

                    //Returned RMA Information.
                    RMAInfo rmaInfo = _mponumber.lsRMAInformationforponumner.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
                    int DeliveredQty;
                    int ExpectedQty;
                    string tck;
                    if (rmaInfo == null)
                    {
                        DeliveredQty = 0;//rmaInfo.DeliveredQty;
                        ExpectedQty = 0;//rmaInfo.ExpectedQty;
                        tck = "";
                    }
                    else
                    {
                        DeliveredQty = rmaInfo.DeliveredQty;
                        ExpectedQty = rmaInfo.ExpectedQty;
                        tck = rmaInfo.TCLCOD_0;
                    }

                    //Set returned details table.

                    string SKUNewName = "";
                    Boolean checkflag = false;
                    if (listofstatus.Count > 0)
                    {
                        for (int i = listofstatus.Count - 1; i >= 0; i--)
                        {
                            if (listofstatus[i].SKUName == SkuNumber.Text && listofstatus[i].NewItemQuantity == Convert.ToInt16(txtRetutn1.Text))
                            {
                                SKUNewName = SkuNumber.Text;
                                Views.clGlobal.SKU_Staus = listofstatus[i].Status;
                                Views.clGlobal.TotalPoints = listofstatus[i].Points;
                                Views.clGlobal.IsScanned = listofstatus[i].IsScanned;
                                Views.clGlobal.IsManually = listofstatus[i].IsMannually;
                                Views.clGlobal.NewItemQty = listofstatus[i].NewItemQuantity;
                                Views.clGlobal._SKU_Qty_Seq = listofstatus[i].skusequence;

                                listofstatus.RemoveAt(i);
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
                        SKUNewName = SkuNumber.Text;
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.IsScanned = 1;
                        Views.clGlobal.IsManually = 1;
                        Views.clGlobal.NewItemQty = 1;
                        Views.clGlobal._SKU_Qty_Seq = 0;

                    }
                    if (Views.clGlobal.Warranty == "0")
                    {
                        Views.clGlobal.SKU_Staus = "Deny";
                    }

                    if (LineType.Text == "6")
                    {
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.IsScanned = 1;
                        Views.clGlobal.IsManually = 1;
                        Views.clGlobal.NewItemQty = 1;
                        Views.clGlobal._SKU_Qty_Seq = 0;
                        txtRetutn.Text = "0";
                    }

                    Guid ReturnDetailsID = _mponumber.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints, Views.clGlobal.IsScanned, Views.clGlobal.IsManually, Views.clGlobal.NewItemQty, Views.clGlobal._SKU_Qty_Seq, ProductID.Text, Convert.ToDecimal(SalesPrice.Text), Convert.ToInt16(LineType.Text));

                    Views.clGlobal.IsScanned = 0;
                    Views.clGlobal.IsManually = 0;


                    if (dt.Rows.Count > 0)
                    {
                        for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow d = dt.Rows[i];
                            if (d["SKU"].ToString() == SkuNumber.Text && d["ItemQuantity"].ToString() == txtRetutn1.Text)
                            {
                                Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), Convert.ToInt16(dt.Rows[i][3].ToString()), Convert.ToInt16(dt.Rows[i][4].ToString()));
                                d.Delete();
                            }
                        }
                    }
                    else
                    {
                        if (check)
                        {
                            Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0, 0);
                        }
                    }




                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mponumber.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    //SKU Reasons Table
                    //foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                    //{
                    //    _mponumber.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                    //}

                    if (LineType.Text != "6")
                    {
                        wndSlipPrint slip = new wndSlipPrint();

                        Views.clGlobal.lsSlipInfo = _mponumber.GetSlipInfo(_lsreturn, SkuNumber.Text, _mNewRMA.GetENACodeByItem(SkuNumber.Text), "", _mNewRMA.GetNewROWID(ReturnTblID), cmbRMAStatus.SelectedIndex.ToString(), Views.clGlobal.SKU_Staus);

                        slip.ShowDialog();
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;

                    }

                    
                    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                    Views.AuditType.lsaudit.Clear();
                }
            }
            listofstatus.Clear();
            lsIsManually.Clear();
            lsskuIsScanned.Clear();
            Views.clGlobal.Ponumber = "";
            Views.clGlobal.IsAlreadySaved = false;
            Views.clGlobal.SKU_Staus = "";
            Views.clGlobal.TotalPoints = 0;
            Views.clGlobal.SKU_Staus = "";
            Views.clGlobal.TotalPoints = 0;
            Views.clGlobal.Warranty = "";
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
                foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                {
                    if (row.IsSelected)
                    {
                        ContentPresenter cp = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                        DataTemplate Dt = cp.ContentTemplate;
                        TextBlock txtReturnGuid = (TextBlock)Dt.FindName("txtReasosnsGuid", cp);
                        TextBlock txtRCount = (TextBlock)Dt.FindName("txtCheckedCount", cp);
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
                txt.Background = new SolidColorBrush(Colors.Gray);
                can.Background = new SolidColorBrush(Color.FromRgb(198, 122, 58));
            }
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            Views.clGlobal.Ponumber = "";
            Views.clGlobal.IsAlreadySaved = false;
            wndBoxInformation boxinfo = new wndBoxInformation();
            clGlobal.IsUserlogged = true;
            boxinfo.Show();
            this.Close();
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


            //Reason re = new Reason();
            //re.ReasonID = Guid.NewGuid();
            //re.Reason1 = "--Select--";

            //lsReturn.Insert(0, re);

            //cmbOtherReason.ItemsSource = lsReturn;

            FillRMAStausAndDecision();

            var data = new RDetails { SKU = "", ProductName = "", Quantity = "1", cat = "" };

            dgPackageInfo.Items.Add(data);



        }

        public class RDetails
        {
            public string SKU { get; set; }
            public string ProductName { get; set; }
            public String Quantity { get; set; }
            public String cat { get; set; }
        }

        private void cmbRMAStatus_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txtPoNumber_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {


                this.Dispatcher.Invoke(new Action(() =>
                {
                    _mponumber.mPOnumberRMA1(txtPoNumber.Text.ToUpper());
                }));

                #region Update mode RMA.
                //RMA number is already present in the database.
                if (Views.clGlobal.IsAlreadySaved)
                {
                    //Get the all information from datebase to the Update mode from RMA Number.
                    List<RMAInfo> lsCustomeronfo = _mNewRMA.GetCustomer(txtPoNumber.Text);
                    lstponumber.Visibility = Visibility.Hidden;
                    _mponumber.lsRMAInformationforponumner = lsCustomeronfo;

                    this.Dispatcher.Invoke(new Action(() => { _mUpdate = new mupdatedForPonumber(_mponumber.lsRMAInformationforponumner[0].PONumber); }));


                    //_mUpdate = new mupdatedForPonumber(_mponumber.lsRMAInformationforponumner[0].PONumber); //mReturn.lsRMAInformation[0].RMANumber);


                    this.Dispatcher.Invoke(new Action(() =>
                    {
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

                    }));

                    this.Dispatcher.Invoke(new Action(() =>
                    {
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

                    }));

                    if (lsCustomeronfo.Count > 0)
                    {
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

                        this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _mUpdate._lsReturnDetails1; }));
                        _lsRMAInfo = lsCustomeronfo;
                        _mponumber.lsRMAInformationforponumner = lsCustomeronfo;

                        _mponumber.mPOnumberRMA1(txtPoNumber.Text.ToUpper());

                        // _mponumber = new mPOnumberRMA(txtPoNumber.Text.ToUpper());
                        txtbarcode.Focus();
                        if (lsCustomeronfo[0].VendorNumber.ToString() == "GENC0001" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0404" || lsCustomeronfo[0].VendorNumber.ToString() == "INTC0017" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0551" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0795")
                        {
                            Views.clGlobal.ScenarioType = "HomeDepot";
                            txtbarcode.Focus();
                            ErrorMsg("Please Check this RMA is Wrong or Not By Scanning the Barcode.", Color.FromRgb(185, 84, 0));


                            DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                            DateTime CurrentDate = DateTime.UtcNow;
                            TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                            int Days = Diff.Days;
                            Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                            if (Days <= 60)
                            {
                                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "1";
                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                            }
                            else
                            {
                                ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "0";
                                Views.clGlobal.SKU_Staus = "Deny";
                                Views.clGlobal.TotalPoints = points;
                                Views.clGlobal.WrongRMAFlag = "N/A";

                                cmbRMAStatus.SelectedIndex = 2;

                                MessageBox.Show("This Return is NOT in Warranty.");

                                // btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });
                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                            }

                        }

                        else if (lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0143" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0432")
                        {
                            Views.clGlobal.ScenarioType = "Lowes";
                            ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                            txtbarcode.Focus();
                            //dgPackageInfo.IsEnabled = false;
                        }
                        else
                        {
                            Views.clGlobal.ScenarioType = "Others";
                            txtbarcode.Focus();

                            Views.clGlobal.WrongRMAFlag = "0";
                            DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                            DateTime CurrentDate = DateTime.UtcNow;
                            TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                            int Days = Diff.Days;
                            Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                            if (Days <= 60)
                            {
                                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "1";
                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                            }
                            else
                            {
                                ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "0";
                                Views.clGlobal.SKU_Staus = "Deny";
                                Views.clGlobal.TotalPoints = points;
                                Views.clGlobal.WrongRMAFlag = "N/A";

                                cmbRMAStatus.SelectedIndex = 2;


                                MessageBox.Show("This Return is NOT in Warranty.");

                                //  btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                            }
                            _mponumber.lsRMAInformationforponumner = lsCustomeronfo;
                        }
                        //Initialize the Dispacher that shows all values from the Update model.
                        dtLoadUpdate = new DispatcherTimer();
                        dtLoadUpdate.Interval = new TimeSpan(0, 0, 0, 0, 100);
                        dtLoadUpdate.Tick += dtLoadUpdate_Tick;
                        //start the dispacher.
                        dtLoadUpdate.Start();
                    }

                #endregion

                }
                else
                {

                    List<RMAInfo> lsCustomeronfo = new List<RMAInfo>();
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        lsCustomeronfo = _mNewRMA.GetCustomer(txtPoNumber.Text);
                    }));// List<RMAInfo> lsCustomeronfo = _mNewRMA.GetCustomer(txtPoNumber.Text);
                    lstponumber.Visibility = Visibility.Hidden;


                    if (lsCustomeronfo.Count > 0)
                    {
                        txtVendorName.Text = lsCustomeronfo[0].VendorName;
                        txtVendorNumber.Text = lsCustomeronfo[0].VendorNumber;
                        txtAddress.Text = lsCustomeronfo[0].Address1;
                        txtCountry.Text = lsCustomeronfo[0].Country;
                        txtCustCity.Text = lsCustomeronfo[0].City;
                        txtState.Text = lsCustomeronfo[0].State;
                        txtZipCode.Text = lsCustomeronfo[0].ZipCode;
                        txtName.Text = lsCustomeronfo[0].CustomerName1;

                        //cmbRMAStatus.SelectedIndex = Convert.ToInt16(_mUpdate._ReturnTbl1.RMAStatus);
                        //cmbRMADecision.SelectedIndex = Convert.ToInt16(lsCustomeronfo[0].);

                        //txtRMANumber.Text=_mUpdate.
                        // dgPackageInfo.ItemsSource = lsCustomeronfo;
                        this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = lsCustomeronfo; }));
                        _lsRMAInfo = lsCustomeronfo;
                        _mponumber.lsRMAInformationforponumner = lsCustomeronfo;


                        this.Dispatcher.Invoke(new Action(() => { _mponumber.mPOnumberRMA1(txtPoNumber.Text.ToUpper()); }));



                        // _mponumber = new mPOnumberRMA(txtPoNumber.Text.ToUpper());
                        txtbarcode.Focus();
                        if (lsCustomeronfo[0].VendorNumber.ToString() == "GENC0001" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0404" || lsCustomeronfo[0].VendorNumber.ToString() == "INTC0017" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0551" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0795")
                        {
                            Views.clGlobal.ScenarioType = "HomeDepot";
                            txtbarcode.Focus();
                            ErrorMsg("Please Check this RMA is Wrong or Not By Scanning the Barcode.", Color.FromRgb(185, 84, 0));


                            DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                            DateTime CurrentDate = DateTime.UtcNow;
                            TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                            int Days = Diff.Days;
                            Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                            if (Days <= 60)
                            {
                                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "1";
                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                            }
                            else
                            {
                                ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "0";
                                Views.clGlobal.SKU_Staus = "Deny";
                                Views.clGlobal.TotalPoints = points;
                                Views.clGlobal.WrongRMAFlag = "N/A";

                                cmbRMAStatus.SelectedIndex = 2;
                                MessageBox.Show("This Return is NOT in Warranty.");

                                // btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });
                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                            }

                        }

                        else if (lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0143" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0432")
                        {
                            Views.clGlobal.ScenarioType = "Lowes";
                            ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                            txtbarcode.Focus();
                            //dgPackageInfo.IsEnabled = false;
                        }
                        else
                        {
                            Views.clGlobal.ScenarioType = "Others";
                            txtbarcode.Focus();

                            Views.clGlobal.WrongRMAFlag = "0";
                            DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                            DateTime CurrentDate = DateTime.UtcNow;
                            TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                            int Days = Diff.Days;
                            Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                            if (Days <= 60)
                            {
                                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "1";
                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                            }
                            else
                            {
                                ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "0";
                                Views.clGlobal.SKU_Staus = "Deny";
                                Views.clGlobal.TotalPoints = points;
                                Views.clGlobal.WrongRMAFlag = "N/A";

                                cmbRMAStatus.SelectedIndex = 2;

                                MessageBox.Show("This Return is NOT in Warranty.");

                                // btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                            }
                            _mponumber.lsRMAInformationforponumner = lsCustomeronfo;
                        }
                    }

                    //Initialize the Dispacher that shows all values from the Update model.
                    dtLoadnormal = new DispatcherTimer();
                    dtLoadnormal.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    dtLoadnormal.Tick += dtLoadnormal_Tick;
                    //start the dispacher.
                    dtLoadnormal.Start();
                }
            }
        }
        void dtLoadUpdate_Tick(object sender, EventArgs e)
        {
            dtLoadUpdate.Stop();
            _showBarcode();
            txtbarcode.Text = "";
            txtbarcode.Focus();
            //set the all setting from update model.
            SetGridChack(dgPackageInfo);

        }

        void dtLoadnormal_Tick(object sender, EventArgs e)
        {
            dtLoadnormal.Stop();
            _showBarcode();
            txtbarcode.Text = "";
            txtbarcode.Focus();
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
            if (lstponumber.SelectedItem != null)
            {
                if (Views.clGlobal.IsAlreadySaved)
                {
                    //Get the all information from datebase to the Update mode from RMA Number.
                    _mUpdate = new mupdatedForPonumber(_mponumber.lsRMAInformationforponumner[0].PONumber); //mReturn.lsRMAInformation[0].RMANumber);

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
                    List<RMAInfo> lsCustomeronfo = _mNewRMA.GetCustomer(txtPoNumber.Text);
                    lstponumber.Visibility = Visibility.Hidden;

                    if (lsCustomeronfo.Count > 0)
                    {
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

                        dgPackageInfo.ItemsSource = _mUpdate._lsReturnDetails1;

                        _lsRMAInfo = lsCustomeronfo;
                        _mponumber.lsRMAInformationforponumner = lsCustomeronfo;

                        _mponumber.mPOnumberRMA1(txtPoNumber.Text.ToUpper());

                        // _mponumber = new mPOnumberRMA(txtPoNumber.Text.ToUpper());
                        txtbarcode.Focus();
                        if (lsCustomeronfo[0].VendorNumber.ToString() == "GENC0001" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0404" || lsCustomeronfo[0].VendorNumber.ToString() == "INTC0017" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0551" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0795")
                        {
                            Views.clGlobal.ScenarioType = "HomeDepot";
                            txtbarcode.Focus();
                            ErrorMsg("Please Check this RMA is Wrong or Not By Scanning the Barcode.", Color.FromRgb(185, 84, 0));


                            DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                            DateTime CurrentDate = DateTime.UtcNow;
                            TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                            int Days = Diff.Days;
                            Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                            if (Days <= 60)
                            {
                                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "1";
                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                            }
                            else
                            {
                                ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "0";
                                Views.clGlobal.SKU_Staus = "Deny";
                                Views.clGlobal.TotalPoints = points;
                                Views.clGlobal.WrongRMAFlag = "N/A";

                                cmbRMAStatus.SelectedIndex = 2;
                                MessageBox.Show("This Return is NOT in Warranty.");

                                // btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });
                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                            }

                        }

                        else if (lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0143" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0432")
                        {
                            Views.clGlobal.ScenarioType = "Lowes";
                            ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                            txtbarcode.Focus();
                            //dgPackageInfo.IsEnabled = false;
                        }
                        else
                        {
                            Views.clGlobal.ScenarioType = "Others";
                            txtbarcode.Focus();

                            Views.clGlobal.WrongRMAFlag = "0";
                            DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                            DateTime CurrentDate = DateTime.UtcNow;
                            TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                            int Days = Diff.Days;
                            Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                            if (Days <= 60)
                            {
                                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "1";
                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                            }
                            else
                            {
                                ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "0";
                                Views.clGlobal.SKU_Staus = "Deny";
                                Views.clGlobal.TotalPoints = points;
                                Views.clGlobal.WrongRMAFlag = "N/A";
                                cmbRMAStatus.SelectedIndex = 2;

                                MessageBox.Show("This Return is NOT in Warranty.");

                                //  btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                            }
                            _mponumber.lsRMAInformationforponumner = lsCustomeronfo;
                        }
                        //Initialize the Dispacher that shows all values from the Update model.
                        dtLoadUpdate = new DispatcherTimer();
                        dtLoadUpdate.Interval = new TimeSpan(0, 0, 0, 0, 100);
                        dtLoadUpdate.Tick += dtLoadUpdate_Tick;
                        //start the dispacher.
                        dtLoadUpdate.Start();
                    }



                }
                else
                {
                    List<RMAInfo> lsCustomeronfo = _mNewRMA.GetCustomer(txtPoNumber.Text);
                    lstponumber.Visibility = Visibility.Hidden;

                    if (lsCustomeronfo.Count > 0)
                    {
                        txtVendorName.Text = lsCustomeronfo[0].VendorName;
                        txtVendorNumber.Text = lsCustomeronfo[0].VendorNumber;
                        txtAddress.Text = lsCustomeronfo[0].Address1;
                        txtCountry.Text = lsCustomeronfo[0].Country;
                        txtCustCity.Text = lsCustomeronfo[0].City;
                        txtState.Text = lsCustomeronfo[0].State;
                        txtZipCode.Text = lsCustomeronfo[0].ZipCode;
                        txtName.Text = lsCustomeronfo[0].CustomerName1;
                        dgPackageInfo.ItemsSource = lsCustomeronfo;
                        _lsRMAInfo = lsCustomeronfo;
                        _mponumber.lsRMAInformationforponumner = lsCustomeronfo;

                        _mponumber.mPOnumberRMA1(txtPoNumber.Text.ToUpper());

                        // _mponumber = new mPOnumberRMA(txtPoNumber.Text.ToUpper());
                        txtbarcode.Focus();
                        if (lsCustomeronfo[0].VendorNumber.ToString() == "GENC0001" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0404" || lsCustomeronfo[0].VendorNumber.ToString() == "INTC0017" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0551" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0795")
                        {
                            Views.clGlobal.ScenarioType = "HomeDepot";
                            txtbarcode.Focus();
                            ErrorMsg("Please Check this RMA is Wrong or Not By Scanning the Barcode.", Color.FromRgb(185, 84, 0));


                            DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                            DateTime CurrentDate = DateTime.UtcNow;
                            TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                            int Days = Diff.Days;
                            Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                            if (Days <= 60)
                            {
                                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "1";
                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                            }
                            else
                            {
                                ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "0";
                                Views.clGlobal.SKU_Staus = "Deny";
                                Views.clGlobal.TotalPoints = points;
                                Views.clGlobal.WrongRMAFlag = "N/A";

                                cmbRMAStatus.SelectedIndex = 2;

                                MessageBox.Show("This Return is NOT in Warranty.");

                                // btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });
                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                            }

                        }

                        else if (lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0143" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0432")
                        {
                            Views.clGlobal.ScenarioType = "Lowes";
                            ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                            txtbarcode.Focus();
                            //dgPackageInfo.IsEnabled = false;
                        }
                        else
                        {
                            Views.clGlobal.ScenarioType = "Others";
                            txtbarcode.Focus();

                            Views.clGlobal.WrongRMAFlag = "0";
                            DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                            DateTime CurrentDate = DateTime.UtcNow;
                            TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                            int Days = Diff.Days;
                            Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                            if (Days <= 60)
                            {
                                ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "1";
                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                            }
                            else
                            {
                                ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                                Views.clGlobal.Warranty = "0";
                                Views.clGlobal.SKU_Staus = "Deny";
                                Views.clGlobal.TotalPoints = points;
                                Views.clGlobal.WrongRMAFlag = "N/A";

                                cmbRMAStatus.SelectedIndex = 2;

                                MessageBox.Show("This Return is NOT in Warranty.");

                                //  btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                            }
                            _mponumber.lsRMAInformationforponumner = lsCustomeronfo;
                        }
                    }

                    //Initialize the Dispacher that shows all values from the Update model.
                    dtLoadnormal = new DispatcherTimer();
                    dtLoadnormal.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    dtLoadnormal.Tick += dtLoadnormal_Tick;
                    //start the dispacher.
                    dtLoadnormal.Start();
                }


            }
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

        private void ErrorMsg(string Msg, Color BgColor)
        {
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            bdrMsg.Visibility = System.Windows.Visibility.Visible;
            bdrMsg.Background = new SolidColorBrush(BgColor);
            txtError.Text = Msg;
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {

            if (!(txtError.Text == "This Return is NOT in Warranty."))
            {
                foreach (DataGridRow item in GetDataGridRows(dgPackageInfo))
                {
                    ContentPresenter butoninfo = dgPackageInfo.Columns[0].GetCellContent(item) as ContentPresenter;
                    DataTemplate DtQty = butoninfo.ContentTemplate;
                    Button txtRetutn = (Button)DtQty.FindName("btnGreen", butoninfo);
                    txtRetutn.Visibility = System.Windows.Visibility.Hidden;
                    Button txtRetutn2 = (Button)DtQty.FindName("btnRed", butoninfo);
                    txtRetutn2.Visibility = System.Windows.Visibility.Visible;
                    txtbarcode.Text = "";
                    txtbarcode.Focus();
                }
                if (Views.clGlobal.ScenarioType == "Lowes")
                {
                    CanvasConditions.IsEnabled = false;
                    txtbarcode.Focus();
                    btnAdd.IsEnabled = true;
                    Button btnRed = (Button)e.Source;
                    Canvas SpButtons = (Canvas)btnRed.Parent;
                    Button btnGreen = SpButtons.FindName("btnGreen") as Button;
                    DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
                    if (row.Background == Brushes.Gray)
                    {

                        btnGreen.Visibility = System.Windows.Visibility.Visible;
                        btnRed.Visibility = System.Windows.Visibility.Hidden;
                    }
                    GreenRowsNumber1.Add(row.GetIndex());
                    bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                    txtbarcode.Text = "";
                    txtbarcode.Focus();

                    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");

                }
                if (Views.clGlobal.ScenarioType == "HomeDepot")
                {

                    // CanvasConditions.IsEnabled = true;
                    txtbarcode.Focus();
                    btnAdd.IsEnabled = true;
                    CanvasConditions.IsEnabled = false;
                    Button btnRed = (Button)e.Source;
                    Canvas SpButtons = (Canvas)btnRed.Parent;
                    Button btnGreen = SpButtons.FindName("btnGreen") as Button;
                    DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();

                    ContentPresenter Cntskustatus = dgPackageInfo.Columns[6].GetCellContent(row) as ContentPresenter;
                    DataTemplate Dtskustatus = Cntskustatus.ContentTemplate;
                    TextBlock txtskustatus = (TextBlock)Dtskustatus.FindName("tbskustatus", Cntskustatus);

                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;


                    ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                    TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);



                    if (row.Background == Brushes.Gray)
                    {
                        CanvasConditions.IsEnabled = true;

                        btnInstalledNo.IsChecked = true;
                        btnBoxNotNew.IsChecked = true;
                        btnStatusNo.IsChecked = true;


                        btnGreen.Visibility = System.Windows.Visibility.Visible;
                        btnRed.Visibility = System.Windows.Visibility.Hidden;
                    }
                    if (row.Background == Brushes.Gray && txtskustatus.Text != "")
                    {
                        CanvasConditions.IsEnabled = false;
                        string msg = "";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (SkuNumber.Text == dt.Rows[i][0].ToString() && txtRetutn2.Text == dt.Rows[i][4].ToString())
                            {
                                msg = dt.Rows[i][1].ToString() + " : " + dt.Rows[i][2].ToString() + "\n" + msg;
                            }
                        }

                        MessageBox.Show(msg);
                    }


                    GreenRowsNumber1.Add(row.GetIndex());
                    bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                    txtbarcode.Text = "";
                    txtbarcode.Focus();

                    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");

                }
                if (Views.clGlobal.ScenarioType == "Others")
                {

                    //CanvasConditions.IsEnabled = true;
                    txtbarcode.Focus();
                    btnAdd.IsEnabled = true;
                    CanvasConditions.IsEnabled = false;
                    Button btnRed = (Button)e.Source;
                    Canvas SpButtons = (Canvas)btnRed.Parent;
                    Button btnGreen = SpButtons.FindName("btnGreen") as Button;
                    DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();

                    ContentPresenter Cntskustatus = dgPackageInfo.Columns[6].GetCellContent(row) as ContentPresenter;
                    DataTemplate Dtskustatus = Cntskustatus.ContentTemplate;
                    TextBlock txtskustatus = (TextBlock)Dtskustatus.FindName("tbskustatus", Cntskustatus);

                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                    
                    ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                    TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);



                    if (row.Background == Brushes.Gray)
                    {
                        CanvasConditions.IsEnabled = true;

                        btnInstalledNo.IsChecked = true;
                        btnBoxNotNew.IsChecked = true;
                        btnStatusNo.IsChecked = true;

                        btnGreen.Visibility = System.Windows.Visibility.Visible;
                        btnRed.Visibility = System.Windows.Visibility.Hidden;
                    }

                    if (row.Background == Brushes.Gray && txtskustatus.Text != "")
                    {
                        CanvasConditions.IsEnabled = false;
                        string msg = "";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (SkuNumber.Text == dt.Rows[i][0].ToString() && txtRetutn2.Text == dt.Rows[i][4].ToString())
                            {
                                msg = dt.Rows[i][1].ToString() + " : " + dt.Rows[i][2].ToString() + "\n" + msg;
                            }
                        }

                        MessageBox.Show(msg);
                    }

                    GreenRowsNumber1.Add(row.GetIndex());
                    bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                    txtbarcode.Text = "";
                    txtbarcode.Focus();

                    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");

                }
            }

            //  mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
        }

        private void btnPluse_Click(object sender, RoutedEventArgs e)
        {
            StackPanel Sp = (StackPanel)(sender as Control).Parent;
            StackPanel Sp2 = (StackPanel)Sp.Parent;
            DataGridRow row = (DataGridRow)Sp2.FindParent<DataGridRow>();
            if (GreenRowsNumber1.Contains(row.GetIndex()))
            {
                try
                {
                    foreach (TextBlock t in Sp2.Children)
                    {
                        if (Convert.ToInt32(t.Text) >= 0)
                        {
                            t.Text = (Convert.ToInt32(t.Text) + 1).ToString();
                        }
                        break;
                    }
                }
                catch (Exception)
                { }
            }
            else
            {
                mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.SelectItem__00.ToString(), DateTime.UtcNow.ToString());
                ErrorMsg("Please select the item.", Color.FromRgb(185, 84, 0));
            }
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //create object stack panel where Button Belongs to
                StackPanel sp = (StackPanel)(sender as Control).Parent;
                StackPanel sp2 = (StackPanel)sp.Parent;
                DataGridRow row = (DataGridRow)sp2.FindParent<DataGridRow>();
                if (GreenRowsNumber1.Contains(row.GetIndex()))
                {
                    try
                    {
                        foreach (TextBlock t in sp2.Children)
                        {
                            if (Convert.ToInt32(t.Text) > 0)
                            {
                                t.Text = (Convert.ToInt32(t.Text) - 1).ToString();
                            }
                            break;
                        }
                    }
                    catch (Exception)
                    { }
                }
                else
                {
                    mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.SelectItem__00.ToString(), DateTime.UtcNow.ToString());
                    ErrorMsg("Please select the item.", Color.FromRgb(185, 84, 0));
                }
            }
            catch (Exception)
            { }
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            if (Views.clGlobal.ScenarioType == "Lowes")
            {
                CanvasConditions.IsEnabled = false;
                txtbarcode.Focus();
            }
            if (Views.clGlobal.ScenarioType == "HomeDepot" || Views.clGlobal.ScenarioType == "Others")
            {
                CanvasConditions.IsEnabled = true;
                txtbarcode.Focus();
            }
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

            // mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_UnChecked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
        }

        private void dgPackageInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = dgPackageInfo.SelectedIndex;
            if (selectedIndex != -1)
            {
                foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                {
                    if (row.IsSelected)
                    {
                        ContentPresenter cp = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                        DataTemplate Dt = cp.ContentTemplate;
                        StackPanel spProductIMages = (StackPanel)Dt.FindName("spProductImages", cp);
                        spRowImages = spProductIMages;
                        ScrollViewer SvImages = (ScrollViewer)Dt.FindName("svScrollImages", cp);
                        SvImagesScroll = SvImages;
                    }
                }
            }
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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Views.clGlobal.BarcodeValueFound = Views.clGlobal.FBCodeForSKU.BarcodeValueSKU;
            //Views.clGlobal.FBCodeForSKU.PropertyChanged += FBCode_PropertyChanged1;

           

            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Reason", typeof(string));
            dt.Columns.Add("Reason_Value", typeof(string));
            dt.Columns.Add("Points", typeof(int));
            dt.Columns.Add("ItemQuantity", typeof(string));

            txtbarcode.Focus();
            List<Reason> lsReturn = _mNewRMA.GetReasons();

            //add reason select to the Combobox other reason.
            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";
            lsReturn.Insert(0, re);
            cmbSkuReasons.ItemsSource = lsReturn;


            cmbRMAStatus.SelectedIndex = 0;
            cmbRMADecision.SelectedIndex = 0;

            Barcode.Camera.DeleteLocalImages();
            if (Views.clGlobal.IsAlreadySaved)
            {
                //txtPoNumber.Text = Views.clGlobal.Ponumber;
                //InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
                //txtPoNumber_KeyDown_1(txtPoNumber, new KeyEventArgs { });

                List<RMAInfo> lsCustomeronfo = _mNewRMA.GetCustomer(Views.clGlobal.Ponumber);
                lstponumber.Visibility = Visibility.Hidden;
                _mponumber.lsRMAInformationforponumner = lsCustomeronfo;

                _mUpdate = new mupdatedForPonumber(_mponumber.lsRMAInformationforponumner[0].PONumber); //mReturn.lsRMAInformation[0].RMANumber);

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


                if (lsCustomeronfo.Count > 0)
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

                    dgPackageInfo.ItemsSource = _mUpdate._lsReturnDetails1;
                    _lsRMAInfo = lsCustomeronfo;
                    _mponumber.lsRMAInformationforponumner = lsCustomeronfo;

                    _mponumber.mPOnumberRMA1(txtPoNumber.Text.ToUpper());
                    txtbarcode.Focus();
                    // _mponumber = new mPOnumberRMA(txtPoNumber.Text.ToUpper());
                    txtbarcode.Focus();
                    if (lsCustomeronfo[0].VendorNumber.ToString() == "GENC0001" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0404" || lsCustomeronfo[0].VendorNumber.ToString() == "INTC0017" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0551" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0795")
                    {
                        Views.clGlobal.ScenarioType = "HomeDepot";
                        txtbarcode.Focus();
                        ErrorMsg("Please Check this RMA is Wrong or Not Byscanning the Barcode.", Color.FromRgb(185, 84, 0));


                        DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                        DateTime CurrentDate = DateTime.UtcNow;
                        TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                        int Days = Diff.Days;
                        Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                        if (Days <= 60)
                        {
                            ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                            Views.clGlobal.Warranty = "1";
                            txtbarcode.Text = "";
                            txtbarcode.Focus();

                        }
                        else
                        {
                            ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                            Views.clGlobal.Warranty = "0";
                            Views.clGlobal.SKU_Staus = "Deny";
                            Views.clGlobal.TotalPoints = points;
                            Views.clGlobal.WrongRMAFlag = "N/A";

                            cmbRMAStatus.SelectedIndex = 2;

                            MessageBox.Show("This Return is NOT in Warranty.");

                            // btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });
                            txtbarcode.Text = "";
                            txtbarcode.Focus();
                        }

                    }

                    else if (lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0143" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0432")
                    {
                        Views.clGlobal.ScenarioType = "Lowes";
                        ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                        txtbarcode.Focus();
                        //dgPackageInfo.IsEnabled = false;
                    }
                    else
                    {
                        Views.clGlobal.ScenarioType = "Others";
                        txtbarcode.Focus();

                        Views.clGlobal.WrongRMAFlag = "0";
                        DateTime DeliveryDate = _lsRMAInfo[0].DeliveryDate;
                        DateTime CurrentDate = DateTime.UtcNow;
                        TimeSpan Diff = CurrentDate.Subtract(DeliveryDate);
                        int Days = Diff.Days;
                        Views.clGlobal.ShipDate_ScanDate_Diff = Days;
                        if (Days <= 60)
                        {
                            ErrorMsg("Select Item and Go ahead", Color.FromRgb(185, 84, 0));
                            Views.clGlobal.Warranty = "1";
                            txtbarcode.Text = "";
                            txtbarcode.Focus();

                        }
                        else
                        {
                            ErrorMsg("This Return is NOT in Warranty.", Color.FromRgb(185, 84, 0));
                            Views.clGlobal.Warranty = "0";
                            Views.clGlobal.SKU_Staus = "Deny";
                            Views.clGlobal.TotalPoints = points;
                            Views.clGlobal.WrongRMAFlag = "N/A";

                            cmbRMAStatus.SelectedIndex = 2;
                            MessageBox.Show("This Return is NOT in Warranty.");

                            //  btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                            txtbarcode.Text = "";
                            txtbarcode.Focus();
                        }
                        _mponumber.lsRMAInformationforponumner = lsCustomeronfo;
                    }
                    //Initialize the Dispacher that shows all values from the Update model.
                    dtLoadUpdate = new DispatcherTimer();
                    dtLoadUpdate.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    dtLoadUpdate.Tick += dtLoadUpdate_Tick;
                    //start the dispacher.
                    dtLoadUpdate.Start();
                }

            }


            // cmbOtherReason.ItemsSource = lsReturn;
        }
       
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            #region HomwDepot
            if (Views.clGlobal.ScenarioType == "HomeDepot")
            {
                //  DataTable dt1 = new DataTable();


                string SelectedskuName = "";
                string ItemQuantity = "";
                string SKUSequence = "";
                //string skusequence="";
                foreach (DataGridRow item in GetDataGridRows(dgPackageInfo))
                {
                    ContentPresenter butoninfo = dgPackageInfo.Columns[0].GetCellContent(item) as ContentPresenter;
                    DataTemplate DtQty = butoninfo.ContentTemplate;
                    Button txtRetutn = (Button)DtQty.FindName("btnGreen", butoninfo);
                    if (txtRetutn.Visibility == Visibility.Visible)
                    {
                        item.IsEnabled = false;
                        // item.Background = Brushes.Red;
                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(item) as TextBlock;
                        SelectedskuName = SkuNumber.Text;

                        ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(item) as ContentPresenter;
                        DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                        TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);
                        ItemQuantity = txtRetutn2.Text;


                        ContentPresenter CntQuantity21 = dgPackageInfo.Columns[2].GetCellContent(item) as ContentPresenter;
                        DataTemplate DtQty21 = CntQuantity21.ContentTemplate;
                        TextBlock txtRetutn21 = (TextBlock)DtQty21.FindName("tbQty", CntQuantity21);
                        SKUSequence = txtRetutn21.Text;

                    }
                }
                if (Views.clGlobal.IsAlreadySaved)
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow d = dt.Rows[i];
                        if (d["SKU"].ToString() == SelectedskuName.ToString() && d["ItemQuantity"].ToString() == ItemQuantity)
                            d.Delete();
                    }
                }

                #region DtOperaion
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
                #endregion



                StatusAndPoints _lsstatusandpoints = new StatusAndPoints();
                _lsstatusandpoints.SKUName = SelectedskuName;
                _lsstatusandpoints.Status = Views.clGlobal.SKU_Staus;
                _lsstatusandpoints.Points = Convert.ToInt16(lblpoints.Content);//Views.clGlobal.TotalPoints;
                _lsstatusandpoints.NewItemQuantity = Convert.ToInt16(ItemQuantity);
                _lsstatusandpoints.skusequence = Convert.ToInt16(SKUSequence);

                for (int i = 0; i < lsskuIsScanned.Count; i++)
                {
                    if (lsskuIsScanned[i].SKUName == SelectedskuName)
                    {
                        _lsstatusandpoints.IsScanned = lsskuIsScanned[i].IsScanned;
                        break;
                    }
                }

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

                //if (cmbSkuReasons.SelectedIndex == 0 && txtskuReasons.Text == "")
                //{
                //    MessageBox.Show("Please Select or Enter Reason");
                //    btnAdd.IsEnabled = true;
                //    CanvasConditions.IsEnabled = true;
                //}
                //else
                //{
                SkuReasonIDSequence lsskusequenceReasons = new SkuReasonIDSequence();
                lsskusequenceReasons.ReasonID = SkuReasonID;
                lsskusequenceReasons.SKU_sequence = Convert.ToInt16(ItemQuantity);
                lsskusequenceReasons.SKUName = SelectedskuName;
                _lsReasonSKU.Add(lsskusequenceReasons);

                fillComboBox();

                cmbSkuReasons.SelectedIndex = 0;
                txtskuReasons.Text = "";
                //}


                #endregion

            }
            #endregion

            #region Others
            if (Views.clGlobal.ScenarioType == "Others")
            {
                string SelectedskuName = "";
                string ItemQuantity = "";
                string SKUSequence = "";
                foreach (DataGridRow item in GetDataGridRows(dgPackageInfo))
                {
                    ContentPresenter butoninfo = dgPackageInfo.Columns[0].GetCellContent(item) as ContentPresenter;
                    DataTemplate DtQty = butoninfo.ContentTemplate;
                    Button txtRetutn = (Button)DtQty.FindName("btnGreen", butoninfo);
                    if (txtRetutn.Visibility == Visibility.Visible)
                    {
                        item.IsEnabled = false;
                        // item.Background = Brushes.Red;
                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(item) as TextBlock;
                        SelectedskuName = SkuNumber.Text;

                        ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(item) as ContentPresenter;
                        DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                        TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);
                        ItemQuantity = txtRetutn2.Text;


                        ContentPresenter CntQuantity21 = dgPackageInfo.Columns[2].GetCellContent(item) as ContentPresenter;
                        DataTemplate DtQty21 = CntQuantity21.ContentTemplate;
                        TextBlock txtRetutn21 = (TextBlock)DtQty21.FindName("tbQty", CntQuantity21);
                        SKUSequence = txtRetutn21.Text;


                    }
                }

                if (Views.clGlobal.IsAlreadySaved)
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow d = dt.Rows[i];
                        if (d["SKU"].ToString() == SelectedskuName.ToString() && d["ItemQuantity"].ToString() == ItemQuantity)
                            d.Delete();
                    }
                }

                #region Dtoperation
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
                #endregion



                StatusAndPoints _lsstatusandpoints = new StatusAndPoints();
                _lsstatusandpoints.SKUName = SelectedskuName;
                _lsstatusandpoints.Status = Views.clGlobal.SKU_Staus;
                _lsstatusandpoints.Points = Convert.ToInt16(lblpoints.Content);
                _lsstatusandpoints.NewItemQuantity = Convert.ToInt16(ItemQuantity);
                _lsstatusandpoints.skusequence = Convert.ToInt16(SKUSequence);

                for (int i = 0; i < lsskuIsScanned.Count; i++)
                {
                    if (lsskuIsScanned[i].SKUName == SelectedskuName)
                    {
                        _lsstatusandpoints.IsScanned = lsskuIsScanned[i].IsScanned;
                        break;
                    }

                }
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

                //Views.clGlobal.TotalPoints;
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

                //if (cmbSkuReasons.SelectedIndex == 0 && txtskuReasons.Text == "")
                //{
                //    MessageBox.Show("Please Select or Enter Reason");
                //    btnAdd.IsEnabled = true;
                //   // CanvasConditions.IsEnabled = true;
                //}
                //else
                //{
                SkuReasonIDSequence lsskusequenceReasons = new SkuReasonIDSequence();
                lsskusequenceReasons.ReasonID = SkuReasonID;
                lsskusequenceReasons.SKU_sequence = Convert.ToInt16(ItemQuantity);
                lsskusequenceReasons.SKUName = SelectedskuName;
                _lsReasonSKU.Add(lsskusequenceReasons);

                fillComboBox();

                cmbSkuReasons.SelectedIndex = 0;
                txtskuReasons.Text = "";
                //}

                #endregion

            }
            #endregion



        }
        Boolean itemnew = true;
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
        }

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

                btnManufacturerNo.IsChecked = false;
                btnManufacturerYes.IsChecked = false;
                btntransiteNo.IsChecked = false;
                btntransiteYes.IsChecked = false;
                btnStatusNo.IsChecked = false;
                btnStatusYes.IsChecked = false;


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

                btnManufacturerNo.IsChecked = false;
                btnManufacturerYes.IsChecked = false;
                btntransiteNo.IsChecked = false;
                btntransiteYes.IsChecked = false;
                btnStatusNo.IsChecked = false;
                btnStatusYes.IsChecked = false;

            }
            ISinstalled = true;



            ErrorMsg("This Item is Rejected.", Color.FromRgb(185, 84, 0));
            //btnStatusNo.IsEnabled = false;
            // btnStatusYes.IsEnabled = false;
            btnAdd.IsEnabled = true;
        }
        Boolean ISinstalled = true;
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

            btnStatusNo.IsChecked = true;
        }

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
        }
        Boolean IsStatus = true;
        Boolean IsManufacture = true;
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
            // Views.clGlobal.SKU_Staus = "";

            if (btnManufacturerNo.IsChecked == true && btntransiteNo.IsChecked == true)
            {
                Views.clGlobal.SKU_Staus = "Deny";
            }
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
            // Views.clGlobal.SKU_Staus = "";

            if (btnManufacturerNo.IsChecked == true && btntransiteNo.IsChecked == true)
            {
                Views.clGlobal.SKU_Staus = "Deny";
            }

        }
        int count = 0;
        int points = 0;
        int max;
        private void txtbarcode_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtbarcode.Text.Trim() != "")
            {
                Boolean flag = false;
                //Boolean RMACheck = false;

                #region Lowes
                if (Views.clGlobal.ScenarioType == "Lowes")
                {
                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {
                        SkuAndIsScanned _lsskuandscanned = new SkuAndIsScanned();
                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        string Str = txtbarcode.Text.TrimStart('0').ToString();
                        string sku = _mponumber.GetENACodeByItem(SkuNumber.Text);

                        ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                        DataTemplate DtQty = CntQuantity.ContentTemplate;
                        TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                        if (sku == Str)
                        {
                            _lsskuandscanned.SKUName = SkuNumber.Text;
                            _lsskuandscanned.IsScanned = 1;
                            lsskuIsScanned.Add(_lsskuandscanned);
                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            //row.Background = Brushes.Gray;



                            if (sku == Str && txtRetutn.Text == "0")
                            {
                                row.Background = Brushes.Gray;
                                txtRetutn.Text = "1";
                            }
                            else if (sku == Str && txtRetutn.Text == "1")
                            {
                                List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                                foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                                {
                                    //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                                    RMAInfo ls = new RMAInfo();
                                    TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                                    ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                                    TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                                    TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row1) as TextBlock;

                                    TextBlock SalePrices = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                                    ls.SKUNumber = SkuNumber1.Text;
                                    ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                                    ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                                    ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                                    ls.ProductID = ProductID.Text;
                                    if (sku == _mponumber.GetENACodeByItem(SkuNumber1.Text))
                                    {
                                        max = Convert.ToInt16(txtRetutn2.Text);
                                    }
                                    _lsRMAInfo1.Add(ls);
                                }

                                RMAInfo ls1 = new RMAInfo();
                                SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                                ls1.SKUNumber = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                                ls1.ProductID = _mponumber.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                                ls1.SalesPrice = 0;
                                _lsIsmanually1.IsScanned = 1;
                                _lsIsmanually1.SKUName = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                                lsIsManually.Add(_lsIsmanually1);


                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                                ls1.SKU_Qty_Seq = 1;
                                ls1.SKU_Sequence = max + 1;
                                max = 0;

                                _lsRMAInfo1.Add(ls1);

                                this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                                dtLoadUpdate1 = new DispatcherTimer();
                                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                                //start the dispacher.
                                dtLoadUpdate1.Start();
                            }




                            txtbarcode.Text = "";
                            txtbarcode.Focus();
                            flag = true;
                            count++;
                            //break;
                        }
                    }

                    if (!flag)
                    {
                        List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                        foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                        {
                            //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                            RMAInfo ls = new RMAInfo();
                            TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                            ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                            TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                            ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                            TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                            TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row1) as TextBlock;

                            TextBlock SalePrices = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                            ls.SKUNumber = SkuNumber1.Text;
                            ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                            ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                            ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                            ls.ProductID = ProductID.Text;

                            _lsRMAInfo1.Add(ls);
                        }

                        RMAInfo ls1 = new RMAInfo();
                        SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                        ls1.SKUNumber = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                        ls1.ProductID = _mponumber.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                        ls1.SalesPrice = 0;
                        _lsIsmanually1.IsScanned = 1;
                        _lsIsmanually1.SKUName = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                        lsIsManually.Add(_lsIsmanually1);


                        txtbarcode.Text = "";
                        txtbarcode.Focus();

                        ls1.SKU_Qty_Seq = 1;
                        ls1.SKU_Sequence = 1;
                        max = 0;

                        _lsRMAInfo1.Add(ls1);

                        this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                        dtLoadUpdate1 = new DispatcherTimer();
                        dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                        dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                        //start the dispacher.
                        dtLoadUpdate1.Start();



                        txtbarcode.Text = "";
                        txtbarcode.Focus();
                    }

                    if (CountSelected() == dgPackageInfo.Items.Count)
                    {
                        Views.clGlobal.WrongRMAFlag = "0";
                        ErrorMsg("This is Correct RMA", Color.FromRgb(185, 84, 0));
                        txtbarcode.Text = "";

                        cmbRMAStatus.SelectedIndex = 1;

                        //  RMACheck = true;
                        count = 0;
                        txtbarcode.Focus();
                    }

                }
                #endregion

                #region HomeDepot
                if (Views.clGlobal.ScenarioType == "HomeDepot")
                {
                    Boolean itemcheck = true;
                    for (int i = 0; i < _mponumber.lsRMAInformationforponumner.Count; i++)
                    {
                        if (_mponumber.lsRMAInformationforponumner[i].SKUNumber == _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString()))
                        {
                            itemcheck = false;// MessageBox.Show("This Scanned item is not part of PO.");

                        }
                    }

                    if (itemcheck)
                    {
                        MessageBox.Show("This Scanned item is not part of PO.");
                        cmbRMAStatus.SelectedIndex = 2;
                    }

                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {
                        SkuAndIsScanned _lsskuandscanned = new SkuAndIsScanned();
                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                        ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                        DataTemplate DtQty = CntQuantity.ContentTemplate;
                        TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                        TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row) as TextBlock;

                        TextBlock SalesPrices = dgPackageInfo.Columns[8].GetCellContent(row) as TextBlock;

                        string Str = txtbarcode.Text.TrimStart('0').ToString();
                        string sku = _mponumber.GetENACodeByItem(SkuNumber.Text);
                        if (sku == Str)
                        {
                            _lsskuandscanned.SKUName = SkuNumber.Text;
                            _lsskuandscanned.IsScanned = 1;

                            lsskuIsScanned.Add(_lsskuandscanned);
                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            // row.Background = Brushes.Gray;

                            if (sku == Str && txtRetutn.Text == "0")
                            {
                                row.Background = Brushes.Gray;
                                txtRetutn.Text = "1";
                            }
                            else if (sku == Str && txtRetutn.Text == "1")
                            {
                                List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                                foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                                {
                                    //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                                    RMAInfo ls = new RMAInfo();
                                    TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                                    TextBlock ProductID1 = dgPackageInfo.Columns[7].GetCellContent(row1) as TextBlock;

                                    TextBlock SalePrices = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                                    ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                                    TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                                    ls.SKUNumber = SkuNumber1.Text;
                                    ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                                    ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                                    ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                                    ls.ProductID = ProductID1.Text;
                                    //int temp;
                                    if (sku == _mponumber.GetENACodeByItem(SkuNumber1.Text))
                                    {
                                        if (max < Convert.ToInt16(txtRetutn2.Text))
                                        {
                                            max = Convert.ToInt16(txtRetutn2.Text);
                                        }
                                    }

                                    _lsRMAInfo1.Add(ls);
                                }

                                RMAInfo ls1 = new RMAInfo();
                                SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                                ls1.SKUNumber = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                                ls1.ProductID = _mponumber.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];
                                ls1.SalesPrice = 0;
                                _lsIsmanually1.IsScanned = 1;
                                _lsIsmanually1.SKUName = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                                lsIsManually.Add(_lsIsmanually1);


                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                                ls1.SKU_Qty_Seq = 1;
                                ls1.SKU_Sequence = max + 1;
                                max = 0;

                                _lsRMAInfo1.Add(ls1);

                                this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                                dtLoadUpdate1 = new DispatcherTimer();
                                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                                //start the dispacher.
                                dtLoadUpdate1.Start();


                            }

                            Views.clGlobal.IsScanned = 1;
                            txtbarcode.Text = "";
                            txtbarcode.Focus();
                            flag = true;
                            count++;
                            //break;
                        }
                    }

                    if (!flag)
                    {
                        Views.clGlobal.WrongRMAFlag = "1";
                        // ErrorMsg("This Scanned item is not part of PO.", Color.FromRgb(185, 84, 0));
                        Views.clGlobal.SKU_Staus = "Reject";
                        Views.clGlobal.TotalPoints = points;// lblpoints.Content;
                        Views.clGlobal.Warranty = "N/A";
                        Views.clGlobal.IsManually = 1;


                        // MessageBox.Show("This Scanned item is not part of PO.");

                        List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                        foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                        {
                            //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                            RMAInfo ls = new RMAInfo();
                            TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                            ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                            TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                            ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                            TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);


                            TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row1) as TextBlock;

                            TextBlock SalePrices = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                            ls.SKUNumber = SkuNumber1.Text;
                            ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                            ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                            ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                            ls.ProductID = ProductID.Text;

                            _lsRMAInfo1.Add(ls);
                        }

                        RMAInfo ls1 = new RMAInfo();
                        SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                        ls1.SKUNumber = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                        ls1.ProductID = _mponumber.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                        ls1.SalesPrice = 0;
                        _lsIsmanually1.IsScanned = 1;
                        _lsIsmanually1.SKUName = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                        lsIsManually.Add(_lsIsmanually1);


                        txtbarcode.Text = "";
                        txtbarcode.Focus();

                        ls1.SKU_Qty_Seq = 1;
                        ls1.SKU_Sequence = 1;

                        Views.clGlobal.WrongRMAFlag = "1";

                        max = 0;

                        _lsRMAInfo1.Add(ls1);

                        this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                        dtLoadUpdate1 = new DispatcherTimer();
                        dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                        dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                        //start the dispacher.
                        dtLoadUpdate1.Start();
                    }

                    txtbarcode.Text = "";
                    txtbarcode.Focus();


                    if (CountSelected() == dgPackageInfo.Items.Count)
                    {
                        Views.clGlobal.WrongRMAFlag = "0";
                        ErrorMsg("This is Correct RMA", Color.FromRgb(185, 84, 0));
                        txtbarcode.Text = "";

                        cmbRMAStatus.SelectedIndex = 1;

                        //  RMACheck = true;
                        count = 0;
                        txtbarcode.Focus();
                    }
                    else
                    {
                        Views.clGlobal.WrongRMAFlag = "1";
                        Views.clGlobal.Warranty = "N/A";
                    }
                }
                #endregion

                #region Others
                if (Views.clGlobal.ScenarioType == "Others")
                {
                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {
                        SkuAndIsScanned _lsskuandscanned = new SkuAndIsScanned();

                        ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                        DataTemplate DtQty = CntQuantity.ContentTemplate;
                        TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        string Str = txtbarcode.Text.TrimStart('0').ToString();
                        string sku = _mponumber.GetENACodeByItem(SkuNumber.Text);
                        if (sku == Str)
                        {
                            _lsskuandscanned.SKUName = SkuNumber.Text;
                            _lsskuandscanned.IsScanned = 1;

                            lsskuIsScanned.Add(_lsskuandscanned);

                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            //row.Background = Brushes.Gray;
                            if (sku == Str && txtRetutn.Text == "0")
                            {
                                row.Background = Brushes.Gray;
                                txtRetutn.Text = "1";
                            }
                            else if (sku == Str && txtRetutn.Text == "1")
                            {
                                List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                                foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                                {
                                    //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                                    RMAInfo ls = new RMAInfo();
                                    TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                                    ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                                    TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                                    TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row1) as TextBlock;

                                    TextBlock SalePrices = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                                    ls.SKUNumber = SkuNumber1.Text;
                                    ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                                    ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                                    if (sku == _mponumber.GetENACodeByItem(SkuNumber1.Text))
                                    {
                                        if (max < Convert.ToInt16(txtRetutn2.Text))
                                        {
                                            max = Convert.ToInt16(txtRetutn2.Text);
                                        }
                                    }
                                    _lsRMAInfo1.Add(ls);
                                }

                                RMAInfo ls1 = new RMAInfo();
                                SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                                ls1.SKUNumber = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                                ls1.ProductID = _mponumber.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                                ls1.SalesPrice = 0;
                                _lsIsmanually1.IsScanned = 1;
                                _lsIsmanually1.SKUName = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                                lsIsManually.Add(_lsIsmanually1);


                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                                ls1.SKU_Qty_Seq = 1;
                                ls1.SKU_Sequence = max + 1;
                                max = 0;

                                _lsRMAInfo1.Add(ls1);

                                this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                                dtLoadUpdate1 = new DispatcherTimer();
                                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                                //start the dispacher.
                                dtLoadUpdate1.Start();
                            }
                            Views.clGlobal.IsScanned = 1;
                            txtbarcode.Text = "";
                            txtbarcode.Focus();
                            flag = true;
                            count++;
                            //break;

                        }
                    }
                    if (!flag)
                    {
                        List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                        foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                        {
                            //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                            RMAInfo ls = new RMAInfo();
                            TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                            ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                            TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                            ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                            TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                            TextBlock ProductID = dgPackageInfo.Columns[7].GetCellContent(row1) as TextBlock;

                            TextBlock SalePrices = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;


                            ls.SKUNumber = SkuNumber1.Text;
                            ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                            ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                            ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                            ls.ProductID = ProductID.Text;

                            _lsRMAInfo1.Add(ls);
                        }

                        RMAInfo ls1 = new RMAInfo();
                        SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                        ls1.SKUNumber = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                        ls1.ProductID = _mponumber.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                        ls1.SalesPrice = 0;
                        _lsIsmanually1.IsScanned = 1;
                        _lsIsmanually1.SKUName = _mponumber.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                        lsIsManually.Add(_lsIsmanually1);


                        txtbarcode.Text = "";
                        txtbarcode.Focus();

                        ls1.SKU_Qty_Seq = 1;
                        ls1.SKU_Sequence = 1;
                        max = 0;

                        _lsRMAInfo1.Add(ls1);

                        this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                        dtLoadUpdate1 = new DispatcherTimer();
                        dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                        dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                        //start the dispacher.
                        dtLoadUpdate1.Start();

                    }
                    txtbarcode.Text = "";
                    txtbarcode.Focus();

                    if (CountSelected() == dgPackageInfo.Items.Count)
                    {
                        Views.clGlobal.WrongRMAFlag = "0";
                        cmbRMAStatus.SelectedIndex = 1;
                        // ErrorMsg("This is Correct RMA", Color.FromRgb(185, 84, 0));
                        txtbarcode.Text = "";
                        //  RMACheck = true;
                        count = 0;
                        txtbarcode.Focus();
                    }

                }
                #endregion
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _showBarcode();
            txtbarcode.Focus();
        }

        private void _showBarcode()
        {
            try
            {
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                {

                    DataGridRow row1 = (DataGridRow)row;
                    TextBlock SKUNo = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                    TextBlock LineType = dgPackageInfo.Columns[9].GetCellContent(row1) as TextBlock;

                    if (LineType.Text == "6")
                    {
                        ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                        DataTemplate DtQty = CntQuantity.ContentTemplate;
                        TextBlock txtproductName = (TextBlock)DtQty.FindName("tbQty", CntQuantity);
                        txtproductName.Text = "";

                        ContentPresenter _contentPar = dgPackageInfo.Columns[4].GetCellContent(row1) as ContentPresenter;
                        DataTemplate _dataTemplate = _contentPar.ContentTemplate;
                        Image _imgBarcode = (Image)_dataTemplate.FindName("imgBarCode", _contentPar);
                        // TextBlock txtComboNumber = (TextBlock)_dataTemplate.FindName("txtGroupID", _contentPar);
                        _imgBarcode.Visibility = Visibility.Hidden;

                        ContentPresenter CntSequence = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                        DataTemplate DtQty2 = CntSequence.ContentTemplate;
                        TextBlock txtproductName2 = (TextBlock)DtQty2.FindName("tbDQyt", CntSequence);

                        txtproductName2.Text = "";

                        ContentPresenter _contentPar1 = dgPackageInfo.Columns[3].GetCellContent(row1) as ContentPresenter;
                        DataTemplate _dataTemplate1 = _contentPar1.ContentTemplate;
                        StackPanel _imgBarcode1 = (StackPanel)_dataTemplate1.FindName("spProductImages", _contentPar1);
                        // TextBlock txtComboNumber = (TextBlock)_dataTemplate.FindName("txtGroupID", _contentPar);
                        _imgBarcode1.Visibility = Visibility.Hidden;

                        ContentPresenter CntQuantity1 = dgPackageInfo.Columns[0].GetCellContent(row1) as ContentPresenter;
                        DataTemplate DtQty1 = CntQuantity1.ContentTemplate;

                        Button buttonred = (Button)DtQty1.FindName("btnRed", CntQuantity1);

                        Button buttonGreen = (Button)DtQty1.FindName("btnGreen", CntQuantity1);

                        buttonred.Visibility = Visibility.Hidden;
                        buttonGreen.Visibility = Visibility.Hidden;

                        row1.IsEnabled = false;
                    }

                    String SkuName = SKUNo.Text.ToString();

                    //Convert SKU name to UPC COde;
                    String UPC_Code = _mponumber.GetENACodeByItem(SkuName);//_shipment.ShipmentDetailSage.FirstOrDefault(i => i.SKU == SkuName).UPCCode;
                    if (UPC_Code.Trim() == "") UPC_Code = "00000000000";

                    //clGlobal.call.SKUnameToUPCCode(SKUNo.Text.ToString());
                    ContentPresenter sp = dgPackageInfo.Columns[4].GetCellContent(row1) as ContentPresenter;
                    DataTemplate myDataTemplate = sp.ContentTemplate;
                    Image ImgbarcodSet = (Image)myDataTemplate.FindName("imgBarCode", sp);
                    System.Drawing.Image Barcodeimg = null;
                    try
                    {
                        Barcodeimg = b.Encode(BarcodeLib.TYPE.UPCA, UPC_Code, System.Drawing.Color.Black, System.Drawing.Color.White, 300, 60);
                    }
                    catch (Exception)
                    {
                        //Log the Error to the Error Log table
                        //  ErrorLoger.save("wndShipmentDetailPage - _showBarcode_Sub1", "[" + DateTime.UtcNow.ToString() + "]" + Ex.ToString(), DateTime.UtcNow, Global.LoggedUserId);
                    }
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();

                    // Save to a memory stream...
                    Barcodeimg.Save(ms, ImageFormat.Bmp);

                    // Rewind the stream...
                    ms.Seek(0, SeekOrigin.Begin);

                    // Tell the WPF image to use this stream...
                    bi.StreamSource = ms;
                    bi.EndInit();
                    ImgbarcodSet.Source = bi;

                    try
                    {
                        // txtScannSKu.Focus();
                    }
                    catch (Exception)
                    {
                        //Log the Error to the Error Log table
                        //  ErrorLoger.save("wndShipmentDetailPage - _showBarcode_Sub2", "[" + DateTime.UtcNow.ToString() + "]" + Ex.ToString(), DateTime.UtcNow, Global.LoggedUserId);
                    }

                }
            }
            catch (Exception)
            {
                //Log the Error to the Error Log table
                //  ErrorLoger.save("wndShipmentDetailPage - _showBarcode", "[" + DateTime.UtcNow.ToString() + "]" + Ex.ToString(), DateTime.UtcNow, Global.LoggedUserId);
            }
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

        void dtLoadUpdate1_Tick(object sender, EventArgs e)
        {
            dtLoadUpdate1.Stop();

            foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
            {
                TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                ContentPresenter CntQuantity = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                DataTemplate DtQty = CntQuantity.ContentTemplate;
                TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbDQyt", CntQuantity);


                ContentPresenter CntSkuStatus = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                DataTemplate DtSKuStatus = CntSkuStatus.ContentTemplate;
                TextBlock txtSkuStatus = (TextBlock)DtSKuStatus.FindName("tbskustatus", CntSkuStatus);

                for (int i = 0; i < listofstatus.Count; i++)
                {
                    if (listofstatus[i].NewItemQuantity == Convert.ToInt16(txtRetutn.Text) && listofstatus[i].SKUName == SkuNumber.Text)
                    {
                        // row1.IsEnabled = false;
                        row1.Background = Brushes.Gray;
                        txtSkuStatus.Text = listofstatus[i].Status;
                    }
                }

                if (txtRetutn1.Text == "1")
                {
                    row1.Background = Brushes.Gray;
                }
            }

            _showBarcode();

        }


        protected void SetGridChack(DataGrid Grid)
        {
            try
            {
                //SetReasons(_mUpdate._ReturnTbl.ReturnReason);
                foreach (DataGridRow row in GetDataGridRows(Grid))
                {


                    for (int i = 0; i < _mUpdate._lsReturnDetails1.Count(); i++)
                    {
                        // item SKUNumber
                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        //CheckBOx item Peresent
                        ContentPresenter CntPersenter = dgPackageInfo.Columns[0].GetCellContent(row) as ContentPresenter;
                        DataTemplate DataTemp = CntPersenter.ContentTemplate;
                        Button btnGreen = (Button)DataTemp.FindName("btnGreen", CntPersenter);
                        Button btnRed = (Button)DataTemp.FindName("btnRed", CntPersenter);

                        if (_mUpdate._lsReturnDetails1[i].SKUNumber == SkuNumber.Text && btnGreen.Visibility == System.Windows.Visibility.Hidden)
                        {
                            GreenRowsNumber.Add(row.GetIndex());
                            // btnGreen.Visibility = System.Windows.Visibility.Visible;
                            btnRed.Visibility = System.Windows.Visibility.Visible;

                            if (_mUpdate._lsReturnDetails1[i].SKU_Qty_Seq == 1)
                            {
                                // btnGreen.Visibility = System.Windows.Visibility.Visible; //row.IsEnabled = false;
                                row.Background = Brushes.Gray;
                            }

                            //item Returned Quantity.
                            //ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                            //DataTemplate DtQty = CntQuantity.ContentTemplate;
                            //TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);
                            //txtRetutn.Text = _mUpdate._lsReturnDetails1[i].ReturnQty.ToString();


                            //item Status.k
                            //ContentPresenter CntStatus = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                            //DataTemplate DtStatus = CntStatus.ContentTemplate;
                            //TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;
                            //TextBlock txtCheckedCount = DtStatus.FindName("txtCheckedCount", CntStatus) as TextBlock;

                            //txtRGuid.Text = GetReasonFronList(_mUpdate._lsReturnDetails[i].ReturnDetailID);

                            //txtCheckedCount.Text = ((txtRGuid.Text.ToString().Split(new char[] { '#' }).Count()) - 1).ToString() + " Reasons";

                            //Images Stack Panel.
                            ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                            DataTemplate DtImages = CntImag.ContentTemplate;
                            StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                            foreach (var Imgitem in _mUpdate._lsImages1)
                            {
                                if (Imgitem.ReturnDetailID == _mUpdate._lsReturnDetails1[i].ReturnDetailID)
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



        private int CountSelected()
        {
            int countselectedRow = 0;
            try
            {
                foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                {
                    if (row.Background == Brushes.Gray)
                    {
                        countselectedRow++;
                    }
                }
            }
            catch (Exception)
            {
            }
            return countselectedRow;
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
           // wndCamera camra = new wndCamera();
          //  camra.ShowDialog();
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

        //void FBCode_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (Views.clGlobal.FBCodeForSKU.BarcodeValueSKU != "")
        //        CaptureTime_Tick1(Views.clGlobal.FBCodeForSKU.BarcodeValueSKU);
        //}

        //void CaptureTime_Tick1(String BarcodeReded)
        //{
        //    if (BarcodeReded.Trim() != "")
        //    {
        //        try
        //        {
        //            txtbarcode.Text = BarcodeReded;
        //           // txtAddress.Text = BarcodeReded;
        //            txtbarcode.Focus();
                    
        //            InputSimulator.SimulateKeyDown(VirtualKeyCode.RETURN);
        //            Views.clGlobal.FBCodeForSKU.BarcodeValueSKU = "";
        //            Views.clGlobal.BarcodeValueFound = "";

        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("txtLoginKey :" + ex.Message);
        //        }
        //    }
        //}
    }
    
}
