using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
   public class Reason
    {
       
      public Guid ReasonID { get; set; }
      public string Reason1 { get; set; }

      public Reason()
      {
          
      }

      public Reason(SaveRMAServiceRefer.ReasonsDTO _ReasonsDTO)
      {
          if (_ReasonsDTO.ReasonID != null) this.ReasonID = _ReasonsDTO.ReasonID;
          if (_ReasonsDTO.Reason != null) this.Reason1 = _ReasonsDTO.Reason;
      }

      public Reason(GetRMAServiceRef.ReasonsDTO _ReasonsDTO)
      {
          if (_ReasonsDTO.ReasonID != null) this.ReasonID = _ReasonsDTO.ReasonID;
          if (_ReasonsDTO.Reason != null) this.Reason1 = _ReasonsDTO.Reason;
      }

      public GetRMAServiceRef.ReasonsDTO CopyToGetDTO(Reason _Reason)
      {
          GetRMAServiceRef.ReasonsDTO _return = new GetRMAServiceRef.ReasonsDTO();
          if (_Reason.ReasonID != null) _return.ReasonID = _Reason.ReasonID;
          if (_Reason.Reason1 != null) _return.Reason = _Reason.Reason1;
          return _return;
      }

      public SaveRMAServiceRefer.ReasonsDTO CopyToSaveDTO(Reason _Reason)
      {
          SaveRMAServiceRefer.ReasonsDTO _return = new SaveRMAServiceRefer.ReasonsDTO();
          if (_Reason.ReasonID != null) _return.ReasonID = _Reason.ReasonID;
          if (_Reason.Reason1 != null) _return.Reason = _Reason.Reason1;
          return _return;
      }
    }
}
