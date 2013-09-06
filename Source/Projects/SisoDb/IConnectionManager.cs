using System;
using System.Data.Common;

namespace SisoDb
{
    public interface IConnectionManager
    {
        Func<DbConnection, DbConnection> OnConnectionCreated { get; set; }
        
        DbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo);
        DbConnection OpenClientConnection(ISisoConnectionInfo connectionInfo);
        void Reset();
        void ReleaseAllConnections();
        void ReleaseServerConnection(DbConnection dbConnection);
        void ReleaseClientConnection(DbConnection dbConnection);
    }
}