using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.Sql.Smo.PowerShell
{
    public class SmoLicense
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES
        public string ComputerName { get; }
        public string InstanceName { get; }
        public string ProductKey { get; }
        public DatabaseEngineEdition SqlEdition { get; }
        public SQLYearVersion SqlVersion { get; }
        public Version Version { get; }

        #endregion

        #region CONSTRUCTORS
        internal SmoLicense(string computerName, ProductRegistry prodReg, char[] charArray)
        {
            this.ProductKey = this.GetProductKey(prodReg.DigitalProductId, prodReg.Version.Major, charArray);
            this.ComputerName = computerName;
            this.InstanceName = prodReg.Instance;
            this.SqlEdition = prodReg.Edition;
            this.SqlVersion = FromVersion(prodReg.Version);
            this.Version = prodReg.Version;
        }

        #endregion

        #region BACKEND/PRIVATE METHODS
        private static SQLYearVersion FromVersion(Version vers) => (SQLYearVersion)vers.Major;

        private string GetProductKey(byte[] digitalProductId, int major, char[] charArray)
        {
            string prodKey = string.Empty;
            digitalProductId = major >= 11
                    ? digitalProductId.Take(67).ToArray()
                    : digitalProductId.Skip(52).Take(15).ToArray();

            for (int i = 24; i >= 0; i--)
            {
                int k = 0;
                for (int j = 14; j >= 0; j--)
                {
                    k = k * 256 ^ digitalProductId[j];
                    digitalProductId[j] = Convert.ToByte(Math.Truncate((double)k / 24));
                    k = k % 24;
                }
                prodKey = charArray[k] + prodKey;
                if (i % 5 == 0 && i != 0)
                {
                    prodKey = "-" + prodKey;
                }
            }
            return prodKey;
        }

        #endregion
    }
}