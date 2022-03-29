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
        public string year { get; set; } //e.g. 2022
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

        public Survey(string EUID, string term, string year, string department, string courseNumber, string sectionNumber, List<int> answerList, string additionalComments)
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
            survey.SurveyId = 0;

            await using(var context = new ABETDBContext())
            {
                //FIXME - Check if that EUID has already submitted a survey, and early out if it already exists
                if (await ContainsSurvey(survey) == true)
                    return;

                context.Surveys.Add(survey);
                context.SaveChanges();
            }
        }

        private async static Task<bool> ContainsSurvey(Survey survey)
        {
            await using(var context = new ABETDBContext())
            {
                Survey result = context.Surveys.FirstOrDefault(s => s.EUID == survey.EUID && s.term == survey.term && s.year == survey.year && s.courseNumber == survey.courseNumber && s.department == survey.department);

                if (result == null)
                    return false;
                else
                    return true;
            }
        }
    }
}