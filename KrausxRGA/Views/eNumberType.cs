using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{

    /// <summary>
    /// This enum defines the Type of entered Number.
    /// </summary>
    public enum eNumberType
    {
        UnIdefined =0,
        SRNumber = 1,
        OrderNumber=2,
        PONumber,
        ShipmentNumber,
        VendorNumber 
    }
}
