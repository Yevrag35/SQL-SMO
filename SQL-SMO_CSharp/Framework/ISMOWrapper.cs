using Microsoft.SqlServer.Management.Smo;
using System;

namespace SQL.SMO.Framework
{
    public interface ISMOWrapper
    {
        string Name { get; }
        Type OriginalType { get; }
        object ShowOriginal();
        object Load(params string[] propertyNames);
    }
}
