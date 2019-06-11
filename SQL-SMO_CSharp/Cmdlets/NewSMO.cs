using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace SQL.SMO.Cmdlets
{
    #region New-SMO -- PSCmdlet
    [Cmdlet(VerbsCommon.New, "SMO", DefaultParameterSetName = "None")]
    [OutputType(typeof(Server))]
    [CmdletBinding(PositionalBinding = false)]
    public class NewSMO : PSCmdlet
    {
        #region Constants
        private const string constr = "Server={0};";

        #endregion

        #region Parameters
        [Parameter(Mandatory = false, Position = 0,
            HelpMessage = "The name of the SQL server (preferably the FQDN)")]
        public string ServerName = "localhost";

        [Parameter(Mandatory = false, Position = 1)]
        public string InstanceName = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "The SQL Authentication credentials for the chosen server/instance.")]
        public PSCredential SQLCredential;

        private bool _ssl;
        [Parameter(Mandatory = true, ParameterSetName = "UsingSSL")]
        public SwitchParameter EncryptConnection
        {
            get => _ssl;
            set => _ssl = value;
        }

        [Parameter(Mandatory = false, ParameterSetName = "UsingSSL")]
        public bool TrustServerCertificate = true;

        #endregion

        #region Process
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            dynamic sqlConn = MakeConnection(ServerName, InstanceName, SQLCredential);
            
            //ConfigProperty
            ServerConnection conn;
            if (sqlConn is SqlConnection)
            {
                try
                {
                    sqlConn.Open();
                }
                catch (SqlException e)
                {
                    throw new ContextExecutionError("A SQL error occurred while setting the context!  " + e.Message, e);
                }
                catch (InvalidOperationException e)
                {
                    throw new ContextExecutionError("An invalid operation occurred while setting the context!  " + e.Message, e);
                }
                conn = new ServerConnection(sqlConn);
            }
            else
            {
                conn = sqlConn;
            }
            var smo = new Server(conn);
            TestSMO(smo);
            WriteObject(smo, false);
        }

        #endregion
        #region Cmdlet Methods
        private dynamic MakeConnection(string name, string instance, PSCredential sqlCreds)
        {
            string srvStr = !string.IsNullOrEmpty(instance) && instance != "MSSQLSERVER"
                ? string.Format(constr, name + "\\" + instance)
                : string.Format(constr, name);

            dynamic sqlConn;
            if (sqlCreds != null)
            {
                sqlConn = new ServerConnection()
                {
                    LoginSecure = false,
                    ServerInstance = name,
                    Login = sqlCreds.UserName,
                    SecurePassword = sqlCreds.Password,
                    EncryptConnection = _ssl
                };
                if (_ssl)
                    sqlConn.TrustServerCertificate = TrustServerCertificate;
            }
            else
            {
                sqlConn = new SqlConnection();
                srvStr = srvStr + "Integrated Security=true;";
                if (_ssl)
                {
                    var trust = Convert.ToString(TrustServerCertificate).ToLower();
                    srvStr = srvStr + "Encrypt=true;TrustServerCertificate=" + trust + ";";
                }

                sqlConn.ConnectionString = srvStr;
            }
            return sqlConn;
        }

        private const string TEST_MEMBER = "Status";

        private void TestSMO(Server srv, string memberToTest = TEST_MEMBER)
        {
            var t = srv.GetType();
            try
            {
                t.InvokeMember(memberToTest, BindingFlags.GetProperty, null, srv, null);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException.InnerException != null && e.InnerException.InnerException is SqlException)
                    {
                        throw new ContextExecutionError(e.InnerException.InnerException.Message, (SqlException)e.InnerException.InnerException);
                    }
                    else if (e.InnerException.InnerException != null)
                    {
                        throw new ContextExecutionError(e.InnerException.InnerException.Message, e.InnerException.InnerException);
                    }
                    else
                    {
                        throw new ContextExecutionError(e.InnerException.Message, e.InnerException);
                    }
                }
                else
                {
                    throw new ContextExecutionError(e.Message, e);
                }
            }
        }

        #endregion
    }

    #endregion
}
