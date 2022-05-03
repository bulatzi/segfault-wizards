using Microsoft.AspNetCore.Mvc;
using AbetApi.EFModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace AbetApi.Controllers
{
    public class ReportController : ControllerBase
    {
        //[Authorize(Roles = RoleTypes.Instructor)]
        [HttpGet("GenerateFullReport")]
        public async Task<IActionResult> GenerateFullReport(string term, int year)
        {
            try
            {
                //Converts the data to a format for the front end and returns the data
                return Ok(AbetApi.Models.FullReport.GenerateFullReport(term, year));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetGrades
    }
}
