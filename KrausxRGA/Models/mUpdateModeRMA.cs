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
       public Return _ReturnTbl { get; protected set; }

       public List<ReturnDetail> _lsReturnDetails { get; protected set; }

       public List<SKUReason> _lsReasons { get; protected set; }

       public List<ReturnImage> _lsImages { get; protected set; }


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
       protected DBLogics.cmdReasonCategory cReasonCategory = new DBLogics.cmdReasonCategory();

       /// <summary>
       /// Transaction table Command object 
       /// </summary>
       protected DBLogics.cmdSKUReasons cSkuReasons= new DBLogics.cmdSKUReasons();


       #endregion

       public mUpdateModeRMA()
       {

       }

       public mUpdateModeRMA(String RMANumber)
       {
           _lsImages = new List<ReturnImage>();
           _lsReasons = new List<SKUReason>();
           GetReturnTbl(RMANumber);
           GetLsReturnDetails(_ReturnTbl.ReturnID);
           GetReasons(_lsReturnDetails);
           GetRerurnImages(_lsReturnDetails);

       }

       protected void GetReturnTbl(String RMANumber)
       {
           try
           {
               _ReturnTbl = cReturnTbl.GetRetutnTblByRMANumber(RMANumber);
           }
           catch (Exception)
           {}
       }

       protected void GetLsReturnDetails(Guid ReturntblID)
       {
           try
           {
               _lsReturnDetails = cRetutnDetailsTbl.GetReturnDetailsByReturnID(ReturntblID);
           }
           catch (Exception)
           {}
       }

       protected void GetReasons(List<ReturnDetail> LsRetnDetails)
       {
           try
           {
               foreach (var lsitem in LsRetnDetails)
               {
                 
                   foreach (var item in cSkuReasons.GetSKuReasonsByReturnDetailsID(lsitem.ReturnDetailID))
                   {
                       _lsReasons.Add(item);
                   }
               }
           }
           catch (Exception)
           {}
       }

       protected void GetRerurnImages(List<ReturnDetail> lsRetnDetails)
       {
           try
           {
               foreach (var Rditem in lsRetnDetails)
               {
                   
                   foreach (var Imgitem in  cRtnImages.GetReturnImagesByReturnDetailsID(Rditem.ReturnDetailID))
                   {
                       _lsImages.Add(Imgitem);
                   }
               }
           }
           catch (Exception)
           {}
       }

    }
}
