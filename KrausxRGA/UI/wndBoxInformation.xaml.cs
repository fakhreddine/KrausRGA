
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
using KrausRGA.Views;
using KrausRGA.Models;
using System.Threading;
using System.Windows.Media.Effects;
using System.Windows.Threading;


namespace KrausRGA.UI
{
    /// <summary>
    /// Avi: 11 Oct 2013. KrausRGA.
    /// Interaction logic for wndBoxInformation.xaml
    /// </summary>
    public partial class wndBoxInformation : Window
    {
        mReturnDetails _mReturn;
        mUser _mUser;
        DispatcherTimer dsptSacnner;
        int ProcessBarValue = 0;
        
        public wndBoxInformation()
        {
            InitializeComponent();

            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
        }

        #region Events

        private void wndLogin_Loaded(object sender, RoutedEventArgs e)
        {
            //mAudit.logthis(eActionType.Login_PageStart.ToString(),"ApplicationStatred", DateTime.UtcNow.ToString());
            //Hide Button Window and show Login Window
            hideButtons(System.Windows.Visibility.Hidden);

            //If User is alrady logged then hide the login screen.
            if (clGlobal.IsUserlogged)
            {
                
                hideButtons(System.Windows.Visibility.Visible);
                _mUser = clGlobal.mCurrentUser;
                btnBoxNumber_Click(btnBoxNumber, new RoutedEventArgs { });
            }
            else
            {
                //If no user is logged in then assign new user to the model
                _mUser = new mUser();
            }
           
        }

        #endregion

        #region private Functions.

        /// <summary>
        /// set the visibilty property of Login textbox and Button controls border
        /// </summary>
        /// <param name="visibility">
        /// System.Windows.Visibility visibility enum Property
        ///passed Visibility work same for buttons but apposit for login box at the same time
        /// </param>
        private void hideButtons(System.Windows.Visibility visibility)
        {
            bdrButtons.Visibility = visibility;
            if (visibility.ToString() == System.Windows.Visibility.Hidden.ToString())
            {
                bdrLogin.Visibility = System.Windows.Visibility.Visible;
                txtLogin.Focus();
            }
            else
            {
                bdrLogin.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        #endregion

        private void txtLogin_KeyDown(object sender, KeyEventArgs e)
        {
                   
            //If pressed key is Enter then Scan for UserName and  show  hide Buttons.
            if (e.Key == Key.Enter)
            {
                if (txtLogin.Text.Trim() != "")
                {
                    if (_mUser.IsValidUser(txtLogin.Text, "2wvcDW8j"))
                    {
                        if (_mUser.IsPermitedTo(ePermissione.Login))
                        {
                            hideButtons(System.Windows.Visibility.Visible);

                            btnBoxNumber_Click(btnBoxNumber, new RoutedEventArgs { });
                            //Set UserLogged flag true.
                            clGlobal.IsUserlogged = true;

                            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Login_Success.ToString(), DateTime.UtcNow.ToString(),_mUser.UserInfo.UserFullName);

                            //Manage Current User information.
                            clGlobal.mCurrentUser = _mUser;
                            ErrorMsg("Welcome " + _mUser.UserInfo.UserFullName.ToString(), Color.FromRgb(84, 185, 0));
                        }
                        else
                        {
                            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.UserPermissonDenied.ToString(), DateTime.UtcNow.ToString(), _mUser.UserInfo.UserFullName);
                            ErrorMsg("You are not permitted to login.", Color.FromRgb(185, 84, 0));
                            txtLogin.Text = "";
                        }
                    }
                    else
                    {
                        mRMAAudit.NoUserlogthis(eActionType.LoginFail__00.ToString(), DateTime.UtcNow.ToString(),txtLogin.Text.ToString());
                        ErrorMsg("Invalid user information.", Color.FromRgb(185, 84, 0));
                        txtLogin.Text = "";
                    }
                }
            }
           

        }

        private void btnBoxNumber_Click(object sender, RoutedEventArgs e)
        {
            bdrScan.Visibility = System.Windows.Visibility.Hidden;
            bdrScan.Visibility = System.Windows.Visibility.Visible;
            txtScan.Focus();
        }

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Application.Current.Dispatcher.BeginInvoke((ThreadStart)delegate()
             {
                 ScannProgressBarStart();
             });
                if (txtScan.Text.Trim() != "") //if clear text box.
                {
                    String TempRMANumber = txtScan.Text.ToUpper();
                    //call constructor of Return Model.
                    _mReturn = new mReturnDetails(txtScan.Text.ToUpper());

                    //keeps deep copy throughout project to access.
                    clGlobal.mReturn = _mReturn;

                    if (_mReturn.IsValidNumber) //Is number valid or not.
                    {
                        if (!_mReturn.IsAlreadySaved) //Is number already saved in database.
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                                {
                                    //Create new instance of window.
                                    wndSrNumberInfo wndMain = new wndSrNumberInfo();
                                    mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ValidRMANumberScan.ToString(), DateTime.UtcNow.ToString(), _mReturn.EnteredNumber);
                                    //opens new window.
                                    wndMain.Show();
                                }));

                            //close this screen.
                            this.Close();
                        }
                        else
                        {
                            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.AlreadySaved_RMANumberScanned__00.ToString(), DateTime.UtcNow.ToString(),_mReturn.EnteredNumber);
                            ErrorMsg(_mReturn.EnteredNumber + " is already saved.", Color.FromRgb(185, 84, 0));
                            txtScan.Text = "";
                        }
                    }
                    else
                    {
                        mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.InvalidRMANumberScanned__00.ToString(), DateTime.UtcNow.ToString(), TempRMANumber);
                        ErrorMsg("Invalid Number. Please check the number.", Color.FromRgb(185, 84, 0));
                        txtScan.Text = "";
                    }
                }
                else
                {
                    txtScan.Text = "";
                }
            }
        }

        public void ScannProgressBarStart()
        {
            dsptSacnner = new DispatcherTimer();
            dsptSacnner.Tick +=dsptSacnner_Tick;
            dsptSacnner.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dsptSacnner.Start();
        }

        private void dsptSacnner_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke((ThreadStart)delegate()
            {
                ProcessBarValue = ProcessBarValue + 1;
                pbrScanner.Value = ProcessBarValue;
                if (ProcessBarValue > 100)
                {
                    dsptSacnner.Stop();
                    pbrScanner.Value = 0;
                    ProcessBarValue = 0;
                }
            });
        }

        #region Error message strip functions.

        /// <summary>
        /// background color for message is default. light blue.
        /// </summary>
        /// <param name="Msg">
        /// String Message to be show.
        /// </param>
        private void ErrorMsg(string Msg)
        {
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            bdrMsg.Visibility = System.Windows.Visibility.Visible;
            bdrMsg.Background = new SolidColorBrush(Color.FromRgb(0, 122, 204));
            txtError.Text = Msg;
        }

        /// <summary>
        /// overload for Error message with color. 
        /// sets color for backGround.
        /// </summary>
        /// <param name="Msg">
        /// String message to show.
        /// </param>
        /// <param name="BgColor">
        /// Color for Background.
        /// </param>
        private void ErrorMsg(string Msg, Color BgColor)
        {
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;
            bdrMsg.Visibility = System.Windows.Visibility.Visible;
            bdrMsg.Background = new SolidColorBrush(BgColor);
            txtError.Text = Msg;
        }

        #endregion

        private void wndLogin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (bdrScan.Visibility == Visibility.Visible)
            {
              //  mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.WindowClosed.ToString(), DateTime.UtcNow.ToString(), "login Window");
            }


            if(bdrLogin.Visibility==Visibility.Visible)
            {
                mRMAAudit.NoUserlogthis(eActionType.WindowClosed.ToString(), DateTime.UtcNow.ToString(),"login Window");
                //mAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.ApplicationExit.ToString(), DateTime.UtcNow.ToString(), _mUser.UserInfo.UserName.ToString());
            }
        }


    }

}
