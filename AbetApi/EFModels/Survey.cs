using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json.Serialization;
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
    //! The Survey Class
    /*! 
     * This class gets called by the SurveyController class
     * and provides functions to get and return data
     */
    public class Survey
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //! The SurveyID setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public int SurveyId { get; set; }
        //! The EUID setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string EUID { get; set; }
        //! The term setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string term { get; set; } //e.g. Fall
        //! The year setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public int year { get; set; } //e.g. 2022
        //! The department setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string department { get; set; } //e.g. CSCE
        //! The courseNumber setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string courseNumber { get; set; } //e.g. 3600
        //! The sectionNumber setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string sectionNumber { get; set; } //e.g. 003 or 250
        //! The answerString setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string answerString { get; set; }
        //! The additionalComments setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string additionalComments { get; set; }
        private const int numberOfQuestions = 100;

        //! The ConvertAnswersToString Function
        /*!
         * This function is a local string function to convert an answerlist into a string
         * It first creates a new Stringbuilder, then iterates over the answerlist, appending them.
         * Finally, it converts it to a string before returning it.
         * \param answerList A list of ints that is the answer list for a survey
         */
        private string ConvertAnswersToString(List<int> answerList)
        {
            StringBuilder strBuilder = new StringBuilder("", numberOfQuestions);

            foreach(var number in answerList)
                strBuilder.Append(number);

            return strBuilder.ToString();
        }

        //! The ConvertAnswersToList Function
        /*!
         * This function is a local List<int> function to convert string answers to int answers
         * It first creats a new list of ints, then iterates through characters in the string of answers
         * It then parses them as ints and adds them to the list.
         * \param answerString A string of answers
         */
        private List<int> ConvertAnswersToList(string answerString)
        {
            List<int> list = new List<int>();

            foreach(char c in answerString)
                list.Add(int.Parse(c.ToString()));
            return list;
        }

        //! Paramaterized Constructor
        /*! 
         * This is a constructor to create a Survey object from the given parameters
         * \param EUID A string of the desired users enterprise user identification (EUID).
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department The name of the department for the survey
         * \param courseNumber The course number of the course for the survey
         * \param sectionNumber The section number of the section of the course for the survey
         * \param answerList a list of ints with the answers given
         * \param additionalComments Any additional comments associated with the survey
         */
        public Survey(string EUID, string term, int year, string department, string courseNumber, string sectionNumber, List<int> answerList, string additionalComments)
        {
            this.EUID = EUID;
            this.term = term;
            this.year = year;
            this.department = department;
            this.courseNumber = courseNumber;
            this.sectionNumber = sectionNumber;
            this.answerString = this.ConvertAnswersToString(answerList);
            this.additionalComments = additionalComments;
        }

        //! Constructor
        /*! 
         * This Constructor builds a Survey object
         */
        public Survey()
        {
            //Intentionally left blank for entity framework
        }

        //This function is called when a survey is submitted
        //! The PostSurvey Function
        /*!
         * This function creates and then adds it to the database
         * It is an async Task to pass exceptions to the Controllers.SurveyController in Controllers
         * It first checks for null inputs in all survey subfields. It then formats term and EUID to a standard.
         * Finally, it checks if the survey already exists, and if not, adds and saves it.
         * \param survey A survey object, utilized for the actual ABET surveys by/for professors
         */
        public async static Task PostSurvey(Survey survey)
        {
            // Sets the survey id to be 0, so entity framework will give it a primary key
            survey.SurveyId = 0;

            //Check if the term is null or empty
            if (survey.term == null || survey.term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (survey.year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the course number is null or empty.
            if (survey.courseNumber == null || survey.courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the section number is null or empty.
            if (survey.sectionNumber == null || survey.sectionNumber == "")
            {
                throw new ArgumentException("The section number cannot be empty.");
            }

            //Check if the EUID is null or empty.
            if (survey.EUID == null || survey.EUID == "")
            {
                throw new ArgumentException("The EUID cannot be empty.");
            }

            //Check if the answer string is null or empty.
            if(survey.answerString == null || survey.answerString == "")
            {
                throw new ArgumentException("The answer list cannot be empty.");
            }

            //Format term and EUID to follow a standard.
            survey.term = survey.term[0].ToString().ToUpper() + survey.term.Substring(1);
            survey.EUID = survey.EUID.ToLower();

            await using(var context = new ABETDBContext())
            {
                //Try to find the survey to be submitted.
                Survey result = context.Surveys.FirstOrDefault(s => s.EUID == survey.EUID && s.term == survey.term && s.year == survey.year && s.courseNumber == survey.courseNumber && s.department == survey.department);
                
                //If the survey to be submitted is already found, then throw an exception.
                if (result != null)
                {
                    throw new ArgumentException("This EUID has already submit a survey.");
                }

                context.Surveys.Add(survey);
                context.SaveChanges();
            }
        }
    }
}