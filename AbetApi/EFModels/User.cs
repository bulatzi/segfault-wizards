using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AbetApi.EFModels
{
    public class User
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EUID { get; set; }
        [JsonIgnore]
        public ICollection<Role> Roles { get; set; }

        public User()
        {
            this.Roles = new List<Role>();
        }

        // THROWS AN INNER EXCEPTION THAT MIGHT NEED TO BE RETHROWN FOR DETAILS
        // This function creates a user profile in the DB with the given information
        // EUID must be unique from other users in the DB
        public async static Task AddUser(User User)
        {
            //Sets the user ID to 0, to allow the database to auto increment the UserId value
            User.UserId = 0;

            //FIXME - Add error checking. User requires a unique EUID that breaks if you try to save changes with 2 of the same EUID.
            await using (var context = new ABETDBContext())
            {
                context.Users.Add(User);
                context.SaveChanges();
            }
        } // AddUser

        //This function finds the user with the EUID of the first input argument,
        //and updates their information with NewUserInfo
        public async static Task EditUser(string EUID, User NewUserInfo)
        {
            //Sets the user ID to 0, to allow the database to auto increment the UserId value
            NewUserInfo.UserId = 0;

            await using (var context = new ABETDBContext())
            {
                //find the user with a matching EUID
                var user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                //Copy new values of user over to the user that's being edited
                user.FirstName = NewUserInfo.FirstName;
                user.LastName = NewUserInfo.LastName;
                user.EUID = NewUserInfo.EUID;
                context.SaveChanges();
            }
        } // EditUser

        // This function deletes a selected user
        // Anybody calling this function should make sure you want to call this function. Deletions are final.
        public async static Task DeleteUser(string EUID)
        {
            await using (var context = new ABETDBContext())
            {
                //find the user with a matching EUID
                var user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                //Delete the result, and save changes
                context.Remove(user);
                context.SaveChanges();
            }
        } // DeleteUser

        // This function returns user information for the provided EUID
        public async static Task<User> GetUser(string EUID)
        {
            await using (var context = new ABETDBContext())
            {
                //find the user with a matching EUID
                var user = context.Users.FirstOrDefault(p => p.EUID == EUID);
                return user;
            }
        } // GetUser

        public User(string FirstName, string LastName, string EUID)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.EUID = EUID;
        }

        // Gets a list of roles from the selected user
        public static async Task<List<Role>> GetRolesByUser(string EUID)
        {
            await using (var context = new ABETDBContext())
            {
                // Finds the user
                var user = context.Users.FirstOrDefault(u => u.EUID == EUID);
                if (user == null)
                    return null;

                //This uses explicit loading to tell the database we want Roles loaded
                context.Entry(user).Collection(user => user.Roles).Load();

                // Converts the roles of that user in to a list and returns the list.
                var output = user.Roles.ToList();
                return output;
            }
        } // GetRolesByUser
    } // User
}
