using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel.ShippingManagerFunctions;

namespace KrausRGA.EntityModel.ShippingManagerFunctions
{
    /// <summary>
    /// Avinash :22 oct 2013
    /// shipping table commands.
    /// GetData
    /// </summary>
  public  class cmdShipping
    {
        //Shipping Manager Database Object
        Shipping_ManagerEntities entShippingManager = new Shipping_ManagerEntities();

        /// <summary>
        /// Shipping table search for shipping Number.
        /// </summary>
        /// <param name="ShippingNumber">
        /// String uppar Case shipping Number.
        /// </param>
        /// <returns>
        /// Shipping table Object.
        /// </returns>
        public  Shipping GetData(String ShippingNumber)
        {
            Shipping shipping = new Shipping();
            try
            {
                shipping = entShippingManager.Shippings.FirstOrDefault(i => i.ShippingNum == ShippingNumber);
            }
            catch (Exception)
            {
            }
            return shipping;
        }
    }
}
