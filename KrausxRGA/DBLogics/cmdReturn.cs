using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;


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
                var TemoRtn = cmd.entGet.ReturnAll().ToList();
                foreach (var Rtnitem in TemoRtn)
                {
                    _lsReturn.Add(new Return(Rtnitem));
                }
            }
            catch (Exception)
            {}
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
                _returnObj = new Return(cmd.entGet.ReturnByReturnID(ReturnID));
            }
            catch (Exception)
            {}
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
                _returnTableObj = new Return(cmd.entGet.ReturnByRMANumber(RMANumber));
            }
            catch (Exception)
            { }
            return _returnTableObj;
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
                _returnFlag = cmd.entSave.Return(ObjReturnTbl.CopyToSaveDTO(ObjReturnTbl));
            }
            catch (Exception)
            { }
            return _returnFlag;
        }

        #endregion

        #region Delete methods.



        #endregion

    }
}
