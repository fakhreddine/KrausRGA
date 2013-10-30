using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.DBLogics
{
    /// <summary>
    /// Avinash : 30 oct 2013,
    /// All Datase ralated operation related to SR number
    /// and its related operations are performated here.
    /// this operation realted to x3v6.
    /// All operation are get not set.
    /// </summary>
   public  class  cmdSRNumber
    {
        #region getOperations



       /// <summary>
       /// Check that SRNumber is Present in database.
       /// </summary>
       /// <param name="SRNumber">
       /// String SRNumber To be checekd.
       /// </param>
       /// <returns>
       /// Boolan value that true if SR number is persent in the database.
       /// </returns>
       public Boolean IsSRNumberPresetInDB(string SRNumber)
       {

           Boolean _isSRNumberPresent = true;
           try
           {

           }
           catch (Exception)
           {}
           return _isSRNumberPresent;
       }

        #endregion

    }
}
