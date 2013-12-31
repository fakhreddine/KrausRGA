using KrausRGA.DBLogics;
using KrausRGA.VersionCheck;
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
            Service.entGet.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(KrausRGA.Properties.Settings.Default.GetServicePath.ToString()), Service.entGet.Endpoint.Address.Identity, Service.entGet.Endpoint.Address.Headers);
            Service.entGet.Open();
            Service.entSave.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(KrausRGA.Properties.Settings.Default.SetServicePath.ToString()), Service.entGet.Endpoint.Address.Identity, Service.entGet.Endpoint.Address.Headers);
            Service.entSave.Open();

            #endregion

            #region Application Start From another Application.
            //String[] LoginParametrs = e.Args;
            //if (LoginParametrs.Count() > 0)
            //{
            //    StartupParametrs.UserName = LoginParametrs[0];
            //} 
            #endregion

            #region Update Version 

            String _appVersion = ApplicationDeployment.IsNetworkDeployed
                   ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                   : Assembly.GetExecutingAssembly().GetName().Version.ToString();
            String DBVersionNumber = Service.entGet.GetRMALatestVersionNumber();
            if (_appVersion != DBVersionNumber)
            {
                DBVersionNumber = DBVersionNumber.Replace('.', '_');
                try { Directory.Delete(Environment.CurrentDirectory + "\\RGA\\", true); }
                catch (Exception) { }
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\RGA\\");
                GetFileNames.Url = "http://192.168.5.66/RGAVersions/";
                foreach (String Sitem in GetFileNames.ListDiractory())
                {
                    GetFileNames.downloadFromFTP(Sitem, Environment.CurrentDirectory + "\\RGA\\");
                    if (Sitem.Contains(".txt"))
                    {
                        File.Move(Environment.CurrentDirectory + "\\RGA\\" + Sitem, Environment.CurrentDirectory + "\\RGA\\" + Sitem.Replace(".txt", ""));
                    }

                }
                String DirPath = Environment.CurrentDirectory + "\\RGA\\";
                System.Diagnostics.ProcessStartInfo RgaApplication = new System.Diagnostics.ProcessStartInfo();
                RgaApplication.FileName = DirPath + "KrausRGA.exe";
                RgaApplication.Verb = "runas";
                RgaApplication.WorkingDirectory = DirPath;
                RgaApplication.UseShellExecute = true;
                System.Diagnostics.Process.Start(RgaApplication);
                this.Shutdown();
            #endregion
            }
        }
    }
}
