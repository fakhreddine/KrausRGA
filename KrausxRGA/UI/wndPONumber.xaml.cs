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

        DataTable dt = new DataTable();

       // mReturnDetails _mReturn =clGlobal.mReturn;

        string SKU, _SKU;
        string PName, _PName;
        string Qty, _Qty;
        string Cat, _Cat;

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

        //private void cbrDamaged_Checked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrDamaged.Content.ToString());
        //    bdrDamaged.Inside();
        //}

        //private void cbrDamaged_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrDamaged.Content.ToString());
        //    bdrDamaged.Outside();
        //}

        //private void cbrDuplicate_Checked(object sender, RoutedEventArgs e)
        //{
        //   mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrDuplicate.Content.ToString());
        //    bdrDuplicate.Inside();
        //}

        //private void cbrDuplicate_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrDuplicate.Content.ToString());
        //    bdrDuplicate.Outside();
        //}

        //private void cbrIncorrectOrder_Checked(object sender, RoutedEventArgs e)
        //{
        //   mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrIncorrectOrder.Content.ToString());
        //    bdrIcorrectOrder.Inside();
        //}

        //private void cbrIncorrectOrder_Unchecked(object sender, RoutedEventArgs e)
        //{
        //   mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrIncorrectOrder.Content.ToString());
        //    bdrIcorrectOrder.Outside();
        //}

        //private void cbrDisplayedDiff_Checked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrDisplayedDiff.Content.ToString());
        //    bdrDisplayedDiff.Inside();
        //}

        //private void cbrDisplayedDiff_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrDisplayedDiff.Content.ToString());
        //    bdrDisplayedDiff.Outside();
        
        //}

        //private void cbrSatisfied_Checked(object sender, RoutedEventArgs e)
        //{
        //   mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrSatisfied.Content.ToString());
        //    bdrSatisfied.Inside();
        //}

        //private void cbrSatisfied_Unchecked(object sender, RoutedEventArgs e)
        //{
        //   mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrSatisfied.Content.ToString());
        //    bdrSatisfied.Outside();
        //}

        //private void cbrWrong_Checked(object sender, RoutedEventArgs e)
        //{
        //    bdrRecivedWrong.Inside();
        //}

        //private void cbrWrong_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    bdrRecivedWrong.Outside();
        //}

        private void btnHomeDone_Click(object sender, RoutedEventArgs e)
        {


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
            Guid ReturnTblID = _mponumber.SetReturnTbl(_lsreturn, "", RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID, wrongRMA, Warranty, 60, 0);
            MessageBox.Show("RMA number for this return is : " + _mNewRMA.GetNewROWID(ReturnTblID));
            foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
            {
                ////CheckBOx item Peresent
                //ContentPresenter CntPersenter = dgPackageInfo.Columns[0].GetCellContent(row) as ContentPresenter;
                //DataTemplate DataTemp = CntPersenter.ContentTemplate;
                //Button btnGreen = (Button)DataTemp.FindName("btnGreen", CntPersenter);

                //if (btnGreen.Visibility == System.Windows.Visibility.Visible)
                //{
                //    // If item present in the return 
                //    // item SKUNumber
                //    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                //    //Product Name.
                //    TextBlock ProcutName = dgPackageInfo.Columns[2].GetCellContent(row) as TextBlock;

                //    //item Returned Quantity.
                //    ContentPresenter CntQuantity = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                //    DataTemplate DtQty = CntQuantity.ContentTemplate;
                //    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                //    //Images Stack Panel.
                //    ContentPresenter CntImag = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                //    DataTemplate DtImages = CntImag.ContentTemplate;
                //    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                //    //item Status.k
                //    ContentPresenter CntStatus = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                //    DataTemplate DtStatus = CntStatus.ContentTemplate;
                //    TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;

                //    //Returned RMA Information.
                //    RMAInfo rmaInfo = _mponumber.lsRMAInformationforponumner.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text && xrm.ProductName == ProcutName.Text);
                //    int DeliveredQty = rmaInfo.DeliveredQty;
                //    Nullable<int> ExpectedQty = null;
                //    int v2;
                //    v2 = ExpectedQty.GetValueOrDefault();
                //    int v3;
                //    Nullable<int> ReturnQty = null;
                //    v3 = ReturnQty.GetValueOrDefault();
                //    string tck = rmaInfo.TCLCOD_0;

                //    //Set returned details table.
                //    Guid ReturnDetailsID = _mponumber.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, ProcutName.Text, DeliveredQty, v2, v3, tck, clGlobal.mCurrentUser.UserInfo.UserID);

                //    //Save Images info Table.
                //    foreach (Image imageCaptured in SpImages.Children)
                //    {
                //        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                //        //Set Images table from model.
                //        Guid ImageID = _mponumber.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                //    }

                //    //SKU Reasons Table
                //    foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                //    {
                //        _mponumber.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                //    }

                //    wndSlipPrint slip = new wndSlipPrint();

                //    Views.clGlobal.lsSlipInfo = _mponumber.GetSlipInfo(_lsreturn, SkuNumber.Text, _mNewRMA.GetENACodeByItem(SkuNumber.Text), "", _mNewRMA.GetNewROWID(ReturnTblID));

                //    slip.ShowDialog();

                //    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                //    Views.AuditType.lsaudit.Clear();
                //}
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

                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                    //item Status.k
                    ContentPresenter CntStatus = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtStatus = CntStatus.ContentTemplate;
                    TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;

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
                    Guid ReturnDetailsID = _mponumber.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, "Refund", 0);


                    Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0);



                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mponumber.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    //SKU Reasons Table
                    foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                    {
                        _mponumber.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                    }

                    wndSlipPrint slip = new wndSlipPrint();

                    Views.clGlobal.lsSlipInfo = _mponumber.GetSlipInfo(_lsreturn, SkuNumber.Text, _mNewRMA.GetENACodeByItem(SkuNumber.Text), "", _mNewRMA.GetNewROWID(ReturnTblID));

                    slip.ShowDialog();

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

                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                    //item Status.k
                    ContentPresenter CntStatus = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtStatus = CntStatus.ContentTemplate;
                    TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;

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
                    if (listofstatus.Count > 0)
                    {
                        for (int i = 0; i < listofstatus.Count; i++)
                        {
                            if (listofstatus[i].SKUName == SkuNumber.Text)
                            {
                                SKUNewName = SkuNumber.Text;
                                Views.clGlobal.SKU_Staus = listofstatus[i].Status;
                                Views.clGlobal.TotalPoints = listofstatus[i].Points;
                                break;
                            }
                        }
                    }
                    else
                    {
                        SKUNewName = SkuNumber.Text;
                        Views.clGlobal.SKU_Staus = "Reject";
                        Views.clGlobal.TotalPoints = 0;
                    }


                    //Set returned details table.
                    Guid ReturnDetailsID = _mponumber.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SKUNewName, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints);
                    // j++;

                    Views.clGlobal.SKU_Staus = "";
                    Views.clGlobal.TotalPoints = 0;
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), Convert.ToInt16(dt.Rows[i][3].ToString()));
                        }
                        dt.Clear();
                    }
                    else
                    {
                        Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0);
                    }

                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mponumber.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    //SKU Reasons Table
                    foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                    {
                        _mponumber.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                    }

                    if (!(Views.clGlobal.WrongRMAFlag == "1"))
                    {
                        wndSlipPrint slip = new wndSlipPrint();

                        Views.clGlobal.lsSlipInfo = _mponumber.GetSlipInfo(_lsreturn, SkuNumber.Text, _mNewRMA.GetENACodeByItem(SkuNumber.Text), "", _mNewRMA.GetNewROWID(ReturnTblID));

                        slip.ShowDialog();
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

                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                    //item Status.k
                    ContentPresenter CntStatus = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtStatus = CntStatus.ContentTemplate;
                    TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;

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
                    if (listofstatus.Count > 0)
                    {
                        for (int i = 0; i < listofstatus.Count; i++)
                        {
                            if (listofstatus[i].SKUName == SkuNumber.Text)
                            {
                                SKUNewName = SkuNumber.Text;
                                Views.clGlobal.SKU_Staus = listofstatus[i].Status;
                                Views.clGlobal.TotalPoints = listofstatus[i].Points;
                                break;
                            }
                        }
                    }
                    else
                    {
                        SKUNewName = SkuNumber.Text;
                        Views.clGlobal.SKU_Staus = "Reject";
                        Views.clGlobal.TotalPoints = 0;
                    }
                    Guid ReturnDetailsID = _mponumber.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Guid ReturnedSKUPoints = _mponumber.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), Convert.ToInt16(dt.Rows[i][3].ToString()));
                        }
                        dt.Clear();
                    }

                    Views.clGlobal.SKU_Staus = "";
                    Views.clGlobal.TotalPoints = 0;


                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mponumber.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    //SKU Reasons Table
                    foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                    {
                        _mponumber.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                    }

                    wndSlipPrint slip = new wndSlipPrint();

                    Views.clGlobal.lsSlipInfo = _mponumber.GetSlipInfo(_lsreturn, SkuNumber.Text, _mNewRMA.GetENACodeByItem(SkuNumber.Text), "", _mNewRMA.GetNewROWID(ReturnTblID));

                    slip.ShowDialog();

                    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                    Views.AuditType.lsaudit.Clear();
                }
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
        
        //private String ReturnReasons()
        //{
        //    String _ReturnReason = "";

        //    if (cbrDamaged.IsChecked == true) _ReturnReason = _ReturnReason + txtitemdamage.Text;

        //    if (cbrDisplayedDiff.IsChecked == true) _ReturnReason = _ReturnReason + txtDisplayedOff.Text;

        //    if (cbrDuplicate.IsChecked == true) _ReturnReason = _ReturnReason + txtDuplicate.Text;

        //    if (cbrIncorrectOrder.IsChecked == true) _ReturnReason = _ReturnReason + txtinccorectorder.Text;

        //    if (cbrSatisfied.IsChecked == true) _ReturnReason = _ReturnReason + txtSatisfied.Text;

        //    if (cbrWrong.IsChecked == true) _ReturnReason = _ReturnReason + txtreceicewrongitem.Text;

        //    _ReturnReason += txtOtherReason.Text;

        //    return _ReturnReason;

        //}

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
        
        //private void ContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrDamaged, txtitemdamage, cnvDamage);
        //}

        //private void ContentControl_MouseDown_2(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrWrong, txtreceicewrongitem, cnvRecieve);
        //}

        //private void ContentControl_MouseDown_4(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrIncorrectOrder, txtinccorectorder, cnvInccorectorder);
        //}

        //private void ContentControl_MouseDown_3(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrDisplayedDiff, txtDisplayedOff, cnvDisplayedOff);
        //}

        //private void ContentControl_MouseDown_1(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrDuplicate, txtDuplicate, cnvDuplicate);
        //}

        //private void ContentControl_MouseDown_5(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrSatisfied, txtSatisfied, cnvSatisfied);
        //}

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            wndBoxInformation boxinfo = new wndBoxInformation();
            clGlobal.IsUserlogged = true;
            boxinfo.Show();
            this.Close();
        }

        private void cmbOtherReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbOtherReason.SelectedIndex == 0)
            {
                txtOtherReason.Text = "";
            }
            else
            {
                Reason s = (Reason)cmbOtherReason.SelectedItem;
                txtOtherReason.Text = s.Reason1.ToString();
            }
        }

        public void FillRMAStausAndDecision()
        {
            cmbRMADecision.ItemsSource = _mNewRMA.GetRMAStatusList();
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


            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";

            lsReturn.Insert(0, re);

            cmbOtherReason.ItemsSource = lsReturn;

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

        //private void tbQty_KeyDown_1(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        var data = new RDetails { SKU = "", ProductName = "", Quantity = "1", cat = "" };

        //        dgPackageInfo.Items.Add(data);
        //    }
        //}

        //private void txtSKU_TextChanged_1(object sender, TextChangedEventArgs e)
        //{
        //    List<String> _lsNewRMAnumber = new List<string>();

        //    string che = ((System.Windows.Controls.TextBox)(e.Source)).Text.ToUpper();

        //    _lsNewRMAnumber = _mNewRMA.NewRMAInfo(che);

        //    lstSKU.Visibility = Visibility.Visible;

        //    lstSKU.ItemsSource = _lsNewRMAnumber;

        //}

        //private void lstSKU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //    ListBox itembox = (ListBox)sender;

        //    if (itembox.SelectedItem != null)
        //    {
        //        string item = lstSKU.SelectedItem.ToString();

        //        string[] NewRMA = item.Split(new char[] { '#' });

        //        string NewSKU = NewRMA[0];
        //        string NewPName = NewRMA[1];
        //        string Category = NewRMA[2];

        //        int index = dgPackageInfo.SelectedIndex;

        //        DataGridCell cell = GetCell(index, 0);
        //        ContentPresenter CntPersenter = cell.Content as ContentPresenter;
        //        DataTemplate DataTemp = CntPersenter.ContentTemplate;
        //        ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text = NewSKU;

        //        DataGridCell cell1 = GetCell(index, 1);
        //        ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
        //        DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;
        //        ((TextBox)DataTemp1.FindName("txtProductName", CntPersenter1)).Text = NewPName;

        //        DataGridCell cell2 = GetCell(index, 2);
        //        ContentPresenter CntPersenter2 = cell2.Content as ContentPresenter;
        //        DataTemplate DataTemp2 = CntPersenter2.ContentTemplate;
        //        ((TextBox)DataTemp2.FindName("tbQty", CntPersenter2)).Focus();

        //        InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);

        //        DataGridCell cell7 = GetCell(index, 5);
        //        ContentPresenter CntPersenter7 = cell7.Content as ContentPresenter;
        //        DataTemplate DataTemp7 = CntPersenter7.ContentTemplate;
        //        ((TextBox)DataTemp7.FindName("txtcategory", CntPersenter7)).Text = Category;

        //        lstSKU.Visibility = Visibility.Hidden;

        //    }
        //}


        // protected void _addToStackPanel(StackPanel StackPanelName, Image CapImage)
        //{
        //    try
        //    {
        //        StackPanelName.Children.Add(CapImage);
        //    }
        //    catch (Exception)
        //    { }
        //}
        
        //void img_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    Image img = (Image)sender;
        //    bdrZoomImage.Visibility = System.Windows.Visibility.Hidden;
        //    imgZoom.Source = img.Source;
        //}

        //private void imgZoom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    imgZoom.Source = null;
        //    bdrZoomImage.Visibility = System.Windows.Visibility.Hidden;
        //}

        //private void ContentControl_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        //{
        //    ContentControl cnt = (ContentControl)sender;
        //    DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();

        //    StackPanel spRowImages = cnt.FindName("spProductImages") as StackPanel;

        //    if (_mReturn.GreenRowsNumber.Contains(row.GetIndex()))
        //    {
        //        try
        //        {
        //            //Show Camera.
        //            Barcode.Camera.Open();
        //            foreach (String Nameitem in Views.clGlobal.lsImageList)
        //            {
        //                try
        //                {
        //                    string path = "C:\\Images\\";

        //                    BitmapSource bs = new BitmapImage(new Uri(path + Nameitem));

        //                    Image img = new Image();
        //                    //Zoom image.
        //                    img.MouseEnter += img_MouseEnter;

        //                    img.Height = 62;
        //                    img.Width = 74;
        //                    img.Stretch = Stretch.Fill;
        //                    img.Name = Nameitem.ToString().Split(new char[] { '.' })[0];
        //                    img.Source = bs;
        //                    img.Margin = new Thickness(0.5);

        //                    //Images added to the Row.
        //                    _addToStackPanel(spRowImages, img);
        //                }
        //                catch (Exception)
        //                {
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //        }
        //    }
        //    else
        //    {
        //        mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.SelectItem__00.ToString(), DateTime.UtcNow.ToString());
        //        ErrorMsg("Please select the item.", Color.FromRgb(185, 84, 0));
        //    }
        //}
    

      

      

        private void txtPoNumber_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
              List<RMAInfo> lsCustomeronfo=_mNewRMA.GetCustomer(txtPoNumber.Text);
              lstponumber.Visibility = Visibility.Hidden;   

              if (lsCustomeronfo.Count>0)
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
                  txtbarcode.Focus();
                  if (lsCustomeronfo[0].VendorNumber.ToString() == "GENC0001" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0404" || lsCustomeronfo[0].VendorNumber.ToString() == "INTC0017" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0551" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0795")
                  {
                      Views.clGlobal.ScenarioType = "HomeDepot";
                      txtbarcode.Focus();
                      ErrorMsg("This is HomeDepot RMA Please Check this is WrongRMA or Not Byscanning the Barcode.", Color.FromRgb(185, 84, 0));

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
                  }
                  _mponumber.lsRMAInformationforponumner = lsCustomeronfo;
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
                    txtVendorName.Text = lsCustomeronfo[0].VendorName;
                    txtVendorNumber.Text = lsCustomeronfo[0].VendorNumber;
                    txtPoNumber.Text = lsCustomeronfo[0].PONumber;
                    txtAddress.Text = lsCustomeronfo[0].Address1;
                    txtCountry.Text = lsCustomeronfo[0].Country;
                    txtCustCity.Text = lsCustomeronfo[0].City;
                    txtState.Text = lsCustomeronfo[0].State;
                    txtZipCode.Text = lsCustomeronfo[0].ZipCode;
                    txtName.Text = lsCustomeronfo[0].CustomerName1;
                    dgPackageInfo.ItemsSource = lsCustomeronfo;
                    _lsRMAInfo = lsCustomeronfo;
                    txtbarcode.Focus();
                    if (lsCustomeronfo[0].VendorNumber.ToString() == "GENC0001" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0404" || lsCustomeronfo[0].VendorNumber.ToString() == "INTC0017" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0551" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0795")
                    {
                        Views.clGlobal.ScenarioType = "HomeDepot";
                        txtbarcode.Focus();
                        ErrorMsg("This is HomeDepot RMA Please Check this is WrongRMA or Not Byscanning the Barcode.", Color.FromRgb(185, 84, 0));

                    }

                    else if (lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0143" || lsCustomeronfo[0].VendorNumber.ToString() == "DOMC0432")
                    {
                        Views.clGlobal.ScenarioType = "Lowes";
                        txtbarcode.Focus();
                        ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                        //dgPackageInfo.IsEnabled = false;
                    }
                    else
                    {
                        Views.clGlobal.ScenarioType = "Others";
                        txtbarcode.Focus();
                    }

                    _mponumber.lsRMAInformationforponumner = lsCustomeronfo;
                }
                lstponumber.Visibility = Visibility.Hidden;    
            }
        }

        //private void txtVendorNumber_KeyDown_1(object sender, KeyEventArgs e)
        //{
        //    if (Key.Down==Key.Enter)
        //    {
        //         txtVendorName.Text = _mNewRMA.GetVenderNameByVenderNumber(txtVendorNumber.Text);
        //    }
        //    lstVenderNumber.Visibility = Visibility.Hidden;   
        //}

        //private void txtVendorNumber_TextChanged_1(object sender, TextChangedEventArgs e)
        //{
        //    if (txtVendorNumber.Text == "")
        //    {
        //        lstVenderNumber.Visibility = Visibility.Hidden;
        //        txtVendorName.Text = "";
        //    }
        //    else
        //    {
        //        lstVenderNumber.ItemsSource = _mNewRMA.GetVanderNumber(txtVendorNumber.Text.ToUpper());
        //        lstVenderNumber.Visibility = Visibility.Visible;
        //    }
        //}

        //private void lstVenderNumber_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        //{
        //    if (lstVenderNumber.SelectedItem!=null)
        //    {
        //        txtVendorNumber.Text = lstVenderNumber.SelectedItem.ToString();
        //        txtVendorName.Text = _mNewRMA.GetVenderNameByVenderNumber(txtVendorNumber.Text);
        //    }
        //    lstVenderNumber.Visibility = Visibility.Hidden;
        //    lstVenderName.Visibility = Visibility.Hidden;
        //}

        //private void lstVenderName_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        //{
        //    if (lstVenderName.SelectedItem != null)
        //    {
        //        txtVendorName.Text = lstVenderName.SelectedItem.ToString();
        //        txtVendorNumber.Text = _mNewRMA.GetVenderNumberByVenderName(txtVendorName.Text);
        //    }
        //    lstVenderName.Visibility = Visibility.Hidden;
        //    lstVenderNumber.Visibility = Visibility.Hidden;
        //}

        //private void txtVendorName_KeyDown_1(object sender, KeyEventArgs e)
        //{
        //    if (Key.Down == Key.Enter)
        //    {
        //        txtVendorNumber.Text = _mNewRMA.GetVenderNumberByVenderName(txtVendorName.Text);
        //    }
        //    lstVenderName.Visibility = Visibility.Hidden;   
        //}

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

        //private void txtVendorNumber_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (txtVendorNumber.Text == "")
        //    {
        //        lstVenderNumber.Visibility = Visibility.Hidden;
        //       // txtVendorName.Text = "";
        //    }
        //    else
        //    {
        //        lstVenderNumber.ItemsSource = _mNewRMA.GetVanderNumber(txtVendorNumber.Text.ToUpper());
        //        lstVenderNumber.Visibility = Visibility.Visible;
        //    }
        //}

        //private void txtVendorName_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    lstVenderName.Visibility = Visibility.Hidden;
        //}

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
                    txtbarcode.Focus();
                }
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

                Button btnRed = (Button)e.Source;
                Canvas SpButtons = (Canvas)btnRed.Parent;
                Button btnGreen = SpButtons.FindName("btnGreen") as Button;
                btnGreen.Visibility = System.Windows.Visibility.Visible;
                btnRed.Visibility = System.Windows.Visibility.Hidden;

                DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
                GreenRowsNumber1.Add(row.GetIndex());
                bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                txtbarcode.Focus();

                mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
            }

          //  mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
        }

        private void btnPluse_Click(object sender, RoutedEventArgs e)
        {
            StackPanel Sp = (StackPanel)(sender as Control).Parent;
            StackPanel Sp2 = (StackPanel)Sp.Parent;
            DataGridRow row = (DataGridRow)Sp2.FindParent<DataGridRow>();
            if (GreenRowsNumber.Contains(row.GetIndex()))
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
                if (GreenRowsNumber.Contains(row.GetIndex()))
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Reason> lsReturn = _mNewRMA.GetReasons();


            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";

            lsReturn.Insert(0, re);

            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Reason", typeof(string));
            dt.Columns.Add("Reason_Value", typeof(string));
            dt.Columns.Add("Points", typeof(int));

            txtbarcode.Focus();
            cmbOtherReason.ItemsSource = lsReturn;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Views.clGlobal.ScenarioType == "HomeDepot")
            {
                //  DataTable dt1 = new DataTable();


                string SelectedskuName = "";
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
                    }
                }
                DataRow dr = dt.NewRow();
                dr["SKU"] = SelectedskuName;
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
                if (btnStatusYes.IsChecked == true)
                {
                    dr2["Reason"] = lblStatus.Content;
                    dr2["Reason_Value"] = "Yes";
                    dr2["Points"] = 100;
                    dt.Rows.Add(dr2);
                }
                else if (btnStatusNo.IsChecked == true)
                {
                    dr2["Reason"] = lblStatus.Content;
                    dr2["Reason_Value"] = "No";
                    dr2["Points"] = 0;
                    dt.Rows.Add(dr2);
                }

                DataRow dr3 = dt.NewRow();
                dr3["SKU"] = SelectedskuName;
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
                listofstatus.Add(_lsstatusandpoints);

                lblpoints.Content = "";
                points = 0;

                int ro = dt.Rows.Count;
                UncheckAllButtons();
                ErrorMsg("Please Select Next Item or Go ahead.", Color.FromRgb(185, 84, 0));
            }


            if (Views.clGlobal.ScenarioType == "Others")
            {
                //  DataTable dt1 = new DataTable();



                string SelectedskuName = "";
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
                    }
                }
                DataRow dr = dt.NewRow();
                dr["SKU"] = SelectedskuName;
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
                if (btnStatusYes.IsChecked == true)
                {
                    dr2["Reason"] = lblStatus.Content;
                    dr2["Reason_Value"] = "Yes";
                    dr2["Points"] = 100;
                    dt.Rows.Add(dr2);
                }
                else if (btnStatusNo.IsChecked == true)
                {
                    dr2["Reason"] = lblStatus.Content;
                    dr2["Reason_Value"] = "No";
                    dr2["Points"] = 0;
                    dt.Rows.Add(dr2);
                }


                DataRow dr3 = dt.NewRow();
                dr3["SKU"] = SelectedskuName;
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
                listofstatus.Add(_lsstatusandpoints);

                lblpoints.Content = "";
                points = 0;



                int ro = dt.Rows.Count;
                UncheckAllButtons();
                ErrorMsg("Please Select Next Item or Go ahead.", Color.FromRgb(185, 84, 0));
            }
        }
        Boolean itemnew = true;
        private void btnBoxNew_Checked_1(object sender, RoutedEventArgs e)
        {
            if (itemnew)
            {
                points = points + 100;
                lblpoints.Content = points.ToString();
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
            }
            else
            {
                points = points + 0;
                lblpoints.Content = points.ToString();
            }
            itemnew = true;
        }

        private void btnInstalledYes_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!ISinstalled)
            {
                points = points - 100;
                lblpoints.Content = points.ToString();
            }
            else
            {
                points = points + 0;
                lblpoints.Content = points.ToString();
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
            btnManufacturerNo.IsChecked = false;
            btnManufacturerYes.IsChecked = false;
            btntransiteNo.IsChecked = false;
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
                Views.clGlobal.SKU_Staus = "Refund";
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
                Views.clGlobal.SKU_Staus = "Refund";
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
        private void txtbarcode_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Boolean flag = false;
                //Boolean RMACheck = false;


                if (Views.clGlobal.ScenarioType == "Lowes")
                {
                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {

                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        string sku = _mponumber.GetENACodeByItem(SkuNumber.Text);
                        if ("0" + sku == txtbarcode.Text)
                        {
                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            row.Background = Brushes.Gray;
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

                        foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                        {
                            RMAInfo ls = new RMAInfo();
                            TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                            ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                            DataTemplate DtQty = CntQuantity.ContentTemplate;
                            TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                            ls.SKUNumber = SkuNumber.Text;
                            ls.ReturnedQty = Convert.ToInt16(txtRetutn.Text);

                            _lsRMAInfo1.Add(ls);
                        }

                        RMAInfo ls1 = new RMAInfo();


                        ls1.SKUNumber = _mponumber.GetSKUNameByItem(txtbarcode.Text.Remove(0, 1));

                        txtbarcode.Text = "";
                        txtbarcode.Focus();

                        ls1.ReturnedQty = 0;

                        _lsRMAInfo1.Add(ls1);

                        dgPackageInfo.ItemsSource = _lsRMAInfo1;
                    }
                }

                if (Views.clGlobal.ScenarioType == "HomeDepot")
                {

                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {

                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        string sku = _mponumber.GetENACodeByItem(SkuNumber.Text);
                        if ("0" + sku == txtbarcode.Text)
                        {
                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            row.Background = Brushes.Gray;
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
                        ErrorMsg("This item is Wrong.", Color.FromRgb(185, 84, 0));
                        Views.clGlobal.SKU_Staus = "Reject";
                        Views.clGlobal.TotalPoints = points;// lblpoints.Content;
                        Views.clGlobal.Warranty = "N/A";


                        MessageBox.Show("This item is Wrong.");

                        btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                        wndBoxInformation boxinfo = new wndBoxInformation();
                        clGlobal.IsUserlogged = true;
                        boxinfo.Show();
                        this.Close();

                        txtbarcode.Text = "";
                        txtbarcode.Focus();
                    }

                    if (count == dgPackageInfo.Items.Count)
                    {
                        Views.clGlobal.WrongRMAFlag = "0";
                        ErrorMsg("This is Correct RMA", Color.FromRgb(185, 84, 0));
                        txtbarcode.Text = "";
                        //  RMACheck = true;
                        count = 0;
                        txtbarcode.Focus();

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
                            Views.clGlobal.Warranty = "0";

                            MessageBox.Show("This Return is NOT in Warranty.");

                            btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                            wndBoxInformation boxinfo = new wndBoxInformation();
                            clGlobal.IsUserlogged = true;
                            boxinfo.Show();
                            this.Close();

                            txtbarcode.Text = "";
                            txtbarcode.Focus();
                        }
                    }
                }

                if (Views.clGlobal.ScenarioType == "Others")
                {
                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {

                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        string sku = _mponumber.GetENACodeByItem(SkuNumber.Text);
                        if ("0" + sku == txtbarcode.Text)
                        {
                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            row.Background = Brushes.Gray;
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

                        foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                        {
                            RMAInfo ls = new RMAInfo();
                            TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                            ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                            DataTemplate DtQty = CntQuantity.ContentTemplate;
                            TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                            ls.SKUNumber = SkuNumber.Text;
                            ls.ReturnedQty = Convert.ToInt16(txtRetutn.Text);

                            _lsRMAInfo1.Add(ls);
                        }

                        RMAInfo ls1 = new RMAInfo();

                        string ss = txtbarcode.Text.Remove(0, 1);
                        ls1.SKUNumber = _mponumber.GetSKUNameByItem(txtbarcode.Text.Remove(0, 1));

                        txtbarcode.Text = "";
                        txtbarcode.Focus();

                        //ls1.SKUNumber = _mReturn.GetSKUNameByItem("846639015226");
                        ls1.ReturnedQty = 0;
                        // .Remove(0, 1)
                        _lsRMAInfo1.Add(ls1);

                        dgPackageInfo.ItemsSource = _lsRMAInfo1;
                    }
                    if (count == dgPackageInfo.Items.Count)
                    {
                        Views.clGlobal.WrongRMAFlag = "0";
                        ErrorMsg("This is Correct RMA", Color.FromRgb(185, 84, 0));
                        txtbarcode.Text = "";
                        //  RMACheck = true;
                        count = 0;
                        txtbarcode.Focus();

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
                            Views.clGlobal.Warranty = "0";

                            MessageBox.Show("This Return is NOT in Warranty.");

                            btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                            wndBoxInformation boxinfo = new wndBoxInformation();
                            clGlobal.IsUserlogged = true;
                            boxinfo.Show();
                            this.Close();

                            txtbarcode.Text = "";
                            txtbarcode.Focus();
                        }
                    }
                }

                //if (!RMACheck)
                //{
                //    ErrorMsg("This is Wrong RMA", Color.FromRgb(185, 84, 0));
                //    txtbarcode.Text = "";
                //    txtbarcode.Focus();
                //}
                //else
                //{
                //    ErrorMsg("This is Wrong RMA", Color.FromRgb(185, 84, 0));
                //    txtbarcode.Text = "";
                //    txtbarcode.Focus();
                //}
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


                    String SkuName = SKUNo.Text.ToString();

                    //Convert SKU name to UPC COde;
                    String UPC_Code = _mponumber.GetENACodeByItem(SkuName);//_shipment.ShipmentDetailSage.FirstOrDefault(i => i.SKU == SkuName).UPCCode;
                    if (UPC_Code.Trim() == "") UPC_Code = "00000000000";

                    //clGlobal.call.SKUnameToUPCCode(SKUNo.Text.ToString());
                    ContentPresenter sp = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
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

       


        //protected void SetGridChack(DataGrid Grid)
        //{
        //    try
        //    {
        //        SetReasons(_mUpdate._ReturnTbl.ReturnReason);
        //        foreach (DataGridRow row in GetDataGridRows(Grid))
        //        {


        //            for (int i = 0; i < _mUpdate._lsReturnDetails.Count(); i++)
        //            {
        //                // item SKUNumber
        //                TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
        //                //CheckBOx item Peresent
        //                ContentPresenter CntPersenter = dgPackageInfo.Columns[0].GetCellContent(row) as ContentPresenter;
        //                DataTemplate DataTemp = CntPersenter.ContentTemplate;
        //                Button btnGreen = (Button)DataTemp.FindName("btnGreen", CntPersenter);
        //                Button btnRed = (Button)DataTemp.FindName("btnRed", CntPersenter);

        //                if (_mUpdate._lsReturnDetails[i].SKUNumber == SkuNumber.Text && btnGreen.Visibility == System.Windows.Visibility.Hidden)
        //                {
        //                    _mReturn.GreenRowsNumber.Add(row.GetIndex());
        //                    btnGreen.Visibility = System.Windows.Visibility.Visible;
        //                    btnRed.Visibility = System.Windows.Visibility.Hidden;
        //                    //item Returned Quantity.
        //                    ContentPresenter CntQuantity = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
        //                    DataTemplate DtQty = CntQuantity.ContentTemplate;
        //                    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);
        //                    txtRetutn.Text = _mUpdate._lsReturnDetails[i].ReturnQty.ToString();


        //                    //item Status.k
        //                    ContentPresenter CntStatus = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
        //                    DataTemplate DtStatus = CntStatus.ContentTemplate;
        //                    TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;
        //                    TextBlock txtCheckedCount = DtStatus.FindName("txtCheckedCount", CntStatus) as TextBlock;

        //                    txtRGuid.Text = GetReasonFronList(_mUpdate._lsReturnDetails[i].ReturnDetailID);

        //                    txtCheckedCount.Text = ((txtRGuid.Text.ToString().Split(new char[] { '#' }).Count()) - 1).ToString() + " Reasons";

        //                    //Images Stack Panel.
        //                    ContentPresenter CntImag = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
        //                    DataTemplate DtImages = CntImag.ContentTemplate;
        //                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

        //                    foreach (var Imgitem in _mUpdate._lsImages)
        //                    {
        //                        if (Imgitem.ReturnDetailID == _mUpdate._lsReturnDetails[i].ReturnDetailID)
        //                        {
        //                            try
        //                            {
        //                                BitmapSource bs = new BitmapImage(new Uri(Imgitem.SKUImagePath));

        //                                Image img = new Image();
        //                                //Zoom image.
        //                                img.MouseEnter += img_MouseEnter;

        //                                img.Height = 62;
        //                                img.Width = 74;
        //                                img.Stretch = Stretch.Fill;
        //                                String Name = Imgitem.SKUImagePath.Remove(0, Imgitem.SKUImagePath.IndexOf("SR"));
        //                                img.Name = Name.ToString().Split(new char[] { '.' })[0];
        //                                img.Source = bs;
        //                                img.Margin = new Thickness(0.5);

        //                                //Images added to the Row.
        //                                _addToStackPanel(SpImages, img);
        //                            }
        //                            catch (Exception)
        //                            {
        //                            }
        //                        }
        //                    }
        //                }

        //            }
        //        }

        //    }
        //    catch (Exception)
        //    { }
        //}
    }
    
}
