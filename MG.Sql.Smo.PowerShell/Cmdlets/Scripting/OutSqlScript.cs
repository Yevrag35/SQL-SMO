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

        protected override void BeginProcessing()
        {
            string parentDir = Path.GetDirectoryName(this.FilePath);

            if (string.IsNullOrEmpty(parentDir))
                this.FilePath = Path.Combine(this.SessionState.Path.CurrentLocation.Path, this.FilePath);
            
            else if (parentDir.Equals("."))
            {
                string fileName = Path.GetFileName(this.FilePath);
                this.FilePath = Path.Combine(this.SessionState.Path.CurrentLocation.Path, fileName);
            }
            else if (!Directory.Exists(parentDir))
            {
                string msg = string.Format("The specified folder path \"{0}\" does not exist", parentDir);
                throw new ArgumentException(msg);
            }

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            _so.FileName = this.FilePath;
            _so.ToFileOnly = true;
            this.InputObject.Script(_so);
            NoEnd = true;
        }

        #endregion
    }

    public class ValidateFilePathAttribute : ValidateArgumentsAttribute
    {
        private static readonly string[] acceptable = new string[2] { ".sql", ".txt" };

        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (arguments is string str)
            {
                if (!acceptable.Contains(Path.GetExtension(str)))
                    throw new ArgumentException("FilePath must end in \".sql\" or \".txt\"");
            }
            else
                throw new ArgumentException("FilePath must be a string.");
        }
    }
}