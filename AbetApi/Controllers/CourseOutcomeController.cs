using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.EFModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseOutcomeController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("addCourseOutcome")]
        public void CreateCourseOutcome(string term, int year, string classDepartment, string courseNumber,CourseOutcome courseOutcome)
        {
            CourseOutcome.CreateCourseOutcome(term, year, classDepartment, courseNumber, courseOutcome);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteCourseOutcome")]
        public void DeleteCourseOutcome(string term, int year, string classDepartment, string courseNumber, string majorName)
        {
            CourseOutcome.DeleteCourseOutcome(term, year, classDepartment, courseNumber, majorName);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("AddMajorOutcome")]
        public void AddMajorOutcome(string term, int year, string classDepartment, string courseNumber, string majorName, string outcomeName)
        {
            CourseOutcome.AddMajorOutcome(term, year, classDepartment,courseNumber, majorName, outcomeName);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteMajorOutcome")]
        public void RemoveMajorOutcome(string term, int year, string classDepartment, string courseNumber, string majorName, string outcomeName)
        {
            CourseOutcome.RemoveMajorOutcome(term,year, classDepartment, courseNumber,majorName, outcomeName);
        }
    }
}
