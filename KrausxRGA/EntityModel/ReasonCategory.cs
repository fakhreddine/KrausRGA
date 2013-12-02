using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
    public class ReasonCategory
    {
        public Guid ReasonCatID { get; set; }
        public Guid ReasonID { get; set; }
        public string CategoryName { get; set; }
        
        public ReasonCategory()
        {

        }

        public ReasonCategory(SaveRMAServiceRefer.ReasonCategoryDTO _ReasonCategory)
        {
            if (_ReasonCategory.ReasonCatID != Guid.Empty) this.ReasonCatID = (Guid)_ReasonCategory.ReasonCatID;
            if (_ReasonCategory.ReasonID != Guid.Empty) this.ReasonID = (Guid)_ReasonCategory.ReasonID;
            if (_ReasonCategory.CategoryName != null) this.CategoryName = (string)_ReasonCategory.CategoryName;
        }

        public SaveRMAServiceRefer.ReasonCategoryDTO ConvertToSaveDTO(ReasonCategory _ReasonCategory)
        {
            SaveRMAServiceRefer.ReasonCategoryDTO _return = new SaveRMAServiceRefer.ReasonCategoryDTO();
            if (_ReasonCategory.ReasonCatID != Guid.Empty) _return.ReasonCatID = (Guid)_ReasonCategory.ReasonCatID;
            if (_ReasonCategory.ReasonID != Guid.Empty) _return.ReasonID = (Guid)_ReasonCategory.ReasonID;
            if (_ReasonCategory.CategoryName != null) _return.CategoryName = (string)_ReasonCategory.CategoryName;
            return _return;
 
        }
    }
}
