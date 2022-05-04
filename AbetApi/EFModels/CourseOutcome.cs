using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AbetApi.Data;
using System;

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

        public static async Task<List<MajorOutcome>> GetLinkedMajorOutcomes(string term, int year, string department, string courseNumber, string courseOutcomeName, string majorName)
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

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the course outcome name is null or empty.
            if (courseOutcomeName == null || courseOutcomeName == "")
            {
                throw new ArgumentException("The course outcome name cannot be empty.");
            }

            //Check if the major name is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The major name cannot be empty.");
            }

            //Format term, department, course outcome name, and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();
            courseOutcomeName = courseOutcomeName[0].ToString().ToUpper() + courseOutcomeName[1..].ToLower();
            majorName = majorName.ToString().ToUpper();

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Try to find the specified major name.
                Major validMajor = context.Majors.FirstOrDefault(p => p.Name == majorName);

                //Check that the major name is a valid major.
                if (validMajor == null)
                {
                    throw new ArgumentException("The major specified does not exist in the database.");
                }

                //Load the courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the semester has courses.
                if(semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    //if it finds the course, Find all existing course outcomes
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        //Loads existing course outcomes
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();

                        //Check that the course specified has course outcomes.
                        if (course.CourseOutcomes.Count == 0)
                        {
                            throw new ArgumentException("The course specified has no course outcomes.");
                        }

                        //Try to find the course outcome specified.
                        foreach(CourseOutcome courseOutcome in course.CourseOutcomes)
                        {
                            //If the specified course outcome name is found, load it's associated major outcomes
                            if(courseOutcome.Name == courseOutcomeName)
                            {
                                //Loads major outcomes
                                context.Entry(courseOutcome).Collection(courseOutcome => courseOutcome.MajorOutcomes).Load();

                                //Check that the course outcome specified is linked to major outcomes.
                                if (courseOutcome.MajorOutcomes.Count == 0)
                                {
                                    throw new ArgumentException("The course outcome specified has no linked major outcomes.");
                                }
                                
                                //Scan through major outcomes, and remove the entries that are not the major you're looking for
                                List<MajorOutcome> majorOutcomes = new List<MajorOutcome>();
                                foreach(MajorOutcome majorOutcome in courseOutcome.MajorOutcomes)
                                {
                                    context.Entry(majorOutcome).Collection(majorOutcome => majorOutcome.Majors).Load();

                                    foreach(Major major in majorOutcome.Majors)
                                    {
                                        if(major.Name == majorName)
                                        {
                                            majorOutcomes.Add(majorOutcome);
                                        }
                                    }
                                }

                                //Check if any major outcomes had the major name specified.
                                if (majorOutcomes.Count == 0)
                                {
                                    throw new ArgumentException("The major specified has no major outcomes linked to the course outcome specified.");
                                }

                                //Returns those major outcomes as a list
                                return majorOutcomes;
                            }
                        }
                        throw new ArgumentException("The course outcome specified does not exist in the database.");
                    }
                }
                throw new ArgumentException("The course specified does not exist in the database.");
            }
        }

        //This is used to add a course outcome to a course.
        public static async Task CreateCourseOutcome(string term, int year, string department, string courseNumber, CourseOutcome courseOutcome)
        {
            // Sets the course outcome id to be 0, so entity framework will give it a primary key
            courseOutcome.CourseOutcomeId = 0;

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

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the course outcome name is null or empty.
            if (courseOutcome.Name == null || courseOutcome.Name == "")
            {
                throw new ArgumentException("The course outcome name cannot be empty.");
            }

            //Check if the course outcome description is null or empty.
            if (courseOutcome.Description == null || courseOutcome.Description == "")
            {
                throw new ArgumentException("The course outcome description cannot be empty.");
            }

            //Format term, department, and course outcome name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();
            courseOutcome.Name = courseOutcome.Name[0].ToString().ToUpper() + courseOutcome.Name[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                CourseOutcome duplicateCourseOutcome = null;
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

                //Check that the semester has courses.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    //if it finds the course, Find all existing course outcomes
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Check if the course exists.
                if (tempCourse == null)
                {
                    throw new ArgumentException("The course specified does not exist in the database.");
                }

                //Loads existing course outcomes
                context.Entry(tempCourse).Collection(course => course.CourseOutcomes).Load();

                //Try to find the course outcome specified.
                foreach (CourseOutcome tempCourseOutcome in tempCourse.CourseOutcomes)
                {
                    if (tempCourseOutcome.Name == courseOutcome.Name)
                    {
                        duplicateCourseOutcome = tempCourseOutcome;
                        break;
                    }
                }

                //Check if the entry is a duplicate.
                if(duplicateCourseOutcome != null)
                {
                    throw new ArgumentException("That course outcome already exists in the database.");
                }

                //If the entry doesn't exist, add it.
                context.CourseOutcomes.Add(courseOutcome);
                tempCourse.CourseOutcomes.Add(courseOutcome);
                context.SaveChanges();
            }
        } // CreateCourseOutcome

        public static async Task<List<CourseOutcome>> GetCourseOutcomes(string term, int year, string department, string courseNumber)
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

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
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

                //Check that the semester has courses.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no courses.");
                }

                //Try to find the course specified.
                foreach (Course course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        //Loads existing course outcomes
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();

                        //Check if the course has course outcomes.
                        if(course.CourseOutcomes.Count == 0)
                        {
                            throw new ArgumentException("The course specified has no course outcomes.");
                        }

                        //Returns course outcomes in the form of a list
                        return course.CourseOutcomes.ToList();
                    }
                }

                throw new ArgumentException("The course specified does not exist in the database.");

            }
        }

        //This function finds a course outcome under a course and edits its information with the new information.
        public static async Task EditCourseOutcome(string term, int year, string department, string courseNumber, string courseOutcomeName, CourseOutcome newCourseOutcome)
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

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the course outcome name is null or empty.
            if (courseOutcomeName == null || courseOutcomeName == "")
            {
                throw new ArgumentException("The course outcome name cannot be empty.");
            }

            //Check if the new course outcome name is null or empty.
            if (newCourseOutcome.Name == null || newCourseOutcome.Name == "")
            {
                throw new ArgumentException("The new course outcome name cannot be empty.");
            }

            //Check if the new course outcome description is null or empty.
            if (newCourseOutcome.Description == null || newCourseOutcome.Description == "")
            {
                throw new ArgumentException("The new course outcome deccription cannot be empty.");
            }

            //Format term, department, and course outcome name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();
            courseOutcomeName = courseOutcomeName[0].ToString().ToUpper() + courseOutcomeName[1..].ToLower();
            newCourseOutcome.Name = newCourseOutcome.Name[0].ToString().ToUpper() + newCourseOutcome.Name[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                CourseOutcome tempCourseOutcome = null;
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

                //Check that the semester has courses.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no courses.");
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

                //Loads existing course outcomes
                context.Entry(tempCourse).Collection(course => course.CourseOutcomes).Load();

                //Check that the course has course outcomes.
                if (tempCourse.CourseOutcomes.Count == 0)
                {
                    throw new ArgumentException("The course specified has no course outcomes.");
                }

                //If it does, then try to find the course outcome specified.
                foreach (CourseOutcome courseOutcome in tempCourse.CourseOutcomes)
                {
                    //Check to make sure that the new course outcome identifier isn't a duplicate.
                    if (courseOutcome.Name == newCourseOutcome.Name)
                    {
                        throw new ArgumentException("The new course outcome identifier already exists in the database.");
                    }

                    if (courseOutcome.Name == courseOutcomeName)
                    {
                        tempCourseOutcome = courseOutcome;
                    }
                }

                //Check that the course outcome specified was found.
                if (tempCourseOutcome == null)
                {
                    throw new ArgumentException("The course outcome specified does not exist in the database.");
                }

                //Remove the course specified.
                tempCourseOutcome.Name = newCourseOutcome.Name;
                tempCourseOutcome.Description = newCourseOutcome.Description;
                context.SaveChanges();
            }
        }

        //This function finds a course outcome under a course and deletes the course outcome container
        public static async Task DeleteCourseOutcome(string term, int year, string department, string courseNumber, string courseOutcomeName)
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

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the course outcome name is null or empty.
            if (courseOutcomeName == null || courseOutcomeName == "")
            {
                throw new ArgumentException("The course outcome name cannot be empty.");
            }

            //Format term, department, and course outcome name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();
            courseOutcomeName = courseOutcomeName[0].ToString().ToUpper() + courseOutcomeName[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                //Find the semester
                //Find the course
                //find the course outcome
                //removes the course outcome object for the provided course/major name.

                CourseOutcome tempCourseOutcome = null;
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

                //Check that the semester has courses.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no courses.");
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

                //Loads existing course outcomes
                context.Entry(tempCourse).Collection(course => course.CourseOutcomes).Load();

                //Check that the course has course outcomes.
                if (tempCourse.CourseOutcomes.Count == 0)
                {
                    throw new ArgumentException("The course specified has no course outcomes.");
                }

                //If it does, then try to find the course outcome specified.
                foreach (CourseOutcome courseOutcome in tempCourse.CourseOutcomes)
                {
                    if (courseOutcome.Name == courseOutcomeName)
                    {
                        tempCourseOutcome = courseOutcome;
                        break;
                    }
                }

                //Check that the course outcome specified was found.
                if(tempCourseOutcome == null)
                {
                    throw new ArgumentException("The course outcome specified does not exist in the database.");
                }

                //Remove the course outcome specified.
                context.Remove(tempCourseOutcome);
                context.SaveChanges();
            }
        } // DeleteCourseOutcome

        // This is used to add an outcome (from a major) to the course outcome object.
        // The major and outcome must already exist before you call this function
        public static async Task LinkToMajorOutcome(string term, int year, string department, string courseNumber, string courseOutcomeName, string majorName, string majorOutcomeName)
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

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the course outcome name is null or empty.
            if (courseOutcomeName == null || courseOutcomeName == "")
            {
                throw new ArgumentException("The course outcome name cannot be empty.");
            }

            //Check if the major name is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The major name cannot be empty.");
            }

            //Check major outcome name is null or empty.
            if (majorOutcomeName == null || majorOutcomeName == "")
            {
                throw new ArgumentException("The major outcome name cannot be empty.");
            }

            //Format term, department, course outcome name, and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();
            courseOutcomeName = courseOutcomeName[0].ToString().ToUpper() + courseOutcomeName[1..].ToLower();
            majorName = majorName.ToUpper();
            majorOutcomeName = majorOutcomeName[0].ToString().ToUpper() + majorOutcomeName[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                //Find the semester
                //Find the course
                //find the course outcome
                //Add the designator string

                Major tempMajor = null;
                MajorOutcome tempMajorOutcome = null;
                Course tempCourse = null;
                CourseOutcome tempCourseOutcome = null;

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Check that the semester has majors.
                if (semester.Majors.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no majors.");
                }

                //Finds the relevant major
                foreach (Major major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        tempMajor = major;
                        break;
                    }
                }

                //Check if the major specified exists.
                if (tempMajor == null)
                {
                    throw new ArgumentException("The major specified does not exist in the database.");
                }

                //Load the major outcomes under the specified major.
                context.Entry(tempMajor).Collection(major => major.MajorOutcomes).Load();

                //Check that the major has major outcomes.
                if (tempMajor.MajorOutcomes.Count == 0)
                {
                    throw new ArgumentException("The major specified has no major outcomes.");
                }

                //Finds the specific major outcome
                foreach (MajorOutcome majorOutcome in tempMajor.MajorOutcomes)
                {
                    if (majorOutcomeName == majorOutcome.Name)
                        tempMajorOutcome = majorOutcome;
                }

                //Check if the major outcome specified exists.
                if (tempMajorOutcome == null)
                {
                    throw new ArgumentException("The major outcome specified does not exist in the database.");
                }

                //Load courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the semester has courses.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no courses.");
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

                //Check if the course specified exists.
                if(tempCourse == null)
                {
                    throw new ArgumentException("The course specified does not exist in the database.");
                }

                //Load course outcomes under the specified course.
                context.Entry(tempCourse).Collection(course => course.CourseOutcomes).Load();

                //Check that the course has course outcomes.
                if(tempCourse.CourseOutcomes.Count == 0)
                {
                    throw new ArgumentException("The course specified has no course outcomes.");
                }

                //Try to find the course outcome specified.
                foreach (CourseOutcome courseOutcome in tempCourse.CourseOutcomes)
                {
                    if (courseOutcome.Name == courseOutcomeName)
                    {
                        tempCourseOutcome = courseOutcome;
                        break;
                    }
                }

                //Check if the course outcome specified exists.
                if (tempCourseOutcome == null)
                {
                    throw new ArgumentException("The course outcome specified does not exist in the database.");
                }

                //Load the links between a course outcome and major outcomes.
                context.Entry(tempCourseOutcome).Collection(courseOutcome => courseOutcome.MajorOutcomes).Load();

                foreach (MajorOutcome majorOutcome in tempCourseOutcome.MajorOutcomes)
                {
                    if(majorOutcome.Name == majorOutcomeName)
                    {
                        throw new ArgumentException("The course outcome specified already has a link to the major outcome specified.");
                    }
                }
                
                //Adds the outcome designator to the course outcomes
                tempCourseOutcome.MajorOutcomes.Add(tempMajorOutcome);
                context.SaveChanges();
            }
        } // AddMajorOutcome

        //This is used to remove an outcome (from a major) from the course outcome object.
        public static async Task RemoveLinkToMajorOutcome(string term, int year, string department, string courseNumber, string courseOutcomeName, string majorName, string majorOutcomeName)
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

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            //Check if the course outcome name is null or empty.
            if (courseOutcomeName == null || courseOutcomeName == "")
            {
                throw new ArgumentException("The course outcome name cannot be empty.");
            }

            //Check if the major name is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The major name cannot be empty.");
            }

            //Check major outcome name is null or empty.
            if (majorOutcomeName == null || majorOutcomeName == "")
            {
                throw new ArgumentException("The major outcome name cannot be empty.");
            }

            //Format term, department, course outcome name, and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            department = department.ToUpper();
            courseOutcomeName = courseOutcomeName[0].ToString().ToUpper() + courseOutcomeName[1..].ToLower();
            majorName = majorName.ToUpper();
            majorOutcomeName = majorOutcomeName[0].ToString().ToUpper() + majorOutcomeName[1..].ToLower();

            await using (var context = new ABETDBContext())
            {
                //Find the semester
                //Find the course
                //find the course outcome
                //Add the designator string

                Major tempMajor = null;
                MajorOutcome tempMajorOutcome = null;
                Course tempCourse = null;
                CourseOutcome tempCourseOutcome = null;

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Check that the semester has majors.
                if (semester.Majors.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no majors.");
                }

                //Finds the relevant major
                foreach (Major major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        tempMajor = major;
                        break;
                    }
                }

                //Check if the major specified exists.
                if (tempMajor == null)
                {
                    throw new ArgumentException("The major specified does not exist in the database.");
                }

                //Load the major outcomes under the specified major.
                context.Entry(tempMajor).Collection(major => major.MajorOutcomes).Load();

                //Check that the major has major outcomes.
                if (tempMajor.MajorOutcomes.Count == 0)
                {
                    throw new ArgumentException("The major specified has no major outcomes.");
                }

                //Finds the specific major outcome
                foreach (MajorOutcome majorOutcome in tempMajor.MajorOutcomes)
                {
                    if (majorOutcomeName == majorOutcome.Name)
                    {
                        tempMajorOutcome = majorOutcome;
                        break;
                    }
                }

                //Check if the major outcome specified exists.
                if (tempMajorOutcome == null)
                {
                    throw new ArgumentException("The major outcome specified does not exist in the database.");
                }

                //Load courses under the specified semester.
                context.Entry(semester).Collection(semester => semester.Courses).Load();

                //Check that the semester has courses.
                if (semester.Courses.Count == 0)
                {
                    throw new ArgumentException("The semester specified has no courses.");
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

                //Check if the course specified exists.
                if (tempCourse == null)
                {
                    throw new ArgumentException("The course specified does not exist in the database.");
                }

                //Load course outcomes under the specified course.
                context.Entry(tempCourse).Collection(course => course.CourseOutcomes).Load();

                //Check that the course has course outcomes.
                if (tempCourse.CourseOutcomes.Count == 0)
                {
                    throw new ArgumentException("The course specified has no course outcomes.");
                }

                //Try to find the course outcome specified.
                foreach (CourseOutcome courseOutcome in tempCourse.CourseOutcomes)
                {
                    if (courseOutcome.Name == courseOutcomeName)
                    {
                        tempCourseOutcome = courseOutcome;
                        break;
                    }
                }

                //Check if the course outcome specified exists.
                if (tempCourseOutcome == null)
                {
                    throw new ArgumentException("The course outcome specified does not exist in the database.");
                }

                //Load major outcomes linked to the course outcome specified.
                context.Entry(tempCourseOutcome).Collection(courseOutcome => courseOutcome.MajorOutcomes).Load();

                //We set tempMajorOutcome to null again for the error check later to verify if there is a link between the course outcome and major outcome specified.
                tempMajorOutcome = null;

                //Finds the specific major outcome
                foreach (MajorOutcome majorOutcome in tempCourseOutcome.MajorOutcomes)
                {
                    if (majorOutcomeName == majorOutcome.Name)
                    {
                        tempMajorOutcome = majorOutcome;
                        break;
                    }
                }

                //Check if the major outcome specified is linked to the course outcome specified.
                if (tempMajorOutcome == null)
                {
                    throw new ArgumentException("The course outcome specified does not have a link to the major outcome specified.");
                }

                //Adds the outcome designator to the course outcomes
                tempCourseOutcome.MajorOutcomes.Remove(tempMajorOutcome);
                context.SaveChanges();
            }
        } // RemoveMajorOutcome
    } // CourseOutcome
}
