using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo.UnitTests
{
    [TestClass]
    public class OtherTests
    {
        private const string TEST_TABLE = "tempTableEquality";
        private const string TEST_TABLE_2 = TEST_TABLE + "2";


        [TestMethod]
        public void DatabaseEqualityComparers()
        {
            Context.DBValidate();

            var dbCol = Context.Databases;
            Assert.IsInstanceOfType(dbCol, typeof(DatabaseCollection));
            if (dbCol.Count < 2)
                Assert.Inconclusive("There need to be at least 2 databases to test for database equality.");

            #region DATABASE EQUALITY TEST

            var db1 = dbCol[0];
            var db2 = dbCol[1];

            var dbEquality = new DatabaseEquality();
            Assert.IsNotNull(dbEquality);

            bool thisShouldBeTrue = dbEquality.Equals(db1, db1);
            Assert.IsTrue(thisShouldBeTrue);

            bool thisShouldBeFalse = dbEquality.Equals(db1, db2);
            Assert.IsFalse(thisShouldBeFalse);

            #endregion
        }

        [TestMethod]
        public void TableEqualityComparers()
        {
            Context.DBValidate();

            var dbCol = Context.Databases.FindAll(x => x.Tables.Count >= 2);
            if (dbCol.Count < 2)
                Assert.Inconclusive("There need to be at least 2 databases with at least 2 tables to test for table equality");

            #region TABLE EQUALITY TEST
            var db1 = dbCol[0];
            var db2 = dbCol[1];

            var tb1 = db1.Tables[0];
            var tb2 = db1.Tables[1];
            var tb3 = db2.Tables[0];
            var tb4 = db2.Tables[1];

            var tabEquality = new TableEquality();
            Assert.IsNotNull(tabEquality);

            Assert.IsTrue(tabEquality.Equals(tb1, tb1));
            Assert.IsFalse(tabEquality.Equals(tb1, tb2));
            Assert.IsTrue(tabEquality.Equals(tb3, tb3));
            Assert.IsFalse(tabEquality.Equals(tb3, tb4));

            Assert.IsFalse(tabEquality.Equals(tb1, tb3));
            Assert.IsFalse(tabEquality.Equals(tb2, tb4));

            var nt1 = new Table(db1, TEST_TABLE);
            var nt2 = new Table(db2, TEST_TABLE);
            Assert.IsFalse(tabEquality.Equals(nt1, nt2));

            #endregion
        }

        [TestMethod]
        public void ColumnEqualityComparers()
        {
            Context.DBValidate();
            var dbCol = Context.Databases.FindAll(x => x.Tables.Count >= 2);

            var tbCol = new List<Table>(3);
            bool finalBreak = false;
            for (int i = 0; i < dbCol.Count; i++)
            {
                Database db = dbCol[i];
                if (!finalBreak)
                {
                    for (int t = 0; t < db.Tables.Count; t++)
                    {
                        Table tbl = db.Tables[t];
                        if (tbl.Columns.Count >= 2)
                        {
                            tbCol.Add(tbl);
                        }
                        if (tbCol.Count == 2)
                            break;

                        else if (tbCol.Count == 3)
                        {
                            finalBreak = true;
                            break;
                        }
                    }
                }
                else
                    break;
            }

            if (tbCol.Count < 3)
                Assert.Inconclusive("There must be at least 2 databases with at least 2 tables and at least 2 columns in each table to test for column equality.");

            var col1 = tbCol[0].Columns[0];
            var col2 = tbCol[0].Columns[1];
            var col3 = tbCol[1].Columns[0];
            var col4 = tbCol[2].Columns[0];

            var colEquality = new ColumnEquality();
            Assert.IsTrue(colEquality.Equals(col1, col1));
            Assert.IsFalse(colEquality.Equals(col1, col2));
            Assert.IsFalse(colEquality.Equals(col3, col4));
        }
    }
}
