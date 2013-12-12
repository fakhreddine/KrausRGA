using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.ErrorLogger;

namespace KrausRGA.DBLogics
{
    /// <summary>
    /// Shriram Rajaram 20/11/2013 : Kraus RGA System
    /// Interaction logic for get,set and delete
    /// in audit table
    /// </summary>

  public class cmdAudit
    {
        #region declaration
        //RMAsystem Database object
       // RMASYSTEMEntities RMA = new RMASYSTEMEntities();
        #endregion

        /// <summary>
        /// get All data from the audit Table
        /// </summary>
        /// <returns></returns>
      public List<Audit> GetAudit()
      {
          List<Audit> _lsReturn = new List<Audit>();
          try
          {
              var adt = (from auditdetail in Service.entGet.AuditAll()
                         select auditdetail).ToList();
              foreach (var Aitem in adt)
              {
                  Audit _audit = new Audit(Aitem);
                  _lsReturn.Add(_audit);
              }
          }
          catch (Exception ex)
          {
              ex.LogThis("cmdAudit/GetAudit()");
          }
          return _lsReturn;
      }

        /// <summary>
        /// This Fuction for get detail of audit by UserID  
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public Audit GetdatafromauditbyUserid(Guid UserID)
        {
            Audit AuditUser = new Audit();
            try
            {
                AuditUser = new Audit(Service.entGet.AuditAll().FirstOrDefault(i => i.UserID == UserID));
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdAudit/GetdatafromauditbyUserid");
            }
            return AuditUser;
        }
        /// <summary>
        /// insert and update the records in audit table
        /// if data already exist then it will update or insert
        /// </summary>
        /// <param name="userlog"></param>
        /// <returns></returns>
        public Boolean UpsertofAudit(Audit userlog)
        {
            Boolean _returnflag = false;
            try
            {
                SaveRMAServiceRefer.AuditDTO AD = userlog.ConvertTOSaveDTO(userlog);
                
                _returnflag = Service.entSave.UpsertAudit(AD);
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdAudit/UpsertofAudit");
            }
            return _returnflag;
        }
       
    }
}
