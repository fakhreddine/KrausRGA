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

        mUser _mUser = clGlobal.mCurrentUser;

        Guid ReturnDetailsID;

        ScrollViewer SvImagesScroll;

        List<int> GreenRowsNumber = new List<int>();

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

        private void cbrWrong_Checked(object sender, RoutedEventArgs e)
        {
            bdrRecivedWrong.Inside();
        }

        private void cbrWrong_Unchecked(object sender, RoutedEventArgs e)
        {
            bdrRecivedWrong.Outside();
        }

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

            //Save to RMA Master Table.
            Guid ReturnTblID = _mNewRMA.SetReturnTbl(_lsreturn,ReturnReasons(), RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID);
            MessageBox.Show("RMA number for this return is : " + _mNewRMA.GetNewROWID(ReturnTblID));
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
                    RMAInfo rmaInfo = _mponumber.lsRMAInformationforponumner.FirstOrDefault(xrm => xrm.SKUNumber == SkuNumber.Text && xrm.ProductName == ProcutName.Text);
                    int DeliveredQty = rmaInfo.DeliveredQty;
                    Nullable<int> ExpectedQty = null;
                    int v2;
                    v2 = ExpectedQty.GetValueOrDefault();
                    int v3;
                    Nullable<int> ReturnQty = null;
                    v3 = ReturnQty.GetValueOrDefault();
                    string tck = rmaInfo.TCLCOD_0;

                    //Set returned details table.
                    Guid ReturnDetailsID = _mponumber.SetReturnDetailTbl(Guid.NewGuid(), ReturnTblID, SkuNumber.Text, ProcutName.Text, DeliveredQty, v2, v3, tck, clGlobal.mCurrentUser.UserInfo.UserID);

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
        
        private String ReturnReasons()
        {
            String _ReturnReason = "";

            if (cbrDamaged.IsChecked == true) _ReturnReason = _ReturnReason + txtitemdamage.Text;

            if (cbrDisplayedDiff.IsChecked == true) _ReturnReason = _ReturnReason + txtDisplayedOff.Text;

            if (cbrDuplicate.IsChecked == true) _ReturnReason = _ReturnReason + txtDuplicate.Text;

            if (cbrIncorrectOrder.IsChecked == true) _ReturnReason = _ReturnReason + txtinccorectorder.Text;

            if (cbrSatisfied.IsChecked == true) _ReturnReason = _ReturnReason + txtSatisfied.Text;

            if (cbrWrong.IsChecked == true) _ReturnReason = _ReturnReason + txtreceicewrongitem.Text;

            _ReturnReason += txtOtherReason.Text;

            return _ReturnReason;

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
        
        private void ContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeColor(cbrDamaged, txtitemdamage, cnvDamage);
        }

        private void ContentControl_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            ChangeColor(cbrWrong, txtreceicewrongitem, cnvRecieve);
        }

        private void ContentControl_MouseDown_4(object sender, MouseButtonEventArgs e)
        {
            ChangeColor(cbrIncorrectOrder, txtinccorectorder, cnvInccorectorder);
        }

        private void ContentControl_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            ChangeColor(cbrDisplayedDiff, txtDisplayedOff, cnvDisplayedOff);
        }

        private void ContentControl_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            ChangeColor(cbrDuplicate, txtDuplicate, cnvDuplicate);
        }

        private void ContentControl_MouseDown_5(object sender, MouseButtonEventArgs e)
        {
            ChangeColor(cbrSatisfied, txtSatisfied, cnvSatisfied);
        }

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
                  txtAddress.Text = lsCustomeronfo[0].Address1;
                  txtCountry.Text = lsCustomeronfo[0].Country;
                  txtCustCity.Text = lsCustomeronfo[0].City;
                  txtState.Text = lsCustomeronfo[0].State;
                  txtZipCode.Text = lsCustomeronfo[0].ZipCode;
                  txtName.Text = lsCustomeronfo[0].CustomerName1;
                  dgPackageInfo.ItemsSource = lsCustomeronfo;
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
                    txtPoNumber.Text = lsCustomeronfo[0].PONumber;
                    txtAddress.Text = lsCustomeronfo[0].Address1;
                    txtCountry.Text = lsCustomeronfo[0].Country;
                    txtCustCity.Text = lsCustomeronfo[0].City;
                    txtState.Text = lsCustomeronfo[0].State;
                    txtZipCode.Text = lsCustomeronfo[0].ZipCode;
                    txtName.Text = lsCustomeronfo[0].CustomerName1;
                    dgPackageInfo.ItemsSource = lsCustomeronfo;
                    _mponumber.lsRMAInformationforponumner = lsCustomeronfo;
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
        private void ErrorMsg(string Msg, Color BgColor)
        {
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            bdrMsg.Visibility = System.Windows.Visibility.Visible;
            bdrMsg.Background = new SolidColorBrush(BgColor);
            txtError.Text = Msg;
        }
        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            Button btnRed = (Button)e.Source;
            Canvas SpButtons = (Canvas)btnRed.Parent;
            Button btnGreen = SpButtons.FindName("btnGreen") as Button;
            btnGreen.Visibility = System.Windows.Visibility.Visible;
            btnRed.Visibility = System.Windows.Visibility.Hidden;

            DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
           GreenRowsNumber.Add(row.GetIndex());
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;

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
            Button btnGreen = (Button)e.Source;
            Canvas SpButtons = (Canvas)btnGreen.Parent;
            Button btnRed = SpButtons.FindName("btnRed") as Button;
            btnGreen.Visibility = System.Windows.Visibility.Hidden;
            btnRed.Visibility = System.Windows.Visibility.Visible;

            DataGridRow row = (DataGridRow)btnGreen.FindParent<DataGridRow>();
            GreenRowsNumber.Remove(row.GetIndex());
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;

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

            cmbOtherReason.ItemsSource = lsReturn;
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
