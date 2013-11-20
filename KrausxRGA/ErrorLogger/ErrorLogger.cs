using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KrausRGA.ErrorLogger
{
    /// <summary>
    /// Avinash : 14 Nov : Kraus GRA.
    /// Error logger extention all project catch.
    /// loggs error in text file named as RGAErrors.txt.
    /// </summary>
    public static class ErrorLogger
    {
        /// <summary>
        /// Static path of error log file.
        /// </summary>
        static readonly String _FilePath = "C:\\RGAErrorLog.txt";
        
        /// <summary>
        /// Log error to the text file on path "C:\\RGAErrorLog.txt"
        /// This method is with User ID
        /// </summary>
        /// <param name="Ex_message">
        /// String Error message from the catch.
        /// </param>
        /// <param name="UserID">
        /// Guid user Id for which Error caught.
        /// </param>
        /// <param name="ErrorLocation">
        /// String Error location.( this may be function location with file and class name.)
        /// </param>
        public static void LogThis(this String Ex_message, Guid UserID, String ErrorLocation )
        {
            String[] Lines = { "", "",};
            Lines[0] = DateTime.Now.ToString("MMM dd, yyy hh:mm.fff tt") + " ==> User ID : " + UserID + "Error Location : " + ErrorLocation;
            Lines[1] = Ex_message;
            try
            {
                //append lines to the file.
                File.AppendAllLines(_FilePath,Lines);
            }
            catch (FileNotFoundException)
            {
                //on file not found create new file.
                File.Create(_FilePath);

                //Recursive call to same function after file created.
                LogThis(Ex_message, UserID, ErrorLocation);
            }
        }

        /// <summary>
        /// Log error to the text file on path "C:\\RGAErrorLog.txt"
        /// </summary>
        /// <param name="Ex_message">
        /// String Error message from the catch.
        /// </param>
        /// <param name="UserID">
        /// Guid user Id for which Error caught.
        /// </param>
        /// <param name="ErrorLocation">
        /// String Error location.( this may be function location with file and class name.)
        /// </param>
        public static void LogThis(this String Ex_message, String ErrorLocation)
        {
            String[] Lines = { "", "", };
            Lines[0] = DateTime.Now.ToString("MMM dd, yyy hh:mm.fff tt") + " ==> Error Location : " + ErrorLocation;
            Lines[1] = Ex_message;
            try
            {
                //append lines to the file.
                File.AppendAllLines(_FilePath, Lines);
            }
            catch (FileNotFoundException)
            {
                //on file not found create new file.
                File.Create(_FilePath);

                //Recursive function call.
                LogThis(Ex_message, ErrorLocation);
            }
        }

    }
}
