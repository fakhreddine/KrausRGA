using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.Models
{
    /// <summary>
    /// Avainsh : 30-oct 2013
    /// Model for Entered Number validations, 
    /// also all information about that number.
    /// </summary>
   public class mReturnDetails
    {
        /// <summary>
        /// Enterd Number.
        /// </summary>
        protected static string _SRNumber { get; set; }

        /// <summary>
        /// Paramerised constructor for calss
        /// </summary>
        /// <param name="SRNumber">
        /// String Enterd number for 
        /// </param>
         public mReturnDetails(String SRNumber)
        {
            _SRNumber = SRNumber;
        }

        /// <summary>
        /// Scanned Number is valid or not. 
        /// this is checked in x3v6 database 
        /// also you can add user validation to ented number validate.
        /// </summary>
        /// <param name="SRNumber">
        /// String SRNumber entered
        /// </param>
        /// <returns>
        /// Boolean value true if valid enterd number else false.
        /// </returns>
        protected Boolean IsValidNumberEntred(String SRNumber)
        {
            Boolean _isNumberValid = false;

            try
            {

            }
            catch (Exception)
            { }

            return _isNumberValid;
        }



    }
}
