using KrausRGA.Barcode;
using KrausRGA.EntityModel;
using KrausRGA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KrausRGA.Views
{
    public static class clGlobal 
    {
        /// return model object.
        public static mReturnDetails mReturn;

        public static mPOnumberRMA mPONumber;

        public static Boolean IsAlreadySaved;

        //user Logged in or not.
        public static Boolean IsUserlogged = false;


        public static Image Zoomimages;

        //User information maintain 
        public static mUser mCurrentUser;

        public static string ScenarioType;

        public static string WrongRMAFlag;

        public static string Warranty;

        public static int ShipDate_ScanDate_Diff;

        public static string SKU_Staus;

        public static int TotalPoints;

        public static int IsScanned;

        public static int IsManually;

        public static int NewItemQty;

        public static int _SKU_Qty_Seq;

        public static string Ponumber;

        public static string NewRGANumber;

        public static string Redirect;

        public static List<StatusAndPoints> statuspoints;

        //audit object.
        public static mRMAAudit mAuditDetail;

        public static List<String> lsImageList;
        
        public static string BarcodeValueFound;

        public static FoundBarcode FBCode = new FoundBarcode();

        public static BarcodeForSKU FBCodeForSKU = new BarcodeForSKU();


        public static List<cSlipInfo> lsSlipInfo;

    }
}
