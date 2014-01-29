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
    /// Interaction logic for wndNewRMANumber.xaml
    /// </summary>
    public partial class wndNewRMANumber : Window
    {

        mNewRMANumber _mNewRMA = new mNewRMANumber();

        mUser _mUser = clGlobal.mCurrentUser;

        Guid ReturnDetailsID;

        string SKU,_SKU;
        string PName,_PName;
        string Qty,_Qty;
        string Cat, _Cat;

        DateTime eastern = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Eastern Standard Time");

         StackPanel spRowImages;

        public wndNewRMANumber()
        {
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

            //TextBlock cbk = (TextBlock)e.Source;
            //Canvas cs = cbk.Parent as Canvas;
            //TextBlock txtReasonGuids = cs.FindName("txtReasosnsGuid") as TextBlock;
            //DataGridRow row = (DataGridRow)cbk.FindParent<DataGridRow>();

          
                cvItemStatus.Visibility = System.Windows.Visibility.Visible;
                //TextBlock tbSKUName = dgPackageInfo.Columns[1].GetCellContent(row) as TextBlock;
              //  txtSKUname.Text = tbSKUName.Text.ToString();

                int index = dgPackageInfo.SelectedIndex;

                DataGridCell cell = GetCell(index, 0);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;

                string Sku = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text;

                List<String> NewRMAnumber=new List<string>();


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
              //  mRMAAudit.logthis(clGlobal.mCurrentUser.UserInfo.UserID.ToString(), eActionType.SelectItem__00.ToString(), DateTime.UtcNow.ToString());
               // ErrorMsg("Please select the item.", Color.FromRgb(185, 84, 0));
            
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

            List<Return> _lsreturn = new List<Return>();
            Return ret = new Return();
            ret.RMANumber = txtRMANumber.Text;
            ret.VendoeName = txtVendorName.Text;
            ret.VendorNumber = txtVendorNumber.Text;
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

            for (int i = 0; i < dgPackageInfo.Items.Count; i++)
            {

                DataGridCell cell = GetCell(i, 0);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;

                SKU = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text.ToString();


                DataGridCell cell1 = GetCell(i, 1);
                ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
                DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;
                PName = ((TextBox)DataTemp1.FindName("txtProductName", CntPersenter1)).Text.ToString();


                DataGridCell cell2 = GetCell(i, 2);
                ContentPresenter CntPersenter2 = cell2.Content as ContentPresenter;
                DataTemplate DataTemp2 = CntPersenter2.ContentTemplate;
                Qty = ((TextBox)DataTemp2.FindName("tbQty", CntPersenter2)).Text.ToString();

                DataGridCell cell5 = GetCell(i, 5);
                ContentPresenter CntPersenter5 = cell5.Content as ContentPresenter;
                DataTemplate DataTemp5 = CntPersenter5.ContentTemplate;
                Cat = ((TextBox)DataTemp5.FindName("txtcategory", CntPersenter5)).Text.ToString();
             


                DataGridCell cell3 = GetCell(i, 4);
                ContentPresenter CntPersenter3 = cell3.Content as ContentPresenter;
                DataTemplate DataTemp3 = CntPersenter3.ContentTemplate;
                TextBlock txtRGuid = DataTemp3.FindName("txtReasosnsGuid", CntPersenter3) as TextBlock;


                DataGridCell cell4 = GetCell(i, 3);
                ContentPresenter CntPersenter4 = cell4.Content as ContentPresenter;
                DataTemplate DataTemp4 = CntPersenter4.ContentTemplate;
                StackPanel SpImages = (StackPanel)DataTemp4.FindName("spProductImages", CntPersenter4);


                if (SKU != null) _SKU = SKU;
                if (PName != null) _PName = PName;
                if (Qty != null) _Qty = Qty;
                if (Cat != null) _Cat = Cat;

                

                if (_SKU!="" && _PName!="")
                {
                   ReturnDetailsID = _mNewRMA.SetReturnDetailTbl(ReturnTblID, _SKU, _PName, 0, 0, Convert.ToInt32(_Qty), _Cat, clGlobal.mCurrentUser.UserInfo.UserID);    
                }


                foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                {
                    _mNewRMA.SetTransaction(Ritem, ReturnDetailsID);
                }

                foreach (Image imageCaptured in SpImages.Children)
                {
                    String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + imageCaptured.Name.ToString() + ".jpg";

                    //Set Images table from model.
                    Guid ImageID = _mNewRMA.SetReturnedImages(ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                }

            }
            wndBoxInformation wndBox = new wndBoxInformation();
            clGlobal.IsUserlogged = true;
            this.Close();
            //close wait screen.
            WindowThread.Stop();
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

        private void tbQty_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var data = new RDetails { SKU = "", ProductName = "", Quantity = "1", cat = "" };

                dgPackageInfo.Items.Add(data);
            }
        }

        private void txtSKU_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            List<String> _lsNewRMAnumber = new List<string>();

            string che = ((System.Windows.Controls.TextBox)(e.Source)).Text;

            _lsNewRMAnumber = _mNewRMA.NewRMAInfo(che);

            lstSKU.Visibility = Visibility.Visible;

            lstSKU.ItemsSource = _lsNewRMAnumber;

            //int index = dgPackageInfo.SelectedIndex;

            //int position =( (index) * 90)+110;

            //Canvas.SetTop(lstSKU, position);

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
        
        private void ContentControl_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            int index = dgPackageInfo.SelectedIndex;
            DataGridCell cell = GetCell(index, 3);
            ContentPresenter CntPersenter = cell.Content as ContentPresenter;
            DataTemplate DataTemp = CntPersenter.ContentTemplate;
            spRowImages = (StackPanel)DataTemp.FindName("spProductImages", CntPersenter);
                try
                {
                    //Show Camera.
                    Barcode.Camera.Open();
                    foreach (String Nameitem in Views.clGlobal.lsImageList)
                    {
                        try
                        {
                            string path = KrausRGA.Properties.Settings.Default.DrivePath + "\\";

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

        private void dgPackageInfo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // int selectedIndex = dgPackageInfo.SelectedIndex;
            //if (selectedIndex != -1)
            //{
            //    foreach (DataGridRow row in GetDataGridRows(dgPackageInfo))
            //    {
            //        if (row.IsSelected)
            //        {
            //            ContentPresenter cp = dgPackageInfo.Columns[4].GetCellContent(row) as ContentPresenter;
            //            DataTemplate Dt = cp.ContentTemplate;
            //            StackPanel spProductIMages = (StackPanel)Dt.FindName("spProductImages", cp);
            //            spRowImages = spProductIMages;
            //            ScrollViewer SvImages = (ScrollViewer)Dt.FindName("svScrollImages", cp);
            //            SvImagesScroll = SvImages;
            //        }
            //    }
            // }
            // }

        }

        private void dgPackageInfo_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            dgPackageInfo.Items.RemoveAt(dgPackageInfo.SelectedIndex);
        }


    }
}
