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
        /* Test(s) for the User methods */

        public void AddUserHelper(User __user)
        {
            User.AddUser(__user);
        }

        public void RemoveUserHelper(string __euid)
        {
            User.DeleteUser(__euid);
        }

        public void EditUserHelper(string __euid, User __newuserinfo)
        {
            User.EditUser(__euid, __newuserinfo);
        }

        public User GetUserHelper(string __euid)
        {
            return User.GetUser(__euid).Result;
        }

        [TestMethod]
        public void TestAddUser()
        {
            var __user = new User("Gandalf", "Grey", "gtg001");
            AddUserHelper(__user);
            var __expectedresult = GetUserHelper(__user.EUID);
            Assert.AreEqual(__user.EUID, __expectedresult.EUID);
            Assert.AreEqual(__user.FirstName, __expectedresult.FirstName);
            Assert.AreEqual(__user.LastName, __expectedresult.LastName);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            var __user = new User("Samwise", "Gamgee", "ssg001");
            User.AddUser(__user);
            var __euidtodelete = "ssg001";
            RemoveUserHelper(__euidtodelete);
            var __expectedresult = GetUserHelper(__user.EUID);
            Assert.AreEqual(__expectedresult, null);
        }

        [TestMethod]
        public void TestEditUser()
        {
            var __euidtoedit = "ssg001";
            var __user = new User("Frodo", "Baggins", "fb0001");
            AddUserHelper(__user);
            EditUserHelper(__euidtoedit, __user);
            var __expectedresult = GetUserHelper(__user.EUID);
            Assert.AreEqual(__user.FirstName, __expectedresult.FirstName);
            Assert.AreEqual(__user.LastName, __expectedresult.LastName);
            Assert.AreEqual(__user.EUID, __expectedresult.EUID);
            RemoveUserHelper("TestID");
        }
    }
}