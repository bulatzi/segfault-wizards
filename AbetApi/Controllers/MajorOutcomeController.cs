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
        public void AddMajorOutcome(string term, int year, string majorName, MajorOutcome majorOutcome)
        {
            EFModels.MajorOutcome.AddMajorOutcome(term, year, majorName, majorOutcome);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetMajorOutcome")]
        public MajorOutcome GetMajorOutcome(string term, int year, string majorName, string outcomeName)
        {
            var taskResult = EFModels.MajorOutcome.GetMajorOutcome(term: term, year: year, majorName, outcomeName);

            var result = taskResult.Result;
            return result;
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPatch("EditMajorOutcome")]
        public void EditMajorOutcome(string term, int year, string majorName, string outcomeName, MajorOutcome NewValue)
        {
            EFModels.MajorOutcome.EditMajorOutcome(term, year, majorName,outcomeName,NewValue);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteMajorOutcome")]
        public void DeleteMajorOutcome(string term, int year, string majorName, string outcomeName)
        {
            EFModels.MajorOutcome.DeleteMajorOutcome(term, year, majorName, outcomeName);
        }
        
    }
}
