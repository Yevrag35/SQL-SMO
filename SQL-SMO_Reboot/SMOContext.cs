﻿using Microsoft.SqlServer.Management.Smo;
using System;
using System.Linq;
using System.Reflection;

namespace MG.Sql
{
    public static class SMOContext
    {
        public static Server Connection { get; private set; }

        public static void AddConnection(Server connection, bool force)
        {
            if (force || (!force && !IsSet))
            {
                if (IsSet)
                    Disconnect();

                Connection = connection;
            }
            else
                throw new SMOContextAlreadySetException();
        }

        public static bool IsSet => Connection != null;
        public static bool IsConnected => Connection.ConnectionContext.IsOpen;

        public static void Disconnect() => Connection.ConnectionContext.Disconnect();


    }
}
