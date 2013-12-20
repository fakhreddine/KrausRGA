using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
   public class Return
    {
       
        public Guid ReturnID { get; set; }
        public string RMANumber { get; set; }
        public string ShipmentNumber { get; set; }
        public string OrderNumber { get; set; }
        public string PONumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string VendorNumber { get; set; }
        public string VendoeName { get; set; }
        public string CustomerName1 { get; set; }
        public string CustomerName2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ReturnReason { get; set; }
        public byte? RMAStatus { get; set; }
        public byte? Decision { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int ROWID { get; set; }
        public String RGAROWID { get; set; }

        public Return()
        { }

        public Return(SaveRMAServiceRefer.ReturnDTO _ReturnDTO)
        {
            if (_ReturnDTO.ReturnID != Guid.Empty) this.ReturnID = (Guid)_ReturnDTO.ReturnID;
            if (_ReturnDTO.RMANumber != null) this.RMANumber = _ReturnDTO.RMANumber;
            if (_ReturnDTO.ShipmentNumber != null) this.ShipmentNumber = _ReturnDTO.ShipmentNumber;
            if (_ReturnDTO.OrderNumber != null) this.OrderNumber = _ReturnDTO.OrderNumber;
            if (_ReturnDTO.PONumber != null) this.PONumber = _ReturnDTO.PONumber;
            if (_ReturnDTO.OrderDate != null) this.OrderDate = (DateTime)_ReturnDTO.OrderDate;
            if (_ReturnDTO.DeliveryDat != null) this.DeliveryDate = (DateTime)_ReturnDTO.DeliveryDat;
            if (_ReturnDTO.ReturnDate != null) this.ReturnDate = (DateTime)_ReturnDTO.ReturnDate;
            if (_ReturnDTO.ReturnDate != null) this.VendorNumber = _ReturnDTO.VendorNumber;
            if (_ReturnDTO.VendoeName != null) this.VendoeName = _ReturnDTO.VendoeName;
            if (_ReturnDTO.CustomerName1 != null) this.CustomerName1 = _ReturnDTO.CustomerName1;
            if (_ReturnDTO.CustomerName2 != null) this.CustomerName2 = _ReturnDTO.CustomerName2;
            if (_ReturnDTO.Address1 != null) this.Address1 = _ReturnDTO.Address1;
            if (_ReturnDTO.Address2 != null) this.Address2 = _ReturnDTO.Address2;
            if (_ReturnDTO.Address3 != null) this.Address3 = _ReturnDTO.Address3;
            if (_ReturnDTO.ZipCode != null) this.ZipCode = _ReturnDTO.ZipCode;
            if (_ReturnDTO.City != null) this.City = _ReturnDTO.City;
            if (_ReturnDTO.State != null) this.State = _ReturnDTO.State;
            if (_ReturnDTO.Country != null) this.Country = _ReturnDTO.Country;
            if (_ReturnDTO.ReturnReason != null) this.ReturnReason = _ReturnDTO.ReturnReason;
            if (_ReturnDTO.RMAStatus != null) this.RMAStatus = _ReturnDTO.RMAStatus;
            if (_ReturnDTO.Decision != null) this.Decision = _ReturnDTO.Decision;
            if (_ReturnDTO.CreatedBy != null) this.CreatedBy = _ReturnDTO.CreatedBy;
            if (_ReturnDTO.UpdatedBy != null) this.UpdatedBy = _ReturnDTO.UpdatedBy;
            if (_ReturnDTO.CreatesDate != null) this.CreatedDate = (DateTime)_ReturnDTO.CreatesDate;
            if (_ReturnDTO.UpdatedDate != null) this.UpdatedDate = (DateTime)_ReturnDTO.UpdatedDate;
        }

        public Return(GetRMAServiceRef.ReturnDTO _ReturnDTO)
        {
            if (_ReturnDTO.ReturnID != Guid.Empty) this.ReturnID = (Guid)_ReturnDTO.ReturnID;
            if (_ReturnDTO.RMANumber != null) this.RMANumber = _ReturnDTO.RMANumber;
            if (_ReturnDTO.ShipmentNumber != null) this.ShipmentNumber = _ReturnDTO.ShipmentNumber;
            if (_ReturnDTO.OrderNumber != null) this.OrderNumber = _ReturnDTO.OrderNumber;
            if (_ReturnDTO.PONumber != null) this.PONumber = _ReturnDTO.PONumber;
            if (_ReturnDTO.OrderDate != null) this.OrderDate = (DateTime)_ReturnDTO.OrderDate;
            if (_ReturnDTO.DeliveryDat != null) this.DeliveryDate = (DateTime)_ReturnDTO.DeliveryDat;
            if (_ReturnDTO.ReturnDate != null) this.ReturnDate = (DateTime)_ReturnDTO.ReturnDate;
            if (_ReturnDTO.ReturnDate != null) this.VendorNumber = _ReturnDTO.VendorNumber;
            if (_ReturnDTO.VendoeName != null) this.VendoeName = _ReturnDTO.VendoeName;
            if (_ReturnDTO.CustomerName1 != null) this.CustomerName1 = _ReturnDTO.CustomerName1;
            if (_ReturnDTO.CustomerName2 != null) this.CustomerName2 = _ReturnDTO.CustomerName2;
            if (_ReturnDTO.Address1 != null) this.Address1 = _ReturnDTO.Address1;
            if (_ReturnDTO.Address2 != null) this.Address2 = _ReturnDTO.Address2;
            if (_ReturnDTO.Address3 != null) this.Address3 = _ReturnDTO.Address3;
            if (_ReturnDTO.ZipCode != null) this.ZipCode = _ReturnDTO.ZipCode;
            if (_ReturnDTO.City != null) this.City = _ReturnDTO.City;
            if (_ReturnDTO.State != null) this.State = _ReturnDTO.State;
            if (_ReturnDTO.Country != null) this.Country = _ReturnDTO.Country;
            if (_ReturnDTO.ReturnReason != null) this.ReturnReason = _ReturnDTO.ReturnReason;
            if (_ReturnDTO.RMAStatus != null) this.RMAStatus = _ReturnDTO.RMAStatus;
            if (_ReturnDTO.Decision != null) this.Decision = _ReturnDTO.Decision;
            if (_ReturnDTO.CreatedBy != null) this.CreatedBy = _ReturnDTO.CreatedBy;
            if (_ReturnDTO.UpdatedBy != null) this.UpdatedBy = _ReturnDTO.UpdatedBy;
            if (_ReturnDTO.CreatesDate != null) this.CreatedDate = (DateTime)_ReturnDTO.CreatesDate;
            if (_ReturnDTO.UpdatedDate != null) this.UpdatedDate = (DateTime)_ReturnDTO.UpdatedDate;
        }

        public SaveRMAServiceRefer.ReturnDTO CopyToSaveDTO(Return _ReturnTbl)
        {
            SaveRMAServiceRefer.ReturnDTO _return = new SaveRMAServiceRefer.ReturnDTO();
            if (_ReturnTbl.ReturnID != Guid.Empty) _return.ReturnID = (Guid)_ReturnTbl.ReturnID;
            if (_ReturnTbl.RMANumber != null) _return.RMANumber = _ReturnTbl.RMANumber;
            if (_ReturnTbl.ShipmentNumber != null) _return.ShipmentNumber = _ReturnTbl.ShipmentNumber;
            if (_ReturnTbl.OrderNumber != null) _return.OrderNumber = _ReturnTbl.OrderNumber;
            if (_ReturnTbl.PONumber != null) _return.PONumber = _ReturnTbl.PONumber;
            if (_ReturnTbl.OrderDate != null) _return.OrderDate = (DateTime)_ReturnTbl.OrderDate;
            if (_ReturnTbl.DeliveryDate != null) _return.DeliveryDat = (DateTime)_ReturnTbl.DeliveryDate;
            if (_ReturnTbl.ReturnDate != null) _return.ReturnDate = (DateTime)_ReturnTbl.ReturnDate;
            if (_ReturnTbl.ReturnDate != null) _return.VendorNumber = _ReturnTbl.VendorNumber;
            if (_ReturnTbl.VendoeName != null) _return.VendoeName = _ReturnTbl.VendoeName;
            if (_ReturnTbl.CustomerName1 != null) _return.CustomerName1 = _ReturnTbl.CustomerName1;
            if (_ReturnTbl.CustomerName2 != null) _return.CustomerName2 = _ReturnTbl.CustomerName2;
            if (_ReturnTbl.Address1 != null) _return.Address1 = _ReturnTbl.Address1;
            if (_ReturnTbl.Address2 != null) _return.Address2 = _ReturnTbl.Address2;
            if (_ReturnTbl.Address3 != null) _return.Address3 = _ReturnTbl.Address3;
            if (_ReturnTbl.ZipCode != null) _return.ZipCode = _ReturnTbl.ZipCode;
            if (_ReturnTbl.City != null) _return.City = _ReturnTbl.City;
            if (_ReturnTbl.State != null) _return.State = _ReturnTbl.State;
            if (_ReturnTbl.Country != null) _return.Country = _ReturnTbl.Country;
            if (_ReturnTbl.ReturnReason != null) _return.ReturnReason = _ReturnTbl.ReturnReason;
            if (_ReturnTbl.RMAStatus != null) _return.RMAStatus = _ReturnTbl.RMAStatus;
            if (_ReturnTbl.Decision != null) _return.Decision = _ReturnTbl.Decision;
            if (_ReturnTbl.CreatedBy != null) _return.CreatedBy = _ReturnTbl.CreatedBy;
            if (_ReturnTbl.UpdatedBy != null) _return.UpdatedBy = _ReturnTbl.UpdatedBy;
            if (_ReturnTbl.CreatedDate != null) _return.CreatesDate = (DateTime)_ReturnTbl.CreatedDate;
            if (_ReturnTbl.UpdatedDate != null) _return.UpdatedDate = (DateTime)_ReturnTbl.UpdatedDate;
            return _return;
        }

        public GetRMAServiceRef.ReturnDTO CopyToGetDTO(Return _ReturnTbl)
        {
            GetRMAServiceRef.ReturnDTO _return = new GetRMAServiceRef.ReturnDTO();
            if (_ReturnTbl.ReturnID != Guid.Empty) _return.ReturnID = (Guid)_ReturnTbl.ReturnID;
            if (_ReturnTbl.RMANumber != null) _return.RMANumber = _ReturnTbl.RMANumber;
            if (_ReturnTbl.ShipmentNumber != null) _return.ShipmentNumber = _ReturnTbl.ShipmentNumber;
            if (_ReturnTbl.OrderNumber != null) _return.OrderNumber = _ReturnTbl.OrderNumber;
            if (_ReturnTbl.PONumber != null) _return.PONumber = _ReturnTbl.PONumber;
            if (_ReturnTbl.OrderDate != null) _return.OrderDate = (DateTime)_ReturnTbl.OrderDate;
            if (_ReturnTbl.DeliveryDate != null) _return.DeliveryDat = (DateTime)_ReturnTbl.DeliveryDate;
            if (_ReturnTbl.ReturnDate != null) _return.ReturnDate = (DateTime)_ReturnTbl.ReturnDate;
            if (_ReturnTbl.ReturnDate != null) _return.VendorNumber = _ReturnTbl.VendorNumber;
            if (_ReturnTbl.VendoeName != null) _return.VendoeName = _ReturnTbl.VendoeName;
            if (_ReturnTbl.CustomerName1 != null) _return.CustomerName1 = _ReturnTbl.CustomerName1;
            if (_ReturnTbl.CustomerName2 != null) _return.CustomerName2 = _ReturnTbl.CustomerName2;
            if (_ReturnTbl.Address1 != null) _return.Address1 = _ReturnTbl.Address1;
            if (_ReturnTbl.Address2 != null) _return.Address2 = _ReturnTbl.Address2;
            if (_ReturnTbl.Address3 != null) _return.Address3 = _ReturnTbl.Address3;
            if (_ReturnTbl.ZipCode != null) _return.ZipCode = _ReturnTbl.ZipCode;
            if (_ReturnTbl.City != null) _return.City = _ReturnTbl.City;
            if (_ReturnTbl.State != null) _return.State = _ReturnTbl.State;
            if (_ReturnTbl.Country != null) _return.Country = _ReturnTbl.Country;
            if (_ReturnTbl.ReturnReason != null) _return.ReturnReason = _ReturnTbl.ReturnReason;
            if (_ReturnTbl.RMAStatus != null) _return.RMAStatus = _ReturnTbl.RMAStatus;
            if (_ReturnTbl.Decision != null) _return.Decision = _ReturnTbl.Decision;
            if (_ReturnTbl.CreatedBy != null) _return.CreatedBy = _ReturnTbl.CreatedBy;
            if (_ReturnTbl.UpdatedBy != null) _return.UpdatedBy = _ReturnTbl.UpdatedBy;
            if (_ReturnTbl.CreatedDate != null) _return.CreatesDate = (DateTime)_ReturnTbl.CreatedDate;
            if (_ReturnTbl.UpdatedDate != null) _return.UpdatedDate = (DateTime)_ReturnTbl.UpdatedDate;
            return _return;
        }
   }
}
