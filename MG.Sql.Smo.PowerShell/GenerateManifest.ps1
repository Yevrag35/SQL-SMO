﻿param
(
    [parameter(Mandatory=$true, Position=0)]
    [string] $DebugDirectory,

    [parameter(Mandatory=$true, Position=1)]
    [string] $ModuleFileDirectory,

    [parameter(Mandatory=$true, Position=2)]
    [string] $AssemblyInfo,

    [parameter(Mandatory=$true, Position=3)]
    [string] $TargetFileName
)

## Get Module Version
$assInfo = Get-Content $AssemblyInfo;
foreach ($line in $assInfo)
{
    if ($line -like "*AssemblyFileVersion(*")
    {
        $vers = $line -replace '^\s*\[assembly\:\sAssemblyFileVersion\(\"(.*?)\"\)\]$', '$1';
    }
}
$allFiles = Get-ChildItem $ModuleFileDirectory * -File;
$References = Join-Path "$ModuleFileDirectory\.." "ReferencedAssemblies";

[string[]]$allDlls = Get-ChildItem $References -Include *.dll -Exclude 'System.Management.Automation.dll' -Recurse | Select -ExpandProperty Name;
# Import-Module $(Join-Path $DebugDirectory $TargetFileName);
# $moduleInfo = Get-Command -Module $($TargetFileName.Replace('.dll', ''));
# [string[]]$allCmd = $moduleInfo | ? { $_.CommandType -eq "Cmdlet" } | Select -ExpandProperty Name;
# [string[]]$allAlias = $moduleInfo | ? { $_.CommandType -eq "Alias" } | Select -ExpandProperty Name;
[string[]]$allFormats = $allFiles | ? { $_.Extension -eq ".ps1xml" } | Select -ExpandProperty Name;

$manifestFile = $TargetFileName.Replace('.dll', '.psd1');
# $allNames = @($($allFiles | Select -ExpandProperty Name), $manifestFile);

$allFiles | Copy-Item -Destination $DebugDirectory -Force;

$manifest = @{
    Path               = $(Join-Path $DebugDirectory $manifestFile)
    Guid               = 'd2f53583-045a-4939-a12b-bd446e0b7fcd';
    Description        = 'A module for gathering and editing SQL Server Instance properties utilizing SQL Management Objects.'
    Author             = 'Mike Garvey'
    CompanyName        = 'DGR Systems, LLC.'
    Copyright          = '(c) 2018 DGR Systems, LLC.  All rights reserved.'
    ModuleVersion      = $vers.Trim()
    PowerShellVersion  = '4.0'
    RootModule         = $TargetFileName
    RequiredAssemblies = $allDlls
    AliasesToExport    = 'Get-SQLMemoryLimit', 'Set-SQLMemoryLimit'
    CmdletsToExport    = @( 'Disconnect-SMO', 'Get-SMOColumn', 'Get-SMOConfiguration', 
							'Get-SMOConnection', 'Get-SMODatabase', 'Get-SMOJob', 'Get-SMOMemoryLimit', 
							'Get-SMOSqlAgent', 'Get-SMOTable', 'New-SMO', 'Set-SMOConfiguration', 
							'Set-SMOContext', 'Set-SMOMemoryLimit')
    FunctionsToExport  = @()
    VariablesToExport  = ''
    FormatsToProcess   = $allFormats
    ProjectUri	       = 'https://github.com/Yevrag35/SQL-SMO'
    Tags               = @( 'SQL', 'Server', 'Instance', 'Property', 'Setting', 'Settings',
                            'Set', 'Change', 'SMO', 'Management', 'Object', 'Memory', 'Tables',
                            'Columns', 'Max', 'Min', 'MB', 'Megabyte', 'Feature', 'Microsoft',
                            'New', 'Value', 'Configuration', 'Alter', 'Authentication', 'Connection',
                            'Get', 'Set', 'RAM', 'Credential', 'Integrated', 'Security', 'Documenting',
                            'default', 'connection', 'string', 'data', 'context', 'format', 'Database',
                            'Query', 'Progress', 'bar' )
};

New-ModuleManifest @manifest;