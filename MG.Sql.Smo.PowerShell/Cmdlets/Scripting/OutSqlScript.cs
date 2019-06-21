using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace MG.Sql.Smo.PowerShell
{
    [Cmdlet(VerbsData.Out, "SqlScript", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class OutSqlScript : WriteToSqlScript, IDynamicParameters
    {
        #region PARAMETERS
        [Parameter(Mandatory = true, Position = 0)]
        [Alias("Path")]
        [ValidateFilePath]
        public string FilePath { get; set; }

        #endregion

        #region CMDLET PROCESSING
        public override object GetDynamicParameters() => base.GetDynamicParameters();

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            _so.FileName = this.FilePath;
            _so.ToFileOnly = true;
            StringCollection script = this.InputObject.Script(_so);
        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }

    public class ValidateFilePathAttribute : ValidateArgumentsAttribute
    {
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            string[] acceptable = new string[2] { ".sql", ".txt" };

            if (arguments is string str)
            {
                if (!acceptable.Contains(Path.GetExtension(str)))
                    throw new ArgumentException("FilePath must end in \".sql\" or \".txt\"");

                else if (!Directory.Exists(Path.GetDirectoryName(str)))
                    throw new ArgumentException("The specified folder path does not exist.");
            }
            else
                throw new ArgumentException("FilePath must be a string.");
        }
    }
}