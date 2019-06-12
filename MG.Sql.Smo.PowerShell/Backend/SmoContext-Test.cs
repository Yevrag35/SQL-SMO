using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MG.Sql.Smo.PowerShell
{
    public static class SmoContext
    {
        private static Server _con;
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
        public static string[] JobNames { get; private set; }
        public static List<string> ServerCollations { get; private set; }

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

        public static bool IsSet => Connection != null;
        public static bool IsConnected => Connection.ConnectionContext.IsOpen;

        public static void Disconnect()
        {
            Connection.ConnectionContext.Disconnect();
            //Connection = null;
            GC.Collect();
        }

        public class CaseInsensitiveComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => x.Equals(y, StringComparison.CurrentCultureIgnoreCase);
            public int GetHashCode(string obj) => obj.GetHashCode();
        }
    }


}

