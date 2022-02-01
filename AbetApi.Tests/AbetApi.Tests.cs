using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbetApi.EFModels;
using AbetApi.Data;



namespace AbetApi.Tests
{
    [TestClass]
    public class AbetApiTests
    {
        [TestMethod]
        public void TestAddUser()
        {
            // Instantiate a user, in local memory
            var __user = new User("Cory", "Bishop", "scb0231");
            // Write user to DB
            User.AddUser(__user);
            // Grab the user's information via EUID
            var __expectedresult = User.GetUser("scb0231").Result;
            // Check attributes vs one another
            Assert.AreEqual(__user.EUID, __expectedresult.EUID);
            Assert.AreEqual(__user.FirstName, __expectedresult.FirstName);
            Assert.AreEqual(__user.LastName, __expectedresult.LastName);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            // Delete a user we know exists
            // Need to handle case where we delete a user who doesn't exist in DB
            // This test case going to fail until we flesh out method a little more
            var __user = new User("John", "Connor", "terminator");
            User.AddUser(__user); 
            var __euidtodelete = "terminator";
            User.DeleteUser(__euidtodelete);
            var __expectedresult = User.GetUser("terminator");
            Assert.AreEqual(__expectedresult.Result, null);
        }

    }
}