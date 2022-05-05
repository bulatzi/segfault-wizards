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
                //Get the specified grades
                var grades = await Grade.GetGrades(term, year, department, courseNumber, sectionNumber);

                //Converts the data to a format for the front end and returns the data
                return Ok(AbetApi.Models.Grade.ConvertToModelGrade(grades));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetGrades

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("SetGrades")]
        public async Task<IActionResult> SetGrades(string term, int year, string department, string courseNumber, string sectionNumber, Dictionary<string, AbetApi.Models.Grade> gradesDictionary)
        {
            //List<Grade> grades
            try
            {
                await Grade.SetGrades(term, year, department, courseNumber, sectionNumber, AbetApi.Models.Grade.ConvertToEFModelGrade(gradesDictionary));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetGrades
    }
}
