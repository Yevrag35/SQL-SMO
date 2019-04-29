using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data.SqlClient;

namespace MG.Sql.Smo
{
    public class SmoConfiguration
    {
        #region FIELDS/CONSTANTS
        private readonly Configuration _config;

        #endregion

        #region CONSTRUCTORS
        public SmoConfiguration(Configuration config) => _config = config;

        #endregion

        #region METHODS


        #endregion
    }
}