using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Data;
using AbetApi.EFModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MajorOutcomeController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("AddMajorOutcome")]
        public async Task<IActionResult> AddMajorOutcome(string term, int year, string majorName, MajorOutcome majorOutcome)
        {
            try
            {
                await MajorOutcome.AddMajorOutcome(term, year, majorName, majorOutcome);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // AddMajorOutcome

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetMajorOutcome")]
        public async Task<IActionResult> GetMajorOutcome(string term, int year, string majorName, string outcomeName)
        {
            try
            {
                return Ok(await MajorOutcome.GetMajorOutcome(term: term, year: year, majorName, outcomeName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetMajorOutcome

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPatch("EditMajorOutcome")]
        public async Task<IActionResult> EditMajorOutcome(string term, int year, string majorName, string outcomeName, MajorOutcome NewValue)
        {
            try
            {
                await MajorOutcome.EditMajorOutcome(term, year, majorName, outcomeName, NewValue);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // EditMajorOutcome

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteMajorOutcome")]
        public async Task<IActionResult> DeleteMajorOutcome(string term, int year, string majorName, string outcomeName)
        {
            try
            {
                await MajorOutcome.DeleteMajorOutcome(term, year, majorName, outcomeName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // DeleteMajorOutcome
    } // MajorOutcomeController
}
