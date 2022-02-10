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
    public class MajorController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddMajor")]
        public void AddMajor(string term, int year, string name)
        {
            EFModels.Major.AddMajor(term, year, name);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetMajors")]
        public List<Major> GetMajors(string term, int year)
        {
            return EFModels.Major.GetMajors(term, year);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPatch("EditMajor")]
        public void EditMajor(string term, int year, string name, string NewValue)
        {
            EFModels.Major.EditMajor(term, year, name, NewValue);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteMajor")]
        public void DeleteMajor(string term, int year, string name)
        {
            EFModels.Major.DeleteMajor(term, year, name);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetCoursesByMajor")]
        public List<Course> GetCoursesByMajor(string term, int year, string name)
        {
            var taskResult = EFModels.Major.GetCoursesByMajor(term, year, name);

            var result = taskResult.Result;
            return result;
        }
    }
}
