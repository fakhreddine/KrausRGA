using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.DBLogics;

namespace KrausRGA.Models
{
   public class mUpdateModeRMA
    {
       public Return _ReturnTbl { get; set; }

       public List<ReturnDetail> _lsReturnDetails { get; set; }

       public List<Reason> _lsReasons { get; set; }

       public List<ReturnImage> _lsImages { get; set; }


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

       public mUpdateModeRMA()
       {

       }

       public mUpdateModeRMA(String RMANumber)
       {



       }

       public void GetReturnTbl(String RMANumber)
       {
           try
           {
               _ReturnTbl = cReturnTbl.GetRetutnTblByRMANumber(RMANumber);
           }
           catch (Exception)
           {}
       }

       public void GetLsReturnDetails(Guid ReturntblID)
       {
           try
           {
               _lsReturnDetails = cRetutnDetailsTbl.GetReturnDetailsByReturnID(ReturntblID);
           }
           catch (Exception)
           {}
       }
       


    }
}
