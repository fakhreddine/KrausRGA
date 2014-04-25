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
   public class mPOnumberRMA
    {
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

        /// <summary>
        /// List Of information of RMA details.
        /// </summary>
        public List<RMAInfo> lsRMAInformationforponumner { get;  set; }

        public List<Reason> GetReasons(String SKUName)
        {
            List<Reason> _lsReasons = new List<Reason>();
            try
            {
                //find category of product.
                String CategoryOFSKU = lsRMAInformationforponumner.FirstOrDefault(Sk => Sk.SKUNumber == SKUName).TCLCOD_0;
                _lsReasons = cRtnreasons.GetReasons(CategoryOFSKU);
            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/GetReasons(String SKUName)");
            }
            return _lsReasons;
        }


        public Guid SetReturnDetailTbl(Guid ReturnDetailsID, Guid ReturnTblID, String SKUNumber, String ProductName, int DeliveredQty, int ExpectedQty, int ReturnQty, string TK, Guid CreatedBy)
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
                TblReturnDetails.ExpectedQty =ExpectedQty;
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

        public List<cSlipInfo> GetSlipInfo(string SkuNumber, String EANCode, String ReturnReasons, DateTime ScannedDate, DateTime ExpirationDate)
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
                slip.SRNumber = lsRMAInformationforponumner[0].RMANumber;
                slip.EANCode = EANCode;
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

        public String GetSageReasonBySKUSR(String SRNumber, String SKUNumber)
        {
            string SageReasons = "";
            try
            {
                SageReasons = cSage.GetSageReason(SRNumber, SKUNumber);
                if (SageReasons.Trim() == "")
                {
                    SageReasons = "N/A";
                }
            }
            catch (Exception)
            {
            }
            return SageReasons;
        }

        public List<cSlipInfo> GetSlipInfo(List<Return> lsNewRMA, string SkuNumber, String EANCode, String ReturnReasons, string NewRGANumber)
        {
            List<cSlipInfo> _lsslipinfo = new List<cSlipInfo>();
            try
            {
                cSlipInfo slip = new cSlipInfo();
                slip.ProductName = SkuNumber;
                slip.Reason = ReturnReasons;
                slip.ReceivedBY = clGlobal.mCurrentUser.UserInfo.UserName;
                slip.ReceivedDate = lsNewRMA[0].ScannedDate;
                slip.Expiration = lsNewRMA[0].ExpirationDate;
                slip.SRNumber = NewRGANumber;
                slip.EANCode = EANCode;
                _lsslipinfo.Add(slip);
            }
            catch (Exception)
            {

            }
            return _lsslipinfo;
        }

    }
}
