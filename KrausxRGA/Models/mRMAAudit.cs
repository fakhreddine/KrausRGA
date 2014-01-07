using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.ErrorLogger;
using KrausRGA.DBLogics;
using KrausRGA.EntityModel;
using KrausRGA.Views;

namespace KrausRGA.Models
{
    public  class mRMAAudit
    {
        /// <summary>
        /// Create cmdaudit Object.
        /// </summary>
      public static DBLogics.cmdRMAAudit _audit = new DBLogics.cmdRMAAudit();

        /// <summary>
        /// Save the audit information.
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ActionType"></param>
        /// <param name="ActionTime"></param>
        /// <returns></returns>
        public static Boolean logthis(String UserID, String ActionType, String ActionTime)
        {
            Boolean _flag = false;
            try
            {
                List<RMAAudit> _UserLog = new List<RMAAudit>();
                RMAAudit _UserC = new RMAAudit();
                _UserC.UserLogID = Guid.NewGuid();
                Guid TuserID = Guid.Empty;
                Guid.TryParse(UserID.ToString(), out TuserID);
                _UserC.UserID = TuserID;
                _UserC.ActionType = ActionType;
                _UserC.ActionTime = Convert.ToDateTime(ActionTime);
                _UserC.ActionValue = "NewAppOpen";
                Views.AuditType.lsaudit.Add(_UserC);
                _UserLog.Add(_UserC);
              //  _flag = _audit.UpsertofAudit(_UserC);
            }
            catch (Exception ex)
            {
                ex.LogThis("mAudit/logthis");
            }
            return _flag;
        }

        /// <summary>
        /// Save Audit information ActionType,ActionTime,ActionValue.
        /// </summary>
        /// <param name="ActionType"></param>
        /// <param name="ActionTime"></param>
        /// <param name="ActionValue"></param>
        /// <returns></returns>
        public static Boolean NoUserlogthis(String ActionType, String ActionTime,String ActionValue)
        {
            Boolean _flag = false;
            try
            {
                List<RMAAudit> _UserLog = new List<RMAAudit>();
                RMAAudit _UserC = new RMAAudit();
                _UserC.UserLogID = Guid.NewGuid();
                //Guid TuserID = Guid.Empty;
                //Guid.TryParse(UserID.ToString(), out TuserID);
                // _UserC.UserID = Guid.Empty;
                _UserC.ActionType = ActionType;
                _UserC.ActionTime = Convert.ToDateTime(ActionTime);
                _UserC.ActionValue = ActionValue;
                Views.AuditType.lsaudit.Add(_UserC);
                _UserLog.Add(_UserC);
              //  _flag = _audit.UpsertofAudit(_UserC);
            }
            catch (Exception ex)
            {
                ex.LogThis("mAudit/NoUserlogthis");
            }
            return _flag;
        }


        /// <summary>
        /// Save Audit Information userId,ActionType,ActionTime,ActionValue.
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="ActionType"></param>
        /// <param name="ActionTime"></param>
        /// <param name="ActionValue"></param>
        /// <returns>
        /// Return Boolean.
        /// </returns>
        public static Boolean logthis(String UserID, String ActionType, String ActionTime,String ActionValue)
        {
            Boolean _flag = false;
            try
            {
                List<RMAAudit> _UserLog = new List<RMAAudit>();
                RMAAudit _UserC = new RMAAudit();
                _UserC.UserLogID = Guid.NewGuid();
                Guid TuserID = Guid.Empty;
                Guid.TryParse(UserID.ToString(), out TuserID);
                _UserC.UserID = TuserID;
                _UserC.ActionType = ActionType;
                _UserC.ActionTime = Convert.ToDateTime(ActionTime);
                _UserC.ActionValue = ActionValue;
                _UserLog.Add(_UserC);
                Views.AuditType.lsaudit.Add(_UserC);
               // _flag = _audit.UpsertofAudit(_UserC);
            }
            catch (Exception ex)
            {
                ex.LogThis("mAudit/logthis(String UserID, String ActionType, String ActionTime,String ActionValue)");
            }
            return _flag;
        }

        public static Boolean saveaudit(List<RMAAudit> lsaudit)
        {
            Boolean _flag = false;
            try
            {
                foreach (var item in lsaudit)
                {
                RMAAudit _UserC = new RMAAudit();
               _UserC.UserLogID = Guid.NewGuid();
                Guid TuserID = item.UserID;
                Guid.TryParse(item.UserID.ToString(), out TuserID);
                _UserC.UserID = TuserID;
                _UserC.ActionType = item.ActionType;
                _UserC.ActionTime = Convert.ToDateTime(item.ActionTime);
                _UserC.ActionValue = item.ActionValue;
                //_UserLog.Add(_UserC);
                _flag = _audit.UpsertofAudit(_UserC);
                }
            }
            catch (Exception)
            {
            }
            return _flag;
        }

        /// <summary>
        /// Initalize connections of services
        /// </summary>
        public mRMAAudit()
        {
        }

    }
}
