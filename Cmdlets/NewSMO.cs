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
        public string InstanceName = String.Empty;

        [Parameter(Mandatory = false, HelpMessage = "The SQL Authentication credentials for the chosen server/instance.")]
        public PSCredential SQLCredential;

        #endregion
        #region Process
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            SqlConnection sqlConn = MakeConnection(ServerName, InstanceName, SQLCredential);
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
            //ConfigProperty
            ServerConnection conn = new ServerConnection(sqlConn);
            Server smo = new Server(conn);
            WriteObject(smo, false);
        }

        #endregion
        #region Cmdlet Methods
        private SqlConnection MakeConnection(string name, string instance, PSCredential sqlCreds)
        {
            string srvStr;
            if (!String.IsNullOrEmpty(instance) && instance != "MSSQLSERVER")
            {
                srvStr = String.Format(constr, name + "\\" + instance);
            }
            else
            {
                srvStr = String.Format(constr, name);
            }
            SqlConnection sqlConn = new SqlConnection();
            if (sqlCreds != null)
            {
                SecureString pass = sqlCreds.Password;
                pass.MakeReadOnly();
                SqlCredential sc = new SqlCredential(sqlCreds.UserName, pass);
                sqlConn.Credential = sc;
            }
            else
            {
                srvStr = srvStr + "Integrated Security=true;";
            }
            sqlConn.ConnectionString = srvStr;
            return sqlConn;
        }

        #endregion
    }

    #endregion
}
