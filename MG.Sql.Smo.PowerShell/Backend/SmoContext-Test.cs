using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Text.RegularExpressions;

namespace MG.Sql.Smo.PowerShell
{
    public static class SmoContext
    {
        #region PRIVATE FIELDS/CONSTANTS
        private static Server _con;
        private static readonly string[] PropsToLoad = new string[2]
        {
            DNS_ROOT, NB_NAME
        };

        private const string DEF_NAM_CTX = "defaultNamingContext";
        private const string DN = "distinguishedName";
        private const string DNS_ROOT = "dnsRoot";
        private const string DOM_DN_REGEX = @"(?:\,DC\=(?<" + NON_DC + @">\w{1,}))";
        private const string BAD_AD_MSG = "Cannot determine the NetBIOS name of the AD object automatically.  Piping AD objects to SQL-SMO cmdlets from non-AD joined machines is not supported.";
        private const string LDAP = "LDAP://";
        private const string NB_FILTER = "(&(" + NB_NAME + "=*)(" + DNS_ROOT + "={0}))";
        private const string NB_NAME = "nETBIOSName";
        private const string NON_DC = "nondc";
        private const string OBJ_CLS = "objectClass";
        private const string PART_SEARCH_FILTER = "(&(objectClass=crossRefContainer))";
        private const string ROOTDSE = "rootDSE";
        private const string SAM_FILTER = "sAMAccountName={0}";

        #endregion

        #region PUBLIC FIELDS
        public static Server Connection
        {
            get => _con;
            set
            {
                _con = value;
                DatabaseNames = _con.Databases.Cast<Database>().Select(x => x.Name).ToArray();
                JobNames = _con.JobServer.Jobs.Cast<Job>().Select(x => x.Name).ToArray();
                DataTable dt = _con.EnumCollations();
                DataColumn dc = dt.Columns[BaseSqlCmdlet.NAME];
                ServerCollations = new List<string>(dt.Rows.Cast<DataRow>().Select(x => x.Field<string>(dc)));
            }
        }
        public static string[] DatabaseNames { get; private set; }
        public static bool IsSet => Connection != null;
        public static bool IsConnected => Connection.ConnectionContext.IsOpen;
        public static string[] JobNames { get; private set; }
        public static List<string> ServerCollations { get; private set; }

        #endregion

        #region METHODS

        public static void AddConnection(Server connection, bool force)
        {
            if (force || (!force && !IsSet))
            {
                if (IsSet)
                    Disconnect();

                Connection = connection;
            }
            else
                throw new SmoContextAlreadySetException();
        }
        public static void Disconnect()
        {
            Connection.ConnectionContext.Disconnect();
            //Connection = null;
            GC.Collect();
        }

        public static LoginType? FindADLoginFromObjectClass(string className)
        {
            switch (className)
            {
                case "user":
                    return LoginType.WindowsUser;

                case "computer":
                    return LoginType.WindowsUser;

                case "group":
                    return LoginType.WindowsGroup;

                default:
                    return null;
            }
        }
        public static string FindADObjectClassFromSamAccountName(string sam, out string dn)
        {
            using (DirectoryEntry domDe = GetCurrentDomainRoot())
            {
                string filter = string.Format(SAM_FILTER, sam);
                string[] props = new string[2]
                {
                    DN, OBJ_CLS
                };
                using (var searcher = new DirectorySearcher(domDe, filter, props, SearchScope.Subtree))
                {
                    SearchResult result = searcher.FindOne();
                    string cls = null;
                    dn = null;
                    if (result != null)
                    {
                        cls = result.Properties[OBJ_CLS].OfType<string>().LastOrDefault();
                        dn = result.Properties[DN].OfType<string>().FirstOrDefault(x => !x.Equals(string.Empty));
                    }
                    return cls;
                }
            }
        }

        public static DirectoryEntry GetConfigurationContext(DirectoryEntry rootDSE)
        {
            return new DirectoryEntry(
                string.Format(
                    "{0}{1}",
                    LDAP,
                    rootDSE.Properties["configurationNamingContext"].Value
                )
            );
        }

        public static DirectoryEntry GetCurrentDomainRoot()
        {
            using (DirectoryEntry rootDse = GetRootDSE())
            {
                string domStr = string.Format("{0}{1}", LDAP, rootDse.Properties[DEF_NAM_CTX].Value);
                return new DirectoryEntry(domStr);
            }
        }

        public static string GetNetBiosFromDn(string distinguishedName)
        {
            using (DirectoryEntry rootDse = GetRootDSEFromDN(distinguishedName, out string domainFqdn))
            {
                using (DirectoryEntry configDe = GetConfigurationContext(rootDse))
                {
                    using (DirectoryEntry partsDe = GetPartitionsEntry(configDe))
                    {
                        string filter = string.Format(NB_FILTER, domainFqdn);
                        using (var searcher = new DirectorySearcher(partsDe, filter, PropsToLoad, SearchScope.Subtree))
                        {
                            string nbName = null;
                            SearchResult result = searcher.FindOne();
                            if (result != null && result.Properties.Contains(NB_NAME))
                                nbName = result.Properties[NB_NAME].OfType<string>().FirstOrDefault(x => x != string.Empty);

                            return nbName;
                        }
                    }
                }
            }
        }

        public static DirectoryEntry GetPartitionsEntry(DirectoryEntry configDN)
        {
            string[] props = new string[1] { DN };
            using (var ConfigSearcher = new DirectorySearcher(configDN, PART_SEARCH_FILTER, props, SearchScope.OneLevel))
            {
                SearchResult result = ConfigSearcher.FindOne();
                return result.GetDirectoryEntry();
            }
        }

        public static DirectoryEntry GetRootDSE() => new DirectoryEntry(LDAP + ROOTDSE);

        public static DirectoryEntry GetRootDSEFromDN(string distinguishedName, out string DomainFQDN)
        {
            DomainFQDN = null;
            MatchCollection domMatches = Regex.Matches(distinguishedName, DOM_DN_REGEX, RegexOptions.IgnoreCase);
            if (domMatches.Count > 0)
            {
                DomainFQDN = string.Join(".", new List<string>(domMatches.OfType<Match>().Select(x => x.Groups[NON_DC].Value)));
                string rootDseStr = string.Format(LDAP + "{0}/" + ROOTDSE, DomainFQDN);
                try
                {
                    var rootDse = new DirectoryEntry(rootDseStr);
                    rootDse.RefreshCache();
                    return rootDse;
                }
                catch
                {
                    throw new InvalidOperationException(BAD_AD_MSG);
                }
            }
            else
                throw new ArgumentException(distinguishedName + " is not a valid distinguishedName.");
        }

        #endregion

        #region STRING COMPARER
        public class CaseInsensitiveComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => x.Equals(y, StringComparison.CurrentCultureIgnoreCase);
            public int GetHashCode(string obj) => obj.GetHashCode();
        }

        #endregion
    }
}

