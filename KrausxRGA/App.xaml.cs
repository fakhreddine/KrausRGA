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
            #region Application Start From another Application.
            //String[] LoginParametrs = e.Args;
            //if (LoginParametrs.Count() > 0)
            //{
            //    StartupParametrs.UserName = LoginParametrs[0];
            //} 
            #endregion


            String s = ApplicationDeployment.IsNetworkDeployed
                   ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                   : Assembly.GetExecutingAssembly().GetName().Version.ToString();
             s = s.Replace('.', '_');
             try { Directory.Delete(Environment.CurrentDirectory + "\\RGA\\", true); }
             catch(Exception) { }
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\RGA\\");
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\RGA\\Application Files\\");
            Directory.CreateDirectory(Environment.CurrentDirectory + "\\RGA\\Application Files\\KrausRGA_" + s + "\\");
            GetFileNames.Url = "http://wc.kraus.net/RGAUpdates/";
            foreach (String Sitem in GetFileNames.ListDiractory())
            {
                GetFileNames.downloadFromFTP(Sitem, Environment.CurrentDirectory + "\\RGA\\");
            }

            GetFileNames.Url = "http://wc.kraus.net/RGAUpdates/Application%20Files/KrausRGA_" + s + "/";
            foreach (String Sitem in GetFileNames.ListDiractory())
            {
                GetFileNames.downloadFromFTP(Sitem, Environment.CurrentDirectory + "\\RGA\\Application Files\\KrausRGA_" + s + "\\");
            }
                        
        }
    }
}
