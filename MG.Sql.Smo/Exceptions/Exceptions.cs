﻿using System;
using System.Data.SqlClient;

namespace MG.Sql.Smo.Exceptions
{
    #region SMO Exception Classes
    public interface ISMOException
    {
        bool HasDefaultMessage { get; }
    }

    public class SmoContextAlreadySetException : ArgumentException, ISMOException
    {
        public bool HasDefaultMessage => true;

        private const string _def = "The SMO context is already set!  Use the \"Force\" parameter if overriding was intended.";
        
        public SmoContextAlreadySetException(string message = _def)
            : base(message)
        {
        }
    }
    public class SmoContextNotSetException : InvalidOperationException, ISMOException
    {
        public bool HasDefaultMessage => true;
        private const string _def = "The SMO context is not set!  Run the 'Connect-SmoServer' cmdlet to set the context first then re-run this command.";
        public SmoContextNotSetException(string message = _def)
            : base(message)
        {
        }
    }
    public class ContextExecutionError : InvalidOperationException, ISMOException
    {
        public bool HasDefaultMessage => false;
        public ContextExecutionError(string message, SqlException e)
            : base(message, e)
        {
        }
        public ContextExecutionError(string message, InvalidOperationException e)
            : base(message, e)
        {
        }
        public ContextExecutionError(string message, Exception e)
            : base(message, e)
        {
        }
    }

    public class ReadOnlyCollectionException : NotSupportedException, ISMOException
    {
        public bool HasDefaultMessage => true;
        public string OffendingOperation { get; }
        public object[] OffendingArguments { get; }
        private const string defMsg = "This collection object is read-only.  " +
            "'Add, AddRange, Remove, RemoveAt, RemoveAll, Clear, Insert, CopyTo' methods will not be available.";
        public ReadOnlyCollectionException(string methodName, params object[] arguments)
            : base(defMsg)
        {
            OffendingOperation = methodName;
            OffendingArguments = arguments;
        }
    }

    #endregion
}
