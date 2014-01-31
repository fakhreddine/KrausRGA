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
            //Service.entGet.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(KrausRGA.Properties.Settings.Default.GetServicePath.ToString()), Service.entGet.Endpoint.Address.Identity, Service.entGet.Endpoint.Address.Headers);
            //Service.entGet.Open();
            //Service.entSave.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(KrausRGA.Properties.Settings.Default.SetServicePath.ToString()), Service.entGet.Endpoint.Address.Identity, Service.entGet.Endpoint.Address.Headers);
            //Service.entSave.Open();
            #endregion

            #region Application Start From another Application.
            //String[] LoginParametrs = e.Args;
            //if (LoginParametrs.Count() > 0)
            //{
            //    StartupParametrs.UserName = LoginParametrs[0];
            //} 
            #endregion

            #region Update Version

            String _appVersion = File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[0];

            String DBVersionNumber = _appVersion;
            try
            {

                DBVersionNumber = Service.entGet.GetRMALatestVersionNumber();

                //Replace current text to new Database number.
                File.WriteAllText(Environment.CurrentDirectory + "\\VersionNumber.txt", File.ReadAllText(Environment.CurrentDirectory + "\\VersionNumber.txt").Replace(_appVersion, DBVersionNumber));

                //if (_appVersion != DBVersionNumber)
                //{
                //    String DirPath = Environment.CurrentDirectory;
                //    System.Diagnostics.ProcessStartInfo RgaApplication = new System.Diagnostics.ProcessStartInfo();
                //    RgaApplication.FileName = DirPath + "\\RGA.exe";
                //    RgaApplication.Verb = "runas";
                //    RgaApplication.WorkingDirectory = DirPath;
                //    RgaApplication.UseShellExecute = true;
                //    System.Diagnostics.Process.Start(RgaApplication);
                //    this.Shutdown();
                //}
            }
            catch (Exception)
            { }


            #endregion
        }
    }
}
