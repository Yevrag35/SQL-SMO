using MG.Attributes;
using Microsoft.SqlServer.Management.Smo;
using System;

namespace SQL.SMO.Config
{
    public enum MeasurementUnit : long
    {
        [MGName("Bytes")]
        Bytes = 1L
        ,
        [MGName("Kilobytes")]
        KB = 1024L
        ,
        [MGName("Megabytes")]
        MB = 1048576L
        ,
        [MGName("Gigabytes")]
        GB = 1073741824L
    }
    public class MemoryProperty : AttributeResolver
    {
        // Fields
        private MeasurementUnit _u;
        private readonly long _min;
        private readonly long _max;
        public long? MinimumMemory => _min / (long)_u;
        public long? MaximumMemory => _max / (long)_u;
        public string InUnitsOf => GetNameAttribute(_u);

        public MemoryProperty(ConfigProperty minMemory, ConfigProperty maxMemory, MeasurementUnit inUnits)
        {
            _min = minMemory.RunValue * (long)MeasurementUnit.MB;
            _max = maxMemory.RunValue * (long)MeasurementUnit.MB;
            _u = inUnits;
        }

        public MemoryProperty(long? min, long? max, MeasurementUnit inUnits)
        {
            if (min.HasValue)
            {
                _min = min.Value * (long)inUnits;
            }
            if (max.HasValue)
            {
                _max = max.Value * (long)inUnits;
            }
            _u = inUnits;
        }

        public override string ToString() =>
            Convert.ToString(_min / (long)MeasurementUnit.MB) + ", " + Convert.ToString(_max / (long)MeasurementUnit.MB);

        internal object ToString(MemType type, MeasurementUnit inUnits)
        {
            switch (type)
            {
                case MemType.Min:
                    return _min / (long)inUnits;
                case MemType.Max:
                    return _max / (long)inUnits;
                default:
                    return null;
            }
        }


        public string[] ToString(MeasurementUnit inUnits)
        {
            var outStr = new string[2];
            if (inUnits == MeasurementUnit.Bytes)
            {
                outStr[0] = Convert.ToString(_min);
                outStr[1] = Convert.ToString(_max);
            }
            else
            {
                outStr[0] = Convert.ToString(_min / (long)inUnits);
                outStr[1] = Convert.ToString(_max / (long)inUnits);
            }
            return outStr;
        }

        public MemoryProperty ChangeUnitsTo(MeasurementUnit inUnits)
        {
            _u = inUnits;
            return this;
        }

        internal enum MemType : int
        {
            Min = 0,
            Max = 1
        }
    }
}
