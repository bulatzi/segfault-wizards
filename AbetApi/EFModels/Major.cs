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
        public async static Task<List<Course>> GetCoursesByMajor(string term, int year, string majorName)
        {
            //This function follows join tables from: Semester -> Major -> MajorOutcome -> CourseOutcome -> Course
            //This function could be very inefficient with production amounts of data in it. It's searching through every course mapped to a course/major outcome, so it could try to add the same course to the hashset as many as 7+ times.

            await using (var context = new ABETDBContext())
            {
                //this finds the semester to start at and loads all the courses
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Loads Majors
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Finds the relevant Major
                Major major = null;
                foreach (var tempMajor in semester.Majors)
                {
                    //If it finds the major, move on, otherwise return null
                    if (tempMajor.Name == majorName)
                    {
                        major = tempMajor;
                    }
                }
                if (major == null)
                    return null;

                //Loads all relevant major outcomes
                context.Entry(major).Collection(major => major.MajorOutcomes).Load();

                //Go through each list of course outcomes and add the course
                HashSet<Course> courses = new HashSet<Course>();
                foreach (var majorOutcome in major.MajorOutcomes)
                {
                    //Load course outcomes
                    context.Entry(majorOutcome).Collection(majorOutcome => majorOutcome.CourseOutcomes).Load();

                    foreach (var courseOutcome in majorOutcome.CourseOutcomes)
                    {
                        //Load the relevant course
                        context.Entry(courseOutcome).Collection(courseOutcome => courseOutcome.Courses).Load();

                        //Take that course, and add it to the hash set. This may add something to the list that's already there, but the hash set gets rid of duplicates.
                        Course tempCourse = courseOutcome.Courses.FirstOrDefault();
                        if (tempCourse != null)
                        {
                            courses.Add(tempCourse);
                        }
                    }
                }
                return courses.ToList();
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
