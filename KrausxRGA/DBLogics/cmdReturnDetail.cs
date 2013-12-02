using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;

namespace KrausRGA.DBLogics
{
   public class cmdReturnDetail
    {
       //Entity RGA System database object.
      // RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

        #region Get methods.



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
               _returnFlag = cmd.entSave.ReturnDetails(ReturnDetailsObj.ConvertToSaveDTO(ReturnDetailsObj));
           }
           catch (Exception)
           {}
           return _returnFlag;
 
       }

        #endregion

        #region Deleted.



        
        #endregion
    }
}
