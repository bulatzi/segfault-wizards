using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AbetApi.EFModels;
using AbetApi.Authentication;

namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MajorController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddMajor")]
        public async Task<IActionResult> AddMajor(string term, int year, string name)
        {
            try
            {
                await Major.AddMajor(term, year, name);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // AddMajor

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetMajors")]
        public async Task<IActionResult> GetMajors(string term, int year)
        {
            try
            {
                return Ok(await Major.GetMajors(term, year));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetMajors

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPatch("EditMajor")]
        public async Task<IActionResult> EditMajor(string term, int year, string name, string NewValue)
        {
            try
            {
                await Major.EditMajor(term, year, name, NewValue);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // EditMajor

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteMajor")]
        public async Task<IActionResult> DeleteMajor(string term, int year, string name)
        {
            try
            {
                await Major.DeleteMajor(term, year, name);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // DeleteMajor


        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetCoursesByMajor")]
        public async Task<IActionResult> GetCoursesByMajor(string term, int year, string major)
        {
            try
            {
                return Ok(await Major.GetCoursesByMajor(term, year, major));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetCoursesByMajor

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetMajorOutcomesByMajor")]
        public async Task<IActionResult> GetMajorOutcomesByMajor(string term, int year, string majorName)
        {
            try
            {
                return Ok(await Major.GetMajorOutcomesByMajor(term, year, majorName));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    } // MajorController
}