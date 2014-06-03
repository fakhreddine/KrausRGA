using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.Views;
using KrausRGA.EntityModel;
using KrausRGA.DBLogics;
using KrausRGA.ErrorLogger;
using System.Windows.Controls;
using System.Collections;

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

        /// <summary>
        /// Reasons Table Command Object.
        /// </summary>
        protected DBLogics.cmdReasons cRtnreasons = new DBLogics.cmdReasons();

        /// <summary>
        /// Reasoncategory table Command Object
        /// </summary>
        protected DBLogics.cmdReasonCategory crtReasonCategory = new DBLogics.cmdReasonCategory();

        /// <summary>
        /// Transaction table Command object 
        /// </summary>
        protected DBLogics.cmdSKUReasons crtTransaction = new DBLogics.cmdSKUReasons();



        protected DBLogics.cmdReturnedSKUPoints cRetutnedSKUPoints = new DBLogics.cmdReturnedSKUPoints();

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

            //Check that SR Number is persent in database.
            IsAlreadySaved = IsNumberAlreadyPresent(EnteredNumber);


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

        /// <summary>
        /// Check that number is already saved in database.
        /// </summary>
        public Boolean IsAlreadySaved { get; protected set; }

        /// <summary>
        /// Stores Green row from the Grid
        /// </summary>
        public List<int> GreenRowsNumber = new List<int>();


        /// <summary>
        /// Store Return List
        /// </summary>
        private Return _Return = new Return();
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
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/GetEnteredNumberType");
            }

            return _numberType;

        }

        /// <summary>
        /// Scanned Number is valid or not. 
        /// this is checked in x3v6 database
        /// lsRMAInformation object is filled if the Number is valid.
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
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/GetIsValidNumberEntreds");
            }

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


                RAMStatus ram = new RAMStatus();
                ram.ID = 0;
                ram.Status = "Incomplete";

                RAMStatus ram1 = new RAMStatus();
                ram1.ID = 1;
                ram1.Status = "Complete";

                RAMStatus ram3 = new RAMStatus();
                ram3.ID = 2;
                ram3.Status = "Wrong RMA";


                lsReturn.Add(ram);
                lsReturn.Add(ram1);
                lsReturn.Add(ram3);

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/GetRMAStatusList");
            }
            return lsReturn;
        }


        public List<RAMStatus> GetRMADecision()
        {
            List<RAMStatus> lsReturn = new List<RAMStatus>();
            try
            {


                RAMStatus ram = new RAMStatus();
                ram.ID = 0;
                ram.Status = "Pending";

                RAMStatus ram1 = new RAMStatus();
                ram1.ID = 1;
                ram1.Status = "Deny";

                RAMStatus ram3 = new RAMStatus();
                ram3.ID = 2;
                ram3.Status = "Full Refund";

                RAMStatus ram4 = new RAMStatus();
                ram4.ID = 3;
                ram4.Status = "Partial-Refund";



                lsReturn.Add(ram);
                lsReturn.Add(ram1);
                lsReturn.Add(ram3);
                lsReturn.Add(ram4);

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/GetRMAStatusList");
            }
            return lsReturn;
        }

        /// <summary>
        /// Check that SRnumber is already saved in databse or not.
        /// </summary>
        /// <param name="SRnumber">
        /// String SR Number
        /// </param>
        /// <returns>
        /// Boolean Value True is present else false.
        /// </returns>
        public Boolean IsNumberAlreadyPresent(String SRnumber)
        {
            Boolean _return = false;
            //RMA databse Object.
            try
            {
                _Return = new Return(Service.entGet.ReturnByRMANumber(SRnumber));
                String Anyvalue = _Return.RMANumber;
                if (Anyvalue == SRnumber) _return = true;

                //Check Decision is Always new.
              //  IsValidNumber = CanUserOpenThis();
            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/IsNumberAlreadyPresent");
            }
            return _return;

        }

        /// <summary>
        /// Validation for decision is always New.
        /// </summary>
        /// <returns>
        /// return Boolean value.
        /// </returns>
        public Boolean CanUserOpenThis()
        {
            Boolean _flag = IsValidNumber;
            try
            {
                if (_Return.Decision != 0 && _Return.Decision != null)
                    _flag = false;

            }
            catch (Exception)
            { }

            return _flag;
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
        public Guid SetReturnTbl(String ReturnReason, Byte RMAStatus, Byte Decision, Guid CreatedBy, DateTime ScannedDate, DateTime ExpirationDate, string Wrong_RMA_Flg, string Warranty_STA, int Setting_Wty_Days, int ShipDate_ScanDate_Days_Diff)
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
                TblRerutn.ScannedDate = ScannedDate;
                TblRerutn.ExpirationDate = ExpirationDate;
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

                TblRerutn.Wrong_RMA_Flg = Wrong_RMA_Flg;
                TblRerutn.Warranty_STA = Warranty_STA;
                TblRerutn.Setting_Wty_Days = Setting_Wty_Days;
                TblRerutn.ShipDate_ScanDate_Days_Diff = ShipDate_ScanDate_Days_Diff;

                TblRerutn.UpdatedDate = DateTime.Now;

                //On success of transaction it returns id.
                if (cReturnTbl.UpsertReturnTbl(TblRerutn)) _returnID = TblRerutn.ReturnID;

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetReturnTbl");
            }
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
        public Guid SetReturnDetailTbl(Guid ReturnDetailsID, Guid ReturnTblID, String SKUNumber, String ProductName, int DeliveredQty, int ExpectedQty, int ReturnQty, string TK, Guid CreatedBy, string SKU_Status, int SKU_Reason_Total_Points, int IsScanned, int Manually, int NewItemQty, int SKU_Qty_Seq,string ProductID,decimal SalesPrice)
        {
            Guid _ReturnID = Guid.Empty;
            try
            {
                ReturnDetail TblReturnDetails = new ReturnDetail();

                TblReturnDetails.ReturnDetailID = ReturnDetailsID;
                TblReturnDetails.ReturnID = ReturnTblID;
                TblReturnDetails.SKUNumber = SKUNumber;
                TblReturnDetails.ProductName = ProductName;
                TblReturnDetails.DeliveredQty = DeliveredQty;
                TblReturnDetails.ExpectedQty = ExpectedQty;
                TblReturnDetails.TCLCOD_0 = TK;
                TblReturnDetails.ReturnQty = ReturnQty;
                TblReturnDetails.ProductStatus = 0;
                TblReturnDetails.CreatedBy = CreatedBy;
                TblReturnDetails.CreatedDate = DateTime.UtcNow;
                TblReturnDetails.UpadatedDate = DateTime.UtcNow;
                TblReturnDetails.UpdatedBy = CreatedBy;

                TblReturnDetails.SKU_Status = SKU_Status;
                TblReturnDetails.SKU_Reason_Total_Points = SKU_Reason_Total_Points;
                TblReturnDetails.IsSkuScanned = IsScanned;
                TblReturnDetails.IsManuallyAdded = Manually;

                TblReturnDetails.SKU_Sequence = NewItemQty;
                TblReturnDetails.SKU_Qty_Seq = SKU_Qty_Seq;

                TblReturnDetails.SalesPrice = SalesPrice;
                TblReturnDetails.ProductID = ProductID;

                //On Success of transaction.
                if (cRetutnDetailsTbl.UpsetReturnDetail(TblReturnDetails)) _ReturnID = TblReturnDetails.ReturnDetailID;

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetReturnDetailTbl");
            }
            return _ReturnID;
        }

        /// <summary>
        /// Insert new record into ReturnImages table.
        /// </summary>
        /// <param name="ReturnDetailID">
        /// Guid ReturnDetail Table ID.
        /// </param>
        /// <param name="ImagePath">
        /// String Image Path.
        /// </param>
        /// <param name="CreatedBy">
        /// Guid Created By UserID.
        /// </param>
        /// <returns>
        /// Guid of ReturnImageID New inserted Record id. 
        /// </returns>
        public Guid SetReturnedImages(Guid ImageID, Guid ReturnDetailID, String ImagePath, Guid CreatedBy)
        {
            Guid _ReturnID = Guid.Empty;
            try
            {
                ReturnImage RtnImages = new ReturnImage();

                RtnImages.ReturnImageID = ImageID;
                RtnImages.ReturnDetailID = ReturnDetailID;
                RtnImages.SKUImagePath = ImagePath;
                RtnImages.CreatedBy = CreatedBy;
                RtnImages.CreatedDate = DateTime.UtcNow;
                RtnImages.UpadatedBy = CreatedBy;
                RtnImages.UpadatedDate = DateTime.UtcNow;
                if (cRtnImages.UpsertReturnImage(RtnImages)) _ReturnID = RtnImages.ReturnImageID;

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetReturnedImages");
            }
            return _ReturnID;
        }

        #endregion

        public Guid SetReasons(string Reasons)
        {
            Guid _reasonID = Guid.Empty;

            try
            {
                Reason ReasonTable = new Reason();

                ReasonTable.ReasonID = Guid.NewGuid();
                ReasonTable.Reason1 = Reasons;

                if (cRtnreasons.InsertReasons(ReasonTable)) _reasonID = ReasonTable.ReasonID;
            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetReasons");
            }
            return _reasonID;
        }

        public Guid SetReasonCategories(Guid ReasonID, string SKUName)
        {
            Guid _reasonCatID = Guid.Empty;
            try
            {
                String categoryName = lsRMAInformation.FirstOrDefault(Sk => Sk.SKUNumber == SKUName).TCLCOD_0;
                ReasonCategory reasonCat = new ReasonCategory();
                reasonCat.ReasonCatID = Guid.NewGuid();
                reasonCat.ReasonID = ReasonID;
                reasonCat.CategoryName = categoryName;

                if (crtReasonCategory.SetReasonCategory(reasonCat)) _reasonCatID = reasonCat.ReasonCatID;
            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetReasonCategories");
            }
            return _reasonCatID;

        }
        public Guid SetTransaction(Guid SKUReasonID, Guid ReasonID, Guid ReturnDetailID)
        {
            Guid _transationID = Guid.Empty;
            try
            {
                SKUReason tra = new SKUReason();
                tra.SKUReasonID = SKUReasonID;
                tra.ReasonID = ReasonID;
                tra.ReturnDetailID = ReturnDetailID;

                if (crtTransaction.SetTransaction(tra)) _transationID = tra.SKUReasonID;
            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetTransaction");
            }
            return _transationID;
        }
        public List<Reason> GetReasons()
        {
            List<Reason> reasonList = new List<Reason>();

            try
            {
                reasonList = cRtnreasons.GetReasons();
            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/GetReasons");
            }
            return reasonList;
        }


        /// <summary>
        /// Gives the Reaturn reasons of product by its category.
        /// </summary>
        /// <param name="SKUName">
        /// string SKU number.
        /// </param>
        /// <returns>
        /// List of Reasons table.
        /// </returns>
        public List<Reason> GetReasons(String SKUName)
        {
            List<Reason> _lsReasons = new List<Reason>();
            try
            {
                //find category of product.
                String CategoryOFSKU = lsRMAInformation.FirstOrDefault(Sk => Sk.SKUNumber == SKUName).TCLCOD_0;
                _lsReasons = cRtnreasons.GetReasons(CategoryOFSKU);
            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/GetReasons(String SKUName)");
            }
            return _lsReasons;
        }

        public Boolean DeleteReturnDetails(Guid ReturnDetailsID)
        {
            return cRetutnDetailsTbl.DeleteReturnDetails(ReturnDetailsID);
        }


        public List<cSlipInfo> GetSlipInfo(string SkuNumber,String EANCode, String ReturnReasons,DateTime ScannedDate, DateTime ExpirationDate,string RMAStatus,string ItemStatus)
        {
            List<cSlipInfo> _lsslipinfo = new List<cSlipInfo>();
            try
            {
                cSlipInfo slip = new cSlipInfo();
                slip.ProductName = SkuNumber;
                slip.Reason = ReturnReasons;
                slip.ReceivedBY = clGlobal.mCurrentUser.UserInfo.UserName;
                slip.ReceivedDate = ScannedDate;
                slip.Expiration = ExpirationDate;
                slip.SRNumber = lsRMAInformation[0].RMANumber;
                slip.EANCode = EANCode;
                slip.RMAStatus = RMAStatus;
                slip.ItemStatus = ItemStatus;
                _lsslipinfo.Add(slip);
            }
            catch (Exception)
            {

            }
            return _lsslipinfo;
        }

        public String GetENACodeByItem(string ItemName)
        {
            string ENACode = "";
            try
            {
                ENACode = cSage.GetEANCode(ItemName);
            }
            catch (Exception)
            {
            }
            return ENACode;
        
        }

        public String GetSKUNameByItem(string code)
        {
            string SKU = "";
            try
            {
                SKU = cSage.GetPruductNameByEANCode(code);
            }
            catch (Exception)
            {
            }
            return SKU;

        }
        public String GetSKUNameAndProductNameByItem(string code)
        {
            string SKU = "";
            try
            {
                SKU = cSage.GetPruductNameAndProductIDByEANCode(code);
            }
            catch (Exception)
            {
            }
            return SKU;

        }




        public String GetReasonsByRDetail(Guid ReturnDetailID)
        {
            string Reasons = "";
            try
            {
                Reasons = cRtnreasons.GetReasonsByReturnDetailID(ReturnDetailID);
            }
            catch (Exception)
            {
            }
            return Reasons;
        }

        public String GetSageReasonBySKUSR(String SRNumber,String SKUNumber)
        {
            string SageReasons = "";
            try
            {
                 SageReasons = cSage.GetSageReason(SRNumber, SKUNumber);
                 if (SageReasons.Trim()=="")
                 {
                     SageReasons = "N/A";  
                 }
            }
            catch (Exception)
            {
            }
            return SageReasons;
        }


        public Guid SetReturnedSKUPoints(Guid ReturnedSKUID, Guid ReturnDetailsID, Guid ReturnTblID, String SKU, String Reason, string Reason_Value, int Points,int skusequence)
        {
            Guid _ReturnedskuID = Guid.Empty;
            try
            {
                ReturnedSKUPoints TblReturnedSKUPoints = new ReturnedSKUPoints();

                TblReturnedSKUPoints.ID = ReturnedSKUID;
                TblReturnedSKUPoints.ReturnDetailID = ReturnDetailsID;
                TblReturnedSKUPoints.ReturnID = ReturnTblID;
                TblReturnedSKUPoints.SKU = SKU;
                TblReturnedSKUPoints.Reason = Reason;
                TblReturnedSKUPoints.Reason_Value = Reason_Value;
                TblReturnedSKUPoints.Points = Points;
                TblReturnedSKUPoints.SkuSequence = skusequence;
            

                //On Success of transaction.
                if (cRetutnedSKUPoints.UpsertReturnedSKUPoints(TblReturnedSKUPoints)) _ReturnedskuID = TblReturnedSKUPoints.ID;

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnedSKUPoints/SetReturnedSKUPoints");
            }
            return _ReturnedskuID;
        }

    }

    


    



    public class RAMStatus
    {
        public int ID { get; set; }
        public String Status { get; set; }
    }
}
