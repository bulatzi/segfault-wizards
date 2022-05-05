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
        [JsonIgnore]
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
            this.CoordinatorEUID = coordinator;
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
            //Sets the course id to be 0, so entity framework will give it a primary key
            course.CourseId = 0;

            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the coordinator EUID is null or empty.
            if (course.CoordinatorEUID == null || course.CoordinatorEUID == "")
            {
                throw new ArgumentException("The coordinator EUID cannot be empty.");
            }

            //Check if the course number is null or empty.
            if (course.CourseNumber == null || course.CourseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the course name is null or empty.
            if (course.DisplayName == null || course.DisplayName == "")
            {
                throw new ArgumentException("The course name cannot be empty.");
            }

            //Check if the department is null or empty.
            if (course.Department == null || course.Department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
            }

            //Format term, department, and coordinator EUID to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            course.Department = course.Department.ToUpper();
            course.CoordinatorEUID = course.CoordinatorEUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Try to find the course specified to protect against duplicates.
                foreach (Course iteratorCourse in semester.Courses)
                {
                    if (iteratorCourse.CourseNumber == course.CourseNumber)
                    {
                        throw new ArgumentException("That course already exists in the database.");
                    }
                }

                //If the entry doesn't exist, add it.
                context.Courses.Add(course);
                semester.Courses.Add(course);
                context.SaveChanges();
            }
        } // AddCourse

        public async static Task<Course> GetCourse(string term, int year, string department, string courseNumber)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the department is null or empty.
            if (department == null || department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
            }

            //Format term, department, and coordinator EUID to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();

            //This code block finds the given semester, and then searches its courses for the provided course details
            //This search assumes there will only be one department/course number combo.
            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                foreach (Course course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        return course;
                    }
                }

                throw new ArgumentException("The course specified does not exist in the database.");
            }
        } // GetCourse

        public async static Task EditCourse(string term, int year, string department, string courseNumber, Course NewValue)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the old department is null or empty.
            if (department == null || department == "")
            {
                throw new ArgumentException("The old department cannot be empty.");
            }

            //Check if the old course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The old course number cannot be empty.");
            }

            //Check if the new coordinator EUID is null or empty.
            if (NewValue.CoordinatorEUID == null || NewValue.CoordinatorEUID == "")
            {
                throw new ArgumentException("The new coordinator EUID cannot be empty.");
            }

            //Check if the new course number is null or empty.
            if (NewValue.CourseNumber == null || NewValue.CourseNumber == "")
            {
                throw new ArgumentException("The new course number cannot be empty.");
            }

            //Check if the new course name is null or empty.
            if (NewValue.DisplayName == null || NewValue.DisplayName == "")
            {
                throw new ArgumentException("The new course name cannot be empty.");
            }

            //Check if the new department is null or empty.
            if (NewValue.Department == null || NewValue.Department == "")
            {
                throw new ArgumentException("The new department cannot be empty.");
            }

            //Format term, old department, new department and new coordinator EUID to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();
            NewValue.Department = NewValue.Department.ToUpper();
            NewValue.CoordinatorEUID = NewValue.CoordinatorEUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    //Check for duplicates.
                    if (course.CourseNumber == NewValue.CourseNumber)
                    {
                        throw new ArgumentException("That course number already exists in the database.");
                    }

                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                    }
                }

                //Check that the course specified was found.
                if (tempCourse == null)
                {
                    throw new ArgumentException("The course specified to edit does not exist in the database.");
                }

                //Edit the course specified.
                tempCourse.CoordinatorEUID = NewValue.CoordinatorEUID;
                tempCourse.Department = NewValue.Department;
                tempCourse.CourseNumber = NewValue.CourseNumber;
                tempCourse.DisplayName = NewValue.DisplayName;
                context.SaveChanges();
            }
        } // EditCourse

        public async static Task DeleteCourse(string term, int year, string department, string courseNumber)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the department is null or empty.
            if (department == null || department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
            }

            //Format term and department to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Check that the course specified was found.
                if (tempCourse == null)
                {
                    throw new ArgumentException("The course specified does not exist in the database.");
                }

                //Delete the course specified.
                context.Remove(tempCourse);
                context.SaveChanges();
            }
        } // DeleteCourse

        // This function gets all the sections from the course specified by the input arguments
        public static async Task<List<Section>> GetSectionsByCourse(string term, int year, string department, string courseNumber)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the department is null or empty.
            if (department == null || department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
            }

            //Format term and department to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        //Load the sections under that course.
                        context.Entry(course).Collection(course => course.Sections).Load();

                        //Check that the specified course has sections under it.
                        if (course.Sections.Count == 0)
                        {
                            throw new ArgumentException("The specified course has no sections.");
                        }

                        //Return list of sections.
                        return course.Sections.ToList();
                    }
                }

                throw new ArgumentException("The course specified does not exist in the database.");
            }
        } // GetSections

        public static async Task<List<string>> getMajorsThatRequireCourse(string term, int year, string department, string courseNumber)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the department is null or empty.
            if (department == null || department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
            }

            //Format term and department to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();

            await using (var context = new ABETDBContext())
            {
                HashSet<string> majors = new HashSet<string>();

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        //Load the course outcomes under the course specified.
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();

                        //Check that the specified course has course outcomes under it.
                        if (course.CourseOutcomes.Count == 0)
                        {
                            throw new ArgumentException("The specified course has no course outcomes.");
                        }

                        foreach (CourseOutcome courseOutcome in course.CourseOutcomes)
                        {
                            //Load the major outcomes linked to each course outcome.
                            context.Entry(courseOutcome).Collection(courseOutcome => courseOutcome.MajorOutcomes).Load();

                            foreach (MajorOutcome majorOutcome in courseOutcome.MajorOutcomes)
                            {
                                //Load the majors attached to each major outcome.
                                context.Entry(majorOutcome).Collection(majorOutcome => majorOutcome.Majors).Load();

                                foreach (Major major in majorOutcome.Majors)
                                {
                                    majors.Add(major.Name);
                                }
                            }
                        }

                        return majors.ToList();
                    }
                }

                throw new ArgumentException("The course specified does not exist in the database.");
            }
        } // getMajorsThatRequireCourse

        //this function returns a list of all courses in a given department for a given semester
        public static async Task<List<Course>> GetCoursesByDepartment(string term, int year, string department)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the department is null or empty.
            if (department == null || department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
            }

            //Format term and department to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();

            await using (var context = new ABETDBContext())
            {
                List<Course> courses = new List<Course>();

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == department)
                    {
                        courses.Add(course);
                    }
                }

                return courses;
            }
        } // GetCoursesByDepartment

        //this function returns a list of all course names in a given department for a given semester
        public static async Task<List<string>> GetCourseNamesByDepartment(string term, int year, string department)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the department is null or empty.
            if (department == null || department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
            }

            //Format term and department to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();

            await using (var context = new ABETDBContext())
            {
                List<string> courseNames = new List<string>();

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == department)
                    {
                        courseNames.Add(course.DisplayName);
                    }
                }

                return courseNames;
            }
        } // GetCourseNamesByDepartment

        //this function returns a list of all departments that have courses for a given semester
        public static async Task<List<string>> GetDepartments(string term, int year)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Format term and department to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                HashSet<string> departments = new HashSet<string>();

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    departments.Add(course.Department);
                }
                return departments.ToList();
            }
        } // GetDepartments

        //gets all the courseoutcomes assigned to a course
        public static async Task<List<MajorOutcome>> GetMajorOutcomesSatisfied(string term, int year, string department, string courseNumber)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the department is null or empty.
            if (department == null || department == "")
            {
                throw new ArgumentException("The department cannot be empty.");
            }

            //Format term and department to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();

            await using (var context = new ABETDBContext())
            {
                HashSet<MajorOutcome> majorOutcomes = new HashSet<MajorOutcome>();
                Course tempCourse = null;

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    if (course.CourseNumber == courseNumber && course.Department == department)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Load the course outcomes under the specified course
                context.Entry(tempCourse).Collection(course => course.CourseOutcomes).Load();

                foreach (CourseOutcome courseoutcome in tempCourse.CourseOutcomes)
                {
                    context.Entry(courseoutcome).Collection(courseoutcome => courseoutcome.MajorOutcomes).Load();
                    foreach (MajorOutcome majoroutcome in courseoutcome.MajorOutcomes)
                    {
                        majorOutcomes.Add(majoroutcome);
                    }
                }

                return majorOutcomes.ToList();
            }
        }//GetCoursesCourseOutcomes

        //Returns a list of all courses for a semester
        public static async Task<List<Course>> GetCourses(string term, int year)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Format term and department to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the specified semester has courses under it.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The specified semester has no courses.");
                }

                //Return the courses under the specified semester.
                return semester.Courses.ToList();
            }
        } // GetCourses

        //A function that returns a list of all the courses taught by a specific instructor

    } // Course
}
