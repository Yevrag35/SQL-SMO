using MG.Encryption;
using MG.Sql.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MG.Sql.Smo.UnitTests
{
    public static class Context
    {
        public static Server Connection { get; set; }
        public static List<Database> Databases { get; set; }
        public static bool IsSet => Connection != null && Connection.ConnectionContext != null && Connection.ConnectionContext.IsOpen;

        public static Server Connect(string server)
        {
            string conStr = string.Format("Server={0};Integrated Security=true;Encrypt=true;TrustServerCertificate=true;", server);
            var sqlCon = new SqlConnection(conStr);
            sqlCon.Open();
            var serverCon = new ServerConnection(sqlCon);
            return new Server(serverCon);
        }
        public static Server Connect(string server, string user, string pass)
        {
            string conStr = string.Format("Server={0};Encrypt=true;TrustServerCertificate=true;", server);
            var ss = new SecureString();
            foreach (char c in pass)
            {
                ss.AppendChar(c);
            }
            ss.MakeReadOnly();
            var sqlCreds = new SqlCredential(user, ss);
            var sqlCon = new SqlConnection(conStr, sqlCreds);
            sqlCon.Open();
            var serverCon = new ServerConnection(sqlCon);
            return new Server(serverCon);
        }

        internal static void SecretConnect()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("USERDNSDOMAIN")))
            {
                Connection = Connect("GARVMEDIA.yevrag35.com");
            }
            else
            {
                byte[] bytes = null;
                using (var reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Mike Garvey\\SQL-SMO"))
                {
                    bytes = reg.GetValue("blah") as byte[];
                }
                var meth = new Methods();
                StringResult res = meth.DecryptContent(bytes);
                Connection = Connect("dgrlab-sccmsql.dgrlab.com", "mikelogin", res.String.ToString());
            }
        }

        internal static void DBValidate()
        {
            Validate();
            if (Databases == null)
                Databases = new List<Database>(Connection.Databases.Cast<Database>());
        }

        internal static void Validate()
        {
            if (!IsSet)
                SecretConnect();
        }
    }
}
