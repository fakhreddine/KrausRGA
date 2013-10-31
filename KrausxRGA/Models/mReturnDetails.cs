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
    public class mReturnDetails
    {  
       

        #region Class Contructor

        /// <summary>
        /// Class Constructor. 
        /// calls all methods when that finds 
        /// Valid Eetered number, RMA Information of number.
        /// and Type of the Number as enum NumberType.
        /// </summary>
        /// <param name="SRNumber">
        /// String Number To be check.
        /// </param>
        public mReturnDetails(String ScannedNumber)
        {
            //set entered Number Property of class.
            EnteredNumber = ScannedNumber;

            //Find Type of enum entered number.
            EnumNumberType = GetEnteredNumberType(EnteredNumber);

            //Find valid Number or not.
            IsValidNumber = GetIsValidNumberEntred(EnteredNumber, EnumNumberType);

        }

        #endregion

        #region class Properties

        /// <summary>
        /// String SR Number Which is Valid.
        /// </summary>
        public String EnteredNumber { get; protected set; }

        /// <summary>
        /// Type Of Entred Number. 
        /// </summary>
        public NumberType EnumNumberType { get; protected set; }

        /// <summary>
        /// Entered Number Is valid Or Not.
        /// </summary>
        public Boolean IsValidNumber { get; protected set; }

        /// <summary>
        /// List Of information of RMA details.
        /// </summary>
        public List<RMAInfo> lsRMAInformation { get; protected set; }

        #endregion

        #region Member Functions of class.
        
        /// <summary>
        /// Entered Number Type.
        /// if its PO number then lsRMAInformation will not be null of this class.
        /// </summary>
        /// <param name="Number">
        /// String SRNumber.
        /// </param>
        /// <returns>
        /// enum of NumberType.
        /// </returns>
        public NumberType GetEnteredNumberType(String Number)
        {
            cmdSageOperations Sage = new cmdSageOperations();

            NumberType _numberType = new NumberType();
            try
            {
                _numberType = NumberType.UnIdefined;

                if (Number.ToUpper().Contains("SR"))
                    _numberType = NumberType.SRNumber;
                else if (Number.ToUpper().Contains("SH"))
                    _numberType = NumberType.ShipmentNumber;
                else if (Number.ToUpper().Contains("SO"))
                    _numberType = NumberType.OrderNumber;
                else if (Number.ToUpper().Contains("DOM"))
                    _numberType = NumberType.VendorNumber;
                else
                {
                    lsRMAInformation = Sage.GetRMAInfoByPONumber(Number);
                    if (lsRMAInformation.Count() > 0)
                        _numberType = NumberType.PONumber;
                }
            }
            catch (Exception)
            { }

            return _numberType;

        }

        /// <summary>
        /// Scanned Number is valid or not. 
        /// this is checked in x3v6 database
        /// lsRMAInformation object is filled if the Number is valid.s;;
        /// also you can add user validation to ented number validate.
        /// </summary>
        /// <param name="Number">
        /// String SRNumber entered
        /// </param>
        /// <returns>
        /// Boolean value true if valid enterd number else false.
        /// </returns>
        public Boolean GetIsValidNumberEntred(String Number, NumberType enumNumberType)
        {
            cmdSageOperations Sage = new cmdSageOperations();

            Boolean _isNumberValid = false;

            try
            {
                switch (enumNumberType)
                {
                        //Order Number Case.
                    case NumberType.OrderNumber:
                        lsRMAInformation = Sage.GetRMAInfoBySONumber(Number);
                        if (lsRMAInformation.Count() > 0)
                            _isNumberValid = true;
                        break;

                        //SR Number Case.
                    case NumberType.SRNumber:
                        lsRMAInformation = Sage.GetRMAInfoBySRNumber(Number);
                        if (lsRMAInformation.Count() > 0)
                            _isNumberValid = true;
                        break;

                        //Shipment Number case.
                    case NumberType.ShipmentNumber:
                        lsRMAInformation = Sage.GetRMAInfoByShipmentNumber(Number);
                        if (lsRMAInformation.Count() > 0)
                            _isNumberValid = true;
                        break;

                        //PO Number Case. no need to set lsRMAInformation. is set when PO Number validation check
                    case NumberType.PONumber:
                        _isNumberValid = true;
                        break;

                        //Default Number case. also UnIdentified case.
                    default:
                        _isNumberValid = false;
                        break;
                }

            }
            catch (Exception)
            { }

            return _isNumberValid;
        }

        #endregion
    }
}
