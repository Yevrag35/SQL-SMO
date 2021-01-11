# SQL-SMO PowerShell Module
[![version](https://img.shields.io/powershellgallery/v/SQL-SMO.svg)](https://www.powershellgallery.com/packages/SQL-SMO)
[![downloads](https://img.shields.io/powershellgallery/dt/SQL-SMO.svg?label=downloads)](https://www.powershellgallery.com/stats/packages/SQL-SMO?groupby=Version)

### Work on this module has ceased indefinitely as I'm too busy involved in other projects and no longer work on SQL.

This is a module for gathering and editing SQL Server Instance properties utilizing SQL Management Objects __without__ the need for SQL Management Studio to be installed.

[Link to PowerShell Gallery](https://www.powershellgallery.com/packages/SQL-SMO)

[Check out the wiki](https://github.com/Yevrag35/SQL-SMO/wiki) for information about how to get started and information on each of the cmdlets.

---

## What's new in 1.0.0-beta?

I have been re-working the module (yet again...) to a new one.  As such, I'm eliminating the following commands:
__New-SMO__
__Set-SMOContext__

They will both be replaced by Connect-SmoServer.  'Connect-SmoServer' will accomplish what both commands did; establishing a set context to a particular SQL server/instance.

As of 6/11/19, I've separated System Messages from the Server object.  Enumerating and retrieving the SystemMessageCollection from an Smo.Server class causes a memory usage spike in the calling PowerShell process.  If you want to retrieve the messages, use the new cmdlet 'Get-SmoSystemMessages'.

One new command, that I like, is the Find-SmoSqlInstance cmdlet.  This command will find you SQL instances on a remote machine by using a combination of search methods (e.g. - WMI, Registry, & SQL Browser).  The command can also use piped in objects from other cmdlets like "Get-ADComputer".

Like so:

```Get-ADComputer -Filter * | Find-SmoSqlInstance -SearchMethod WMI```

## All Cmdlets as of 1.0.0-beta

*Cmdlets in __bold__ are new or renamed.*

* __Add-SmoDbUser__
* Connect-SmoServer
* Disconnect-SmoServer
* __Find-SmoSqlInstance__
* Get-SmoColumn
* __Get-SmoCredential__
* Get-SmoDatabase
* __Get-SmoDatabaseLog__
* __Get-SmoDbUser__
* __Get-SmoJob__
* __Get-SmoJobCategory__
* __Get-SmoJobServer__
* __Get-SmoJobStep__
* __Get-SmoLogin__
* __Get-SmoProcess__
* Get-SmoServer
* __Get-SmoServerCollation__
* __Get-SmoServerConfig__
* __Get-SmoServerErrorLog__
* __Get-SmoSystemMessages__
* Get-SmoTable
* __Get-SmoUserDefinedMessages__
* __Get-SmoView__
* __New-SmoCredential__
* __New-SmoJobCategory__
* __New-SmoSqlLogin__
* __New-SmoWindowsLogin__
* __Out-SmoSqlScript__
* __Remove-SmoDatabase__
* __Remove-SmoDbUser__
* __Remove-SmoJob__
* __Remove-SmoJobCategory__
* __Remove-SmoLogin__
* __Rename-SmoLogin__
* __Set-SmoDatabaseState__
* __Set-SmoJob__
* __Set-SmoJobCategory__
* __Set-SmoJobServer__
* __Set-SmoLogin__
* __Set-SmoLoginPassword__
* __Set-SmoServerConfig__
* __Start-SmoJob__
* __Stop-SmoJob__
* __Stop-SmoProcess__
* __Write-SmoSqlScript__
