#Requires -Version 5.0 -RunAsAdministrator

$curDir = Split-Path $MyInvocation.MyCommand.Definition -Parent;

$sourceDir = "$curDir\..";
$symDebug = "$curDir\bin\Debug";
$symRelease = "$curDir\bin\Release";

@($symDebug, $symRelease) |
%{
    $sym = $_
    foreach ($item in @("Assemblies", "Formats", "SQL-SMO.psd1"))
    {
        try
        {
            New-Item -Name $item -Path $sym -ItemType SymbolicLink -Value "$sourceDir\$item" -ea Stop
        }
        catch [System.IO.IOException]
        {
            Write-Warning "$item symLink exists already..."
        }
    }
}