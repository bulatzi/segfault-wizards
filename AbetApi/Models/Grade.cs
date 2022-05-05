using AbetApi.EFModels;
using System.Collections.Generic;

namespace AbetApi.Models
{
    //This class is used to convert EFModel grade objects in to a format for the front end
    public class Grade
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int F { get; set; }
        public int W { get; set; }
        public int I { get; set; }
        public int TotalStudents { get; set; }

        public Grade()
        { 
        
        }
        public Grade(AbetApi.EFModels.Grade grade)
        {
            this.A = grade.A;
            this.B = grade.B;
            this.C = grade.C;
            this.D = grade.D;
            this.F = grade.F;
            this.W = grade.W;
            this.I = grade.I;
            this.TotalStudents = grade.TotalStudents;
        }

        //This function takes a dictionary of Models.Grade objects and converts them to a list of EFModels.Grade objects
        public static List<AbetApi.EFModels.Grade> ConvertToEFModelGrade(Dictionary<string, AbetApi.Models.Grade> gradesDictionary)
        {
            List<AbetApi.EFModels.Grade> gradesList = new List<AbetApi.EFModels.Grade>();
            foreach (KeyValuePair<string, AbetApi.Models.Grade > grade in gradesDictionary)
            {
                gradesList.Add(new AbetApi.EFModels.Grade(grade.Key, grade.Value.A, grade.Value.B, grade.Value.C, grade.Value.D, grade.Value.F, grade.Value.W, grade.Value.I, grade.Value.TotalStudents));
            }

            return gradesList;
        }

        //This function takes a list of EFModels.Grade objects and converts them to a dictionary of Models.Grade objects
        public static Dictionary<string, AbetApi.Models.Grade> ConvertToModelGrade(List<AbetApi.EFModels.Grade> gradesList)
        {
            //Creates a dictionary to store data in a format requested by the front end team
            Dictionary<string, AbetApi.Models.Grade> dictionary = new Dictionary<string, AbetApi.Models.Grade>();

            //For each grade object, convert the object to the requested data format
            foreach (EFModels.Grade grade in gradesList)
            {
                dictionary.Add(grade.Major, new AbetApi.Models.Grade(grade));
            }

            return dictionary;
        }
    }
}
