using MG.Sql.Smo.PowerShell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Management.Automation;
using System.Threading;

namespace MG.Sql.Smo.UnitTests
{
    [TestClass]
    public class CmdletTesting
    {
        [TestMethod]
        public void ConnectServerCmdlet()
        {
            var connect = new ConnectServer
            {
                ServerName = "TPSCCM.yevrag35.com"
            };
            Assert.IsTrue(connect.Invoke<object>().Count() == 0);

            Thread.Sleep(2000);

            var disconnect = new DisconnectServer();

            Assert.IsTrue(disconnect.Invoke<object>().Count() == 0);

            Thread.Sleep(500);

            connect.EncryptConnection = true;
            connect.Force = true;
            Assert.IsTrue(connect.Invoke<object>().Count() == 0);

            Thread.Sleep(2000);

            disconnect.Invoke();
        }


    }
}
