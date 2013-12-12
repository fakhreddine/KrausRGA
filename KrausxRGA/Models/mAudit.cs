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
    public  class mAudit
    {
        /// <summary>
        /// Create cmdaudit Object.
        /// </summary>
      public static DBLogics.cmdAudit _audit = new DBLogics.cmdAudit();

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
                List<Audit> _UserLog = new List<Audit>();
                Audit _UserC = new Audit();
                _UserC.UserLogID = Guid.NewGuid();
                Guid TuserID = Guid.Empty;
                Guid.TryParse(UserID.ToString(), out TuserID);
                _UserC.UserID = TuserID;
                _UserC.ActionType = ActionType;
                _UserC.ActionTime = Convert.ToDateTime(ActionTime);
                _UserC.ActionValue = "NewAppOpen";
                _UserLog.Add(_UserC);
                _flag = _audit.UpsertofAudit(_UserC);
            }
            catch (Exception ex)
            {
                ex.LogThis("mAudit/logthis");
            }
            return _flag;
        }

        public static Boolean NoUserlogthis(String ActionType, String ActionTime,String ActionValue)
        {
            Boolean _flag = false;
            try
            {
                List<Audit> _UserLog = new List<Audit>();
                Audit _UserC = new Audit();
                _UserC.UserLogID = Guid.NewGuid();
                //Guid TuserID = Guid.Empty;
                //Guid.TryParse(UserID.ToString(), out TuserID);
                // _UserC.UserID = Guid.Empty;
                _UserC.ActionType = ActionType;
                _UserC.ActionTime = Convert.ToDateTime(ActionTime);
                _UserC.ActionValue = ActionValue;
                _UserLog.Add(_UserC);
                _flag = _audit.UpsertofAudit(_UserC);
            }
            catch (Exception ex)
            {
                ex.LogThis("mAudit/NoUserlogthis");
            }
            return _flag;
        }



        public static Boolean logthis(String UserID, String ActionType, String ActionTime,String ActionValue)
        {
            Boolean _flag = false;
            try
            {
                List<Audit> _UserLog = new List<Audit>();
                Audit _UserC = new Audit();
                _UserC.UserLogID = Guid.NewGuid();
                Guid TuserID = Guid.Empty;
                Guid.TryParse(UserID.ToString(), out TuserID);
                _UserC.UserID = TuserID;
                _UserC.ActionType = ActionType;
                _UserC.ActionTime = Convert.ToDateTime(ActionTime);
                _UserC.ActionValue = ActionValue;
                _UserLog.Add(_UserC);
                _flag = _audit.UpsertofAudit(_UserC);
            }
            catch (Exception ex)
            {
                ex.LogThis("mAudit/logthis(String UserID, String ActionType, String ActionTime,String ActionValue)");
            }
            return _flag;
        }

        /// <summary>
        /// Initalize connections of services
        /// </summary>
        public mAudit()
        {
        }

    }
}
