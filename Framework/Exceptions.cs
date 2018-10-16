using System;
using System.Data.SqlClient;
using System.Management.Automation;

namespace SQL.SMO.Framework
{
    #region SMO Exception Classes
    public interface ISMOException
    {
        bool HasDefaultMessage { get; }
        Exception ThisException { get; }
    }

    public class SMOContextAlreadySetException : ArgumentException, ISMOException
    {
        public bool HasDefaultMessage => true;
        public Exception ThisException => this;

        private const string _def = "The SMO context is already set!  Use the \"Force\" parameter if overriding was intended.";
        
        public SMOContextAlreadySetException(string message = _def)
            : base(message)
        {
        }
    }
    public class SMOContextNotSetException : InvalidOperationException, ISMOException
    {
        public bool HasDefaultMessage => true;
        public Exception ThisException => this;
        private const string _def = "The SMO context is not set!  Set the context first then re-run this command.";
        public SMOContextNotSetException(string message = _def)
            : base(message)
        {
        }
    }
    public class ContextExecutionError : InvalidOperationException, ISMOException
    {
        public bool HasDefaultMessage => false;
        public Exception ThisException => this;
        public ContextExecutionError(string message, SqlException e)
            : base(message, e)
        {
        }
        public ContextExecutionError(string message, InvalidOperationException e)
            : base(message, e)
        {
        }
    }

    #endregion
    #region Static Exception Invoker
    public static class Error
    {
        public static ErrorRecord Throw(ISMOException e, object target = null)
        {
            var rec = new ErrorRecord((System.Exception)e, e.GetType().FullName, ErrorCategory.InvalidOperation, target);
            return rec;
        }
    }

    #endregion
}
