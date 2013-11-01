using KrausRGA.DBLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;

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

       #endregion

      #region class properties

      public User UserInfo { get; protected set; }

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

              if (user != null)
              {
                  UserInfo = user;
                  _FlagReturn = true;
              }
          }
          catch (Exception)
          { }
          return _FlagReturn;
      }

      #endregion

      


    }
}
