using Microsoft.AspNetCore.Mvc;
using AbetApi.EFModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GradeController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpGet("GetGrades")]
        public async Task<IActionResult> GetGrades(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            try
            {
                return Ok(await Grade.GetGrades(term, year, department, courseNumber, sectionNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetGrades

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("SetGrades")]
        public async Task<IActionResult> SetGrades(string term, int year, string department, string courseNumber, string sectionNumber, List<Grade> grades)
        {
            try
            {
                await Grade.SetGrades(term, year, department, courseNumber, sectionNumber, grades);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetGrades
    }
}
