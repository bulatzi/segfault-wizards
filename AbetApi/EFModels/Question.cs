using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Models;
using AbetApi.Data;

//! The EFModels namespace
/*! 
 * This namespace falls under the AbetAPI namespace, and is for EFModels.
 * The EFModels are generally called from the Controllers namespace, to 
 * provide the controllers functionality, ultimately giving endpoints/functionality
 * for the UI elements
 */
namespace AbetApi.EFModels
{
    //! The Question Class
    /*! 
     * This class gets called by the SurveyController class
     * and provides functions to get and return data
     */
    public class Question
    {
        //! The QuestionID setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public int QuestionId { get; set; }
        //! The term setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string term { get; set; }
        //! The year setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public int year { get; set; }
        //! The questionSetName setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string questionSetName { get; set; }
        //! The question setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string question { get; set; }

        //! Constructor
        /*! 
         * This Constructor builds a Question object
         */
        public Question()
        {
            //This was intentionally left blank for entity framework
        }

        //! Paramaterized Constructor
        /*! 
         * This is a constructor to create a Question object from the given term, year, questionSetName, question
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param questionSetName Name for the question set
         * \param questions a question in string form
         */
        public Question(string term, int year, string questionSetName, string question)
        {
            this.QuestionId = 0;
            this.term = term;
            this.year = year;
            this.questionSetName = questionSetName;
            this.question = question;
        }

        //! The GetQuestions function
        /*! 
         * This function finds all questions that belong to a set of questions for a specified year/term
         * It is an async Task<QuestionSet> to pass exceptions and QuestionSet Objects to the Controllers.SurveyController in Controllers
         * It first checks for null inputs, then appropriate year ranges, finally formatting the term to a standard and creating the list to 
         * store relevent questions from the set to a list. It then checks for the questions and adds them to a list, returning it as a 
         * QuestionSet object.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param questionSetName Name for the question set
         */
        public async static Task<QuestionSet> GetQuestions(string term, int year, string questionSetName)
        {
            //Check if the term is null or empty
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the questionSetName is null or empty.
            if(questionSetName == null || questionSetName == "")
            {
                throw new ArgumentException("The question set name cannot be empty.");
            }

            //Format term to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();

            //Creates a list to store relevant selected questions
            List<string> questions = new List<string>();

            await using (var context = new ABETDBContext())
            {
                //Scan through all the entries of the questions table
                foreach (Question question in context.Questions)
                {
                    //If the question goes by the same question set name, and it's in the right year/term, add the question to the list
                    if (question.questionSetName == questionSetName && question.term == term && question.year == year)
                        //FIXME - rework GetQuestions to use a fully parameterized constructor instead of adding to the list directly
                        questions.Add(question.question);
                }
            }

            //Creates an object of QuestionSet, which is used to return a set of questions to the front end
            return new QuestionSet(questionSetName, questions);
        } // GetQuestions

        //FIXME - Make this look to see if any surveys have been posted on these questions. If so, early out of this function and don't change anything.
        //! The GetQuestions function
        /*! 
         * This function removes all of the previous versions of questions, and then saves the new version of questions with the same identifiers
         * It is an async Task to pass exceptions to the Controllers.SurveyController in Controllers
         * It first checks for null inputs, then appropriate year ranges, finally formatting the term to a standard.
         * It then sorts through the questions, deleting the previous and adding the new version questions.
         * Returns a 200 OK if successful, otherwise a 400 bad request with the argument exception message.
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param questionSetName Name for the question set
         */
        public async static Task SaveQuestions(string term, int year, QuestionSet questionSet)
        {
            //Check if the term is null or empty
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the questionSetName is null or empty.
            if (questionSet.questionSetName == null || questionSet.questionSetName == "")
            {
                throw new ArgumentException("The question set name cannot be empty.");
            }

            //Format term to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();

            await using (var context = new ABETDBContext())
            {

                //Delete existing questions with these identifiers
                //Scan through all the entries of the questions table
                foreach (Question question in context.Questions)
                {
                    //If the question goes by the same question set name, and it's in the right year/term, remove the question from the list
                    if (question.questionSetName == questionSet.questionSetName && question.term == term && question.year == year)
                    {
                        context.Remove(question);
                    }
                }

                foreach (string questionString in questionSet.questions)
                {
                    Question tempQuestion = new Question(term, year, questionSet.questionSetName, questionString);
                    context.Add(tempQuestion);
                }
                context.SaveChanges();
            }
        } // SaveQuestions
    }
}
