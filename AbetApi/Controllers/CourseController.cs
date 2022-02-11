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
    public class CourseController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPost("AddCourse")]
        public void AddCourse(string term, int year, Course course)
        {
            EFModels.Course.AddCourse(term, year, course);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetCourse")]       
        public Course GetCourse(string term, int year, string department, string courseNumber)
        {
            var taskResult = EFModels.Course.GetCourse(term, year, department, courseNumber);

            var result = taskResult.Result;

            return result;

        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpPatch("EditCourse")]
        public void EditCourse(string term, int year, string department, string courseNumber, Course NewValue)
        {
            EFModels.Course.EditCourse(term, year, department,courseNumber, NewValue);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpDelete("DeleteCourse")]
        public void DeleteCourse(string term, int year, string department, string courseNumber)
        {
            EFModels.Course.DeleteCourse(term, year, department, courseNumber);
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetSections")]
        public List<Section> GetSections(string term, int year, string department, string courseNumber)
        {
            var taskResult = EFModels.Course.GetSections(term, year, department, courseNumber);

            var result = taskResult.Result;

            return result;
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetMajorsThatRequireCourse")]
        public List<string> getMajorsThatRequireCourse(string term, int year, string department, string courseNumber)
        {
            var taskResult = EFModels.Course.getMajorsThatRequireCourse(term, year, department, courseNumber);

            var result = taskResult.Result;

            return result;
        }
        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetCoursesByDepartment")]
        public List<Course> GetCoursesByDepartment(string term, int year, string department)
        {
            var taskResult = EFModels.Course.GetCoursesByDepartment(term, year, department);

            var result = taskResult.Result;

            return result;
        }

        [Authorize(Roles = RoleTypes.Coordinator)]
        [HttpGet("GetCourseNamesByDepartment")]
        public List<string> GetCourseNamesByDepartment(string term, int year, string department)
        {
            var taskResult = EFModels.Course.GetCourseNamesByDepartment(term, year, department);

            var result = taskResult.Result;

            return result;
        }
    }
}
