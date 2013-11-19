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
using WebcamControl;
using System.Drawing.Imaging;
using Microsoft.Expression.Encoder.Devices;
using System.Windows.Controls.Primitives;

namespace KrausRGA.UI
{

    /// <summary>
    /// Interaction logic for wndSrNumberInfo.xaml
    /// </summary>
    public partial class wndSrNumberInfo : Window
    {
        #region Declarations.

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

            bdrMsg.Visibility = System.Windows.Visibility.Hidden;

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

            // Create directory for saving image files.
            string imgPath = @"C:\SKUReturned";

            if (Directory.Exists(imgPath) == false)
            {
                Directory.CreateDirectory(imgPath);
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

        private void bdrButtonTemp_Loaded(object sender, RoutedEventArgs e)
        {
            //Remove this Button from UI.
            btnTemp.Focus();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
            bdrCamera.Visibility = System.Windows.Visibility.Visible;

            DataGridRow Rone = new DataGridRow();
            var v = dgPackageInfo.SelectedIndex;
            foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
            {
                if (row.IsSelected)
                {
                    Rone = row;
                    //ContentPresenter cp = dgPackageInfo.Columns[4].GetCellContent(Rone) as ContentPresenter;
                    //DataTemplate Dt = cp.ContentTemplate;
                    //StackPanel spProductIMages = (StackPanel)Dt.FindName("spProductImages", cp);
                    //spRowImages = spProductIMages;

                }
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

                int panelWidth = Convert.ToInt32(WebCamCtrl.ActualWidth);
                int panelHeight = Convert.ToInt32(WebCamCtrl.ActualHeight);

                var DirInfo = new DirectoryInfo(@"C:\SKUReturned");
                String ImageName = (from f in DirInfo.GetFiles()
                                    orderby f.LastWriteTime descending
                                    select f).First().Name.ToString();
                String ReNamed =DateTime.Now.ToString("ddMMMyyyy_hh_mm_ssfff_tt");
                File.Move(@"C:\SKUReturned\" + ImageName, @"C:\SKUReturned\" + "KRAUSGRA" + ReNamed+".jpeg");
                BitmapSource bs = new BitmapImage(new Uri(@"C:\SKUReturned\" + "KRAUSGRA" + ReNamed + ".jpeg"));

                Image img = new Image();
                //Zoom image.
                img.MouseEnter += img_MouseEnter;

                img.Height = 62;
                img.Width = 74;
                img.Stretch = Stretch.Fill;
                img.Name = "KRAUSGRA" + ReNamed ;
                img.Source = bs;
                img.Margin = new Thickness(1.0);
                // _addToStackPanel(spPhotos,img);

                //Images added to the Row.
                _addToStackPanel(spRowImages, img);

                img.Focus();
                sclPh.ScrollToRightEnd();
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
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            WebCamCtrl.StopCapture();

            bdrCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStartCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStop.Visibility = System.Windows.Visibility.Hidden;

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
            foreach (var cbk2 in cvCheckboxHolder.Children) //For each Control in the Canvas.
            {
                if (cbk2.GetType() == cbk.GetType())//If control is type of checkbox.
                {
                    cbk = (CheckBox)cbk2;
                    if (cbk.IsChecked == true) //if checkbox is checked.
                    {
                        _ReturnReason += cbk.Content.ToString() + " ";
                    }
                }
            }
            _ReturnReason += txtOtherReason.Text;

            return _ReturnReason;

        }

        #endregion

        private void btnHomeDone_Click(object sender, RoutedEventArgs e)
        {

            Byte RMAStatus = Convert.ToByte(cmbRMAStatus.SelectedValue.ToString());
            Byte Decision = Convert.ToByte(cmbRMADecision.SelectedValue.ToString());

            //Save to RMA Master Table.
            Guid ReturnTblID = _mReturn.SetReturnTbl(ReturnReasons(), RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID);

            foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
            {
                //CheckBOx item Peresent
                ContentPresenter CntPersenter = dgPackageInfo.Columns[0].GetCellContent(row) as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;
                CheckBox cbkItemPersent = (CheckBox)DataTemp.FindName("chkIsItemPresent", CntPersenter);

                // If item present in the return 
                if (cbkItemPersent.IsChecked == true)
                {

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
                    ComboBox cmbStatus = (ComboBox)DtStatus.FindName("cmbItemStatus", CntStatus);
                    int SelectedStatus = Convert.ToInt32(cmbStatus.SelectedIndex.ToString());
                    //Views.eStatus PStatus = (eStatus)Enum.Parse(typeof(eStatus), SelectedVal, true);

                    //Returned RMA Information.
                    RMAInfo rmaInfo = _mReturn.lsRMAInformation.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text && xrm.ProductName == ProcutName.Text);
                    int DeliveredQty = rmaInfo.DeliveredQty;
                    int ExpectedQty = rmaInfo.ExpectedQty;

                    //Set returned details table.
                    Guid ReturnDetailsID = _mReturn.SetReturnDetailTbl(ReturnTblID, SkuNumber.Text, ProcutName.Text, DeliveredQty, ExpectedQty, Convert.ToInt32(txtRetutn.Text), SelectedStatus, clGlobal.mCurrentUser.UserInfo.UserID);

                    //Save Images info Table.
                    foreach (Image imageCaptured in SpImages.Children)
                    {
                        String NameImage ="C:\\SKUReturned\\"+ imageCaptured.Name.ToString() +".jpeg";

                        //Set Images table from model.
                        Guid ImageID = _mReturn.SetReturnedImages(ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                    }
                }

            }

            wndBoxInformation wndBox = new wndBoxInformation();
            clGlobal.IsUserlogged = true;
            wndBox.Show();
            this.Close();
        }
  

        private void tbrgzdetail_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
                  
                if (cmbRMAStatus.SelectedIndex==0 && cmbRMADecision.SelectedIndex==0)
                {
                    bdrMsg.Visibility = System.Windows.Visibility.Visible;
                   ErrorMsg("Please select the RMA Status and RMA Decision.", Color.FromRgb(185, 84, 0));   
                }
                else if (cmbRMAStatus.SelectedIndex == 0 && cmbRMADecision.SelectedIndex != 0)
                {
                    bdrMsg.Visibility = System.Windows.Visibility.Visible;
                    ErrorMsg("Please select the RMA Status.", Color.FromRgb(185, 84, 0));   
                }
                else if (cmbRMAStatus.SelectedIndex != 0 && cmbRMADecision.SelectedIndex == 0)
                {
                    bdrMsg.Visibility = System.Windows.Visibility.Visible;
                    ErrorMsg("Please select the RMA Decision.", Color.FromRgb(185, 84, 0));   
                }
                else if (cmbRMAStatus.SelectedIndex != 0 && cmbRMADecision.SelectedIndex != 0)
                {
                    bdrMsg.Visibility = System.Windows.Visibility.Hidden;
                }
            
            
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
            catch (Exception)
            { }
        }

        private void btnPluse_Click(object sender, RoutedEventArgs e)
        {
            StackPanel Sp = (StackPanel)(sender as Control).Parent;
            StackPanel Sp2 = (StackPanel)Sp.Parent;
            try
            {
                foreach (TextBlock t in Sp2.Children)
                {
                    if (Convert.ToInt32(t.Text)>0)
                    {
                        t.Text = (Convert.ToInt32(t.Text) + 1).ToString();
                    }
                    break;
                }
            }
            catch (Exception)
            {}
        
        }
    }
}
