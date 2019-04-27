using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommunications.Connect, "SmoServer", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "None")]
    [Alias("consmo")]
    [CmdletBinding(PositionalBinding = false)]
    public class ConnectSmoServer : PSCmdlet
    {
        #region PRIVATE CONSTANTS
        private const string CON_STRING = "Server={0};";
        private const string INST_FORMAT = "{0}\\{1}";
        private const string INTEGRATED_FORMAT = "Integrated Security=true;";
        private const string ENCRYPT_FORMAT = "Encrypt={0};TrustServerCertificate={1};";

        #endregion

        #region PARAMETERS

        [Parameter(Mandatory = false, Position = 0, ValueFromPipelineByPropertyName = true,
            HelpMessage = "The name of the SQL server (preferably the FQDN)")]
        public string ServerName = "localhost";

        [Parameter(Mandatory = false, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string InstanceName { get; set; }

        [Parameter(Mandatory = false)]
        public PSCredential SQLCredential { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "UsingSSL")]
        public SwitchParameter EncryptConnection { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "UsingSSL")]
        public bool TrustServerCertificate = true;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        #endregion

        #region CMDLET PROCESS

        protected override void ProcessRecord()
        {
            if (InstanceName == "MSSQLSERVER")
                InstanceName = null;

            var smo = new Server(
                this.CreateConnection(
                    ServerName, InstanceName, SQLCredential, EncryptConnection.ToBool(), TrustServerCertificate
                )
            );
            this.TestSMO(smo);
            SMOContext.AddConnection(smo, Force.ToBool());
        }

        #endregion

        #region CMDLET METHODS

        private string AddEncryption(string conStr, bool encrypt, bool trustCert)
        {
            string finalStr = encrypt
                ? string.Format(
                    "{0}{1}", conStr,
                    string.Format(
                        ENCRYPT_FORMAT,
                        Convert.ToString(encrypt),
                        Convert.ToString(trustCert)
                    )
                )
                : conStr;

            return finalStr;
        }

        private string AddIntegratedSecurity(string conStr) => string.Format("{0}{1}", conStr, INTEGRATED_FORMAT);

        private ServerConnection CreateConnection(string server, string instance, PSCredential sqlCreds, bool encrypt, bool trustCert)
        {
            if (!string.IsNullOrEmpty(instance))
                server = string.Format(INST_FORMAT, server, instance);

            string withServer = string.Format(CON_STRING, server);
            string building = this.AddEncryption(withServer, encrypt, trustCert);

            return this.NewConnection(building, sqlCreds);
        }

        private ServerConnection NewConnection(string conStr, PSCredential sqlCreds)
        {
            SqlConnection sqlCon = null;
            if (sqlCreds == null)
            {
                string finalStr = this.AddIntegratedSecurity(conStr);
                sqlCon = new SqlConnection(finalStr);
            }
            else
            {
                sqlCreds.Password.MakeReadOnly();
                var sc = new SqlCredential(sqlCreds.UserName, sqlCreds.Password);
                sqlCon = new SqlConnection(conStr, sc);
            }
            sqlCon.Open();
            return new ServerConnection(sqlCon);
        }

        private void TestSMO(Server srv)
        {
            string memberToTest = "Status";
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
}
