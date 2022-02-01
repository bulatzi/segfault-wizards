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
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetAllMajors")]
        public List<Major> GetAllMajors(string term, int year)
        {
            return Major.GetAllMajors(term, year);
        }


    }
}
