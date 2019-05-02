using Microsoft.ActiveDirectory.Management;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsCommon.Find, "SmoSqlInstance", ConfirmImpact = ConfirmImpact.None,
        DefaultParameterSetName = "SpecifyComputerName")]
    [OutputType(typeof(SqlInstanceResult))]
    public class FindSmoSqlInstance : ProgressCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS
        private const string COMPUTERNAME = "COMPUTERNAME";
        private const string DISPLAY_NAME = "DisplayName";
        private const string DNS_HOSTNAME = "DNSHostName";
        private const string WMI = "WMI";
        private const string REG = "Registry";
        private const string BROWSER = "SQLBrowser";
        private const string REGANDWMI = "RegistryAndWMI";
        private const string UDP_RESP_REGEX = @"(ServerName;(\w+);InstanceName;(\w+);IsClustered;(\w+);Version;(\d+\.\d+\.\d+\.\d+);(tcp;(\d+)){0,1})";
        private const string SSREGEX = @"^SQL\sServer\s\((.{1,})\)$";
        private const string SERVICE_WMI = "SELECT Name, DisplayName FROM Win32_Service WHERE " + DISPLAY_NAME + " LIKE 'SQL Server (%)'";
        private const string SERVICE_NS = "\\\\{0}\\root\\cimv2";
        private const string REG_KEY = "SOFTWARE\\Microsoft\\Microsoft SQL Server\\Instance Names\\SQL";
        private const string ACTIVITY = "Searching for SQL Instances";
        private const string STATUS_FORMAT = "Querying object {0}/{1}...";
        private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.Instance;
        private const string pName = "BrowserTimeoutInSecs";
        private static readonly byte[] PACKET = new byte[] { 0x03 };
        private const int PORT = 1434;
        private const int THREE = 3000;
        private const int THOUSAND = 1000;
        private const int ZERO = 0;
        private static readonly Type pType = typeof(int);
        private static readonly Collection<Attribute> attCol = new Collection<Attribute>
        {
            new ParameterAttribute
            {
                Mandatory = false
            }
        };
        private List<string> Names;
        protected override ICollection<string> Items => Names;
        protected override string Activity => ACTIVITY;
        protected override string StatusFormat => STATUS_FORMAT;
        private List<Task<IEnumerable<SqlInstanceResult>>> Tasks;
        private int TotalCount;
        private RuntimeDefinedParameterDictionary rtDict;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = false, ParameterSetName = "SpecifyComputerName", ValueFromPipeline = true, Position = 0)]
        public string ComputerName = Environment.GetEnvironmentVariable(COMPUTERNAME);

        [Parameter(Mandatory = true, ParameterSetName = "ByADComputerPipeline", DontShow = true, ValueFromPipeline = true)]
        public ADComputer InputObject { get; set; }

        [Parameter(Mandatory = false)]
        [SupportsWildcards]
        public string Name { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        [ValidateSet(REGANDWMI, REG, WMI, BROWSER)]
        public string SearchMethod = WMI;

        [Parameter(Mandatory = false)]
        public PSCredential Credential { get; set; }

        #endregion

        #region CMDLET PROCESSING
        public object GetDynamicParameters()
        {
            if (SmoContext.IsSet && SmoContext.IsConnected && SearchMethod.Equals(BROWSER))
            {
                if (rtDict == null)
                {
                    rtDict = this.GetRTDictionary();
                }
                return rtDict;
            }
            else
                return null;
        }

        protected override void BeginProcessing()
        {
            Names = new List<string>();
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
            if (SearchMethod != BROWSER)
                Tasks.Add(this.QueryAsync(ComputerName));

            else
                Names.Add(ComputerName);
        }

        protected override void EndProcessing()
        {
            if (SearchMethod != BROWSER)
            {
                TotalCount = Tasks.Count;

                while (Tasks.Count > 0)
                {
                    this.UpdateProgressAsync(0, Tasks.Count);
                    for (int cn = Tasks.Count - 1; cn >= 0; cn--)
                    {
                        Task<IEnumerable<SqlInstanceResult>> t = Tasks[cn];

                        if (t.IsCompleted)
                        {
                            if (!t.IsCanceled && t.Result != null)
                            {
                                IEnumerable<SqlInstanceResult> outRes = null;
                                if (!string.IsNullOrEmpty(Name))
                                {
                                    WildcardPattern wc = this.GetWildcard(Name);
                                    outRes = this.FilterByNameParameter(t.Result, wc);
                                }
                                else
                                    outRes = t.Result;

                                WriteObject(outRes, true);
                            }

                            Tasks.Remove(t);
                        }
                    }
                    Thread.Sleep(1000);
                }
                this.UpdateProgress(0);
            }
            else
            {
                using (var udpClient = new UdpClient())
                {
                    int to = THREE;
                    if (rtDict[pName].Value != null)
                        to = Convert.ToInt32(rtDict[pName].Value) * THOUSAND;

                    udpClient.Client.ReceiveTimeout = to;
                    var results = this.SqlBrowserQuery(Names, udpClient);
                    WriteObject(results, true);
                    this.UpdateProgress(0);
                }
            }
        }

        #endregion

        #region CMDLET METHODS
        private void UpdateProgressAsync(int id, int on)
        {
            int realOn = TotalCount - on;
            var progressRecord = new ProgressRecord(id, ACTIVITY, string.Format(
                STATUS_FORMAT, realOn, TotalCount));
            double num = Math.Round(realOn / (double)TotalCount * HUNDRED, ROUND_DIGITS, MIDPOINT);
            progressRecord.PercentComplete = Convert.ToInt32(num);
            WriteProgress(progressRecord);
        }

        private async Task<IEnumerable<SqlInstanceResult>> QueryAsync(string computerName)
        {
            IEnumerable<SqlInstanceResult> results = null;
            if (SearchMethod != BROWSER)
            {
                var source = new CancellationTokenSource(7000);
                results = await Task.Run(() =>
                {
                    bool retry = false;
                    IEnumerable<SqlInstanceResult> reses = null;
                    if (SearchMethod == REGANDWMI || SearchMethod == WMI)
                    {
                        try
                        {
                            reses = this.FindInstancesUsingWMI(computerName);
                        }
                        catch (COMException)
                        {
                            retry = true;
                        }
                    }
                    if (SearchMethod == REG || (retry && SearchMethod == REGANDWMI))
                    {
                        try
                        {
                            reses = this.FindInstancesUsingRegistry(computerName);
                        }
                        catch (IOException) { }
                    }
                    return reses;
                }, source.Token);
            }
            return results;
        }

        private List<SqlInstanceResult> SqlBrowserQuery(List<string> names, UdpClient udpClient)
        {
            var list = new List<SqlInstanceResult>();
            for (int i = 1; i <= names.Count; i++)
            {
                this.UpdateProgress(0, i);
                IEnumerable<SqlInstanceResult> result = this.FindBySQLBrowser(names[i-1], udpClient);
                if (result != null)
                    list.AddRange(result);
            }
            return list;
        }

        private IEnumerable<SqlInstanceResult> FindBySQLBrowser(string computerName, UdpClient udpClient)
        {
            var ipEndpoint = new IPEndPoint(IPAddress.Any, ZERO);
            udpClient.Client.Blocking = true;
            byte[] received = null;
            try
            {
                udpClient.Connect(computerName, PORT);
                udpClient.Send(PACKET, PACKET.Length);
                received = udpClient.Receive(ref ipEndpoint);
            }
            catch (SocketException)
            {
                return null;
            }

            string response = Encoding.ASCII.GetString(received);
            MatchCollection matches = Regex.Matches(response, UDP_RESP_REGEX);
            var list = new List<SqlInstanceResult>(matches.Count);
            foreach (Match match in matches)
            {
                list.Add(new SqlInstanceResult(computerName, match.Groups[3].Value));
            }
            return list;
        }

        private IEnumerable<SqlInstanceResult> FindInstancesUsingWMI(string computerName)
        {
            var names = new List<SqlInstanceResult>();
            string ns = string.Format(SERVICE_NS, computerName);
            var co = new ConnectionOptions();
            if (this.MyInvocation.BoundParameters.ContainsKey("Credential"))
            {
                co.Username = Credential.UserName;
                co.SecurePassword = Credential.Password;
            }
            var scope = new ManagementScope(ns, co);
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

        private IEnumerable<SqlInstanceResult> FilterByNameParameter(IEnumerable<SqlInstanceResult> results, WildcardPattern wc)
        {
            var list = results.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                SqlInstanceResult s = list[i];
                if (!wc.IsMatch(s.InstanceName))
                    list.Remove(s);
            }
            return list;
        }

        private WildcardPattern GetWildcard(string name) => new WildcardPattern(name, WildcardOptions.IgnoreCase);

        private RuntimeDefinedParameterDictionary GetRTDictionary()
        {
            var rtDict = new RuntimeDefinedParameterDictionary
            {
                { pName, new RuntimeDefinedParameter(pName, pType, attCol) }
            };
            return rtDict;
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