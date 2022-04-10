using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using AbetApi.Data;

namespace AbetApi.EFModels
{
    public class Section
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SectionId { get; set; }
        public string InstructorEUID { get; set; }
        public bool IsSectionCompleted { get; set; }
        public string SectionNumber { get; set; } //Ex: 1
        public int NumberOfStudents { get; set; }
        [JsonIgnore]
        public ICollection<Grade> Grades { get; set; }

        public Section(string instructorEUID, bool sectionCompleted, string sectionNumber, int numberOfStudents)
        {
            this.InstructorEUID = instructorEUID;
            this.IsSectionCompleted = sectionCompleted;
            this.SectionNumber = sectionNumber;
            this.NumberOfStudents = numberOfStudents;
        }

        public Section()
        {
            this.Grades = new List<Grade>();
        }

        //I am intentionally not error checking for department, because the project scope only covers the CSCE department.
        //This means that the department member variable for the class Course should be removed and I am anticipating that.
        public static async Task AddSection(string term, int year, string department, string courseNumber, Section section)
        {
            // Sets the section id to be 0, so entity framework will give it a primary key
            section.SectionId = 0;

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

            //Check if the instructor EUID is null or empty.
            if (section.InstructorEUID == null || section.InstructorEUID == "")
            {
                throw new ArgumentException("The instructor EUID cannot be empty.");
            }

            //IsSectionCompleted does not need to be checked, because if a value is not given, then it defaults to 0.

            //Check if the section number is null or empty.
            if(section.SectionNumber == null || section.SectionNumber == "")
            {
                throw new ArgumentException("The section number cannot be empty.");
            }

            //Check if the number of students is greater than 0.
            if(section.NumberOfStudents < 1)
            {
                throw new ArgumentException("The number of students cannot be zero.");
            }

            //Format term and instructor EUID to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);
            //When formatting EUID I want to ask Ludi if an EUID will always be three letters followed by 4 numbers, or if we don't care to check formatting that specifically.
            section.InstructorEUID = section.InstructorEUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;

                //Find the semester/course the section will belong to
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Check if the semester is null.
                if(semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Check if course is null.
                if(tempCourse == null)
                {
                    throw new ArgumentException("The specified course does not exist in the database.");
                }

                //Load the sections under the course specified.
                context.Entry(tempCourse).Collection(course => course.Sections).Load();

                //Try to find the new section in the database.
                foreach (Section duplicateSection in tempCourse.Sections)
                {
                    //If we do find the section already in the database, that is a duplicate and we do not allow duplicates.
                    if (duplicateSection.SectionNumber == section.SectionNumber)
                    {
                        throw new ArgumentException("That section already exists in the database.");
                    }
                }

                //Add the section to the database table, and the course join table, then save changes
                context.Sections.Add(section);
                tempCourse.Sections.Add(section);

                context.SaveChanges();
            }
        } // AddSection

        public static async Task<Section> GetSection(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            //Check if the term is null or empty
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

            //Check if the section number is null or empty.
            if (sectionNumber == null || sectionNumber == "")
            {
                throw new ArgumentException("The section number cannot be empty.");
            }

            //Format term to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;
                Section tempSection = null;
                
                //Try to find the semester specified.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Check if the semester is null.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under that semester and try to find the course specified.
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Check if course is null.
                if(tempCourse == null)
                {
                    throw new ArgumentException("The specified course does not exist in the database.");
                }

                //Load the sections under that course and try to find the section specified.
                context.Entry(tempCourse).Collection(course => course.Sections).Load();
                foreach (var section in tempCourse.Sections)
                {
                    if (section.SectionNumber == sectionNumber)
                    {
                        tempSection = section;
                        break;
                    }
                }

                //Check if section is null.
                if (tempSection == null)
                {
                    throw new ArgumentException("The specified section does not exist in the database.");
                }

                return tempSection;
            }
        } // GetSection

        public static async Task EditSection(string term, int year, string department, string courseNumber, string sectionNumber, Section NewValue)
        {
            //Check if the term is null or empty
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

            //Check if the section number is null or empty.
            if (sectionNumber == null || sectionNumber == "")
            {
                throw new ArgumentException("The section number cannot be empty.");
            }

            //Check if the new instructor EUID is null or empty.
            if (NewValue.InstructorEUID == null || NewValue.InstructorEUID == "")
            {
                throw new ArgumentException("The new instructor EUID cannot be empty.");
            }

            //NewValue.IsSectionCompleted does not need to be checked, because if a value is not given, then it defaults to 0.

            //Check if the new section number is null or empty.
            if (NewValue.SectionNumber == null || NewValue.SectionNumber == "")
            {
                throw new ArgumentException("The new section number cannot be empty.");
            }

            //Check if the new number of students is greater than 0.
            if (NewValue.NumberOfStudents < 1)
            {
                throw new ArgumentException("The new number of students cannot be zero.");
            }

            //Format term and new instructor EUID to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);
            //When formatting EUID I want to ask Ludi if an EUID will always be three letters followed by 4 numbers, or if we don't care to check formatting that specifically.
            NewValue.InstructorEUID = NewValue.InstructorEUID.ToLower();

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;
                Section tempSection = null;

                //Try to find the semester specified.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Check if the semester is null.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under that semester and try to find the course specified.
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Check if course is null.
                if (tempCourse == null)
                {
                    throw new ArgumentException("The specified course does not exist in the database.");
                }

                //Load the sections under that course and try to find the section specified.
                context.Entry(tempCourse).Collection(course => course.Sections).Load();

                //Try to find the new section information in the database.
                foreach (var section in tempCourse.Sections)
                {
                    //If we are trying to change a sections number to an already existing section number, that is a duplicate and we do not allow duplicates.                    
                    if(sectionNumber != NewValue.SectionNumber && section.SectionNumber == NewValue.SectionNumber)
                    {
                        throw new ArgumentException("The new section number already exists in the database.");
                    }

                    //Find the section specified to edit.
                    if (section.SectionNumber == sectionNumber)
                    {
                        tempSection = section;
                    }
                }

                //Check if section is null.
                if (tempSection == null)
                {
                    throw new ArgumentException("The specified section to edit does not exist in the database.");
                }

                tempSection.InstructorEUID = NewValue.InstructorEUID;
                tempSection.IsSectionCompleted = NewValue.IsSectionCompleted;
                tempSection.SectionNumber = NewValue.SectionNumber;
                tempSection.NumberOfStudents = NewValue.NumberOfStudents;

                context.SaveChanges();
            }
        } // EditSection

        public static async Task DeleteSection(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            //Check if the term is null or empty
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

            //Check if the section number is null or empty.
            if (sectionNumber == null || sectionNumber == "")
            {
                throw new ArgumentException("The section number cannot be empty.");
            }

            //Format term to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;
                Section tempSection = null;

                //Try to find the semester specified.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Check if the semester is null.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under that semester and try to find the course specified.
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Check if course is null.
                if (tempCourse == null)
                {
                    throw new ArgumentException("The specified course does not exist in the database.");
                }

                //Load the sections under that course and try to find the section specified.
                context.Entry(tempCourse).Collection(course => course.Sections).Load();
                foreach (var section in tempCourse.Sections)
                {
                    if (section.SectionNumber == sectionNumber)
                    {
                        tempSection = section;
                        break;
                    }
                }

                //Check if section is null.
                if (tempSection == null)
                {
                    throw new ArgumentException("The specified section does not exist in the database.");
                }

                context.Remove(tempSection);
                context.SaveChanges();
            }
        } // DeleteSection

        //This function will return a list of sections taught by that instructor
        public static async Task<List<AbetApi.Models.SectionInfo>> GetSectionsByInstructor(string term, int year, string instructorEUID)
        {
            //Find the semester
            //For each course, scan through their sections
            //for each section, validate if the instructor is teaching this course
            //if no, move on
            //if yes, build that model object

            //Check if the term is null or empty
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Format term to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;

                //Try to find the semester specified.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Check if the semester is null.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the courses under that semester and try to find the course specified.
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                //Load each section under each course
                foreach (var course in semester.Courses)
                {
                    context.Entry(course).Collection(course => course.Sections).Load();
                }

                //scan over all sections, looking for that instructor. If found, add it to the list
                List<AbetApi.Models.SectionInfo> sectionInfoList = new List<AbetApi.Models.SectionInfo>();
                foreach (var course in semester.Courses)
                {
                    foreach (var section in course.Sections)
                    {
                        if (section.InstructorEUID == instructorEUID)
                        {
                            sectionInfoList.Add(new AbetApi.Models.SectionInfo(course.DisplayName, course.CourseNumber, section.SectionNumber, section.InstructorEUID, course.CoordinatorEUID));
                        }
                    }
                }

                return sectionInfoList;
            }
        }
    } // Section
}
