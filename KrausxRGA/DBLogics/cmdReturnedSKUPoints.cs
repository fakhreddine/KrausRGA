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

    }
}
