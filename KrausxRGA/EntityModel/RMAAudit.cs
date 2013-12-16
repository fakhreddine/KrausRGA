using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
    public class RMAAudit
    {

        /// <summary>
        /// Blank Constroctor.
        /// </summary>
        public RMAAudit()
        { }

        public Guid UserLogID { get; set; }
        public Guid UserID { get; set; }
        public string ActionType { get; set; }
        public DateTime? ActionTime { get; set; }
        public string ActionValue { get; set; }

        /// <summary>
        /// Constructor of class that converts GetRMAServiceRef.RMAAuditDTO To audit.
        /// </summary>
        /// <param name="_RMAAuditDTO">
        /// GetRMAServiceRef.RMAAuditDTO class object.
        /// </param>
        public RMAAudit(GetRMAServiceRef.RMAAuditDTO _RMAAuditDTO)
        {
            if (_RMAAuditDTO.UserLogID != Guid.Empty) this.UserLogID = (Guid)_RMAAuditDTO.UserLogID;
            if (_RMAAuditDTO.UserID != Guid.Empty) this.UserID = _RMAAuditDTO.UserID;
            if (_RMAAuditDTO.ActionType != null) this.ActionType = _RMAAuditDTO.ActionType;
            if (_RMAAuditDTO.ActionTime != null) this.ActionTime = _RMAAuditDTO.ActionTime;
            if (_RMAAuditDTO.ActionValue != null) this.ActionValue = _RMAAuditDTO.ActionValue;
        }

        /// <summary>
        /// Constructor of class that converts SaveRMAServiceRefer.RMAAuditDTO To audit.
        /// </summary>
        /// <param name="_RMAAuditDTO">
        /// SaveRMAServiceRefer.RMAAuditDTO class object.
        /// </param>
        public RMAAudit(SaveRMAServiceRefer.RMAAuditDTO _RMAAuditDTO)
        {
            if (_RMAAuditDTO.UserLogID != Guid.Empty) this.UserLogID = (Guid)_RMAAuditDTO.UserLogID;
            if (_RMAAuditDTO.UserID != Guid.Empty) this.UserID = (Guid)_RMAAuditDTO.UserID;
            if (_RMAAuditDTO.ActionType != null) this.ActionType = _RMAAuditDTO.ActionType;
            if (_RMAAuditDTO.ActionTime != null) this.ActionTime = _RMAAuditDTO.ActionTime;
            if (_RMAAuditDTO.ActionValue != null) this.ActionValue = _RMAAuditDTO.ActionValue;
        }

        /// <summary>
        /// Convert Audit class object to SaveRMAServiceRefer.RMAAuditDTO
        /// </summary>
        /// <param name="_Audit">
        /// Audit class object.
        /// </param>
        /// <returns>
        /// SaveRMAServiceRefer.RMAAuditDTO object of same value.
        /// </returns>
        public SaveRMAServiceRefer.RMAAuditDTO ConvertTOSaveDTO(RMAAudit _Audit)
        {
            SaveRMAServiceRefer.RMAAuditDTO _DTO = new SaveRMAServiceRefer.RMAAuditDTO();
            if (_Audit.UserLogID != Guid.Empty) _DTO.UserLogID = (Guid)_Audit.UserLogID;
            if (_Audit.UserID != Guid.Empty) _DTO.UserID = (Guid)_Audit.UserID;
            if (_Audit.ActionType != null) _DTO.ActionType = _Audit.ActionType;
            if (_Audit.ActionTime != null) _DTO.ActionTime = _Audit.ActionTime;
            if (_Audit.ActionValue != null) _DTO.ActionValue = _Audit.ActionValue;
            return _DTO;
        }

        
        /// <summary>
        /// Convert Audit class object to GetRMAServiceRef.RMAAuditDTO
        /// </summary>
        /// <param name="_Audit">
        /// Audit class object.
        /// </param>
        /// <returns>
        /// GetRMAServiceRef.RMAAuditDTO object of same value.
        /// </returns>
        public GetRMAServiceRef.RMAAuditDTO ConvertToGetDTO(RMAAudit _Audit)
        {
            GetRMAServiceRef.RMAAuditDTO _DTO = new GetRMAServiceRef.RMAAuditDTO();
            if (_Audit.UserLogID != Guid.Empty) _DTO.UserLogID = (Guid)_Audit.UserLogID;
            if (_Audit.UserID != Guid.Empty) _DTO.UserID = (Guid)_Audit.UserID;
            if (_Audit.ActionType != null) _DTO.ActionType = _Audit.ActionType;
            if (_Audit.ActionTime != null) _DTO.ActionTime = _Audit.ActionTime;
            if (_Audit.ActionValue != null) _DTO.ActionValue = _Audit.ActionValue;
            return _DTO;
        }
    }
}
