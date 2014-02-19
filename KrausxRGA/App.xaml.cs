using KrausRGA.DBLogics;
//using KrausRGA.VersionCheck;
using KrausRGA.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using KrausRGA.Views;
namespace KrausRGA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Add this method override
        protected override void OnStartup(StartupEventArgs e)
        {
            

            #region Initailise servies
            Service.entGet = new GetRMAServiceRef.GetClient();
            Service.entSave = new SaveRMAServiceRefer.SaveClient();
            Service.entDelete = new DeleteRMAServiceRef.DeleteClient();
            //Service.entGet.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(KrausRGA.Properties.Settings.Default.GetServicePath.ToString()), Service.entGet.Endpoint.Address.Identity, Service.entGet.Endpoint.Address.Headers);
            //Service.entGet.Open();
            //Service.entSave.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(KrausRGA.Properties.Settings.Default.SetServicePath.ToString()), Service.entGet.Endpoint.Address.Identity, Service.entGet.Endpoint.Address.Headers);
            //Service.entSave.Open();
            #endregion

            #region Map Images Drive

            //Map network drive to the local system.
            MapDrive();


            #endregion

            #region Update Version

            String _appVersion = File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[0];

            String DBVersionNumber = _appVersion;
            try
            {

                DBVersionNumber = Service.entGet.GetRMALatestVersionNumber();

                //Replace current text to new Database number.
                File.WriteAllText(Environment.CurrentDirectory + "\\VersionNumber.txt", File.ReadAllText(Environment.CurrentDirectory + "\\VersionNumber.txt").Replace(_appVersion, DBVersionNumber));

                if (_appVersion != DBVersionNumber)
                {
                    String DirPath = Environment.CurrentDirectory;
                    System.Diagnostics.ProcessStartInfo RgaApplication = new System.Diagnostics.ProcessStartInfo();
                    RgaApplication.FileName = DirPath + "\\RGA.exe";
                    RgaApplication.Verb = "runas";
                    RgaApplication.WorkingDirectory = DirPath;
                    RgaApplication.UseShellExecute = true;
                    System.Diagnostics.Process.Start(RgaApplication);
                    this.Shutdown();
                }
            }
            catch (Exception)
            { }


            #endregion

           
        }


        /// <summary>
        /// Map the network drive to the System.
        /// </summary>
        private void MapDrive()
        {
            try
            {
                NetworkDrive drive = new NetworkDrive();
                drive.Force = true;
                drive.LocalDrive = "X:";
                drive.ShareName = @"\\192.168.1.172\Macintosh HD\ftp_share\RGAImages";
                drive.SaveCredentials = true;
                drive.MapDrive("mediaserver", "kraus2013");
            }
            catch (Exception)
            {}

        }
    }
}
