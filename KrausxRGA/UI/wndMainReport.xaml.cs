using KrausRGA.Models;
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
    /// Interaction logic for wndMainReport.xaml
    /// </summary>
    public partial class wndMainReport : Window
    {
        mPOnumberRMA _mponumner = new mPOnumberRMA();

        mupdatedForPonumber forgetdata = new mupdatedForPonumber();

        mUpdateModeRMA forSRnumber = new mUpdateModeRMA();

        public wndMainReport()
        {
            InitializeComponent();
            _reportViewer.Load += ReportViewer_Load;
        }
        private bool _isReportViewerLoaded;

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            if (!_isReportViewerLoaded)
            {
                var retunbyrow = _mponumner.GetReturnByRowID(clGlobal.NewRGANumber)[0];

                if (retunbyrow.RMANumber == "N/A")
                {
                    forgetdata = new mupdatedForPonumber(retunbyrow.PONumber);

                    Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    //forgetdata._lsReturnDetails1 dataset = new  forgetdata._lsReturnDetails1();


                   // dataset.BeginInit();
                    reportDataSource1.Name = "DataSet1";
                    //Name of the report dataset in our .RDLC file
                    reportDataSource1.Value = forgetdata._lsReturnDetails1;
                    this._reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                    this._reportViewer.LocalReport.ReportPath = "~/KrausRGA/KrausxRGA/UI/MainReport.rdlc";
                   // forgetdata._lsReturnDetails1.EndInit();
                    //fill data into WpfApplication4DataSet
                    //WpfApplication4DataSetTableAdapters.AccountsTableAdapter accountsTableAdapter = new WpfApplication4DataSetTableAdapters.AccountsTableAdapter();

                    //   .ReturnDetailsDTO accountsTableAdapter = new GetRMAServiceRef.ReturnDetailsDTO();

                    //accountsTableAdapter.ClearBeforeFill = true;
                    //accountsTableAdapter.Fill(forgetdata._lsReturnDetails1);
                    _reportViewer.RefreshReport();
                    _isReportViewerLoaded = true;
          
                }
                else
                {
                    this.Dispatcher.Invoke(new Action(() => { forSRnumber = new mUpdateModeRMA(retunbyrow.RMANumber); }));

                  

                }






               
            }
        }
    }
}
