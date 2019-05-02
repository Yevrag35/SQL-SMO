﻿using System;

namespace MG.Sql.Smo
{
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

    public enum BinaryChoice
    {
        Disabled,
        Enabled
    }
}