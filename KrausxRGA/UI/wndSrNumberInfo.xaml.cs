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
namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndSrNumberInfo.xaml
    /// </summary>
    public partial class wndSrNumberInfo : Window 
    {
        public wndSrNumberInfo()
        {
            InitializeComponent();
        }

        private void bdrButtonTemp_Loaded(object sender, RoutedEventArgs e)
        {



            //Remove this Button from UI.
            btnTemp.Focus();
           
        }
    }
}