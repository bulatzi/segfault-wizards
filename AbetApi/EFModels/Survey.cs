using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json.Serialization;
using AbetApi.Data;

namespace AbetApi.EFModels
{
    public class Survey
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SurveyId { get; set; }
        public string EUID { get; set; }
        public string term { get; set; } //e.g. Fall
        public int year { get; set; } //e.g. 2022
        public string department { get; set; } //e.g. CSCE
        public string courseNumber { get; set; } //e.g. 3600
        public string sectionNumber { get; set; } //e.g. 003 or 250
        public string answerString { get; set; }
        public string additionalComments { get; set; }
        private const int numberOfQuestions = 100;

        private string ConvertAnswersToString(List<int> answerList)
        {
            StringBuilder strBuilder = new StringBuilder("", numberOfQuestions);

            foreach(var number in answerList)
                strBuilder.Append(number);

            return strBuilder.ToString();
        }

        private List<int> ConvertAnswersToList(string answerString)
        {
            List<int> list = new List<int>();

            foreach(char c in answerString)
                list.Add(int.Parse(c.ToString()));
            return list;
        }

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

        public Survey()
        {
            //Intentionally left blank for entity framework
        }

        //This function is called when a survey is submitted
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

            //Check if the department is null or empty.
            if (survey.department == null || survey.department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
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
            survey.term = survey.term[0].ToString().ToUpper() + survey.term[1..].ToLower();
            survey.department = survey.department.ToUpper();
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