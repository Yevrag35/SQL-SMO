using Microsoft.ActiveDirectory.Management;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Timers;

namespace MG.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SmoSqlInstance", ConfirmImpact = ConfirmImpact.None,
        DefaultParameterSetName = "SpecifyComputerName")]
    [OutputType(typeof(SqlInstanceResult))]
    public class GetSmoSqlInstance : PSCmdlet
    {
        private const string COMPUTERNAME = "COMPUTERNAME";
        private const string DISPLAY_NAME = "DisplayName";
        private const string DNS_HOSTNAME = "DNSHostName";
        private const string WMI = "WMI";
        private const string REG = "Registry";
        private const string BOTH = "Both";
        private const string SSREGEX = @"^SQL\sServer\s\((.{1,})\)$";
        private const string SERVICE_WMI = "SELECT Name, DisplayName FROM Win32_Service WHERE " + DISPLAY_NAME + " LIKE 'SQL Server (%)'";
        private const string SERVICE_NS = "\\\\{0}\\root\\cimv2";
        private const string REG_KEY = "SOFTWARE\\Microsoft\\Microsoft SQL Server\\Instance Names\\SQL";
        private const string ACTITIY = "Searching for SQL Instances";
        private const string STATUS_FORMAT = "Querying object {0}/{1}...";
        private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.Instance;

        //private List<string> Names;
        private List<Task<IEnumerable<SqlInstanceResult>>> Tasks;
        private int TotalCount;

        #region PARAMETERS
        [Parameter(Mandatory = false, ParameterSetName = "SpecifyComputerName", ValueFromPipeline = true, Position = 0)]
        public string ComputerName = Environment.GetEnvironmentVariable(COMPUTERNAME);

        [Parameter(Mandatory = true, ParameterSetName = "ByADComputerPipeline", DontShow = true, ValueFromPipeline = true)]
        public ADComputer InputObject { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        [ValidateSet(BOTH, REG, WMI)]
        public string SearchMethod = WMI;

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            //Names = new List<string>();
            Tasks = new List<Task<IEnumerable<SqlInstanceResult>>>();
        }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "ByADComputerPipeline")
            {
                ComputerName = !string.IsNullOrEmpty(this.InputObject.DNSHostName) 
                    ? this.InputObject.DNSHostName 
                    : this.InputObject.Name;
            }
            //Names.Add(ComputerName);
            Tasks.Add(this.QueryAsync(ComputerName));
        }

        protected override void EndProcessing()
        {
            TotalCount = Tasks.Count;

            while (Tasks.Count > 0)
            {
                this.UpdateProgress(0, Tasks.Count);
                for (int cn = Tasks.Count - 1; cn >= 0; cn--)
                {
                    Task<IEnumerable<SqlInstanceResult>> t = Tasks[cn];

                    if (t.IsCompleted)
                    {
                        if (t.Result != null)
                            WriteObject(t.Result, true);
                        Tasks.Remove(t);
                    }
                    else if (t.IsCanceled)
                    {
                        Tasks.Remove(t);
                    }
                }
                Thread.Sleep(1000);
            }
            this.UpdateProgress(0);
        }

        #endregion

        #region CMDLET METHODS
        private void UpdateProgress(int id, int on)
        {
            int realOn = TotalCount - on;
            var progressRecord = new ProgressRecord(id, ACTITIY, string.Format(
                STATUS_FORMAT, realOn, TotalCount));
            double num = Math.Round(realOn / (double)TotalCount * 100, 2, MidpointRounding.ToEven);
            progressRecord.PercentComplete = Convert.ToInt32(num);
            WriteProgress(progressRecord);
        }
        private void UpdateProgress(int id)
        {
            var pr = new ProgressRecord(id, ACTITIY, "Completed")
            {
                RecordType = ProgressRecordType.Completed
            };
            WriteProgress(pr);
        }

        private async Task<IEnumerable<SqlInstanceResult>> QueryAsync(string computerName)
        {
            var source = new CancellationTokenSource(7000);
            return await Task.Run(() =>
            {
                bool retry = false;
                IEnumerable<SqlInstanceResult> results = null;
                if (SearchMethod == BOTH || SearchMethod == WMI)
                {
                    try
                    {
                        results = this.FindInstancesUsingWMI(computerName);
                    }
                    catch (COMException)
                    {
                        retry = true;
                    }
                }
                if (SearchMethod == REG || (retry && SearchMethod == BOTH))
                {
                    try
                    {
                        results = this.FindInstancesUsingRegistry(computerName);
                    }
                    catch (IOException) { }
                }
                return results;
            }, source.Token);
        }

        private IEnumerable<SqlInstanceResult> FindInstancesUsingWMI(string computerName)
        {
            var names = new List<SqlInstanceResult>();
            string ns = string.Format(SERVICE_NS, computerName);
            var scope = new ManagementScope(ns);
            scope.Connect();
            var query = new ObjectQuery(SERVICE_WMI);
            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                using (ManagementObjectCollection col = searcher.Get())
                {
                    
                    foreach (ManagementObject mo in col)
                    {
                        using (mo)
                        {
                            string name = (string)mo[DISPLAY_NAME];
                            Match match = Regex.Match(name, SSREGEX);
                            if (match.Success)
                            {
                                string instName = match.Groups[1].Value;
                                names.Add(new SqlInstanceResult(computerName, instName));
                            }
                        }
                    }
                    return names;
                }
            }
        }

        private IEnumerable<SqlInstanceResult> FindInstancesUsingRegistry(string computerName)
        {
            var results = new List<SqlInstanceResult>();
            using (var registry = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, computerName))
            {
                using (RegistryKey key = registry.OpenSubKey(REG_KEY, false))
                {
                    if (key != null)
                    {
                        string[] instNames = key.GetValueNames();
                        for (int i = 0; i < instNames.Length; i++)
                        {
                            results.Add(new SqlInstanceResult(
                                computerName,
                                instNames[i]
                            ));
                        }
                        return results;
                    }
                    else
                        return null;
                }
            }
        }

        #endregion
    }

    public class SqlInstanceResult
    {
        public string ServerName { get; }
        public string InstanceName { get; }

        internal SqlInstanceResult(string serverName, string instance)
        {
            this.ServerName = serverName;
            this.InstanceName = instance;
        }
    }
}