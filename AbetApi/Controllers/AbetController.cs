using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Data;
using System;
using Microsoft.AspNetCore.Http;
using static AbetApi.Models.AbetModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;


//This file handles the communication between the frontend and the API.
namespace AbetApi.Controller
{
    [Route("api")]
    [ApiController]
    public class AbetController : ControllerBase
    {
        private readonly IMockAbetRepo mockAbetRepo;
        private readonly ILdap ldap;
        private readonly IAbetRepo abetRepo;
        private readonly ITokenGenerator tokenGenerator;

        public AbetController(IMockAbetRepo mockAbetRepo, ILdap ldap, ITokenGenerator tokenGenerator, IAbetRepo abetRepo)
        {
            this.mockAbetRepo = mockAbetRepo;
            this.ldap = ldap;
            this.tokenGenerator = tokenGenerator;
            this.abetRepo = abetRepo;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] BodyParams body)
        {
            if (string.IsNullOrEmpty(body.UserId) || string.IsNullOrEmpty(body.Password))
                return BadRequest();

            //remove this part later, it is the login for the superuser to bypass ldap
            if (body.UserId == "admin" && body.Password == "admin")
                return Ok(new { token = tokenGenerator.GenerateToken(body.UserId, "Admin"), role = "Admin" });

            bool loginSuccessful = await Task.FromResult(ldap.ValidateCredentials(body.UserId, body.Password, out bool internalErrorOccurred));
            
            if (loginSuccessful && !internalErrorOccurred)
            {
                //find the role of the user and generate a JWT token and send the info to the frontend
                string role = mockAbetRepo.GetRole(body.UserId);
                //string role = abetRepo.GetRole(body.UserId);
                string token = tokenGenerator.GenerateToken(body.UserId, role);             

                return Ok(new { token, role }); //user is logged in
            }
            else if (!loginSuccessful && !internalErrorOccurred)
                return Unauthorized(); //incorrect login credentials
            else
                return StatusCode(500); //internal server error
        }

        //INSTRUCTOR LEVEL FUNCTIONS
        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("sections/by-userid-semester-year")]
        public IEnumerable<Section> GetSectionsByUserId([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetSectionsByUserId(body.UserId, body.Year, body.Semester);
            //return abetRepo.GetSectionsByUserId(body.UserId, body.Year, body.Semester);
        }

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("forms/by-section")]
        public Form GetFormBySection([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetFormBySection(body.Section);
        }

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("forms/new-blank")]
        public Form GetBlankForm([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetBlankForm();
        }

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("forms/post-form")]
        public ActionResult PostForm([FromBody] BodyParams body)
        {
            if (mockAbetRepo.PostForm(body.Form))
                return Ok();
            else
                return BadRequest();
        }

        //COORDINATOR LEVEL FUNCTIONS
        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("forms/by-course")]
        public IEnumerable<Form> GetFormsByCourse([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetFormsByCourse(body.Course);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("courses/post-comment")]
        public ActionResult PostComment([FromBody] BodyParams body)
        {
            if (mockAbetRepo.PostComment(body.Course))
                return Ok();
            else
                return BadRequest();
        }

        //ADMIN LEVEL FUNCTIONS
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("sections/by-semester-year")]
        public IEnumerable<Section> GetAllSections([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetSectionsByYearAndSemester(body.Year, body.Semester);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("forms/by-semester-year")]
        public IEnumerable<Form> GetAllForms([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetFormsByYearAndSemester(body.Year, body.Semester);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("sections/post-section")]
        public ActionResult PostSection([FromBody] BodyParams body)
        {
            if (mockAbetRepo.PostSection(body.Section))
                return Ok();
            else
                return BadRequest();
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("faculty/get-list")]
        public FacultyList GetFacultyList()
        {
            return mockAbetRepo.GetFacultyList();
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("faculty/add-member")]
        public ActionResult AddFacultyMember([FromBody] BodyParams body)
        {
            if (mockAbetRepo.AddFacultyMember(body.Info, body.Role))
                return Ok();
            else
                return BadRequest();
        }
    }
}
