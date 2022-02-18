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
          // GIVES A 204 FOR BLANK INPUT
          // This function takes an EUID and returns the user information for that EUID.
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpGet("GetUser")]
          public async Task<IActionResult> GetUser(string EUID)
          {
               try
               {
                    return Ok(await EFModels.User.GetUser(EUID));
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // GetUser

          // THROWS AN INNER EXCEPTION THAT MIGHT NEED TO BE RETHROWN FOR DETAILS
          // This function creates a user with the provided information.
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpPost("AddUser")]
          public async Task<IActionResult> AddUser(User user)
          {
               try
               {
                    await EFModels.User.AddUser(user);
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // AddUser

          // This function deletes a user's profile from the databse.
          // This does not delete a user from the UNT system. It only removes their roles to this system.
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpDelete("DeleteUser")]
          public async Task<IActionResult> DeleteUser(string EUID)
          {
               try
               {
                    await EFModels.User.DeleteUser(EUID);
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // DeleteUser

          // This function updates a user with the provided information
          // User is selected via the given EUID
          // information provided in NewUserInfo is used to replace the existing information
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpPatch("EditUser")]
          public async Task<IActionResult> EditUser(string EUID, User NewUserInfo)
          {
               try
               {
                    await EFModels.User.EditUser(EUID, NewUserInfo);
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // EditUser

          // THROWS AN INNER EXCEPTION THAT MIGHT NEED TO BE RETHROWN FOR DETAILS
          // This function creates a user with the provided information.
          [Authorize(Roles = RoleTypes.Admin)]
          [HttpPost("AddUserWithRoles")]
          public async Task<IActionResult> AddUserWithRoles(AbetApi.Models.UserWithRoles userWithRoles)
          {
               try
               {
                    await EFModels.User.AddUser(userWithRoles.user);
                    foreach (var role in userWithRoles.roles)
                    {
                         try
                         {
                              await EFModels.Role.AddRoleToUser(userWithRoles.user.EUID, role);
                         }
                         catch (Exception ex)
                         {
                              return BadRequest(ex.Message);
                         }
                    }
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }

          } // AddUserWithRoles
     } // UserController
}
