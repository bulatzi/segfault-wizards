using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AbetApi.Data;
using System.Threading.Tasks;

using System;
using System.Diagnostics;

namespace AbetApi.EFModels
{
    public class Major
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int MajorId { get; set; }
        public string Name { get; set; } // e.g. CS, CE, etc...
        [JsonIgnore]
        public ICollection<MajorOutcome> MajorOutcomes { get; set; }

        [JsonIgnore]
        public ICollection<Semester> Semesters { get; set; } //

        [JsonIgnore]
        public ICollection<Course> CoursesRequiredBy { get; set; }

        public Major()
        {
            this.Semesters = new List<Semester>();
            this.MajorOutcomes = new List<MajorOutcome>();
            this.CoursesRequiredBy = new List<Course>();
        }

        public Major(string Name)
        {
            this.Name = Name;
        }

        // FIXME - Majors are required to have a semester that they're a part of.
        public static async Task AddMajor(string term, int year, string name)
        {
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                Major major = new Major(name);

                context.Majors.Add(major);
                semester.Majors.Add(major);
                context.SaveChanges();

                return;
            }
        } // AddMajor

        public static async Task<List<Major>> GetMajors(string term, int year)
        {
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //if (semester == null)
                //     return null;

                if (semester == null)
                    throw new Exception("It was wrong lol and you should feel bad");

                context.Entry(semester).Collection(semester => semester.Majors).Load();

                return semester.Majors.ToList();
            }
        } // GetMajors

        public async static Task EditMajor(string term, int year, string name, string NewValue)
        {
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                foreach (var major in semester.Majors)
                {
                    if (major.Name == name)
                    {
                        major.Name = NewValue;
                        context.SaveChanges();
                        return;
                    }
                }
            }
        } // EditMajor

        public async static Task DeleteMajor(string term, int year, string name)
        {
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                foreach (var major in semester.Majors)
                {
                    if (major.Name == name)
                    {
                        context.Remove(major);
                        context.SaveChanges();
                        return;
                    }
                }
            }
        } // DeleteMajor

        //This function gets all of the courses required by a major
        //it takes a term and year to find a semester and the name of the major being looked into
        public async static Task<List<Course>> GetCoursesByMajor(string term, int year, string majorName)
        {
            List<Course> list = new List<Course>();
            await using (var context = new ABETDBContext())
            {
                //this finds the semester to start at and loads all the courses
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                //for every course loaded, load the course outcomes
                foreach (var course in semester.Courses)
                {
                    context.Entry(course).Collection(course => course.CourseOutcomes).Load();
                    //for every course outcome, check if it maps to the major
                    foreach (var courseOutcome in course.CourseOutcomes)
                    {
                        //if it maps to the major
                        if (courseOutcome.Major == majorName)
                        {
                            //add it to the list
                            list.Add(course);
                        }
                    }
                }
                return list;
            }
        } // GetCoursesByMajor

        public async static Task<List<MajorOutcome>> GetMajorOutcomesByMajor(string term, int year, string majorName)
        {

            List<MajorOutcome> majorOutcomes = new List<MajorOutcome>();
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();
                foreach (var major in semester.Majors)
                {
                    if (majorName == major.Name)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();
                        foreach (var majoroutcome in major.MajorOutcomes)
                        {
                            majorOutcomes.Add(majoroutcome);
                        }
                    }
                }
                return majorOutcomes.ToList();
            }
        }//GetMajorOutcomesByMajor

    } // Major
}
