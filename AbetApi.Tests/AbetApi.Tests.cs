using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AbetApi.Data;
using AbetApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbetApi.EFModels;
using System.Collections.Generic;


namespace AbetApi.Tests
{
    [TestClass]
    public class AbetApiTests
    {
        /*
         * General Note(s):
         * Once error handling is implemented, additional and enhanced testing
         * will be included. But until then, this is a basis for checking basic
         * functionality as it exists currently. 
         * Test(s) for the User methods 
         */

        
        public void AddUserHelper(User user)
        {
            User.AddUser(user);
        }

        public void RemoveUserHelper(string euid)
        {
            User.DeleteUser(euid);
        }

        public void EditUserHelper(string euid, User newuserinfo)
        {
            User.EditUser(euid, newuserinfo);
        }

        public User GetUserHelper(string euid)
        {
            return User.GetUser(euid).Result;
        }

        [TestMethod]
        public void TestAddUser()
        {
            var user = new User("Gandalf", "Grey", "gtg001");
            AddUserHelper(user);
            var expectedresult = GetUserHelper(user.EUID);
            Assert.AreEqual(user.EUID, expectedresult.EUID);
            Assert.AreEqual(user.FirstName, expectedresult.FirstName);
            Assert.AreEqual(user.LastName, expectedresult.LastName);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            var user = new User("Samwise", "Gamgee", "ssg001");
            User.AddUser(user);
            var euidtodelete = "ssg001";
            RemoveUserHelper(euidtodelete);
            var expectedresult = GetUserHelper(user.EUID);
            Assert.AreEqual(expectedresult, null);
        }

        [TestMethod]
        public void TestEditUser()
        {
            var euidtoedit = "ssg001";
            var user = new User("Frodo", "Baggins", "fb0001");
            AddUserHelper(user);
            EditUserHelper(euidtoedit, user);
            var expectedresult = GetUserHelper(user.EUID);
            Assert.AreEqual(user.FirstName, expectedresult.FirstName);
            Assert.AreEqual(user.LastName, expectedresult.LastName);
            Assert.AreEqual(user.EUID, expectedresult.EUID);
            RemoveUserHelper("TestID");
        }

        /* Test(s) for the Role methods 
         * Need to implement remove role, but waiting on error handling
         * implementation.
         */

        public void CreateRoleHelper(string newrole)
        {
            Role.CreateRole(newrole);
        }

        public void AddRolesToUserHelper(string newrole, List<string> euids)
        {
            CreateRoleHelper(newrole);
            foreach (string euid in euids)
            {
                Role.AddRoleToUser(euid, newrole);
            }
        }

        [TestMethod]
        public void TestGetUsersByRole()
        {
            string newrole = "AdminTest";
            List<string> euids = new List<string>() { "fb0001", "gtg001" };
            AddRolesToUserHelper(newrole, euids);
            var results = Role.GetUsersByRole(newrole).Result;
            foreach(var result in results)
            {
                Assert.AreEqual(1, result.Roles.Count); // Each user's .Count should be 1
            }
        }
        
        [TestMethod]
        public void TestRemoveRoleFromUser()
        {
            string removerole = "AdminTest";
            string euid = "fb0001";
            Role.RemoveRoleFromUser(euid, removerole);
            var results = Role.GetUsersByRole(removerole).Result;
            Assert.AreEqual(1, results.Count);
        }

        /* Test(s) for the Semester methods */

        public int GetSemesterHelper()
        {
            return Semester.GetSemesters().Count;
        }

        [TestMethod]
        public void TestAddSemester()
        {
            var numsemesters = GetSemesterHelper();
            Semester semester = new Semester("Spring", 3030);
            Semester.AddSemester(semester);
            var results = GetSemesterHelper();
            Assert.IsTrue(results - numsemesters ==  1); // Will want to fix this in the future
        }

        [TestMethod]
        public void TestDeleteSemester()
        {
            Semester semester = new Semester("Spring", 3030);
            var numsemesters = GetSemesterHelper();
            Semester.DeleteSemester(semester.Term, semester.Year);
            var results = GetSemesterHelper();
            Assert.IsTrue(numsemesters - results == 1);
        }

        /* Test(s) for the Course methods */
        public Course CreateCourseHelper(string coordinator, string coursenumber, string displayname, string coordinatorcomment, bool iscoursecompleted, string department)
        {
            return new Course(coordinator, coursenumber, displayname, coordinatorcomment, iscoursecompleted, department); 
        }

        [TestMethod]
        public void TestAddCourse()
        {
            Semester semester = new Semester("Spring", 3030);
            Semester.AddSemester(semester);
            var course = CreateCourseHelper(
                "Elrond, Lord of Rivendell",
                "2001",
                "The Lord of the Rings",
                "A mighty Elf-ruler of old who lived in Middle-earth from the First Age to the beginning of the Fourth Age.",
                true,
                "FTSY"
           );
            Course.AddCourse("Spring", 3030, course);
            //FIXME Need a return status of code here for successful insertion
        }

        [TestMethod]
        public void TestGetCourse()
        {
            var course = Course.GetCourse("Spring", 3030, "FTSY", "2021");
            Assert.IsNotNull(course, "The course does not exist.");
        }

    }
}