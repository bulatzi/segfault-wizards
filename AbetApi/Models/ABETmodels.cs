using System;
using System.Collections.Generic;
using System.Linq;

namespace AbetApi.Models
{
    public class AbetModels
    {
        public class Info
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Id { get; set; }
        }

        public class Instructor : Info
        {
            public Instructor(string firstName, string lastName, string id)
            {
                this.FirstName = firstName;
                this.LastName = lastName;
                this.Id = id;
            }

            public Instructor()
            { }
        }

        public class Coordinator : Info
        {
            public Coordinator(string firstName, string lastName, string id)
            {
                this.FirstName = firstName;
                this.LastName = lastName;
                this.Id = id;
            }

            public Coordinator()
            { }
        }

        public class Admin : Info
        {
            public Admin(string firstName, string lastName, string id)
            {
                this.FirstName = firstName;
                this.LastName = lastName;
                this.Id = id;
            }

            public Admin()
            { }
        }

        public class Course
        {
            public Coordinator Coordinator { get; set; }
            public int CourseNumber { get; set; } //Ex: 2100
            public string DisplayName { get; set; } //Ex: "Assembly Langauge And Computer Organization"
            public string CoordinatorComment { get; set; }
            public bool IsCourseCompleted { get; set; }
            public string Department { get; set; } //Ex. "CSCE"
            public string Semester { get; set; }
            public int Year { get; set; }
        }

        public class Section : Course
        {
            public Instructor Instructor { get; set; }
            public bool IsSectionCompleted { get; set; }
            public int SectionNumber { get; set; } //Ex: 1
            public int NumberOfStudents { get; set; }

            public Section(Instructor instructor, Coordinator coordinator, bool sectionCompleted, int sectionNumber, int numberOfStudents, int courseNumber, string displayName, bool courseCompleted, string coordinatorComment, string department)
            {
                this.Instructor = instructor;
                this.Coordinator = coordinator;
                this.IsSectionCompleted = sectionCompleted;
                this.SectionNumber = sectionNumber;
                this.NumberOfStudents = numberOfStudents;
                this.CourseNumber = courseNumber;
                this.DisplayName = displayName;
                this.IsCourseCompleted = courseCompleted;
                this.CoordinatorComment = coordinatorComment;
                this.Department = department;
            }

            public Section()
            { }
        }

        public class BodyParams
        {
            public string UserId { get; set; }
            public string Password { get; set; }
            public int Year { get; set; }
            public string Semester { get; set; }
            public Form Form { get; set; }
            public Section Section { get; set; }
            public Course Course { get; set; }
            public Info Info { get; set; }
            public string Role { get; set; }
            public string Program { get; set; }
        }

        public class Form
        {
            public Section Section { get; set; }
            public List<Course_Outcomes> Outcomes { get; set; }  // array 
            public Grades ITGrades { get; set; }
            public Grades CSGrades { get; set; }
            public Grades CEGrades { get; set; }

            public Form(Section section, List<Course_Outcomes> outcome, Grades itgrade, Grades csgrade, Grades cegrade)
            {
                this.Section = section;
                this.Outcomes = outcome;
                this.ITGrades = itgrade;
                this.CSGrades = csgrade;
                this.CEGrades = cegrade;
            }

            public Form()
            { }
        }

        public class Course_Outcomes
        {
            public string Outcome { get; set; }
            public int NumberOfIT { get; set; }
            public int NumberOfCS { get; set; }
            public int NumberOfCE { get; set; }
            public List<StudentWorks> StudentWorks { get; set; }

            public Course_Outcomes(string outcome, int numberofIT, int numberofCS, int numberofCE, List<StudentWorks> studentworks)
            {
                this.Outcome = outcome;
                this.NumberOfIT = numberofIT;
                this.NumberOfCS = numberofCS;
                this.NumberOfCE = numberofCE;
                this.StudentWorks = studentworks;
            }
        }

        public class Student_Outcomes
        {
            public int Number { get; set; }
            public string Outcome { get; set; }
            public Student_Outcomes(int number, string outcome)
            {
                this.Number = number;
                this.Outcome = outcome;
            }
        }

        public class Course_Objective
        {
            public string Course_Name { get; set; }
            public string Course_Outcome { get; set; }
            public List<int> Student_Outcome_mapping { get; set; }
        }

        public class Course_Objectives
        {
            public string program { get; set; }
            public List<Student_Outcomes> student_Outcomes;
            public List<Course_Objective> course_Objectives;
        }
        public class StudentWorks
        {
            public string StudentWork { get; set; }
            public string FileUploaded { get; set; }
            public StudentWorks(string studentWork, string fileUploaded)
            {
                this.StudentWork = studentWork;
                this.FileUploaded = fileUploaded;
            }
        }

        public class Grades
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
            public int D { get; set; }
            public int F { get; set; }
            public int W { get; set; }
            public int I { get; set; }
            public int TotalStudents { get; set; }

            public Grades(int a, int b, int c, int d, int f, int w, int i, int totalStudents)
            {
                this.A = a;
                this.B = b;
                this.C = c;
                this.D = d;
                this.F = f;
                this.W = w;
                this.I = i;
                this.TotalStudents = totalStudents;
            }
        }

        public class FacultyList
        {
            public List<Info> Normal { get; set; } = new List<Info>();   //instructors, coordinators and admins
            public List<Info> Adjuncts { get; set; } = new List<Info>(); //teaching adjuncts
            public List<Info> Fellows { get; set; } = new List<Info>();  //teaching fellows
        }
    }
}

/*
 *  public class Section
    {
        public string Department { get; set; }
        public string Course_Name { get; set; }
        public int Course_Number { get; set; }
        public int Section_Number { get; set; }
        public string Instructor_Name { get; set; }
        public string Coordinator_Name { get; set; }
        public int Number_Of_Students { get; set; }
    }
 * 
 * 
 * //public Dictionary<string,string> CourseOutcomes { get; set; }
 */
