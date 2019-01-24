# SQL-SMO PowerShell Module
[![version](https://img.shields.io/powershellgallery/v/SQL-SMO.svg)](https://www.powershellgallery.com/packages/SQL-SMO)
[![downloads](https://img.shields.io/powershellgallery/dt/SQL-SMO.svg?label=downloads)](https://www.powershellgallery.com/stats/packages/SQL-SMO?groupby=Version)

This is a module for gathering and editing SQL Server Instance properties utilizing SQL Management Objects __without__ the need for SQL Management Studio to be installed.

[Link to PowerShell Gallery](https://www.powershellgallery.com/packages/SQL-SMO)

[Check out the wiki](https://github.com/Yevrag35/SQL-SMO/wiki) for information about how to get started and information on each of the cmdlets.

---

## What's new in 0.5.0.0?

New cmdlet structure and new Commands!  "Get-SMOSqlAgent" & "Get-SMOJob"

I spent most of 0.5.0.0's development re-focusing on dynamic list views and dynamic property loading for each of the SQL objects you can retrieve (i.e. - databases, tables, columns, etc.).

The result is making available __ALL__ of the object's original properties without displaying them by default.  In a very similar way to MS's Active Directory PowerShell module, most commands now have a "-Properties" parameter that can load and display the desired properties (along with default ones) specified.

The best part about that is that __THEY'RE DYNAMICALLY RETRIEVED FOR YOU!__

![DynamicProperties](https://images.yevrag35.com/DynamicProperties.gif)
