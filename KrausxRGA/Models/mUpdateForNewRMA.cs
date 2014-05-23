using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.DBLogics;


namespace KrausRGA.Models
{
    public class mUpdateForNewRMA
    {
        public Return _ReturnTbl1 { get; protected set; }

        public List<ReturnDetail> _lsReturnDetails1 { get; protected set; }

        public List<SKUReason> _lsReasons1 { get; protected set; }

        public List<ReturnImage> _lsImages1 { get; protected set; }

        public List<ReturnedSKUPoints> _lsskuandpoints { get; protected set; }


     

        /// <summary>
        /// sage operations command class object.
        /// </summary>
        protected DBLogics.cmdSageOperations cSage = new DBLogics.cmdSageOperations();

        /// <summary>
        /// Return Tables command class.
        /// </summary>
        protected DBLogics.cmdReturn cReturnTbl = new DBLogics.cmdReturn();

        /// <summary>
        ///ReturnDetails Tables commad class object.
        /// </summary>
        protected DBLogics.cmdReturnDetail cRetutnDetailsTbl = new DBLogics.cmdReturnDetail();

        /// <summary>
        /// RetutnImages Table Command class Object.
        /// </summary>
        protected DBLogics.cmdReturnImages cRtnImages = new DBLogics.cmdReturnImages();

        /// <summary>
        /// Reasons Table Command Object.
        /// </summary>
        protected DBLogics.cmdReasons cRtnreasons = new DBLogics.cmdReasons();

        /// <summary>
        /// Reasoncategory table Command Object
        /// </summary>
        protected DBLogics.cmdReasonCategory cReasonCategory = new DBLogics.cmdReasonCategory();

        /// <summary>
        /// Transaction table Command object 
        /// </summary>
        protected DBLogics.cmdSKUReasons cSkuReasons = new DBLogics.cmdSKUReasons();

        protected DBLogics.cmdReturnedSKUPoints cSkupoints = new DBLogics.cmdReturnedSKUPoints();

        public mUpdateForNewRMA()
        {

        }
        public mUpdateForNewRMA(String NewRGANumber)
        {
            _lsImages1 = new List<ReturnImage>();
            _lsReasons1 = new List<SKUReason>();
            GetReturnTbl(NewRGANumber);
            GetLsReturnDetails(_ReturnTbl1.ReturnID);
            GetReasons(_lsReturnDetails1);
            GetRerurnImages(_lsReturnDetails1);
            GetSKUAndPointsByReturnID(_ReturnTbl1.ReturnID);

        }

        protected void GetReturnTbl(String RGANumber)
        {
            try
            {
                List<Return> rt = new List<Return>();
                // _ReturnTbl = cReturnTbl.GetReturnTblByPONumber(RMANumber);

                var listReturnTbl = Service.entGet.ReturnByRGAROWID(RGANumber).ToList();
                foreach (var lsitem in listReturnTbl)
                {
                    rt.Add(new Return(lsitem));
                }
                _ReturnTbl1 = rt.FirstOrDefault(i => i.RGAROWID == RGANumber);
                //_ReturnTbl = rt.SingleOrDefault(i=


            }
            catch (Exception)
            { }
        }

        protected void GetLsReturnDetails(Guid ReturntblID)
        {
            try
            {
                _lsReturnDetails1 = cRetutnDetailsTbl.GetReturnDetailsByReturnID(ReturntblID);
            }
            catch (Exception)
            { }
        }

        protected void GetReasons(List<ReturnDetail> LsRetnDetails)
        {
            try
            {
                foreach (var lsitem in LsRetnDetails)
                {

                    foreach (var item in cSkuReasons.GetSKuReasonsByReturnDetailsID(lsitem.ReturnDetailID))
                    {
                        _lsReasons1.Add(item);
                    }
                }
            }
            catch (Exception)
            { }
        }

        protected void GetRerurnImages(List<ReturnDetail> lsRetnDetails)
        {
            try
            {
                foreach (var Rditem in lsRetnDetails)
                {

                    foreach (var Imgitem in cRtnImages.GetReturnImagesByReturnDetailsID(Rditem.ReturnDetailID))
                    {
                        _lsImages1.Add(Imgitem);
                    }
                }
            }
            catch (Exception)
            { }
        }



        protected void GetSKUAndPointsByReturnID(Guid ReturntblID)
        {
            try
            {
                _lsskuandpoints = cSkupoints.GetReturnedSKUPointsByReturnID(ReturntblID);
            }
            catch (Exception)
            { }
        }
        protected void GetSKUAndPointsByReturnDetailID(Guid ReturntDetailID)
        {
            try
            {
                _lsskuandpoints = cSkupoints.GetReturnedSKUPointsByReturnDetailID(ReturntDetailID);
            }
            catch (Exception)
            { }
        }



    }
}
