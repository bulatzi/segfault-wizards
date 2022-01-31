using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Data;
using AbetApi.EFModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        // This function takes an EUID and returns the user information for that EUID.
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetUser")]
        public User GetUser(string EUID)
        {
            //Returns a task with the result
            var taskResult = EFModels.User.GetUser(EUID);

            //Unwraps the result in to a User object
            var result = taskResult.Result;

            //returns the unwrapped object
            return result;
        }

        // This function creates a user with the provided information.
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddUser")]
        public void AddUser(User user)
        {
            EFModels.User.AddUser(user);
        }

        // This function deletes a user's profile from the databse.
        // This does not delete a user from the UNT system. It only removes their roles to this system.
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteUser")]
        public void DeleteUser(string EUID)
        {
            EFModels.User.DeleteUser(EUID);
        }

        // This function updates a user with the provided information
        // User is selected via the given EUID
        // information provided in NewUserInfo is used to replace the existing information
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPatch("EditUser")]
        public void EditUser(string EUID,User NewUserInfo)
        {
            EFModels.User.EditUser(EUID, NewUserInfo);
        }

        // This function creates a user with the provided information.
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddUserWithRoles")]
        public void AddUserWithRoles(AbetApi.Models.UserWithRoles userWithRoles)
        {
            EFModels.User.AddUser(userWithRoles.user);
            foreach (var role in userWithRoles.roles)
            {
                EFModels.Role.AddRoleToUser(userWithRoles.user.EUID, role);
            }
        }
    }
}
