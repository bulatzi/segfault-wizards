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

        public static void AddSection(string term, int year, string department, string courseNumber, Section section)
        {
            section.SectionId = 0;
            using(var context = new ABETDBContext())
            {
                //Find the semester/course the section will belong to
                Course tempCourse = null;
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.Department == department && course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Add the section to the database table, and the course join table, then save changes
                context.Sections.Add(section);
                tempCourse.Sections.Add(section);
                context.SaveChanges();
            }
        }

        public static Section GetSection(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            using (var context = new ABETDBContext())
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
                            if(section.SectionNumber == sectionNumber )
                            {
                                return section;
                            }
                        }
                        return null;
                    }
                }
                return null;
            }
        }

        public static void EditSection(string term, int year, string department, string courseNumber, string sectionNumber, Section NewValue)
        {
            using (var context = new ABETDBContext())
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
                            if (section.SectionNumber == sectionNumber)
                            {
                                section.InstructorEUID = NewValue.InstructorEUID;
                                section.IsSectionCompleted = NewValue.IsSectionCompleted;
                                section.SectionNumber = NewValue.SectionNumber;
                                section.NumberOfStudents = NewValue.NumberOfStudents;
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        public static void DeleteSection(string term, int year, string department, string courseNumber, string sectionNumber)
        {
            using (var context = new ABETDBContext())
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
                            if (section.SectionNumber == sectionNumber)
                            {
                                context.Remove(section);
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
