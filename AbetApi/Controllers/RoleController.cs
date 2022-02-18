using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AbetApi.EFModels;
using AbetApi.Authentication;


namespace AbetApi.Controllers
{
     // This controller is used to manage roles
     // Role names are case sensitive
     [ApiController]
     [Route("[controller]")]
     public class RoleController : ControllerBase
     {
          // This function returns all users with the provided role name
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpGet("GetUsersByRole")]
          public async Task<List<User>> GetUsersByRole(string roleName)
          {
               var temp = await Role.GetUsersByRole(roleName);
               return temp;
          }

          // This function returns a list of all users with the Admin/Instructor/Coordinator roles
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpGet("GetFaculty")]
          public async Task<AbetApi.Models.Faculty> GetFaculty()
          {
               List<User> admins = new List<User>();
               List<User> instructors = new List<User>();
               List<User> coordinators = new List<User>();

               AbetApi.Models.Faculty temp = new AbetApi.Models.Faculty
               {
                    Admins = await Role.GetUsersByRole("Admin"),
                    Instructors = await Role.GetUsersByRole("Instructor"),
                    Coordinators = await Role.GetUsersByRole("Coordinator")
               };

               return temp;
          }

          // This function creates a role with a given role name
          // Role name can include any characters. Any function calls for a role will be case sensitive.
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpPost("CreateRole")]
          public void CreateRole(string roleName)
          {
               Role.CreateRole(roleName);
          }

          // This function deletes a role by the given name
          // Anybody that calls this endpoint should include a verification before actually calling this endpoint. Deletions are final.
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpDelete("DeleteRole")]
          public void DeleteRole(string roleName)
          {
               Role.DeleteRole(roleName);
          }

          // This function adds the provided role to the given user (via EUID)
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpPost("AddRoleToUser")]
          public async Task<IActionResult> AddRoleToUser(string EUID, string roleName)
          {
               try
               {
                    await Role.AddRoleToUser(EUID, roleName);
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          }

          // This function removes the selected role from the selected user (via EUID)
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpDelete("RemoveRoleFromUser")]
          public void RemoveRoleFromUser(string EUID, string roleName)
          {
               Role.RemoveRoleFromUser(EUID, roleName);
          }
     }
}
