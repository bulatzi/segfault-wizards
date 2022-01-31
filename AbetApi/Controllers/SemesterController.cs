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
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetSemesters")]
        public List<Semester> GetSemesters()
        {
            return Semester.GetSemesters();
        }

        //This function adds a semester with the provided information to the database.
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddSemester")]
        public void AddSemester(Semester semester)
        {
            EFModels.Semester.AddSemester(semester);
        }
    }
}