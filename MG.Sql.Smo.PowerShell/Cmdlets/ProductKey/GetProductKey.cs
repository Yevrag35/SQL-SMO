using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "ProductKey", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SmoLicense))]
    public class GetProductKey : PSCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS

        private const string BASE_KEY = "SOFTWARE\\Microsoft\\Microsoft SQL Server";
        private const string CLIENT_SETUP = "ClientSetup";
        private const string DEF_INST = "MSSQLSERVER";
        private const string DIGITALID = "DigitalProductID";
        private const string EDITION = "Edition";
        private const string PATCH_LEVEL = "PatchLevel";
        private const string SETUP = "Setup";
        private const string TOOLS = "Tools";

        private static char[] CharArray;
        private static readonly string[] STR_ARRAY = new string[24]
        {
            "B", "C", "D", "F", "G", "H", "J", "K", "M", "P", "Q", "R", "T", "V", "W", "X", "Y", "2", "3", "4", "6", "7", "8", "9"
        };

        #endregion

        #region PARAMETERS

        [Parameter(Mandatory = false, Position = 0, ValueFromPipelineByPropertyName = true)]
        [Alias("ServerName")]
        public string[] ComputerName { get; set; }

        [Parameter(Mandatory = false, Position = 1, ValueFromPipelineByPropertyName = true)]
        [Alias("Instance")]
        public string[] InstanceName { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => CharArray = GetCharArray(STR_ARRAY);

        protected override void ProcessRecord()
        {
            this.GetEverythingReady();

            for (int c = 0; c < this.ComputerName.Length; c++)
            {
                string cn = this.ComputerName[c];
                IEnumerable<ProductRegistry> haveLicenses = null;
                try
                {
                    haveLicenses = GetRegistryKeys(cn);
                }
                catch (IOException ex)
                {
                    string msg = string.Format("The remote registry service on {0} is not running or the server is offline.", cn);
                    var errRec = new ErrorRecord(new IOException(msg), typeof(IOException).FullName, ErrorCategory.ConnectionError, cn);
                    base.WriteError(errRec);
                }

                if (haveLicenses != null)
                {
                    foreach (ProductRegistry prodReg in haveLicenses)
                    {
                        var license = new SmoLicense(cn, prodReg, CharArray);
                        base.WriteObject(license);
                    }
                }
            }
        }

        #endregion

        private void GetEverythingReady()
        {
            if (!this.MyInvocation.BoundParameters.ContainsKey("ComputerName") &&
                SmoContext.IsSet && SmoContext.IsConnected)
            {
                this.ComputerName = new string[1] { SmoContext.Connection.Name };
            }
            else if (!this.MyInvocation.BoundParameters.ContainsKey("ComputerName"))
                throw new SmoContextNotSetException();

            if (!this.MyInvocation.BoundParameters.ContainsKey("InstanceName"))
            {
                this.InstanceName = new string[1] { DEF_INST };
            }
        }

        #region CMDLET METHODS
        private static char[] GetCharArray(string[] chars)
        {
            char[] chArray = new char[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                chArray[i] = chars[i][0];
            }
            return chArray;
        }

        private IEnumerable<ProductRegistry> GetRegistryKeys(string cn)
        {
            var comparer = SmoContext.GetComparer();
            var list = new List<ProductRegistry>();
            using (var regKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, cn))
            {
                using (RegistryKey baseKey = regKey.OpenSubKey(BASE_KEY))
                {
                    foreach (string key in baseKey.GetSubKeyNames())
                    {
                        for (int i = 0; i < this.InstanceName.Length; i++)
                        {
                            string inst = this.InstanceName[i];
                            if (key.Contains(inst))
                            {
                                ProductRegistry prodReg = GetSetupKeys(inst, key, baseKey);
                                if (prodReg != null)
                                    list.Add(prodReg);
                            }
                        }
                    }
                }
            }
            return list;
        }

        private static ProductRegistry GetSetupKeys(string instance, string key, RegistryKey baseKey)
        {
            IEqualityComparer<string> comparer = SmoContext.GetComparer();
            using (RegistryKey test = baseKey.OpenSubKey(key))
            {
                if (test.GetSubKeyNames().Contains(SETUP))
                {
                    using (RegistryKey setup = test.OpenSubKey(SETUP))
                    {
                        return setup.GetValueNames().Contains(DIGITALID)
                            ? new ProductRegistry(instance, setup)
                            : null;
                    }
                }
                else
                    return null;
            }
            
        }

        private static void MassDispose(IEnumerable<IDisposable> disposables)
        {
            foreach (IDisposable disp in disposables)
            {
                disp.Dispose();
            }
        }

        #endregion
    }
}