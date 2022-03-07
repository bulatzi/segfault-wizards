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
        //[Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("addCourseOutcome")]
        public async Task<IActionResult> CreateCourseOutcome(string term, int year, string classDepartment, string courseNumber, CourseOutcome courseOutcome)
        {
            try
            {
                await CourseOutcome.CreateCourseOutcome(term, year, classDepartment, courseNumber, courseOutcome);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // CreateCourseOutcome

        //[Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteCourseOutcome")]
        public async Task<IActionResult> DeleteCourseOutcome(string term, int year, string classDepartment, string courseNumber, string majorName)
        {
            try
            {
                await CourseOutcome.DeleteCourseOutcome(term, year, classDepartment, courseNumber, majorName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } //DeleteCourseOutcome

        //[Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("AddMajorOutcome")]
        public async Task<IActionResult> AddMajorOutcome(string term, int year, string classDepartment, string courseNumber, string majorName, string outcomeName)
        {
            try
            {
                await CourseOutcome.AddMajorOutcome(term, year, classDepartment, courseNumber, majorName, outcomeName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // AddMajorOutcome

        //[Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteMajorOutcome")]
        public async Task<IActionResult> RemoveMajorOutcome(string term, int year, string classDepartment, string courseNumber, string majorName, string outcomeName)
        {
            try
            {
                await CourseOutcome.RemoveMajorOutcome(term, year, classDepartment, courseNumber, majorName, outcomeName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // RemoveMajorOutcome
    } // CourseOutcomeController
}
