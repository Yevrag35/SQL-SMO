using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell
{
    internal static class SMOContext
    {
        private static Server _con;
        internal static Server Connection
        {
            get => _con;
            set
            {
                _con = value;
                DatabaseNames = _con.Databases.Cast<Database>().Select(x => x.Name).ToArray();
                JobNames = _con.JobServer.Jobs.Cast<Job>().Select(x => x.Name).ToArray();
            }
        }

        internal static string[] DatabaseNames { get; private set; }
        internal static string[] JobNames { get; private set; }

        internal static void AddConnection(Server connection, bool force)
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

        internal static bool IsSet => Connection != null;
        internal static bool IsConnected => Connection.ConnectionContext.IsOpen;

        internal static void Disconnect()
        {
            Connection.ConnectionContext.Disconnect();
            Connection = null;
            GC.Collect();
        }
    }
}
