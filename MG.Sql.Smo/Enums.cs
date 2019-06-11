using System;

namespace MG.Sql.Smo
{
    public enum BinaryChoice
    {
        Disabled,
        Enabled
    }

    public enum DatabaseStatus
    {
        Online,
        Offline,
        Detached
    }

    public enum ReadAccess
    {
        ReadWrite,
        ReadOnly
    }

    public enum SQLYearVersion
    {
        SQLServer2008R2 = 1,
        SQLServer2005 = 9,
        SQLServer2008 = 10,
        SQLServer2012 = 11,
        SQLServer2014 = 12,
        SQLServer2016 = 13,
        SQLServer2017 = 14
    }
}
