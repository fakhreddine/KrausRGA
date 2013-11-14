using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    /// <summary>
    /// Avinash: 31Oct 2013
    /// Returned RMA Information.
    /// </summary>
    public class RMAInfo
    {
        public String RMANumber { get; set; }
        public String ShipmentNumber { get; set; }
        public String OrderNumber { get; set; }
        public String PONumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public String VendorNumber { get; set; }
        public String VendorName { get; set; }
        public String SKUNumber { get; set; }
        public String ProductName { get; set; }
        public int DeliveredQty { get; set; }
        public int ExpectedQty { get; set; }
        public int ReturnedQty { get; set; }
        public String CustomerName1 { get; set; }
        public String CustomerName2 { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Address3 { get; set; }
        public String ZipCode { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Country { get; set; }
    }
}
