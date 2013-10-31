using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.Views;
using KrausRGA.DBLogics;

namespace KrausRGA.Models
{
    /// <summary>
    /// Avainsh : 30-oct 2013
    /// Model for Entered Number validations, 
    /// also all information about that number.
    /// </summary>
    public static class mReturnDetails
    {
        /// <summary>
        /// Enterd Number.
        /// </summary>
        private static String _SRNumber { get; set; }

        //list of RMA Information.
        public static List<RMAInfo> lsRMAInformation = new List<RMAInfo>();

        /// <summary>
        /// Entered Number Type.
        /// if its PO number then lsRMAInformation will not be null of this class.
        /// </summary>
        /// <param name="SRNumber">
        /// String SRNumber.
        /// </param>
        /// <returns>
        /// enum of NumberType.
        /// </returns>
        public static NumberType EnteredNumberType()
        {
            //Sage Operations class that perform get operations on the sage.
            cmdSageOperations Sage = new cmdSageOperations();

            NumberType _numberType = new NumberType();
            try
            {
                _numberType = NumberType.UnDefined;

                if (_SRNumber.ToUpper().Contains("SR"))
                    _numberType = NumberType.SRNumber;
               else if (_SRNumber.ToUpper().Contains("SH"))
                    _numberType = NumberType.ShipmentNumber;
               else if (_SRNumber.ToUpper().Contains("SO"))
                    _numberType = NumberType.OrderNumber;
                else if (_SRNumber.ToUpper().Contains("DOM"))
                    _numberType = NumberType.VendorNumber;
                else
                {
                    lsRMAInformation = Sage.GetRMAInfoByPONumber(_SRNumber);
                    if (lsRMAInformation.Count() > 0)
                        _numberType = NumberType.PONumber;
                }
            }
            catch (Exception)
            {}

            return _numberType;
 
        }

        /// <summary>
        /// Scanned Number is valid or not. 
        /// this is checked in x3v6 database
        /// lsRMAInformation object is filled if the Number is valid.s;;
        /// also you can add user validation to ented number validate.
        /// </summary>
        /// <param name="SRNumber">
        /// String SRNumber entered
        /// </param>
        /// <returns>
        /// Boolean value true if valid enterd number else false.
        /// </returns>
        public static Boolean IsValidNumberEntred(this String SRNumber)
        {
            Boolean _isNumberValid = false;

            try
            {

            }
            catch (Exception)
            { }

            return _isNumberValid;
        }

        /// <summary>
        /// Get RMA
        /// </summary>
        /// <param name="SRNumber">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static List<RMAInfo> GetRMAInfo(this String SRNumber)
        {
            List<RMAInfo> lsRMAinfo = new List<RMAInfo>();
            try
            {

            }
            catch (Exception)
            { }
            return lsRMAinfo;
        }



    }
}
