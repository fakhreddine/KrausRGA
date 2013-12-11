using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    public enum eActionType
    {
        Login = 1,
        Login_PageStart,
        LoginFail__00,
        UserPermissonDenied,
        Login_InvalidUser__00,
        Login_Success,
        Logout,
        ApplicationExit,
        ValidRMANumberScan,
        InvalidRMANumberScanned__00,
        AlreadySaved_RMANumberScanned__00,
        SelectItem__00,
        Tab_changed,
        ComboBox_ItemSelected,
        Reason_Checked,
        Reason_Unchecked,
        ProductPersentInRMA_Checked,
        ProductPersentInRMA_UnChecked,
        Camera_Started,
        Image_Captured,
        New_ReturnReason_Added,
        Done_Clicked,
        Camera_Stoped
    }
}
