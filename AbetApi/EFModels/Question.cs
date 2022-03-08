using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Models;
using AbetApi.Data;

namespace AbetApi.EFModels
{
    public class Question
    {
        public int QuestionId { get; set; }
        public string term { get; set; }
        public string year { get; set; }
        public string questionSetName { get; set; }
        public string question { get; set; }

        public Question()
        {
            //This was intentionally left blank for entity framework
        }

        public Question(string term, string year, string questionSetName, string question)
        {
            this.QuestionId = 0;
            this.term = term;
            this.year = year;
            this.questionSetName = questionSetName;
            this.question = question;
        }

        //This function finds all questions that belong to a set of questions for a specified year/term
        public async static Task<QuestionSet> GetQuestions(string term, string year, string questionSetName)
        {
            //Creats a list to store relevant selected questions
            List<string> questions = new List<string>();

            await using (var context = new ABETDBContext())
            {
                //Scan through all the entries of the questions table
                foreach (var question in context.Questions)
                {
                    //If the question goes by the same question set name, and it's in the right year/term, add the question to the list
                    if (question.questionSetName == questionSetName && question.term == term && question.year == year)

                        questions.Add(question.question);
                }
            }

            //Creates an object of QuestionSet, which is used to return a set of questions to the front end
            QuestionSet questionSet = new QuestionSet(questionSetName, questions);

            return questionSet;
        }

        //Removes all of the previous versions of questions, and then saves the new version of questions with the same identifiers
        //FIXME - Make this look to see if any surveys have been posted on these questions. If so, early out of this function and don't change anything.
        public async static Task SaveQuestions(string term, string year, QuestionSet questionSet)
        {
            //Delete existing questions with these identifiers
            await DeleteQuestions(term, year, questionSet.questionSetName);

            //save new ones
            //convert a question set in to a list of questions
            //Add each question

            //FIXME - rework GetQuestions to use a fully parameterized constructor instead of adding to the list directly

            await using (var context = new ABETDBContext())
            {
                foreach(var questionString in questionSet.questions)
                {
                    Question tempQuestion = new Question(term, year, questionSet.questionSetName, questionString);
                    context.Add(tempQuestion);
                }
                context.SaveChanges();
            }
        }

        private static async Task DeleteQuestions(string term, string year, string questionSetName)
        {
            await using (var context = new ABETDBContext())
            {
                //Scan through all the entries of the questions table
                foreach (var question in context.Questions)
                {
                    //If the question goes by the same question set name, and it's in the right year/term, remove the question from the list
                    if (question.questionSetName == questionSetName && question.term == term && question.year == year)
                    {
                        context.Remove(question);
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
