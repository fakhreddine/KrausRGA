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
        RMASYSTEMEntities rma = new RMASYSTEMEntities();
        /// <summary>
        /// Get tha all reasons From The Reason_table
        /// </summary>
        /// <returns></returns>
        public List<Reason> GetReasons()
        {
           List<Reason> _Reasons=new List<Reason>();
            try
            {
                _Reasons = (from reson in rma.Reasons
                            select reson).ToList();

              //  r.Insert(0, new { ReasonID = 0, Reason1 = "--Select--" });new {reson.ReasonID,reson.Reason1 }

                //_Reasons.Insert(0,"--Select--");

               // _Reasons = r.ToList();
                
            }
            catch (Exception)
            {
                
                throw;
            }

            return _Reasons;
        
        }
        /// <summary>
        /// this function is for Insert the reasons in to the Reason Table and Update 
        /// the Records of Reason Table.....
        /// </summary>
        /// <param name="reasonID"></param>
        /// <returns></returns>
        public Boolean InsertReasons(Reason reasonID)
        {
            Boolean status = false;
            try
            {
               
                    rma.AddToReasons(reasonID);
                rma.SaveChanges();
                status = true;

            }
            catch (Exception)
            {
                
                throw;
            }
            return status;
        }


    }
}
