using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.ErrorLogger;

namespace KrausRGA.DBLogics
{
   public class cmdSKUReasons
    {
       //create object of the RMASystemEntity
      // RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

       public Boolean SetTransaction(SKUReason Trans)
       {
           Boolean _status = false;
           try
           {
               _status = Service.entSave.SKUReasons(Trans.CopyToSaveDTO(Trans));
           }
           catch (Exception ex)
           {
               ex.LogThis("cmdSKUReasons/SetTransaction");
           }
           return _status;
       
       }

       public List<SKUReason> GetSKuReasonsByReturnDetailsID(Guid ReturnDetailID)
       {
           List<SKUReason> _lsReturn = new List<SKUReason>();
           try
           {

           }
           catch (Exception)
           {}
           return _lsReturn;

       }

    }
}
