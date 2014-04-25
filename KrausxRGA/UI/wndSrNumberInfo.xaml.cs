﻿using System;
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
using System.Windows.Threading;
using System.ComponentModel;
using System.Data;

namespace KrausRGA.UI
{

    /// <summary>
    /// Interaction logic for wndSrNumberInfo.xaml
    /// </summary>
    public partial class wndSrNumberInfo : Window
    {

        #region Declarations.
        //where the images captured stored.
        string imgPath = KrausRGA.Properties.Settings.Default.DrivePath;
        //user models object.
        mUser _mUser = clGlobal.mCurrentUser;
        
        //return details model object.
        mReturnDetails _mReturn = clGlobal.mReturn;
        
        //Print slip class list.
        List<cSlipInfo> _lsSlpiInfo = new List<cSlipInfo>();
        
        //RMA information from sage list.
        List<RMAInfo> _lsRMAInfo = new List<RMAInfo>();
        
        //Update mode opened saves the details of RMA.
        mUpdateModeRMA _mUpdate;

        //Stack Panel in row assigned to this and used in Images captured add.
        StackPanel spRowImages;

        //Scroll Viewer from selected Row;
        ScrollViewer SvImagesScroll;

        //Dispacher that works when the RMA number opend in Upadate mode.
        DispatcherTimer dtLoadUpdate;

        BackgroundWorker Worker = new BackgroundWorker();


        List<StatusAndPoints> listofstatus = new List<StatusAndPoints>();

        //recoded saving thread.
        public static Thread thSaving;

        DataTable dt = new DataTable();

        #endregion

        public wndSrNumberInfo()
        {
            #region Font size set of the application
            
            //get the font sizes from the text file.
            String[] FontSizes = File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[1].Split(new char[] { '-' });
            String HeaderSize = FontSizes[1];
            String ControlSize = FontSizes[2];
            String VeriableSize = FontSizes[0];
            Resources["FontSize"] = Convert.ToDouble(VeriableSize);
            Resources["HeaderSize"] = Convert.ToDouble(HeaderSize);
            Resources["ContactFontSize"] = Convert.ToDouble(ControlSize);

           


            #endregion

            InitializeComponent();

            Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
        }
        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                this.Dispatcher.Invoke(new Action(_showBarcode));

            }
            catch (Exception)
            {
                //Log the Error to the Error Log table
                //   ErrorLoger.save("wndShipmentDetailPage - Worker_DoWork", "[" + DateTime.UtcNow.ToString() + "]" + Ex.ToString(), DateTime.UtcNow, Global.LoggedUserId);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //fill status and Decision combobox;
            FillRMAStausAndDecision();
            txtbarcode.Focus();
            //fill OtherReason ComboBox
            List<Reason> lsReturn = _mReturn.GetReasons();

            //add reason select to the Combobox other reason.
            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";
            lsReturn.Insert(0, re);
            cmbOtherReason.ItemsSource = lsReturn;


            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Reason", typeof(string));
            dt.Columns.Add("Reason_Value", typeof(string));
            dt.Columns.Add("Points", typeof(int));
           



            //RMA information assigned from the Model of Return.
            _lsRMAInfo = _mReturn.lsRMAInformation;

            #region Display all to the window..
            
            lblRMANumber.Content = _lsRMAInfo[0].RMANumber;
            tbCustomerName.Text = _lsRMAInfo[0].CustomerName1;
            lblRMAReqDate.Content = _lsRMAInfo[0].ReturnDate.ToString("MMM dd, yyyy");
            lblVendorNumber.Content = _lsRMAInfo[0].VendorNumber.ToString();
            lblVendorName.Content = _lsRMAInfo[0].VendorName;
            lblPoNumber.Content = _lsRMAInfo[0].PONumber.ToString();
            lblCustomerAddress.Text = _lsRMAInfo[0].Address1;
            lblCustCity.Content = _lsRMAInfo[0].City;
            lblState.Content = _lsRMAInfo[0].State;
            lblZipCode.Content = _lsRMAInfo[0].ZipCode;
            lblCountry.Content = _lsRMAInfo[0].Country;
            lblExpirationDate.Content = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(60), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).ToString("MMM dd, yyyy");
            dgPackageInfo.ItemsSource = _lsRMAInfo;

            if (_lsRMAInfo[0].VendorNumber.ToString() == "GENC0001" || _lsRMAInfo[0].VendorNumber.ToString() == "DOMC0404" || _lsRMAInfo[0].VendorNumber.ToString() == "INTC0017" || _lsRMAInfo[0].VendorNumber.ToString() == "DOMC0551" || _lsRMAInfo[0].VendorNumber.ToString() == "DOMC0795")
            {
                Views.clGlobal.ScenarioType = "HomeDepot";
                ErrorMsg("This is HomeDepot RMA Please Check this is WrongRMA or Not Byscanning the Barcode.", Color.FromRgb(185, 84, 0));
                
            }

            else if (_lsRMAInfo[0].VendorNumber.ToString() == "DOMC0143" || _lsRMAInfo[0].VendorNumber.ToString() == "DOMC0432")
            {
                Views.clGlobal.ScenarioType = "Lowes";
                ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                //dgPackageInfo.IsEnabled = false;
            }
            else
            {
                Views.clGlobal.ScenarioType = "Others";
            }
            
            

          //  _showBarcode();
            #endregion

            #region Update mode RMA.
            //RMA number is already present in the database.
            if (Views.clGlobal.mReturn.IsAlreadySaved)
            {
                //Get the all information from datebase to the Update mode from RMA Number.
                _mUpdate = new mUpdateModeRMA(Views.clGlobal.mReturn.lsRMAInformation[0].RMANumber);
                
                //Show the Expiry date.
                lblExpirationDate.Content = _mUpdate._ReturnTbl.ExpirationDate.ToString("MMM dd yyyy");
                
                //Initialize the Dispacher that shows all values from the Update model.
                dtLoadUpdate = new DispatcherTimer();
                dtLoadUpdate.Interval = new TimeSpan(0, 0, 0, 0, 100);
                dtLoadUpdate.Tick += dtLoadUpdate_Tick;
                //start the dispacher.
                dtLoadUpdate.Start();
            }
 
            #endregion
            this.Dispatcher.Invoke(new Action(() => { Button_Click_1(btnshow, new RoutedEventArgs { }); }));
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
                    String UPC_Code = _mReturn.GetENACodeByItem(SkuName);//_shipment.ShipmentDetailSage.FirstOrDefault(i => i.SKU == SkuName).UPCCode;
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
        void dtLoadUpdate_Tick(object sender, EventArgs e)
        {
            dtLoadUpdate.Stop();
            //set the all setting from update model.
            SetGridChack(dgPackageInfo);
            
        }

        #region Data Grid Events.

        private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentControl cnt = (ContentControl)sender;
            DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();

            StackPanel spRowImages = cnt.FindName("spProductImages") as StackPanel;

            if (_mReturn.GreenRowsNumber.Contains(row.GetIndex()))
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

        private void dgPackageInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = dgPackageInfo.SelectedIndex;
            if (selectedIndex != -1)
            {
                foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                {
                    if (row.IsSelected)
                    {
                        ContentPresenter cp = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                        DataTemplate Dt = cp.ContentTemplate;
                        StackPanel spProductIMages = (StackPanel)Dt.FindName("spProductImages", cp);
                        spRowImages = spProductIMages;
                        ScrollViewer SvImages = (ScrollViewer)Dt.FindName("svScrollImages", cp);
                        SvImagesScroll = SvImages;
                    }
                }
            }
        }

        #endregion

        #region Web cam Methods


        #region Zoom Images.

        void img_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            bdrZoomImage.Visibility = System.Windows.Visibility.Visible;
            imgZoom.Source = img.Source;
        }

        private void imgZoom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgZoom.Source = null;
            bdrZoomImage.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        /// <summary>
        /// remove child controles from Stackpanel
        /// </summary>
        /// <param name="stackPacnel">
        /// StackPanel UI Element
        /// </param>
        private void removeStackPanelChild(StackPanel stackPacnel)
        {
            try
            {
                while (stackPacnel.Children.Count > 0)
                {
                    stackPacnel.Children.RemoveAt(stackPacnel.Children.Count - 1);
                }
            }
            catch (Exception)
            { }
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
       
        #endregion

        #region Functions.

        /// <summary>
        /// This is to all return DataGridRows Object
        /// </summary>
        /// <param name="grid"> Grid View object</param>
        /// <returns>DataGridRow</returns>
        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }


        public void FillRMAStausAndDecision()
        {
            cmbRMADecision.ItemsSource = _mReturn.GetRMAStatusList();
            cmbRMAStatus.ItemsSource = _mReturn.GetRMAStatusList();
        }

        /// <summary>
        /// check the Checked chekboxes from return reason tab
        /// and combine strings of checked checkboxes.
        /// Default includes Other reason.
        /// </summary>
        /// <returns>
        /// String Return Reason.
        /// </returns>
        //private String ReturnReasons()
        //{
            //String _ReturnReason = "";
            //if (cbrDamaged.IsChecked == true) _ReturnReason = _ReturnReason + txtitemdamage.Text;

            //if (cbrDisplayedDiff.IsChecked == true) _ReturnReason = _ReturnReason + txtDisplayedOff.Text;

            //if (cbrDuplicate.IsChecked == true) _ReturnReason = _ReturnReason + txtDuplicate.Text;

            //if (cbrIncorrectOrder.IsChecked == true) _ReturnReason = _ReturnReason + txtinccorectorder.Text;

            //if (cbrSatisfied.IsChecked == true) _ReturnReason = _ReturnReason + txtSatisfied.Text;

            //if (cbrWrong.IsChecked == true) _ReturnReason = _ReturnReason + txtreceicewrongitem.Text;

            //_ReturnReason += txtOtherReason.Text;

            //return _ReturnReason;

       // }

        /// <summary>
        /// Find Guid from row.
        /// </summary>
        /// <param name="Row">
        /// dgReasons Row object.
        /// </param>
        /// <returns>
        /// string Guid.
        /// </returns>
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
        #endregion

        private void btnHomeDone_Click(object sender, RoutedEventArgs e)
        {
          //  WindowThread.start();

            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Done_Clicked.ToString(), DateTime.UtcNow.ToString(), _mReturn.EnteredNumber);
          
            //Check the ReasonCombo Select or not
            if (cmbOtherReason.forcombobox())
            {
                mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.New_ReturnReason_Added.ToString(), DateTime.UtcNow.ToString());
                Guid reasonID = _mReturn.SetReasons(txtOtherReason.Text);
            }
            txtOtherReason.Text = "";
            txtItemReason.Text = "";

            bdrMsg.Visibility = System.Windows.Visibility.Hidden;

            Byte RMAStatus = Convert.ToByte(cmbRMAStatus.SelectedValue.ToString());
            Byte Decision = Convert.ToByte(cmbRMADecision.SelectedValue.ToString());
            DateTime ScannedDate = DateTime.UtcNow;
            DateTime ExpirationDate = DateTime.UtcNow.AddDays(60);
            //Save to RMA Master Table.
            //Guid ReturnTblID;

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
            Guid ReturnTblID = _mReturn.SetReturnTbl("", RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID, ScannedDate, ExpirationDate, wrongRMA, Warranty, 60, 0);//ReturnReasons()
            
            if (Views.clGlobal.mReturn.IsAlreadySaved)
            {
                ReturnTblID = _mUpdate._ReturnTbl.ReturnID;
                foreach (var ReturnDetailsID in _mUpdate._lsReturnDetails)
                {
                    _mReturn.DeleteReturnDetails(ReturnDetailsID.ReturnDetailID);
                }
            }
            int j = 0;
            foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
            {
                //CheckBOx item Peresent
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
                        RMAInfo rmaInfo = _mReturn.lsRMAInformation.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
                        int DeliveredQty = 0;//rmaInfo.DeliveredQty;
                        int ExpectedQty = 0;//rmaInfo.ExpectedQty;
                        string tck = "";//rmaInfo.TCLCOD_0;

                        //Set returned details table.
                        Guid ReturnDetailsID = _mReturn.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, "Refund", 0);


                        Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(),ReturnDetailsID,ReturnTblID, SkuNumber.Text, "sf", "sdf", 0);



                        //Save Images info Table.
                        foreach (Image imageCaptured in SpImages.Children)
                        {
                            String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                            //Set Images table from model.
                            Guid ImageID = _mReturn.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                        }

                        //SKU Reasons Table
                        foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                        {
                            _mReturn.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                        }

                        wndSlipPrint slip = new wndSlipPrint();

                        Views.clGlobal.lsSlipInfo = _mReturn.GetSlipInfo(SkuNumber.Text, _mReturn.GetENACodeByItem(SkuNumber.Text), _mReturn.GetSageReasonBySKUSR(lblRMANumber.Content.ToString(), SkuNumber.Text), ScannedDate, ExpirationDate);

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
                        RMAInfo rmaInfo = _mReturn.lsRMAInformation.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
                        int DeliveredQty = 0;//rmaInfo.DeliveredQty;
                        int ExpectedQty = 0;//rmaInfo.ExpectedQty;
                        string tck = "";//rmaInfo.TCLCOD_0;

                        //for (int i = 0; i < listofstatus.Count; i++)
                        //{
                            
                        //}


                        //Set returned details table.
                        Guid ReturnDetailsID = _mReturn.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, listofstatus[j].Status, listofstatus[j].Points);
                        j++;
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(),Convert.ToInt16(dt.Rows[i][3].ToString()));
                            }
                            dt.Clear();
                        }
                        else
                        {
                            Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0);    
                        }
                        
                        //Save Images info Table.
                        foreach (Image imageCaptured in SpImages.Children)
                        {
                            String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                            //Set Images table from model.
                            Guid ImageID = _mReturn.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                        }

                        //SKU Reasons Table
                        foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                        {
                            _mReturn.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                        }

                        if (!(Views.clGlobal.WrongRMAFlag=="1"))
                        {
                            wndSlipPrint slip = new wndSlipPrint();

                            Views.clGlobal.lsSlipInfo = _mReturn.GetSlipInfo(SkuNumber.Text, _mReturn.GetENACodeByItem(SkuNumber.Text), _mReturn.GetSageReasonBySKUSR(lblRMANumber.Content.ToString(), SkuNumber.Text), ScannedDate, ExpirationDate);

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
                    RMAInfo rmaInfo = _mReturn.lsRMAInformation.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
                    int DeliveredQty = 0;//rmaInfo.DeliveredQty;
                    int ExpectedQty = 0;//rmaInfo.ExpectedQty;
                    string tck = "";//rmaInfo.TCLCOD_0;

                    //Set returned details table.
                    Guid ReturnDetailsID = _mReturn.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), Convert.ToInt16(dt.Rows[i][3].ToString()));
                        }
                        dt.Clear();
                    }

                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mReturn.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    //SKU Reasons Table
                    foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                    {
                        _mReturn.SetTransaction(Guid.NewGuid(), Ritem, ReturnDetailsID);

                    }

                    wndSlipPrint slip = new wndSlipPrint();

                    Views.clGlobal.lsSlipInfo = _mReturn.GetSlipInfo(SkuNumber.Text, _mReturn.GetENACodeByItem(SkuNumber.Text), _mReturn.GetSageReasonBySKUSR(lblRMANumber.Content.ToString(), SkuNumber.Text), ScannedDate, ExpirationDate);

                    slip.ShowDialog();

                    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                    Views.AuditType.lsaudit.Clear();
                }
            }
            wndBoxInformation wndBox = new wndBoxInformation();
            clGlobal.IsUserlogged = true;
          //  WindowThread.Stop();
            //close wait screen.
            wndBox.Show();
            this.Close();
        }

        private void ErrorMsg(string Msg, Color BgColor)
        {
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            bdrMsg.Visibility = System.Windows.Visibility.Visible;
            bdrMsg.Background = new SolidColorBrush(BgColor);
            txtError.Text = Msg;
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //create object stack panel where Button Belongs to
                StackPanel sp = (StackPanel)(sender as Control).Parent;
                StackPanel sp2 = (StackPanel)sp.Parent;
                DataGridRow row = (DataGridRow)sp2.FindParent<DataGridRow>();
                if (_mReturn.GreenRowsNumber.Contains(row.GetIndex()))
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

        private void btnPluse_Click(object sender, RoutedEventArgs e)
        {
            StackPanel Sp = (StackPanel)(sender as Control).Parent;
            StackPanel Sp2 = (StackPanel)Sp.Parent;
            DataGridRow row = (DataGridRow)Sp2.FindParent<DataGridRow>();
            if (_mReturn.GreenRowsNumber.Contains(row.GetIndex()))
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

        private void chkIsItemPresent_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cbk = (CheckBox)e.Source;
            DataGridRow row = (DataGridRow)cbk.FindParent<DataGridRow>();
            row.Background = new SolidColorBrush(Color.FromArgb(100, 117, 162, 97));
        }

        private void chkIsItemPresent_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cbk = (CheckBox)e.Source;
            DataGridRow row = (DataGridRow)cbk.FindParent<DataGridRow>();
            row.Background = new SolidColorBrush(Color.FromArgb(100, 195, 145, 117));
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
                }
                if (Views.clGlobal.ScenarioType=="Lowes")
                {
                    CanvasConditions.IsEnabled = false;
                }
                if (Views.clGlobal.ScenarioType == "HomeDepot" || Views.clGlobal.ScenarioType == "Others")
                {
                    CanvasConditions.IsEnabled = true;
                }
              
                Button btnRed = (Button)e.Source;
                Canvas SpButtons = (Canvas)btnRed.Parent;
                Button btnGreen = SpButtons.FindName("btnGreen") as Button;
                btnGreen.Visibility = System.Windows.Visibility.Visible;
                btnRed.Visibility = System.Windows.Visibility.Hidden;

                DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
                _mReturn.GreenRowsNumber.Add(row.GetIndex());
                bdrMsg.Visibility = System.Windows.Visibility.Hidden;

                mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
            }

           
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            if (Views.clGlobal.ScenarioType == "Lowes")
            {
                CanvasConditions.IsEnabled = false;
            }
            if (Views.clGlobal.ScenarioType == "HomeDepot" || Views.clGlobal.ScenarioType == "Others")
            {
                CanvasConditions.IsEnabled = true;
            }

            Button btnGreen = (Button)e.Source;
            Canvas SpButtons = (Canvas)btnGreen.Parent;
            Button btnRed = SpButtons.FindName("btnRed") as Button;
            btnGreen.Visibility = System.Windows.Visibility.Hidden;
            btnRed.Visibility = System.Windows.Visibility.Visible;

            DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
            _mReturn.GreenRowsNumber.Remove(row.GetIndex());
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;

            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_UnChecked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
        }

        #region CheckBox Toggel.

        private void cntItemStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            TextBlock cbk = (TextBlock)e.Source;
            Canvas cs = cbk.Parent as Canvas;
            TextBlock txtReasonGuids = cs.FindName("txtReasosnsGuid") as TextBlock;
            DataGridRow row = (DataGridRow)cbk.FindParent<DataGridRow>();

            if (_mReturn.GreenRowsNumber.Contains(row.GetIndex()))
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
                        ContentPresenter cp = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
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

        //private void cbrWrong_Checked(object sender, RoutedEventArgs e)
        //{
        //    bdrRecivedWrong.Inside();
        //}

        //private void cbrWrong_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    bdrRecivedWrong.Outside();
        //}

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
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrDuplicate.Content.ToString());
        //    bdrDuplicate.Inside();
        //}

        //private void cbrDuplicate_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrDuplicate.Content.ToString());
        //    bdrDuplicate.Outside();
        //}

        //private void cbrIncorrectOrder_Checked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrIncorrectOrder.Content.ToString());
        //    bdrIcorrectOrder.Inside();
        //}

        //private void cbrIncorrectOrder_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrIncorrectOrder.Content.ToString());
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
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrSatisfied.Content.ToString());
        //    bdrSatisfied.Inside();
        //}

        //private void cbrSatisfied_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrSatisfied.Content.ToString());
        //    bdrSatisfied.Outside();
        //}

        #endregion

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
            if (e.Key == Key.Enter)
            {
                if (txtItemReason.Text != "Please type other Reason")
                {
                    Guid reasonID = _mReturn.SetReasons(txtItemReason.Text);
                    Guid ReasoncatID = _mReturn.SetReasonCategories(reasonID, txtSKUname.Text);
                    FilldgReasons(txtSKUname.Text.ToString());
                    txtItemReason.Text = "";
                    txtItemReason.Text = "Please type other Reason";
                }
            }
        }

        private void txtItemReason_GotFocus(object sender, RoutedEventArgs e)
        {
            txtItemReason.Text = "";
        }

        public void FilldgReasons(String SKUName)
        {
            dgReasons.ItemsSource = _mReturn.GetReasons(SKUName);
        }

        private void tbrgzdetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.WindowClosed.ToString(), DateTime.UtcNow.ToString(), "RMA Details Window");
            mRMAAudit.saveaudit(Views.AuditType.lsaudit);
            Views.AuditType.lsaudit.Clear();
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            wndBoxInformation boxinfo = new wndBoxInformation();
            clGlobal.IsUserlogged = true;
            boxinfo.Show();
            this.Close();
        }

        private void ContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
           // ChangeColor(cbrDamaged, txtitemdamage, cnvDamage); 
        }

        private void ChangeColor(CheckBox Chk, TextBlock txt,Canvas can)
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

        //private void ContentControl_MouseDown_1(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrDuplicate,txtDuplicate,cnvDuplicate);
        //}

        //private void ContentControl_MouseDown_2(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrWrong, txtreceicewrongitem, cnvRecieve);
        //}

        //private void ContentControl_MouseDown_3(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrDisplayedDiff, txtDisplayedOff,cnvDisplayedOff);
        //}

        //private void ContentControl_MouseDown_4(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrIncorrectOrder, txtinccorectorder,cnvInccorectorder);
        //}

        //private void ContentControl_MouseDown_5(object sender, MouseButtonEventArgs e)
        //{
        //    ChangeColor(cbrSatisfied, txtSatisfied,cnvSatisfied);
        //}


        protected void SetGridChack(DataGrid Grid)
        {
            try
            {
               // SetReasons(_mUpdate._ReturnTbl.ReturnReason);
                foreach (DataGridRow row in GetDataGridRows(Grid))
                {


                    for (int i = 0; i < _mUpdate._lsReturnDetails.Count(); i++)
                    {
                        // item SKUNumber
                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        //CheckBOx item Peresent
                        ContentPresenter CntPersenter = dgPackageInfo.Columns[0].GetCellContent(row) as ContentPresenter;
                        DataTemplate DataTemp = CntPersenter.ContentTemplate;
                        Button btnGreen = (Button)DataTemp.FindName("btnGreen", CntPersenter);
                        Button btnRed = (Button)DataTemp.FindName("btnRed", CntPersenter);

                        if (_mUpdate._lsReturnDetails[i].SKUNumber == SkuNumber.Text && btnGreen.Visibility == System.Windows.Visibility.Hidden )
                        {
                            _mReturn.GreenRowsNumber.Add(row.GetIndex());
                            btnGreen.Visibility = System.Windows.Visibility.Visible;
                            btnRed.Visibility = System.Windows.Visibility.Hidden;
                            //item Returned Quantity.
                            ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                            DataTemplate DtQty = CntQuantity.ContentTemplate;
                            TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);
                            txtRetutn.Text = _mUpdate._lsReturnDetails[i].ReturnQty.ToString();


                            //item Status.k
                            ContentPresenter CntStatus = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                            DataTemplate DtStatus = CntStatus.ContentTemplate;
                            TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;
                            TextBlock txtCheckedCount = DtStatus.FindName("txtCheckedCount", CntStatus) as TextBlock;
                            
                                txtRGuid.Text = GetReasonFronList(_mUpdate._lsReturnDetails[i].ReturnDetailID);

                              txtCheckedCount.Text=  ((txtRGuid.Text.ToString().Split(new char[] { '#' }).Count()) -1).ToString() + " Reasons";

                              //Images Stack Panel.
                              ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                              DataTemplate DtImages = CntImag.ContentTemplate;
                              StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                              foreach (var Imgitem in _mUpdate._lsImages)
                              {
                                  if (Imgitem.ReturnDetailID == _mUpdate._lsReturnDetails[i].ReturnDetailID)
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

        public String GetReasonFronList(Guid ReturDetailsID)
        {
            String IDs = "";
            foreach (var item in _mUpdate._lsReasons)
            {
                if (item.ReturnDetailID == ReturDetailsID)
                {
                    IDs = IDs + "#"+ item.ReasonID;
                }
            }
            return IDs;
        }

        //public void SetReasons(String Resns)
        //{
        //    String[] Rlin = Resns.Split(new char[]{'.'});

        //    foreach (var ritem in Rlin)
        //    {
        //        if (cbrDamaged.Content.ToString().Contains(ritem))
        //        {
        //            cbrDamaged.IsChecked = false;
        //            ChangeColor(cbrDamaged, txtitemdamage, cnvDamage);
        //        }
        //        else if (cbrDisplayedDiff.Content.ToString().Contains(ritem))
        //        {
        //            cbrDisplayedDiff.IsChecked = false;
        //            ChangeColor(cbrDisplayedDiff, txtDisplayedOff, cnvDisplayedOff);
        //        }
        //        else if (cbrDuplicate.Content.ToString().Contains(ritem))
        //        {
        //            cbrDuplicate.IsChecked = false;
        //            ChangeColor(cbrDuplicate, txtDuplicate, cnvDuplicate);
        //        }
        //        else if (cbrIncorrectOrder.Content.ToString().Contains(ritem))
        //        {
        //            cbrIncorrectOrder.IsChecked = false;
        //            ChangeColor(cbrIncorrectOrder, txtinccorectorder, cnvInccorectorder);
        //        }
        //        else if (cbrSatisfied.Content.ToString().Contains(ritem))
        //        {
        //            cbrSatisfied.IsChecked = false;
        //            ChangeColor(cbrSatisfied, txtSatisfied, cnvSatisfied);
        //        }
        //        else if (cbrWrong.Content.ToString().Contains(ritem))
        //        {
        //            cbrWrong.IsChecked = false;
        //            ChangeColor(cbrWrong, txtreceicewrongitem, cnvRecieve);
        //        }
        //        else
        //        {
        //            cmbOtherReason.SelectedIndex = cmbOtherReason.Items.IndexOf(new object[] { ritem.ToString() });
        //            txtOtherReason.Text = ritem.ToString();
        //        }

        //    }
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _showBarcode();
            txtbarcode.Focus();
        }
        int count = 0;

       
      

        private void txtbarcode_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Boolean flag = false;
                //Boolean RMACheck = false;


                if (Views.clGlobal.ScenarioType=="Lowes")
                {
                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {

                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        string sku = _mReturn.GetENACodeByItem(SkuNumber.Text);
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


                        ls1.SKUNumber = _mReturn.GetSKUNameByItem(txtbarcode.Text.Remove(0, 1));

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
                        string sku = _mReturn.GetENACodeByItem(SkuNumber.Text);
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
                        string sku = _mReturn.GetENACodeByItem(SkuNumber.Text);
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
                            ls1.SKUNumber = _mReturn.GetSKUNameByItem(txtbarcode.Text.Remove(0, 1));
                       
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
        Boolean ISinstalled=true;
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
        private void btnStatusNo_Checked(object sender, RoutedEventArgs e)
        {
          //  ErrorMsg("This Item able to Refund.", Color.FromRgb(185, 84, 0));
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
        Boolean IsManufacture = true;
        private void btnManufacturerYes_Checked(object sender, RoutedEventArgs e)
        {
             // ErrorMsg("This Item able to Refund.", Color.FromRgb(185, 84, 0));
            //btnAdd.IsEnabled = true;
           
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

            if (btnManufacturerNo.IsChecked==true && btntransiteNo.IsChecked==true)
            {
                Views.clGlobal.SKU_Staus = "Deny";
            }

            //ErrorMsg("This Item is Rejected.", Color.FromRgb(185, 84, 0));
           // btnAdd.IsEnabled = true;
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
                 _lsstatusandpoints.Status = Views.clGlobal.SKU_Staus;
                 _lsstatusandpoints.Points =Convert.ToInt16(lblpoints.Content);//Views.clGlobal.TotalPoints;
                 listofstatus.Add(_lsstatusandpoints);

                 lblpoints.Content = "";

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
                    dr1["Points"] =100;
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
                _lsstatusandpoints.Status = Views.clGlobal.SKU_Staus;
                _lsstatusandpoints.Points = Convert.ToInt16(lblpoints.Content);//Views.clGlobal.TotalPoints;
                listofstatus.Add(_lsstatusandpoints);

                lblpoints.Content = "";




                int ro = dt.Rows.Count;
                UncheckAllButtons();
                ErrorMsg("Please Select Next Item or Go ahead.", Color.FromRgb(185, 84, 0));
            }



            //if (txtError.Text=="This Item is Rejected.")
            //{
            //    foreach (DataGridRow item in GetDataGridRows(dgPackageInfo))
            //    {
            //        ContentPresenter butoninfo = dgPackageInfo.Columns[0].GetCellContent(item) as ContentPresenter;
            //        DataTemplate DtQty = butoninfo.ContentTemplate;
            //        Button txtRetutn = (Button)DtQty.FindName("btnGreen", butoninfo);
            //        if (txtRetutn.Visibility==Visibility.Visible)
            //        {
            //            item.Background = Brushes.Red;
            //        }
            //    }
            //}
            //else if (txtError.Text == "This Item able to Refund.")
            //{
            //    foreach (DataGridRow item in GetDataGridRows(dgPackageInfo))
            //    {
            //        ContentPresenter butoninfo = dgPackageInfo.Columns[0].GetCellContent(item) as ContentPresenter;
            //        DataTemplate DtQty = butoninfo.ContentTemplate;
            //        Button txtRetutn = (Button)DtQty.FindName("btnGreen", butoninfo);
            //        if (txtRetutn.Visibility == Visibility.Visible)
            //        {
            //            item.Background = Brushes.Green;
            //        }
            //    }
            //}
            //ErrorMsg("Please Select Next Item or Go ahead.", Color.FromRgb(185, 84, 0));
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



        int points=0;
        Boolean itemnew=true;
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
    }
}

