using KrausRGA.Barcode;
using KrausRGA.EntityModel;
using KrausRGA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static string ScenarioType;

        public static string WrongRMAFlag;

        public static string Warranty;

        public static int ShipDate_ScanDate_Diff;

        public static string SKU_Staus;

        public static int TotalPoints;

        public static List<StatusAndPoints> statuspoints;

        //audit object.
        public static mRMAAudit mAuditDetail;

        public static List<String> lsImageList;
        
        public static string BarcodeValueFound;

        public static FoundBarcode FBCode = new FoundBarcode();

        public static List<cSlipInfo> lsSlipInfo;

    }
}
