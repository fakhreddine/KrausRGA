using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
    public  class Role
    {

        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? Updatedby { get; set; }


        public Role(GetRMAServiceRef.RoleDTO _role)
        {
            if (_role.RoleID != Guid.Empty) this.RoleId = (Guid)_role.RoleID;
            if (_role.Name != null) this.Name = _role.Name;
            if (_role.Action != null) this.Action = _role.Action;
            if (_role.CreatedDateTime != null) this.CreatedDateTime = (DateTime)_role.CreatedDateTime;
            if (_role.UpdatedDateTime != null) this.UpdatedDateTime = (DateTime)_role.UpdatedDateTime;
            if (_role.CreatedBy != null) this.CreatedBy = _role.CreatedBy;
            if (_role.Updatedby != null) this.Updatedby = _role.Updatedby;
        }

        public GetRMAServiceRef.RoleDTO CopyToGetDTO(Role _role)
        {
            GetRMAServiceRef.RoleDTO _return = new GetRMAServiceRef.RoleDTO();
            if (_role.RoleId != Guid.Empty) this.RoleId = (Guid)_role.RoleId;
            if (_role.Name != null) this.Name = _role.Name;
            if (_role.Action != null) this.Action = _role.Action;
            if (_role.CreatedDateTime != null) this.CreatedDateTime = (DateTime)_role.CreatedDateTime;
            if (_role.UpdatedDateTime != null) this.UpdatedDateTime = (DateTime)_role.UpdatedDateTime;
            if (_role.CreatedBy != null) this.CreatedBy = _role.CreatedBy;
            if (_role.Updatedby != null) this.Updatedby = _role.Updatedby;
            return _return;


        }
        public Role()
        {

        }

    }
}
