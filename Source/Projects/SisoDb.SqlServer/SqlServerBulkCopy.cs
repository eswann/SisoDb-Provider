using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SisoDb.Dac;
using SisoDb.Dac.Profiling;
using SisoDb.EnsureThat;

namespace SisoDb.SqlServer
{
    public class SqlServerBulkCopy : IDbBulkCopy
    {
        private IDbClient _dbClient;
        private SqlBulkCopy _innerBulkCopy;

        public SqlServerBulkCopy(IDbClient dbClient)
        {
            Ensure.That(dbClient, "dbClient").IsNotNull();

            _dbClient = dbClient;
            Initialize(dbClient.Connection, dbClient.Transaction);
        }

        private void Initialize(IDbConnection connection, IDbTransaction transaction = null)
        {
            _innerBulkCopy = new SqlBulkCopy(
                connection.ToSqlConnection(),
                SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls,
                transaction.ToSqlTransaction())
            {
                NotifyAfter = 0
            };
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_innerBulkCopy != null)
            {
                _innerBulkCopy.Close();
                _innerBulkCopy = null;
            }

            _dbClient = null;
        }

        public string DestinationTableName
        {
            set { _innerBulkCopy.DestinationTableName = value; }
        }

        public int BatchSize
        {
            set { _innerBulkCopy.BatchSize = value; }
        }

        public void AddColumnMapping(string sourceFieldName, string destinationFieldName)
        {
            _innerBulkCopy.ColumnMappings.Add(sourceFieldName, destinationFieldName);
        }

        public void Write(DbDataReader reader)
        {
            _dbClient.ExecuteNonQuery("SET XACT_ABORT ON;");
            _innerBulkCopy.WriteToServer(reader);
        }

        public async Task WriteAsync(DbDataReader reader)
        {
            await _dbClient.ExecuteNonQueryAsync("SET XACT_ABORT ON;");
            await _innerBulkCopy.WriteToServerAsync(reader);
        }
    }
}