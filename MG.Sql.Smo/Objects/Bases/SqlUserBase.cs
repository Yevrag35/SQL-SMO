using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security;

namespace MG.Sql.Smo
{
    public abstract class SqlUserBase : IRefreshable, ISfcValidate
    {
        #region FIELDS/CONSTANTS

        private const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
        private const string SECRET_METHOD = "GetPropValueOptionalAllowNull";
        private SqlSmoObject _smo;
        private readonly MethodInfo SecretMethod = typeof(SqlSmoObject).GetMethod(SECRET_METHOD, FLAGS);

        #endregion

        #region PROPERTIES
        public abstract string AsymmetricKey { get; set; }
        public abstract string Certificate { get; set; }
        public abstract DateTime CreateDate { get; }
        public abstract DateTime DateLastModified { get; }
        public abstract int ID { get; }
        public abstract bool IsSystemObject { get; }
        public abstract LoginType LoginType { get; }
        public abstract string Name { get; set; }
        public abstract string Sid { get; }

        #endregion

        #region CONSTRUCTORS
        public SqlUserBase(SqlSmoObject smoObj) => _smo = smoObj;

        #endregion

        #region PUBLIC METHODS

        public ExecutionManager GetExecutionManager() => _smo.ExecutionManager;
        public AbstractCollectionBase GetParentCollection() => _smo.ParentCollection;
        public SqlPropertyCollection GetProperties() => _smo.Properties;
        public ServerVersion GetServerVersion() => _smo.ServerVersion;
        public SqlSmoState GetState() => _smo.State;
        public Urn GetUrn() => _smo.Urn;
        public object GetUserData() => _smo.UserData;

        public List<object> Discover() => _smo.Discover();
        public void ExecuteWithModes(SqlExecutionModes modes, Action action) => _smo.ExecuteWithModes(modes, action);
        public T GetPropValueOptional<T>(string propertyName, T defaultValue) => _smo.GetPropValueOptional(propertyName, defaultValue);
        public string GetSqlVersionAndName() => _smo.GetSqlServerVersionName();
        public IComparer<string> GetStringComparer() => _smo.GetStringComparer();
        public void InitChildCollection(Urn childType, bool forScripting) => _smo.InitChildCollection(childType, forScripting);
        public bool Initialize() => _smo.Initialize();
        public bool Initialize(bool allProperties) => _smo.Initialize(allProperties);
        public bool IsExpressSku() => _smo.IsExpressSku();
        public bool IsSupportedObject<T>(ScriptingPreferences sp = null) where T : SqlSmoObject
        {
            return _smo.IsSupportedObject<T>(sp);
        }
        public bool IsSupportedProperty(string propName) => _smo.IsSupportedProperty(propName);
        public virtual void Refresh() => _smo.Refresh();
        public void SetAccessToken(IRenewableToken token) => _smo.SetAccessToken(token);
        public void SetState(SqlSmoState state) => _smo.SetState(state);
        public void Touch() => _smo.Touch();
        public ValidationState Validate(string methodName, params object[] arguments) => _smo.Validate(methodName, arguments);

        public static SqlUserBase ConvertFromSql(Login login)
        {
            SmoLogin smo = login;
            return smo;
        }
        public static SqlUserBase ConvertFromSql(User user)
        {
            SmoUser smo = user;
            return smo;
        }

        #endregion

        #region BACKEND/PRIVATE METHODS
        protected private bool? GetPropValueEvenIfNull(string propName) => (bool?)SecretMethod.Invoke(_smo, new object[1] { propName });

        #endregion
    }
}