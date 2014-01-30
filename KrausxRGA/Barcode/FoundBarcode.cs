using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KrausRGA.Barcode
{
    public class FoundBarcode : INotifyPropertyChanged
    {
        private string barcodeValue;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public string BarcodeValue
        {
            get { return barcodeValue; }
            set
            {
                if (value != barcodeValue)
                {
                    barcodeValue = value;
                    OnPropertyChanged("BarcodeValue");
                }
            }
        }
    }
}
