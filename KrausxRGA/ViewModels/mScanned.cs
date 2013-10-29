using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;


namespace KrausRGA.ViewModels
{
   
    /// <summary>
    /// Avinash-  Date : Oct 22, 2013.
    /// Model For scanned value search and its related functions
    /// </summary>
    public class mScanned
    {
        //Shipping Manager entity Object.
        Shipping_ManagerEntities entShipping = new Shipping_ManagerEntities();

        public static string ScannedNumber { get; set; }
        public static Shipping ShippingTblInfo = null;

        /// <summary>
        /// Search value in database and Rerurn Enum of ScannedValueType 
        /// that contails Value Types.
        /// </summary>
        /// <param name="ScannedValue">
        /// String Scanned value Type
        /// </param>
        /// <returns>
        /// String Enum of ScannedValueType.
        /// </returns>
        public String SearchValueIntoDatabase(String ScannedValue)
        {
            String _dbFoundValue = GRAEnum.ScannedValueType.NoMatch.ToString();
            try
            {

            }
            catch (Exception)
            { }
            return _dbFoundValue;
        }


    }
}
