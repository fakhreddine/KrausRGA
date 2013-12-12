using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrausRGA.EntityModel;
using KrausRGA.ErrorLogger;

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
      // RMASYSTEMEntities entRMADB = new RMASYSTEMEntities();
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
               var UserList = from ent in Service.entGet.UserAll()
                               select ent;

               //Add each user information to the return list.
               foreach (var Useritem in UserList)
               {
                   User user = new User(Useritem);
                   _lsUserReturn.Add(user);
               }

           }
           catch (Exception ex)
           {
               ex.LogThis("cmdUser/GetUserTbl");
           }

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
               _lsUserReturn =new User(Service.entGet.UserAll().FirstOrDefault(i=>i.UserID == Userid));
           }
           catch (Exception ex)
           {
               ex.LogThis("cmdUser/GetUserTbl(Guid Userid)");
           }
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
               _return =new User(Service.entGet.UserByUserName(LoginUserName));//.Users.FirstOrDefault(user => user.UserName == LoginUserName);
           }
           catch (Exception ex)
           {
               ex.LogThis("cmdUser/GetUserTbl(String LoginUserName)");
           }
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
             
               var userInfo = Service.entGet.UserByRoleID(RoleID);

               foreach (var useritem in userInfo)
               {
                   _lsReturn.Add(new User(useritem));
               }
           }
           catch (Exception ex)
           {
               ex.LogThis("cmdUser/GetUserByRoleID");
           }
           return _lsReturn;
       }

       /// <summary>
       /// Gives user information from user master table
       /// User Name (Login Name )and its password.
       /// </summary>
       /// <param name="UserName">
       /// String UserName (Ligin Name Not Name)
       /// </param>
       /// <param name="Password">
       /// String Password .
       /// </param>
       /// <returns>
       /// User class object with information yield null.
       /// </returns>
       public User GetUserByUserNamePassword(String UserName, String Password)
       {
           User _userReturn = new User();

           try
           {
               var username = Service.entGet.UserByUserName(UserName);
               if (username.UserName !=null)
               {
                   if (username.UserPassword == Password) _userReturn = new User(username);
               }
           }
           catch (Exception ex)
           {
               ex.LogThis("cmdUser/GetUserByUserNamePassword");
           }

           return _userReturn;
       }

        #endregion

    }
}
