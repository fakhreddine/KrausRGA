using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.Views;
using KrausRGA.EntityModel;

namespace KrausRGA.Models
{
    /// <summary>
    /// Avainsh : 30-oct 2013 :Kraus GRA.
    /// Model for Entered Number validations, 
    /// also all information about that number.
    /// class properties are Auto-set when Constructor is called.
    /// </summary>
    public class mReturnDetails
    {
        #region Declarations.

        /// <summary>
        /// sage operations command class object.
        /// </summary>
        protected DBLogics.cmdSageOperations cSage = new DBLogics.cmdSageOperations();

        /// <summary>
        /// Return Tables command class.
        /// </summary>
        protected DBLogics.cmdReturn cReturnTbl = new DBLogics.cmdReturn();

        /// <summary>
        ///ReturnDetails Tables commad class object.
        /// </summary>
        protected DBLogics.cmdReturnDetail cRetutnDetailsTbl = new DBLogics.cmdReturnDetail();

        /// <summary>
        /// RetutnImages Table Command class Object.
        /// </summary>
        protected DBLogics.cmdReturnImages cRtnImages = new DBLogics.cmdReturnImages();


        #endregion

        #region Class Contructors

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
        public eNumberType EnumNumberType { get; protected set; }

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
        public eNumberType GetEnteredNumberType(String Number)
        {
            eNumberType _numberType = new eNumberType();
            try
            {
                _numberType = eNumberType.UnIdefined;

                if (Number.ToUpper().Contains("SR"))
                    _numberType = eNumberType.SRNumber;
                else if (Number.ToUpper().Contains("SH"))
                    _numberType = eNumberType.ShipmentNumber;
                else if (Number.ToUpper().Contains("SO"))
                    _numberType = eNumberType.OrderNumber;
                else if (Number.ToUpper().Contains("DOM"))
                    _numberType = eNumberType.VendorNumber;
                else
                {
                    lsRMAInformation = cSage.GetRMAInfoByPONumber(Number);
                    if (lsRMAInformation.Count() > 0)
                        _numberType = eNumberType.PONumber;
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
        public Boolean GetIsValidNumberEntred(String Number, eNumberType enumNumberType)
        {
            
            Boolean _isNumberValid = false;

            try
            {
                switch (enumNumberType)
                {
                    //Order Number Case.
                    case eNumberType.OrderNumber:
                        lsRMAInformation = cSage.GetRMAInfoBySONumber(Number);
                        if (lsRMAInformation.Count() > 0)
                            _isNumberValid = true;
                        break;

                    //SR Number Case.
                    case eNumberType.SRNumber:
                        lsRMAInformation = cSage.GetRMAInfoBySRNumber(Number);
                        if (lsRMAInformation.Count() > 0)
                            _isNumberValid = true;
                        break;

                    //Shipment Number case.
                    case eNumberType.ShipmentNumber:
                        lsRMAInformation = cSage.GetRMAInfoByShipmentNumber(Number);
                        if (lsRMAInformation.Count() > 0)
                            _isNumberValid = true;
                        break;

                    //PO Number Case. no need to set lsRMAInformation. is set when PO Number validation check
                    case eNumberType.PONumber:
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

        /// <summary>
        /// create shallow copy of current class;
        /// </summary>
        /// <returns>
        /// new object with shallow copy.
        /// </returns>
        public mReturnDetails GetShollowCopy()
        {
            return (mReturnDetails)this.MemberwiseClone();
        }

        /// <summary>
        /// Temparary function 
        /// to fill combo box RMA Status and RMA Decision.
        /// </summary>
        /// <returns>
        /// list Status.
        /// </returns>
        public List<RAMStatus> GetRMAStatusList()
        {
            List<RAMStatus> lsReturn = new List<RAMStatus>();
            try
            {
                RAMStatus ram2 = new RAMStatus();
                ram2.ID = -1;
                ram2.Status = "-- Select --";

                RAMStatus ram = new RAMStatus();
                ram.ID = 0;
                ram.Status = "Approved";

                RAMStatus ram1 = new RAMStatus();
                ram1.ID = 1;
                ram1.Status = "Pending";

                RAMStatus ram3 = new RAMStatus();
                ram3.ID = 1;
                ram3.Status = "Canceled";

                lsReturn.Add(ram2);
                lsReturn.Add(ram);
                lsReturn.Add(ram1);
                lsReturn.Add(ram3);

            }
            catch (Exception)
            { }
            return lsReturn;
        }

        #endregion

        #region Set Methods of Database.

        /// <summary>
        /// Insert data To the Return Master table. uses current object from model class to fill RMA information.
        /// (RMA Information ex. RMANumber, ShippingNumber, VendorName, CustomerName.....etc.)
        /// combine with passed parameters.
        /// </summary>
        /// <param name="ReturnReason">
        /// String Return reason.
        /// </param>
        /// <param name="RMAStatus">
        /// Byte RMA Status.
        /// </param>
        /// <param name="Decision">
        /// Byte Decision.
        /// </param>
        /// <returns>
        /// Guid RetutnID that is inserted or updated on transaction filure it return empty Guid.
        /// </returns>
        public Guid SetReturnTbl(String ReturnReason, Byte RMAStatus, Byte Decision, Guid CreatedBy)
        {
            Guid _returnID = Guid.Empty;
            try
            {
                //Return table new object.
                Return TblRerutn = new Return();

                TblRerutn.ReturnID = Guid.NewGuid();
                TblRerutn.RMANumber = lsRMAInformation[0].RMANumber;
                TblRerutn.ShipmentNumber = lsRMAInformation[0].ShipmentNumber;
                TblRerutn.OrderNumber = lsRMAInformation[0].OrderNumber;
                TblRerutn.PONumber = lsRMAInformation[0].PONumber;
                TblRerutn.OrderDate = lsRMAInformation[0].OrderDate;
                TblRerutn.DeliveryDate = lsRMAInformation[0].DeliveryDate;
                TblRerutn.ReturnDate = lsRMAInformation[0].ReturnDate;
                TblRerutn.VendorNumber = lsRMAInformation[0].VendorNumber;
                TblRerutn.VendoeName = lsRMAInformation[0].VendorName;
                TblRerutn.CustomerName1 = lsRMAInformation[0].CustomerName1;
                TblRerutn.CustomerName2 = lsRMAInformation[0].CustomerName2;
                TblRerutn.Address1 = lsRMAInformation[0].Address1;
                TblRerutn.Address2 = lsRMAInformation[0].Address2;
                TblRerutn.Address3 = lsRMAInformation[0].Address3;
                TblRerutn.ZipCode = lsRMAInformation[0].ZipCode;
                TblRerutn.City = lsRMAInformation[0].City;
                TblRerutn.State = lsRMAInformation[0].State;
                TblRerutn.Country = lsRMAInformation[0].Country;
                TblRerutn.ReturnReason = ReturnReason;
                TblRerutn.RMAStatus = RMAStatus;
                TblRerutn.Decision = Decision;
                TblRerutn.CreatedBy = CreatedBy;
                TblRerutn.CreatedDate = DateTime.UtcNow;
                TblRerutn.UpdatedBy = null;
                TblRerutn.UpdatedDate = null;

                //On success of transaction it returns id.
                if (cReturnTbl.UpsertReturnTbl(TblRerutn)) _returnID = TblRerutn.ReturnID;

            }
            catch (Exception)
            { }
            return _returnID;
        }

        /// <summary>
        /// Insert new record in to ReturnDetail Table.
        /// </summary>
        /// <param name="ReturnTblID">
        /// Guid Return Master Table Id.
        /// </param>
        /// <param name="SKUNumber">
        /// Strng SKUNumber.
        /// </param>
        /// <param name="ProductName">
        /// String Product Name.
        /// </param>
        /// <param name="DeliveredQty">
        /// int Delivered Quantity.
        /// </param>
        /// <param name="ExpectedQty">
        /// Int Expected Quantity.
        /// </param>
        /// <param name="ReturnQty">
        /// int Returned Quantity.
        /// </param>
        /// <param name="ProductStatus">
        /// int Product Status.
        /// </param>
        /// <param name="CreatedBy">
        /// Guid Created By User ID.
        /// </param>
        /// <returns>
        /// Guild new ReturnDetailID
        /// </returns>
        public Guid SetReturnDetailTbl(Guid ReturnTblID, String SKUNumber, String ProductName, int DeliveredQty, int ExpectedQty, int ReturnQty, int ProductStatus,Guid CreatedBy)
        {
            Guid _ReturnID = Guid.Empty;
            try
            {
                ReturnDetail TblReturnDetails = new ReturnDetail();

                TblReturnDetails.ReturnDetailID = Guid.NewGuid();
                TblReturnDetails.ReturnID = ReturnTblID;
                TblReturnDetails.SKUNumber = SKUNumber;
                TblReturnDetails.ProductName = ProductName;
                TblReturnDetails.DeliveredQty = DeliveredQty;
                TblReturnDetails.ExpectedQty = ExpectedQty;
                TblReturnDetails.ReturnQty = ReturnQty;
                TblReturnDetails.ProductStatus = ProductStatus;
                TblReturnDetails.CreatedBy = CreatedBy;
                TblReturnDetails.CreatedDate = DateTime.UtcNow;
                TblReturnDetails.UpdatedBy = null;
                TblReturnDetails.UpadatedDate = null;
                
                //On Success of transaction.
                if (cRetutnDetailsTbl.UpsetReturnDetail(TblReturnDetails)) _ReturnID = TblReturnDetails.ReturnDetailID;

            }
            catch (Exception)
            { }
            return _ReturnID;
        }

        public Guid SetReturnedImages(Guid ReturnDetailID, String ImagePath)
        {
            Guid _ReturnID = Guid.Empty;
            try
            {

            }
            catch (Exception)
            {}
            return _ReturnID;
        }

        #endregion
    }

    public class RAMStatus
    {
        public int ID { get; set; }
        public String Status { get; set; }
    }
}
