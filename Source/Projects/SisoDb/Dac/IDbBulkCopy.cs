using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace SisoDb.Dac
{
    public interface IDbBulkCopy : IDisposable
    {
        string DestinationTableName { set; }
        int BatchSize { set; }
        void AddColumnMapping(string sourceFieldName, string destinationFieldName);

        void Write(DbDataReader reader);
        Task WriteAsync(DbDataReader reader);
    }
}