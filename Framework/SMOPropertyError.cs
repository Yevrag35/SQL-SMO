using System;

namespace SQL.SMO.Framework
{
    public class SMOPropertyError : ISMOWrapper
    {
        private readonly Exception _origEx;

        public string Name { get; }
        public Type OriginalType { get; }

        public string ExceptionType =>
            _origEx.InnerException != null ?
                _origEx.InnerException.GetType().FullName :
                _origEx.GetType().FullName;
        public string ErrorMessage =>
            _origEx.InnerException != null ?
                _origEx.InnerException.Message :
                _origEx.Message;


        internal protected SMOPropertyError(string pName, Exception e, Type pType)
        {
            Name = pName;
            _origEx = e;
            OriginalType = pType;
        }

        public object ShowOriginal() => throw new NotImplementedException();
        public void Load(params string[] vwha) => throw new NotImplementedException();
    }
}
