using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Data;
using AbetApi.EFModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;

//! The Controllers Namespace
/*! 
 * This namespace falls under the AbetAPI namespace, and is for controllers.
 * The controller generally tie directly into the EFModels namespace function
 * to provide the ABET website with endpoints/functionality for UI elements
 */
namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //! The UsersController Class
    /*! 
     * This class controls most of the user endpoints
     * It inherits from ControllerBase
     */
    public class UsersController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetUser")]
        //! The GetUser function
        /*! 
         * This function takes an EUID and returns the user information for that EUID. Utilizes EFModels.User in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param EUID A string of the desired users enterprise user identification (EUID).
         */
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

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddUser")]
        //! The AddUser function
        /*! 
         * This function creates a user with the provided information. Utilizes EFModels.User in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param user A user object that contains relevent user information.
         */
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

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteUser")]
        //! The DeleteUser function
        /*! 
         * This function deletes a user's profile from the databse. Utilizes EFModels.User in EFModels.
         * This does not delete a user from the UNT system. It only removes their roles to this system.
         * Returns A 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param EUID A string of the desired users enterprise user identification (EUID).
         */
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

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPatch("EditUser")]
        //! The EditUser function
        /*! 
         * This function updates a user with the provided information. Utilizes EFModels.User in EFModels.
         * User is selected via the given EUID, and information provided in NewUserInfo is used to replace the existing information.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param EUID A string of the desired users enterprise user identification (EUID).
         * \param NewUserInfo A User object that provides the relevent info for editing an existing user.
         */
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

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddUserWithRoles")]
        //! AddUserWithRoles function
        /*! 
         * This function creates a user with the provided information (Has roles associated already). Utilizes Models.UserWithRoles in Models.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param userWithRoles a Models.UserWithRoles object
         */
        public async Task<IActionResult> AddUserWithRoles(AbetApi.Models.UserWithRoles userWithRoles)
        {
            try
            {
                await EFModels.User.AddUser(userWithRoles.user);
                foreach (string role in userWithRoles.roles)
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
