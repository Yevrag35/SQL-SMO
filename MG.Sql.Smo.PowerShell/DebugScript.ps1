$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
foreach ($dll in $(Get-ChildItem $curDir *.dll -File | ? { $_.Name -ne "System.Management.Automation.dll"}))
{
	Import-Module $($dll | Convert-Path);
}

Connect-SmoServer -ServerName GARVMEDIA.yevrag35.com -EncryptConnection -TrustServerCertificate:$false