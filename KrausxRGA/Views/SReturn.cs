using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    /// <summary>
    /// Avinash : 31OCt2013
    /// Return Main module for return goods.
    /// </summary>
   public class SReturn
    {
        public String SRNumber{ get; set; }
        public DateTime ReturnDate { get; set; }
        public String CustomerName { get; set; }
        public String CustomerAddress { get; set; }
        public String CustomerCity { get; set; }
        public String CustomerState { set; get; }
        public String CustomerCountry { set; get; }
        public String CustomerZipCode { set; get; }
        public String VendonderNumber { get; set; }
        public String VendonderName { get; set; }
        public String PONumber { get; set; }
        public String TrackingNumbers { set; get; }
        public String RMAStatus { set; get; }
        public String Decision { get; set; }
    }
}
