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
        public List<Section> GetSectionsByUserId([FromBody] BodyParams body)
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
        public List<Form> GetFormsByCourse([FromBody] BodyParams body)
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
        public ActionResult GetAllSections([FromBody] BodyParams body)
        {
            if (body.Year < 1) return BadRequest();
            if (String.IsNullOrEmpty(body.Semester)) return BadRequest();
            return Ok(mockAbetRepo.GetSectionsByYearAndSemester(body.Year, body.Semester));
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("forms/by-semester-year")]
        public List<Form> GetAllForms([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetFormsByYearAndSemester(body.Year, body.Semester);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("sections/add-section")]
        public ActionResult AddSection([FromBody] BodyParams body)
        {
            if (mockAbetRepo.AddSection(body.Section))
                return Ok();
            else
                return BadRequest();
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("faculty/get-list")]
        //Old code
        /*
        public FacultyList GetFacultyList()
        {
            return mockAbetRepo.GetFacultyList();
            //return abetRepo.GetFacultyList();
        }*/
        
        //Refactored version
        public ActionResult GetFacultyList()
        {
            return Ok(mockAbetRepo.GetFacultyList());
        }
        

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("faculty/add-member")]
        public ActionResult AddFacultyMember([FromBody] BodyParams body)
        {
            //if (abetRepo.AddFacultyMember(body.Info, body.FacultyType))
            if (mockAbetRepo.AddFacultyMember(body.Info, body.FacultyType))
                return Ok();
            else
                return BadRequest();
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("outcomes/get-outcomes-by-program")]
        public Program_Outcomes GetCourseObjectives([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetCourseObjectives(body.Program);
            //return abetRepo.GetCourseObjectives(body.Program);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("courses/get-by-department")]
        public List<Course> GetCoursesByDepartment([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetCoursesByDepartment(body.Department);
            //return abetRepo.GetCoursesByDepartment(body.Department);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("courses/add-course")]
        public ActionResult AddCourse([FromBody] BodyParams body)
        {
            if (mockAbetRepo.AddCourse(body.Course))
            //if (abetRepo.AddCourse(body.Course))
                return Ok();
            else
                return BadRequest();
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("courses/remove-course")]
        public ActionResult RemoveCourse([FromBody] BodyParams body)
        {
            if (mockAbetRepo.AddCourse(body.Course))
            //if (abetRepo.RemoveCourse(body.Course))
                return Ok();
            else
                return BadRequest();
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("course-outcomes/post-outcomes")]
        public ActionResult PostCourseOutcomes([FromBody] BodyParams body)
        {
            if (mockAbetRepo.PostCourseOutcomes(body.CourseOutcomesList))
                return Ok();
            else
                return BadRequest();
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("course-outcomes/by-course")]
        public List<Course_Outcome> GetCourseOutcomesByCourse([FromBody] BodyParams body)
        {
            return mockAbetRepo.GetCourseOutcomesByCourse(body.Course);
        }
    }
}
