using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.ErrorLogger;

namespace KrausRGA.DBLogics
{
    public class cmdReturnedSKUPoints
    {
        public Boolean UpsertReturnedSKUPoints(ReturnedSKUPoints ObjReturnedSKU)
        {
            Boolean _returnFlag = false;
            try
            {
                _returnFlag = Service.entSave.ReturnedSKUPoints(ObjReturnedSKU.ConvertToSaveDTO(ObjReturnedSKU));
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReturnedSKUpoint/UpsertReturnedSKUPoints");
            }
            return _returnFlag;
        }
        public List<ReturnedSKUPoints> GetReturnedSKUPointsByReturnID(Guid ReturnID)
        {
            List<ReturnedSKUPoints> lsskuandpoint = new List<ReturnedSKUPoints>();
            try
            {
              

                var points = Service.entGet.GetSKUReasonandPointsByReturnID(ReturnID);

                foreach (var item in points)
                {
                    ReturnedSKUPoints skuandpoint = new ReturnedSKUPoints();
                    skuandpoint.ID = item.ID;
                    skuandpoint.ReturnID = item.ReturnID;
                    skuandpoint.ReturnDetailID = item.ReturnDetailID;
                    skuandpoint.Reason_Value = item.Reason_Value;
                    skuandpoint.Reason = item.Reason;
                    skuandpoint.SKU = item.SKU;
                    skuandpoint.Points = item.Points;
                    skuandpoint.SkuSequence = item.SkuSequence;
                    lsskuandpoint.Add(skuandpoint);
                }
               

            }
            catch (Exception)
            {
            }
            return lsskuandpoint;
        }


        public List<ReturnedSKUPoints> GetReturnedSKUPointsByReturnDetailID(Guid ReturnDetailID)
        {
            List<ReturnedSKUPoints> lsskuandpoint = new List<ReturnedSKUPoints>();
            try
            {
               

                var points = Service.entGet.GetSKUReasonandPointsByReturnID(ReturnDetailID);

                foreach (var item in points)
                {
                    ReturnedSKUPoints skuandpoint = new ReturnedSKUPoints();
                    skuandpoint.ID = item.ID;
                    skuandpoint.ReturnID = item.ReturnID;
                    skuandpoint.ReturnDetailID = item.ReturnDetailID;
                    skuandpoint.Reason_Value = item.Reason_Value;
                    skuandpoint.Reason = item.Reason;
                    skuandpoint.SKU = item.SKU;
                    skuandpoint.Points = item.Points;
                    skuandpoint.SkuSequence = item.SkuSequence;
                    lsskuandpoint.Add(skuandpoint);
                }
               

            }
            catch (Exception)
            {
            }
            return lsskuandpoint;
        }

    }
}
