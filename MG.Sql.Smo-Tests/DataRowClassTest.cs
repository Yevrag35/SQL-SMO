using MG.Sql.Smo;
using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo.UnitTests
{
    [TestClass]
    public class DataRowClassTest
    {
        private const string TEST_COLLATION = "SQL_Latin1_General_CP1_CI_AS";
        private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.Instance;
        private static readonly Type DRTYPE = typeof(DataRow);
        private static readonly Type DTTYPE = typeof(DataTable);

        [TestMethod]
        public void TestCollation()
        {
            Type colType = typeof(Collation);
            Type colCol = typeof(CollationCollection);

            // Test constructors
            ConstructorInfo[] constrs = colType.GetConstructors();
            Assert.IsTrue(constrs.Length > 0 &&
                constrs.Any(
                    x => x.GetParameters().First(
                        p => p.ParameterType.Equals(DRTYPE)) != null));

            // Test properties can be set.
            Assert.IsTrue(colType.GetProperties(FLAGS).All(x => x.CanWrite));

            Context.Validate();

            var cols = CollationCollection.GetCollations(Context.Connection);
            Assert.IsNotNull(cols);
            Assert.IsInstanceOfType(cols[0], colType);
            if (cols.Count > 1)
            {
                cols.SortByName();
                Assert.IsTrue(cols[0].Name.CompareTo(cols[1].Name) < 0);

                cols.SortByName(true);
                Assert.IsTrue(cols[0].Name.CompareTo(cols[1].Name) > 0);

                Assert.IsInstanceOfType(cols.Clone(), colCol);
                Assert.IsNotNull(cols.Find(x => x.Name.Equals(TEST_COLLATION, StringComparison.CurrentCultureIgnoreCase)));
                Assert.IsTrue(cols.FindAll(x => x.LocaleID.Equals(1033)).Count > 0);
                var newCol = cols.FindByLocaleID(1033);
                Assert.IsNotNull(newCol);
                Assert.IsTrue(newCol.Count > 0);
                Assert.IsInstanceOfType(newCol, colCol);
            }

            Assert.IsNotNull(CollationCollection.GetCollations(Context.Connection, CollationVersion.Version80));
            Assert.IsNotNull(CollationCollection.GetCollations(Context.Connection, 1033));
            Assert.IsNotNull(CollationCollection.GetCollations(Context.Connection, 1033, CollationVersion.Version80));
        }

        [TestMethod]
        public void TestErrorLog()
        {
            Type elType = typeof(ErrorLog);

            // Test constructors
            ConstructorInfo[] constrs = elType.GetConstructors();
            Assert.IsTrue(constrs.Length > 0 &&
                constrs.Any(
                    x => x.GetParameters().First(
                        p => p.ParameterType.Equals(DRTYPE)) != null));

            // Test properties can be set.
            Assert.IsTrue(elType.GetProperties(FLAGS).All(x => x.CanWrite));

            // Get test materials
            Context.Validate();
            var els = ErrorLog.GetErrorLogs(Context.Connection);

            Assert.IsNotNull(els);
            Assert.IsTrue(els.GetType().GetInterfaces().Contains(
                typeof(IEnumerable<ErrorLog>)));

        }

        [TestMethod]
        public void TestJobHistory()
        {
            Type jhcType = typeof(JobHistoryCollection);
            Type jhiType = typeof(JobHistoryItem);

            ConstructorInfo[] constrs = jhiType.GetConstructors();
            Assert.IsTrue(constrs.Length > 0 &&
                constrs.Any(
                    x => x.GetParameters().First(
                        p => p.ParameterType.Equals(DRTYPE)) != null));

            Assert.IsTrue(jhiType.BaseType.Equals(typeof(DataRowClass)));

            Assert.IsTrue(jhiType.GetProperties(FLAGS).All(x => x.CanWrite));

            Context.Validate();

            var jh = JobHistoryCollection.GetHistory(Context.Connection.JobServer);
            Assert.IsNotNull(jh);
            Assert.IsInstanceOfType(jh, jhcType);
            
            if (jh.Count > 0)
            {
                jh.SortByDate();
                for (int i = 0; (jh.Count == 1 && i < 1) || (jh.Count >= 2 && i < 2); i++)
                {
                    var jhi = jh[i];
                    Assert.IsInstanceOfType(jhi, jhiType);
                    //Assert.IsInstanceOfType(jhi, typeof(DataRowClass));
                }
                if (jh.Count > 1)
                {
                    Assert.IsTrue(jh[0].RunDate.CompareTo(jh[1].RunDate) <= 0);
                    jh.SortByDate(true);
                    Assert.IsTrue(jh[0].RunDate.CompareTo(jh[1].RunDate) >= 0);

                    jh.SortByJobAndServer();

                    var job1 = jh[0];
                    var job2 = jh[1];
                    if (job1.JobName == job2.JobName && jh.Count > 2)
                        job2 = jh[2];

                    Assert.IsTrue(job1.JobName.CompareTo(job2.JobName) <= 0);
                }
            }
        }
    }
}
