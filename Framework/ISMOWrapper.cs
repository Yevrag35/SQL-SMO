using Microsoft.SqlServer.Management.Smo;
using System;

namespace SQL.SMO.Framework
{
    public interface ISMOWrapper
    {
        string Name { get; }
        Type OriginalType { get; }
        object ShowOriginal();
        void Load(params string[] propertyNames);
    }
}
