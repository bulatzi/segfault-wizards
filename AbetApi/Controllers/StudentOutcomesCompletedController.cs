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
    public class StudentOutcomesCompletedController : ControllerBase
    {
        //[Authorize(Roles = RoleTypes.Instructor)]
        [HttpGet("GetStudentOutcomesCompleted")]
        public async Task<IActionResult> GetStudentOutcomesCompleted(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            try
            {
                //Get the specified grades
                //var grades = await Grade.GetGrades(term, year, department, courseNumber, sectionNumber);

                //Converts the data to a format for the front end and returns the data
                //return Ok(AbetApi.Models.Grade.ConvertToModelGrade(grades));
                return Ok(AbetApi.Models.StudentOutcomesCompleted.ConvertToModelStudentOutcomesCompleted());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetStudentOutcomesCompleted

        //[Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("SetStudentOutcomesCompleted")]
        public async Task<IActionResult> SetStudentOutcomesCompleted(string term, int year, string department, string courseNumber, string sectionNumber, Dictionary<string, string> studentOutcomesCompletedDictionary)
        {
            try
            {
                //await Grade.SetGrades(term, year, department, courseNumber, sectionNumber, AbetApi.Models.Grade.ConvertToEFModelGrade(gradesDictionary));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // SetStudentOutcomesCompleted
    }
}
