$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
foreach ($dll in $(Get-ChildItem $curDir *.psd1 -File))
{
	Import-Module $($dll | Convert-Path);
}

#$creds = Get-Credential mikelogin
#Connect-SmoServer -ServerName "DGRLAB-SCCMSQL.dgrlab.com" -EncryptConnection -Credential $creds
Connect-SmoServer -ServerName GARVMEDIA.yevrag35.com -EncryptConnection