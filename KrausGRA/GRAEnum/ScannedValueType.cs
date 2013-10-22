using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausGRA.GRAEnum
{
    public enum ScannedValueType
    {
        NoMatch=0,
        BOXNUM = 1,
        ShippingNumber =2,
        PackingNumber,
        OrderNumber,
    }
}
