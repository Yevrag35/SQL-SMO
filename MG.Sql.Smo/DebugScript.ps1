﻿$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
foreach ($dll in $(Get-ChildItem $curDir *.dll -File))
{
	Import-Module $($dll | Convert-Path);
}

$sqlConn = New-Object System.Data.SqlClient.SqlConnection(
	"Server=GARVMEDIA.yevrag35.com;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;"
);
#$creds = Get-Credential mikelogin;
#$creds.Password.MakeReadOnly();
#$sqlCreds = New-Object System.Data.SqlClient.SqlCredential($creds.UserName, $creds.Password);
#$sqlConn = New-Object System.Data.SqlClient.SqlConnection("Server=DGRLAB-SCCMSQL.dgrlab.com;Encrypt=true;TrustServerCertificate=true;", $sqlCreds);
$srv = New-Object Microsoft.SqlServer.Management.Smo.Server($sqlConn);
$smo = New-Object MG.Sql.Smo.SmoServer($srv);