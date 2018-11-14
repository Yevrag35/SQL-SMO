using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Security;

namespace SQL.SMO.Cmdlets
{
    #region New-SMO -- PSCmdlet
    [Cmdlet(VerbsCommon.New, "SMO")]
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
                    SecurePassword = sqlCreds.Password
                };
            }
            else
            {
                sqlConn = new SqlConnection();
                srvStr = srvStr + "Integrated Security=true;";
                sqlConn.ConnectionString = srvStr;
            }
            return sqlConn;
        }

        #endregion
    }

    #endregion
}
