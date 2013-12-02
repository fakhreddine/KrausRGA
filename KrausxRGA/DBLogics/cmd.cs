using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.DBLogics
{
    /// <summary>
    /// set service reference endpoints from config file 
    /// and create new object of each service.
    /// </summary>
   public class  cmd
    {
       /// <summary>
       /// Save service object.
       /// </summary>
      public static SaveRMAServiceRefer.SaveClient entSave = new SaveRMAServiceRefer.SaveClient(KrausRGA.Properties.Settings.Default.SetServicePath.ToString());

       /// <summary>
       /// Get Service object.
       /// </summary>
      public static GetRMAServiceRef.GetClient entGet = new GetRMAServiceRef.GetClient(KrausRGA.Properties.Settings.Default.GetServicePath.ToString());

      
       
    }
}
