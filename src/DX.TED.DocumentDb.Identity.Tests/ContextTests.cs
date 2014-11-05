using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DX.TED.DocumentDb.Identity.Tests
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        [TestCategory("AspNetIdentity.Integration")]
        public void ValidateEmptyConstructor()
        {
            var ctx = new DocumentDbContext();

            Assert.IsNotNull(ctx, "got no context");

            ctx.DeleteDatabase();
        }

        [TestMethod]
        [TestCategory("AspNetIdentity.Integration")]
        public void ValidateConstructorWithStringForAlternateConnectionOverride()
        {
            var ctx = new DocumentDbContext("OtherConnection");

            Assert.IsNotNull(ctx, "got no context with string");

        }

        [TestMethod]
        [TestCategory("AspNetIdentity.Integration")]
        //[ExpectedException(typeof(InvalidOperationException))]
        public void ValidateConstructorWithStringForAlternateConnectionOverrideFails()
        {
            var expectedMessage = "Can't get to collection as DB is still null";
            try
            {
                var ctx = new DocumentDbContext("OtherConnectionShouldFail");
                 Assert.IsNotNull(ctx, "got no context with string");
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.CheckAggregateException(expectedMessage));
            }

        }

        [TestMethod]
        [TestCategory("AspNetIdentity")]
        [ExpectedException(typeof(UriFormatException))]
        public void InvalidUriForConnectionString()
        {
            var ctx = new DocumentDbContext("InvalidUri");
        }

        
        [TestMethod]
        [TestCategory("AspNetIdentity")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void InvalidFormatForConnectionString()
        {
            var ctx = new DocumentDbContext("InvalidFormat");
        }

        
        [TestMethod]
        [TestCategory("AspNetIdentity")]
        [ExpectedException(typeof(FormatException))]
        public void InvalidTrueFalseForConnectionString()
        {
            var ctx = new DocumentDbContext("InvalidTrueFalse");
        }


        [TestMethod]
        [TestCategory("AspNetIdentity")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PassedNullString()
        {
            new DocumentDbContext(null);
        }

        [TestMethod]
        [TestCategory("AspNetIdentity")]
        [ExpectedException(typeof(ArgumentException))]
        public void PassedEmptyString()
        {
            new DocumentDbContext("");
        }

        [TestMethod]
        [TestCategory("AspNetIdentity")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ConstructorWithInvalidConnection()
        {
            var ctx = new DocumentDbContext("MissingConnection");

            Assert.IsNotNull(ctx, "got no context with string");

        }

        [TestMethod]
        [TestCategory("AspNetIdentity.Integration")]
        public void ConstructorInvalidAuthKey()
        {
            string message = null;
            string expectedMessage = "the input authorization token can't serve the request";

            try
            {
                var ctx = new DocumentDbContext("InvalidAuthKey");
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.CheckAggregateException(expectedMessage));
            }
            catch (DocumentClientException ex)
            {
                message = ex.Message;
                Assert.IsTrue(
                message.Contains(expectedMessage.ToLowerInvariant()), 
                "Invalid message on bad key exception");
            }

            

        }

        [TestMethod]
        [TestCategory("AspNetIdentity.Integration")]
        public void CreateDatabaseCheckThenDeleteIt()
        {
            var ctx = new DocumentDbContext("CreateDatabaseCheck", false);
            Assert.IsNotNull(ctx.Database, "Database is null");
            ctx.DeleteDatabase();
        }

        [TestMethod]
        [TestCategory("AspNetIdentity")]
        [ExpectedException(typeof(ConfigurationErrorsException))]
        public void ConstructorWithMissingConnectionStringFailed()
        {
            var ctx = new DocumentDbContext("NoConnectionString");

        }

    }




}
