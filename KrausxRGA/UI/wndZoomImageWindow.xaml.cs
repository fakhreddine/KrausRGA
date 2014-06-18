using KrausRGA.Views;
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
    /// Interaction logic for wndZoomImageWindow.xaml
    /// </summary>
    public partial class wndZoomImageWindow : Window
    {
        public wndZoomImageWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            imgzoom.Source = clGlobal.Zoomimages.Source;
        }
    }
}
