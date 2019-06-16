using MG.Sql.Smo.Exceptions;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MG.Sql.Smo.PowerShell.Backend
{
    public static class SmoDatabase
    {
        #region FIELDS/CONSTANTS
        public const string LOG_NAME_FORMAT = "{0}\\{1}_Log.ldf";
        public const string MODEL = "model";
        public const string MODELDEV = MODEL + "dev";
        public const string MODELLOG = MODEL + "log";
        public const string PRIMARY = "PRIMARY";
        public const string PRIMARY_FILE_FORMAT = "{0}\\{1}_" + PRIMARY + ".mdf";
        //public const string SECONDARY = ""

        #endregion

        #region PROPERTIES


        #endregion

        #region PUBLIC METHODS
        public static string GetDefaultDatabasePath()
        {
            return Path.GetDirectoryName(SmoContext.Connection.DefaultFile);
        }
        public static string GetDefaultLogPath()
        {
            return Path.GetDirectoryName(SmoContext.Connection.DefaultLog);
        }

        public static KeyValuePair<double, FileGrowthType> GetModelDBAutoGrowth()
        {
            DataFile modelDev = SmoContext.Connection.Databases[MODEL].FileGroups[PRIMARY].Files[MODELDEV];
            return new KeyValuePair<double, FileGrowthType>(modelDev.Growth, modelDev.GrowthType);
        }
        public static double GetModelDBInitialSize()
        {
            return SmoContext.Connection.Databases[MODEL].FileGroups[PRIMARY].Files[MODELDEV].Size;
        }
        public static double GetModelDBMaxSize()
        {
            return SmoContext.Connection.Databases[MODEL].FileGroups[PRIMARY].Files[MODELDEV].MaxSize;
        }
        public static KeyValuePair<double, FileGrowthType> GetModelLogAutoGrowth()
        {
            LogFile log = SmoContext.Connection.Databases[MODEL].LogFiles[MODELLOG];
            return new KeyValuePair<double, FileGrowthType>(log.Growth, log.GrowthType);
        }
        public static double GetModelLogMaxSize()
        {
            return SmoContext.Connection.Databases[MODEL].LogFiles[MODELLOG].MaxSize;
        }
        public static double GetModelLogSize()
        {
            return SmoContext.Connection.Databases[MODEL].LogFiles[MODELLOG].Size;
        }

        public static Database NewDatabaseObject(string dbName, string collation, RecoveryModel recoveryModel)
        {
            var db = new Database(SmoContext.Connection, dbName, SmoContext.Connection.DatabaseEngineEdition)
            {
                Collation = collation,
                RecoveryModel = recoveryModel,
            };
            return db;
        }

        public static void NewLogFile(ref Database db, string logFile, double? logSize = null, double? logGrowth = null, FileGrowthType? growthType = null, double? maxSize = null)
        {
            if (!logSize.HasValue)
                logSize = GetModelLogSize();

            if (!logGrowth.HasValue || !growthType.HasValue)
            {
                KeyValuePair<double, FileGrowthType> ag = GetModelLogAutoGrowth();
                logGrowth = ag.Key;
                growthType = ag.Value;
            }
            var lf = new LogFile(db, Path.GetFileNameWithoutExtension(logFile))
            {
                FileName = logFile,
                Growth = logGrowth.Value,
                GrowthType = growthType.Value,
                Size = logSize.Value
            };
            if (maxSize.HasValue)
                lf.MaxSize = maxSize.Value;

            db.LogFiles.Add(lf);
        }

        public static void NewPrimaryFile(ref FileGroup fileGroup, string dataFolderPath, double? initialSize = null, double? fileGrowth = null, FileGrowthType? growthType = null, double? maxSize = null)
        {
            string fileName = string.Format(PRIMARY_FILE_FORMAT, dataFolderPath, fileGroup.Parent.Name);
            if (!fileGrowth.HasValue || !growthType.HasValue)
            {
                KeyValuePair<double, FileGrowthType> autoGrowth = GetModelDBAutoGrowth();
                fileGrowth = autoGrowth.Key;
                growthType = autoGrowth.Value;
            }
            if (!initialSize.HasValue)
                initialSize = GetModelDBInitialSize();

            if (!maxSize.HasValue)
                maxSize = GetModelDBMaxSize();

            fileGroup.Files.Add(new DataFile(fileGroup, Path.GetFileNameWithoutExtension(fileName))
            {
                FileName = fileName,
                IsPrimaryFile = true,
                Growth = fileGrowth.Value,
                GrowthType = growthType.Value,
                MaxSize = maxSize.Value,
                Size = initialSize.Value
            });
        }

        public static FileGroup NewPrimaryFileGroup(ref Database newDb)
        {
            var primaryFG = new FileGroup(newDb, "PRIMARY");
            newDb.FileGroups.Add(primaryFG);
            return primaryFG;
        }

        public static void NewPrimaryLogFile(ref Database db, string logFileFolder, double? logSize = null, double? logGrowth = null, FileGrowthType? growthType = null)
        {
            string fileName = string.Format(LOG_NAME_FORMAT, logFileFolder, db.Name);
            NewLogFile(ref db, fileName, logSize, logGrowth, growthType);
        }

        public static void SetOwner(ref Database db, string owner) => db.SetOwner(owner);

        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }
}