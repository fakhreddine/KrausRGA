using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KrausRGA.EntityModel.ShippingManagerFunctions
{

    /// <summary>
    /// Avinash - 22 Oct 2013
    /// Abstract class for all Get methods
    /// </summary>
    abstract public class AbstractClassMethods
    {
        /// <summary>
        /// Get Data From database
        /// </summary>
        /// <typeparam name="T">
        /// Return Type Object.
        /// </typeparam>
        /// <returns>
        /// Object of return type
        /// </returns>
        abstract public T GetData<T>();
        
    }
}
