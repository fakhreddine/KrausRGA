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
       RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

       public Boolean SetTransaction(SKUReason Trans)
       {
           Boolean _status = false;
           try
           {
               entRMA.AddToSKUReasons(Trans);
               entRMA.SaveChanges();
               _status = true;
           }
           catch (Exception)
           {
           }
           return _status;
       
       }

    }
}
