using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace MG.Sql.Smo.UnitTests
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void TestConstructors()
        {
            Context.Validate();

            var smoConf = new SmoConfiguration(Context.Connection.Configuration);
            Assert.IsNotNull(smoConf);
        }
    }
}
