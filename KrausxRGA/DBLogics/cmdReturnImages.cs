using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;

namespace KrausRGA.DBLogics
{
    public class cmdReturnImages
    {
        #region Declarations.

        //RMASYSTEM Database entity object.
        RMASYSTEMEntities entRMA = new RMASYSTEMEntities(); 
        
        #endregion

        #region Get methods.
        


        #endregion

        #region Set methods.
        
        /// <summary>
        ///  Upsert method for ReturnImages Table.
        /// </summary>
        /// <param name="ReturnImageObj">
        /// ReturnImages Table Object to be Updated or inserted.
        /// </param>
        /// <returns>
        /// Boolean value indiction Trasction success or failuer.
        /// </returns>
        public Boolean UpsertReturnImage(ReturnImage ReturnImageObj)
        {
            Boolean _returnFlag = false;
            try
            {
                ReturnImage Images = new ReturnImage();
                Images = entRMA.ReturnImages.SingleOrDefault(rImag => rImag.ReturnImageID == ReturnImageObj.ReturnImageID);
                
                //If record not present in database then insert new record.
                if (Images==null)
                {
                    entRMA.AddToReturnImages(ReturnImageObj);
                }
                else //If record persernt in database then update that record.
                {
                    Images = ReturnImageObj;
                }
                entRMA.SaveChanges();
                _returnFlag = true;
            }
            catch (Exception)
            {}
            return _returnFlag;
        }

        #endregion

        #region Delete methods.

        #endregion

    }
}
