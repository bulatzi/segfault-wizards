using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using AbetApi.Data;

namespace AbetApi.EFModels
{
    public class Course
    {
        [JsonIgnore]
        public int CourseId { get; set; }
        public string CoordinatorEUID { get; set; }
        public string CourseNumber { get; set; } //Ex: 2100
        public string DisplayName { get; set; } //Ex: "Assembly Langauge And Computer Organization"
        public string CoordinatorComment { get; set; }
        public bool IsCourseCompleted { get; set; } // (if it's true, can't edit any more) Ask for a confirmation before setting to true
        public string Department { get; set; } //Ex. "CSCE"
        public List<CourseOutcome> CourseOutcomes { get; set; } // Specific majors/outcomes will be added here
        [JsonIgnore]
        public ICollection<Major> RequiredByMajors { get; set; } // Majors that require this course will be added here
        [JsonIgnore]
        public ICollection<Section> Sections { get; set; }
        [JsonIgnore]
        public ICollection<Semester> Semesters { get; set; } //

        public Course(string coordinator, string courseNumber, string displayName, string coordinatorComment, bool isCourseCompleted, string department) : this()
        {
            this.CourseNumber = courseNumber; // e.g. 2100
            this.DisplayName = displayName; // A human readable name. (Intro to networks)
            this.CoordinatorComment = coordinatorComment; // A miscellanious comment
            this.IsCourseCompleted = isCourseCompleted; // true if it's finished, false otherwise
            this.Department = department; // e.g. CSCE
        }

        public Course()
        {
            this.CourseOutcomes = new List<CourseOutcome>();
            this.RequiredByMajors = new List<Major>();
            this.Sections = new List<Section>();
            this.Semesters = new List<Semester>();
        }

        public static void AddCourse(string term, int year, Course course)
        {
            course.CourseId = 0;

            using (var context = new ABETDBContext())
            {
                //FIXME - This probably needs a null check, or it'll break here if they try to add a course to a null semester
                //var semester = Semester.GetSemester(term, year);
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Courses.Add(course);
                semester.Courses.Add(course);
                context.SaveChanges();
            }
        }

        public static Course GetCourse(string term, int year, string department, string courseNumber)
        {
            using(var context = new ABETDBContext())
            {
                //This code block finds the given semester, and then searches its courses for the provided course details
                //This search assumes there will only be one department/course number combo.
                //FIXME - Add a null check
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach(var course in semester.Courses)
                {
                    if(course.Department == department && course.CourseNumber == courseNumber)
                    {
                        return course;
                    }
                }
                return null;
            }
        }

        public static void EditCourse(string term, int year, string department, string courseNumber, Course NewValue)
        {
            using(var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        //FIXME - make a copy constructor. Bad maintainability.
                        course.CoordinatorEUID = NewValue.CoordinatorEUID;
                        course.CourseNumber = NewValue.CourseNumber;
                        course.DisplayName = NewValue.DisplayName;
                        course.CoordinatorComment = NewValue.CoordinatorComment;
                        course.IsCourseCompleted = NewValue.IsCourseCompleted;
                        course.Department = NewValue.Department;
                        course.CourseOutcomes = NewValue.CourseOutcomes;
                        context.SaveChanges();
                        return;
                    }
                }
            }
        }

        public static void DeleteCourse(string term, int year, string department, string courseNumber)
        {
            using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        context.Remove(course);
                        context.SaveChanges();
                        return;
                    }
                }
            }
        }

        // This function gets all the sections from the course specified by the input arguments
        public static List<Section> GetSections(string term, int year, string department, string courseNumber)
        {
            List<Section> list = new List<Section>();

            using(var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        context.Entry(course).Collection(course => course.Sections).Load();
                        foreach(var section in course.Sections)
                        {
                            list.Add(section);
                        }
                        return list;
                    }
                }
                return null;
            }
        }
    }
}
