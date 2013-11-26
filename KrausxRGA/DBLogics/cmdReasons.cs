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
        RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

      
      /// <summary>
        /// Get tha all reasons From The Reason_table
        /// </summary>
        /// <returns></returns>
        public List<Reason> GetReasons()
        {
           List<Reason> _Reasons=new List<Reason>();
            try
            {
                _Reasons = (from reson in entRMA.Reasons
                            select reson).ToList();
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

               _lsReturn = (from RCategory in entRMA.ReasonCategories
                                   join Rsn in entRMA.Reasons
                                   on RCategory.ReasonID equals Rsn.ReasonID
                                   where RCategory.CategoryName == _category
                                   select Rsn).ToList();

            }
            catch (Exception)
            {}
            return _lsReturn;

        }

      /// <summary>
        /// this function is for Insert the reasons in to the Reason Table and Update 
        /// the Records of Reason Table.....
        /// </summary>
        /// <param name="reasonID">
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public Boolean InsertReasons(Reason reasonID)
        {
            Boolean status = false;
            try
            {
                entRMA.AddToReasons(reasonID);
                entRMA.SaveChanges();
                status = true;
            }
            catch (Exception)
            {}
            return status;
        }


    }
}
