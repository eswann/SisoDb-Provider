using System;
using System.Data;
using System.Data.Common;
using SisoDb.Dac;
using SisoDb.EnsureThat;

namespace SisoDb.SqlServer
{
    public class SqlServerConnectionManager : IConnectionManager
    {
        private Func<DbConnection, DbConnection> _onConnectionCreated;

        protected readonly IAdoDriver Driver;

        public SqlServerConnectionManager(IAdoDriver driver)
        {
            Ensure.That(driver, "driver").IsNotNull();

            Driver = driver;
            OnConnectionCreated = cn => cn;
        }

        public Func<DbConnection, DbConnection> OnConnectionCreated
        {
            get { return _onConnectionCreated; }
            set
            {
                Ensure.That(value, "OnConnectionCreated").IsNotNull();
                _onConnectionCreated = value;
            }
        }

        public virtual void Reset()
        {
            OnConnectionCreated = cn => cn;
        }

        public virtual DbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = OnConnectionCreated(Driver.CreateConnection(connectionInfo.ServerConnectionString));
            cn.Open();
            return cn;
        }

        public virtual DbConnection OpenClientConnection(ISisoConnectionInfo connectionInfo)
        {
            var cn = OnConnectionCreated(Driver.CreateConnection(connectionInfo.ClientConnectionString));
            cn.Open();
            return cn;
        }

        public virtual void ReleaseAllConnections() { }

        public virtual void ReleaseServerConnection(DbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }

        public virtual void ReleaseClientConnection(DbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }
    }
}
