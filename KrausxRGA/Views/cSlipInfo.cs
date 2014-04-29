using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    public class cSlipInfo
    {
        public DateTime ReceivedDate { get; set; }
        public DateTime Expiration { get; set; }
        public string ReceivedBY { get; set; }
        public string  Reason { get; set; }
        public string SRNumber { get; set; }
        public string ProductName { get; set; }
        public string EANCode { get; set; }

        public string RMAStatus { get; set; }
        public string ItemStatus { get; set; }
    }
}
