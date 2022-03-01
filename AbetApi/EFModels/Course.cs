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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CourseId { get; set; }
        public string CoordinatorEUID { get; set; }
        public string CourseNumber { get; set; } //Ex: 2100
        public string DisplayName { get; set; } //Ex: "Assembly Langauge And Computer Organization"
        public string CoordinatorComment { get; set; }
        public bool IsCourseCompleted { get; set; } // (if it's true, can't edit any more) Ask for a confirmation before setting to true
        public string Department { get; set; } //Ex. "CSCE"
        [JsonIgnore]
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

        public async static Task AddCourse(string term, int year, Course course)
        {
            course.CourseId = 0;

            await using (var context = new ABETDBContext())
            {
                //FIXME - This probably needs a null check, or it'll break here if they try to add a course to a null semester
                //var semester = Semester.GetSemester(term, year);
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Courses.Add(course);
                semester.Courses.Add(course);
                context.SaveChanges();

                return;
            }
        } // AddCourse

        public async static Task<Course> GetCourse(string term, int year, string department, string courseNumber)
        {
            await using var context = new ABETDBContext();

            //This code block finds the given semester, and then searches its courses for the provided course details
            //This search assumes there will only be one department/course number combo.
            //FIXME - Add a null check
            Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
            context.Entry(semester).Collection(semester => semester.Courses).Load();
            foreach (var course in semester.Courses)
            {
                if (course.Department == department && course.CourseNumber == courseNumber)
                {
                    return course;
                }
            }
            return null;
        } // GetCourse

        public async static Task EditCourse(string term, int year, string department, string courseNumber, Course NewValue)
        {
            await using (var context = new ABETDBContext())
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
                return;
            }
        } // EditCourse

        // THIS THREW THREE EXCEPTIONS ONE MISTAKEN INPUT, ONE FROM ENTITY and TWO FROM SYSTEM.PRIVATE.CORELIB. COULD USE MORE DETAIL FOR THROWN EXCEPTIONS
        // MIGHT BE STANDARD, BUT MESSAGE WAS ALSO 'SEE INTERIOR EXCEPTION'
        public async static Task DeleteCourse(string term, int year, string department, string courseNumber)
        {
            await using (var context = new ABETDBContext())
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
                return;
            }
        } // DeleteCourse

        // This function gets all the sections from the course specified by the input arguments
        public static async Task<List<Section>> GetSections(string term, int year, string department, string courseNumber)
        {
            List<Section> list = new List<Section>();

            await using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        context.Entry(course).Collection(course => course.Sections).Load();
                        foreach (var section in course.Sections)
                        {
                            list.Add(section);
                        }

                    }
                }
                return list;
            }
        } // GetSections

        public static async Task<List<string>> getMajorsThatRequireCourse(string term, int year, string department, string courseNumber)
        {
            List<string> list = new List<string>();

            await using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();
                        foreach (var courseOutcomes in course.CourseOutcomes)
                        {
                            list.Add(courseOutcomes.Major);
                        }

                    }
                }
                return list;
            }
        } // getMajorsThatRequireCourse


        //this function returns a list of all courses in a given department for a given semester
        public static async Task<List<Course>> GetCoursesByDepartment(string term, int year, string department)
        {
            List<Course> list = new List<Course>();
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department)
                    {
                        list.Add(course);
                    }
                }
            }
            return list;
        } // GetCoursesByDepartment

        //this function returns a list of all course names in a given department for a given semester
        public static async Task<List<string>> GetCourseNamesByDepartment(string term, int year, string department)
        {
            List<string> list = new List<string>();
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department)
                    {
                        list.Add(course.DisplayName);
                    }
                }
            }
            return list;
        } // GetCourseNamesByDepartment

        //this function returns a list of all departments that have courses for a given semester
        public static async Task<List<string>> GetDepartments(string term, int year)
        {
            HashSet<string> list = new HashSet<string>();
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var courses in semester.Courses)
                {
                    list.Add(courses.Department);

                }
                return list.ToList();
            }
        } // GetDepartments
    } // Course
}
