using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.NCore.Collections;
using SisoDb.SqlServer;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Sql2005
{
    public class Sql2005DbClient : SqlServerDbClient
    {
        protected override int MaxBatchedIdsSize
        {
            get { return 32; }
        }

        public Sql2005DbClient(IAdoDriver driver, DbConnection connection, IConnectionManager connectionManager, ISqlStatements sqlStatements, IDbPipe pipe)
            : base(driver, connection, connectionManager, sqlStatements, pipe) { }

        public Sql2005DbClient(IAdoDriver driver, DbConnection connection, DbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements, IDbPipe pipe)
            : base(driver, connection, transaction, connectionManager, sqlStatements, pipe) { }

        public override void DeleteAllExceptIds(IEnumerable<IStructureId> structureIds, IStructureSchema structureSchema)
        {
            var sqlFormat = CreateDeleteAllExceptionIdsSql(structureSchema);

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in structureIds.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override async Task DeleteAllExceptIdsAsync(IEnumerable<IStructureId> structureIds, IStructureSchema structureSchema)
        {
            var sqlFormat = CreateDeleteAllExceptionIdsSql(structureSchema);

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in structureIds.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private string CreateDeleteAllExceptionIdsSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            var sqlFormat = SqlStatements.GetSql(DacCommandNames.DeleteAllExceptIds).Inject(structureSchema.GetStructureTableName(), "{0}");
            return sqlFormat;
        }

        public override void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            var sqlFormat = CreateDeleteByIdsSql(structureSchema);

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in ids.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override async Task DeleteByIdsAsync(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            var sqlFormat = CreateDeleteByIdsSql(structureSchema);

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in ids.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private string CreateDeleteByIdsSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            var sqlFormat = SqlStatements.GetSql(DacCommandNames.DeleteByIds).Inject(structureSchema.GetStructureTableName(), "{0}");
            return sqlFormat;
        }

        public override IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            var sqlFormat = CreateGetJsonByIdsSql(structureSchema);

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in ids.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);

                    foreach (var data in RetreiveJson(structureSchema, cmd))
                        yield return data;
                }
            }
        }

        public override async Task<IEnumerable<string>> GetJsonByIdsAsync(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            var jsonResults = new List<string>();

            var sqlFormat = CreateGetJsonByIdsSql(structureSchema);

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in ids.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);

                    jsonResults.AddRange(await RetreiveJsonAsync(structureSchema, cmd));
                }
            }

            return jsonResults;
        }

        private string CreateGetJsonByIdsSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            var sqlFormat = SqlStatements.GetSql(DacCommandNames.GetJsonByIds).Inject(structureSchema.GetStructureTableName(), "{0}");
            return sqlFormat;
        }
    }
}