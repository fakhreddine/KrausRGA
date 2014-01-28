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

namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndNewRMANumber.xaml
    /// </summary>
    public partial class wndNewRMANumber : Window
    {

        mNewRMANumber _mNewRMA = new mNewRMANumber();

        public wndNewRMANumber()
        {
            InitializeComponent();
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

        }

        private void cbrDamaged_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void cbrDuplicate_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void cbrDuplicate_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void cbrIncorrectOrder_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void cbrIncorrectOrder_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void cbrDisplayedDiff_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void cbrDisplayedDiff_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void cbrSatisfied_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void cbrSatisfied_Unchecked(object sender, RoutedEventArgs e)
        {

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

            _lsreturn.Add(ret);

            //Save to RMA Master Table.
            Guid ReturnTblID = _mNewRMA.SetReturnTbl(_lsreturn,ReturnReasons(), RMAStatus, Decision, clGlobal.mCurrentUser.UserInfo.UserID);
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

        }

        private void ContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContentControl_MouseDown_2(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContentControl_MouseDown_4(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContentControl_MouseDown_3(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContentControl_MouseDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContentControl_MouseDown_5(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbOtherReason_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            List<Reason> lsReturn = _mNewRMA.GetReasons();


            Reason re = new Reason();
            re.ReasonID = Guid.NewGuid();
            re.Reason1 = "--Select--";

            lsReturn.Insert(0, re);

            cmbOtherReason.ItemsSource = lsReturn;
        }
    }
}
