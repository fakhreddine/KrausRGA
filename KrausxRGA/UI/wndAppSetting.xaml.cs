using System;
using System.Collections.Generic;
using System.IO;
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
            String[] FontSizes = File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[1].Split(new char[] { '-' });
            String HeaderSize = FontSizes[1];
            String ControlSize = FontSizes[2];
            String VeriableSize = FontSizes[0];

            Resources["FontSize"] = Convert.ToDouble(VeriableSize);
            Resources["HeaderSize"] = Convert.ToDouble(HeaderSize);
            Resources["ContactFontSize"] = Convert.ToDouble(ControlSize);


            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void sldfont_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            String[] FontSizes = File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[1].Split(new char[] { '-' });
            String HeaderSize = FontSizes[1];
            String ControlSize = FontSizes[2];
            String VeriableSize = FontSizes[0];

            Resources["FontSize"] = Convert.ToDouble(VeriableSize) + Convert.ToDouble(sldfont.Value);
            Resources["HeaderSize"] = Convert.ToDouble(HeaderSize) + Convert.ToDouble(sldfont.Value);
            Resources["ContactFontSize"] = Convert.ToDouble(ControlSize) + Convert.ToDouble(sldfont.Value);
        }

        private void btnlogout_Click(object sender, RoutedEventArgs e)
        {
            var msg = MessageBox.Show("Application must be restarted to apply this changes. Do you want to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (msg.ToString() == "Yes")
            {
                File.WriteAllText(Environment.CurrentDirectory + "\\VersionNumber.txt", File.ReadAllText(Environment.CurrentDirectory + "\\VersionNumber.txt").Replace(File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[1].ToString(), Math.Round(Convert.ToDecimal(Resources["FontSize"].ToString()), 0) + "-" + Math.Round(Convert.ToDecimal(Resources["HeaderSize"].ToString()), 0) + "-" + Math.Round(Convert.ToDecimal(Resources["ContactFontSize"].ToString()), 0)));

                String[] FontSizes = File.ReadAllLines(Environment.CurrentDirectory + "\\VersionNumber.txt")[1].Split(new char[] { '-' });
                String HeaderSize = FontSizes[1];
                String ControlSize = FontSizes[2];
                String VeriableSize = FontSizes[0];

                Resources["FontSize"] = Convert.ToDouble(VeriableSize);
                Resources["HeaderSize"] = Convert.ToDouble(HeaderSize);
                Resources["ContactFontSize"] = Convert.ToDouble(ControlSize);

                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
            else
            {
                this.Close();
            }
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            Views.clGlobal.IsUserlogged = true;
            this.Close();
        }

    }
}
