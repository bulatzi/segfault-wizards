using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.EntityFramework;
using AbetApi.EFModels;
using AbetApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AbetApi.Data
{
    // This class is a WIP. It will be used to house helper functions for interfacing with the database.
    public class Database
    {
        //This helper function clears all tables
        // If new tables are added, ensure they are made in to DbSet containers in the context object, and then add them to this function.
        public async static void WipeTables()
        {
            await using (var context = new ABETDBContext())
            {
                context.Courses.Clear();
                context.CourseOutcomes.Clear();
                context.Grades.Clear();
                context.Majors.Clear();
                context.MajorOutcomes.Clear();
                context.Sections.Clear();
                context.Semesters.Clear();
                context.Users.Clear();
                context.Roles.Clear();
                context.Surveys.Clear();
                context.StudentOutcomesCompleted.Clear();
                context.SaveChanges();
            }
        }

        public async void DropDatabase()
        {
            await using (var context = new ABETDBContext())
            {
                context.Database.EnsureDeleted();
            }
        }

        public void WIPDoStuff()
        {
            _= Semester.AddSemester(new Semester("Spring", 2022));
            _= Course.AddCourse("Spring", 2022, new Course("cas0231", "1030", "Something", "", false, "CSCE"));
            _= Course.AddCourse("Spring", 2022, new Course("cas0231", "1040", "Something", "", false, "CSCE"));
            _= Course.AddCourse("Spring", 2022, new Course("cas0231", "3600", "Whatever", "", false, "CSCE"));

            _= Section.AddSection("Spring", 2022, "CSCE", "1030", new Section("cas0231", false, "001", 12));
            _= Section.AddSection("Spring", 2022, "CSCE", "1030", new Section("cas0231", false, "002", 15));
            _= Section.AddSection("Spring", 2022, "CSCE", "1030", new Section("cas0231", false, "003", 29));

            _= Section.AddSection("Spring", 2022, "CSCE", "1040", new Section("cas0231", false, "001", 33));
            _= Section.AddSection("Spring", 2022, "CSCE", "1040", new Section("cas0231", false, "002", 13));
            _= Section.AddSection("Spring", 2022, "CSCE", "1040", new Section("cas0231", false, "003", 5));

            _= Section.AddSection("Spring", 2022, "CSCE", "3600", new Section("cas0231", false, "001", 150));
            _= Section.AddSection("Spring", 2022, "CSCE", "3600", new Section("cas0231", false, "002", 739));
            _= Section.AddSection("Spring", 2022, "CSCE", "3600", new Section("cas0231", false, "003", 42));

            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "1030", new CourseOutcome("1", "Some description 1"));
            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "1030", new CourseOutcome("2", "Some description 2"));
            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "1030", new CourseOutcome("3", "Some description 3"));

            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "1040", new CourseOutcome("1", "Some description 1"));
            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "1040", new CourseOutcome("2", "Some description 2"));
            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "1040", new CourseOutcome("3", "Some description 3"));

            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "3600", new CourseOutcome("1", "Some description 1"));
            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "3600", new CourseOutcome("2", "Some description 2"));
            _= CourseOutcome.CreateCourseOutcome("Spring", 2022, "CSCE", "3600", new CourseOutcome("3", "Some description 3"));

            _= Major.AddMajor("Spring", 2022, "CS");
            _= Major.AddMajor("Spring", 2022, "IT");

            _= MajorOutcome.AddMajorOutcome("Spring", 2022, "CS", new MajorOutcome("1", "Accomplishes gud at computers"));
            _= MajorOutcome.AddMajorOutcome("Spring", 2022, "CS", new MajorOutcome("2", "Accomplishes making type fast"));
            _= MajorOutcome.AddMajorOutcome("Spring", 2022, "CS", new MajorOutcome("3", "Accomplishes websites"));

            _= MajorOutcome.AddMajorOutcome("Spring", 2022, "IT", new MajorOutcome("1", "IT MO #1"));
            _= MajorOutcome.AddMajorOutcome("Spring", 2022, "IT", new MajorOutcome("2", "IT MO #2"));
            _= MajorOutcome.AddMajorOutcome("Spring", 2022, "IT", new MajorOutcome("3", "IT MO #3"));

            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1030", "1", "CS", "1");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1030", "1", "CS", "2");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1030", "2", "CS", "3");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1030", "3", "CS", "3");

            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1040", "1", "CS", "1");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1040", "1", "CS", "2");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1040", "2", "CS", "3");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1040", "3", "CS", "3");

            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "3600", "1", "CS", "1");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "3600", "2", "CS", "2");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1040", "2", "CS", "3");
            _= CourseOutcome.LinkToMajorOutcome("Spring", 2022, "CSCE", "1040", "3", "CS", "3");

            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "001", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "002", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "003", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "001", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "002", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "003", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "001", "3", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "002", "3", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "003", "3", "CS", 10);

            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "001", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "002", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "003", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "001", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "002", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "003", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "001", "3", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "002", "3", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "003", "3", "CS", 10);

            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "001", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "002", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "003", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "001", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "002", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "003", "2", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "001", "3", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "002", "3", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "003", "3", "CS", 10);

            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "001", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "002", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "003", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "001", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "002", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "003", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "001", "3", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "002", "3", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1030", "003", "3", "IT", 10);

            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "001", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "002", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "003", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "001", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "002", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "003", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "001", "3", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "002", "3", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "1040", "003", "3", "IT", 10);

            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "001", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "002", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "003", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "001", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "002", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "003", "2", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "001", "3", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "002", "3", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Spring", 2022, "CSCE", "3600", "003", "3", "IT", 10);

            //For testing the deep copy.
            //_ = Semester.AddSemester(new Semester("Fall", 2022));
            //Semester.DeepCopy("Fall", 2022, "Spring", 2022);
        }

        // This function is here to run arbitrary code from the database class
        // Currently, it's being used to test creating/editing data in the database
        public void DoStuff()
        {
            //Deleted all data from tables
            WipeTables();

            //Adds Users
            _= User.AddUser(new User("Frits", "Odell", "feo0589"));
            _= User.AddUser(new User("Clemente", "Sergei", "cas0231"));
            
            _= User.EditUser("feo0589", new User("Scrappy", "Eagle", "feo0589"));

            _= User.DeleteUser("cas0231");

            //NOTES: User Crud
            //User.AddUser(new User("Frits", "Odell", "cas0231"));
            //User.EditUser("cas0231", new User("Sponge", "Bob", "cas0231"));
            //User.DeleteUser("cas0231");
            //var result = User.GetUser("cas0231").Result;

            //Creates default roles
            _= Role.CreateRole(new Role("Admin"));
            _= Role.CreateRole(new Role("Coordinator"));
            _= Role.CreateRole(new Role("Instructor"));
            _= Role.CreateRole(new Role("Fellow"));
            _= Role.CreateRole(new Role("FullTime"));
            _= Role.CreateRole(new Role("Adjunct"));
            _= Role.CreateRole(new Role("Student"));

            //Gives admin access to:
            _= Role.AddRoleToUser("feo0589", "Admin");
            _= Role.AddRoleToUser("cas0231", "Admin");

            //Gives instructor access to:
            _= Role.AddRoleToUser("feo0589", "Instructor");
            _= Role.AddRoleToUser("cas0231", "Instructor");

            //this returns a list of users by the entered roll
            //var adminListTask = Role.GetUsersByRole("Admin");
            //var adminList = adminListTask.Result;

            //Role.RemoveRoleFromUser("cas0231", "Admin");

            //Semester class testing
            /////////////////////////////////////////////////////////////////////////////////
            //Create
            _= Semester.AddSemester(new Semester("Spring", 2022));
            _= Semester.AddSemester(new Semester("Fall", 2022));
            _= Semester.AddSemester(new Semester("Summer", 2022));
            _= Semester.AddSemester(new Semester("Spring", 2023));
            _= Semester.AddSemester(new Semester("Fall", 2023));
            _= Semester.AddSemester(new Semester("Summer", 2023));
            //Read
            var springSemester = Semester.GetSemester("Spring", 2022);
            var fallSemester = Semester.GetSemester("Fall", 2022); // This won't work. It returns null when it can't find something
            //Update
            _= Semester.EditSemester("Spring", 2022, new Semester("Winter", 2022));
            //Read (again)
            springSemester = Semester.GetSemester("Spring", 2022);
            fallSemester = Semester.GetSemester("Fall", 2022);
            //Delete
            //Semester.DeleteSemester("Fall", 2022);
            System.Console.WriteLine(""); //This is a placeholder for a debugger break point

            //Major class testing
            /////////////////////////////////////////////////////////////////////////////////
            _= Major.AddMajor("Fall", 2022, "CS");
            _= Major.AddMajor("Fall", 2022, "IT");
            _= Major.AddMajor("Fall", 2022, "CYS");
            //var temp = Major.GetMajor("Fall", 2022, "CS");
            //temp = Major.GetMajor("Fall", 2022, "CSE"); // return null
            //Major.EditMajor("Fall", 2022, "CS", new Major("IT"));
            //Major.DeleteMajor("Fall", 2022, "IT");
            System.Console.WriteLine("");

            //MajorOutcome class testing
            /////////////////////////////////////////////////////////////////////////////////
            _= MajorOutcome.AddMajorOutcome("Fall", 2022, "CS", new MajorOutcome("1", "Accomplishes gud at computers"));
            _= MajorOutcome.AddMajorOutcome("Fall", 2022, "CS", new MajorOutcome("2", "Accomplishes making type fast"));
            _= MajorOutcome.AddMajorOutcome("Fall", 2022, "CS", new MajorOutcome("3", "Accomplishes websites"));
            var tempMajorOutcome =  MajorOutcome.GetMajorOutcome("Fall", 2022, "CS", "2");
            _= MajorOutcome.EditMajorOutcome("Fall", 2022, "CS", "3", new MajorOutcome("3", "Gud at spelung"));
            //MajorOutcome.DeleteMajorOutcome("Fall", 2022, "CS", "3");

            System.Console.WriteLine("");

            //Course class testing
            /////////////////////////////////////////////////////////////////////////////////
            _= Course.AddCourse("Fall", 2022, new Course("cas0231", "3600", "Whatever", "", false, "CSCE"));
            _= Course.AddCourse("Fall", 2022, new Course("cas0231", "1040", "Something", "", false, "CSCE"));
            //var course = Course.GetCourse("Fall", 2022, "CSCE", "3600"); // This is an example of a get function that loads one of the lists
            //var tempSemester = Semester.GetSemester("Fall", 2022); // This is a dumb get function, that only returns exactly what you ask for without the lists

            //Course.EditCourse("Fall", 2022, "CSCE", "3600", new Course("cas0231", "2110", "It changed!", "", false, "CSCE"));
            //var course = Course.GetCourse("Fall", 2022, "CSCE", "2110");
            //course = Course.GetCourse("Fall", 2022, "BLEGH", "2110");
            //Course.DeleteCourse("Fall", 2022, "CSCE", "2110");

            //var temp = Semester.GetCourses("Fall", 2022);

            //Section class testing
            /////////////////////////////////////////////////////////////////////////////////
            _= Section.AddSection("Fall", 2022, "CSCE", "3600", new Section("cas0231", false, "001", 150));
            _= Section.AddSection("Fall", 2022, "CSCE", "3600", new Section("cas0231", false, "002", 739));
            _= Section.AddSection("Fall", 2022, "CSCE", "3600", new Section("cas0231", false, "003", 42));
            //var temp = Course.GetSections("Fall", 2022, "CSCE", "3600");
            //var temp = Section.GetSection("Fall", 2022, "CSCE", "3600", "001");
            _= Section.EditSection("Fall", 2022, "CSCE", "3600", "002", new Section("cas0231", true, "002", 140));
            _= Section.DeleteSection("Fall", 2022, "CSCE", "3600", "001");


            //CourseOutcome class testing
            /////////////////////////////////////////////////////////////////////////////////
            _= CourseOutcome.CreateCourseOutcome("Fall", 2022, "CSCE", "3600", new CourseOutcome("1", "Some description 1"));
            _= CourseOutcome.CreateCourseOutcome("Fall", 2022, "CSCE", "3600", new CourseOutcome("2", "Some description 2"));
            _= CourseOutcome.CreateCourseOutcome("Fall", 2022, "CSCE", "1040", new CourseOutcome("1", "Some description 1"));
            _= CourseOutcome.CreateCourseOutcome("Fall", 2022, "CSCE", "1040", new CourseOutcome("2", "Some description 2"));
            _= CourseOutcome.CreateCourseOutcome("Fall", 2022, "CSCE", "1040", new CourseOutcome("2", "Some different description 2")); // This overwrites the other 2 for this course

            //CourseOutcome.AddMajorOutcome("Fall", 2022, "CSCE", "3600", "CS", "739");// These next 3 will not work
            //CourseOutcome.AddMajorOutcome("Fall", 2022, "CSCE", "3600", "CS", "22");
            //CourseOutcome.AddMajorOutcome("Fall", 2022, "CSCE", "1040", "CS", "42");
            _= CourseOutcome.LinkToMajorOutcome("Fall", 2022, "CSCE", "1040","2", "CS", "1"); // This one should work
            _= CourseOutcome.LinkToMajorOutcome("Fall", 2022, "CSCE", "1040", "1", "CS", "2");
            _= CourseOutcome.LinkToMajorOutcome("Fall", 2022, "CSCE", "3600", "1", "CS", "1");
            _= CourseOutcome.LinkToMajorOutcome("Fall", 2022, "CSCE", "3600", "2", "CS", "2");

            System.Console.WriteLine("");

            //CourseOutcome.DeleteCourseOutcome("Fall", 2022, "CSCE", "1040", "2");

            //CourseOutcome.RemoveLinkToMajorOutcome("Fall", 2022, "CSCE", "1040", "2", "CS", "2");

            System.Console.WriteLine(""); //This is a placeholder for a debugger break point
            //Testing for section grades in the section class
            /////////////////////////////////////////////////////////////////////
            //Grade.SetSectionGrade("Fall", 2022, "CSCE", "3600", "002", new Grade())
            List<Grade> grades = new List<Grade>();
            grades.Add(new Grade("CSCE", 1, 2, 3, 4, 5, 6, 7, 666));
            grades.Add(new Grade("EENG", 8, 9, 10, 11, 12, 13, 14, 999));
            _= Grade.SetGrades("Fall", 2022, "CSCE", "3600", "002", grades);


            //Testing section for StudentOutcomesCompleted
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Fall", 2022, "CSCE", "3600", "002", "1", "CS", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Fall", 2022, "CSCE", "3600", "002", "2", "IT", 20);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Fall", 2022, "CSCE", "3600", "002", "1", "IT", 10);
            _= StudentOutcomesCompleted.SetStudentOutcomesCompleted("Fall", 2022, "CSCE", "3600", "002", "2", "CS", 20);

            var temp = StudentOutcomesCompleted.GetStudentOutcomesCompleted("Fall", 2022, "CSCE", "3600", "002");

            //var eh = CourseOutcome.MapCourseOutcomeToMajorOutcome("Fall", 2022, "CSCE", "1040", "3", "CS").Result;
            System.Console.WriteLine(""); //This is a placeholder for a debugger break point
        }
    }
}
