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
        [HttpPost("AddCourseOutcome")]
        public async Task<IActionResult> CreateCourseOutcome(string term, int year, string department, string courseNumber, CourseOutcome courseOutcome)
        {
            try
            {
                await CourseOutcome.CreateCourseOutcome(term, year, department, courseNumber, courseOutcome);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // CreateCourseOutcome

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPatch("EditCourseOutcome")]
        public async Task<IActionResult> EditCourseOutcome(string term, int year, string department, string courseNumber, string courseOutcomeName, CourseOutcome newCourseOutcome)
        {
            try
            {
                await CourseOutcome.EditCourseOutcome(term, year, department, courseNumber, courseOutcomeName, newCourseOutcome);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteCourseOutcome")]
        public async Task<IActionResult> DeleteCourseOutcome(string term, int year, string department, string courseNumber, string name)
        {
            try
            {
                await CourseOutcome.DeleteCourseOutcome(term, year, department, courseNumber, name);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } //DeleteCourseOutcome

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetCourseOutcomes")]
        public async Task<IActionResult> GetCourseOutcomes(string term, int year, string department, string courseNumber)
        {
            try
            {
                return Ok(await CourseOutcome.GetCourseOutcomes(term, year, department, courseNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetLinkedMajorOutcomes")]
        public async Task<IActionResult> GetLinkedMajorOutcomes(string term, int year, string department, string courseNumber, string courseOutcomeName, string majorName)
        {
            try
            {
                return Ok(await CourseOutcome.GetLinkedMajorOutcomes(term, year, department, courseNumber, courseOutcomeName, majorName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("LinkToMajorOutcome")]
        public async Task<IActionResult> LinkToMajorOutcome(string term, int year, string department, string courseNumber, string courseOutcomeName, string majorName, string majorOutcomeName)
        {
            try
            {
                await CourseOutcome.LinkToMajorOutcome(term, year, department, courseNumber, courseOutcomeName, majorName, majorOutcomeName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // AddMajorOutcome

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("RemoveLinkToMajorOutcome")]
        public async Task<IActionResult> RemoveLinkToMajorOutcome(string term, int year, string department, string courseNumber, string courseOutcomeName, string majorName, string majorOutcomeName)
        {
            try
            {
                await CourseOutcome.RemoveLinkToMajorOutcome(term, year, department, courseNumber, courseOutcomeName, majorName, majorOutcomeName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // RemoveMajorOutcome
    } // CourseOutcomeController
}
