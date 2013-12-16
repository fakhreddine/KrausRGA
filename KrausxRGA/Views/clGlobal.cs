using KrausRGA.EntityModel;
using KrausRGA.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    public static class clGlobal
    {
        /// return model object.
        public static mReturnDetails mReturn;

        //user Logged in or not.
        public static Boolean IsUserlogged = false;

        //User information maintain 
        public static mUser mCurrentUser;

        //audit object.
        public static  mRMAAudit mAuditDetail;


    }
}
