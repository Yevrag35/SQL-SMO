using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Cmdlets;
using System;

namespace SQL.SMO.Framework
{
    public class SMOConnection : ISMOObject
    {
        private Server _srv;

        public string Name { get { return _srv.NetName; } }
        public bool Connected { get { return SharedCmdlet.CheckSession(true); } }
        public Version BuildNumber { get { return new Version(_srv.VersionString); } }
        public DatabaseEngineEdition DatabaseEngineEdition { get { return _srv.DatabaseEngineEdition; } }
        public int DatabaseCount { get { return _srv.Databases.Count; } }
        public DatabaseEngineType DatabaseEngineType { get { return _srv.DatabaseEngineType; } }
        public AvailabilityGroupCollection AvailabilityGroups { get { return _srv.AvailabilityGroups; } }
        public bool TcpEnabled { get { return _srv.TcpEnabled; } }
        public ServerStatus Status { get { return _srv.Status; } }
        public string SQLEngineServiceAccount { get { return _srv.ServiceAccount; } }
        public int ProcessorCount { get { return _srv.Processors; } }
        public int PhysicalMemory { get { return _srv.PhysicalMemory; } }
        public ServerLoginMode LoginMode { get { return _srv.LoginMode; } }

        public Type MSType => typeof(Server);

        internal SMOConnection(Server server) => _srv = server;

        public object ShowOriginal() => _srv;
    }
}
