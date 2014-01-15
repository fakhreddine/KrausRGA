
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
using KrausRGA.EntityModel;
using System.IO;
using Microsoft.Expression.Encoder.Devices;
using System.Security.Principal;
using WebcamControl;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace KrausRGA.UI
{
    /// <summary>
    /// Avi: 11 Oct 2013. KrausRGA.
    /// Interaction logic for wndBoxInformation.xaml
    /// </summary>
    public partial class wndBoxInformation : Window
    {
        #region Declation

        //WEB cam .
        int Wheight = 700;
        int Wwidth = 520;

        string imgPath = "C:\\Users\\" +Environment.UserName + "\\Pictures\\Camera Roll\\";
        DispatcherTimer CaptureTime;
        mReturnDetails _mReturn;
        mUser _mUser;
        DispatcherTimer dsptSacnner;
        int ProcessBarValue = 0;

        #endregion

        public wndBoxInformation()
        {
            String[] FontSizes = File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[1].Split(new char[] { '-' });
            String HeaderSize = FontSizes[1];
            String ControlSize = FontSizes[2];
            String VeriableSize = FontSizes[0];

            Resources["FontSize"] = Convert.ToDouble(VeriableSize);
            Resources["HeaderSize"] = Convert.ToDouble(HeaderSize);
            Resources["ContactFontSize"] = Convert.ToDouble(ControlSize);
            InitializeComponent();
            bdrMsg.Visibility = System.Windows.Visibility.Hidden;

            #region Camera Region

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

            WebCamCtrl.FrameRate = 48;
            WebCamCtrl.FrameSize = new System.Drawing.Size(Wheight, Wwidth);

            // Find a/v devices connected to the machine.
            FindDevices();

            VidDvcsComboBox.SelectedIndex = 0;
            AudDvcsComboBox.SelectedIndex = 0;

            #endregion
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
            #region Start application within another application.
            //else if (StartupParametrs.UserName.ToString() != "")
            //{
            //    txtLogin.Text = StartupParametrs.UserName.ToString();
            //    hideButtons(System.Windows.Visibility.Visible);
            //    StartupParametrs.UserName = "";
            //    _mUser = new mUser();
            //    InputManager.Current.ProcessInput(new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter)
            //    {
            //        RoutedEvent = Keyboard.KeyDownEvent
            //    });

            //} 
            #endregion
            else
            {
                //If no user is logged in then assign new user to the model
                _mUser = new mUser();
            }

        }

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
                            //lsaudit.Insert(0

                            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.Login_Success.ToString(), DateTime.UtcNow.ToString(), _mUser.UserInfo.UserFullName);

                            //Manage Current User information.
                            clGlobal.mCurrentUser = _mUser;
                            txtUserName.Text = _mUser.UserInfo.UserFullName.ToString() + Environment.NewLine + "[ " + _mUser.RoleName + " ]";
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
                        mRMAAudit.NoUserlogthis(eActionType.LoginFail__00.ToString(), DateTime.UtcNow.ToString(), txtLogin.Text.ToString());
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
                            mRMAAudit.logthis(_mUser.UserInfo.UserID.ToString(), eActionType.AlreadySaved_RMANumberScanned__00.ToString(), DateTime.UtcNow.ToString(), _mReturn.EnteredNumber);
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
            dsptSacnner.Tick += dsptSacnner_Tick;
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
            if (bdrLogin.Visibility == Visibility.Visible)
            {
                mRMAAudit.NoUserlogthis(eActionType.WindowClosed.ToString(), DateTime.UtcNow.ToString(), "login Window");
                mRMAAudit.saveaudit(Views.AuditType.lsaudit);
                Views.AuditType.lsaudit.Clear();
            }
        }

        private void btnlogout_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        private void btnNewScan_Click(object sender, RoutedEventArgs e)
        {
            wndAppSetting app = new wndAppSetting();
            app.ShowDialog();
        }

        #endregion

        #region camera

        private void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            Barcode.Camera.TakePhoto();
           // bdrCamera.Visibility = System.Windows.Visibility.Visible;
        }

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

        private void btnStartCapture_Click(object sender, RoutedEventArgs e)
        {
            
            bdrCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStartCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStop.Visibility = System.Windows.Visibility.Visible;
            WebCamCtrl.StartCapture();

            CaptureTime = new DispatcherTimer();
            CaptureTime.Interval = new TimeSpan(0, 0, 2);
            CaptureTime.Tick += CaptureTime_Tick;
            CaptureTime.Start();

        }

        void CaptureTime_Tick(object sender, EventArgs e)
        {
            CheckBarcode();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            WebCamCtrl.StopCapture();

            bdrCapture.Visibility = System.Windows.Visibility.Hidden;
            bdrStartCapture.Visibility = System.Windows.Visibility.Visible;
            bdrStop.Visibility = System.Windows.Visibility.Hidden;

            CaptureTime.Stop();
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

            CaptureTime.Stop();
        }

        private void CheckBarcode()
        {
            try
            {

                // Take snapshot of webcam image.
               // WebCamCtrl.TakeSnapshot();
                
                var DirInfo = new DirectoryInfo(imgPath);
                String ImageName = (from f in DirInfo.GetFiles()
                                    orderby f.LastWriteTime descending
                                    select f).First().Name.ToString();

                var ListFiles = (from f in DirInfo.GetFiles()
                                    select f).ToList();
                String ReNamed = DateTime.Now.ToString("ddMMMyyyy_hh_mm_ssfff_tt");
                File.Move(imgPath + ImageName, imgPath + "KRAUSGRA" + ReNamed + ".jpeg");
                BitmapSource bs = new BitmapImage(new Uri(imgPath + "KRAUSGRA" + ReNamed + ".jpeg"));
                String NewName =  imgPath + "KRAUSGRA" + ReNamed + ".jpeg";
                String BarcodeRead = Barcode.BarcodeRead.Read(NewName);
                if (BarcodeRead.ToString() != "")
                {
                    MessageBox.Show(BarcodeRead.ToString(), "Barcode Found");
                }
                foreach (var FilesIN in ListFiles)
                {
                    string Full = FilesIN.FullName.ToString();
                    File.Delete(FilesIN.FullName.ToString());
                }
              
              //  File.Delete(NewName);
                //"C:\\BacodeSamples.jpg";
            }
            catch (Exception)
            { }
        }

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);



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
    }
}
