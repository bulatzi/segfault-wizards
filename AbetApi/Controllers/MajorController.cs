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
            Major.AddMajor(term, year, name);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetMajors")]
        public List<Major> GetMajors(string term, int year)
        {
            return Major.GetMajors(term, year);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPatch("EditMajor")]
        public void EditMajor(string term, int year, string name, string NewValue)
        {
            Major.EditMajor(term, year, name, NewValue);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteMajor")]
        public void DeleteMajor(string term, int year, string name)
        {
            Major.DeleteMajor(term, year, name);
        }
    }
}
