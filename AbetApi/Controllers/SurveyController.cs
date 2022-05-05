using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.EFModels;
using AbetApi.Models;

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
    //! The SurveyController Class
    /*! 
     * This class controls most of the Survey and Question endpoints
     * It inherits from ControllerBase
     */
    public class SurveyController : ControllerBase
    {
        [HttpGet("GetQuestionSet")]
        //! The GetQuestionSet function
        /*! 
         * This function returns a specific set of specified questions. Utilizes EFModels.Question in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param questionSetName Name for the question set
         */
        public async Task<IActionResult> GetQuestionSet(string term, int year, string questionSetName)
        {
            try
            {
                return Ok(await Question.GetQuestions(term, year, questionSetName));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetQuestionSet

        [HttpGet("GetQuestions")]
        //! The GetQuestions function
        /*! 
         * This function will return the standard set of questions. Utilizes EFModels.Question in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         */
        public async Task<IActionResult> GetQuestions(string term, int year)
        {
            try
            {
                List<QuestionSet> questions = new List<QuestionSet>();
                questions.Add(await Question.GetQuestions(term, year, "InstructorQuestions"));
                questions.Add(await Question.GetQuestions(term, year, "TAquestions"));
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // GetQuestions

        [HttpPost("SaveQuestions")]
        //! The SaveQuestions function
        /*! 
         * This function will save a list of questions to a new question set with a specific set name. Utilizes EFModels.Question in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param questionSetName Name for the question set
         * \param questions A list of questions in string form
         */
        public async Task<IActionResult> SaveQuestions(string term, int year, string questionSetName, List<string> questions)
        {
            try
            {
                await Question.SaveQuestions(term, year, new Models.QuestionSet(questionSetName, questions));
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // SaveQuestions

        [HttpPost("PostSurvey")]
        //! The PostSurvey function
        /*! 
         * This function will post a new survey utilizing the listed parameters. Utilizes EFModels.Survey in EFModels.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param EUID A string of the desired users enterprise user identification (EUID).
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department The name of the department for the survey
         * \param courseNumber The course number of the course for the survey
         * \param sectionNumber The section number of the section of the course for the survey
         * \param answerList a list of ints with the answers given
         * \param additionalComments Any additional comments associated with the survey
         */
        public async Task<IActionResult> PostSurvey(string EUID, string term, int year, string department, string courseNumber, string sectionNumber, List<int> answerList, string additionalComments)
        {
            try 
            {
                await Survey.PostSurvey(new Survey(EUID, term, year, department, courseNumber, sectionNumber, answerList, additionalComments));
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } // PostSurvey
    } // SurveyController
}

/*
    Things to do:
        Make editing questions check if any surveys have been posted
            If yes, don't allow editing
        Verify that EUID hasn't already submitted a survey
        Generate reports by course/section
            Convert the string back in to an int list for all relevant surveys
            Add those numbers up, return as a report
 */
