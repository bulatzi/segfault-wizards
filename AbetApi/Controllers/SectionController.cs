using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AbetApi.EFModels;
using AbetApi.Authentication;

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
    //! The SectionController Class
    /*! 
     * This class controls most of the Section endpoints
     * It inherits from ControllerBase
     */
    public class SectionController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddSection")]
        //! The AddSection function
        /*! 
         * This function adds a section/section info to a specific course/courseID. Specific courses (and all their sections)
         * all share the same CourseID, though the InstructorEUID can be different. A SectionID number is auto-generated
         * in sequential order in relation to the other sections. Utilizes EFModels.Section in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param section Section object that contains: InstructorEUID string, sectionCompleted boolean, sectionNumber int, numberOfStudents int
         */
        public async Task<IActionResult> AddSection(string term, int year, string department, string courseNumber, Section section)
        {
            try
            {
                await Section.AddSection(term, year, department, courseNumber, section);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        } // AddSection

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetSection")]
        //! The GetSection function
        /*! 
         * This function gets a JSON object that contains sectonID int, instructorEUD string, isSectioncompleted boolean,
         * sectionNumber string,numberOfStudents int. Utilizes EFModels.Section in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param sectionNumber Course section, such as 001 or 002
         */
        public async Task<IActionResult> GetSection(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            try
            {
                return Ok(await Section.GetSection(term, year, department, courseNumber, sectionNumber));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetSection

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPatch("EditSection")]
        //! The EditSection function
        /*! 
         * This function edits the prexisting sections. Utilizes EFModels.Section in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param sectionNumber Course section, such as 001 or 002
         * \param NewValue NewValue object that contains sectionID int, instructorEUID string, isSectionCompleted boolean, sectionNumber string, numberOfStudents int
         */
        public async Task<IActionResult> EditSection(string term, int year, string department, string courseNumber, string sectionNumber, Section NewValue)
        {
            try
            {
                await Section.EditSection(term, year, department, courseNumber, sectionNumber, NewValue);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // EditSection

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteSection")]
        //! The DeleteSection function
        /*! 
         * This function deletes the prexisting sections. Utilizes EFModels.Section in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param sectionNumber Course section, such as 001 or 002
         */
        public async Task<IActionResult> DeleteSection(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            try
            {
                await Section.DeleteSection(term, year, department, courseNumber, sectionNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // DeleteSection

        [Authorize(Roles = RoleTypes.Instructor)]
        [HttpGet("GetSectionsByInstructor")]
        //! The GetSectionsByInstructor function
        /*! 
         * This function gets sections taught under an Instructor. Utilizes EFModels.Section in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param instructorEUID EUID of the instructor
         */
        public async Task<IActionResult> GetSectionsByInstructor(string term, int year, string instructorEUID)
        {
            try
            {
                return Ok(await Section.GetSectionsByInstructor(term, year, instructorEUID));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    } // SectionController
}