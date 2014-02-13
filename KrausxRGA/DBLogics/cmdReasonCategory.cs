using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.ErrorLogger;

namespace KrausRGA.DBLogics
{
    public class cmdReasonCategory
    {
        //create Entity Object
        //  RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

        /// <summary>
        /// Insert the Reason category in to the Reason category Table
        /// </summary>
        /// <param name="ReasonCat">
        /// Pass table object as parameter 
        /// </param>
        /// <returns>
        /// Return the status as boolean
        /// </returns>
        public Boolean SetReasonCategory(ReasonCategory ReasonCat)
        {
            Boolean _return = false;
            try
            {
                _return = Service.entSave.ReasonCategory(ReasonCat.ConvertToSaveDTO(ReasonCat));
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReasonCategory/SetReasonCategory()");
            }
            return _return;
        }
    }
}
