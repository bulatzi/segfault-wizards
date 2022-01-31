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
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetSemesters")]
        public List<Semester> GetSemesters()
        {
            return Semester.GetSemesters();
        }
}