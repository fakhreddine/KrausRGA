using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;

namespace KrausRGA.DBLogics
{
    /// <summary>
    /// Shriram Rajaram Insert and get the Reasons table
    /// 11/22/2013
    /// </summary>

  public  class cmdReasons
    {
        /// <summary>
        /// Create object of Entity RMAAYSTEMEntities 
        /// </summary>
      //  RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

      
      /// <summary>
        /// Get tha all reasons From The Reason_table
        /// </summary>
        /// <returns></returns>
        public List<Reason> GetReasons()
        {
           List<Reason> _Reasons=new List<Reason>();
            try
            {
                var resn = Service.entGet.ReasonsAll().ToList();
                foreach (var Resnitem in resn)
                {
                    _Reasons.Add(new Reason(Resnitem));
                }
            }
            catch (Exception)
            {}

            return _Reasons;
        
        }

      /// <summary>
      /// Get the reasons for perticular category
      /// </summary>
      /// <param name="CategoryName">
      /// String Category name.
      /// </param>
      /// <returns>
      /// List of reasons.
      /// </returns>
        public List<Reason> GetReasons(String CategoryName)
        {
            List<Reason> _lsReturn = new List<Reason>();
            try
            {
                String _category = CategoryName.ToUpper();

                var Resns = Service.entGet.ReasonByCategoryName(_category);
                foreach (var Rsnitem in Resns)
                {
                    _lsReturn.Add(new Reason(Rsnitem));
                }

            }
            catch (Exception)
            {}
            return _lsReturn;

        }

      /// <summary>
        /// this function is for Insert the reasons in to the Reason Table and Update 
        /// the Records of Reason Table.....
        /// </summary>
        /// <param name="reasonTbl">
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public Boolean InsertReasons(Reason reasonTbl)
        {
            Boolean status = false;
            try
            {
                status = Service.entSave.Reasons(reasonTbl.CopyToSaveDTO(reasonTbl));

            }
            catch (Exception)
            {}
            return status;
        }


    }
}
