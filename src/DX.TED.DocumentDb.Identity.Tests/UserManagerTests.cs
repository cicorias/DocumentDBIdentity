using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DX.TED.DocumentDb.Identity.Models;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using System.Threading.Tasks;

namespace DX.TED.DocumentDb.Identity.Tests
{
    [TestClass]
    public class UserManagerTests
    {

        DocumentDbContext _context;
        string _userName;
        string _userEmail;

        const string _validCharacters = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ!#$%&'*+-/=?^_`{|}~";


        [TestInitialize]
        public void TestSetup()
        {
            _context = new DocumentDbContext("UserManagerTests");
            _userName = RandomString.Generate(20, _validCharacters);
            _userEmail = _userName + "@nowhere.com";
        }


        [TestCleanup]
        public void TestCleanup()
        {
            Helpers.DeleteUser(_context, _userName);
        }



        [TestMethod]
        [TestCategory("UserManager.Integration")]
        public void AddUserViaUserManager()
        {

            IdentityFactoryOptions<ApplicationUserManager> options = new IdentityFactoryOptions<ApplicationUserManager>();
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
            manager = ApplicationUserManager.Initialize(options, manager);

            var password = "Foobar@1234";
            var user = new ApplicationUser { UserName = _userName, Email = _userEmail};
            
            //act
            var result = manager.CreateAsync(user, password).Result;

            foreach (var item in result.Errors)
            {
                Console.WriteLine(item);
            }
            Assert.IsTrue(result.Succeeded, "Didn't succeed for some reason");

        }
    }
}
