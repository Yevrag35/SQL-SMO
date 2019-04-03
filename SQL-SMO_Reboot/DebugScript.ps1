#[CmdletBinding(PositionalBinding = $false)]
#param
#(
#    [parameter(Mandatory, Position=0)]
#    [string] $SQLServer,

#    [parameter(Mandatory=$false, Position=1)]
#    [string] $SQLLoginName,

#    [parameter(Mandatory=$false)]
#    [string] $Instance,

#    [parameter(Mandatory=$false)]
#    [switch] $NoEncryption
#)

$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
foreach ($dll in $(Get-ChildItem $curDir *.dll -File))
{
	Import-Module $($dll | Convert-Path);
}

$sqlConn = New-Object System.Data.SqlClient.SqlConnection("Server=GARVMEDIA.yevrag35.com;Integrated Security=true;Encrypt=true;TrustServerCertificate=true;");
$srv = New-Object Microsoft.SqlServer.Management.Smo.Server($sqlConn);
