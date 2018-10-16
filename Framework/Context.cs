using Microsoft.SqlServer.Management.Smo;
using System;
using System.Linq;
using System.Reflection;

namespace SQL.SMO.Framework
{
    public static class Context
    {
        private static SqlSmoObject _context;

        public static SqlSmoObject Connection => _context;

        private static string[] _propNames = null;
        internal static string[] ConfigProperties => _propNames;

        private static string[] _dbnames;
        internal static string[] DBNames => _dbnames;

        internal static void GetConfigPropertyNames()
        {
            if (IsSet && IsConnected)
            {
                Microsoft.SqlServer.Management.Smo.Configuration cfg = ((Server)_context).Configuration;
                PropertyInfo[] props = cfg.GetType().GetProperties().Where(
                    x => x.Name != "Properties" && x.Name != "Parent"
                ).OrderBy(x => x.Name).ToArray();
                _propNames = new string[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    _propNames[i] = props[i].Name;
                }
            }
            else
            {
                throw new SMOContextNotSetException();
            }
        }

        internal static void GetDatabaseNames()
        {
            if (IsSet && IsConnected)
            {
                DatabaseCollection dbCol = ((Server)Connection).Databases;
                _dbnames = new string[dbCol.Count];
                for (int i = 0; i < dbCol.Count; i++)
                {
                    _dbnames[i] = dbCol[i].Name;
                }
            }
            else
            {
                throw new SMOContextNotSetException();
            }
        }

        internal static bool IsSet => _context != null;

        internal static bool IsConnected => _context != null ? 
            ((Server)_context).ConnectionContext.IsOpen : false;

        internal static void Disconnect(bool discconnect)
        {
            if (discconnect)
            {
                ((Server)_context).ConnectionContext.Disconnect();
                _context = null;
            }
            else
            {
                _context = null;
            }
        }

        internal static void AddConnection(SqlSmoObject connection, bool force = false)
        {
            if (force || (!force && !IsSet))
            {
                if (IsSet)
                {
                    Disconnect(true);
                }
                _context = connection;
            }
            else
            {
                throw new SMOContextAlreadySetException();
            }
        }
        
    }
}
