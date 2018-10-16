@{
    GUID = 'd2f53583-045a-4939-a12b-bd446e0b7fcd'
    Description = 'A module for gathering and editing SQL Server Instance properties utilizing SQL Management Objects.'
    Author = 'Mike Garvey'
    CompanyName = 'DGR Systems, LLC.'
    Copyright = '(c) 2018 DGR Systems, LLC.  All rights reserved.'
    ModuleVersion = '0.4.1'
    PowerShellVersion = '4.0'
    RootModule = "SQL.SMO.dll"
    RequiredAssemblies = @(
        'MG.Attributes.dll',
        'Microsoft.SqlServer.ConnectionInfo.dll',
        'Microsoft.SqlServer.Diagnostics.STrace.dll',
        'Microsoft.SqlServer.Dmf.Common.dll',
        'Microsoft.SqlServer.Management.Sdk.Sfc.dll',
        'Microsoft.SqlServer.ServiceBrokerEnum.dll',
        'Microsoft.SqlServer.Smo.dll',
        'Microsoft.SqlServer.SqlClrProvider.dll',
        'Microsoft.SqlServer.SqlEnum.dll',
        'SQL.SMO.dll'
    )
    FunctionsToExport = @()
    AliasesToExport = @(
        'Get-SMOProperty',
        'nsmo',
        'setsmo',
        'Set-SMOProperty',
        'Get-SQLMemoryLimits'
    )
    CmdletsToExport = @(
		'Get-SMOConfiguration',
		'Disconnect-SMO',
        'New-SMO',
        'Set-SMOContext',
        'Get-SQLMemoryLimit',
        'Get-SMOConnection',
        'Set-SQLMemoryLimit',
        'Set-SMOConfiguration',
        'Get-SMODatabase',
        'Get-SMOColumn',
        'Get-SMOTable'
    )
    FormatsToProcess = @(
        'Formats\SQL.SMO.Databases.SMOColumn.Format.ps1xml',
        'Formats\SQL.SMO.Databases.SMODatabase.Format.ps1xml',
        'Formats\SQL.SMO.Databases.SMOTable.Format.ps1xml',
        'Formats\SQL.SMO.MemoryProperty.Format.ps1xml',
        'Formats\SQL.SMO.SMOProperty.Format.ps1xml'
    )
    VariablesToExport = ''
    FileList = @(
        'MG.Attributes.dll',
        'Microsoft.SqlServer.ConnectionInfo.dll',
        'Microsoft.SqlServer.Diagnostics.STrace.dll',
        'Microsoft.SqlServer.Dmf.Common.dll',
        'Microsoft.SqlServer.Management.Sdk.Sfc.dll',
        'Microsoft.SqlServer.ServiceBrokerEnum.dll',
        'Microsoft.SqlServer.Smo.dll',
        'Microsoft.SqlServer.SqlClrProvider.dll',
        'Microsoft.SqlServer.SqlEnum.dll',
        'Formats\SQL.SMO.Databases.SMOColumn.Format.ps1xml',
        'Formats\SQL.SMO.Databases.SMODatabase.Format.ps1xml',
        'Formats\SQL.SMO.Databases.SMOTable.Format.ps1xml',
		'Formats\SQL.SMO.MemoryProperty.Format.ps1xml',
        'Formats\SQL.SMO.SMOProperty.Format.ps1xml',
        'SQL.SMO.dll',
        'SQL-SMO.psd1',
		'SQL-SMO-Help.xml'
    )
	PrivateData = @{
		PSData = @{
			# Tags applied to this module. These help with module discovery in online galleries.
			Tags = 'SQL','Server','Instance','Property','Setting','Settings','Set','Change','SMO','Management','Object','Memory','Tables','Columns',
			'Max','Min','MB','Megabyte','Feature','Microsoft','New','Value','Configuration','Alter','Authentication','Connection','Get','Set','RAM',
			'Credential','Integrated','Security','Documenting','default','connection','string','data','context','format','Database','Query','Progress','bar'
			# ReleaseNotes of this module
			ReleaseNotes = 'Added progress bars to Get-SMOTable & Get-SMOColumn cmdlets.'
		}
	 }
}