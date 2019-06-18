using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MG.Sql.Smo
{
    public class SqlProcessCollection : DataItemCollection<SqlProcess>
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES
        private Server Server { get; set; }

        #endregion

        #region CONSTRUCTORS
        private SqlProcessCollection(IEnumerable<SqlProcess> procs)
            : base(procs) { }

        private SqlProcessCollection(DataTable dt)
            : base(dt) { }

        #endregion

        #region PUBLIC METHODS
        public static explicit operator SqlProcessCollection(List<SqlProcess> list) => new SqlProcessCollection(list);

        public static SqlProcess GetProcess(int processId, Server server)
        {
            DataTable dt = server.EnumProcesses(processId);
            return new SqlProcess(dt.Rows[0]);
        }
        public static SqlProcessCollection GetProcesses(string loginName, Server server)
        {
            DataTable dt = server.EnumProcesses(loginName);
            return new SqlProcessCollection(dt)
            {
                Server = server
            };
        }
        public static SqlProcessCollection GetProcesses(Server server)
        {
            DataTable dt = server.EnumProcesses(true);
            return new SqlProcessCollection(dt)
            {
                Server = server
            };
        }

        public void KillAll()
        {
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    this[i].Kill(this.Server);
                }
                catch (Exception e)
                {
                    this.ThrowInnerException(e);
                }
            }
            this.Server.Refresh();
        }
        public void KillById(int spid)
        {
            try
            {
                this.Server.KillProcess(spid);
            }
            catch (Exception e)
            {
                this.ThrowInnerException(e);
            }
            this.Server.Refresh();
        }
        public void KillByProcess(SqlProcess proc)
        {
            try
            {
                this.Server.KillProcess(proc.Spid);
            }
            catch (Exception e)
            {
                this.ThrowInnerException(e);
            }
            this.Server.Refresh();
        }
        public void KillAllThatMatch(Predicate<SqlProcess> match)
        {
            IFindable<SqlProcess> procs = base.FindAll(match);
            for (int i = 0; i < procs.Count; i++)
            {
                try
                {
                    procs[i].Kill(this.Server);
                }
                catch (Exception e)
                {
                    this.ThrowInnerException(e);
                }
            }
            this.Server.Refresh();
        }

        public void SortByLogin() => _list.Sort(new SqlProcessLoginComparer());
        public void SortByProgram() => _list.Sort(new SqlProcessProgramComparer());
        public void SortBySpid(bool ascending)
        {
            if (!ascending)
                _list.Sort(new SqlProcessIdComparer());

            else
                _list.Sort(new SqlProcessIdAscendingComparer());
        }

        private void ThrowInnerException(Exception e)
        {
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }
            throw e;
        }

        #endregion

        #region COMPARERS
        private class SqlProcessIdComparer : IComparer<SqlProcess>
        {
            public int Compare(SqlProcess x, SqlProcess y) => x.Spid.CompareTo(y.Spid);
        }
        private class SqlProcessIdAscendingComparer : IComparer<SqlProcess>
        {
            public int Compare(SqlProcess x, SqlProcess y) => x.Spid.CompareTo(y.Spid) * NEG;
        }
        private class SqlProcessProgramComparer : IComparer<SqlProcess>
        {
            public int Compare(SqlProcess x, SqlProcess y)
            {
                return string.IsNullOrEmpty(x.Program) && !string.IsNullOrEmpty(y.Program)
                    ? NEG
                    : !string.IsNullOrEmpty(x.Program) && string.IsNullOrEmpty(y.Program)
                        ? 1
                        : x.Program.CompareTo(y.Program);
            }
        }
        private class SqlProcessLoginComparer : IComparer<SqlProcess>
        {
            public int Compare(SqlProcess x, SqlProcess y)
            {
                return string.IsNullOrEmpty(x.Login) && !string.IsNullOrEmpty(y.Login)
                    ? NEG
                    : !string.IsNullOrEmpty(x.Login) && string.IsNullOrEmpty(y.Login)
                        ? 1
                        : x.Login.CompareTo(y.Login);
            }
        }

        #endregion
    }
}