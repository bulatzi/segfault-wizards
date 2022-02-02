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

        // This function is here to run arbitrary code from the database class
        // Currently, it's being used to test creating/editing data in the database
        public void DoStuff()
        {
            //Deleted all data from tables
            WipeTables();

            //Adds Users
            User.AddUser(new User("Bryan", "Morris", "bdm0121"));
            User.AddUser(new User("Sean", "Boden", "slb0589"));
            User.AddUser(new User("Stephen", "Bishop", "scb0231"));
            User.AddUser(new User("Alex", "Lambert", "apl0075"));
            User.AddUser(new User("Chet", "Lockwood", "cgl0021"));

            User.EditUser("cgl0021", new User("Scrappy", "Eagle", "cgl0021"));

            User.DeleteUser("scb0231");

            //NOTES: User Crud
            //User.AddUser(new User("Bryan", "Morris", "bdm0121"));
            //User.EditUser("bdm0121", new User("Sponge", "Bob", "bdm0121"));
            //User.DeleteUser("bdm0121");
            //var result = User.GetUser("bdm0121").Result;

            //Creates default roles
            Role.CreateRole("Admin");
            Role.CreateRole("Coordinator");
            Role.CreateRole("Instructor");
            Role.CreateRole("Fellow");
            Role.CreateRole("FullTime");
            Role.CreateRole("Adjunct");
            Role.CreateRole("Student");

            //Gives admin access to:
            /*
                Bryan Morris (bdm0121)
                Sean Boden (slb0589)
                Stephen Cory Bishop (scb0231)
                Alex Lambert (apl0075)
                Chet Lockwood (cgl0021)
             */
            Role.AddRoleToUser("bdm0121", "Admin");
            Role.AddRoleToUser("slb0589", "Admin");
            //Role.AddRoleToUser("scb0231", "Admin");
            Role.AddRoleToUser("apl0075", "Admin");
            Role.AddRoleToUser("cgl0021", "Admin");

            Role.AddRoleToUser("slb0589", "Instructor");
            Role.AddRoleToUser("bdm0121", "Instructor");

            //this returns a list of users by the entered roll
            //var adminListTask = Role.GetUsersByRole("Admin");
            //var adminList = adminListTask.Result;

            //Role.RemoveRoleFromUser("bdm0121", "Admin");

            //Semester class testing
            /////////////////////////////////////////////////////////////////////////////////
            //Create
            Semester.AddSemester(new Semester("Spring", 2022));
            Semester.AddSemester(new Semester("Fall", 2022));
            Semester.AddSemester(new Semester("Summer", 2022));
            Semester.AddSemester(new Semester("Spring", 2023));
            Semester.AddSemester(new Semester("Fall", 2023));
            Semester.AddSemester(new Semester("Summer", 2023));
            //Read
            var springSemester = Semester.GetSemester("Spring", 2022);
            var fallSemester = Semester.GetSemester("Fall", 2022); // This won't work. It returns null when it can't find something
            //Update
            Semester.EditSemester("Spring", 2022, new Semester("Fall", 2022));
            //Read (again)
            springSemester = Semester.GetSemester("Spring", 2022);
            fallSemester = Semester.GetSemester("Fall", 2022);
            //Delete
            //Semester.DeleteSemester("Fall", 2022);
            System.Console.WriteLine(""); //This is a placeholder for a debugger break point

            //Major class testing
            /////////////////////////////////////////////////////////////////////////////////
            Major.AddMajor("Fall", 2022, "CS");
            //var temp = Major.GetMajor("Fall", 2022, "CS");
            //temp = Major.GetMajor("Fall", 2022, "CSE"); // return null
            //Major.EditMajor("Fall", 2022, "CS", new Major("IT"));
            //Major.DeleteMajor("Fall", 2022, "IT");
            System.Console.WriteLine("");

            //MajorOutcome class testing
            /////////////////////////////////////////////////////////////////////////////////
            MajorOutcome.AddMajorOutcome("Fall", 2022, "CS", new MajorOutcome("1", "Accomplishes gud at computers"));
            MajorOutcome.AddMajorOutcome("Fall", 2022, "CS", new MajorOutcome("2", "Accomplishes making type fast"));
            MajorOutcome.AddMajorOutcome("Fall", 2022, "CS", new MajorOutcome("3", "Accomplishes websites"));
            var tempMajorOutcome =  MajorOutcome.GetMajorOutcome("Fall", 2022, "CS", "2");
            MajorOutcome.EditMajorOutcome("Fall", 2022, "CS", "3", new MajorOutcome("3", "Gud at spelung"));
            MajorOutcome.DeleteMajorOutcome("Fall", 2022, "CS", "3");

            System.Console.WriteLine("");

            //Course class testing
            /////////////////////////////////////////////////////////////////////////////////
            Course.AddCourse("Fall", 2022, new Course("bdm0121", "3600", "Whatever", "", false, "CSCE"));
            Course.AddCourse("Fall", 2022, new Course("bdm0121", "1040", "Something", "", false, "CSCE"));
            //var course = Course.GetCourse("Fall", 2022, "CSCE", "3600"); // This is an example of a get function that loads one of the lists
            //var tempSemester = Semester.GetSemester("Fall", 2022); // This is a dumb get function, that only returns exactly what you ask for without the lists

            //Course.EditCourse("Fall", 2022, "CSCE", "3600", new Course("bdm0121", "2110", "It changed!", "", false, "CSCE"));
            //var course = Course.GetCourse("Fall", 2022, "CSCE", "2110");
            //course = Course.GetCourse("Fall", 2022, "BLEGH", "2110");
            //Course.DeleteCourse("Fall", 2022, "CSCE", "2110");

            //var temp = Semester.GetCourses("Fall", 2022);

            //Section class testing
            /////////////////////////////////////////////////////////////////////////////////
            Section.AddSection("Fall", 2022, "CSCE", "3600", new Section("bdm0121", false, "001", 150));
            Section.AddSection("Fall", 2022, "CSCE", "3600", new Section("bdm0121", false, "002", 739));
            Section.AddSection("Fall", 2022, "CSCE", "3600", new Section("bdm0121", false, "003", 42));
            //var temp = Course.GetSections("Fall", 2022, "CSCE", "3600");
            //var temp = Section.GetSection("Fall", 2022, "CSCE", "3600", "001");
            Section.EditSection("Fall", 2022, "CSCE", "3600", "002", new Section("bdm0121", true, "002", 140));
            Section.DeleteSection("Fall", 2022, "CSCE", "3600", "001");


            //CourseOutcome class testing
            /////////////////////////////////////////////////////////////////////////////////
            CourseOutcome.CreateCourseOutcome("Fall", 2022, "CSCE", "3600", new CourseOutcome("CS"));
            CourseOutcome.CreateCourseOutcome("Fall", 2022, "CSCE", "1040", new CourseOutcome("CS"));

            //CourseOutcome.AddMajorOutcome("Fall", 2022, "CSCE", "3600", "CS", "739");// These next 3 will not work
            //CourseOutcome.AddMajorOutcome("Fall", 2022, "CSCE", "3600", "CS", "22");
            //CourseOutcome.AddMajorOutcome("Fall", 2022, "CSCE", "1040", "CS", "42");
            CourseOutcome.AddMajorOutcome("Fall", 2022, "CSCE", "1040", "CS", "2"); // This one should work
            CourseOutcome.AddMajorOutcome("Fall", 2022, "CSCE", "1040", "CS", "1");

            System.Console.WriteLine("");

            CourseOutcome.DeleteCourseOutcome("Fall", 2022, "CSCE", "1040", "CS");

            //CourseOutcome.RemoveMajorOutcome("Fall", 2022, "CSCE", "1040", "CS", "2");

            System.Console.WriteLine(""); //This is a placeholder for a debugger break point
        }
    }
}
