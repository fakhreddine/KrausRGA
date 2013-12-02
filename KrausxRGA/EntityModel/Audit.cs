using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
   public class Audit
    {
       
       /// <summary>
       /// Blank Constroctor.
       /// </summary>
       public Audit()
       {      }

       public Guid UserLogID { get; set; }
       public Guid UserID { get; set; }
       public string ActionType { get; set; }
       public DateTime? ActionTime { get; set; }
       public string ActionValue { get; set; }
       
       public Audit(GetRMAServiceRef.AuditDTO _AuditDTO)
       {
           if (_AuditDTO.UserLogID != Guid.Empty) this.UserLogID = (Guid)_AuditDTO.UserLogID;
           if (_AuditDTO.UserID != Guid.Empty) this.UserID = (Guid)_AuditDTO.UserID;
           if (_AuditDTO.ActionType != null) this.ActionType = _AuditDTO.ActionType;
           if (_AuditDTO.ActionTime != null) this.ActionTime = _AuditDTO.ActionTime;
           if (_AuditDTO.ActionValue != null) this.ActionValue = _AuditDTO.ActionValue;
       }

       public Audit(SaveRMAServiceRefer.AuditDTO _AuditDTO)
       {
           if (_AuditDTO.UserLogID != Guid.Empty) this.UserLogID = (Guid)_AuditDTO.UserLogID;
           if (_AuditDTO.UserID != Guid.Empty) this.UserID = (Guid)_AuditDTO.UserID;
           if (_AuditDTO.ActionType != null) this.ActionType = _AuditDTO.ActionType;
           if (_AuditDTO.ActionTime != null) this.ActionTime = _AuditDTO.ActionTime;
           if (_AuditDTO.ActionValue != null) this.ActionValue = _AuditDTO.ActionValue;
       }

       public  SaveRMAServiceRefer.AuditDTO ConvertTOSaveDTO(  Audit _Audit)
       {
           SaveRMAServiceRefer.AuditDTO _DTO = new SaveRMAServiceRefer.AuditDTO();
           if (_Audit.UserLogID != Guid.Empty) _DTO.UserLogID = (Guid)_Audit.UserLogID;
           if (_Audit.UserID != Guid.Empty) _DTO.UserID = (Guid)_Audit.UserID;
           if (_Audit.ActionType != null) _DTO.ActionType = _Audit.ActionType;
           if (_Audit.ActionTime != null) _DTO.ActionTime = _Audit.ActionTime;
           if (_Audit.ActionValue != null) _DTO.ActionValue = _Audit.ActionValue;
           return _DTO;
       }

       public  GetRMAServiceRef.AuditDTO ConvertToGetDTO( Audit _Audit)
       {
           GetRMAServiceRef.AuditDTO _DTO = new GetRMAServiceRef.AuditDTO();
           if (_Audit.UserLogID != Guid.Empty) _DTO.UserLogID = (Guid)_Audit.UserLogID;
           if (_Audit.UserID != Guid.Empty) _DTO.UserID = (Guid)_Audit.UserID;
           if (_Audit.ActionType != null) _DTO.ActionType = _Audit.ActionType;
           if (_Audit.ActionTime != null) _DTO.ActionTime = _Audit.ActionTime;
           if (_Audit.ActionValue != null) _DTO.ActionValue = _Audit.ActionValue;
           return _DTO;
       }
    }
}
