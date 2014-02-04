using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace KrausRGA.DBLogics
{
    /// <summary>
    /// set service reference endpoints from config file 
    /// and create new object of each service.
    /// </summary>
   public class  Service
    {
       
       /// <summary>
       /// Save service object.
       /// </summary>
       public static SaveRMAServiceRefer.SaveClient entSave;
       
       /// <summary>
       /// Get Service object.
       /// </summary>
       public static GetRMAServiceRef.GetClient entGet;

       /// <summary>
       /// Delete Service object.
       /// </summary>
       public static DeleteRMAServiceRef.DeleteClient entDelete;
  
    }
}
