using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbetApi.Models
{
    public class QuestionSet
    {
        public string questionSetName { get; set; }
        public List<string> questions { get; set; }

        public QuestionSet(string questionSetName, List<string> questions)
        {
            this.questionSetName = questionSetName;
            this.questions = questions;
        }
    }
}
