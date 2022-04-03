using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

//! The EFModels namespace
/*! 
 * This namespace falls under the AbetAPI namespace, and is for EFModels.
 * The EFModels are generally called from the Controllers namespace, to 
 * provide the controllers functionality, ultimately giving endpoints/functionality
 * for the UI elements
 */
namespace AbetApi.EFModels
{
    //! The User Class
    /*! 
     * This class gets called by the UsersController class
     * and provides functions to get and return data
     */
    public class User
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //! The UserID setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public int UserId { get; set; }
        //! The FirstName setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string FirstName { get; set; }
        //! The LastName setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string LastName { get; set; }
        //! The EUID setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string EUID { get; set; }
        [JsonIgnore]
        //! The Roles setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public ICollection<Role> Roles { get; set; }

        //This is a constructor to create a user object from the given name and EUID.
        //! Paramaterized Constructor
        /*! 
         * This is a constructor to create a user object from the given name and EUID.
         * \param FirstName First name of the user
         * \param LastName Last name of the user
         * \param EUID The user's EUID
         */
        public User(string FirstName, string LastName, string EUID)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.EUID = EUID;
        }

        //! Constructor
        /*! 
         * This Constructor builds a user with roles
         */
        public User()
        {
            this.Roles = new List<Role>();
        }

        //! The AddUser Function
        /*!
         * This function creates a user profile in the DB with the given information. EUID must be unique from other users in the DB
         * It is an async Task to pass exceptions to the Controllers.UserController in Controllers
         * It first sets the user Id to 0 so that the entity framwork auto increment the value.
         * It then checks that the first name of the user is not null or empty, same with the lasst name, and EUID.
         * It formats the first and last name, and the EUID to follow a standard. It then opens a context with the database,
         * makes changes, and saves the changes. In this process, it will check for duplicate users.
         * \param User A user object holding the relevent data for a user (First name, Last name, EUID).
         */
        public async static Task AddUser(User User)
        {
            //Sets the user ID to 0, to allow the database to auto increment the UserId value
            User.UserId = 0;
            
            //Check that the first name of the user is not null or empty.
            if(User.FirstName == null || User.FirstName == "")
            {
                throw new ArgumentException("The first name cannot be empty.");
            }

            //Check that the last name of the user is not null or empty.
            if(User.LastName == null || User.LastName == "")
            {
                throw new ArgumentException("The last name cannot be empty.");
            }

            //Check that the EUID of the user is not null or empty.
            if(User.EUID == null || User.EUID == "")
            {
                throw new ArgumentException("The EUID cannot be empty.");
            }

            //Format first name, last name, and EUID to follow a standard.
            User.FirstName = User.FirstName[0].ToString().ToUpper() + User.FirstName.Substring(1);
            User.LastName = User.LastName[0].ToString().ToUpper() + User.LastName.Substring(1);
            //When formatting EUID I want to ask Ludi if an EUID will always be three letters followed by 4 numbers, or if we don't care to check formatting that specifically.
            User.EUID = User.EUID.ToLower();

            //Opens a context with the database, makes changes, and saves the changes
            await using (var context = new ABETDBContext())
            {
                //Try to find the user in the database.
                User duplicateUser = context.Users.FirstOrDefault(p => p.EUID == User.EUID);

                //Check the new user to be created does not copy an EUID already being used in the datbaase and throw an exception if it does.
                if (duplicateUser != null)
                {
                    throw new ArgumentException("A user with that EUID already exists in the database.");
                }    

                context.Users.Add(User);
                context.SaveChanges();
            }
        } // AddUser

        //! The GetUser Function
        /*!
         * This function returns user information for the provided EUID. 
         * It is an async Task<User> to pass exceptions and a user object to the Controllers.UserController in Controllers.
         * It first checks for a null input (EUID), then formats the input to a standard, and checks for the user specifiedd by the input.
         * \param EUID A string holding the EUID of the desired user
         */
        public async static Task<User> GetUser(string EUID)
        {
            //Check that the EUID of the user is not null or empty.
            if (EUID == null || EUID == "")
            {
                throw new ArgumentException("The EUID cannot be empty.");
            }

            //Format EUID to follow a standard.
            EUID = EUID.ToLower();

            //Try to find the specified user.
            await using (var context = new ABETDBContext())
            {
                User user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                //If the specified user does not exist, then throw an exception.
                if (user == null)
                {
                    throw new ArgumentException("The user specified does not exist in the database.");
                }

                return user;
            }
        } // GetUser

        //! The EditUser Funciton
        /*!
         * This function finds the user with the EUID of the first input argument, and updates their information with NewUserInfo.
         * It is an async Task to pass exceptions to the Controllers.UserController in Controllers.
         * It first sets the user ID to 0 for entity framework to auto increment.
         * It then checks for null inputs, then formats the input if not null.
         * Then it tries to find the user with the EUID and checks if the change will make a duplicate. Otherwise, makes the changes and saves.
         * \param EUID The users EUID
         * \param NewUserInfo A User object with the edit info
         */
        public async static Task EditUser(string EUID, User NewUserInfo)
        {
            //Sets the user ID to 0, to allow the database to auto increment the UserId value
            NewUserInfo.UserId = 0;

            //Check that the EUID of the existing user information is not null or empty.
            if (EUID == null || EUID == "")
            {
                throw new ArgumentException("The EUID for the user to edit cannot be empty.");
            }

            //Check that the first name of the new user information is not null or empty.
            if (NewUserInfo.FirstName == null || NewUserInfo.FirstName == "")
            {
                throw new ArgumentException("The new first name cannot be empty.");
            }

            //Check that the last name of the new user information is not null or empty.
            if (NewUserInfo.LastName == null || NewUserInfo.LastName == "")
            {
                throw new ArgumentException("The new last name cannot be empty.");
            }

            //Check that the EUID of the new user information is not null or empty.
            if (NewUserInfo.EUID == null || NewUserInfo.EUID == "")
            {
                throw new ArgumentException("The new EUID cannot be empty.");
            }

            //Format first name, last name, and EUID of the new user information to follow a standard.
            NewUserInfo.FirstName = NewUserInfo.FirstName[0].ToString().ToUpper() + NewUserInfo.FirstName.Substring(1);
            NewUserInfo.LastName = NewUserInfo.LastName[0].ToString().ToUpper() + NewUserInfo.LastName.Substring(1);
            NewUserInfo.EUID = NewUserInfo.EUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                //find the user with a matching EUID
                User user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                //Check to make sure the specified user to work on was a valid user from the database and throw an exception if it was not.
                if(user == null)
                {
                    throw new ArgumentException("The user you wanted to edit does not exist in the database.");
                }

                //Try to find the specified user.
                User duplicateUser = context.Users.FirstOrDefault(p => p.EUID == NewUserInfo.EUID);

                //If the new user EUID already exists in the database, then that is a duplicate and we do not allow duplicates.
                if (duplicateUser != null)
                {
                    throw new ArgumentException("The EUID to change to already exists in the database.");
                }

                //Copy new values of user over to the user that's being edited
                user.FirstName = NewUserInfo.FirstName;
                user.LastName = NewUserInfo.LastName;
                user.EUID = NewUserInfo.EUID;
                context.SaveChanges();
            }
        } // EditUser

        //! The DeleteUser Function
        /*!
         * This function deletes a selected user. Anybody calling this function should make sure you want to call this function. Deletions are final.
         * It is an async Task to pass exceptions to the Controllers.UserController in Controllers.
         * It first checks for null inputs, and otherwise formats it to a standard. 
         * It then checks if the user exists, and if so, deletes them and saves the changes.
         * \param EUID A string with the users EUID
         */
        public async static Task DeleteUser(string EUID)
        {
            //Check that the EUID of the user to delete is not null or empty.
            if (EUID == null || EUID == "")
            {
                throw new ArgumentException("The EUID cannot be empty.");
            }

            //Format EUID to follow a standard.
            EUID = EUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                //find the user with a matching EUID
                User user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                //Check to make sure the specified user to work on was a valid user from the database and throw an exception if it was not.
                if (user == null)
                {
                    throw new ArgumentException("The user you wanted to delete does not exist in the database.");
                }

                //Delete the result, and save changes
                context.Remove(user);
                context.SaveChanges();
            }
        } // DeleteUser

        //! The GetRolesByUser Function
        /*!
         * This function gets the roles of a user in a list format.
         * It is an async Task<List<Role>> to pass exceptions and a user.Roles object, in list form, to the Controllers.UserController in Controllers.
         * It first checks for null inputs, then formats the input to a standard
         * It checks that the user exists, then collects the users roles into a list and returns it
         * \param EUID A string with the users EUID
         */
        public static async Task<List<Role>> GetRolesByUser(string EUID)
        {
            //Check that the EUID of the user to find is not null or empty.
            if (EUID == null || EUID == "")
            {
                throw new ArgumentException("The new EUID cannot be empty.");
            }

            //Format EUID to follow a standard.
            EUID = EUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                // Finds the user
                User user = context.Users.FirstOrDefault(u => u.EUID == EUID);

                //Throw an exception if the user specified does not exist.
                if (user == null)
                {
                    throw new ArgumentException("The user specified does not exist in the database.");
                }

                //This uses explicit loading to tell the database we want Roles loaded
                context.Entry(user).Collection(user => user.Roles).Load();

                // Converts the roles of that user in to a list and returns the list.
                return user.Roles.ToList();
            }
        } // GetRolesByUser
    } // User
}
