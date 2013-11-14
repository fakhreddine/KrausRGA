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
        RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

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
                _lsReturn = (from XReturnTbl in entRMA.Returns
                             select XReturnTbl).ToList();
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
                _returnObj = entRMA.Returns.SingleOrDefault(ret => ret.ReturnID == ReturnID );
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
                _returnTableObj = entRMA.Returns.SingleOrDefault(Ret => Ret.RMANumber == RMANumber);
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
                Return _returnTbl = new Return();
                _returnTbl = entRMA.Returns.SingleOrDefault(rtn => rtn.ReturnID == ObjReturnTbl.ReturnID);
                //Insert new record if not persernt perviously
                if (_returnTbl == null)
                {
                    entRMA.AddToReturns(ObjReturnTbl);
                }
                else // Update existing record if present.
                {
                    _returnTbl = ObjReturnTbl;
                }
                entRMA.SaveChanges();
                _returnFlag = true;
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
