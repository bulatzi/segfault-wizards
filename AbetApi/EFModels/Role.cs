using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Data;
using System.Text.Json.Serialization;

namespace AbetApi.EFModels
{
    public class Role
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int RoleId { get; set; }
        public String Name { get; set; }
        [JsonIgnore]
        public ICollection<User> Users { get; set; }
        public Role()
        {
            this.Users = new List<User>();
        }
        public Role(String Name)
        {
            this.Name = Name;
        }

        // This function creates a role with the given name
        // Naming convention is the role name starting with uppercase
        public static async Task CreateRole(Role role)
        {
            //Check that the role name is not null or empty.
            if (role.Name == null || role.Name == "")
            {
                throw new ArgumentException("The role name cannot be empty.");
            }

            //Format role name to follow a standard
            role.Name = role.Name[0].ToString().ToUpper() + role.Name[1..].ToLower();

            // Adds role to the DB.
            await using (var context = new ABETDBContext())
            {
                //Try to find the role in the database.
                Role duplicateRole = context.Roles.FirstOrDefault(p => p.Name == role.Name);

                //Check that the new role to be created is not a duplicate. Throw an exception if it does.
                if(duplicateRole != null)
                {
                    throw new ArgumentException("That role already exists in the database.");
                }

                context.Roles.Add(role);
                context.SaveChanges();
            }
        } // CreateRole

        // This function gives a selected user a provided role.
        public async static Task AddRoleToUser(string EUID, string roleName)
        {
            //Check that the EUID of the user is not null or empty.
            if (EUID == null || EUID == "")
            {
                throw new ArgumentException("The EUID cannot be empty.");
            }

            //Check that the role name is not null or empty.
            if (roleName == null || roleName == "")
            {
                throw new ArgumentException("The role name cannot be empty.");
            }

            //Format role name and EUID to follow a standard.
            roleName = roleName[0].ToString().ToUpper() + roleName[1..].ToLower();
            EUID = EUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                //This finds a role with the given role name.
                Role role = context.Roles.FirstOrDefault(r => r.Name == roleName);

                //If the specified role does not exist then throw an exception.
                if(role == null)
                {
                    throw new ArgumentException("The role specified does not exist in the database.");
                }

                //Find the User if it exists.
                User user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                //If the specified user does not exist then throw an exception.
                if(user == null)
                {
                    throw new ArgumentException("The user specified does not exist in the database.");
                }

                //Load the roles for the specified user.
                context.Entry(user).Collection(user => user.Roles).Load();

                //Check that the user does not already have the role specified and if they do, then throw an exception.
                foreach (Role duplicateRole in user.Roles)
                {
                    if(duplicateRole.Name == role.Name)
                    {
                        throw new ArgumentException("The user specified already has the role specified.");
                    }
                }

                // The role is added to the user, and the changes are saved.
                role.Users.Add(user);
                context.SaveChanges();
            }
        } // AddRoleToUser

        // Gets a list of users with the selected role
        public static async Task<List<User>> GetUsersByRole(string roleName)
        {
            //Check that the role name is not null or empty.
            if (roleName == null || roleName == "")
            {
                throw new ArgumentException("The role name cannot be empty.");
            }

            //Format role name and EUID to follow a standard.
            roleName = roleName[0].ToString().ToUpper() + roleName[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                //This finds a role with the given role name.
                Role role = context.Roles.FirstOrDefault(r => r.Name == roleName);

                //If the specified role does not exist then throw an exception.
                if(role == null)
                {
                    throw new ArgumentException("The role specified does not exist in the database.");
                }

                //This uses explicit loading to tell the database we want Users loaded.
                context.Entry(role).Collection(role => role.Users).Load();

                // Converts the users of that role in to a list and returns the list.
                return role.Users.ToList();
            }
        } // GetUsersByRole

        // This function deletes a selected role
        public static async Task DeleteRole(string roleName)
        {
            //Check that the role name is not null or empty.
            if (roleName == null || roleName == "")
            {
                throw new ArgumentException("The role name cannot be empty.");
            }

            //Format role name and EUID to follow a standard.
            roleName = roleName[0].ToString().ToUpper() + roleName[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                //This finds a role with the given role name.
                Role role = context.Roles.FirstOrDefault(r => r.Name == roleName);

                //If the specified role does not exist then throw an exception.
                if (role == null)
                {
                    throw new ArgumentException("The role specified does not exist in the database.");
                }

                //Delete the role, and save changes
                context.Remove(role);
                context.SaveChanges();
            }
        } // DeleteRole

        // This function removes a role from a user, selected via EUID
        public async static Task RemoveRoleFromUser(string EUID, string roleName)
        {
            //Boolean variable for determining if a user has the role specified.
            bool userHasRole = false;

            //Check that the EUID of the user is not null or empty.
            if (EUID == null || EUID == "")
            {
                throw new ArgumentException("The EUID cannot be empty.");
            }

            //Check that the role name is not null or empty.
            if (roleName == null || roleName == "")
            {
                throw new ArgumentException("The role name cannot be empty.");
            }

            //Format role name and EUID to follow a standard.
            roleName = roleName[0].ToString().ToUpper() + roleName[1..].ToLower();
            EUID = EUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                //This finds a role with the given role name.
                Role role = context.Roles.FirstOrDefault(r => r.Name == roleName);

                //If the specified role does not exist then throw an exception.
                if (role == null)
                {
                    throw new ArgumentException("The role specified does not exist in the database.");
                }

                //Find the User if it exists.
                User user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                //If the specified user does not exist then throw an exception.
                if (user == null)
                {
                    throw new ArgumentException("The user specified does not exist in the database.");
                }

                //Load the roles for the specified user.
                context.Entry(user).Collection(user => user.Roles).Load();

                //Check that the user does not have the role specified.
                foreach (Role hasRole in user.Roles)
                {
                    if (hasRole.Name == role.Name)
                    {
                        userHasRole = true;
                    }
                }

                //Throw an exception if the user does not have the role specified.
                if(!userHasRole)
                {
                    throw new ArgumentException("The user specified does not have the role specified.");
                }

                //This uses explicit loading to tell the database we want role.Users loaded
                context.Entry(role).Collection(role => role.Users).Load();

                //This removes the user from the join table
                role.Users.Remove(user);
                context.SaveChanges();
            }
        } // RemoveRoleFromUser
    } // Role
}