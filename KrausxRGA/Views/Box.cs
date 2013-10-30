using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    public class Box
    {
        public int SKUNUmber { get; set; }
        public String BoxNumber { get; set; }
        public String SKUName { get; set; }
        public String ProductName { get; set; }
        public int Quantity { get; set; }
        public Image[] Images { get; set; }
    }
}
