using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel
{
   public class ReturnImage
    {

        public Guid ReturnImageID { get; set; }
        public Guid ReturnDetailID { get; set; }
        public String SKUImagePath { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpadatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpadatedDate { get; set; }
   
       public ReturnImage ()
	{

	}
       public ReturnImage(SaveRMAServiceRefer.ReturnImagesDTO _ReturnImages)
        {
            if (_ReturnImages.ReturnImageID != Guid.Empty) this.ReturnImageID = _ReturnImages.ReturnImageID;
            if (_ReturnImages.ReturnDetailID != Guid.Empty) this.ReturnDetailID = _ReturnImages.ReturnDetailID;
            if (_ReturnImages.SKUImagePath != null) this.SKUImagePath = _ReturnImages.SKUImagePath;
            if (_ReturnImages.CreatedBy != Guid.Empty) this.CreatedBy = (Guid)_ReturnImages.CreatedBy;
            if (_ReturnImages.UpadatedBy != Guid.Empty) this.UpadatedBy = (Guid)_ReturnImages.UpadatedBy;
            if (_ReturnImages.CreatedDate != null) this.CreatedDate = (DateTime)_ReturnImages.CreatedDate;
            if (_ReturnImages.UpadatedDate != null) this.UpadatedDate = (DateTime)_ReturnImages.UpadatedDate;
        }

       public SaveRMAServiceRefer.ReturnImagesDTO CopyToSaveDTO(ReturnImage _ReturnImages)
       {
           SaveRMAServiceRefer.ReturnImagesDTO _return = new SaveRMAServiceRefer.ReturnImagesDTO();
           if (_ReturnImages.ReturnImageID != Guid.Empty) _return.ReturnImageID = _ReturnImages.ReturnImageID;
            if (_ReturnImages.ReturnDetailID != Guid.Empty) _return.ReturnDetailID = _ReturnImages.ReturnDetailID;
            if (_ReturnImages.SKUImagePath != null) _return.SKUImagePath = _ReturnImages.SKUImagePath;
            if (_ReturnImages.CreatedBy != Guid.Empty) _return.CreatedBy = (Guid)_ReturnImages.CreatedBy;
            if (_ReturnImages.UpadatedBy != Guid.Empty) _return.UpadatedBy = (Guid)_ReturnImages.UpadatedBy;
            if (_ReturnImages.CreatedDate != null) _return.CreatedDate = (DateTime)_ReturnImages.CreatedDate;
            if (_ReturnImages.UpadatedDate != null) _return.UpadatedDate = (DateTime)_ReturnImages.UpadatedDate;
           return _return;
       }
    }
}
