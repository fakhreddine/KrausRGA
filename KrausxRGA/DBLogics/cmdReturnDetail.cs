using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.ErrorLogger;

namespace KrausRGA.DBLogics
{
   public class cmdReturnDetail
    {
       //Entity RGA System database object.
      // RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

        #region Get methods.

       public List<ReturnDetail> GetReturnDetailsByReturnID(Guid ReturnID)
       {
           List<ReturnDetail> _lsReturn = new List<ReturnDetail>();
           try
           {
               var listReturnTbl = Service.entGet.ReturnDetailByretrnID(ReturnID).ToList();
               foreach (var lsitem in listReturnTbl)
               {
                   _lsReturn.Add(new ReturnDetail(lsitem));
                   
               }
           }
           catch (Exception)
           {}
           return _lsReturn;
       }

        #endregion

        #region Set methods.

       /// <summary>
       /// Upsert record in to ReturnDetail table of RMASYSTEM database.
       /// </summary>
       /// <param name="ReturnDetailsObj">
       /// Return table object to be add or update in to the database.
       /// </param>
       /// <returns>
       /// Boolean value true if transaction is success.
       /// otherwise false if transaction is fail.
       /// </returns>
       public Boolean UpsetReturnDetail(ReturnDetail ReturnDetailsObj)
       {
           Boolean _returnFlag = false;
           try
           {
               _returnFlag = Service.entSave.ReturnDetails(ReturnDetailsObj.ConvertToSaveDTO(ReturnDetailsObj));
           }
           catch (Exception ex)
           {
               ex.LogThis("cmdReturnDetail/UpsetReturnDetail");
           }
           return _returnFlag;
 
       }

        #endregion

        #region Deleted.



        
        #endregion
    }
}
