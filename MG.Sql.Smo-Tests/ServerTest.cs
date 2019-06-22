using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MG.Sql.Smo.UnitTests
{
    [TestClass]
    public class ServerTest
    {
        private const string TEST_PROP = "Triggers";
        private const string BAD_PROP = "asdfhgas7u87";

        [TestMethod]
        public void TestConstructors()
        {
            Context.Validate();

            var smoServer = new SmoServer(Context.Connection);

            Assert.IsNotNull(smoServer);
            
            smoServer.LoadProperty(TEST_PROP);
            Assert.IsNotNull(smoServer.Triggers);
            Assert.IsInstanceOfType(smoServer.Triggers, typeof(ServerDdlTriggerCollection));

            smoServer.LoadProperty(BAD_PROP);
            Server servObj = null;
            try
            {
                servObj = smoServer;
            }
            catch
            {
                Assert.Fail("Implicit conversion of SmoServer to Server failed.");
            }
            // and back...
            try
            {
                var another = (SmoServer)servObj;
            }
            catch
            {
                Assert.Fail("Explicit conversion of Server to SmoServer failed.");
            }
        }
    }
}
