using Microsoft.SqlServer.Management.Smo;
using SQL.SMO.Framework;
using System;
using System.Management.Automation;

namespace SQL.SMO.Cmdlets
{
    #region Set-SMOContext -- PSCmdlet
    [Cmdlet(VerbsCommon.Set, "SMOContext")]
    [OutputType(typeof(void))]
    [CmdletBinding(PositionalBinding = false)]
    public class SetSmoContext : PSCmdlet
    {
        #region Parameters
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public Server SMO { get; set; }

        private bool _force;
        [Parameter(Mandatory = false)]
        public SwitchParameter Force
        {
            get => _force;
            set => _force = value;
        }

        #endregion
        #region Begin
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (!_force && Context.IsSet)
            {
                throw new SMOContextAlreadySetException();
            }
            else if (Context.IsSet && Context.IsConnected)
            {
                Context.Disconnect(true);
            }
        }

        #endregion
        #region Process
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if (SMO == null)
            {
                throw new ArgumentNullException("You can not set a \"null\" SMO context!");
            }
            SQL.SMO.Framework.Context.AddConnection(SMO, _force);
            if (SQL.SMO.Framework.Context.ConfigProperties == null)
            {
                SQL.SMO.Framework.Context.GetConfigPropertyNames();
            }
        }

        #endregion
    }

    #endregion
}