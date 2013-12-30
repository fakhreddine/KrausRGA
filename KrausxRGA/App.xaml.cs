using KrausRGA.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
            String[] LoginParametrs = e.Args;
            if (LoginParametrs.Count() > 0)
            {
                StartupParametrs.UserName = LoginParametrs[0];
            }
        }
    }
}
