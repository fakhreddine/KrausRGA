using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.ErrorLogger;

namespace KrausRGA.DBLogics
{
    /// <summary>
    /// Avinash : 14 Nov 2013 : Kraus RGA system.
    /// Interaction logic for get, set and delete method
    /// of Return Table in Database.
    /// </summary>
    public class cmdReturn
    {
        #region Declarations.

        //RMA system database Object.
        //RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

        #endregion

        #region GetMethods.

        /// <summary>
        /// Get All rows from return Table.
        /// </summary>
        /// <returns>
        /// list of return table objects.
        /// if no record found then return null object.
        /// </returns>
        public List<Return> GetReturnTbl()
        {
            List<Return> _lsReturn = new List<Return>();
            try
            {
                var TemoRtn = Service.entGet.ReturnAll().ToList();
                foreach (var Rtnitem in TemoRtn)
                {
                    _lsReturn.Add(new Return(Rtnitem));
                }
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReturn/GetReturnTbl");
            }
            return _lsReturn;
        }

        /// <summary>
        /// Get all information about return table by ReturnID
        /// </summary>
        /// <param name="ReturnID">
        /// Guid Return ID.
        /// </param>
        /// <returns>
        /// Return table object with information.
        /// if no record found for object then return null object.
        /// </returns>
        public Return GetReturnTblByReturnID(Guid ReturnID)
        {
            Return _returnObj = new Return();
            try
            {
                _returnObj = new Return(Service.entGet.ReturnByReturnID(ReturnID));
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReturn/GetReturnTblByReturnID");
            }
            return _returnObj;
        }

        /// <summary>
        /// Get all information of ReturnTable by RMA number./ SR Number.
        /// </summary>
        /// <param name="RMANumber">
        /// String RMA number /SR Number.
        /// </param>
        /// <returns>
        /// Object of Return Table if no record found for RMA Number then return Null Object.
        /// </returns>
        public Return GetRetutnTblByRMANumber(String RMANumber)
        {
            Return _returnTableObj = new Return();
            try
            {
                _returnTableObj = new Return(Service.entGet.ReturnByRMANumber(RMANumber));
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReturn/GetRetutnTblByRMANumber");
            }
            return _returnTableObj;
        }

        public List<Return> GetRetrunByROWID(string ROWID)
        {
            List<Return> _lsreturn = new List<Return>();
            try
            {
                var byrowid = Service.entGet.ReturnByRGAROWID(ROWID).ToList();
                foreach (var item in byrowid)
                {
                    _lsreturn.Add(new Return(item));
                }
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReturn/GetRetrunByROWID");
            }
            return _lsreturn;
        }




        #endregion

        #region Set Methods.

        /// <summary>
        /// Upsert Opration for Return table.
        /// </summary>
        /// <param name="ObjReturnTbl">
        /// Return table object that to be insert or update.
        /// </param>
        /// <returns>
        /// Boolean Value if transaction success else fail.
        /// </returns>
        public Boolean UpsertReturnTbl(Return ObjReturnTbl)
        {
            Boolean _returnFlag = false;
            try
            {
                _returnFlag = Service.entSave.Return(ObjReturnTbl.CopyToSaveDTO(ObjReturnTbl));
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReturn/UpsertReturnTbl");
            }
            return _returnFlag;
        }

        public Boolean UpsertReturnTblByPonumber(Return ObjReturnTbl)
        {
            Boolean _returnFlag = false;
            try
            {
                _returnFlag = Service.entSave.ReturnByPOnmber(ObjReturnTbl.CopyToSaveDTO(ObjReturnTbl));
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReturn/UpsertReturnTblByPOnumber");
            }
            return _returnFlag;
        }

        public Boolean UpsertReturnTblByRGANumber(Return ObjReturnTbl)
        {
            Boolean _returnFlag = false;
            try
            {
                _returnFlag = Service.entSave.ReturnByRGANumber(ObjReturnTbl.CopyToSaveDTO(ObjReturnTbl));
            }
            catch (Exception ex)
            {
                ex.LogThis("cmdReturn/UpsertReturnTblByPOnumber");
            }
            return _returnFlag;
        }


        #endregion

        #region Delete methods.



        #endregion

    }
}
