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

        // SHOULD PROBABLY NOT ALL NULL AS AN INPUT
        // This function creates a role with the given name
        // Naming convention is the role name starting with uppercase
        public static async Task CreateRole(string RoleName)
        {
            // Creates role
            Role role = new Role(RoleName);

            // Adds role to the DB.
            await using (var context = new ABETDBContext())
            {
                context.Roles.Add(role);
                context.SaveChanges();

                return;
            }
        } // CreateRole

        // This function deletes a selected role
        public static async Task DeleteRole(string roleName)
        {
            await using (var context = new ABETDBContext())
            {
                //Finds the role
                var role = context.Roles.FirstOrDefault(r => r.Name == roleName);

                //Delete the role, and save changes
                context.Remove(role);
                context.SaveChanges();

                return;
            }
        } // DeleteRole

        // Gets a list of users with the selected role
        public static async Task<List<User>> GetUsersByRole(string roleName)
        {
            await using (var context = new ABETDBContext())
            {
                // Finds the role
                var role = context.Roles.FirstOrDefault(r => r.Name == roleName);

                //This uses explicit loading to tell the database we want Users loaded
                context.Entry(role).Collection(role => role.Users).Load();

                // Converts the users of that role in to a list and returns the list.
                var output = role.Users.ToList();
                return output;
            }
        } // GetUsersByRole

        // NEEDS AN INNER EXCEPTION THROW, OTHERWISE THE DETAIL IS LACKING, HAPPENS WHEN USER ALREADY ROLE
        // This function gives a selected user a provided role.
        public async static Task AddRoleToUser(string EUID, string roleName)
        {
            await using (var context = new ABETDBContext())
            {
                // This finds a role with the given role name
                var role = context.Roles.FirstOrDefault(r => r.Name == roleName);

                // Find the User, if it exists
                var user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                // The user is added to the role, and the changes are saved
                role.Users.Add(user);
                context.SaveChanges();

                return;
            }
        } // AddRoleToUser

        // This function removes a role from a user, selected via EUID
        public async static Task RemoveRoleFromUser(string EUID, string roleName)
        {
            await using (var context = new ABETDBContext())
            {
                //Finds the first user with a matching EUID
                var user = context.Users.FirstOrDefault(p => p.EUID == EUID);

                //Finds the first role with a matching role name
                var role = context.Roles.FirstOrDefault(r => r.Name == roleName);

                //This uses explicit loading to tell the database we want role.Users loaded
                context.Entry(role).Collection(role => role.Users).Load();

                //This removes the user from the join table
                role.Users.Remove(user);
                context.SaveChanges();

                return;
            }
        } // RemoveRoleFromUser
    } // Role
}