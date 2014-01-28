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


       public List<RMAInfo> lsRMAInformation { get; protected set; }

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

        #endregion


        public Guid SetReturnTbl(List<Return> lsRMAInformation, String ReturnReason, Byte RMAStatus, Byte Decision, Guid CreatedBy)
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
                TblRerutn.VendoeName = lsRMAInformation[0].VendoeName;
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




    }
}
