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

        protected DBLogics.cmdReturnedSKUPoints cRetutnedSKUPoints = new DBLogics.cmdReturnedSKUPoints();
      //  public Boolean IsAlreadySaved { get; protected set; }
        public String EnteredNumber { get; protected set; }
        public List<int> GreenRowsNumber = new List<int>();
        List<Return> _Return = new List<Return>();
        public void mPOnumberRMA1(String ScannedNumber)
        {
            //set entered Number Property of class.
            EnteredNumber = ScannedNumber;

            //Find Type of enum entered number.
         //   EnumNumberType = GetEnteredNumberType(EnteredNumber);

            //Find valid Number or not.
           // IsValidNumber = GetIsValidNumberEntred(EnteredNumber, EnumNumberType);

            //Check that SR Number is persent in database.
          Views.clGlobal.IsAlreadySaved = IsNumberAlreadyPresent(EnteredNumber);


        }


        public Boolean IsNumberAlreadyPresent(String SRnumber)
        {
            Boolean _return = false;
            //RMA databse Object.
            try
            {
                //_Return = new Return(Service.entGet.ReturnByRMANumber(SRnumber));

                List<Return> _lsReturn = new List<Return>();

                //string po = Service.entGet.ReturnByPONumber(SRnumber).SingleOrDefault().PONumber[0].ToString();

                var listReturnTbl = Service.entGet.ReturnByPONumber(SRnumber).ToList();
                foreach (var lsitem in listReturnTbl)
                {
                    _lsReturn.Add(new Return(lsitem));

                }

                string po = _lsReturn[0].PONumber;



                String Anyvalue = po;//_Return.RMANumber;
                if (Anyvalue == SRnumber) _return = true;

                //Check Decision is Always new.
               // IsValidNumber = CanUserOpenThis();
            }
            catch (Exception ex)
            {
                ex.LogThis("mPOnumberRMA/IsNumberAlreadyPresent");
            }
            return _return;

        }

        public Boolean DeleteReturnDetails(Guid ReturnDetailsID)
        {
            return cRetutnDetailsTbl.DeleteReturnDetails(ReturnDetailsID);
        }

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
        public Guid SetReturnTbl(List<Return> lsNewRMA, String ReturnReason, Byte RMAStatus, Byte Decision, Guid CreatedBy, string Wrong_RMA_Flg, string Warranty_STA, int Setting_Wty_Days, int ShipDate_ScanDate_Days_Diff)
        {
            Guid _returnID = Guid.Empty;
            try
            {
                // _lsNEWRMA = lsNewRMA;
                //Return table new object.
                Return TblRerutn = new Return();

                TblRerutn.ReturnID = Guid.NewGuid();
                TblRerutn.RMANumber = null;//lsNewRMA[0].RMANumber;
                TblRerutn.ShipmentNumber = lsNewRMA[0].ShipmentNumber;
                TblRerutn.OrderNumber = lsNewRMA[0].OrderNumber;
                TblRerutn.PONumber = lsNewRMA[0].PONumber;
                TblRerutn.OrderDate = lsNewRMA[0].OrderDate;
                TblRerutn.DeliveryDate = lsNewRMA[0].DeliveryDate;
                TblRerutn.ReturnDate = lsNewRMA[0].ReturnDate;
                TblRerutn.ScannedDate = DateTime.UtcNow;
                TblRerutn.ExpirationDate = DateTime.UtcNow.AddDays(60);
                TblRerutn.VendorNumber = lsNewRMA[0].VendorNumber;
                TblRerutn.VendoeName = lsNewRMA[0].VendoeName;
                TblRerutn.CustomerName1 = lsNewRMA[0].CustomerName1;
                TblRerutn.CustomerName2 = lsNewRMA[0].CustomerName2;
                TblRerutn.Address1 = lsNewRMA[0].Address1;
                TblRerutn.Address2 = null;
                TblRerutn.Address3 = null;
                TblRerutn.ZipCode = lsNewRMA[0].ZipCode;
                TblRerutn.City = lsNewRMA[0].City;
                TblRerutn.State = lsNewRMA[0].State;
                TblRerutn.Country = lsNewRMA[0].Country;
                TblRerutn.ReturnReason = null;
                TblRerutn.RMAStatus = RMAStatus;
                TblRerutn.Decision = Decision;
                TblRerutn.CreatedBy = CreatedBy;
                TblRerutn.CreatedDate = DateTime.UtcNow;
                TblRerutn.UpdatedBy = CreatedBy;
                TblRerutn.UpdatedDate = DateTime.Now;

                TblRerutn.Wrong_RMA_Flg = Wrong_RMA_Flg;
                TblRerutn.Warranty_STA = Warranty_STA;
                TblRerutn.Setting_Wty_Days = Setting_Wty_Days;
                TblRerutn.ShipDate_ScanDate_Days_Diff = ShipDate_ScanDate_Days_Diff;


                


                //On success of transaction it returns id.
                if (cReturnTbl.UpsertReturnTblByPonumber(TblRerutn)) _returnID = TblRerutn.ReturnID;

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetReturnTbl");
            }
            return _returnID;
        }

        public Guid SetReturnDetailTbl(Guid ReturnDetailsID, Guid ReturnTblID, String SKUNumber, String ProductName, int DeliveredQty, int ExpectedQty, int ReturnQty, string TK, Guid CreatedBy, string SKU_Status, int SKU_Reason_Total_Points, int IsScanned, int IsManually, int NewItemQty, int SKU_Qty_Seq, string ProductID, decimal SalesPrice,int LineType)
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
                TblReturnDetails.IsManuallyAdded = IsManually;

                TblReturnDetails.SKU_Sequence = NewItemQty;
                TblReturnDetails.SKU_Qty_Seq = SKU_Qty_Seq;

                TblReturnDetails.SalesPrice = SalesPrice;
                TblReturnDetails.ProductID = ProductID;

                TblReturnDetails.LineType = LineType;

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

        public List<cSlipInfo> GetSlipInfo(List<Return> lsNewRMA, string SkuNumber, String EANCode, String ReturnReasons, string NewRGANumber,string RMAStatus,string ItemStatus)
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
                slip.ItemStatus = ItemStatus;
                slip.RMAStatus = RMAStatus;

                _lsslipinfo.Add(slip);
            }
            catch (Exception)
            {

            }
            return _lsslipinfo;
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

        public Guid SetReturnedSKUPoints(Guid ReturnedSKUID, Guid ReturnDetailsID, Guid ReturnTblID, String SKU, String Reason, string Reason_Value, int Points, int skusequence)
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

        public List<Return> GetReturnByRowID(String RowId)
        {
            List<Return> lsreurnbyrowid = new List<Return>();
            try
            {
                lsreurnbyrowid = cReturnTbl.GetRetrunByROWID(RowId);
            }
            catch (Exception)
            {
                
               
            }
            return lsreurnbyrowid;
        }

    }
}
