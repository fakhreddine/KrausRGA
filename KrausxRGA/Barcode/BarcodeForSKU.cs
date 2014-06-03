using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KrausRGA.Barcode
{
    public class BarcodeForSKU : INotifyPropertyChanged
    {
        private string barcodeValueforSKU;

        /// <summary>
        /// Event to be fired when the barcode value is changed from the previous one.
        /// </summary>
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

        /// <summary>
        /// Property that stores the barcode value.
        /// </summary>
        public string BarcodeValueSKU
        {
            get { return barcodeValueforSKU; }
            set
            {
                if (value != barcodeValueforSKU)
                {
                    barcodeValueforSKU = value;
                    OnPropertyChanged("BarcodeValue");
                }
            }
        }
    }
}
