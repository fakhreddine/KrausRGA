using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    public enum eActionType
    {
        Login=1,
        Login_PageStart,
        LoginFail__00,
        UnAutherisedAccessTry__00,
        Login_InvalidUser__00,
        Login_Success,
        Logout,
        ApplicationExit,
        RMANumberScan,
        InvalidRMANumber__00,
        AlreadySaved__00,
        Load_RMADetail,
        Load_ReturnDetails,
        Load_RetrunReasons,
        PleaseSelectItem__00,
        AtleastOneReasonSelect__00,
        SelectReasons,
        SelectItem
    }
}
