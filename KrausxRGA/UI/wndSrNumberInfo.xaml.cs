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
using WebcamControl;
using System.Drawing.Imaging;
using Microsoft.Expression.Encoder.Devices;
using System.Windows.Controls.Primitives;
using KrausRGA.EntityModel;
using System.Security.Principal;
using System.Runtime.InteropServices;


namespace KrausRGA.UI
{

    /// <summary>
    /// Interaction logic for wndSrNumberInfo.xaml
    /// </summary>
    public partial class wndSrNumberInfo : Window
    {

        #region Declarations.

        string imgPath = KrausRGA.Properties.Settings.Default.DrivePath;

        mUser _mUser = clGlobal.mCurrentUser;

        mReturnDetails _mReturn = clGlobal.mReturn;

        List<RMAInfo> _lsRMAInfo = new List<RMAInfo>();

        //Stack Panel in row assigned to this and used in Images captured add.
        StackPanel spRowImages;
        //Scroll Viewer from selected Row;
        ScrollViewer SvImagesScroll;

        //WEB cam frame height width.
        
        int Wheight = 530;
        int Wwidth = 400;

        #endregion

        public wndSrNumberInfo()
        {
            InitializeComponent();

            #region User Region.

            FillRMAStausAndDecision();

            // Bind the Video and Audio device properties of the
            // Webcam control to the SelectedValue property of 
            // the necessary ComboBox.
            Binding bndg_1 = new Binding("SelectedValue");
            bndg_1.Source = VidDvcsComboBox;
            WebCamCtrl.SetBinding(Webcam.VideoDeviceProperty, bndg_1);

            Binding bndg_2 = new Binding("SelectedValue");
            bndg_2.Source = AudDvcsComboBox;
            WebCamCtrl.SetBinding(Webcam.AudioDeviceProperty, bndg_2);
            // Create directory for saving video files.
            string vidPath = @"C:\VideoClips";

            if (Directory.Exists(vidPath) == false)
            {
                Directory.CreateDirectory(vidPath);
            }

            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            IntPtr admin_token = default(IntPtr);

            LogonUser(KrausRGA.Properties.Settings.Default.UserNameForImagesLogin, "domain", KrausRGA.Properties.Settings.Default.UserPasswordForImages, 9, 0, ref admin_token);

            WindowsIdentity identity = new WindowsIdentity(admin_token);

            WindowsImpersonationContext context = identity.Impersonate();

            try
            {
                if (Directory.Exists(imgPath) == false)
                {
                    Directory.CreateDirectory(imgPath);
                }
            }
            catch
            {
                context.Undo();
            }

            // Set some properties of the Webcam control
            WebCamCtrl.VideoDirectory = vidPath;
            WebCamCtrl.VidFormat = VideoFormat.mp4;

            WebCamCtrl.ImageDirectory = imgPath;
            WebCamCtrl.PictureFormat = ImageFormat.Jpeg;

            WebCamCtrl.FrameRate = 20;
            WebCamCtrl.FrameSize = new System.Drawing.Size(Wheight, Wwidth);

            // Find a/v devices connected to the machine.
            FindDevices();

            VidDvcsComboBox.SelectedIndex = 0;
            AudDvcsComboBox.SelectedIndex = 0;

            #endregion
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //fill OtherReason ComboBox
            List<Reason> lsReturn = _mReturn.GetReasons();

            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";

            lsReturn.Insert(0, re);

            cmbOtherReason.ItemsSource = lsReturn;

            // Display webcam video on control.
            bdrStop.Visibility = System.Windows.Visibility.Hidden;
            bdrCapture.Visibility = System.Windows.Visibility.Hidden;

            _lsRMAInfo = _mReturn.lsRMAInformation;

            //Set all vaues.
            lblRMANumber.Content = _mReturn.EnteredNumber;
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

            dgPackageInfo.ItemsSource = _lsRMAInfo;

        }

        #region Data Grid Events.

        private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentControl cnt = (ContentControl)sender;
            DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();

            if (_mReturn.GreenRowsNumber.Contains(row.GetIndex()))
            {
                bdrCamera.Visibility = System.Windows.Visibility.Visible;
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

        #endregion

        #region Web cam Methods

        private void FindDevices()
        {
            var vidDevices = EncoderDevices.FindDevices(EncoderDeviceType.Video);
            var audDevices = EncoderDevices.FindDevices(EncoderDeviceType.Audio);

            int CameraNumber = KrausRGA.Properties.Settings.Default.CameraNumber;
            int i = 0;
            foreach (EncoderDevice dvc in vidDevices)
            {
                if (i == CameraNumber)
                    VidDvcsComboBox.Items.Add(dvc.Name);
                i++;
            }

            foreach (EncoderDevice dvc in audDevices)
            {
                AudDvcsComboBox.Items.Add(dvc.Name);
            }

        }

        private void SnapshotButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // Take snapshot of webcam image.
                WebCamCtrl.TakeSnapshot();

                var DirInfo = new DirectoryInfo(imgPath);
                String ImageName = (from f in DirInfo.GetFiles()
                                    orderby f.LastWriteTime descending
                                    select f).First().Name.ToString();
                String ReNamed = DateTime.Now.ToString("ddMMMyyyy_hh_mm_ssfff_tt");
                File.Move(imgPath + ImageName, imgPath + "KRAUSGRA" + ReNamed + ".jpeg");
                BitmapSource bs = new BitmapImage(new Uri(imgPath + "KRAUSGRA" + ReNamed + ".jpeg"));

                Image img = new Image();
                //Zoom image.
                img.MouseEnter += img_MouseEnter;

                img.Height = 62;
                img.Width = 74;
                img.Stretch = Stretch.Fill;
                img.Name = "KRAUSGRA" + ReNamed;
                img.Source = bs;
                img.Margin = new Thickness(0.5);

                //Images added to the Row.
                _addToStackPanel(spRowImages, img);

                img.Focus();
                sclPh.ScrollToRightEnd();

                mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Image_Captured.ToString(), DateTime.UtcNow.ToString(), img.Name.ToString());

            }
            catch (Exception)
            { }
        }

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

        private void btnStartCapture_Click(object sender, RoutedEventArgs e)
        {


            bdrCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStartCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStop.Visibility = System.Windows.Visibility.Visible;
            WebCamCtrl.StartCapture();
        //   CanvasToImage.SaveCanvas(this, this.CvsImage, 96, "c:\\canvas.png");

            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Camera_Started.ToString(), DateTime.UtcNow.ToString());
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            WebCamCtrl.StopCapture();

            bdrCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStartCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStop.Visibility = System.Windows.Visibility.Hidden;

            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Camera_Stoped.ToString(), DateTime.UtcNow.ToString());

        }

        private void btnOpenCamera_Click(object sender, RoutedEventArgs e)
        {
            bdrCamera.Visibility = System.Windows.Visibility.Visible;

        }

        private void brnCloseCamera_Click(object sender, RoutedEventArgs e)
        {
            WebCamCtrl.StopCapture();
            WebCamCtrl.Dispose();
            bdrCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStartCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStop.Visibility = System.Windows.Visibility.Hidden;
            bdrCamera.Visibility = System.Windows.Visibility.Hidden;
            removeStackPanelChild(spPhotos);
        }

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
        private String ReturnReasons()
        {
            String _ReturnReason = "";
            CheckBox cbk = new CheckBox();
            Border bdr = new Border();
            foreach (var cbk2 in cvCheckboxHolder.Children) //For each Control in the Canvas.
            {
                if (cbk2.GetType() == bdr.GetType())//If control is type of checkbox.
                {
                    bdr = (Border)cbk2;
                    if (cbk.GetType() == bdr.Child.GetType())
                    {
                        cbk = (CheckBox)bdr.Child;
                        if (cbk.IsChecked == true) //if checkbox is checked.
                        {
                            _ReturnReason += cbk.Content.ToString() + " ";
                        }
                    }
                }
            }
            _ReturnReason += txtOtherReason.Text;

            return _ReturnReason;

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
                CheckBox chkIsChecked = chkDt.FindName("cbReasons", chkCp) as CheckBox;
                TextBlock ResonGuid = dgReasons.Columns[2].GetCellContent(Row) as TextBlock;
                if (chkIsChecked.IsChecked == true) _return = ResonGuid.Text.ToString();
            }
            catch (Exception)
            {
            }
            return _return;
        }
        #endregion

        private void btnHomeDone_Click(object sender, RoutedEventArgs e)
        {

            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Done_Clicked.ToString(), DateTime.UtcNow.ToString(), _mReturn.EnteredNumber);
            //Check the ReasonCombo Select or not

            WindowThread.start();

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

            //Save to RMA Master Table.
            Guid ReturnTblID = _mReturn.SetReturnTbl(ReturnReasons(), RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID);

            foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
            {
                //CheckBOx item Peresent
                ContentPresenter CntPersenter = dgPackageInfo.Columns[0].GetCellContent(row) as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;
                Button btnGreen = (Button)DataTemp.FindName("btnGreen", CntPersenter);

                if (btnGreen.Visibility == System.Windows.Visibility.Visible)
                {
                    // If item present in the return 
                    // item SKUNumber
                    TextBlock SkuNumber = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;

                    //Product Name.
                    TextBlock ProcutName = dgPackageInfo.Columns[2].GetCellContent(row) as TextBlock;

                    //item Returned Quantity.
                    ContentPresenter CntQuantity = dgPackageInfo.Columns[3].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtQty = CntQuantity.ContentTemplate;
                    TextBlock txtRetutn = (TextBlock)DtQty.FindName("tbQty", CntQuantity);

                    //Images Stack Panel.
                    ContentPresenter CntImag = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtImages = CntImag.ContentTemplate;
                    StackPanel SpImages = (StackPanel)DtImages.FindName("spProductImages", CntImag);

                    //item Status.k
                    ContentPresenter CntStatus = dgPackageInfo.Columns[5].GetCellContent(row) as ContentPresenter;
                    DataTemplate DtStatus = CntStatus.ContentTemplate;
                    TextBlock txtRGuid = DtStatus.FindName("txtReasosnsGuid", CntStatus) as TextBlock;

                    //Returned RMA Information.
                    RMAInfo rmaInfo = _mReturn.lsRMAInformation.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text && xrm.ProductName == ProcutName.Text);
                    int DeliveredQty = rmaInfo.DeliveredQty;
                    int ExpectedQty = rmaInfo.ExpectedQty;
                    string tck = rmaInfo.TCLCOD_0;

                    //Set returned details table.
                    Guid ReturnDetailsID = _mReturn.SetReturnDetailTbl(ReturnTblID, SkuNumber.Text, ProcutName.Text, DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), tck, clGlobal.mCurrentUser.UserInfo.UserID);

                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + imageCaptured.Name.ToString() + ".jpeg";

                        //Set Images table from model.
                        Guid ImageID = _mReturn.SetReturnedImages(ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }

                    //SKU Reasons Table
                    foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                    {
                        _mReturn.SetTransaction(Ritem, ReturnDetailsID);
                    }

                   mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                   Views.AuditType.lsaudit.Clear();

                }
            }
            wndBoxInformation wndBox = new wndBoxInformation();
            clGlobal.IsUserlogged = true;
            this.Close();
            //close wait screen.
            WindowThread.Stop();
            wndBox.Show(); 
           
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

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
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

        private void cbrWrong_Checked(object sender, RoutedEventArgs e)
        {
            bdrRecivedWrong.Inside();
        }

        private void cbrWrong_Unchecked(object sender, RoutedEventArgs e)
        {
            bdrRecivedWrong.Outside();
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

        private void cbrDamaged_Checked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrDamaged.Content.ToString());
            bdrDamaged.Inside();
        }

        private void cbrDamaged_Unchecked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrDamaged.Content.ToString());
            bdrDamaged.Outside();
        }

        private void cbrDuplicate_Checked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrDuplicate.Content.ToString());
            bdrDuplicate.Inside();
        }

        private void cbrDuplicate_Unchecked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrDuplicate.Content.ToString());
            bdrDuplicate.Outside();
        }

        private void cbrIncorrectOrder_Checked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrIncorrectOrder.Content.ToString());
            bdrIcorrectOrder.Inside();
        }

        private void cbrIncorrectOrder_Unchecked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrIncorrectOrder.Content.ToString());
            bdrIcorrectOrder.Outside();
        }

        private void cbrDisplayedDiff_Checked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrDisplayedDiff.Content.ToString());
            bdrDisplayedDiff.Inside();
        }

        private void cbrDisplayedDiff_Unchecked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrDisplayedDiff.Content.ToString());
            bdrDisplayedDiff.Outside();
        }

        private void cbrSatisfied_Checked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Checked.ToString(), DateTime.UtcNow.ToString(), cbrSatisfied.Content.ToString());
            bdrSatisfied.Inside();
        }

        private void cbrSatisfied_Unchecked(object sender, RoutedEventArgs e)
        {
            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Reason_Unchecked.ToString(), DateTime.UtcNow.ToString(), cbrSatisfied.Content.ToString());
            bdrSatisfied.Outside();
        }

        #endregion

        private void ctlReasons_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TextBlock cbk = (TextBlock)e.Source;
                Border bdr = (Border)cbk.Parent;
                DataGridRow row = (DataGridRow)cbk.FindParent<DataGridRow>();
                ContentPresenter cp = dgReasons.Columns[0].GetCellContent(row) as ContentPresenter;
                DataTemplate Dt = cp.ContentTemplate;
                CheckBox ch = (CheckBox)Dt.FindName("cbReasons", cp);

                if (ch.IsChecked == true)
                {
                    ch.IsChecked = false;
                }
                else
                {
                    ch.IsChecked = true;
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
            int indext = tbrgzdetail.SelectedIndex;
            if (indext > -1)
            {
                mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.Tab_changed.ToString(), DateTime.UtcNow.ToString(), "Tab Index" + indext.ToString());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.WindowClosed.ToString(), DateTime.UtcNow.ToString(), "RMA Details Window");
            mRMAAudit.saveaudit(Views.AuditType.lsaudit);
            Views.AuditType.lsaudit.Clear();
        }

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

    }
     
}

