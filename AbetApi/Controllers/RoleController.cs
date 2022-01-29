﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbetApi.EFModels;

namespace AbetApi.Controllers
{
    // This controller is used to manage roles
    // Role names are case sensitive
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        // This function returns all users with the provided role name
        [HttpGet("GetUsersByRole")]
        public async Task<List<User>> GetUsersByRole(string roleName)
        {
            var temp = await Role.GetUsersByRole(roleName);
            return temp;
        }

        // This function creates a role with a given role name
        // Role name can include any characters. Any function calls for a role will be case sensitive.
        [HttpPost("CreateRole")]
        public void CreateRole(string roleName)
        {
            Role.CreateRole(roleName);
        }

        // This function deletes a role by the given name
        // Anybody that calls this endpoint should include a verification before actually calling this endpoint. Deletions are final.
        [HttpDelete("DeleteRole")]
        public void DeleteRole(string roleName)
        {
            Role.DeleteRole(roleName);
        }

        // This function adds the provided role to the given user (via EUID)
        [HttpPost("AddRoleToUser")]
        public void AddRoleToUser(string EUID, string roleName)
        {
            Role.AddRoleToUser(EUID, roleName);
        }

        // This function removes the selected role from the selected user (via EUID)
        [HttpDelete("RemoveRoleFromUser")]
        public void RemoveRoleFromUser(string EUID, string roleName)
        {
            Role.RemoveRoleFromUser(EUID, roleName);
        }
    }
}