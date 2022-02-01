using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AbetApi.Data;

namespace AbetApi.EFModels
{
    // This class assigns outcomes to a course. There will be a course outcome object for each major attached to each class.
    // For example, if a course is completed, it could accomplish outcomes 1 and 4 for CS, but 2 and 5 for an IT major. Each major will have
    //      a course outcome object, which maps the major to its accomplished outcomes.
    public class CourseOutcome
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CourseOutcomeId { get; set; }
        public string Major { get; set; } //Searching for the major WILL BE case sensitive
        //public List<string> Outcomes; // This will store designators for major objectives, such as 1/2/etc...
        [JsonIgnore]
        public ICollection<MajorOutcome> MajorOutcomes { get; set; }

        public CourseOutcome()
        {
            this.MajorOutcomes = new List<MajorOutcome>();
        }

        public CourseOutcome(string major)
        {
            this.Major = major;
        }

        //This is used to add a course outcome to a course.
        public static void CreateCourseOutcome(string term, int year, string classDepartment, string courseNumber, CourseOutcome courseOutcome)
        {
            using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                //Finds the semester
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        context.CourseOutcomes.Add(courseOutcome);
                        course.CourseOutcomes.Add(courseOutcome);
                        context.SaveChanges();
                    }
                }
            }
        }

        //This function finds a major under a course and deletes the course outcome container
        public static void DeleteCourseOutcome(string term, int year, string classDepartment, string courseNumber, string majorName)
        {
            using (var context = new ABETDBContext())
            {
                //Find the semester
                //Find the course
                //find the course outcome
                //removes the course outcome object for the provided course/major name.

                //FIXME - Add null checking. It breaks right now if the class doesn't exist, or if the objective doesn't exist.
                //Finds the semester
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Finds the course outcome
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();
                        foreach (var courseOutcome in course.CourseOutcomes)
                        {
                            if (courseOutcome.Major == majorName)
                            {
                                context.Remove(courseOutcome);
                                context.SaveChanges();
                                break;
                            }
                        }
                    }
                }
            }
        }

        // This is used to add an outcome (from a major) to the course outcome object.
        // The major and outcome must already exist before you call this function
        public static void AddMajorOutcome(string term, int year, string classDepartment, string courseNumber, string majorName, string outcomeName)
        {
            using (var context = new ABETDBContext())
            {
                //Find the semester
                //Find the course
                //find the course outcome
                //Add the designator string

                //FIXME - Add null checking. It breaks right now if the class doesn't exist, or if the objective doesn't exist.
                //Finds the semester
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Finds the MajorOutcome
                MajorOutcome tempMajorOutcome = null;
                context.Entry(semester).Collection(semester => semester.Majors).Load();
                //Finds the relevant major
                foreach(var major in semester.Majors)
                {
                    if(major.Name == majorName)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();
                        //Finds the specific major outcome
                        foreach(var majorOutcome in major.MajorOutcomes)
                        {
                            if (outcomeName == majorOutcome.Name)
                                tempMajorOutcome = majorOutcome;
                        }
                    }
                }

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach(var course in semester.Courses)
                {
                    if(course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Finds the course outcome
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();
                        foreach(var courseOutcome in course.CourseOutcomes)
                        {
                            if(courseOutcome.Major == majorName)
                            {
                                //Adds the outcome designator to the course outcomes
                                courseOutcome.MajorOutcomes.Add(tempMajorOutcome);
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        //This is used to remove an outcome (from a major) from the course outcome object.
        public static void RemoveMajorOutcome(string term, int year, string classDepartment, string courseNumber, string majorName, string outcomeName)
        {
            using (var context = new ABETDBContext())
            {
                //Find the semester
                //Find the course
                //find the course outcome
                //Add the designator string

                //FIXME - Add null checking. It breaks right now if the class doesn't exist, or if the objective doesn't exist.
                //Finds the semester
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Finds the MajorOutcome
                MajorOutcome tempMajorOutcome = null;
                context.Entry(semester).Collection(semester => semester.Majors).Load();
                //Finds the relevant major
                foreach (var major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();
                        //Finds the specific major outcome
                        foreach (var majorOutcome in major.MajorOutcomes)
                        {
                            if (outcomeName == majorOutcome.Name)
                                tempMajorOutcome = majorOutcome;
                        }
                    }
                }

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Finds the course outcome
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();
                        foreach (var courseOutcome in course.CourseOutcomes)
                        {
                            if (courseOutcome.Major == majorName)
                            {
                                //Finds the outcome attached to the course, and removes it from the list
                                //It does not delete the outcome, or the courseoutcome. It just kills the join table entry.
                                context.Entry(courseOutcome).Collection(courseOutcome => courseOutcome.MajorOutcomes).Load();
                                foreach(var outcome in courseOutcome.MajorOutcomes)
                                {
                                    if(outcome.Name == outcomeName)
                                    {
                                        courseOutcome.MajorOutcomes.Remove(outcome);
                                        context.SaveChanges();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
