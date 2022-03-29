using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.EFModels;
using AbetApi.Authentication;
using Microsoft.AspNetCore.Authorization;

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

    //! The SemesterController Class
    /*! 
     * This class controls most of the semester endpoints
     * It inherits from ControllerBase
     */
    public class SemesterController : ControllerBase
    {
        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetSemesters")]
        //! The GetSemesters function
        /*! 
         * This function gets all semesters from the Semesters table in the database. Utilizes EFModels.Semester in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message
         */
        public async Task<IActionResult> GetSemesters()
        {
            try
            {
                return Ok(await Semester.GetSemesters());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetSemesters

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddSemester")]
        //! The AddSemester function
        /*!
         * This function adds a semester with the provided information to the database. Utilizes EFModels.Semester in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message
         * \param semester A semester object
         */
        public async Task<IActionResult> AddSemester(Semester semester)
        {
            try
            {
                await Semester.AddSemester(semester);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // AddSemester

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPatch("EditSemester")]
        //! The EditSemesters function
        /*!
         * This function edits an already existing semester with the newly provided information to the database. Utilizes EFModels.Semester in EFModels
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message
         * \param term The term (Spring/Fall) of the respective semester
         * \param year The year of the respective semester
         * \param semester This is a semester object to hold various data, such as year, term, courses, majors, outcomes, etc.
         */
        public async Task<IActionResult> EditSemester(string term, int year, Semester semester)
        {
            try
            {
                await Semester.EditSemester(term, year, semester);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteSemester")]
        //! The DeleteSemester function
        /*!
         * This function deletes a semester from the database with the provided information. Utilizes EFModels.Semester in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message
         * \param semester This is a semester object to hold various data, such as year, term, courses, majors, outcomes, etc.
         */
        public async Task<IActionResult> DeleteSemester(Semester semester)
        {
            try
            {
                await Semester.DeleteSemester(semester.Term, semester.Year);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // DeleteSemester
    } // SemesterController
}