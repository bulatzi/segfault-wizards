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

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteCourseOutcome")]
        public async Task<IActionResult> DeleteCourseOutcome(string term, int year, string classDepartment, string courseNumber, string name)
        {
            try
            {
                await CourseOutcome.DeleteCourseOutcome(term, year, classDepartment, courseNumber, name);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } //DeleteCourseOutcome

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetCourseOutcomes")]
        public async Task<IActionResult> GetCourseOutcomes(string term, int year, string classDepartment, string courseNumber)
        {
            try
            {
                return Ok(await CourseOutcome.GetCourseOutcomes(term, year, classDepartment, courseNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("AddMajorOutcome")]
        public async Task<IActionResult> LinkToMajorOutcome(string term, int year, string classDepartment, string courseNumber, string courseOutcomeName, string majorName, string majorOutcomeName)
        {
            try
            {
                await CourseOutcome.LinkToMajorOutcome(term, year, classDepartment, courseNumber, courseOutcomeName, majorName, majorOutcomeName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // AddMajorOutcome

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteMajorOutcome")]
        public async Task<IActionResult> RemoveLinkToMajorOutcome(string term, int year, string classDepartment, string courseNumber, string courseOutcomeName, string majorName, string majorOutcomeName)
        {
            try
            {
                await CourseOutcome.RemoveLinkToMajorOutcome(term, year, classDepartment, courseNumber, courseOutcomeName, majorName, majorOutcomeName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // RemoveMajorOutcome
    } // CourseOutcomeController
}
