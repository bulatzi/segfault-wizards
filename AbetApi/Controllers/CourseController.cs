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
          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpPost("AddCourse")]
          public async Task<IActionResult> AddCourse(string term, int year, Course course)
          {
               try
               {
                    await Course.AddCourse(term, year, course);
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // AddCourse

          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpGet("GetCourse")]
          public async Task<IActionResult> GetCourse(string term, int year, string department, string courseNumber)
          {
               try
               {
                    return Ok(await Course.GetCourse(term, year, department, courseNumber));
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // GetCourse

          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpPatch("EditCourse")]
          public async Task<IActionResult> EditCourse(string term, int year, string department, string courseNumber, Course NewValue)
          {
               try
               {
                    await Course.EditCourse(term, year, department, courseNumber, NewValue);
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // EditCourse

          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpDelete("DeleteCourse")]
          public async Task<IActionResult> DeleteCourse(string term, int year, string department, string courseNumber)
          {
               try
               {
                    await Course.DeleteCourse(term, year, department, courseNumber);
                    return Ok();
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // DeleteCourse

          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpGet("GetSections")]
          public async Task<IActionResult> GetSections(string term, int year, string department, string courseNumber)
          {
               try
               {
                    return Ok(await Course.GetSections(term, year, department, courseNumber));
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // GetSections

          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpGet("GetMajorsThatRequireCourse")]
          public async Task<IActionResult> getMajorsThatRequireCourse(string term, int year, string department, string courseNumber)
          {
               try
               {
                    return Ok(await Course.getMajorsThatRequireCourse(term, year, department, courseNumber));
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // getMajorsThatRequireCourse

          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpGet("GetCoursesByDepartment")]
          public async Task<IActionResult> GetCoursesByDepartment(string term, int year, string department)
          {
               try
               {
                    return Ok(await Course.GetCoursesByDepartment(term, year, department));
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // GetCoursesByDepartment

          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpGet("GetCourseNamesByDepartment")]
          public async Task<IActionResult> GetCourseNamesByDepartment(string term, int year, string department)
          {
               try
               {
                    return Ok(await Course.GetCourseNamesByDepartment(term, year, department));
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          }

          //[Authorize(Roles = RoleTypes.Coordinator)]
          [HttpGet("GetDepartments")]
          public async Task<IActionResult> GetDepartments(string term, int year)
          {
               try
               {
                    return Ok(await Course.GetDepartments(term, year));
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          } // GetDepartments
     } // CourseController
}
