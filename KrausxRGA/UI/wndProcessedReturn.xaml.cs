using KrausRGA.EntityModel;
using KrausRGA.Models;
using KrausRGA.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndProcessedReturn.xaml
    /// </summary>
    public partial class wndProcessedReturn : Window
    {
        protected DBLogics.cmdReturn cReturnTbl = new DBLogics.cmdReturn();

        mReturnDetails _mReturn;

        mPOnumberRMA _mponumner = new mPOnumberRMA();
        
        mUser _mUser;

        protected DBLogics.cmdReturnDetail cRetutnDetailsTbl = new DBLogics.cmdReturnDetail();
 
        public wndProcessedReturn()
        {
            InitializeComponent();
        }
 
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
           // dgPackageInfo.ItemsSource = cReturnTbl.GetReturnTbl();
            var sort = (from so in cReturnTbl.GetReturnTbl() select so).OrderByDescending(x => x.RGAROWID); //RGAROWID

            dgPackageInfo.ItemsSource = sort;

            clGlobal.Redirect = "";
          

            //var filter = (from p in cReturnTbl.GetReturnTbl()
            //              where (p.ReturnDate <= date1 && (p.ReturnDate >= date2))
            //              select p).ToList();
        }

        private void dgPackageInfo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = dgPackageInfo.SelectedIndex;

            if (selectedIndex != -1)
            {
                DataGridCell cell = GetCell(selectedIndex, 0);
                ContentPresenter CntPersenter = cell.Content as ContentPresenter;
                DataTemplate DataTemp = CntPersenter.ContentTemplate;
                TextBox txtReturnGuid = (TextBox)DataTemp.FindName("txtRGANumber", CntPersenter);
                var retunbyrow = _mponumner.GetReturnByRowID(txtReturnGuid.Text)[0];



                if (retunbyrow.RMANumber != null)
                {
                    _mReturn = new mReturnDetails(retunbyrow.RMANumber);

                    //keeps deep copy throughout project to access.
                    clGlobal.mReturn = _mReturn;

                    if (_mReturn.IsValidNumber) //Is number valid or not.
                    {

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            //Create new instance of window.
                            clGlobal.Redirect = "Processed";
                            wndSrNumberInfo wndMain = new wndSrNumberInfo();
                            // mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ValidRMANumberScan.ToString(), DateTime.UtcNow.ToString(), _mReturn.EnteredNumber);
                            //opens new window.
                            wndMain.Show();
                        }));

                        //close this screen.
                        this.Close();
                    }
                }
                else
                {
                    if (retunbyrow.OrderNumber == "N/A")
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            Views.clGlobal.IsAlreadySaved = true;
                            clGlobal.Redirect = "Processed";
                            //Create new instance of window.
                            wndNewRMANumber wndMain = new wndNewRMANumber();
                           // mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), "Valid_RGANumber_Scan", DateTime.UtcNow.ToString(), _mReturn.EnteredNumber);
                            //opens new window.
                            wndMain.Show();
                        }));

                        //close this screen.
                        this.Close();
                    }
                    else
                    {
                        Views.clGlobal.Ponumber = retunbyrow.PONumber;
                        _mponumner.mPOnumberRMA1(Views.clGlobal.Ponumber);

                        if (Views.clGlobal.IsAlreadySaved)
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                //Create new instance of window.
                                clGlobal.Redirect = "Processed";
                                wndPONumber wndMain = new wndPONumber();
                               // mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), "Valid_RGANumber_Scan", DateTime.UtcNow.ToString(), _mReturn.EnteredNumber);
                                //opens new window.
                                wndMain.Show();
                            }));

                            //close this screen.
                            this.Close();
                        }
                    }

                }

                Guid Returnid = cReturnTbl.GetRetrunByROWID(txtReturnGuid.Text)[0].ReturnID;
             

                dgReturnDetailInfo.ItemsSource = cRetutnDetailsTbl.GetReturnDetailsByReturnID(Returnid);

               


               



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

        private void dgPackageInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          //  IList rows = dgPackageInfo.SelectedItems;
           // DataRowView row = (DataRowView)dgPackageInfo.SelectedItems[0];
            //string val = rows[0] //row["Column ID"].ToString();
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            wndBoxInformation boxinfo = new wndBoxInformation();
            boxinfo.Show();
            this.Close();
        }

        private void rbtAll_Checked_1(object sender, RoutedEventArgs e)
        {
           // dtpfrom.IsEnabled = false;
           // dtpto.IsEnabled = false;


            var sort = (from so in cReturnTbl.GetReturnTbl() select so).OrderByDescending(x => x.RGAROWID); //RGAROWID

            dgPackageInfo.ItemsSource = sort;// cReturnTbl.GetReturnTbl();
        }

        private void rbtBetween_Checked_1(object sender, RoutedEventArgs e)
        {
            dtpfrom.IsEnabled = true;
            dtpto.IsEnabled = true;

        }

        private void dtpfrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = (from p in cReturnTbl.GetReturnTbl()
                          where (p.ReturnDate >= dtpfrom.SelectedDate && (p.ReturnDate <= dtpto.SelectedDate))
                          select p).OrderByDescending(y => y.RGAROWID);

            dgPackageInfo.ItemsSource = filter;
        }

        private void dtpto_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = (from p in cReturnTbl.GetReturnTbl()
                          where (p.ReturnDate >= dtpfrom.SelectedDate && (p.ReturnDate <= dtpto.SelectedDate))
                          select p).OrderByDescending(s => s.RGAROWID);

            dgPackageInfo.ItemsSource = filter;
        }
    }
}
