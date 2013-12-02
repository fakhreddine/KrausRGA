using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.Models;
using KrausRGA.EntityModel;

namespace KrausRGA.DBLogics
{
    /// <summary>
    /// Avinash: 1Nov2013
    /// Role information get set functions for database.
    /// </summary>
   public class cmdRoles
    {
       /// <summary>
       /// RMA Database ovbject.
       /// </summary>
      // RMASYSTEMEntities entRMA = new RMASYSTEMEntities();

        #region Get fucntions of Role class.

       /// <summary>
       /// Gives the whole table information of user.
       /// </summary>
       /// <returns>
       /// List of user onformation
       /// </returns>
       public List<Role> GetRoles()
       {
           List<Role> _lsRuturn = new List<Role>();

           try
           {
               var Rolesinfo = cmd.entGet.RoleAll();
               foreach (var roleitem in Rolesinfo)
               {
                   Role roles = new Role(roleitem);
                   //roles = (Role)Rolesinfo;
                   _lsRuturn.Add(roles);
               }
           }
           catch (Exception)
           {}

           return _lsRuturn;
       }

       /// <summary>
       /// Gives the role information filtred by RoleID
       /// </summary>
       /// <param name="RoleID">
       /// String RoleID.
       /// </param>
       /// <returns>
       /// Role Class Object with filtred RoleID Information
       /// else Null.
       /// </returns>
       public Role GetRole(Guid RoleID)
       {
           Role role = new Role();
           try
           {
               role = new Role(cmd.entGet.RoleByRoleID(RoleID));
           }
           catch (Exception)
           { }
           return role;
       }

        #endregion

    }
}
