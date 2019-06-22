using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.Sql.Smo.PowerShell
{
    public class ProductRegistry
    {
        #region FIELDS/CONSTANTS
        internal const string DIGITALID = "DigitalProductID";
        internal const string EDITION = "Edition";
        internal const string PATCH_LEVEL = "PatchLevel";
        internal const string PRODUCT_ID = "ProductID";
        internal const string VERSION = "Version";

        #endregion

        #region PROPERTIES
        public byte[] DigitalProductId { get; }
        public DatabaseEngineEdition Edition { get; }
        public string Instance { get; }
        public string ProductId { get; }
        public Version Version { get; }

        #endregion

        #region CONSTRUCTORS
        internal ProductRegistry(string instance, RegistryKey regKey)
        {
            this.DigitalProductId = regKey.GetValue(DIGITALID, null) as byte[];
            this.Edition = (DatabaseEngineEdition) Enum.Parse(typeof(DatabaseEngineEdition), ((string)regKey.GetValue(EDITION, null)).Replace(" Edition", string.Empty));
            this.Instance = instance;
            this.ProductId = regKey.GetValue(PRODUCT_ID, null) as string;
            if (Version.TryParse(regKey.GetValue(VERSION) as string, out Version vers))
            {
                this.Version = vers;
            }
        }

        #endregion

        #region PUBLIC METHODS


        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }
}