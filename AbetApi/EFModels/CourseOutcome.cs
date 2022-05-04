using System.Threading.Tasks;
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
        public string Name { get; set; } //
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<MajorOutcome> MajorOutcomes { get; set; }
        [JsonIgnore]
        public ICollection<Course> Courses { get; set; } // This will hold the singular course this outcome belongs to

        public CourseOutcome()
        {
            this.MajorOutcomes = new List<MajorOutcome>();
            this.Courses = new List<Course>();
        }

        public CourseOutcome(string Name, string Description)
        {
            this.Name = Name;
            this.Description = Description;
        }

        public static async Task<List<MajorOutcome>> GetLinkedMajorOutcomes(string term, int year, string classDepartment, string courseNumber, string courseOutcomeName, string majorName)
        {
            await using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                //Finds the semester
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (Course course in semester.Courses)
                {
                    //if it finds the course, Find all existing course outcomes
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Loads existing course outcomes
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();

                        foreach(var courseOutcome in course.CourseOutcomes)
                        {
                            //If the specified course outcome name is found, load it's associated major outcomes
                            if(courseOutcome.Name == courseOutcomeName)
                            {
                                //Loads major outcomes
                                context.Entry(courseOutcome).Collection(courseOutcome => courseOutcome.MajorOutcomes).Load();
                                
                                //Scan through major outcomes, and remove the entries that are not the major you're looking for
                                List<MajorOutcome> majorOutcomes = new List<MajorOutcome>();
                                foreach(var majorOutcome in courseOutcome.MajorOutcomes)
                                {
                                    context.Entry(majorOutcome).Collection(majorOutcome => majorOutcome.Majors).Load();
                                    foreach(var major in majorOutcome.Majors)
                                    {
                                        if(major.Name == majorName)
                                        {
                                            majorOutcomes.Add(majorOutcome);
                                        }
                                    }
                                }
                                //Returns those major outcomes as a list
                                return majorOutcomes;
                            }
                        }
                        return null;
                    }
                }
                return null;
            }
        }

        //This is used to add a course outcome to a course.
        public static async Task CreateCourseOutcome(string term, int year, string classDepartment, string courseNumber, CourseOutcome courseOutcome)
        {
            await using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                //Finds the semester
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (Course course in semester.Courses)
                {
                    //if it finds the course, create/update the course outcomes
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Check if the course outcomes already exist. If they do, change them.
                        //If they don't, create them

                        //Loads existing course outcomes
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();

                        //If the entry already exists, edit the description and return early
                        foreach(var outcome in course.CourseOutcomes)
                        {
                            if(outcome.Name == courseOutcome.Name)
                            {
                                outcome.Description = courseOutcome.Description;
                                context.SaveChanges();
                                return;
                            }
                        }

                        //If the entry doesn't exist, add it.
                        context.CourseOutcomes.Add(courseOutcome);
                        course.CourseOutcomes.Add(courseOutcome);
                        context.SaveChanges();
                    }
                }
                return;
            }
        } // CreateCourseOutcome

        public static async Task<List<CourseOutcome>> GetCourseOutcomes(string term, int year, string classDepartment, string courseNumber)
        {

            await using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                //Finds the semester
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (Course course in semester.Courses)
                {
                    //if it finds the course, Find all existing course outcomes
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Loads existing course outcomes
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();

                        //Returns course outcomes in the form of a list
                        return course.CourseOutcomes.ToList();
                    }
                }
                return null;
            }
        }

        //This function finds a major under a course and deletes the course outcome container
        public static async Task DeleteCourseOutcome(string term, int year, string classDepartment, string courseNumber, string name)
        {
            await using (var context = new ABETDBContext())
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
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Finds the course outcome
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();
                        foreach (CourseOutcome courseOutcome in course.CourseOutcomes)
                        {
                            if (courseOutcome.Name == name)
                            {
                                context.Remove(courseOutcome);
                                context.SaveChanges();
                                break;
                            }
                        }
                    }
                }
                return;
            }
        } // DeleteCourseOutcome

        // NEEDS DUPLICATE DATA HANDLING
        // This is used to add an outcome (from a major) to the course outcome object.
        // The major and outcome must already exist before you call this function
        public static async Task LinkToMajorOutcome(string term, int year, string classDepartment, string courseNumber, string courseOutcomeName, string majorName, string majorOutcomeName)
        {
            await using (var context = new ABETDBContext())
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
                foreach (Major major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();
                        //Finds the specific major outcome
                        foreach (MajorOutcome majorOutcome in major.MajorOutcomes)
                        {
                            if (majorOutcomeName == majorOutcome.Name)
                                tempMajorOutcome = majorOutcome;
                        }
                    }
                }

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Finds the course outcome
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();
                        foreach (CourseOutcome courseOutcome in course.CourseOutcomes)
                        {
                            if (courseOutcome.Name == courseOutcomeName)
                            {
                                //Adds the outcome designator to the course outcomes
                                courseOutcome.MajorOutcomes.Add(tempMajorOutcome);
                                context.SaveChanges();
                            }
                        }
                    }
                }
                return;
            }
        } // AddMajorOutcome

        //This is used to remove an outcome (from a major) from the course outcome object.
        public static async Task RemoveLinkToMajorOutcome(string term, int year, string classDepartment, string courseNumber, string courseOutcomeName, string majorName, string majorOutcomeName)
        {
            await using (var context = new ABETDBContext())
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
                foreach (Major major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();
                        //Finds the specific major outcome
                        foreach (MajorOutcome majorOutcome in major.MajorOutcomes)
                        {
                            if (majorOutcomeName == majorOutcome.Name)
                                tempMajorOutcome = majorOutcome;
                        }
                    }
                }

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == classDepartment && course.CourseNumber == courseNumber)
                    {
                        //Finds the course outcome
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();
                        foreach (CourseOutcome courseOutcome in course.CourseOutcomes)
                        {
                            if (courseOutcome.Name == courseOutcomeName)
                            {
                                //Finds the outcome attached to the course, and removes it from the list
                                //It does not delete the outcome, or the courseoutcome. It just kills the join table entry.
                                context.Entry(courseOutcome).Collection(courseOutcome => courseOutcome.MajorOutcomes).Load();
                                foreach (MajorOutcome outcome in courseOutcome.MajorOutcomes)
                                {
                                    if (outcome.Name == majorOutcomeName)
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
                return;
            }
        } // RemoveMajorOutcome
    } // CourseOutcome
}
