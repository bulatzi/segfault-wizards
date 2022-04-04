using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using AbetApi.Data;

namespace AbetApi.EFModels
{
    public class Grade
    {
        public int GradeId { get; set; }
        public string Major { get; set; }

        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int F { get; set; }
        public int W { get; set; }
        public int I { get; set; }
        public int TotalStudents { get; set; }

        Grade()
        {
            // Intentionally left blank
        }

        public Grade(string Major, int A, int B, int C, int D, int F, int W, int I, int TotalStudents)
        {
            this.Major = Major;
            this.A = A;
            this.B = B;
            this.C = C;
            this.D = D;
            this.F = F;
            this.W = W;
            this.I = I;
            this.TotalStudents = TotalStudents;
        }

        public static async Task SetGrades(string term, int year, string department, string courseNumber, string sectionNumber, List<Grade> grades)
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

            //Checks if the section number is null or empty.
            if (sectionNumber == null || sectionNumber == "")
            {
                throw new ArgumentException("The section number cannot be empty.");
            }

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;

                //Find the semester/course the section will belong to
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Check if the semester is null.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Finds the relevant course
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

                //Load the sections under the course specified.
                context.Entry(tempCourse).Collection(course => course.Sections).Load();

                //For each section
                foreach(var section in tempCourse.Sections)
                {
                    //If the section is found, create/overwrite the grades
                    if(section.SectionNumber == sectionNumber)
                    {
                        //Loads existing grades for that section
                        context.Entry(section).Collection(section => section.Grades).Load();

                        //If there are existing grades, overwrite them if applicable
                        if (section.Grades.Count > 0)
                        {
                            //Compare each grade to each existing grade. If it already exists, just replace it.
                            foreach (var i in section.Grades)
                            {
                                for (int j = 0; j < grades.Count; j++)
                                {
                                    if (i.Major == grades[j].Major)
                                    {
                                        //This copy is written out manually, to ensure we don't accidentally overwrite the primary key of the object in the database
                                        i.A = grades[j].A;
                                        i.B = grades[j].B;
                                        i.C = grades[j].C;
                                        i.D = grades[j].D;
                                        i.F = grades[j].F;
                                        i.W = grades[j].W;
                                        i.I = grades[j].I;
                                        i.TotalStudents = grades[j].TotalStudents;
                                        grades.RemoveAt(j);
                                    }
                                }
                            }
                        }

                        //Add the grades
                        foreach (var grade in grades)
                        {
                            context.Grades.Add(grade);
                            section.Grades.Add(grade);
                        }
                        context.SaveChanges();
                        return;
                    }
                }
            }
        } // AddGrade

        public static async Task<List<Grade>> GetGrades(string term, int year, string department, string courseNumber, string sectionNumber)
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

            //Checks if the section number is null or empty.
            if (sectionNumber == null || sectionNumber == "")
            {
                throw new ArgumentException("The section number cannot be empty.");
            }

            await using (var context = new ABETDBContext())
            {
                Course tempCourse = null;

                //Find the semester/course the section will belong to
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Check if the semester is null.
                if (semester == null)
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
                if (tempCourse == null)
                {
                    throw new ArgumentException("The specified course does not exist in the database.");
                }

                //Load the sections under the course specified.
                context.Entry(tempCourse).Collection(course => course.Sections).Load();

                //Add the section to the database table, and the course join table, then save changes
                //context.Sections.Add(section);
                //tempCourse.Sections.Add(section);

                //context.SaveChanges();

                foreach (var section in tempCourse.Sections)
                {
                    if (section.SectionNumber == sectionNumber)
                    {
                        context.Entry(section).Collection(section => section.Grades).Load();
                        return section.Grades.ToList();
                    }
                }
                return null;
            }
        } // AddGrade
    }
}
