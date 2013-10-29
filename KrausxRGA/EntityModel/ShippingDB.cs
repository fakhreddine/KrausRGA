using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.EntityModel.ShippingManagerFunctions;


namespace KrausRGA.EntityModel
{
    /// <summary>
    /// Avinash : 22 Oct 2013.
    /// Shipping manager database Operations are integrated in this class.
    /// This is static class works as controller to call any function from the database.
    /// </summary>
    public static class ShippingDB
    {
        #region shipping tables
        
        //Shipping Table Operations Class Object.
        public static cmdShipping shippingCMD = new cmdShipping();
        
        #endregion


        public static Shipping GetShippingTbl(String ShipmentNumber)
        {
            return shippingCMD.GetData(ShipmentNumber.ToUpper());
        }

    }
}
