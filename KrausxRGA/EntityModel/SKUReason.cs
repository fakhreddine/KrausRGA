using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
   public class SKUReason
    {
        public Guid SKUReasonID { get; set; }
        public Guid ReasonID { get; set; }
        public Guid ReturnDetailID { get; set; }
       
       
       public SKUReason()
        {

        }

        public SKUReason(SaveRMAServiceRefer.SKUReasonsDTO SKUReasons)
        {
            if (SKUReasons.SKUReasonID != Guid.Empty) this.SKUReasonID = SKUReasons.SKUReasonID;
            if (SKUReasons.ReasonID != Guid.Empty) this.ReasonID = (Guid)SKUReasons.ReasonID;
            if (SKUReasons.ReturnDetailID != Guid.Empty) this.ReturnDetailID = (Guid)SKUReasons.ReturnDetailID;
        }
        

        public SaveRMAServiceRefer.SKUReasonsDTO CopyToSaveDTO(SKUReason _SKUReason)
        {
            SaveRMAServiceRefer.SKUReasonsDTO _return = new SaveRMAServiceRefer.SKUReasonsDTO();
            if (_SKUReason.SKUReasonID != Guid.Empty) _return.SKUReasonID = _SKUReason.SKUReasonID;
            if (_SKUReason.ReasonID != Guid.Empty) _return.ReasonID = (Guid)_SKUReason.ReasonID;
            if (_SKUReason.ReturnDetailID != Guid.Empty) _return.ReturnDetailID = (Guid)_SKUReason.ReturnDetailID;
            return _return;
        
        }



    }
}
