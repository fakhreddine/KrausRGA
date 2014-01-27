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



namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndNewRMANumber.xaml
    /// </summary>
    public partial class wndNewRMANumber : Window
    {

        mNewRMANumber _mNewRMA = new mNewRMANumber();

        mUser _mUser = clGlobal.mCurrentUser;

        string SKU,_SKU;
        string PName,_PName;
        string Qty,_Qty;

        public wndNewRMANumber()
        {
            InitializeComponent();
            FillRMAStausAndDecision();
        }

        private void ContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void cntItemStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ctlReasons_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

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

        }

        private void cbrWrong_Unchecked(object sender, RoutedEventArgs e)
        {

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
            ret.ReturnDate = DateTime.UtcNow;
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
           
            for (int j = 0; j < dgPackageInfo.Columns.Count; j++)
            {
                for (int i = 0; i < dgPackageInfo.Items.Count; i++)
                {
                    // string s = (dgPackageInfo.Items[i] as DataRowView).Row.ItemArray[j].ToString();

                    DataGridCell cell = GetCell(i, j);
                    ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                    DataTemplate DataTemp = CntPersenter.ContentTemplate;

                    SKU = ((TextBox)DataTemp.FindName("txtSKU", CntPersenter)).Text.ToString();
                    PName = ((TextBox)DataTemp.FindName("txtProductName", CntPersenter)).Text.ToString();
                    Qty = ((TextBox)DataTemp.FindName("tbQty", CntPersenter)).Text.ToString();

                    if (SKU != null) _SKU = SKU;
                    if (SKU != null) _PName = PName;
                    if (SKU != null) _Qty = Qty;


                    //SKU = tb.Text;
                    // PName = tb.Text;
                    // Qty = tb.Text;

                }

              //  Guid ReturnDetailsID = _mNewRMA.SetReturnDetailTbl(ReturnTblID, SKU, PName, 0, 0, Convert.ToInt32(Qty), "", clGlobal.mCurrentUser.UserInfo.UserID);
            }

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
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeColor(cbrDamaged, txtitemdamage, cnvDamage); 
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
            cmbRMAStatus.ItemsSource = _mNewRMA.GetRMAStatusList();
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
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
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

            var data = new RDetails { SKU = "", ProductName = "", Quantity="" };

            dgPackageInfo.Items.Add(data);



        }

        public class RDetails
        {
            public string SKU { get; set; }
            public string ProductName { get; set; }
            public String Quantity { get; set; }
        }

        private void cmbRMAStatus_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void tbQty_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var data = new RDetails { SKU = "", ProductName = "", Quantity = "" };

                dgPackageInfo.Items.Add(data);
            }
        }







    }
}
