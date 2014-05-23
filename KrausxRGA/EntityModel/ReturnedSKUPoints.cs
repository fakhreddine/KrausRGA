using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
    public class ReturnedSKUPoints
    {
       
        public Guid ID { get; set; }
        public Guid? ReturnID { get; set; }
        public Guid? ReturnDetailID { get; set; }
        public string SKU { get; set; }
        public string Reason { get; set; }
        public string Reason_Value { get; set; }
        public int? Points { get; set; }
        public int? SkuSequence { get; set; }


        public ReturnedSKUPoints()
        {

        }

        public ReturnedSKUPoints(SaveRMAServiceRefer.ReturnedSKUReasonPointsDTO _ReturnSKUPoints)
        {
            if (_ReturnSKUPoints.ID != Guid.Empty) this.ID = _ReturnSKUPoints.ID;
            if (_ReturnSKUPoints.ReturnID != Guid.Empty) this.ReturnID = _ReturnSKUPoints.ReturnID;
            if (_ReturnSKUPoints.ReturnDetailID != Guid.Empty) this.ReturnDetailID = _ReturnSKUPoints.ReturnDetailID;
            if (_ReturnSKUPoints.SKU != null) this.SKU = _ReturnSKUPoints.SKU;
            if (_ReturnSKUPoints.Reason != null) this.Reason = _ReturnSKUPoints.Reason;
            if (_ReturnSKUPoints.Reason_Value != null) this.Reason_Value = _ReturnSKUPoints.Reason_Value;
            this.SkuSequence = (int)_ReturnSKUPoints.SkuSequence;
        }
        public SaveRMAServiceRefer.ReturnedSKUReasonPointsDTO ConvertToSaveDTO(ReturnedSKUPoints _ReturnedSKUPoints)
        {
            SaveRMAServiceRefer.ReturnedSKUReasonPointsDTO _return = new SaveRMAServiceRefer.ReturnedSKUReasonPointsDTO();

            if (_ReturnedSKUPoints.ID != Guid.Empty) _return.ID = _ReturnedSKUPoints.ID;
            if (_ReturnedSKUPoints.ReturnDetailID != Guid.Empty) _return.ReturnDetailID = _ReturnedSKUPoints.ReturnDetailID;
            if (_ReturnedSKUPoints.ReturnID != Guid.Empty) _return.ReturnID = _ReturnedSKUPoints.ReturnID;
            if (_ReturnedSKUPoints.SKU != null) _return.SKU = _ReturnedSKUPoints.SKU;
            if (_ReturnedSKUPoints.Reason != null) _return.Reason = _ReturnedSKUPoints.Reason;
            if (_ReturnedSKUPoints.Reason_Value != null) _return.Reason_Value = _ReturnedSKUPoints.Reason_Value;

            if (_ReturnedSKUPoints.SkuSequence != null) _return.SkuSequence = _ReturnedSKUPoints.SkuSequence;

             _return.Points =(int) _ReturnedSKUPoints.Points;
        

          //  _return.ID = _ReturnedSKUPoints.ID;
            return _return;

        }

    }
}
