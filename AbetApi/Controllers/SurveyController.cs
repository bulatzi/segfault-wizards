using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.EFModels;
using AbetApi.Models;

namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SurveyController : ControllerBase
    {
        // This function returns a specific set of specified questions
        [HttpGet("GetQuestionSet")]
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
        }

        // This function will return the standard set of questions
        [HttpGet("GetQuestions")]
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
        }

        [HttpPost("SaveQuestions")]
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
        }

        [HttpPost("PostSurvey")]
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
        }
    }
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
