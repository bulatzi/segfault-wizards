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
    public class SemesterController : ControllerBase
    {
        //This function gets all semesters from the Semesters table in the database.
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetSemesters")]
        public async Task<IActionResult> GetSemesters()
        {
            try
            {
                return Ok(await Semester.GetSemesters());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetSemesters

        //This function adds a semester with the provided information to the database.
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddSemester")]
        public async Task<IActionResult> AddSemester(Semester semester)
        {
            try
            {
                await Semester.AddSemester(semester);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // AddSemester

        //This function deletes a semester from the database with the provided information.
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteSemester")]
        public async Task<IActionResult> DeleteSemester(Semester semester)
        {
            try
            {
                await Semester.DeleteSemester(semester.Term, semester.Year);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // DeleteSemester
    } // SemesterController
}