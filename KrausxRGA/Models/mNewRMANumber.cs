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

        protected DBLogics.cmdReturnedSKUPoints cRetutnedSKUPoints = new DBLogics.cmdReturnedSKUPoints();



        public List<int> GreenRowsNumber = new List<int>();

        #endregion

      //  List<RMAInfo> _lsNEWRMA = new List<RMAInfo>();

        public String GetNewROWID(Guid RMAID)
        {
            String _retunn = "";
            try
            {
                _retunn = cReturnTbl.GetReturnTblByReturnID(RMAID).RGAROWID;
            }
            catch (Exception)
            {}
            return _retunn;

        }
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
                TblRerutn.OrderNumber = "N/A";
                TblRerutn.PONumber = lsNewRMA[0].PONumber;
                TblRerutn.OrderDate = DateTime.UtcNow;
                TblRerutn.DeliveryDate = DateTime.UtcNow;
                TblRerutn.ReturnDate = lsNewRMA[0].ReturnDate;
                TblRerutn.ScannedDate = DateTime.UtcNow;
                TblRerutn.ExpirationDate = DateTime.UtcNow.AddDays(60);
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

                TblRerutn.Wrong_RMA_Flg = Wrong_RMA_Flg;
                TblRerutn.Warranty_STA = Warranty_STA;
                TblRerutn.Setting_Wty_Days = Setting_Wty_Days;
                TblRerutn.ShipDate_ScanDate_Days_Diff = ShipDate_ScanDate_Days_Diff;


                //On success of transaction it returns id.
                if (cReturnTbl.UpsertReturnTblByRGANumber(TblRerutn)) _returnID = TblRerutn.ReturnID;

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


        public Guid SetReturnDetailTbl(Guid ReturnTblID, String SKUNumber, String ProductName, int DeliveredQty, int ExpectedQty, int ReturnQty, string TK, Guid CreatedBy, string SKU_Status, int SKU_Reason_Total_Points, int Isscanned, int IsMnually, int NewItemQty, int SKU_Qty_Seq, string ProductID, decimal SalesPrice)
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

                TblReturnDetails.SKU_Status = SKU_Status;
                TblReturnDetails.SKU_Reason_Total_Points = SKU_Reason_Total_Points;
                TblReturnDetails.IsManuallyAdded = IsMnually;
                TblReturnDetails.IsSkuScanned = Isscanned;


                TblReturnDetails.SKU_Sequence = NewItemQty;
                TblReturnDetails.SKU_Qty_Seq = SKU_Qty_Seq;

                TblReturnDetails.ProductID = ProductID;
                TblReturnDetails.SalesPrice = SalesPrice;

                //On Success of transaction.
                if (cRetutnDetailsTbl.UpsetReturnDetail(TblReturnDetails)) _ReturnID = TblReturnDetails.ReturnDetailID;

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnDetails/SetReturnDetailTbl");
            }
            return _ReturnID;
        }

        public Guid SetReturnedSKUPoints(Guid ReturnedSKUID, Guid ReturnDetailsID, Guid ReturnTblID, String SKU, String Reason, string Reason_Value, int Points, int SkuSequence)
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
                TblReturnedSKUPoints.SkuSequence = SkuSequence;


                //On Success of transaction.
                if (cRetutnedSKUPoints.UpsertReturnedSKUPoints(TblReturnedSKUPoints)) _ReturnedskuID = TblReturnedSKUPoints.ID;

            }
            catch (Exception ex)
            {
                ex.LogThis("mReturnedSKUPoints/SetReturnedSKUPoints");
            }
            return _ReturnedskuID;
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

        public List<cSlipInfo> GetSlipInfo(List<Return> lsNewRMA,string SkuNumber, String EANCode, String ReturnReasons,string NewRGANumber,string RMAStatus,String ItemStatus)
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

        public List<RMAInfo> GetCustomer(string PONumber)
        {
            List<RMAInfo> lsCustomer = new List<RMAInfo>();
            try
            {
                lsCustomer = cSage.GetCustInfoByPoNumber(PONumber);
            }
            catch (Exception)
            {
            }
            return lsCustomer;
        
        }

        public List<string> GetPOnumber(string Chars)
        {
            List<string> _lsPonumber = new List<string>();
            try
            {
                _lsPonumber = cSage.GetPONumber(Chars);
            }
            catch (Exception)
            { }
            return _lsPonumber;
        }

        public List<string> GetVenderName(String Chars)
        {
            List<String> _lsvenderName = new List<string>();
            try
            {
                _lsvenderName = cSage.GetVenderName(Chars);
            }
            catch (Exception)
            {
            }
            return _lsvenderName;
        }

        public List<String> GetVanderNumber(String Chars)
        {
            List<string> _lsVenderNumber = new List<string>();
            try
            {
                _lsVenderNumber = cSage.GetVenderNumber(Chars);
            }
            catch (Exception)
            {
            }
            return _lsVenderNumber;
        }

        public string GetVenderNameByVenderNumber(String VenderNumber)
        {
            string VenderName = "";
            try
            {
                VenderName = cSage.GetVenderNamebyVenderNumber(VenderNumber);
            }
            catch (Exception)
            {
            }
            return VenderName;
        }

        public String GetVenderNumberByVenderName(String VenderName)
        {
            string VenderNumber = "";
            try
            {
                VenderNumber = cSage.GetVenderNumberByVenderName(VenderName);
            }
            catch (Exception)
            {
            }
            return VenderNumber;
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
