using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AbetApi.EFModels;



namespace AbetApi.Tests
{
    [TestClass]
    public class AbetApiTests
    {
        [TestMethod]
        public void TestAddUser()
        {
            // Instantiate a user, in local memory
            var __user = new AbetApi.EFModels.User("Cory", "Bishop", "scb0231");
            // Write user to DB
            AbetApi.EFModels.User.AddUser(__user);
            // Grab the user's information via EUID
            var __expectedresult = AbetApi.EFModels.User.GetUser("scb0231").Result;
            // Check attributes vs one another
            Assert.AreEqual(__user.EUID, __expectedresult.EUID);
            Assert.AreEqual(__user.FirstName, __expectedresult.FirstName);
            Assert.AreEqual(__user.LastName, __expectedresult.LastName);
        }

    }
}