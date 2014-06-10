using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
    public class SkuReasonIDSequence
    {
        public Guid ReasonID { get; set; }
        public string SKUName { get; set; }
        public int SKU_sequence { get; set; }
    }
}
