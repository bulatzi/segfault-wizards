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
using System.IO;
using System.Net.Http.Headers;


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
        private readonly IUploadManager uploadManager;
        private const int BAD_REQUEST = 400;            // HTTPS response for bad_request
        private const int NOT_FOUND = 404;              // HTTPS response for not_found

        public AbetController(IMockAbetRepo mockAbetRepo, ILdap ldap, ITokenGenerator tokenGenerator, IAbetRepo abetRepo, IUploadManager uploadManager)
        {
            this.mockAbetRepo = mockAbetRepo;
            this.ldap = ldap;
            this.tokenGenerator = tokenGenerator;
            this.abetRepo = abetRepo;
            this.uploadManager = uploadManager;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] BodyParams body)
        {
            if (string.IsNullOrEmpty(body.UserId) || string.IsNullOrEmpty(body.Password))
                return BadRequest();

            //superuser logins, remove later
            if (body.UserId == "admin" && body.Password == "admin")
                return Ok(new { token = tokenGenerator.GenerateToken(body.UserId, "Admin"), role = "Admin" });
            else if (body.UserId == "instructor" && body.Password == "instructor")
                return Ok(new { token = tokenGenerator.GenerateToken(body.UserId, "Instructor"), role = "Instructor" });
            else if (body.UserId == "coordinator" && body.Password == "coordinator")
                return Ok(new { token = tokenGenerator.GenerateToken(body.UserId, "Coordinator"), role = "Coordinator" });

            ldap.ValidateCredentials(body.UserId, body.Password);

            if (ldap.LoginSuccessful && !ldap.InternalErrorOccurred)
            {
                //find the role of the user and generate a JWT token and send the info to the frontend
                string role = mockAbetRepo.GetRole(body.UserId);
                //string role = abetRepo.GetRole(body.UserId);
                string token = tokenGenerator.GenerateToken(body.UserId, role);

                return Ok(new { token, role }); //user is logged in
            }
            else if (!ldap.LoginSuccessful && !ldap.InternalErrorOccurred)
                return Unauthorized(new { message = ldap.ErrorMessage }); //incorrect login credentials
            else
                return StatusCode(500, new { message = ldap.ErrorMessage }); //internal server error
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
            //return mockAbetRepo.GetFormBySection(body.Section);
            return abetRepo.GetFormBySection(body.Section);
        }

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("forms/new-blank")]
        public Form GetBlankForm([FromBody] BodyParams body)
        {
            //return abetRepo.GetBlankForm(body.Section);
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
            /*
            SqlReturn sqlReturn = new SqlReturn();
            sqlReturn = abetRepo.PostForm(body.Form);
            if (sqlReturn.code == -1)
                return BadRequest(new { sqlReturn.message });
            else
                return Ok();
            */
        }

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("sections/post-section")]
        public ActionResult postSection([FromBody] BodyParams body)
        {
            /*
                Maybe add some error checking to make sure required data is being passed in.
             */
            /* Add the section to the DB */
            if (abetRepo.PostSection(body.Section))
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
            //return abetRepo.GetFormsByCourse(body.Course);
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
        public List<Section> GetAllSections([FromBody] BodyParams body)
        {
            List<Section> sections = new List<Section>();   // Return variable
            
            if (body.Year < 1890) { Response.StatusCode = BAD_REQUEST; return null; }                                                                               // Parameter Check: Valid years
            if ( (body.Semester != "fall") && (body.Semester != "spring") && (body.Semester != "summer") ) { Response.StatusCode = BAD_REQUEST; return null; }      // Parameter Check: Valid semesters

            sections = mockAbetRepo.GetSectionsByYearAndSemester(body.Year, body.Semester);
            if (sections.Count() < 1)       // No results
            {
                Response.StatusCode = NOT_FOUND;
                return null;
            }
            else                            // Results found
            {
                return sections;
            }

            /*
            sections = abetRepo.GetSectionsByYearAndSemester(body.Year, body.Semester);
            if (sections.Count() < 1)      // No results
            {
                Response.StatusCode = NOT_FOUND;
                return null;
            }
            else                           // Results found
            {
                return sections;
            }
            */


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
        public FacultyList GetFacultyList()                     // Original implementation
        {
           return mockAbetRepo.GetFacultyList();
            //return abetRepo.GetFacultyList();
        }

        //Refactored version
        //public ActionResult GetFacultyList()
        //{
        //    return Ok(mockAbetRepo.GetFacultyList());
        //    //return Ok(abetRepo.GetFacultyList());

        //}


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
        [HttpPost("courses/get-by-year-semester")]
        public List<Course> GetCoursesByYear([FromBody] BodyParams body)
        {
            // return 0 for course_id but we dont need ID anymore
            //return abetRepo.GetCoursesByYear(body.Year, body.Semester);
            return mockAbetRepo.GetCoursesByYear(body.Year, body.Semester);
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

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("upload-access-db")]
        [DisableRequestSizeLimit]
        public ActionResult UploadAccessDB()
        {
            uploadManager.ReceiveFile(Request);

            if (uploadManager.FilePath == null)
                return BadRequest(new { message = uploadManager.ErrorMessage });

            System.Diagnostics.Debug.WriteLine(uploadManager.FilePath);

            //Do SQL operations on the Access file

            //delete file?

            return Ok();
        }

    }
}
