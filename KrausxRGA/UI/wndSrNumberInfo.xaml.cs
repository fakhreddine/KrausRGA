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
using System.Windows.Threading;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;


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


        protected DBLogics.cmdReasons cRtnreasons = new DBLogics.cmdReasons();
        //Print slip class list.
        List<cSlipInfo> _lsSlpiInfo = new List<cSlipInfo>();

        //RMA information from sage list.
        List<RMAInfo> _lsRMAInfo = new List<RMAInfo>();

        //Update mode opened saves the details of RMA.
        mUpdateModeRMA _mUpdate;

        Return returnforReopen = new Return();

        //Stack Panel in row assigned to this and used in Images captured add.
        StackPanel spRowImages;

        //Scroll Viewer from selected Row;
        ScrollViewer SvImagesScroll;

        //Dispacher that works when the RMA number opend in Upadate mode.
        DispatcherTimer dtLoadUpdate;

        DispatcherTimer dtLoadUpdate1;

        DispatcherTimer dtLoadnormal;

        BackgroundWorker Worker = new BackgroundWorker();

        List<SkuAndIsScanned> lsskuIsScanned = new List<SkuAndIsScanned>();

        List<SkuAndIsScanned> lsIsManually = new List<SkuAndIsScanned>();

        List<SkuReasonIDSequence> _lsReasonSKU = new List<SkuReasonIDSequence>();

        Boolean check = true;

        List<StatusAndPoints> listofstatus = new List<StatusAndPoints>();

        //recoded saving thread.
        public static Thread thSaving;

        DataTable dt = new DataTable();

        DataTable dtimages = new DataTable();

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

            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Reason", typeof(string));
            dt.Columns.Add("Reason_Value", typeof(string));
            dt.Columns.Add("Points", typeof(int));
            dt.Columns.Add("ItemQuantity", typeof(string));
            //dt.Columns.Add("", typeof(string));

            //DataColumn column = new DataColumn("MyImage"); //Create the column.
            //column.DataType = System.Type.GetType("System.Byte[]"); //Type byte[] to store image bytes.
            //column.AllowDBNull = true;
            //column.Caption = "My Image";

            //dtimages.Columns.Add(column);
            dtimages.Columns.Add("Images", typeof(byte[]));
            dtimages.Columns.Add("SKUName", typeof(string));
            dtimages.Columns.Add("SKUSequence", typeof(int));


            //fill OtherReason ComboBox
            List<Reason> lsReturn = _mReturn.GetReasons();

            //add reason select to the Combobox other reason.
            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";
            lsReturn.Insert(0, re);
            cmbSkuReasons.ItemsSource = lsReturn;
            // cmbSkuReasons.DisplayMemberPath = lsReturn[0].Reason1;
            //cmbSkuReasons.SelectedValuePath = lsReturn[0].ReasonID;


            cmbRMAStatus.SelectedIndex = 0;
            cmbRMADecision.SelectedIndex = 0;

            if (Views.clGlobal.mReturn.IsAlreadySaved)
            {
                _mUpdate = new mUpdateModeRMA(Views.clGlobal.mReturn.lsRMAInformation[0].RMANumber);

                lblRMANumber.Content = _mUpdate._ReturnTbl.RMANumber;
                tbCustomerName.Text = _mUpdate._ReturnTbl.CustomerName1;
                lblRMAReqDate.Content = _mUpdate._ReturnTbl.ReturnDate.ToString("MMM dd, yyyy");
                lblVendorNumber.Content = _mUpdate._ReturnTbl.VendorNumber.ToString();
                lblVendorName.Content = _mUpdate._ReturnTbl.VendoeName;
                lblPoNumber.Text = _mUpdate._ReturnTbl.PONumber.ToString();
                lblCustomerAddress.Text = _mUpdate._ReturnTbl.Address1;
                lblCustCity.Content = _mUpdate._ReturnTbl.City;
                lblState.Content = _mUpdate._ReturnTbl.State;
                lblZipCode.Content = _mUpdate._ReturnTbl.ZipCode;
                lblCountry.Content = _mUpdate._ReturnTbl.Country;

                txtcalltag.Text = _mUpdate._ReturnTbl.CallTag;

                cmbRMAStatus.SelectedIndex = Convert.ToInt16(_mUpdate._ReturnTbl.RMAStatus);
                cmbRMADecision.SelectedIndex = Convert.ToInt16(_mUpdate._ReturnTbl.Decision);

                brdPrint.Visibility = Visibility.Visible;

                lblExpirationDate.Content = _mUpdate._ReturnTbl.ExpirationDate; //TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(60), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).ToString("MMM dd, yyyy");

                this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _mUpdate._lsReturnDetails.OrderBy(x => x.SKU_Sequence); }));

                if (_mUpdate._ReturnTbl.ProgressFlag == 0)
                {
                    chkInProgress.IsChecked = false;
                }

                chkInProgress.IsEnabled = true;


                for (int i = 0; i < _mUpdate._lsskuandpoints1.Count; i++)
                {
                    DataRow dr0 = dt.NewRow();
                    dr0["SKU"] = _mUpdate._lsskuandpoints1[i].SKU;
                    dr0["Reason"] = _mUpdate._lsskuandpoints1[i].Reason;
                    dr0["Reason_Value"] = _mUpdate._lsskuandpoints1[i].Reason_Value;
                    dr0["Points"] = _mUpdate._lsskuandpoints1[i].Points;
                    dr0["ItemQuantity"] = _mUpdate._lsskuandpoints1[i].SkuSequence;
                    dt.Rows.Add(dr0);
                }
                for (int i = 0; i < _mUpdate._lsReturnDetails.Count; i++)
                {
                    StatusAndPoints _lsstatusandpoints = new StatusAndPoints();
                    if (_mUpdate._lsReturnDetails[i].SKU_Status != "")
                    {
                        _lsstatusandpoints.SKUName = _mUpdate._lsReturnDetails[i].SKUNumber;
                        _lsstatusandpoints.Status = _mUpdate._lsReturnDetails[i].SKU_Status;
                        _lsstatusandpoints.Points = _mUpdate._lsReturnDetails[i].SKU_Reason_Total_Points;
                        _lsstatusandpoints.IsMannually = _mUpdate._lsReturnDetails[i].IsManuallyAdded;
                        _lsstatusandpoints.IsScanned = _mUpdate._lsReturnDetails[i].IsSkuScanned;
                        _lsstatusandpoints.NewItemQuantity = _mUpdate._lsReturnDetails[i].SKU_Sequence;
                        _lsstatusandpoints.skusequence = _mUpdate._lsReturnDetails[i].SKU_Qty_Seq;
                        listofstatus.Add(_lsstatusandpoints);
                    }

                }
                if (_mUpdate._ReturnTbl.VendorNumber.ToString() == "GENC0001" || _mUpdate._ReturnTbl.VendorNumber.ToString() == "DOMC0404" || _mUpdate._ReturnTbl.VendorNumber.ToString() == "INTC0017" || _mUpdate._ReturnTbl.VendorNumber.ToString() == "DOMC0551" || _mUpdate._ReturnTbl.VendorNumber.ToString() == "DOMC0795")
                {
                    Views.clGlobal.ScenarioType = "HomeDepot";
                    ErrorMsg("Please Check this RMA is Wrong or Not By Scanning the Barcode.", Color.FromRgb(185, 84, 0));

                    DateTime DeliveryDate = _mUpdate._ReturnTbl.DeliveryDate;//_lsRMAInfo[0].DeliveryDate;
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
                        // Views.clGlobal.Warranty = "N/A";

                        MessageBox.Show("This Return is NOT in Warranty.");

                        // btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });
                        txtbarcode.Text = "";
                        txtbarcode.Focus();
                    }

                }

                else if (_mUpdate._ReturnTbl.VendorNumber.ToString() == "DOMC0143" || _mUpdate._ReturnTbl.VendorNumber.ToString() == "DOMC0432")
                {
                    Views.clGlobal.ScenarioType = "Lowes";
                    ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                }
                else
                {
                    Views.clGlobal.ScenarioType = "Others";

                    DateTime DeliveryDate = _mUpdate._ReturnTbl.DeliveryDate;
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


                        MessageBox.Show("This Return is NOT in Warranty.");

                        //   btnHomeDone_Click(btnHomeDone, new RoutedEventArgs { });

                        txtbarcode.Text = "";
                        txtbarcode.Focus();
                    }
                }

                lblExpirationDate.Content = _mUpdate._ReturnTbl.ExpirationDate.ToString("MMM dd yyyy");

                //Initialize the Dispacher that shows all values from the Update model.
                dtLoadUpdate = new DispatcherTimer();
                dtLoadUpdate.Interval = new TimeSpan(0, 0, 0, 0, 100);
                dtLoadUpdate.Tick += dtLoadUpdate_Tick;
                //start the dispacher.
                dtLoadUpdate.Start();
            }
            else
            {
                //RMA information assigned from the Model of Return.
                _lsRMAInfo = _mReturn.lsRMAInformation;

                #region Display all to the window..

                lblRMANumber.Content = _lsRMAInfo[0].RMANumber;
                tbCustomerName.Text = _lsRMAInfo[0].CustomerName1;
                lblRMAReqDate.Content = _lsRMAInfo[0].ReturnDate.ToString("MMM dd, yyyy");
                lblVendorNumber.Content = _lsRMAInfo[0].VendorNumber.ToString();
                lblVendorName.Content = _lsRMAInfo[0].VendorName;
                lblPoNumber.Text = _lsRMAInfo[0].PONumber.ToString();
                lblCustomerAddress.Text = _lsRMAInfo[0].Address1;
                lblCustCity.Content = _lsRMAInfo[0].City;
                lblState.Content = _lsRMAInfo[0].State;
                lblZipCode.Content = _lsRMAInfo[0].ZipCode;
                lblCountry.Content = _lsRMAInfo[0].Country;
                txtcalltag.Text = _lsRMAInfo[0].CallTag;

                lblExpirationDate.Content = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(60), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).ToString("MMM dd, yyyy");
                this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo.OrderBy(x => x.SKU_Sequence); }));  //dgPackageInfo.ItemsSource = _lsRMAInfo;

                if (_lsRMAInfo[0].VendorNumber.ToString() == "GENC0001" || _lsRMAInfo[0].VendorNumber.ToString() == "DOMC0404" || _lsRMAInfo[0].VendorNumber.ToString() == "INTC0017" || _lsRMAInfo[0].VendorNumber.ToString() == "DOMC0551" || _lsRMAInfo[0].VendorNumber.ToString() == "DOMC0795")
                {
                    Views.clGlobal.ScenarioType = "HomeDepot";
                    ErrorMsg("Please Check this RMA is Wrong or Not By Scanning the Barcode.", Color.FromRgb(185, 84, 0));

                }

                else if (_lsRMAInfo[0].VendorNumber.ToString() == "DOMC0143" || _lsRMAInfo[0].VendorNumber.ToString() == "DOMC0432")
                {
                    Views.clGlobal.ScenarioType = "Lowes";
                    ErrorMsg("This is Lowes.", Color.FromRgb(185, 84, 0));
                    //dgPackageInfo.IsEnabled=FA;
                    //dgPackageInfo.IsEnabled = false;
                }
                else
                {
                    Views.clGlobal.ScenarioType = "Others";

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


                        MessageBox.Show("This Return is NOT in Warranty.");
                    }
                }
                txtbarcode.Text = "";
                txtbarcode.Focus();

                dtLoadnormal = new DispatcherTimer();
                dtLoadnormal.Interval = new TimeSpan(0, 0, 0, 0, 100);
                dtLoadnormal.Tick += dtLoadnormal_Tick;
                //start the dispacher.
                dtLoadnormal.Start();
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



                    //  TextBlock Quantity = dgPackageInfo.Columns[2].GetCellContent(row) as TextBlock;

                    TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row1) as TextBlock;

                    if (LineType.Text == "6")
                    {
                        ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                        DataTemplate DtQty = CntQuantity.ContentTemplate;
                        TextBlock txtproductName = (TextBlock)DtQty.FindName("tbQty", CntQuantity);
                        txtproductName.Text = "";

                        ContentPresenter _contentPar = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                        DataTemplate _dataTemplate = _contentPar.ContentTemplate;
                        Image _imgBarcode = (Image)_dataTemplate.FindName("imgBarCode", _contentPar);
                        // TextBlock txtComboNumber = (TextBlock)_dataTemplate.FindName("txtGroupID", _contentPar);
                        _imgBarcode.Visibility = Visibility.Hidden;

                        ContentPresenter CntSequence = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                        DataTemplate DtQty2 = CntSequence.ContentTemplate;
                        TextBlock txtproductName2 = (TextBlock)DtQty2.FindName("tbDQyt", CntSequence);

                       // txtproductName2.Text = "";

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
                    String UPC_Code = _mReturn.GetENACodeByItem(SkuName);//_shipment.ShipmentDetailSage.FirstOrDefault(i => i.SKU == SkuName).UPCCode;
                    if (UPC_Code == null) UPC_Code = "00000000000";

                    //clGlobal.call.SKUnameToUPCCode(SKUNo.Text.ToString());
                    ContentPresenter sp = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
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

        //public void BoldFontandHideCombp(List<ReturnDetail> lsShipment)
        //{
        //    foreach (var item in lsShipment)
        //    {
        //        if (item.LineType == 6)
        //        {
        //            foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
        //            {
        //                TextBlock txtSKUName = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
        //                if (txtSKUName.Text.ToUpper() == item.SKUNumber.ToUpper())
        //                {
        //                    TextBlock txtproductName = dgPackageInfo.Columns[2].GetCellContent(row) as TextBlock;
        //                    txtproductName.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
        //                    txtproductName.FontWeight = FontWeight.FromOpenTypeWeight(400);


        //                    TextBlock txtQuantity = dgPackageInfo.Columns[3].GetCellContent(row) as TextBlock;
        //                    txtQuantity.Foreground = new SolidColorBrush(Colors.SkyBlue);
        //                    txtQuantity.Text = "0";
        //                    txtSKUName.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
        //                    txtSKUName.FontWeight = FontWeight.FromOpenTypeWeight(400);
        //                    row.Background = new SolidColorBrush(Colors.SkyBlue);


        //                    TextBlock txtPacked = dgPackageInfo.Columns[4].GetCellContent(row) as TextBlock;
        //                    txtPacked.Foreground = new SolidColorBrush(Colors.SkyBlue);


        //                    ContentPresenter sp1 = dgPackageInfo.Columns[6].GetCellContent(row) as ContentPresenter;
        //                    DataTemplate myDataTemplate12 = sp1.ContentTemplate;
        //                    TextBox myTextBlock = (TextBox)myDataTemplate12.FindName("gtxtBox", sp1);
        //                    myTextBlock.Foreground = new SolidColorBrush(Colors.SkyBlue);
        //                    //Hode Quantity Equal Barcode


        //                    ContentPresenter _contentPar = dgPackageInfo.Columns[8].GetCellContent(row) as ContentPresenter;
        //                    DataTemplate _dataTemplate = _contentPar.ContentTemplate;
        //                    Image _imgBarcode = (Image)_dataTemplate.FindName("imgBarCode", _contentPar);
        //                    TextBlock txtComboNumber = (TextBlock)_dataTemplate.FindName("txtGroupID", _contentPar);
        //                    _imgBarcode.Visibility = Visibility.Hidden;
        //                    txtComboNumber.Text = "";



        //                    ContentPresenter sp = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
        //                    DataTemplate myDataTemplate2 = sp.ContentTemplate;
        //                    Button btn = (Button)myDataTemplate2.FindName("btnComplete", sp);
        //                    btn.Visibility = Visibility.Hidden;
        //                    row.IsEnabled = false;
        //                }
        //            }
        //        }
        //    }
        //}




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
            // SetGridChack(dgPackageInfo);
        }

        #region Data Grid Events.

        private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Images Capture By Camera Press  -  Yes\n\nBrowse From System Press - No","Confirmation", MessageBoxButton.YesNoCancel);
            //if (result == MessageBoxResult.Yes)
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
            //                   // img.MouseEnter += img_MouseEnter;

            //                    img.MouseDown += img_MouseDown;

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
            //else if (result == MessageBoxResult.No)
            //{

            //    ContentControl cnt = (ContentControl)sender;
            //    DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();

            //    StackPanel spRowImages = cnt.FindName("spProductImages") as StackPanel;

            //    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            //    // Set filter for file extension and default file extension 
            //    dlg.DefaultExt = ".png";
            //    dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All files (*.*)|*.*";


            //    // Display OpenFileDialog by calling ShowDialog method 
            //    Nullable<bool> result1 = dlg.ShowDialog();


            //    // Get the selected file name and display in a TextBox 
            //    if (result1 == true)
            //    {
            //        // Open document 
            //        string filename = dlg.FileName;

            //        string originalfilename = dlg.SafeFileName.Replace("-", "");

            //        string finalfilename = originalfilename.Replace("_", "");

            //       // textBox1.Text = filename;
            //        //string path = "C:\\Images\\";

            //        BitmapSource bs = new BitmapImage(new Uri(filename));

            //        Image img = new Image();
            //        //Zoom image.
            //       // img.MouseEnter += img_MouseEnter;

            //        img.MouseDown += img_MouseDown;

            //        img.Height = 50;
            //        img.Width = 50;
            //        img.Stretch = Stretch.Fill;
            //        img.Name = finalfilename.ToString().Split(new char[] { '.' })[0];
            //        img.Source = bs;
            //        img.Margin = new Thickness(0.5);

            //        //Images added to the Row.
            //        _addToStackPanel(spRowImages, img);

            //    }
            //}
            //else
            //{
            //    // Cancel code here
            //} 





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

        //void img_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    //Image img = (Image)sender;

        //    clGlobal.Zoomimages = (Image)sender;

        //    wndZoomImageWindow zoom = new wndZoomImageWindow();
        //    zoom.ShowDialog();

        //    // bdrZoomImage.Visibility = System.Windows.Visibility.Visible;
        //    //imgZoom.Source = img.Source;
        //}



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
            cmbRMADecision.ItemsSource = _mReturn.GetRMADecision();
            cmbRMAStatus.ItemsSource = _mReturn.GetRMAStatusList();
        }

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
        Guid ReturnDetailsID;

        private void btnHomeDone_Click(object sender, RoutedEventArgs e)
        {
            //  WindowThread.start();
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

            int InProgress = 0;

            if (chkInProgress.IsChecked == true)
            {
                InProgress = 1;
            }


            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Done_Clicked.ToString(), DateTime.UtcNow.ToString(), _mReturn.EnteredNumber);

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
            Guid ReturnTblID = _mReturn.SetReturnTbl("", RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID, ScannedDate, ExpirationDate, wrongRMA, Warranty, 60, Views.clGlobal.ShipDate_ScanDate_Diff, InProgress);//ReturnReasons()

            if (Views.clGlobal.mReturn.IsAlreadySaved)
            {
                ReturnTblID = _mUpdate._ReturnTbl.ReturnID;
                //_lsreturn.Add(_mUpdate._ReturnTbl1);

                foreach (var ReturnDetailsID in _mUpdate._lsReturnDetails)
                {
                    _mReturn.DeleteReturnDetails(ReturnDetailsID.ReturnDetailID);
                }
            }
            foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
            {
                //CheckBOx item Peresent
                ContentPresenter CntPersenter = dgPackageInfo.Columns[0].GetCellContent(row) as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;
                Button btnGreen = (Button)DataTemp.FindName("btnGreen", CntPersenter);

                #region Lowes
                if (Views.clGlobal.ScenarioType == "Lowes")
                {

                    // If item present in the return 
                    // item SKUNumber


                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;


                    //item Returned Quantity.
                    ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty = CntQuantity.ContentTemplate;
                    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[6].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbDQyt", CntQuantity1);

                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);


                    TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row) as TextBlock;

                    TextBlock SalesPrice = dgPackageInfo.Columns[9].GetCellContent(row) as TextBlock;

                    TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row) as TextBlock;

                    TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row) as TextBlock;

                    TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row) as TextBlock;

                    //Returned RMA Information.
                    RMAInfo rmaInfo = _mReturn.lsRMAInformation.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
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
                            Views.clGlobal.IsScanned = 1;
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

                    if (row.Background == Brushes.SkyBlue)
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

                    Guid ReturnDetailsID = _mReturn.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, 0, Views.clGlobal.IsScanned, Views.clGlobal.IsManually, Convert.ToInt16(txtRetutn1.Text), Convert.ToInt16(txtRetutn.Text), ProductID.Text, Convert.ToDecimal(SalesPrice.Text), Convert.ToInt16(LineType.Text), Convert.ToInt16(ShipmentLines.Text), Convert.ToInt16(ReturnLines.Text));
                    Views.clGlobal.IsScanned = 0;
                    Views.clGlobal.IsManually = 0;


                    Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0, 1);

                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mReturn.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    if (LineType.Text != "6")
                    {
                        wndSlipPrint slip = new wndSlipPrint();

                        Views.clGlobal.lsSlipInfo = _mReturn.GetSlipInfo(SkuNumber.Text, _mReturn.GetENACodeByItem(SkuNumber.Text), _mReturn.GetSageReasonBySKUSR(lblRMANumber.Content.ToString(), SkuNumber.Text), ScannedDate, ExpirationDate, cmbRMAStatus.SelectedIndex.ToString(), "Refund");

                        slip.ShowDialog();
                    }



                    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                    Views.AuditType.lsaudit.Clear();
                    // }
                }
                #endregion

                #region HomeDepot
                if (Views.clGlobal.ScenarioType == "HomeDepot")
                {
                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                    //item Returned Quantity.
                    ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty = CntQuantity.ContentTemplate;
                    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[6].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbDQyt", CntQuantity1);

                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);


                    TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row) as TextBlock;

                    TextBlock SalesPrice = dgPackageInfo.Columns[9].GetCellContent(row) as TextBlock;

                    TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row) as TextBlock;

                    TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row) as TextBlock;

                    TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row) as TextBlock;

                    //Returned RMA Information.
                    RMAInfo rmaInfo = _mReturn.lsRMAInformation.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
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



                    Guid ReturnDetailsID = _mReturn.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints, Views.clGlobal.IsScanned, Views.clGlobal.IsManually, Convert.ToInt16(txtRetutn1.Text), Views.clGlobal._SKU_Qty_Seq, ProductID.Text, Convert.ToDecimal(SalesPrice.Text), Convert.ToInt16(LineType.Text), Convert.ToInt16(ShipmentLines.Text), Convert.ToInt16(ReturnLines.Text));

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
                                Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), Convert.ToInt16(dt.Rows[i][3].ToString()), Convert.ToInt16(dt.Rows[i][4].ToString()));
                                d.Delete();
                            }
                        }
                    }
                    else
                    {
                        //if (check)
                        //{
                        //    Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0, 0);
                        //}
                    }


                    if (_lsReasonSKU.Count > 0)
                    {
                        for (int i = _lsReasonSKU.Count - 1; i >= 0; i--)
                        {
                            if (_lsReasonSKU[i].SKUName == SkuNumber.Text && _lsReasonSKU[i].SKU_sequence == Convert.ToInt16(txtRetutn1.Text))
                            {
                                _mReturn.SetTransaction(Guid.NewGuid(), _lsReasonSKU[i].ReasonID, ReturnDetailsID);
                                _lsReasonSKU.RemoveAt(i);
                            }
                        }
                    }

                    //Save Images info Table.
                   




                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mReturn.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    if (LineType.Text != "6")
                    {
                        wndSlipPrint slip = new wndSlipPrint();

                        Views.clGlobal.lsSlipInfo = _mReturn.GetSlipInfo(SkuNumber.Text, _mReturn.GetENACodeByItem(SkuNumber.Text), _mReturn.GetSageReasonBySKUSR(lblRMANumber.Content.ToString(), SkuNumber.Text), ScannedDate, ExpirationDate, cmbRMAStatus.SelectedIndex.ToString(), Views.clGlobal.SKU_Staus);

                        slip.ShowDialog();
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                    }


                    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                    Views.AuditType.lsaudit.Clear();
                }
                #endregion

                #region Others
                if (Views.clGlobal.ScenarioType == "Others")
                {
                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                    //item Returned Quantity.
                    ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty = CntQuantity.ContentTemplate;
                    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[6].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbDQyt", CntQuantity1);


                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);



                    TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row) as TextBlock;

                    TextBlock SalesPrice = dgPackageInfo.Columns[9].GetCellContent(row) as TextBlock;

                    TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row) as TextBlock;

                    TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row) as TextBlock;

                    TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row) as TextBlock;

                    //Returned RMA Information.
                    RMAInfo rmaInfo = _mReturn.lsRMAInformation.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text);
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

                    for (int i = 0; i < lsIsManually.Count; i++)
                    {
                        if (lsIsManually[i].SKUName == SkuNumber.Text)
                        {
                            SKUNewName = SkuNumber.Text;
                            Views.clGlobal.IsManually = lsIsManually[i].IsScanned;
                            break;
                        }
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

                    ReturnDetailsID = _mReturn.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, "", DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints, Views.clGlobal.IsScanned, Views.clGlobal.IsManually, Convert.ToInt16(txtRetutn1.Text), Views.clGlobal._SKU_Qty_Seq, ProductID.Text, Convert.ToDecimal(SalesPrice.Text), Convert.ToInt16(LineType.Text), Convert.ToInt16(ShipmentLines.Text), Convert.ToInt16(ReturnLines.Text));

                    Views.clGlobal.IsScanned = 0;
                    Views.clGlobal.IsManually = 0;

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow d = dt.Rows[i];
                            if (d["SKU"].ToString() == SkuNumber.Text && d["ItemQuantity"].ToString() == txtRetutn1.Text)
                            {
                                Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), Convert.ToInt16(dt.Rows[i][3].ToString()), Convert.ToInt16(dt.Rows[i][4].ToString()));
                                d.Delete();
                            }
                        }
                    }
                    else
                    {
                        //if (check)
                        //{
                        //    Guid ReturnedSKUPoints = _mReturn.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, SkuNumber.Text, "N/A", "N/A", 0, 0);
                        //}
                    }

                    if (_lsReasonSKU.Count > 0)
                    {
                        for (int i = _lsReasonSKU.Count - 1; i >= 0; i--)
                        {
                            if (_lsReasonSKU[i].SKUName == SkuNumber.Text && _lsReasonSKU[i].SKU_sequence == Convert.ToInt16(txtRetutn1.Text))
                            {
                                _mReturn.SetTransaction(Guid.NewGuid(), _lsReasonSKU[i].ReasonID, ReturnDetailsID);
                                _lsReasonSKU.RemoveAt(i);
                                break;
                            }
                        }
                    }



                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + "\\" + imageCaptured.Name.ToString() + ".jpg";

                        //Set Images table from model.
                        Guid ImageID = _mReturn.SetReturnedImages(Guid.NewGuid(), ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);

                    }

                    if (LineType.Text != "6")
                    {
                        wndSlipPrint slip = new wndSlipPrint();

                        Views.clGlobal.lsSlipInfo = _mReturn.GetSlipInfo(SkuNumber.Text, _mReturn.GetENACodeByItem(SkuNumber.Text), _mReturn.GetSageReasonBySKUSR(lblRMANumber.Content.ToString(), SkuNumber.Text), ScannedDate, ExpirationDate, cmbRMAStatus.SelectedIndex.ToString(), Views.clGlobal.SKU_Staus);

                        slip.ShowDialog();
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                        Views.clGlobal.SKU_Staus = "";
                        Views.clGlobal.TotalPoints = 0;
                    }



                    mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                    Views.AuditType.lsaudit.Clear();
                }
                #endregion


            }
            Views.clGlobal.Warranty = "";
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

            //if (!(txtError.Text == "This Return is NOT in Warranty."))
            //{
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
                if (row.Background == Brushes.SkyBlue)
                {
                    btnGreen.Visibility = System.Windows.Visibility.Visible;
                    btnRed.Visibility = System.Windows.Visibility.Hidden;
                }

                //DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
                _mReturn.GreenRowsNumber.Add(row.GetIndex());
                bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                txtbarcode.Text = "";
                txtbarcode.Focus();

                mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
            }
            if (Views.clGlobal.ScenarioType == "HomeDepot")
            {

                //if (txtError.Text == "Select Item and Go ahead")
                //{
                //  CanvasConditions.IsEnabled = true;
                txtbarcode.Focus();

                btnAdd.IsEnabled = true;
                CanvasConditions.IsEnabled = false;
                Button btnRed = (Button)e.Source;
                Canvas SpButtons = (Canvas)btnRed.Parent;
                Button btnGreen = SpButtons.FindName("btnGreen") as Button;
                DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();

                ContentPresenter Cntskustatus = dgPackageInfo.Columns[7].GetCellContent(row) as ContentPresenter;
                DataTemplate Dtskustatus = Cntskustatus.ContentTemplate;
                TextBlock txtskustatus = (TextBlock)Dtskustatus.FindName("tbskustatus", Cntskustatus);

                TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;


                ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(row) as ContentPresenter;
                DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

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
                    // CanvasConditions.IsEnabled = false;
                   // string msg = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (SkuNumber.Text == dt.Rows[i][0].ToString() && txtRetutn2.Text == dt.Rows[i][4].ToString())
                        {
                           // msg = dt.Rows[i][1].ToString() + " : " + dt.Rows[i][2].ToString() + "\n" + msg;
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

                    for (int i = 0; i < dgPackageInfo.Items.Count; i++)
                    {
                        for (int j = 0; j < _mUpdate._lsReasons.Count; j++)
                        {
                            if (_mUpdate._lsReturnDetails[i].SKUNumber ==SkuNumber.Text && _mUpdate._lsReturnDetails[i].SKU_Sequence==Convert.ToInt16(txtRetutn2.Text) &&  _mUpdate._lsReturnDetails[i].ReturnDetailID == _mUpdate._lsReasons[j].ReturnDetailID)
                            {
                                System.Guid ReturnID = _mUpdate._lsReasons[j].ReturnDetailID;

                                string reas = cRtnreasons.GetReasonsByReturnDetailID(ReturnID);

                                cmbSkuReasons.Text = reas;
                            }
                        }
                    }

                    //_mUpdate._lsReasons


                   // MessageBox.Show(msg);
                }




                _mReturn.GreenRowsNumber.Add(row.GetIndex());
                bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                txtbarcode.Text = "";
                txtbarcode.Focus();

                mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
                // }
            }
            if (Views.clGlobal.ScenarioType == "Others")
            {

                // CanvasConditions.IsEnabled = true;
                txtbarcode.Focus();

                btnAdd.IsEnabled = true;
                CanvasConditions.IsEnabled = false;
                Button btnRed = (Button)e.Source;
                Canvas SpButtons = (Canvas)btnRed.Parent;
                Button btnGreen = SpButtons.FindName("btnGreen") as Button;
                DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();

                ContentPresenter Cntskustatus = dgPackageInfo.Columns[7].GetCellContent(row) as ContentPresenter;
                DataTemplate Dtskustatus = Cntskustatus.ContentTemplate;
                TextBlock txtskustatus = (TextBlock)Dtskustatus.FindName("tbskustatus", Cntskustatus);

                TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;


                ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(row) as ContentPresenter;
                DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);



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
                    //  CanvasConditions.IsEnabled = false;
                    string msg = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (SkuNumber.Text == dt.Rows[i][0].ToString() && txtRetutn2.Text == dt.Rows[i][4].ToString())
                        {
                           // msg = dt.Rows[i][1].ToString() + " : " + dt.Rows[i][2].ToString() + "\n" + msg;
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
                    for (int i = 0; i < dgPackageInfo.Items.Count; i++)
                    {
                        for (int j = 0; j < _mUpdate._lsReasons.Count; j++)
                        {
                            if (_mUpdate._lsReturnDetails[i].SKUNumber == SkuNumber.Text && _mUpdate._lsReturnDetails[i].SKU_Sequence == Convert.ToInt16(txtRetutn2.Text) && _mUpdate._lsReturnDetails[i].ReturnDetailID == _mUpdate._lsReasons[j].ReturnDetailID)
                            {
                                System.Guid ReturnID = _mUpdate._lsReasons[j].ReturnDetailID;

                                string reas = cRtnreasons.GetReasonsByReturnDetailID(ReturnID);

                                cmbSkuReasons.Text = reas;
                            }
                        }
                    }
                   // MessageBox.Show(msg);
                }




                _mReturn.GreenRowsNumber.Add(row.GetIndex());
                bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                txtbarcode.Text = "";
                txtbarcode.Focus();

                mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_Checked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");

            }

            // }


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
                CanvasConditions.IsEnabled = false;
                txtbarcode.Focus();
            }

            btnAdd.IsEnabled = false;
            Button btnGreen = (Button)e.Source;
            Canvas SpButtons = (Canvas)btnGreen.Parent;
            Button btnRed = SpButtons.FindName("btnRed") as Button;
            btnGreen.Visibility = System.Windows.Visibility.Hidden;
            btnRed.Visibility = System.Windows.Visibility.Visible;

            DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
            _mReturn.GreenRowsNumber.Remove(row.GetIndex());
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            txtbarcode.Focus();

            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ProductPersentInRMA_UnChecked.ToString(), DateTime.UtcNow.ToString(), "RowIndex_( " + row.GetIndex().ToString() + " )");
        }

        #region CheckBox Toggel.

        private void cntItemStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

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

        private void ContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // ChangeColor(cbrDamaged, txtitemdamage, cnvDamage); 
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

                        if (_mUpdate._lsReturnDetails[i].SKUNumber == SkuNumber.Text && btnGreen.Visibility == System.Windows.Visibility.Hidden)
                        {
                            _mReturn.GreenRowsNumber.Add(row.GetIndex());
                            //btnGreen.Visibility = System.Windows.Visibility.Visible;
                            btnRed.Visibility = System.Windows.Visibility.Visible;

                            if (_mUpdate._lsReturnDetails[i].SKU_Qty_Seq == 1)
                            {
                                row.Background = Brushes.SkyBlue;// row.IsEnabled = false; //row.Background = Brushes.SkyBlue; // btnGreen.Visibility = System.Windows.Visibility.Visible;//  
                            }
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
                                        // img.MouseEnter += img_MouseEnter;

                                        img.MouseDown += img_MouseDown;



                                        img.Height = 50;
                                        img.Width = 50;
                                        img.Stretch = Stretch.Fill;

                                        if (Imgitem.SKUImagePath.Contains("SR"))
                                        {
                                            String Name = Imgitem.SKUImagePath.Remove(0, Imgitem.SKUImagePath.IndexOf("SR"));
                                            img.Name = Name.ToString().Split(new char[] { '.' })[0];
                                        }
                                        else
                                        {
                                            //string original=Imgitem.SKUImagePath
                                            string path = Imgitem.SKUImagePath; //"C:\\Program Files\\hello.txt";
                                            string[] pathArr = path.Split('\\');
                                            string[] fileArr = pathArr.Last().Split('.');
                                            string fileName = fileArr.Last().ToString();
                                            img.Name = fileName;
                                        }


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

        void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            clGlobal.Zoomimages = (Image)sender;

            wndZoomImageWindow zoom = new wndZoomImageWindow();
            zoom.ShowDialog();
            //throw new NotImplementedException();
        }

        public String GetReasonFronList(Guid ReturDetailsID)
        {
            String IDs = "";
            foreach (var item in _mUpdate._lsReasons)
            {
                if (item.ReturnDetailID == ReturDetailsID)
                {
                    IDs = IDs + "#" + item.ReasonID;
                }
            }
            return IDs;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _showBarcode();
            txtbarcode.Focus();
        }
        int count = 0;



        int max, shipmax, returnmax;
        private void txtbarcode_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtbarcode.Text.Trim() != "")
            {
                Boolean flag = false;
                //Boolean RMACheck = false;

                #region Lowes
                if (Views.clGlobal.ScenarioType == "Lowes")
                {
                    #region part of PO
                    Boolean itemcheck = true;
                    for (int i = 0; i < _mReturn.lsRMAInformation.Count; i++)
                    {
                        if (_mReturn.lsRMAInformation[i].SKUNumber == _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString()))
                        {
                            itemcheck = false;// MessageBox.Show("This Scanned item is not part of PO.");

                        }
                    }

                    if (itemcheck)
                    {
                        MessageBox.Show("This Scanned item is not part of PO.");
                    }
                    #endregion


                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {
                        SkuAndIsScanned _lsskuandscanned = new SkuAndIsScanned();
                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        string Str = txtbarcode.Text.TrimStart('0').ToString();
                        string sku = _mReturn.GetENACodeByItem(SkuNumber.Text);

                        ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                        DataTemplate DtQty = CntQuantity.ContentTemplate;
                        TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                        if (sku == Str)
                        {
                            _lsskuandscanned.SKUName = SkuNumber.Text;
                            _lsskuandscanned.IsScanned = 1;
                            lsskuIsScanned.Add(_lsskuandscanned);
                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            //row.Background = Brushes.SkyBlue;

                            #region If Zero
                            if (sku == Str && txtRetutn.Text == "0")
                            {
                                row.Background = Brushes.SkyBlue;
                                txtRetutn.Text = "1";
                                flag = true;
                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                                break;
                            }
                            #endregion

                            #region IF One
                            else if (sku == Str && txtRetutn.Text == "1" && row.Background != Brushes.SkyBlue)
                            {
                                List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                                foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                                {
                                    //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                                    RMAInfo ls = new RMAInfo();
                                    TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                                    TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row1) as TextBlock;

                                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                                    if (txtRetutn1.Text == "")
                                    {
                                        txtRetutn1.Text = "0";
                                    }

                                    ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                                    TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                                    if (txtRetutn2.Text == "")
                                    {
                                        txtRetutn2.Text = "0";
                                    }

                                    TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                                    TextBlock SalePrices = dgPackageInfo.Columns[9].GetCellContent(row1) as TextBlock;

                                    TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row1) as TextBlock;

                                    TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row1) as TextBlock;

                                    ls.SKUNumber = SkuNumber1.Text;
                                    ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                                    ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                                    ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                                    ls.ProductID = ProductID.Text;
                                    ls.LineType = Convert.ToInt16(LineType.Text);
                                    ls.ShipmentLines = Convert.ToInt16(ShipmentLines.Text);
                                    ls.ReturnLines = Convert.ToInt16(ReturnLines.Text);

                                    if (sku == _mReturn.GetENACodeByItem(SkuNumber1.Text))
                                    {
                                        if (max < Convert.ToInt16(txtRetutn2.Text))
                                        {
                                            max = Convert.ToInt16(txtRetutn2.Text);
                                        }
                                        if (shipmax == Convert.ToInt16(ShipmentLines.Text))
                                        {
                                            shipmax = Convert.ToInt16(ShipmentLines.Text);
                                        }

                                        if (returnmax == Convert.ToInt16(ReturnLines.Text))
                                        {
                                            returnmax = Convert.ToInt16(ReturnLines.Text);
                                        }
                                    }
                                    _lsRMAInfo1.Add(ls);
                                }

                                RMAInfo ls1 = new RMAInfo();
                                SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                                ls1.SKUNumber = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                                ls1.ProductID = _mReturn.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                                ls1.SalesPrice = 0;
                                _lsIsmanually1.IsScanned = 1;
                                _lsIsmanually1.SKUName = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                                lsIsManually.Add(_lsIsmanually1);


                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                                ls1.SKU_Qty_Seq = 1;
                                ls1.SKU_Sequence = max + 1000;
                                ls1.ReturnLines = returnmax + 1000;
                                ls1.ShipmentLines = shipmax + 1000;
                                ls1.LineType = 1;
                                max = 0;
                                returnmax = 0;
                                shipmax = 0;

                                _lsRMAInfo1.Add(ls1);
                                flag = true;
                                this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                                dtLoadUpdate1 = new DispatcherTimer();
                                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                                //start the dispacher.
                                dtLoadUpdate1.Start();
                            }
                            #endregion






                            // txtbarcode.Text = "";
                            // txtbarcode.Focus();

                            count++;
                            //break;
                        }
                    }

                    #region Flag check
                    if (!flag)
                    {
                        List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                        foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                        {
                            //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                            RMAInfo ls = new RMAInfo();
                            TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;
                            string sku = _mReturn.GetENACodeByItem(SkuNumber1.Text);
                            TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row1) as TextBlock;

                            ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                            TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                            if (txtRetutn1.Text == "")
                            {
                                txtRetutn1.Text = "0";
                            }

                            ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                            TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                            if (txtRetutn2.Text == "")
                            {
                                txtRetutn2.Text = "0";
                            }

                            TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                            TextBlock SalePrices = dgPackageInfo.Columns[9].GetCellContent(row1) as TextBlock;


                            TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row1) as TextBlock;

                            TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row1) as TextBlock;

                            ls.SKUNumber = SkuNumber1.Text;
                            ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                            ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                            ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                            ls.ProductID = ProductID.Text;
                            ls.LineType = Convert.ToInt16(LineType.Text);

                            if (sku == _mReturn.GetENACodeByItem(SkuNumber1.Text))
                            {
                                if (max < Convert.ToInt16(txtRetutn2.Text))
                                {
                                    max = Convert.ToInt16(txtRetutn2.Text);
                                }
                                if (shipmax == Convert.ToInt16(ShipmentLines.Text))
                                {
                                    shipmax = Convert.ToInt16(ShipmentLines.Text);
                                }

                                if (returnmax == Convert.ToInt16(ReturnLines.Text))
                                {
                                    returnmax = Convert.ToInt16(ReturnLines.Text);
                                }
                            }



                            _lsRMAInfo1.Add(ls);
                        }

                        RMAInfo ls1 = new RMAInfo();
                        SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                        ls1.SKUNumber = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                        ls1.ProductID = _mReturn.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                        ls1.SalesPrice = 0;

                        _lsIsmanually1.IsScanned = 1;
                        _lsIsmanually1.SKUName = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                        lsIsManually.Add(_lsIsmanually1);


                        txtbarcode.Text = "";
                        txtbarcode.Focus();

                        ls1.SKU_Qty_Seq = 1;
                        ls1.SKU_Sequence = max + 1000;
                        ls1.ReturnLines = returnmax + 1000;
                        ls1.ShipmentLines = shipmax + 1000;
                        ls1.LineType = 1;
                        max = 0;
                        returnmax = 0;
                        shipmax = 0;

                        _lsRMAInfo1.Add(ls1);

                        this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                        dtLoadUpdate1 = new DispatcherTimer();
                        dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                        dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                        //start the dispacher.
                        dtLoadUpdate1.Start();

                    }
                    #endregion



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


                }
                #endregion

                #region HomeDepot
                if (Views.clGlobal.ScenarioType == "HomeDepot")
                {
                    #region part of PO
                    Boolean itemcheck = true;
                    for (int i = 0; i < _mReturn.lsRMAInformation.Count; i++)
                    {
                        if (_mReturn.lsRMAInformation[i].SKUNumber == _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString()))
                        {
                            itemcheck = false;// MessageBox.Show("This Scanned item is not part of PO.");

                        }
                    }

                    if (itemcheck)
                    {
                        MessageBox.Show("This Scanned item is not part of PO.");
                    }
                    #endregion



                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {
                        SkuAndIsScanned _lsskuandscanned = new SkuAndIsScanned();
                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                        TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row) as TextBlock;

                        TextBlock SalesPrices = dgPackageInfo.Columns[9].GetCellContent(row) as TextBlock;

                        ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                        DataTemplate DtQty = CntQuantity.ContentTemplate;
                        TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                        string Str = txtbarcode.Text.TrimStart('0').ToString();
                        string sku = _mReturn.GetENACodeByItem(SkuNumber.Text);
                        if (sku == Str)
                        {
                            _lsskuandscanned.SKUName = SkuNumber.Text;
                            _lsskuandscanned.IsScanned = 1;

                            lsskuIsScanned.Add(_lsskuandscanned);
                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            // row.Background = Brushes.SkyBlue;

                            #region For Zero
                            if (sku == Str && txtRetutn.Text == "0")
                            {
                                row.Background = Brushes.SkyBlue;
                                txtRetutn.Text = "1";
                                flag = true;
                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                                break;
                            }
                            #endregion

                            #region For One
                            else if (sku == Str && txtRetutn.Text == "1" && row.Background != Brushes.SkyBlue)
                            {
                                List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                                foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                                {
                                    //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                                    RMAInfo ls = new RMAInfo();
                                    TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                                    TextBlock ProductID1 = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                                    TextBlock SalePrices = dgPackageInfo.Columns[9].GetCellContent(row1) as TextBlock;

                                    TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row1) as TextBlock;

                                    //ContentPresenter Cntimages = dgPackageInfo.Columns[3].GetCellContent(row1) as ContentPresenter;
                                    //DataTemplate Dtimages = Cntimages.ContentTemplate;
                                    ////ContentControl cntimages=Dtimages.co

                                    //StackPanel stimages = (StackPanel)Dtimages.FindName("spProductImages", Cntimages);

                                    TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row1) as TextBlock;

                                    TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row1) as TextBlock;


                                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                                    if (txtRetutn1.Text == "")
                                    {
                                        txtRetutn1.Text = "0";
                                    }


                                    ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                                    TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                                    if (txtRetutn2.Text == "")
                                    {
                                        txtRetutn2.Text = "0";
                                    }


                                    ls.SKUNumber = SkuNumber1.Text;
                                    ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                                    ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                                    ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                                    ls.ProductID = ProductID1.Text;
                                    ls.LineType = Convert.ToInt16(LineType.Text);
                                    //ls.sta

                                    if (sku == _mReturn.GetENACodeByItem(SkuNumber1.Text))
                                    {
                                        if (max < Convert.ToInt16(txtRetutn2.Text))
                                        {
                                            max = Convert.ToInt16(txtRetutn2.Text);
                                        }
                                        if (shipmax == Convert.ToInt16(ShipmentLines.Text))
                                        {
                                            shipmax = Convert.ToInt16(ShipmentLines.Text);
                                        }

                                        if (returnmax == Convert.ToInt16(ReturnLines.Text))
                                        {
                                            returnmax = Convert.ToInt16(ReturnLines.Text);
                                        }
                                    }
                                    _lsRMAInfo1.Add(ls);
                                }

                                RMAInfo ls1 = new RMAInfo();
                                SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                                ls1.SKUNumber = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                                ls1.ProductID = _mReturn.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];
                                ls1.SalesPrice = 0;
                                _lsIsmanually1.IsScanned = 1;
                                _lsIsmanually1.SKUName = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                                lsIsManually.Add(_lsIsmanually1);


                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                                ls1.SKU_Qty_Seq = 1;
                                ls1.SKU_Sequence = max + 1000;
                                ls1.ReturnLines = returnmax + 1000;
                                ls1.ShipmentLines = shipmax + 1000;
                                ls1.LineType = 1;
                                max = 0;

                                _lsRMAInfo1.Add(ls1);

                                flag = true;
                                this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                                dtLoadUpdate1 = new DispatcherTimer();
                                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                                //start the dispacher.
                                dtLoadUpdate1.Start();


                            }

                            Views.clGlobal.IsScanned = 1;
                            //txtbarcode.Text = "";
                            //txtbarcode.Focus();

                            count++;
                            //break;
                        }
                            #endregion


                    }

                    #region Flag Check
                    if (!flag)
                    {
                        Views.clGlobal.WrongRMAFlag = "1";
                        ErrorMsg("This Scanned item is not part of PO.", Color.FromRgb(185, 84, 0));
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
                            string sku = _mReturn.GetENACodeByItem(SkuNumber1.Text);
                            TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row1) as TextBlock;

                            ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                            TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                            ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtImages = CntImag.ContentTemplate;
                            StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                            if (txtRetutn1.Text == "")
                            {
                                txtRetutn1.Text = "0";
                            }

                            ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                            TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                            if (txtRetutn2.Text == "")
                            {
                                txtRetutn2.Text = "0";
                            }


                            TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                            TextBlock SalePrices = dgPackageInfo.Columns[9].GetCellContent(row1) as TextBlock;

                            TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row1) as TextBlock;

                            TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row1) as TextBlock;

                            //foreach (System.Windows.Controls.Image item in SpImages.Children)
                            //{
                            //    DataRow row = dtimages.NewRow();
                            //   // System.Drawing.Image imagesIS = item;

                            //    row["Images"] =imageToByteArray(ConvertSystemWindowsImagesToDrawingImages(item));//<Image byte>;
                            //    row["SKUName"] = SkuNumber1.Text;
                            //    row["SKUSequence"] = txtRetutn2.Text; 
                            //    dtimages.Rows.Add(row);
                            //}



                            ls.SKUNumber = SkuNumber1.Text;
                            ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                            ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                            ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                            ls.ProductID = ProductID.Text;
                            ls.LineType = Convert.ToInt16(LineType.Text);

                            if (sku == _mReturn.GetENACodeByItem(SkuNumber1.Text))
                            {
                                if (max < Convert.ToInt16(txtRetutn2.Text))
                                {
                                    max = Convert.ToInt16(txtRetutn2.Text);
                                }
                                if (shipmax == Convert.ToInt16(ShipmentLines.Text))
                                {
                                    shipmax = Convert.ToInt16(ShipmentLines.Text);
                                }

                                if (returnmax == Convert.ToInt16(ReturnLines.Text))
                                {
                                    returnmax = Convert.ToInt16(ReturnLines.Text);
                                }
                            }

                            _lsRMAInfo1.Add(ls);
                        }

                        RMAInfo ls1 = new RMAInfo();
                        SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                        ls1.SKUNumber = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                        ls1.ProductID = _mReturn.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                        ls1.SalesPrice = 0;

                        _lsIsmanually1.IsScanned = 1;
                        _lsIsmanually1.SKUName = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                        lsIsManually.Add(_lsIsmanually1);


                        txtbarcode.Text = "";
                        txtbarcode.Focus();

                        ls1.SKU_Qty_Seq = 1;
                        ls1.SKU_Sequence = max + 1000;
                        ls1.ReturnLines = returnmax + 1000;
                        ls1.ShipmentLines = shipmax + 1000;
                        ls1.LineType = 1;
                        max = 0;
                        returnmax = 0;
                        shipmax = 0;

                        _lsRMAInfo1.Add(ls1);

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            dgPackageInfo.ItemsSource = _lsRMAInfo1;
                        }));

                        //for (int i = 0; i < dtimages.Rows.Count; i++)
                        //{
                        //    foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                        //    {
                        //        TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                        //        ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row1) as ContentPresenter;
                        //        DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                        //        TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                        //        ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row1) as ContentPresenter;
                        //        DataTemplate DtImages = CntImag.ContentTemplate;
                        //        StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                        //        if (SkuNumber1.Text == dtimages.Rows[i][1].ToString() && txtRetutn2.Text == dtimages.Rows[i][2].ToString())
                        //        {
                        //            byte[] arra = new byte[50];
                        //            arra[0] = Convert.ToByte(dtimages.Rows[i]["Images"]);

                        //            System.Drawing.Image ima = byteArrayToImage(arra);

                        //            _addToStackPanel(SpImages, ConvertDrawingImageToWPFImage(ima));

                        //        }
                        //    }

                        //}


                        dtLoadUpdate1 = new DispatcherTimer();
                        dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                        dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                        //start the dispacher.
                        dtLoadUpdate1.Start();
                    }
                    #endregion



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
                    #region part of PO
                    Boolean itemcheck = true;
                    for (int i = 0; i < _mReturn.lsRMAInformation.Count; i++)
                    {
                        if (_mReturn.lsRMAInformation[i].SKUNumber == _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString()))
                        {
                            itemcheck = false;// MessageBox.Show("This Scanned item is not part of PO.");

                        }
                    }

                    if (itemcheck)
                    {
                        MessageBox.Show("This Scanned item is not part of PO.");
                    }
                    #endregion

                    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                    {
                        SkuAndIsScanned _lsskuandscanned = new SkuAndIsScanned();

                        ContentPresenter CntQuantity = dgPackageInfo.Columns[2].GetCellContent(row) as ContentPresenter;
                        DataTemplate DtQty = CntQuantity.ContentTemplate;
                        TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                        TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
                        string Str = txtbarcode.Text.TrimStart('0').ToString();
                        string sku = _mReturn.GetENACodeByItem(SkuNumber.Text);
                        if (sku == Str)
                        {
                            _lsskuandscanned.SKUName = SkuNumber.Text;
                            _lsskuandscanned.IsScanned = 1;

                            lsskuIsScanned.Add(_lsskuandscanned);

                            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                            //row.Background = Brushes.SkyBlue;

                            #region For Zero
                            if (sku == Str && txtRetutn.Text == "0")
                            {
                                row.Background = Brushes.SkyBlue;
                                txtRetutn.Text = "1";
                                flag = true;
                                txtbarcode.Text = "";
                                txtbarcode.Focus();
                                break;
                            }
                            #endregion

                            #region For one
                            else if (sku == Str && txtRetutn.Text == "1" && row.Background != Brushes.SkyBlue)
                            {
                                List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                                foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                                {
                                    //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                                    TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row1) as TextBlock;

                                    //if (LineType.Text != "6")
                                    //{

                                    RMAInfo ls = new RMAInfo();
                                    TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                                    ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                                    TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                                    if (txtRetutn1.Text == "")
                                    {
                                        txtRetutn1.Text = "0";
                                    }


                                    ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                                    DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                                    TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                                    if (txtRetutn2.Text == "")
                                    {
                                        txtRetutn2.Text = "0";
                                    }


                                    TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                                    TextBlock SalePrices = dgPackageInfo.Columns[9].GetCellContent(row1) as TextBlock;

                                    TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row) as TextBlock;

                                    TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row) as TextBlock;

                                    ls.SKUNumber = SkuNumber1.Text;
                                    ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                                    ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                                    ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                                    ls.ProductID = ProductID.Text;
                                    ls.LineType = Convert.ToInt16(LineType.Text);

                                    if (sku == _mReturn.GetENACodeByItem(SkuNumber1.Text))
                                    {
                                        if (max < Convert.ToInt16(txtRetutn2.Text))
                                        {
                                            max = Convert.ToInt16(txtRetutn2.Text);
                                        }
                                        if (shipmax == Convert.ToInt16(ShipmentLines.Text))
                                        {
                                            shipmax = Convert.ToInt16(ShipmentLines.Text);
                                        }

                                        if (returnmax == Convert.ToInt16(ReturnLines.Text))
                                        {
                                            returnmax = Convert.ToInt16(ReturnLines.Text);
                                        }
                                    }
                                    _lsRMAInfo1.Add(ls);
                                    //}
                                }

                                RMAInfo ls1 = new RMAInfo();
                                SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                                ls1.SKUNumber = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                                ls1.ProductID = _mReturn.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                                ls1.SalesPrice = 0;
                                ls1.LineType = 1;

                                _lsIsmanually1.IsScanned = 1;
                                _lsIsmanually1.SKUName = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                                lsIsManually.Add(_lsIsmanually1);


                                txtbarcode.Text = "";
                                txtbarcode.Focus();

                                ls1.SKU_Qty_Seq = 1;
                                ls1.SKU_Sequence = max + 1000;
                                ls1.ReturnLines = returnmax + 1000;
                                ls1.ShipmentLines = shipmax + 1000;
                                max = 0;

                                _lsRMAInfo1.Add(ls1);
                                flag = true;
                                this.Dispatcher.Invoke(new Action(() => { dgPackageInfo.ItemsSource = _lsRMAInfo1; }));

                                dtLoadUpdate1 = new DispatcherTimer();
                                dtLoadUpdate1.Interval = new TimeSpan(0, 0, 0, 0, 10);
                                dtLoadUpdate1.Tick += dtLoadUpdate1_Tick;
                                //start the dispacher.
                                dtLoadUpdate1.Start();
                            }
                            Views.clGlobal.IsScanned = 1;
                            // txtbarcode.Text = "";
                            //  txtbarcode.Focus();

                            count++;
                            //break;



                            #endregion
                        }
                    }

                    #region Flag Check
                    if (!flag)
                    {
                        List<RMAInfo> _lsRMAInfo1 = new List<RMAInfo>();
                        foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
                        {
                            //SkuAndIsScanned _lsIsmanually = new SkuAndIsScanned();
                            RMAInfo ls = new RMAInfo();
                            TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;
                            string sku = _mReturn.GetENACodeByItem(SkuNumber1.Text);
                            TextBlock LineType = dgPackageInfo.Columns[10].GetCellContent(row1) as TextBlock;

                            ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                            TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                            if (txtRetutn1.Text == "")
                            {
                                txtRetutn1.Text = "0";
                            }

                            ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                            DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                            TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);


                            if (txtRetutn2.Text == "")
                            {
                                txtRetutn2.Text = "0";
                            }

                            TextBlock ProductID = dgPackageInfo.Columns[8].GetCellContent(row1) as TextBlock;

                            TextBlock SalePrices = dgPackageInfo.Columns[9].GetCellContent(row1) as TextBlock;

                            TextBlock ShipmentLines = dgPackageInfo.Columns[11].GetCellContent(row1) as TextBlock;

                            TextBlock ReturnLines = dgPackageInfo.Columns[12].GetCellContent(row1) as TextBlock;

                            ls.SKUNumber = SkuNumber1.Text;
                            ls.SKU_Qty_Seq = Convert.ToInt16(txtRetutn1.Text);
                            ls.SKU_Sequence = Convert.ToInt16(txtRetutn2.Text);
                            ls.SalesPrice = Convert.ToDecimal(SalePrices.Text);
                            ls.ProductID = ProductID.Text;
                            ls.LineType = Convert.ToInt16(LineType.Text);

                            if (sku == _mReturn.GetENACodeByItem(SkuNumber1.Text))
                            {
                                if (max < Convert.ToInt16(txtRetutn2.Text))
                                {
                                    max = Convert.ToInt16(txtRetutn2.Text);
                                }
                                if (shipmax == Convert.ToInt16(ShipmentLines.Text))
                                {
                                    shipmax = Convert.ToInt16(ShipmentLines.Text);
                                }

                                if (returnmax == Convert.ToInt16(ReturnLines.Text))
                                {
                                    returnmax = Convert.ToInt16(ReturnLines.Text);
                                }
                            }

                            _lsRMAInfo1.Add(ls);
                        }

                        RMAInfo ls1 = new RMAInfo();
                        SkuAndIsScanned _lsIsmanually1 = new SkuAndIsScanned();

                        ls1.SKUNumber = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());
                        ls1.ProductID = _mReturn.GetSKUNameAndProductNameByItem(txtbarcode.Text.TrimStart('0').ToString()).ToString().Split(new char[] { '@' })[1];

                        ls1.SalesPrice = 0;
                        _lsIsmanually1.IsScanned = 1;
                        _lsIsmanually1.SKUName = _mReturn.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                        lsIsManually.Add(_lsIsmanually1);


                        txtbarcode.Text = "";
                        txtbarcode.Focus();

                        ls1.SKU_Qty_Seq = 1;
                        ls1.SKU_Sequence = max + 1000;
                        ls1.ReturnLines = returnmax + 1000;
                        ls1.ShipmentLines = shipmax + 1000;
                        ls1.LineType = 1;
                        max = 0;
                        returnmax = 0;
                        shipmax = 0;


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
                    #endregion



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

            }
        }
        //public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        //{
        //    MemoryStream ms = new MemoryStream(byteArrayIn);
        //    System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
        //    return returnImage;
        //    //MemoryStream ms = new MemoryStream(byteArrayIn);
        //    //Image returnImage = Image.FromStream(ms);
        //    //return returnImage;
        //}
        //public byte[] imageToByteArray(System.Drawing.Image imageIn)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
        //    return ms.ToArray();
        //}

        //private System.Drawing.Image ConvertSystemWindowsImagesToDrawingImages(System.Windows.Controls.Image Images)
        //{
        //    // System.Drawing.Image ConvertedImages = new System.Drawing.Image();

        //    //BitmapImage bi = new BitmapImage(new Uri("pack://application:,,,/Assembly;component/Image.png", UriKind.Absolute));

        //    System.Windows.Controls.Image oldImage = new Image();

        //    oldImage.Source = Images.Source;

        //    MemoryStream ms = new MemoryStream();

        //    System.Windows.Media.Imaging.BmpBitmapEncoder bbe = new BmpBitmapEncoder();

        //    bbe.Frames.Add(BitmapFrame.Create(new Uri(oldImage.Source.ToString(), UriKind.RelativeOrAbsolute)));

        //    bbe.Save(ms);

        //    System.Drawing.Image newImage = System.Drawing.Image.FromStream(ms);


        //    return newImage;
        //}



        private System.Windows.Controls.Image ConvertDrawingImageToWPFImage(System.Drawing.Image gdiImg)
        {


            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            //convert System.Drawing.Image to WPF image
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(gdiImg);
            IntPtr hBitmap = bmp.GetHbitmap();
            System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            img.Source = WpfBitmap;
            img.Width = 74;
            img.Height = 62;
            img.Stretch = Stretch.Fill;
            return img;
        }


        Boolean ISinstalled = true;

        void dtLoadUpdate1_Tick(object sender, EventArgs e)
        {
            dtLoadUpdate1.Stop();

            foreach (DataGridRow row1 in GetDataGridRows(dgPackageInfo))
            {
                TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row1) as TextBlock;

                ContentPresenter CntQuantity1 = dgPackageInfo.Columns[2].GetCellContent(row1) as ContentPresenter;
                DataTemplate DtQty1 = CntQuantity1.ContentTemplate;
                TextBlock txtRetutn1 = (TextBlock)DtQty1.FindName("tbQty", CntQuantity1);

                ContentPresenter CntQuantity = dgPackageInfo.Columns[6].GetCellContent(row1) as ContentPresenter;
                DataTemplate DtQty = CntQuantity.ContentTemplate;
                TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbDQyt", CntQuantity);

                ContentPresenter CntSkuStatus = dgPackageInfo.Columns[7].GetCellContent(row1) as ContentPresenter;
                DataTemplate DtSKuStatus = CntSkuStatus.ContentTemplate;
                TextBlock txtSkuStatus = (TextBlock)DtSKuStatus.FindName("tbskustatus", CntSkuStatus);

                for (int i = 0; i < listofstatus.Count; i++)
                {
                    if (listofstatus[i].NewItemQuantity == Convert.ToInt16(txtRetutn.Text) && listofstatus[i].SKUName == SkuNumber.Text)
                    {
                        // row1.IsEnabled = false;
                        row1.Background = Brushes.SkyBlue;
                        txtSkuStatus.Text = listofstatus[i].Status;
                    }
                }

                if (txtRetutn1.Text == "1")
                {
                    row1.Background = Brushes.SkyBlue;
                }

                //for (int i = 0; i < dtimages.Rows.Count; i++)
                //{
                //    foreach (DataGridRow row12 in GetDataGridRows(dgPackageInfo))
                //    {
                //        TextBlock SkuNumber1 = dgPackageInfo.Columns[1].GetCellContent(row12) as TextBlock;

                //        ContentPresenter CntQuantity2 = dgPackageInfo.Columns[5].GetCellContent(row12) as ContentPresenter;
                //        DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                //        TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);

                //        ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row12) as ContentPresenter;
                //        DataTemplate DtImages = CntImag.ContentTemplate;
                //        StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                //        if (SkuNumber1.Text == dtimages.Rows[i][1].ToString() && txtRetutn2.Text == dtimages.Rows[i][2].ToString())
                //        {
                //           // string arr = dtimages.Rows[i]["Images"];
                //            //arra[0] = dtimages.Rows[i]["Images"];

                //            //TypeConverter objConverter = TypeDescriptor.GetConverter(dtimages.Rows[i]["Images"].GetType());
                //           // byte[] data = (byte[])objConverter.ConvertTo(dtimages.Rows[i]["Images"], typeof(byte[]));


                //            System.Drawing.Image ima = byteArrayToImage(ObjectToByteArray(dtimages.Rows[i]["Images"]));

                //            _addToStackPanel(SpImages, ConvertDrawingImageToWPFImage(ima));

                //        }
                //    }

                //}




            }

            _showBarcode();

        }
        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
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

                btnStatusNo.IsChecked = false;
                btnStatusYes.IsChecked = false;
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
                Views.clGlobal.SKU_Staus = "";
            }
            else
            {

            }
            ISinstalled = false;
            Views.clGlobal.SKU_Staus = "";
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            // btnAdd.IsEnabled = false;
            btnStatusNo.IsEnabled = true;
            btnStatusYes.IsEnabled = true;

            btnInstalledYes.IsChecked = false;
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

            btnStatusNo.IsChecked = false;
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
        Boolean IsManufacture = true;
        private void btnManufacturerYes_Checked(object sender, RoutedEventArgs e)
        {
            // ErrorMsg("This Item able to Refund.", Color.FromRgb(185, 84, 0));
            //btnAdd.IsEnabled = true;

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

            //ErrorMsg("This Item is Rejected.", Color.FromRgb(185, 84, 0));
            // btnAdd.IsEnabled = true;
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

                        ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(item) as ContentPresenter;
                        DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                        TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);
                        ItemQuantity = txtRetutn2.Text;

                        ContentPresenter CntQuantity21 = dgPackageInfo.Columns[2].GetCellContent(item) as ContentPresenter;
                        DataTemplate DtQty21 = CntQuantity21.ContentTemplate;
                        TextBlock txtRetutn21 = (TextBlock)DtQty21.FindName("tbQty", CntQuantity21);
                        SKUSequence = txtRetutn21.Text;


                    }
                }
                if (Views.clGlobal.mReturn.IsAlreadySaved)
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
                //dr[""]
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

                if (Views.clGlobal.mReturn.IsAlreadySaved)
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
                    SkuReasonID = _mReturn.SetReasons(txtskuReasons.Text);
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

                        ContentPresenter CntQuantity2 = dgPackageInfo.Columns[6].GetCellContent(item) as ContentPresenter;
                        DataTemplate DtQty2 = CntQuantity2.ContentTemplate;
                        TextBlock txtRetutn2 = (TextBlock)DtQty2.FindName("tbDQyt", CntQuantity2);
                        ItemQuantity = txtRetutn2.Text;


                        ContentPresenter CntQuantity21 = dgPackageInfo.Columns[2].GetCellContent(item) as ContentPresenter;
                        DataTemplate DtQty21 = CntQuantity21.ContentTemplate;
                        TextBlock txtRetutn21 = (TextBlock)DtQty21.FindName("tbQty", CntQuantity21);
                        SKUSequence = txtRetutn21.Text;

                    }
                }

                if (Views.clGlobal.mReturn.IsAlreadySaved)
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
                if (Views.clGlobal.mReturn.IsAlreadySaved)
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
                    SkuReasonID = _mReturn.SetReasons(txtskuReasons.Text);
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
            #endregion
        }

        private void fillComboBox()
        {
            List<Reason> lsReturn = _mReturn.GetReasons();

            //add reason select to the Combobox other reason.
            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";
            lsReturn.Insert(0, re);
            cmbSkuReasons.ItemsSource = lsReturn;
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



        int points = 0;
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

        private void dgPackageInfo_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

        private void dgPackageInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //dgPackageInfo.Items.RemoveAt(dgPackageInfo.SelectedIndex);
        }

        private int CountSelected()
        {
            int countselectedRow = 0;
            try
            {
                foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
                {
                    if (row.Background == Brushes.SkyBlue)
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

        private void ContentControl_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {



        }

        private void AddImage_Click_1(object sender, RoutedEventArgs e)
        {
            ContentControl cnt = (ContentControl)sender;
            DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();

            //StackPanel spRowImages = cnt.FindName("spProductImages") as StackPanel;

            ContentPresenter CntImag = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
            DataTemplate DtImages = CntImag.ContentTemplate;

            StackPanel spRowImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

            if (_mReturn.GreenRowsNumber.Contains(row.GetIndex()))
            {
                MessageBoxResult result = MessageBox.Show("Images Capture By Camera Press  -  Yes\n\nBrowse From System Press - No", "Confirmation", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
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
                                // img.MouseEnter += img_MouseEnter;

                                img.MouseDown += img_MouseDown;

                                img.Height = 50;
                                img.Width = 50;
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
                else if (result == MessageBoxResult.No)
                {

                    //ContentControl cnt1 = (ContentControl)sender;
                    //DataGridRow row1 = (DataGridRow)cnt.FindParent<DataGridRow>();

                    //StackPanel spRowImages1 = cnt1.FindName("spProductImages") as StackPanel;

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

                        string originalfilename = dlg.SafeFileName.Replace("-", "");

                        string finalfilename = originalfilename.Replace("_", "");

                        string ImageName = finalfilename.Replace("%", "");

                        string HashName = ImageName.Replace("#", "");

                        string AName = HashName.Replace("@", "");

                        Barcode.Camera.CopytoNetwork(AName);

                        // textBox1.Text = filename;
                        string path = "C:\\Images\\";


                        File.Copy(filename, path + "\\" + AName, true);

                        Barcode.Camera.CopytoNetwork(AName);

                        BitmapSource bs = new BitmapImage(new Uri(path + AName));

                        Image img = new Image();
                        //Zoom image.
                        // img.MouseEnter += img_MouseEnter;

                        img.MouseDown += img_MouseDown;

                        img.Height = 50;
                        img.Width = 50;
                        img.Stretch = Stretch.Fill;
                        img.Name = AName.ToString().Split(new char[] { '.' })[0];
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
            else
            {
                mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.SelectItem__00.ToString(), DateTime.UtcNow.ToString());
                ErrorMsg("Please select the item.", Color.FromRgb(185, 84, 0));
            }

        }

        private void btnPrint_Click_1(object sender, RoutedEventArgs e)
        {
            wndRMAFormPrint slip = new wndRMAFormPrint();
            clGlobal.NewRGANumber = _mUpdate._ReturnTbl.RGAROWID;
            slip.ShowDialog();
        }
    }
}

