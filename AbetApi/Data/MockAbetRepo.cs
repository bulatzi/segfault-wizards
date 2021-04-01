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
        public List<Course> courses = new List<Course>();
        public Form[] forms = new Form[5];
        public Admin[] admins = new Admin[3];
        public Instructor[] instructors = new Instructor[4];
        public Coordinator[] coordinators = new Coordinator[4];
        public List<CourseMapping> outcomes = new List<CourseMapping>();
        List<Course_Objective> c_objs = new List<Course_Objective>();
        List<Student_Outcome> s_outcomes = new List<Student_Outcome>();                                                                                                                                     

        //Generate the fake data
        public MockAbetRepo()
        {
            // ********* Fake Section Data *********
            Instructor instructor6 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator6 = new Coordinator("Mark", "Thompson", "MT2020");
            Section section6 = new Section(instructor6, coordinator6, false, "001", 49, "3600", "Intro to System Programming", false, "", "CSCE");
            Course course1 = new Course(coordinator6, "3600", "Intro to System Programming", "Very good!", true, "CSCE", "Spring", 2020);

            Instructor instructor7 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator7 = new Coordinator("David", "Keathly", "DK2121");
            Section section7 = new Section(instructor7, coordinator7, false, "001", 90, "1030", "Computer Science I", false, "", "CSCE");
            Course course2 = new Course(coordinator7, "1030", "Computer Science I", "Just terrible.", true, "CSCE", "Spring", 2020);

            Instructor instructor8 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator8 = new Coordinator("David", "Keathly", "DK2121");
            Section section8 = new Section(instructor8, coordinator8, false, "002", 96, "1030", "Computer Science I", true, "Good work!", "CSCE");

            Instructor instructor9 = new Instructor("Yan", "Huang", "YH2221");
            Coordinator coordinator9 = new Coordinator("Krishna", "Kavi", "KK2521");
            Section section9 = new Section(instructor9, coordinator9, false, "001", 51, "2610", "Assembly Langauge And Computer Organization", true, "Just disgraceful.", "CSCE");
            Course course3 = new Course(coordinator9, "2610", "Assembly Langauge And Computer Organization", "", false, "CSCE", "Spring", 2020);

            Instructor instructor10 = new Instructor("Wei", "Jin", "WJ2135");
            Coordinator coordinator10 = new Coordinator("Krishna", "Kavi", "KK2521");
            Section section10 = new Section(instructor10, coordinator10, false, "007", 37, "2610", "Assembly Langauge And Computer Organization", false, "", "CSCE");

            Instructor instructor11 = new Instructor("Stephanie", "Ludi", "SL2121");
            Coordinator coordinator11 = new Coordinator("Stephanie", "Ludi", "SL2121");
            Section section11 = new Section(instructor11, coordinator11, false, "001", 72, "4901", "Software Development Capstone", true, "Nice job!", "CSCE");

            // ********* Fake Form Data *********
            Instructor instructor1 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator1 = new Coordinator("Mark", "Thompson", "MT2020");
            Section section1 = new Section(instructor1, coordinator1, false, "001", 0, "3600", "Intro to System Programming", false, "", "CSCE");
            StudentWork studentwork1 = new StudentWork("StudentWork1", "file1");
            StudentWork studentwork2 = new StudentWork("StudentWork2", "file2");
            List<StudentWork> listOfWorks1 = new List<StudentWork>();
            listOfWorks1.Add(studentwork1);
            listOfWorks1.Add(studentwork2);
            Grades gradeIT = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section1.NumberOfStudents = gradeIT.TotalStudents + gradeCS.TotalStudents + gradeCE.TotalStudents;
            OutcomeObjective outcome1 = new OutcomeObjective("Outcome1: this is outcome 1", 20, 30, 40, listOfWorks1);
            OutcomeObjective outcome11 = new OutcomeObjective("Outcome2: this is outcome 2", 10, 20, 10, listOfWorks1);
            List<OutcomeObjective> outcomes1 = new List<OutcomeObjective> { outcome1, outcome11 };
            section1.Year = 2020;
            section1.Semester = "fall";
            Form instructorForm1 = new Form(section1, outcomes1, gradeIT, gradeCS, gradeCE);

            Instructor instructor2 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator2 = new Coordinator("David", "Keathly", "DK2121");
            Section section2 = new Section(instructor2, coordinator2, false, "001", 0, "1030", "Computer Science I", false, "", "CSCE");
            Grades gradeIT1 = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS1 = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE1 = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section2.NumberOfStudents = gradeIT1.TotalStudents + gradeCS1.TotalStudents + gradeCE1.TotalStudents;
            OutcomeObjective outcome2 = new OutcomeObjective("Outcome1: this is outcome 1", 20, 30, 40, listOfWorks1);
            OutcomeObjective outcome22 = new OutcomeObjective("Outcome2: this is outcome 2", 10, 10, 40, listOfWorks1);
            List<OutcomeObjective> outcomes2 = new List<OutcomeObjective> { outcome2, outcome22 };
            section2.Year = 2020;
            section2.Semester = "fall";
            Form instructorForm2 = new Form(section2, outcomes2, gradeIT1, gradeCS1, gradeCE1);

            Instructor instructor3 = new Instructor("Mark", "Thompson", "MT2020");
            Coordinator coordinator3 = new Coordinator("David", "Keathly", "DK2121");
            Section section3 = new Section(instructor3, coordinator3, false, "002", 0, "1030", "Computer Science I", true, "Good work!", "CSCE");
            Grades gradeIT2 = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS2 = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE2 = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section3.NumberOfStudents = gradeIT2.TotalStudents + gradeCS2.TotalStudents + gradeCE2.TotalStudents;
            OutcomeObjective outcome3 = new OutcomeObjective("Outcome1: this is outcome 1", 20, 30, 40, listOfWorks1);
            OutcomeObjective outcome33 = new OutcomeObjective("Outcome2: this is outcome 2", 20, 30, 40, listOfWorks1);
            List<OutcomeObjective> outcomes3 = new List<OutcomeObjective> { outcome3, outcome33 };
            section3.Year = 2020;
            section3.Semester = "fall";
            Form instructorForm3 = new Form(section3, outcomes3, gradeIT2, gradeCS2, gradeCE2);

            Instructor instructor4 = new Instructor("Yan", "Huang", "YH2221");
            Coordinator coordinator4 = new Coordinator("Krishna", "Kavi", "KK2521");
            Section section4 = new Section(instructor4, coordinator4, false, "001", 0, "2610", "Assembly Langauge And Computer Organization", true, "Just disgraceful.", "CSCE");
            Grades gradeIT3 = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS3 = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE3 = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section4.NumberOfStudents = gradeIT3.TotalStudents + gradeCS3.TotalStudents + gradeCE3.TotalStudents;
            OutcomeObjective outcome4 = new OutcomeObjective("Outcome1: this is outcome 1", 20, 30, 40, listOfWorks1);
            OutcomeObjective outcome44 = new OutcomeObjective("Outcome2: this is outcome 2", 10, 10, 40, listOfWorks1);
            List<OutcomeObjective> outcomes4 = new List<OutcomeObjective> { outcome4, outcome44 };
            section4.Year = 2020;
            section4.Semester = "fall";
            Form instructorForm4 = new Form(section4, outcomes4, gradeIT3, gradeCS3, gradeCE3);

            Instructor instructor5 = new Instructor("Yan", "Huang", "YH2221");
            Coordinator coordinator5 = new Coordinator("Krishna", "Kavi", "KK2521");
            Section section5 = new Section(instructor5, coordinator5, false, "001", 0, "2610", "Assembly Langauge And Computer Organization", true, "Just disgraceful.", "CSCE");
            Grades gradeIT4 = new Grades(2, 5, 6, 7, 1, 1, 9, 31);
            Grades gradeCS4 = new Grades(2, 5, 6, 7, 1, 2, 9, 32);
            Grades gradeCE4 = new Grades(2, 5, 6, 7, 1, 3, 9, 33);
            section5.NumberOfStudents = gradeIT4.TotalStudents + gradeCS4.TotalStudents + gradeCE4.TotalStudents;
            OutcomeObjective outcome5 = new OutcomeObjective("Outcome1: this is outcome 1", 20, 30, 50, listOfWorks1);
            OutcomeObjective outcome55 = new OutcomeObjective("Outcome2: this is outcome 2", 10, 10, 40, listOfWorks1);
            List<OutcomeObjective> outcomes5 = new List<OutcomeObjective> { outcome5, outcome55 };
            section5.Year = 2020;
            section5.Semester = "fall";
            Form instructorForm5 = new Form(section5, outcomes5, gradeIT4, gradeCS4, gradeCE4);

            Admin admin1 = new Admin("Indiana", "Jones", "IJ1981");
            Admin admin2 = new Admin("Darth", "Vader", "DV1977");
            Admin admin3 = new Admin("Harry", "Potter", "HP2001");

            //course and student outcome sample data
            int[] mapped1 = { 0, 0, 0, 0, 0, 0 };
            int[] mapped2 = { 0, 0, 0, 0, 0, 0, 0 };
            CourseMapping c_outcome1 = new CourseMapping(1, "Describe how a computer's CPU, Main Memory, Secondary Storage and " +
                "I/O work together to execute a computer program.", mapped1);
            CourseMapping c_outcome2 = new CourseMapping(2, "Make use of a computer system's hardware, editor(s), operating system, system " +
                "software and network to build computer software and submit that software for grading.", mapped1);
            CourseMapping c_outcome3 = new CourseMapping(3, "Describe algorithms to perform simple tasks such as numeric computation, " +
                "searching and sorting, choosing among several options, string manipulation, and use of pseudo-random numbers in simulation " +
                "of such tasks as rolling dice.", mapped1);
            CourseMapping c_outcome4 = new CourseMapping(4, "Write readable, efficient and correct C/C++ programs that include " +
                "programming structures such as assignment statements, selection statements, loops, arrays, pointers, " +
                "console and file I/O, structures, command line arguments, both standard library and user-defined functions, " +
                "and multiple header (.h) and code (.c) files.", mapped1);
            CourseMapping c_outcome5 = new CourseMapping(5, "Use commonly accepted practices and tools to find and fix runtime " +
                "and logical errors in software.", mapped1);
            CourseMapping c_outcome6 = new CourseMapping(6, "Describe a software process model that can be used to develop significant " +
                "applications composed of hundreds of functions.", mapped1);
            CourseMapping c_outcome7 = new CourseMapping(7, "Perform the steps necessary to edit, compile, link and execute C/C++ programs.", mapped1);

            Course_Objective c_obj = new Course_Objective("CSCE 1030 Computer Science I", outcomes);

            Student_Outcome s_outcome1 = new Student_Outcome(1, "Analyze a complex computing problem and to apply principles of computing " +
                "and other relevant disciplines to identify solutions.");
            Student_Outcome s_outcome2 = new Student_Outcome(2, "Design, implement, and evaluate a computing-based solution to meet a given set of " +
                "computing requirements in the context of the program’s discipline.");
            Student_Outcome s_outcome3 = new Student_Outcome(3, "Communicate effectively in a variety of professional contexts.");
            Student_Outcome s_outcome4 = new Student_Outcome(4, "Recognize professional responsibilities and make informed judgements in " +
                "computing practice based on legal and ethical principles.");
            Student_Outcome s_outcome5 = new Student_Outcome(5, "Function effectively as a member or leader of a team engaged in " +
                "activities appropriate to the program’s discipline.");
            Student_Outcome s_outcome6 = new Student_Outcome(6, "Apply computer science theory and software development fundamentals to produce computing-based solutions.");

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
            courses = new List<Course> { course1, course2, course3 };
            outcomes = new List<CourseMapping> { c_outcome1, c_outcome2, c_outcome3, c_outcome4, c_outcome5, c_outcome6, c_outcome7 };
            s_outcomes = new List<Student_Outcome> { s_outcome1, s_outcome2, s_outcome3, s_outcome4, s_outcome5, s_outcome6 };
            c_objs = new List<Course_Objective> { c_obj };
        }

        public List<AbetModels.Section> GetSectionsByUserId(string userId, int year, string semester)
        {
            List<Section> toReturn = new List<Section>();

            foreach (Section s in sections)
            {
                if (String.Equals(s.Instructor.Id, userId) || String.Equals(s.Coordinator.Id, userId)) toReturn.Add(s);
            }

            return toReturn;
        }

        public List<AbetModels.Section> GetSectionsByYearAndSemester(int year, string semester)
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
            try
            {
                foreach (Form i in forms)
                {
                    if (String.Equals(i.Section.CourseNumber, section.CourseNumber) && String.Equals(i.Section.SectionNumber, section.SectionNumber))
                        return i;
                }
            }
            catch
            {
                return null;
            }
            
            return null;
        }

        public List<Form> GetFormsByCourse(Course course)
        {
            List<Form> toReturn = new List<Form>();

            foreach (Form i in forms)
            {
                if (String.Equals(i.Section.CourseNumber, course.CourseNumber)) toReturn.Add(i);
            }

            return toReturn;
        }

        public List<Form> GetFormsByYearAndSemester(int year, string semester)
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

        public bool AddSection(Section section)
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
                facultyList.FullTime.Add(instructor);
            
            foreach (Coordinator coordinator in coordinators)
                if (!facultyList.FullTime.Contains(coordinator))
                    facultyList.FullTime.Add(coordinator);

            foreach (Admin admin in admins)
                if (!facultyList.FullTime.Contains(admin))
                    facultyList.FullTime.Add(admin);

            return facultyList;
        }

        public bool AddFacultyMember(Info info, string facultyType)
        {
            //store member in DB
            return true;
        }

        public Program_Outcomes GetCourseObjectives(string program)
        {
            Program_Outcomes p_outcome = new Program_Outcomes("Computer Science", c_objs, s_outcomes);

            return p_outcome;
        }

        public List<Course> GetCoursesByDepartment(string department)
        {
            List<Course> courseList = new List<Course>();

            foreach (Course course in courses)
                if (course.Department == department && !courseList.Contains(course))
                    courseList.Add(course);

            return courseList;
        }

        public bool AddCourse(Course course)
        {
            courses.Add(course);
            return true;
        }

        public bool RemoveCourse(Course course)
        {
            courses.Remove(course);
            return true;
        }

        public bool PostCourseOutcomes(List<CourseOutcome> courseOutcomesList)
        {
            //foreach (CourseMapping outcome in courseOutcomesList)
              //  outcomes.Add(outcome);

            return true;
        }

        public List<CourseMapping> GetCourseOutcomesByCourse(Course course)
        {
            return outcomes;
        }

        public List<Course> GetCoursesByYear(int year, string semester)
        {

            Coordinator coordinator11 = new Coordinator("Mark", "Thompson", "MT2020");
            Coordinator coordinator12 = new Coordinator("David", "Keathly", "DK2121");

            Course course11 = new Course(coordinator11, "1030", "Computer Science I", "Just Terrible!", true, "CSCE", semester, year);
            Course course12 = new Course(coordinator12, "3600", "Intro to System Programming", "Very good!", true, "CSCE", semester, year);

            List<Course> courseList = new List<Course> { course11, course12};

            return courseList;
        }

        public bool PostSection(Section section)
        {
            return true;
        }
    }
}
