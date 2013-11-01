using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;

namespace KrausRGA.DBLogics
{
    /// <summary>
    /// Avinash : 1Nov2013
    /// User Get Set functions and other user related functions.
    /// that is performed on RMASYSTEM database.
    /// </summary>
   public class cmdUser
    {

       /// <summary>
       /// RMASYSTEM database object.
       /// </summary>
       RMASYSTEMEntities entRMADB = new RMASYSTEMEntities();

        #region Get Operation for User Table.

       /// <summary>
       /// This will give you all users information. 
       /// Withaout any arguments or parameters.
       /// </summary>
       /// <returns>
       /// list of user class information.
       /// </returns>
       public List<User> GetUserTbl()
       {
           //Return list object.
           List<User> _lsUserReturn = new List<User>();
           try
           {
               //Select all users from Database.
               var UserList = from ent in entRMADB.Users
                               select ent;

               //Add each user information to the return list.
               foreach (var Useritem in UserList)
               {
                   User user = new User();
                   user = (User)Useritem;
                   _lsUserReturn.Add(user);
               }

           }
           catch (Exception)
           {}

           return _lsUserReturn;
       }

       /// <summary>
       /// Get user information by his Guid.
       /// </summary>
       /// <param name="Userid">
       /// Guid UserID. 
       /// </param>
       /// <returns>
       /// User class with user informatipn its null if information not found about user.
       /// </returns>
       public User GetUserTbl(Guid Userid)
       {
           User _lsUserReturn = new User();
           try
           {
               _lsUserReturn = entRMADB.Users.FirstOrDefault(i=>i.UserID == Userid);
           }
           catch (Exception)
           {}
           return _lsUserReturn;

       }

       /// <summary>
       /// User information get form user table
       /// whith parameter String UserName which is login username
       /// not name of the user.
       /// </summary>
       /// <param name="LoginUserName">
       /// String Login user Name not User name.
       /// </param>
       /// <returns>
       /// User class object its null if user information not found.
       /// </returns>
       public User GetUserTbl(String LoginUserName)
       {
           User _return = new User();
           try
           {
               _return = entRMADB.Users.FirstOrDefault(user => user.UserName == LoginUserName);
           }
           catch (Exception)
           {}
           return _return;
       }

       /// <summary>
       /// this gives user information filterd by RoleID
       /// </summary>
       /// <param name="RoleID">
       /// Guild Role ID.
       /// </param>
       /// <returns>
       /// List of user information .
       /// </returns>
       public List<User> GetUserByRoleID(Guid RoleID)
       {
           List<User> _lsReturn = new List<User>();
           try
           {
               var userInfo = from user in entRMADB.Users
                              where user.RoleId == RoleID
                              select user;

               foreach (var useritem in userInfo)
               {
                   User _user = new User();
                   _user = (User)useritem;

                   //Add to return list.
                   _lsReturn.Add(_user);
               }
           }
           catch (Exception)
           {}
           return _lsReturn;
       }
       
        #endregion

    }
}
