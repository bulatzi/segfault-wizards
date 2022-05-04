using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AbetApi.Authentication;


namespace AbetApi.Controllers
{
    public class Authentication : ControllerBase
    {
        private readonly ILdap ldap;
        private readonly ITokenGenerator tokenGenerator;
        public Authentication(ILdap ldap, ITokenGenerator tokenGenerator)
        {
            this.ldap = ldap;
            this.tokenGenerator = tokenGenerator;
        }

        // This function is used to return a token that contains all of the roles a user has after successfully logging in
        [HttpPost("Login")]
        public ActionResult Login(string EUID, string password)
        {
            if (string.IsNullOrEmpty(EUID) || string.IsNullOrEmpty(password))
                return BadRequest();

            //A list used to store all of the roles given to the user logging in
            List<string> rolesToAdd = new List<string>();

            //DELETEME - Development Bypass for login credentials //////////////////////////////////////////
            //For user, type the role in all lowercase
            //For password, just type something. It could be anything.
            switch (EUID)
            {
                case "admin":
                    rolesToAdd.Add("Admin");
                    break;
                case "instructor":
                    rolesToAdd.Add("Instructor");
                    break;
                case "coordinator":
                    rolesToAdd.Add("Coordinator");
                    break;
                case "student":
                    rolesToAdd.Add("Student");
                    break;
            }
            if (rolesToAdd.Count > 0)
                return Ok(new { token = tokenGenerator.GenerateToken(EUID, rolesToAdd) });
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            //Validates user/password combo with UNT domain controller
            ldap.ValidateCredentials(EUID, password);

            //If the login worked, get all of the roles that user has, build a token, and return the token
            if (ldap.LoginSuccessful && !ldap.InternalErrorOccurred)
            {
                try
                {
                    var roles = EFModels.User.GetRolesByUser(EUID).Result;

                    //All users are at least a student
                    if (roles == null || roles.Count == 0)
                    {
                        rolesToAdd.Add("Student");
                    }
                    else if (roles.Count > 0)
                    {
                        foreach (EFModels.Role itr in roles)
                        {
                            rolesToAdd.Add(itr.Name);
                        }
                    }

                    string token = tokenGenerator.GenerateToken(EUID, rolesToAdd);

                    return Ok(new { token }); //user is logged in
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            //If their credentials are incorrect, send an error
            else if (!ldap.LoginSuccessful && !ldap.InternalErrorOccurred)
                return BadRequest(new { message = ldap.ErrorMessage });
            //If this endpoint breaks for any other reason
            else
                return StatusCode(500, new { message = ldap.ErrorMessage }); //internal server error (500 error)
        }
    }
}
