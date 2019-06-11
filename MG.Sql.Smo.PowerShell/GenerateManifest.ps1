param
(
    [parameter(Mandatory = $true, Position = 0)]
    [string] $DebugDirectory,

    [parameter(Mandatory = $true, Position = 1)]
    [string] $ModuleFileDirectory,

    [parameter(Mandatory = $true, Position = 2)]
    [string] $AssemblyInfo,

    [parameter(Mandatory = $true, Position = 3)]
    [string] $TargetFileName
)

## Clear out files
Get-ChildItem -Path $DebugDirectory -Include *.ps1xml -Recurse | Remove-Item -Force;

## Get Module Version
$assInfo = Get-Content -Path $AssemblyInfo;
foreach ($line in $assInfo)
{
    if ($line -like "*AssemblyFileVersion(*")
    {
        $vers = $line -replace '^\s*\[assembly\:\sAssemblyFileVersion\(\"(.*?)\"\)\]$', '$1';
    }
}
$allFiles = Get-ChildItem $ModuleFileDirectory -Include * -Exclude *.old -Recurse;
$References = Join-Path "$ModuleFileDirectory\.." "Assemblies";

[string[]]$allDlls = Get-ChildItem $References -Include *.dll -Exclude 'System.Management.Automation.dll' -Recurse | Select-Object -ExpandProperty Name;
# Import-Module $(Join-Path $DebugDirectory $TargetFileName);
# $moduleInfo = Get-Command -Module $($TargetFileName.Replace('.dll', ''));
# [string[]]$allCmd = $moduleInfo | ? { $_.CommandType -eq "Cmdlet" } | Select -ExpandProperty Name;
# [string[]]$allAlias = $moduleInfo | ? { $_.CommandType -eq "Alias" } | Select -ExpandProperty Name;
[string[]]$allFormats = $allFiles | Where-Object -FilterScript { $_.Extension -eq ".ps1xml" } | Select-Object -ExpandProperty Name;

$manifestFile = "SQL-SMO.psd1"
# $allNames = @($($allFiles | Select -ExpandProperty Name), $manifestFile);

$allFiles | Copy-Item -Destination $DebugDirectory -Force;
$modPath = Join-Path $DebugDirectory $manifestFile

$manifest = @{
    Path                   = $modPath
    Guid                   = 'd2f53583-045a-4939-a12b-bd446e0b7fcd';
    Description            = 'A module for gathering and editing SQL Server Instance properties utilizing SQL Management Objects without the need for SQL Management Studio to be installed.'
    Author                 = 'Mike Garvey'
    CompanyName            = 'DGR Systems, LLC.'
    Copyright              = '(c) 2019 DGR Systems, LLC.  All rights reserved.'
    ModuleVersion          = $($vers.Trim() -split '\.' | Select-Object -First 3) -join '.'
    PowerShellVersion      = '4.0'
    DotNetFrameworkVersion = '4.7'
    RootModule             = $TargetFileName
    DefaultCommandPrefix   = "Smo"
    RequiredAssemblies     = $allDlls
    CmdletsToExport        = @( 'Connect-Server', 'Disconnect-Server', 'Find-SqlInstance',
                                'Get-Column', 'Get-ServerConfig', 'Get-Server', 'Get-SystemMessages',
                                'Get-Connection', 
                                'Get-Database', 'Get-DatabaseState', 'Get-AgentJob', 'Get-Table',
                                'Get-AgentServer', 'Start-AgentJob', 'Set-ServerConfig', 'Set-AgentJob', 
                                'Set-AgentServer', 'Stop-AgentJob')
    VariablesToExport      = ''
    FormatsToProcess       = if ($allFormats.Length -gt 0) { $allFormats } else { @() };
    ProjectUri             = 'https://github.com/Yevrag35/SQL-SMO'
    Tags                   = @( 'SQL', 'Server', 'Instance', 'Property', 'Setting', 'Settings',
                                'Set', 'Change', 'Smo', 'Management', 'Object', 'Memory', 'Tables',
                                'Columns', 'Max', 'Min', 'MB', 'Megabyte', 'Feature', 'Microsoft',
                                'New', 'Value', 'Configuration', 'Alter', 'Authentication', 'Connection',
                                'Get', 'Set', 'RAM', 'Credential', 'Integrated', 'Security', 'Documenting',
                                'default', 'connection', 'string', 'data', 'context', 'format', 'Database',
                                'Query', 'Progress', 'bar', 'find', 'db', 'instance' )
};

New-ModuleManifest @manifest;
Update-ModuleManifest -Path $modPath -Prerelease 'alpha'