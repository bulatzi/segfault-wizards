using AbetApi.EFModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;


namespace AbetApi.Tests
{
    [TestClass]
    public class AbetApiTests
    {

        [TestInitialize()]
        public void Initialize()
        {
            Console.WriteLine("Initializing test...");
            Semester semester = new Semester("Spring", 3030);
            _ = Semester.AddSemester(semester);
            AddMajorHelper("FTSY", "Spring", 3030);
            var user = new User("Gandalf", "Grey", "gtg001");
            AddUserHelper(user);
            var role = new Role("AdminTest");
            _ = Role.CreateRole(role);

        }

        [TestCleanup()]
        public void Cleanup()
        {
            Console.WriteLine("Cleaning up...");
            _ = Semester.DeleteSemester("Spring", 3030);
            _ = Major.DeleteMajor("Spring", 3030, "FTSY");
            _ = User.DeleteUser("gtg001");
            _ = Role.DeleteRole("AdminTest");
        }

        /*
         * General Note(s):
         * Once error handling is implemented, additional and enhanced testing
         * will be included. But until then, this is a basis for checking basic
         * functionality as it exists currently. 
         * Test(s) for the User methods 
         */


        public void AddUserHelper(User user)
        {
            _ = User.AddUser(user);
        }

        public void RemoveUserHelper(string euid)
        {
            _ = User.DeleteUser(euid);
        }

        public void EditUserHelper(string euid, User newuserinfo)
        {
            _ = User.EditUser(euid, newuserinfo);
        }

        public User GetUserHelper(string euid)
        {
            return User.GetUser(euid).Result;
        }

        [TestMethod]
        public void TestAddUser()
        {
            try
            {
                var user = new User("Samwise", "Gamgee", "ssg001");
                _ = User.AddUser(user);
                var expectedresult = GetUserHelper(user.EUID);
                Assert.AreEqual(user.EUID, expectedresult.EUID);
                Assert.AreEqual(user.FirstName, expectedresult.FirstName);
                Assert.AreEqual(user.LastName, expectedresult.LastName);
            }
            finally
            {
                _ = User.DeleteUser("ssg001");
            }

        }

        [TestMethod]
        public void TestDeleteUser()
        {
            var user = new User("Samwise", "Gamgee", "ssg001");
            _ = User.AddUser(user);
            try
            {
                RemoveUserHelper(user.EUID);
                var chkUser = User.GetUser(user.EUID);
                Assert.AreEqual(chkUser.Exception.Message, "One or more errors occurred. (The user specified does not exist in the database.)");

            }
            finally
            {
                RemoveUserHelper(user.EUID);
            }
        }

        [TestMethod]
        public void TestEditUser()
        {
            try
            {
                var user1 = new User("Samwise", "Gamgee", "ssg001");
                _ = User.AddUser(user1);
                var euidtoedit = "ssg001";
                var user2 = new User("Frodo", "Baggins", "fb001");
                AddUserHelper(user2);
                EditUserHelper(euidtoedit, user2);
                var expectedresult = GetUserHelper(user2.EUID);
                Assert.AreEqual(user2.FirstName, expectedresult.FirstName);
                Assert.AreEqual(user2.LastName, expectedresult.LastName);
                Assert.AreEqual(user2.EUID, expectedresult.EUID);
            }
            finally
            {
                _ = User.DeleteUser("ssg001");
                _ = User.DeleteUser("fb001");
            }
        }

        /* Test(s) for the Role methods 
         * Need to implement remove role, but waiting on error handling
         * implementation.
         */

        [TestMethod]
        public void TestGetUsersByRole()
        {
            try
            {
                _ = Role.AddRoleToUser("gtg001", "AdminTest");
                var results = Role.GetUsersByRole("AdminTest").Result;
                foreach (var result in results)
                {
                    Assert.AreEqual(1, result.Roles.Count); // Each user's .Count should be 1
                }
            }
            finally
            {
                _ = Role.RemoveRoleFromUser("gtg001", "AdminTest");
            }
        }

        [TestMethod]
        public void TestRemoveRoleFromUser()
        {
            try
            {
                _ = Role.AddRoleToUser("gtg001", "AdminTest");
                var results = Role.GetUsersByRole("AdminTest");
                foreach (var result in results.Result)
                {
                    Assert.AreEqual(1, result.Roles.Count); // Each user's .Count should be 1
                }
                _ = Role.RemoveRoleFromUser("gtg001", "AdminTest");
                var results2 = Role.GetUsersByRole("AdminTest").Result.Count;
                Assert.AreEqual(results2, 0);
            }
            finally
            {
                _ = Role.RemoveRoleFromUser("gtg001", "AdminTest");
            }
        }

        /* Test(s) for the Semester methods */

        public int GetSemesterHelper()
        {
            return Semester.GetSemesters().Result.Count;
        }

        [TestMethod]
        public void TestAddSemester()
        {
            try
            {
                var numsemesters = GetSemesterHelper();
                Semester semester = new Semester("Fall", 3030);
                _ = Semester.AddSemester(semester);
                var results = GetSemesterHelper();
                Assert.IsTrue(results - numsemesters == 1); // Will want to fix this in the future
            }
            finally
            {
                _ = Semester.DeleteSemester("Fall", 3030);
            }
        }

        [TestMethod]
        public void TestDeleteSemester()
        {
            try
            {
                var numsemesters = GetSemesterHelper();
                Semester semester = new Semester("Fall", 3030);
                _ = Semester.AddSemester(semester);
                var results = GetSemesterHelper();
                Assert.AreEqual(numsemesters + 1, results);
                _ = Semester.DeleteSemester(semester.Term, semester.Year);
                results = GetSemesterHelper();
                Assert.AreEqual(numsemesters, results);
            }
            finally
            {
                _ = Semester.DeleteSemester("Fall", 3030);
            }
        }

        /* Test(s) for the Major methods */

        public void AddMajorHelper(string name, string term, int year)
        {
            _ = Major.AddMajor(term, year, name);
        }

        [TestMethod]
        public void TestAddMajor()
        {
            string name = "SYFY";
            string term = "Spring";
            int year = 3030;
            _ =  Major.AddMajor(term, year, name);
            bool chkMajor = false;
            try
            {
                var results = Major.GetMajors(term, year);
                foreach(var result in results.Result)
                {
                    if (result.Name == name)
                    {
                        chkMajor = true;
                    }
                }
                Assert.IsTrue(chkMajor);
            }
            finally
            {
                _ = Major.DeleteMajor(term, year, name);
            }
        }

        [TestMethod]
        public void TestGetMajor()
        {
            Assert.IsTrue(true);
            try
            {
                _ = Major.AddMajor("Spring", 3030, "SYFY");
                var results = Major.GetMajors("Spring", 3030);
                bool chkMajor = false;
                foreach(var result in results.Result)
                {
                    if(result.Name.ToUpper() == "SYFY")
                    {
                        chkMajor = true;
                    }
                }
                Assert.IsTrue(chkMajor);
            }
            finally 
            {
                _ = Major.DeleteMajor("Spring", 3030, "SYFY");
            }
        }

        [TestMethod]
        public void TestGetMajors()
        {
            var results = Major.GetMajors("Spring", 3030);
            bool chkIfMajorExists = false;
            foreach (var result in results.Result)
            {
                if (result.Name == "FTSY")
                {
                    chkIfMajorExists = true;
                }
            }
            Assert.IsTrue(chkIfMajorExists);
        }

        [TestMethod]
        public void TestEditMajors()
        {
            _ = Major.AddMajor("Spring", 3030, "TestEdit");
            _ = Major.EditMajor("Spring", 3030, "TestEdit", "TestEditFTSY");
            bool chkMajor = false;
            var results = Major.GetMajors("Spring", 3030);
            foreach(var result in results.Result)
            {
                if (result.Name.ToUpper() == "TESTEDITFTSY")
                {
                    chkMajor = true;
                }
            }
            Assert.IsTrue(chkMajor);
        }

        [TestMethod]
        public void TestDeleteMajor()
        {
            _ = Major.DeleteMajor("Spring", 3030, "TestEditFTSY");
            var results = Major.GetMajors("Spring", 3030);
            bool chkMajor = false;
            foreach(var result in results.Result)
            {
                if(result.Name.ToUpper() == "TESTEDITFTSY")
                {
                    chkMajor = true;
                }
            }
            Assert.IsFalse(chkMajor);
        }

        /* Test(s) for the Course methods */
        public Course CreateCourseHelper(string coordinator, string coursenumber, string displayname, string coordinatorcomment, bool iscoursecompleted, string department)
        {
            return new Course(coordinator, coursenumber, displayname, coordinatorcomment, iscoursecompleted, department);
        }

        [TestMethod]
        public void TestAddCourse()
        {
            Semester semester = new Semester("Spring", 3031);
            _ = Semester.AddSemester(semester);
            Course course = new Course(
                "Elrond, Lord of Rivendell",
                "2021",
                "The Lord of the Rings",
                "A mighty Elf-ruler of old who lived in Middle-earth from the First Age to the beginning of the Fourth Age.",
                true,
                "FTSY"
           );
            _ = Course.AddCourse("Spring", 3031, course);
            var result = Course.GetCourse("Spring", 3031, course.Department, course.CourseNumber).Result;
            Assert.AreEqual(result.CoordinatorEUID.ToLower(), course.CoordinatorEUID);
            Assert.AreEqual(result.CourseNumber, course.CourseNumber);
            Assert.AreEqual(result.DisplayName, course.DisplayName);
            Assert.AreEqual(result.CoordinatorComment, course.CoordinatorComment);
            Assert.AreEqual(result.IsCourseCompleted, course.IsCourseCompleted);
            Assert.AreEqual(result.Department, course.Department);
        }

        [TestMethod]
        public void TestGetCourse()
        {
            var course = Course.GetCourse("Spring", 3031, "ThisCourseDefinitelyDoesNotExist", "3031");
            Assert.AreEqual(course.Exception.Message, "One or more errors occurred. (The course specified does not exist in the database.)");
        }

        [TestMethod]
        public void TestEditCourse()
        {
            Course course = new Course(
            "Elrond, Lord of Rivendell",
            "2021",
            "The Lord of the Rings",
            "A mighty Elf-ruler of old who lived in Middle-earth from the First Age to the beginning of the Fourth Age.",
            true,
            "FTSY"
            );

            Course newCourse = new Course(
            "Elrond, Lord of Rivendell (EDIT)",
            "2022",
            "The Lord of the Rings (EDIT)",
            "A mighty Elf-ruler of old who lived in Middle-earth from the First Age to the beginning of the Fourth Age. (EDIT)",
            false,
            "FTSY"
            );

            _ = Course.EditCourse("Spring", 3031, course.Department, course.CourseNumber, newCourse);
            var result = Course.GetCourse("Spring", 3031, "FTSY", "2022").Result;
            Assert.AreNotEqual(result.CoordinatorEUID, course.CoordinatorEUID);
            Assert.AreNotEqual(result.CourseNumber, course.CourseNumber);
            Assert.AreNotEqual(result.DisplayName, course.DisplayName);
            Assert.AreEqual(result.CoordinatorComment, course.CoordinatorComment);
            Assert.AreEqual(result.IsCourseCompleted, course.IsCourseCompleted);
            Assert.AreEqual(result.CourseNumber, "2022");

        }

        ///* Test(s) for the Section methods */
        [TestMethod]
        public void TestAddSection()
        {
            try
            {
                Course course = new Course("gtg001", "2022", "TestCourse", "Test Coordinator Comment", true, "FTSY");
                _ = Course.AddCourse("Spring", 3030, course);
                Section section = new Section("gtg001", true, "000099", 9);
                _ = Section.AddSection("Spring", 3030, "FTSY", "2022", section);
                var result = Section.GetSection("Spring", 3030, "FTSY", "2022", section.SectionNumber).Result;
                Assert.AreEqual(result.InstructorEUID, section.InstructorEUID);
                Assert.AreEqual(result.IsSectionCompleted, section.IsSectionCompleted);
                Assert.AreEqual(result.NumberOfStudents, section.NumberOfStudents);
                Assert.AreEqual(result.SectionNumber, section.SectionNumber);
            }
            finally
            {
                _ = Section.DeleteSection("Spring", 3030, "FTSY", "2022", "000099");
            }
        }

        [TestMethod]
        public void TestEditSection()
        {
            Section section1 = new Section("gtg001", false, "000005", 10);
            Section section2 = new Section("gtg001", false, "000009", 10);

            _ = Section.AddSection("Spring", 3031, "FTSY", "2022", section1);
            _ = Section.EditSection("Spring", 3031, "FTSY", "2022", "000005", section2);
            var result = Section.GetSection("Spring", 3031, "FTSY", "2022", section2.SectionNumber).Result;
            Assert.AreEqual(result.InstructorEUID, section1.InstructorEUID);
            Assert.IsFalse(result.IsSectionCompleted);
            Assert.AreNotEqual(result.NumberOfStudents, 9);
            Assert.AreEqual(result.SectionNumber, "000009");
        }
    }
}