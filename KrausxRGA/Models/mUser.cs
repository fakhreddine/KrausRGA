using KrausRGA.DBLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.Views;
using KrausRGA.ErrorLogger;

namespace KrausRGA.Models
{
    /// <summary>
    /// Avinash : 1 Nov 2013.
    /// contains user information which is logged-in.
    /// </summary>
   public class mUser
   {
       #region Command Objects

       //User command Objects
      protected cmdUser _cuser = new cmdUser();

       /// <summary>
       /// Role Command object.
       /// </summary>
      protected cmdRoles _cRoles = new cmdRoles();

       #endregion

      #region class properties

      public User UserInfo { get; protected set; }

      public String RoleName { get; set; }
      #endregion

      #region Member Functions of class.
      
       /// <summary>
       /// check that user is vliad to login or not.
       /// if user is valid then UserInfo property get field.
       /// </summary>
       /// <param name="UserName">
       /// String UserName (loginName) 
       /// </param>
       /// <param name="Password">
       /// String Password.
       /// </param>
       /// <returns>
       /// Boolean Value indicationg User is Valid or not.
       /// </returns>
       public Boolean IsValidUser(String UserName, String Password)
      {
          Boolean _FlagReturn = false;
          try
          {
              User user = new User();
              user = _cuser.GetUserByUserNamePassword(UserName, Password);
              if (user.UserName != null)
              {
                  UserInfo = user;
                  RoleName = _cRoles.GetRole(user.RoleId).Name.ToString();
                  _FlagReturn = true;
              }
          }
          catch (Exception ex)
          {
              ex.LogThis("mUser/IsValidUser");
          }
          return _FlagReturn;
      }
      
       /// <summary>
       /// Permission to acces specific object from the Application.
       /// </summary>
       /// <param name="IsPermission">
       /// Enum of type ePermission.
       /// </param>
       /// <returns>
       /// Boolean value indicating permited to access not.
       /// </returns>
       public Boolean IsPermitedTo(ePermissione IsPermission)
       {
           Boolean _return = false;
               try
               {
                   //Chech that user is valid and its information is persent in the database.
                   if (!_cRoles.IsSuperUser(UserInfo.UserID))
                   {
                       if (UserInfo.UserName != "" || UserInfo.UserName != null)
                       {
                           //check that given enum type is persent int the action allowed to do for user.
                           //if (UserInfo.Role.Action.Contains(IsPermission.ToString()))
                           // {
                           //set rerutn value falg to true if its found.
                           _return = true;
                           //}
                       } 
                   }
                   else
                   {
                       _return = true;
                   }
               }
               catch (Exception ex)
               {
                   ex.LogThis("mUser/IsPermitedTo");
               }
           return _return;
       }

       public mUser()
       {

          
       }
      #endregion

    }
}
