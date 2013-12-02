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
        { }

        public Guid UserLogID { get; set; }
        public Guid UserID { get; set; }
        public string ActionType { get; set; }
        public DateTime? ActionTime { get; set; }
        public string ActionValue { get; set; }

        /// <summary>
        /// Constructor of class that converts GetRMAServiceRef.AuditDTO To audit.
        /// </summary>
        /// <param name="_AuditDTO">
        /// GetRMAServiceRef.AuditDTO class object.
        /// </param>
        public Audit(GetRMAServiceRef.AuditDTO _AuditDTO)
        {
            if (_AuditDTO.UserLogID != Guid.Empty) this.UserLogID = (Guid)_AuditDTO.UserLogID;
            if (_AuditDTO.UserID != Guid.Empty) this.UserID = (Guid)_AuditDTO.UserID;
            if (_AuditDTO.ActionType != null) this.ActionType = _AuditDTO.ActionType;
            if (_AuditDTO.ActionTime != null) this.ActionTime = _AuditDTO.ActionTime;
            if (_AuditDTO.ActionValue != null) this.ActionValue = _AuditDTO.ActionValue;
        }

        /// <summary>
        /// Constructor of class that converts SaveRMAServiceRefer.AuditDTO To audit.
        /// </summary>
        /// <param name="_AuditDTO">
        /// SaveRMAServiceRefer.AuditDTO class object.
        /// </param>
        public Audit(SaveRMAServiceRefer.AuditDTO _AuditDTO)
        {
            if (_AuditDTO.UserLogID != Guid.Empty) this.UserLogID = (Guid)_AuditDTO.UserLogID;
            if (_AuditDTO.UserID != Guid.Empty) this.UserID = (Guid)_AuditDTO.UserID;
            if (_AuditDTO.ActionType != null) this.ActionType = _AuditDTO.ActionType;
            if (_AuditDTO.ActionTime != null) this.ActionTime = _AuditDTO.ActionTime;
            if (_AuditDTO.ActionValue != null) this.ActionValue = _AuditDTO.ActionValue;
        }

        /// <summary>
        /// Convert Audit class object to SaveRMAServiceRefer.AuditDTO
        /// </summary>
        /// <param name="_Audit">
        /// Audit class object.
        /// </param>
        /// <returns>
        /// SaveRMAServiceRefer.AuditDTO object of same value.
        /// </returns>
        public SaveRMAServiceRefer.AuditDTO ConvertTOSaveDTO(Audit _Audit)
        {
            SaveRMAServiceRefer.AuditDTO _DTO = new SaveRMAServiceRefer.AuditDTO();
            if (_Audit.UserLogID != Guid.Empty) _DTO.UserLogID = (Guid)_Audit.UserLogID;
            if (_Audit.UserID != Guid.Empty) _DTO.UserID = (Guid)_Audit.UserID;
            if (_Audit.ActionType != null) _DTO.ActionType = _Audit.ActionType;
            if (_Audit.ActionTime != null) _DTO.ActionTime = _Audit.ActionTime;
            if (_Audit.ActionValue != null) _DTO.ActionValue = _Audit.ActionValue;
            return _DTO;
        }

        /// <summary>
        /// Convert Audit class object to GetRMAServiceRef.AuditDTO
        /// </summary>
        /// <param name="_Audit">
        /// Audit class object.
        /// </param>
        /// <returns>
        /// GetRMAServiceRef.AuditDTO object of same value.
        /// </returns>
        public GetRMAServiceRef.AuditDTO ConvertToGetDTO(Audit _Audit)
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
