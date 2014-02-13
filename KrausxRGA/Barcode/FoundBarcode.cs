using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KrausRGA.Barcode
{
    /// <summary>
    /// Avinash : 14 Feb Kraus RGA project.
    /// if barcode found need some event to be triggered so this class 
    /// provides the notification to show that the barcode in the image is found 
    /// class is joined with the output of the scannig class to change the values of the barcode
    /// this class alon can not scan the barcode from the image.
    /// </summary>
    public class FoundBarcode : INotifyPropertyChanged
    {
        private string barcodeValue;

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
