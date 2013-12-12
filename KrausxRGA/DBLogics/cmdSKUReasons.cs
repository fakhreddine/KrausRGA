using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;

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
           catch (Exception)
           {
           }
           return _status;
       
       }

    }
}
