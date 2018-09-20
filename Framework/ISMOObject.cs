using System;

namespace SQL.SMO.Framework
{
    public interface ISMOObject
    {
        string Name { get; }
        Type MSType { get; }
        object ShowOriginal();
    }
}
