using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.NCore.Collections;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.SqlServer
{
    public class SqlServerDbClient : DbClientBase
    {
        protected virtual int MaxBatchedIdsSize
        {
            get { return 200; }
        }

        public SqlServerDbClient(IAdoDriver driver, DbConnection connection, IConnectionManager connectionManager, ISqlStatements sqlStatements, IDbPipe pipe)
            : base(driver, connection, connectionManager, sqlStatements, pipe)
        {
        }

        public SqlServerDbClient(IAdoDriver driver, DbConnection connection, DbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements, IDbPipe pipe)
            : base(driver, connection, transaction, connectionManager, sqlStatements, pipe)
        {
        }

        protected override IDbBulkCopy GetBulkCopy()
        {
            return new SqlServerBulkCopy(this);
        }

        public override void DeleteAllExceptIds(IEnumerable<IStructureId> structureIds, IStructureSchema structureSchema)
        {
            var sql = CreateDeleteAllExceptionIdsSql(structureSchema);

            using (var cmd = CreateCommand(sql))
            {
                foreach (var idBatch in structureIds.Batch(MaxBatchedIdsSize))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override async Task DeleteAllExceptIdsAsync(IEnumerable<IStructureId> structureIds, IStructureSchema structureSchema)
        {
            var sql = CreateDeleteAllExceptionIdsSql(structureSchema);

            using (var cmd = CreateCommand(sql))
            {
                foreach (var idBatch in structureIds.Batch(MaxBatchedIdsSize))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private string CreateDeleteAllExceptionIdsSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteAllExceptIds").Inject(structureSchema.GetStructureTableName());
            return sql;
        }

        public override void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            var sql = CreateDeleteByIdsSql(structureSchema);

            using (var cmd = CreateCommand(sql))
            {
            	foreach (var idBatch in ids.Batch(MaxBatchedIdsSize))
            	{
					cmd.Parameters.Clear();
					cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));
					cmd.ExecuteNonQuery();	
            	}
            }
        }

        public override async Task DeleteByIdsAsync(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            var sql = CreateDeleteByIdsSql(structureSchema);

            using (var cmd = CreateCommand(sql))
            {
                foreach (var idBatch in ids.Batch(MaxBatchedIdsSize))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private string CreateDeleteByIdsSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.DeleteByIds).Inject(structureSchema.GetStructureTableName());
            return sql;
        }

        public override IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            var sql = GetJsonByIdsSql(structureSchema);

            using (var cmd = CreateCommand(sql))
            {
				foreach (var idBatch in ids.Batch(MaxBatchedIdsSize))
				{
					cmd.Parameters.Clear();
                    cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));

				    foreach (var data in RetreiveJson(structureSchema, cmd))
				        yield return data;
				}
            }
        }

        public override async Task<IEnumerable<string>> GetJsonByIdsAsync(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            var sql = GetJsonByIdsSql(structureSchema);

            var returnJson = new List<string>();

            using (var cmd = CreateCommand(sql))
            {
                foreach (var idBatch in ids.Batch(MaxBatchedIdsSize))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));

                    returnJson.AddRange(await RetreiveJsonAsync(structureSchema, cmd));

                }
            }

            return returnJson;
        }

        private string GetJsonByIdsSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.GetJsonByIds).Inject(structureSchema.GetStructureTableName());
            return sql;
        }
    }
}