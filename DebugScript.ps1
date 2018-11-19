[CmdletBinding(PositionalBinding = $false)]
param
(
    [parameter(Mandatory, Position=0)]
    [string] $SQLServer,

    [parameter(Mandatory=$false, Position=1)]
    [string] $SQLLoginName,

    [parameter(Mandatory=$false)]
    [string] $Instance,

    [parameter(Mandatory=$false)]
    [switch] $NoEncryption
)

$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
Import-Module "$curDir\SQL.SMO.psd1";
$connArgs = @{ ServerName = $SQLServer };
if (-not $PSBoundParameters.ContainsKey("NoEncryption"))
{
    $connArgs.EncryptConnection = $null;
}
if ($PSBoundParameters.ContainsKey("Instance"))
{
    $connArgs.InstanceName = $Instance;
}
if ($PSBoundParameters.ContainsKey("SQLLoginName"))
{
	$SQLCredentials = Get-Credential $SQLLoginName;
	if ($SQLCredentials.Password)
	{
		$connArgs.SQLCredential = $SQLCredentials;
	}
    else
	{
		exit;
	}
}

New-SMO @connArgs | Set-SMOContext;