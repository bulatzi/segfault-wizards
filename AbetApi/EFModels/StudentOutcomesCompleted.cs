using System;
using AbetApi.Data;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbetApi.EFModels
{
    // This object is a container for storing multiple majors and numbers of students who completed various outcomes.
    public class StudentOutcomesCompleted
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int StudentOutcomesCompletedId { get; set; }
        public string Term { get; set; }
        public int Year { get; set; }
        public string ClassDepartment { get; set; }
        public string CourseNumber { get; set; }
        public string CourseOutcomeName { get; set; }
        public string SectionName { get; set; }
        public string MajorName { get; set; }
        public int StudentsCompleted { get; set; }
        public StudentOutcomesCompleted(string Term, int Year, string ClassDepartment, string CourseNumber, string SectionName, string CourseOutcomeName,  string MajorName, int StudentsCompleted)
        {
            this.Term = Term;
            this.Year = Year;
            this.ClassDepartment = ClassDepartment;
            this.CourseNumber = CourseNumber;
            this.CourseOutcomeName = CourseOutcomeName;
            this.SectionName = SectionName;
            this.MajorName = MajorName;
            this.StudentsCompleted = StudentsCompleted;
        }

        public async static Task SetStudentOutcomesCompleted(string Term, int Year, string ClassDepartment, string CourseNumber, string SectionName, string CourseOutcomeName, string MajorName, int StudentsCompleted)
        {
            await using(var context = new ABETDBContext())
            {
                //Search existing items. If one of that major name already exists, overwrite it and exit early

                List<StudentOutcomesCompleted> doesExist = context.StudentOutcomesCompleted.Where(p => p.Term == Term && p.Year == Year && p.ClassDepartment == ClassDepartment && p.CourseNumber == CourseNumber && p.SectionName == SectionName && p.CourseOutcomeName == CourseOutcomeName && p.MajorName == MajorName).ToList();
                if(doesExist.Any())
                {
                    foreach(var tempStudentOutcomesCompleted in doesExist)
                    {
                        if (tempStudentOutcomesCompleted.MajorName == MajorName)
                        {
                            tempStudentOutcomesCompleted.StudentsCompleted = StudentsCompleted;
                            context.SaveChanges();
                            return;
                        }
                    }
                }

                //If the object doesn't exist, add it and return
                StudentOutcomesCompleted studentOutcomesCompleted = new StudentOutcomesCompleted(Term, Year, ClassDepartment, CourseNumber, SectionName, CourseOutcomeName, MajorName, StudentsCompleted);
                studentOutcomesCompleted.StudentOutcomesCompletedId = 0;

                context.StudentOutcomesCompleted.Add(studentOutcomesCompleted);
                context.SaveChanges();

                return;
            }

        }

        public async static Task<List<StudentOutcomesCompleted>> GetStudentOutcomesCompleted(string Term, int Year, string ClassDepartment, string CourseNumber, string SectionName)
        {
            //each object is a standalone object
            //write it as a linq statement, matching on all the parameters provided in the function call

            await using (var context = new ABETDBContext())
            {
                List<StudentOutcomesCompleted> studentOutcomesCompleted = context.StudentOutcomesCompleted.Where<StudentOutcomesCompleted>(p => p.Term == Term && p.Year == Year && p.ClassDepartment == ClassDepartment && p.CourseNumber == CourseNumber && p.SectionName == SectionName).ToList();

                return studentOutcomesCompleted;
            }
        }

        public async static Task<List<StudentOutcomesCompleted>> GetSemesterStudentOutcomesCompleted(string Term, int Year)
        {
            await using (var context = new ABETDBContext())
            {
                List<StudentOutcomesCompleted> studentOutcomesCompleted = context.StudentOutcomesCompleted.Where<StudentOutcomesCompleted>(p => p.Term == Term && p.Year == Year).ToList();

                return studentOutcomesCompleted;
            }
        }
    }
}
