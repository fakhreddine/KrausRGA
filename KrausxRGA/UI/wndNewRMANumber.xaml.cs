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

         DataTable dt = new DataTable();

         List<StatusAndPoints> listofstatus = new List<StatusAndPoints>();

        Guid ReturnDetailsID;

        string SKU,_SKU;
        string PName,_PName;
        string Qty,_Qty;
        string Cat, _Cat;

        DateTime eastern = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Eastern Standard Time");

         StackPanel spRowImages;

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

            if (((TextBox)DataTemp2.FindName("txtSKU", CntPersenter2)).Text != "" && ((TextBox)DataTemp1.FindName("txtProductName", CntPersenter1)).Text != "")
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
            MessageBox.Show("RMA number for this return is : " + _mNewRMA.GetNewROWID(ReturnTblID));
            for (int i = 0; i < dgPackageInfo.Items.Count; i++)
            {

                DataGridCell cell = GetCell(i, 1);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;

                SKU = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text.ToString();


                DataGridCell cell1 = GetCell(i, 2);
                ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
                DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;
                PName = ((TextBox)DataTemp1.FindName("txtProductName", CntPersenter1)).Text.ToString();


                DataGridCell cell2 = GetCell(i, 3);
                ContentPresenter CntPersenter2 = cell2.Content as ContentPresenter;
                DataTemplate DataTemp2 = CntPersenter2.ContentTemplate;
                Qty = ((TextBox)DataTemp2.FindName("tbQty", CntPersenter2)).Text.ToString();

                DataGridCell cell5 = GetCell(i, 6);
                ContentPresenter CntPersenter5 = cell5.Content as ContentPresenter;
                DataTemplate DataTemp5 = CntPersenter5.ContentTemplate;
                Cat = ((TextBox)DataTemp5.FindName("txtcategory", CntPersenter5)).Text.ToString();



                //DataGridCell cell3 = GetCell(i, 5);
                //ContentPresenter CntPersenter3 = cell3.Content as ContentPresenter;
                //DataTemplate DataTemp3 = CntPersenter3.ContentTemplate;
                //TextBlock txtRGuid = DataTemp3.FindName("txtReasosnsGuid", CntPersenter3) as TextBlock;


                DataGridCell cell4 = GetCell(i, 4);
                ContentPresenter CntPersenter4 = cell4.Content as ContentPresenter;
                DataTemplate DataTemp4 = CntPersenter4.ContentTemplate;
                StackPanel SpImages = (StackPanel)DataTemp4.FindName("spProductImages", CntPersenter4);


                if (SKU != null) _SKU = SKU;
                if (PName != null) _PName = PName;
                if (Qty != null) _Qty = Qty;
                if (Cat != null) _Cat = Cat;

                string SKUNewName = "";
                if (listofstatus.Count > 0)
                {
                    for (int j = 0; j < listofstatus.Count; j++)
                    {
                        if (listofstatus[j].SKUName == _SKU)
                        {
                            SKUNewName = _SKU;
                            Views.clGlobal.SKU_Staus = listofstatus[j].Status;
                            Views.clGlobal.TotalPoints = listofstatus[j].Points;
                            break;
                        }
                    }
                }
                else
                {
                    SKUNewName = _SKU;
                    Views.clGlobal.SKU_Staus = "Reject";
                    Views.clGlobal.TotalPoints = 0;
                }

                if (_SKU != "")
                {
                    ReturnDetailsID = _mNewRMA.SetReturnDetailTbl(ReturnTblID, _SKU, _PName, 0, 0, Convert.ToInt32(_Qty), _Cat, clGlobal.mCurrentUser.UserInfo.UserID, Views.clGlobal.SKU_Staus, Views.clGlobal.TotalPoints, 1, 1);
                }

                if (dt.Rows.Count > 0)
                {
                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        Guid ReturnedSKUPoints = _mNewRMA.SetReturnedSKUPoints(Guid.NewGuid(), ReturnDetailsID, ReturnTblID, dt.Rows[k][0].ToString(), dt.Rows[k][1].ToString(), dt.Rows[k][2].ToString(), Convert.ToInt16(dt.Rows[k][3].ToString()));
                    }
                    dt.Clear();
                }



                //foreach (Guid Ritem in (txtRGuid.Text.ToString().GetGuid()))
                //{
                //    _mNewRMA.SetTransaction(Ritem, ReturnDetailsID);
                //}

                foreach (Image imageCaptured in SpImages.Children)
                {
                    String NameImage = KrausRGA.Properties.Settings.Default.DrivePath + imageCaptured.Name.ToString() + ".jpg";

                    //Set Images table from model.
                    Guid ImageID = _mNewRMA.SetReturnedImages(ReturnDetailsID, NameImage, clGlobal.mCurrentUser.UserInfo.UserID);
                }

                if (_SKU != "")
                {

                    wndSlipPrint slip = new wndSlipPrint();

                    Views.clGlobal.lsSlipInfo = _mNewRMA.GetSlipInfo(_lsreturn, _SKU, _mNewRMA.GetENACodeByItem(_SKU), "", _mNewRMA.GetNewROWID(ReturnTblID), Views.clGlobal.WrongRMAFlag, Views.clGlobal.SKU_Staus);

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

        //private void cmbOtherReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (cmbOtherReason.SelectedIndex == 0)
        //    {
        //        txtOtherReason.Text = "";
        //    }
        //    else
        //    {
        //        Reason s = (Reason)cmbOtherReason.SelectedItem;
        //        txtOtherReason.Text = s.Reason1.ToString();
        //    }
        //}

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


            //Reason re = new Reason();
            //re.ReasonID = Guid.NewGuid();
            //re.Reason1 = "--Select--";

            //lsReturn.Insert(0, re);

               dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Reason", typeof(string));
            dt.Columns.Add("Reason_Value", typeof(string));
            dt.Columns.Add("Points", typeof(int));

           // cmbOtherReason.ItemsSource = lsReturn;

            FillRMAStausAndDecision();

            var data = new RDetails { SKU = "", ProductName = "", Quantity = "1", cat = "" };

            dgPackageInfo.Items.Add(data);
            txtbarcode.Focus();

        

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
        
        private void ContentControl_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            ContentControl cnt = (ContentControl)sender;
            StackPanel spRowImages = cnt.FindName("spProductImages") as StackPanel;
         
            DataGridRow row = (DataGridRow)cnt.FindParent<DataGridRow>();
            int index = row.GetIndex();


            DataGridCell cell = GetCell(index, 0);
            ContentPresenter CntPersenter = cell.Content as ContentPresenter;
            DataTemplate DataTemp = CntPersenter.ContentTemplate;

            DataGridCell cell1 = GetCell(index, 1);
            ContentPresenter CntPersenter1 = cell1.Content as ContentPresenter;
            DataTemplate DataTemp1 = CntPersenter1.ContentTemplate;

            if (((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text != "" && ((TextBox)DataTemp1.FindName("txtProductName", CntPersenter1)).Text != "")
            {
                try
                {
                    //Show Camera.
                    Barcode.Camera.Open();
                    foreach (String Nameitem in Views.clGlobal.lsImageList)
                    {
                        try
                        {
                            string path = "C:\\images" + "\\";

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

            for (int i = 0; i < dgPackageInfo.Items.Count; i++)
            {

                DataGridCell cell = GetCell(i, 0);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DtQty = CntPersenter.ContentTemplate;

                DataGridCell cell2 = GetCell(i, 1);
                ContentPresenter CntPersenter2 = cell2.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter2.ContentTemplate;

                SKU = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter2)).Text.ToString();

                Button txtRetutn = (Button)DtQty.FindName("btnGreen", CntPersenter);

                if (txtRetutn.Visibility == Visibility.Visible)
                {
                    SelectedskuName = SKU;
                    //dgPackageInfo.Items[i] = IsEnabled = true;
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
            itemnew = true;
            IsStatus = true;
            IsManufacture = true;
            IsDefectiveTransite = true;
            ISinstalled = true;



            int ro = dt.Rows.Count;
            UncheckAllButtons();
           // ErrorMsg("Please Select Next Item or Go ahead.", Color.FromRgb(185, 84, 0));
            btnAdd.IsEnabled = false;
        }
        

        Boolean itemnew = true;
        int count = 0;
        int points = 0;
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
            Views.clGlobal.SKU_Staus = "Refund";
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
            btnManufacturerNo.IsChecked = false;
            btnManufacturerYes.IsChecked = false;
            btntransiteNo.IsChecked = false;
            btntransiteYes.IsChecked = false;
            IsManufacture = true;
            IsDefectiveTransite = true;
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
            CanvasConditions.IsEnabled = true;
            btnAdd.IsEnabled = true;
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


                DataGridCell cell = GetCell(count, 1);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;
                ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text = _mNewRMA.GetSKUNameByItem(txtbarcode.Text.TrimStart('0').ToString());

                txtbarcode.Text = "";
                txtbarcode.Focus();

                var data = new RDetails { SKU = "", ProductName = "", Quantity = "1", cat = "" };

                dgPackageInfo.Items.Add(data);
            }
        }
       
    }
}
