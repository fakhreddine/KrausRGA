using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.Views;
using KrausRGA.EntityModel;
using KrausRGA.DBLogics;
using KrausRGA.ErrorLogger;


namespace KrausRGA.Models
{
   public class mNewRMANumber
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


        public List<int> GreenRowsNumber = new List<int>();

        #endregion

      //  List<RMAInfo> _lsNEWRMA = new List<RMAInfo>();
       


        public Guid SetReturnTbl(List<Return> lsNewRMA, String ReturnReason, Byte RMAStatus, Byte Decision, Guid CreatedBy)
        {
            Guid _returnID = Guid.Empty;
            try
            {
               // _lsNEWRMA = lsNewRMA;
                //Return table new object.
                Return TblRerutn = new Return();

                TblRerutn.ReturnID = Guid.NewGuid();
                TblRerutn.RMANumber = lsNewRMA[0].RMANumber;
                TblRerutn.ShipmentNumber = lsNewRMA[0].ShipmentNumber;
                TblRerutn.OrderNumber = "N/A";
                TblRerutn.PONumber = lsNewRMA[0].PONumber;
                TblRerutn.OrderDate = DateTime.UtcNow;
                TblRerutn.DeliveryDate = DateTime.UtcNow;
                TblRerutn.ReturnDate = lsNewRMA[0].ReturnDate;
                TblRerutn.VendorNumber = lsNewRMA[0].VendorNumber;
                TblRerutn.VendoeName = lsNewRMA[0].VendoeName;
                TblRerutn.CustomerName1 = lsNewRMA[0].CustomerName1;
                TblRerutn.CustomerName2 = "N/A";
                TblRerutn.Address1 = lsNewRMA[0].Address1;
                TblRerutn.Address2 = "N/A";
                TblRerutn.Address3 = "N/A";
                TblRerutn.ZipCode = lsNewRMA[0].ZipCode;
                TblRerutn.City = lsNewRMA[0].City;
                TblRerutn.State = lsNewRMA[0].State;
                TblRerutn.Country = lsNewRMA[0].Country;
                TblRerutn.ReturnReason = ReturnReason;
                TblRerutn.RMAStatus = RMAStatus;
                TblRerutn.Decision = Decision;
                TblRerutn.CreatedBy = CreatedBy;
                TblRerutn.CreatedDate = DateTime.UtcNow;
                TblRerutn.UpdatedBy = null;
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


        public List<RAMStatus> GetRMAStatusList()
        {
            List<RAMStatus> lsReturn = new List<RAMStatus>();
            try
            {
                RAMStatus ram2 = new RAMStatus();
                ram2.ID = 0;
                ram2.Status = "New";

                RAMStatus ram = new RAMStatus();
                ram.ID = 1;
                ram.Status = "Approved";

                RAMStatus ram1 = new RAMStatus();
                ram1.ID = 2;
                ram1.Status = "Pending";

                RAMStatus ram3 = new RAMStatus();
                ram3.ID = 3;
                ram3.Status = "Canceled";

                lsReturn.Add(ram2);
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

        public Guid SetReturnDetailTbl(Guid ReturnTblID, String SKUNumber, String ProductName, int DeliveredQty, int ExpectedQty, int ReturnQty, string TK, Guid CreatedBy)
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
                TblReturnDetails.TCLCOD_0 = TK;
                TblReturnDetails.ReturnQty = ReturnQty;
                TblReturnDetails.ProductStatus = 0;
                TblReturnDetails.CreatedBy = CreatedBy;
                TblReturnDetails.CreatedDate = DateTime.UtcNow;
                TblReturnDetails.UpadatedDate = DateTime.UtcNow;
                TblReturnDetails.UpdatedBy = CreatedBy;

                //On Success of transaction.
                if (cRetutnDetailsTbl.UpsetReturnDetail(TblReturnDetails)) _ReturnID = TblReturnDetails.ReturnDetailID;

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetReturnDetailTbl");
            }
            return _ReturnID;
        }

        public List<Reason> GetReasons(String Category)
        {
            List<Reason> _lsReasons = new List<Reason>();
            try
            {
                //find category of product.
               // String CategoryOFSKU = lsRMAInformation.FirstOrDefault(Sk => Sk.SKUNumber == SKUName).TCLCOD_0;
                _lsReasons = cRtnreasons.GetReasons(Category);
            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/GetReasons(String SKUName)");
            }
            return _lsReasons;
        }

        public List<string> NewRMAInfo(string Chars)
        {
            List<string> _lsNewRMA = new List<string>();
            try
            {
                _lsNewRMA = cSage.GetNewRMANumber(Chars);
            }
            catch (Exception)
            {   }
            return _lsNewRMA;
        }


        public Guid SetTransaction(Guid ReasonID, Guid ReturnDetailID)
        {
            Guid _transationID = Guid.Empty;
            try
            {
                SKUReason tra = new SKUReason();
                tra.SKUReasonID = Guid.NewGuid();
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


        public Guid SetReturnedImages(Guid ReturnDetailID, String ImagePath, Guid CreatedBy)
        {
            Guid _ReturnID = Guid.Empty;
            try
            {
                ReturnImage RtnImages = new ReturnImage();

                RtnImages.ReturnImageID = Guid.NewGuid();
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


    }
}
