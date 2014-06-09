using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Views
{
    /// <summary>
    /// Avinash: 31Oct 2013
    /// Returned RMA Information.
    /// </summary>
    public class RMAInfo
    {
        public String RMANumber { get; set; }
        public String ShipmentNumber { get; set; }
        public String OrderNumber { get; set; }
        public String PONumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public String VendorNumber { get; set; }
        public String VendorName { get; set; }
        public String SKUNumber { get; set; }
        public String ProductName { get; set; }
        public int DeliveredQty { get; set; }
        public int ExpectedQty { get; set; }
        public int ReturnedQty { get; set; }
        public String CustomerName1 { get; set; }
        public String CustomerName2 { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Address3 { get; set; }
        public String ZipCode { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Country { get; set; }
        public string TCLCOD_0 { get;set; }
        public int SKU_Sequence { get; set; }
        public int SKU_Qty_Seq { get; set; }

        public string ProductID { get; set; }
        public decimal? SalesPrice { get; set; }

        public int? LineType { get; set; }

        public RMAInfo()
        {

        }

        public RMAInfo(GetRMAServiceRef.RMAInfoDTO _sage)
        {
            if (_sage.RMANumber != null) this.RMANumber = (string)_sage.RMANumber;
            if (_sage.ShipmentNumber != null) this.ShipmentNumber = (string)_sage.ShipmentNumber;
            if (_sage.OrderNumber != null) this.OrderNumber = (string)_sage.OrderNumber;
            if (_sage.PONumber != null) this.PONumber = (string)_sage.PONumber;
            if (_sage.OrderDate != null) this.OrderDate = (DateTime)_sage.OrderDate;
            if (_sage.DeliveryDate != null) this.DeliveryDate = (DateTime)_sage.DeliveryDate;
            if (_sage.ReturnDate != null) this.ReturnDate = (DateTime)_sage.ReturnDate;
            if (_sage.VendorNumber != null) this.VendorNumber = (string)_sage.VendorNumber;
            if (_sage.VendorName != null) this.VendorName = (string)_sage.VendorName;
            if (_sage.SKUNumber != null) this.SKUNumber = (string)_sage.SKUNumber;
            if (_sage.ProductName != null) this.ProductName = (string)_sage.ProductName;
            this.DeliveredQty = (int)_sage.DeliveredQty;
            this.ExpectedQty = (int)_sage.ExpectedQty;
            this.ReturnedQty = (int)_sage.ReturnedQty;
            if (_sage.CustomerName1 != null) this.CustomerName1 = (string)_sage.CustomerName1;
            if (_sage.CustomerName2 != null) this.CustomerName2 = (string)_sage.CustomerName2;
            if (_sage.Address1 != null) this.Address1 = (string)_sage.Address1;
            if (_sage.Address2 != null) this.Address2 = (string)_sage.Address2;
            if (_sage.Address3 != null) this.Address3 = (string)_sage.Address3;
            if (_sage.ZipCode != null) this.ZipCode = (string)_sage.ZipCode;
            if (_sage.City != null) this.City = (string)_sage.City;
            if (_sage.State != null) this.State = (string)_sage.State;
            if (_sage.Country != null) this.Country = (string)_sage.Country;
            if (_sage.TCLCOD_0 != null) this.TCLCOD_0 = (string)_sage.TCLCOD_0;
            this.SKU_Sequence = (int)_sage.SKU_Sequence;
            this.SKU_Qty_Seq = (int)_sage.SKU_Qty_Seq;

            if (_sage.ProductID != null) this.ProductID = (string)_sage.ProductID;
            if (_sage.SalesPrice != null) this.SalesPrice = _sage.SalesPrice;

            if (_sage.LineType != null) this.LineType = _sage.LineType;

        }

        public GetRMAServiceRef.RMAInfoDTO CopyToGetDTO(RMAInfo _sage)
        {
            GetRMAServiceRef.RMAInfoDTO _return = new GetRMAServiceRef.RMAInfoDTO();
            if (_sage.RMANumber != null) this.RMANumber = (string)_sage.RMANumber;
            if (_sage.ShipmentNumber != null) this.ShipmentNumber = (string)_sage.ShipmentNumber;
            if (_sage.OrderNumber != null) this.OrderNumber = (string)_sage.OrderNumber;
            if (_sage.PONumber != null) this.PONumber = (string)_sage.PONumber;
            if (_sage.OrderDate != null) this.OrderDate = (DateTime)_sage.OrderDate;
            if (_sage.DeliveryDate != null) this.DeliveryDate = (DateTime)_sage.DeliveryDate;
            if (_sage.ReturnDate != null) this.ReturnDate = (DateTime)_sage.ReturnDate;
            if (_sage.VendorNumber != null) this.VendorNumber = (string)_sage.VendorNumber;
            if (_sage.VendorName != null) this.VendorName = (string)_sage.VendorName;
            if (_sage.SKUNumber != null) this.SKUNumber = (string)_sage.SKUNumber;
            if (_sage.ProductName != null) this.ProductName = (string)_sage.ProductName;
            this.DeliveredQty = (int)_sage.DeliveredQty;
            this.ExpectedQty = (int)_sage.ExpectedQty;
            this.ReturnedQty = (int)_sage.ReturnedQty;
            if (_sage.CustomerName1 != null) this.CustomerName1 = (string)_sage.CustomerName1;
            if (_sage.CustomerName2 != null) this.CustomerName2 = (string)_sage.CustomerName2;
            if (_sage.Address1 != null) this.Address1 = (string)_sage.Address1;
            if (_sage.Address2 != null) this.Address2 = (string)_sage.Address2;
            if (_sage.Address3 != null) this.Address3 = (string)_sage.Address3;
            if (_sage.ZipCode != null) this.ZipCode = (string)_sage.ZipCode;
            if (_sage.City != null) this.City = (string)_sage.City;
            if (_sage.State != null) this.State = (string)_sage.State;
            if (_sage.Country != null) this.Country = (string)_sage.Country;
            if (_sage.TCLCOD_0 != null) this.TCLCOD_0 = (string)_sage.TCLCOD_0;
            this.SKU_Sequence = (int)_sage.SKU_Sequence;
            this.SKU_Qty_Seq = (int)_sage.SKU_Qty_Seq;

            if (_sage.ProductID != null) this.ProductID = (string)_sage.ProductID;
            if (_sage.SalesPrice != null) this.SalesPrice = _sage.SalesPrice;

            if (_sage.LineType != null) this.LineType = _sage.LineType;

            return _return;
        }

    }
}
