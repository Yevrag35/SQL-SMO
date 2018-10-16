# SQL-SMO PowerShell Module

This is a module for gathering and editing SQL Server Instance properties utilizing SQL Management Objects.

[Link to PowerShell Gallery](https://www.powershellgallery.com/packages/SQL-SMO)

[Check out the wiki](https://github.com/Yevrag35/SQL-SMO/wiki) for information about each of the cmdlets.

---

## How to Connect to SQL

To connect to a SQL instance, simply use the <code>New-SMO</code> cmdlet (_the command will default to the localhost and MSSQLSERVER for the instance_).  You connect the same way you would as with SSMS; either with:

1. Your current Windows session credentials
1. Specify SQL Credentials used in SQL Authentication.

__In order to keep the memory and processing cost down, I'd advise immediately storing the SMO object as a variable.__

Using the stored SMO variable, you are free to manipulate the SQL server instance as it correlates to your access.

To provide a convenient method for working with SMO's and to use the other cmdlets in this module, pipe the SMO to the <code>Set-SMOContext</code> cmdlet.  After that, you can run the other cmdlets without having to carry around the SMO variable.

* <code>New-SMO -ServerName "sql.contoso.com" -InstanceName "DataWarehouse" | Set-SMOContext</code>

* <code>New-SMO -SQLCredential (Get-Credential sa) | Set-SMOContext</code>

* <code>$smo = New-SMO -ServerName "sql.contoso.com";<br>Set-SMOContext -SMO $smo;</code>
