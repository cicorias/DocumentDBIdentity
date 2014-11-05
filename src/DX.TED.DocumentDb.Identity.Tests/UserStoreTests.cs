using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace DX.TED.DocumentDb.Identity.Tests
{
    [TestClass]
    public class UserStoreTests
    {

        DocumentDbContext docDbContext = null;

        [TestInitialize]
        public void Initialize()
        {
            docDbContext = new DocumentDbContext("UserStoreTests");
        }

        [TestCleanup]
        public void Cleanup()
        {

        }

        [TestMethod]
        [TestCategory("UserStore.Integration")]
        public void CreateUser()
        {
            string userName = "chuckTEST";
            UserStore<IdentityUser> userStore = new UserStore<IdentityUser>(docDbContext);

            var idUser = new IdentityUser(userName);
            var task = userStore.CreateAsync(idUser);
            task.Wait();

            var user = GetUser(userName);

            Console.WriteLine(user.Id);

            Assert.AreEqual(idUser.Id, user.Id,"User ID's are not equal");

            Helpers.DeleteUser(docDbContext, userName);
        }

        [TestMethod]
        [TestCategory("UserStore.Integration")]
        public void FindByUserId()
        {
            string userName = "chuckTEST";
            UserStore<IdentityUser> userStore = new UserStore<IdentityUser>(docDbContext);

            var idUser = new IdentityUser(userName);
            var task = userStore.CreateAsync(idUser);
            task.Wait();

            //act
            var foundUser = userStore.FindByIdAsync(idUser.Id).Result;

            Console.WriteLine(foundUser.Id);

            Assert.AreEqual(idUser.Id, foundUser.Id, "User ID's are not equal");

            Helpers.DeleteUser(docDbContext, userName);
        }

        [TestMethod]
        [TestCategory("UserStore.Integration")]
        public void FindByUserName()
        {
            string userName = "chuckTEST";
            UserStore<IdentityUser> userStore = new UserStore<IdentityUser>(docDbContext);

            var idUser = new IdentityUser(userName);
            var task = userStore.CreateAsync(idUser);
            task.Wait();

            //act
            var foundUser = userStore.FindByNameAsync(userName).Result;

            Console.WriteLine(foundUser.Id);

            Assert.AreEqual(idUser.Id, foundUser.Id, "User ID's are not equal");

            Helpers.DeleteUser(docDbContext, userName);
        }

        [TestMethod]
        [TestCategory("UserStore.Integration")]
        public void FindUserClaims()
        {
            string userName = "chuckTEST";
            UserStore<IdentityUser> userStore = new UserStore<IdentityUser>(docDbContext);

            var idUser = new IdentityUser(userName);
            var task = userStore.CreateAsync(idUser);
            task.Wait();

            //act
            var claims = userStore.GetClaimsAsync(idUser).Result;

            Console.WriteLine(claims.Count);

            Helpers.DeleteUser(docDbContext, userName);

        }

        IdentityUser GetUser(string userName)
        {
            DocumentClient client = new DocumentClient(docDbContext.Uri, docDbContext.AuthKey);
            Task<DocumentCollection> task1 = Helpers.GetDocCollection(docDbContext);

            FeedOptions options = new FeedOptions { };
            var result = client.CreateDocumentQuery<IdentityUser>(task1.Result.SelfLink)
                     .Where(f => f.UserName == userName).ToArray().FirstOrDefault();

            return result;
        }





    }



}
