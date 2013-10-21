using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausGRA.ViewModels
{
    /// <summary>
    /// Avinash-  Date : Oct 22, 2013.
    /// Model For scanned value search and its related functions
    /// </summary>
  public class mScanned
    {

      public string ScannedNumber { get; set; }
      

      /// <summary>
      /// Search value scanned in to Database and Defin the Type Of value Enum
      /// </summary>
      /// <param name="ScannedValue">
      /// String Scanned Value.
      /// </param>
      /// <returns>
      /// Enum ScannedValueType.
      /// </returns>
      public String SearchValueIntoDatabase(String ScannedValue)
      {
          String _dbFoundValue = "";
          try
          {

          }
          catch (Exception)
          {}
          return _dbFoundValue;
 
      }
        


    }
}
