using System.Data.Common;

namespace SisoDb.Dac
{
    public interface IAdoDriver
    {
        int CommandTimeout { get; set; }
        DbConnection CreateConnection(string connectionString);
        DbCommand CreateCommand(DbConnection connection, string sql, DbTransaction transaction = null, params IDacParameter[] parameters);
        DbCommand CreateSpCommand(DbConnection connection, string spName, DbTransaction transaction = null, params IDacParameter[] parameters);
        void AddCommandParametersTo(DbCommand cmd, params IDacParameter[] parameters);
    }
}