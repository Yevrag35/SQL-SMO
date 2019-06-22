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

[string[]]$verbs = Get-Verb | Select-Object -ExpandProperty Verb;
$patFormat = '^({0})(\S{{1,}})\.cs';
$pattern = $patFormat -f ($verbs -join '|');
$cmdletFormat = "{0}-{1}";

$baseCmdletDir = Join-Path "$ModuleFileDirectory\.." "Cmdlets";
[string[]]$folders = [System.IO.Directory]::EnumerateDirectories($baseCmdletDir, "*", [System.IO.SearchOption]::TopDirectoryOnly) | Where-Object { -not $_.EndsWith('Bases') };

$aliasPat = '\[alias\(\"(.{1,})\"\)\]'
$csFiles = @(Get-ChildItem -Path $folders *.cs -File);
$Cmdlets = New-Object System.Collections.Generic.List[string] $csFiles.Count;
$Aliases = New-Object System.Collections.Generic.List[string];
foreach ($cs in $csFiles)
{
	$match = [regex]::Match($cs.Name, $pattern)
    $Cmdlets.Add(($cmdletFormat -f $match.Groups[1].Value, $match.Groups[2].Value));
    $content = Get-Content -Path $file -Raw;
    $aliasMatch = [regex]::Match($content, $aliasPat, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase);
    if ($aliasMatch.Success)
    {
        $Aliases.Add($aliasMatch.Groups[1].Value);
    }
}

[string[]]$allDlls = Get-ChildItem $References -Include *.dll -Exclude 'System.Management.Automation.dll' -Recurse | Select-Object -ExpandProperty Name;
[string[]]$allFormats = $allFiles | Where-Object -FilterScript { $_.Extension -eq ".ps1xml" } | Select-Object -ExpandProperty Name;

$manifestFile = "SQL-SMO.psd1"

$allFiles | Copy-Item -Destination $DebugDirectory -Force;
$modPath = Join-Path $DebugDirectory $manifestFile;

$manifest = @{
    Path                   = $modPath
    Guid                   = 'd2f53583-045a-4939-a12b-bd446e0b7fcd';
    Description            = 'A module for gathering and editing SQL Server Instance properties utilizing SQL Management Objects without the need for SQL Management Studio to be installed.'
    Author                 = 'Mike Garvey'
    CompanyName            = 'Yevrag35, LLC.'
    Copyright              = '(c) 2019 Yevrag35, LLC.  All rights reserved.'
    ModuleVersion          = $($vers.Trim() -split '\.' | Select-Object -First 3) -join '.'
    PowerShellVersion      = '4.0'
    DotNetFrameworkVersion = '4.7'
    RootModule             = $TargetFileName
    DefaultCommandPrefix   = "Smo"
    RequiredAssemblies     = $allDlls
	CmdletsToExport		   = $Cmdlets
#    VariablesToExport      = ''
    FormatsToProcess       = if ($allFormats.Length -gt 0) { $allFormats } else { @() };
    ProjectUri             = 'https://github.com/Yevrag35/SQL-SMO'
	LicenseUri			   = 'https://raw.githubusercontent.com/Yevrag35/SQL-SMO/master/LICENSE'
	HelpInfoUri			   = 'https://github.com/Yevrag35/SQL-SMO/issues'
    Tags                   = @( 'SQL', 'Server', 'Instance', 'Property', 'Setting', 'Settings',
                                'Set', 'Change', 'Smo', 'Management', 'Object', 'Memory', 'Tables',
                                'Columns', 'Max', 'Min', 'MB', 'Megabyte', 'Feature', 'Microsoft',
                                'New', 'Value', 'Configuration', 'Alter', 'Authentication', 'Connection',
                                'Get', 'Set', 'RAM', 'Credential', 'Integrated', 'Security', 'Documenting',
                                'default', 'connection', 'string', 'data', 'context', 'format', 'Database',
                                'Query', 'Progress', 'bar', 'find', 'db', 'instance' )
};
if ($Aliases.Count -gt 0)
{
    $manifest.AliasesToExport = $Aliases.ToArray();
}

New-ModuleManifest @manifest;
Update-ModuleManifest -Path $modPath -Prerelease 'beta';