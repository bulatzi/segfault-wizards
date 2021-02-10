using AbetApi.Models;
using AbetApi.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using static AbetApi.Models.AbetModels;

namespace AbetApi.Data
{

    public class MockAbetRepo : IMockAbetRepo
    {
        public Section[] sections = new Section[6];
        public Form[] forms = new Form[5];
        public Admin[] admins = new Admin[3];
        public Instructor[] instructors = new Instructor[4];
        public Coordinator[] coordinators = new Coordinator[4];

        //Generate the fake data
        public MockAbetRepo()
        {
            // ********* Fake Section Data *********
            Instructor instructor6 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator6 = new Coordinator("Mark", "Thompson", "MT2020");
            Section section6 = new Section(instructor6, coordinator6, false, 1, 49, 3600, "Intro to System Programming", false, "", "CSCE");

            Instructor instructor7 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator7 = new Coordinator("David", "Keathly", "DK2121");
            Section section7 = new Section(instructor7, coordinator7, false, 1, 90, 1030, "Computer Science I", false, "", "CSCE");

            Instructor instructor8 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator8 = new Coordinator("David", "Keathly", "DK2121");
            Section section8 = new Section(instructor8, coordinator8, false, 2, 96, 1030, "Computer Science I", true, "Good work!", "CSCE");

            Instructor instructor9 = new Instructor("Yan", "Huang", "YH2221");
            Coordinator coordinator9 = new Coordinator("Krishna", "Kavi", "KK2521");
            Section section9 = new Section(instructor9, coordinator9, false, 1, 51, 2610, "Assembly Langauge And Computer Organization", true, "Just disgraceful.", "CSCE");

            Instructor instructor10 = new Instructor("Wei", "Jin", "WJ2135");
            Coordinator coordinator10 = new Coordinator("Krishna", "Kavi", "KK2521");
            Section section10 = new Section(instructor10, coordinator10, false, 7, 37, 2610, "Assembly Langauge And Computer Organization", false, "", "CSCE");

            Instructor instructor11 = new Instructor("Stephanie", "Ludi", "SL2121");
            Coordinator coordinator11 = new Coordinator("Stephanie", "Ludi", "SL2121");
            Section section11 = new Section(instructor11, coordinator11, false, 1, 72, 4901, "Software Development Capstone", true, "Nice job!", "CSCE");

            // ********* Fake Form Data *********
            Instructor instructor1 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator1 = new Coordinator("Mark", "Thompson", "MT2020");
            Section section1 = new Section(instructor1, coordinator1, false, 1, 0, 3600, "Intro to System Programming", false, "", "CSCE");
            StudentWorks studentwork1 = new StudentWorks("StudentWork1", "file1");
            StudentWorks studentwork2 = new StudentWorks("StudentWork2", "file2");
            List<StudentWorks> listOfWorks1 = new List<StudentWorks>();
            listOfWorks1.Add(studentwork1);
            listOfWorks1.Add(studentwork2);
            Grades gradeIT = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section1.NumberOfStudents = gradeIT.TotalStudents + gradeCS.TotalStudents + gradeCE.TotalStudents;
            Outcomes outcome1 = new Outcomes("Outcome1: this is outcome 1", 20, 30, 40, listOfWorks1);
            Outcomes outcome11 = new Outcomes("Outcome2: this is outcome 2", 10, 20, 10, listOfWorks1);
            List<Outcomes> outcomes1 = new List<Outcomes> { outcome1, outcome11 };
            section1.Year = 2020;
            section1.Semester = "fall";
            Form instructorForm1 = new Form(section1, outcomes1, gradeIT, gradeCS, gradeCE);

            Instructor instructor2 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator2 = new Coordinator("David", "Keathly", "DK2121");
            Section section2 = new Section(instructor2, coordinator2, false, 1, 0, 1030, "Computer Science I", false, "", "CSCE");
            Grades gradeIT1 = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS1 = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE1 = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section2.NumberOfStudents = gradeIT1.TotalStudents + gradeCS1.TotalStudents + gradeCE1.TotalStudents;
            Outcomes outcome2 = new Outcomes("Outcome1: this is outcome 1", 20, 30, 40, listOfWorks1);
            Outcomes outcome22 = new Outcomes("Outcome2: this is outcome 2", 10, 10, 40, listOfWorks1);
            List<Outcomes> outcomes2 = new List<Outcomes> { outcome2, outcome22 };
            section2.Year = 2020;
            section2.Semester = "fall";
            Form instructorForm2 = new Form(section2, outcomes2, gradeIT1, gradeCS1, gradeCE1);

            Instructor instructor3 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator3 = new Coordinator("David", "Keathly", "DK2121");
            Section section3 = new Section(instructor3, coordinator3, false, 2, 0, 1030, "Computer Science I", true, "Good work!", "CSCE");
            Grades gradeIT2 = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS2 = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE2 = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section3.NumberOfStudents = gradeIT2.TotalStudents + gradeCS2.TotalStudents + gradeCE2.TotalStudents;
            Outcomes outcome3 = new Outcomes("Outcome1: this is outcome 1", 20, 30, 40, listOfWorks1);
            Outcomes outcome33 = new Outcomes("Outcome2: this is outcome 2", 20, 30, 40, listOfWorks1);
            List<Outcomes> outcomes3 = new List<Outcomes> { outcome3, outcome33 };
            section3.Year = 2020;
            section3.Semester = "fall";
            Form instructorForm3 = new Form(section3, outcomes3, gradeIT2, gradeCS2, gradeCE2);

            Instructor instructor4 = new Instructor("Yan", "Huang", "YH2221");
            Coordinator coordinator4 = new Coordinator("Krishna", "Kavi", "KK2521");
            Section section4 = new Section(instructor4, coordinator4, false, 1, 0, 2610, "Assembly Langauge And Computer Organization", true, "Just disgraceful.", "CSCE");
            Grades gradeIT3 = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS3 = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE3 = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section4.NumberOfStudents = gradeIT3.TotalStudents + gradeCS3.TotalStudents + gradeCE3.TotalStudents;
            Outcomes outcome4 = new Outcomes("Outcome1: this is outcome 1", 20, 30, 40, listOfWorks1);
            Outcomes outcome44 = new Outcomes("Outcome2: this is outcome 2", 10, 10, 40, listOfWorks1);
            List<Outcomes> outcomes4 = new List<Outcomes> { outcome4, outcome44 };
            section4.Year = 2020;
            section4.Semester = "fall";
            Form instructorForm4 = new Form(section4, outcomes4, gradeIT3, gradeCS3, gradeCE3);

            Instructor instructor5 = new Instructor("Yan", "Huang", "YH2221");
            Coordinator coordinator5 = new Coordinator("Krishna", "Kavi", "KK2521");
            Section section5 = new Section(instructor5, coordinator5, false, 1, 0, 2610, "Assembly Langauge And Computer Organization", true, "Just disgraceful.", "CSCE");
            Grades gradeIT4 = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS4 = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE4 = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section5.NumberOfStudents = gradeIT4.TotalStudents + gradeCS4.TotalStudents + gradeCE4.TotalStudents;
            Outcomes outcome5 = new Outcomes("Outcome1: this is outcome 1", 20, 30, 50, listOfWorks1);
            Outcomes outcome55 = new Outcomes("Outcome2: this is outcome 2", 10, 10, 40, listOfWorks1);
            List<Outcomes> outcomes5 = new List<Outcomes> { outcome5, outcome55 };
            section5.Year = 2020;
            section5.Semester = "fall";
            Form instructorForm5 = new Form(section5, outcomes5, gradeIT4, gradeCS4, gradeCE4);

            Admin admin1 = new Admin("Indiana", "Jones", "IJ1981");
            Admin admin2 = new Admin("Darth", "Vader", "DV1977");
            Admin admin3 = new Admin("Harry", "Potter", "HP2001");

            section6.Year = 2020;
            section6.Semester = "fall";
            section7.Year = 2020;
            section7.Semester = "fall";
            section8.Year = 2020;
            section8.Semester = "fall";
            section9.Year = 2020;
            section9.Semester = "fall";
            section10.Year = 2020;
            section10.Semester = "fall";

            forms = new Form[] { instructorForm1, instructorForm2, instructorForm3, instructorForm4, instructorForm5 };
            sections = new Section[] { section6, section7, section8, section9, section10, section11 };
            admins = new Admin[] { admin1, admin2, admin3 };
            instructors = new Instructor[] { instructor1, instructor4, instructor10, instructor11 };
            coordinators = new Coordinator[] { coordinator1, coordinator2, coordinator4, coordinator11 };
        }

        public IEnumerable<AbetModels.Section> GetSectionsByUserId(string userId, int year, string semester)
        {
            List<Section> toReturn = new List<Section>();

            foreach (Section s in sections)
            {
                if (String.Equals(s.Instructor.Id, userId) || String.Equals(s.Coordinator.Id, userId)) toReturn.Add(s);
            }

            return toReturn;
        }

        public IEnumerable<AbetModels.Section> GetSectionsByYearAndSemester(int year, string semester)
        {
            List<Section> toReturn = new List<Section>();

            foreach (Section s in sections)
            {
                if (String.Equals(s.Year, year) && String.Equals(s.Semester, semester)) toReturn.Add(s);
            }

            return toReturn;
        }

        public Form GetFormBySection(Section section)
        {
            foreach (Form i in forms)
            {
                if (String.Equals(i.Section.CourseNumber, section.CourseNumber) && String.Equals(i.Section.SectionNumber, section.SectionNumber))
                    return i;
            }

            return null;
        }

        public IEnumerable<Form> GetFormsByCourse(Course course)
        {
            List<Form> toReturn = new List<Form>();

            foreach (Form i in forms)
            {
                if (String.Equals(i.Section.CourseNumber, course.CourseNumber)) toReturn.Add(i);
            }

            return toReturn;
        }

        public IEnumerable<Form> GetFormsByYearAndSemester(int year, string semester)
        {
            List<Form> toReturn = new List<Form>();

            foreach (Form i in forms)
            {
                if (String.Equals(i.Section.Year, year) && String.Equals(i.Section.Semester, semester)) toReturn.Add(i);
            }

            return toReturn;
        }

        public Form GetBlankForm()
        {
            Form toReturn = new Form();
            toReturn.Section = new Section();
            toReturn.CEGrades = new Grades(0, 0, 0, 0, 0, 0, 0, 0);
            toReturn.CSGrades = new Grades(0, 0, 0, 0, 0, 0, 0, 0);
            toReturn.ITGrades = new Grades(0, 0, 0, 0, 0, 0, 0, 0);
            toReturn.Section.Instructor = new Instructor();
            toReturn.Section.Coordinator = new Coordinator();

            return toReturn;
        }

        public bool PostForm(Form form)
        {
            //store form in database
            return true;
        }

        public bool PostComment(Course course)
        {
            //course conatins all of the necessary data to make the SQL query

            //store comment in database
            return true;
        }

        public bool PostSection(Section section)
        {
            //store section in database
            return true;
        }

        public string GetRole(string userId)
        {
            //call to DB to get the role associated with the userId
            return "Admin";
        }

        public FacultyList GetFacultyList()
        {
            //call to DB to get all entries in staff table
            FacultyList facultyList = new FacultyList();

            //add all normal faculty, there are no adjuncts or teaching fellows for now
            foreach (Instructor instructor in instructors)
                facultyList.Normal.Add(instructor);
            
            foreach (Coordinator coordinator in coordinators)
                if (!facultyList.Normal.Contains(coordinator))
                    facultyList.Normal.Add(coordinator);

            foreach (Admin admin in admins)
                if (!facultyList.Normal.Contains(admin))
                    facultyList.Normal.Add(admin);

            return facultyList;
        }

        public bool AddFacultyMember(Info info, string role)
        {
            //store member in DB
            return true;
        }
    }
}
