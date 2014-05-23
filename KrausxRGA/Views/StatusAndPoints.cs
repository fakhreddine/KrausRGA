using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    public class StatusAndPoints
    {
        public string SKUName { get; set; }
        public string  Status { get; set; }
        public int Points { get; set; }
        public int IsScanned { get; set; }
        public int IsMannually { get; set; }
        public int NewItemQuantity { get; set; }
        public int skusequence { get; set; }
    }
}
