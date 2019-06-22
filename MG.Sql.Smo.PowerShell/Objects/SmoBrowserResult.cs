using System;

namespace MG.Sql.Smo.PowerShell
{
    public class SmoBrowserResult
    {
        public string ServerName { get; }
        public string InstanceName { get; }
        public Version Version { get; }
        public bool IsClustered { get; }

        internal SmoBrowserResult(string server, string instance, string version, string isClus)
        {
            this.ServerName = server;
            this.InstanceName = instance;
            if (!string.IsNullOrEmpty(version) && Version.TryParse(version, out Version vers))
                this.Version = vers;

            this.IsClustered = isClus.Equals("Yes", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
