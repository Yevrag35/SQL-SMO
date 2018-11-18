[CmdletBinding(PositionalBinding = $false)]
param
(
    [parameter(Mandatory, Position=0)]
    [string] $SQLServer,

    [parameter(Mandatory=$false, Position=1)]
    [pscredential] $SQLCredentials,

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
if ($PSBoundParameters.ContainsKey("SQLCredentials"))
{
    $connArgs.SQLCredential = $SQLCredentials;
}

New-SMO @connArgs | Set-SMOContext;