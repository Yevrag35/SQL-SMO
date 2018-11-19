using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Cmdlets;
using System;

namespace SQL.SMO.Framework
{
    public class SMOConnection : ISMOWrapper
    {
        private Server _srv;

        public string Name => _srv.NetName;
        public bool Connected => SharedCmdlet.CheckSession(true);
        public Version BuildNumber => new Version(_srv.VersionString);
        public DatabaseEngineEdition DatabaseEngineEdition => _srv.DatabaseEngineEdition;
        public int DatabaseCount => _srv.Databases.Count;
        public DatabaseEngineType DatabaseEngineType => _srv.DatabaseEngineType;
        public AvailabilityGroupCollection AvailabilityGroups => _srv.AvailabilityGroups;
        public bool TcpEnabled => _srv.TcpEnabled;
        public ServerStatus Status => _srv.Status;
        public string SQLEngineServiceAccount => _srv.ServiceAccount;
        public int ProcessorCount => _srv.Processors;
        public int PhysicalMemory => _srv.PhysicalMemory;
        public ServerLoginMode LoginMode => _srv.LoginMode;

        public Type OriginalType => typeof(Server);

        internal SMOConnection(Server server) => _srv = server;

        public object ShowOriginal() => _srv;
        public object Load(params string[] vwha) => throw new NotImplementedException();
    }
}
