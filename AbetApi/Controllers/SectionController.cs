using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AbetApi.EFModels;
using AbetApi.Authentication;

namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SectionController : ControllerBase
    {
        ////////////////////////////////////////////////////////////////////////////////////
        // AddSection //
        // string term:          Semester term, such as Fall or Spring
        // int year:             School year, such as 2022 or 2023
        // string department:    Major department, such as CSCE or MEEN
        // string courseNumber:  Course identifier, such as 3600 for Systems Programming
        // Section section:      Section object that contains: InstructorEUID string, 
        //                       sectionCompleted boolean, sectionNumber int,
        //                       numberOfStudents int
        // description:          This function adds a section/section info to a specific
        //                       course/courseID. Specific courses (and all their sections)
        //                       all share the same CourseID, though the InstructorEUID
        //                       can be different. A SectionID number is auto-generated
        //                       in sequential order in relation to the other sections
        ////////////////////////////////////////////////////////////////////////////////////
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("AddSection")]
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

        ////////////////////////////////////////////////////////////////////////////////////
        // GetSection //
        // string term:          Semester term, such as Fall or Spring
        // int year:             School year, such as 2022 or 2023
        // string department:    Major department, such as CSCE or MEEN
        // string courseNumber:  Course identifier, such as 3600 for Systems Programming
        // string sectionNumber: Course section, such as 001 or 002
        // description:          This function gets a JSON object that contains sectonID int,
        //                       instructorEUD string, isSectioncompleted boolean, 
        //                       sectionNumber string,numberOfStudents int
        ////////////////////////////////////////////////////////////////////////////////////
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("GetSection")]
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

        ////////////////////////////////////////////////////////////////////////////////////
        // EditSection //
        // string term:          Semester term, such as Fall or Spring
        // int year:             School year, such as 2022 or 2023
        // string department:    Major department, such as CSCE or MEEN
        // string courseNumber:  Course identifier, such as 3600 for Systems Programming
        // string sectionNumber: Course section, such as 001 or 002
        // Section NewValue:     NewValue object that contains sectionID int,
        //                       instructorEUID string, isSectionCompleted boolean,
        //                       sectionNumber string, numberOfStudents int
        // description:          This function edits the prexisting sections
        ////////////////////////////////////////////////////////////////////////////////////
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPatch("EditSection")]
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

        ////////////////////////////////////////////////////////////////////////////////////
        // DeleteSection //
        // string term:          Semester term, such as Fall or Spring
        // int year:             School year, such as 2022 or 2023
        // string department:    Major department, such as CSCE or MEEN
        // string courseNumber:  Course identifier, such as 3600 for Systems Programming
        // string sectionNumber: Course section, such as 001 or 002
        // description:          This function deletes the prexisting sections
        ////////////////////////////////////////////////////////////////////////////////////
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("DeleteSection")]
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
    } // SectionController
}