using Microsoft.AspNetCore.Mvc;
using AbetApi.EFModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

//! The Controllers Namespace
/*! 
 * This namespace falls under the AbetAPI namespace, and is for controllers.
 * The controller generally tie directly into the EFModels namespace function
 * to provide the ABET website with endpoints/functionality for UI elements
 */
namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //! The StudentOutcomesCompletedController Class
    /*! 
     * This class controls most of the Student outcomes completed endpoints
     * It inherits from ControllerBase
     */
    public class StudentOutcomesCompletedController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpGet("GetStudentOutcomesCompleted")]
        //! The GetStudentOutcomesCompleted function
        /*! 
         * This function gets a list of StudentOutcomesCompleted objects.
         * It then sends that list to ConvertToModelStudentOutcomesCompleted to attach a number of students that have completed
         * an associated course/major outcome. Utilizes EFModels.StudentOutcomesCompleted in EFModels and Models.StudentOutcomesCompleted in Models
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param sectionNumber Course section, such as 001 or 002
         */
        public async Task<IActionResult> GetStudentOutcomesCompleted(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            try
            {
                return Ok(AbetApi.Models.StudentOutcomesCompleted.ConvertToModelStudentOutcomesCompleted(term, year, department, courseNumber, StudentOutcomesCompleted.GetStudentOutcomesCompleted(term, year, department, courseNumber, sectionNumber).Result));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetStudentOutcomesCompleted

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpPost("SetStudentOutcomesCompleted")]
        //! The SetStudentOutcomesCompleted function
        /*! 
         * This function sets the students completed for a given major/course outcome.
         * If the the given major/course outcome doesn't exits, it creates it. Utilizes EFModels.StudentOutcomesCompleted in EFModels and Models.StudentOutcomesCompleted in Models
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param sectionNumber Course section, such as 001 or 002
         * \param studentOutcomesCompletedDictionary
         */
        public async Task<IActionResult> SetStudentOutcomesCompleted(string term, int year, string department, string courseNumber, string sectionNumber, List<Dictionary<string, string>> studentOutcomesCompletedDictionary)
        {
            try
            {
                //await Grade.SetGrades(term, year, department, courseNumber, sectionNumber, AbetApi.Models.Grade.ConvertToEFModelGrade(gradesDictionary));

                //Need to turn a dictionary back in to a list of singular StudentOutcomesCompleted objects
                //There will be one for each major mentioned in that dictionary

                List<StudentOutcomesCompleted> tempList = AbetApi.Models.StudentOutcomesCompleted.ConvertToEFModelStudentOutcomesCompleted(term, year, department, courseNumber, sectionNumber, studentOutcomesCompletedDictionary);

                foreach(var item in tempList)
                {
                    StudentOutcomesCompleted.SetStudentOutcomesCompleted(item.Term, item.Year, item.ClassDepartment, item.CourseNumber, item.SectionName, item.CourseOutcomeName, item.MajorName, item.StudentsCompleted);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // SetStudentOutcomesCompleted
    }
}
