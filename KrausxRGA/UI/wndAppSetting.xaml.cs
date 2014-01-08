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

namespace KrausRGA.UI
{
    /// <summary>
    /// Interaction logic for wndAppSetting.xaml
    /// </summary>
    public partial class wndAppSetting : Window
    {
        public wndAppSetting()
        {
            //String[] FontSizes = Properties.Settings.Default.fontsize_headersiz_constantsize.ToString().Split(new char[] { '_' });
            //String HeaderSize = FontSizes[1];
            //String ControlSize = FontSizes[2];
            //String VeriableSize = FontSizes[0];

            //Resources["FontSize"] = Convert.ToDouble(VeriableSize);
            //Resources["HeaderSize"] = Convert.ToDouble(HeaderSize);
            //Resources["ContactFontSize"] = Convert.ToDouble(ControlSize);


            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void sldfont_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Resources["FontSize"] = 22 + Convert.ToDouble(sldfont.Value);
            Resources["HeaderSize"] = 45 + Convert.ToDouble(sldfont.Value);
            Resources["ContactFontSize"] = 20 + Convert.ToDouble(sldfont.Value);
        }

        private void btnlogout_Click(object sender, RoutedEventArgs e)
        {
            
            //Properties.Settings.Default.fontsize_headersiz_constantsize = Resources["FontSize"].ToString() + "_" + Resources["HeaderSize"].ToString() + "_" + Resources["ContactFontSize"].ToString();
            //Properties.Settings.Default.Save();
            //String[] FontSizes = Properties.Settings.Default.fontsize_headersiz_constantsize.ToString().Split(new char[] { '_' });
            //String HeaderSize = FontSizes[1];
            //String ControlSize = FontSizes[2];
            //String VeriableSize = FontSizes[0];

            //Resources["FontSize"] =Convert.ToDouble( VeriableSize);
            //Resources["HeaderSize"] =Convert.ToDouble( HeaderSize);
            //Resources["ContactFontSize"] =Convert.ToDouble( ControlSize);

         var msg= MessageBox.Show("You must Restart Application", "Warning", MessageBoxButton.YesNo,MessageBoxImage.Warning);

         if (msg.ToString() == "Yes")
         {
             System.Windows.Forms.Application.Restart();
             Application.Current.Shutdown();
         }
         else
         {
             this.Close();
         }
        }
    }
}
