﻿using System.Collections.Generic;
using System.Data.Common;
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
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteAllExceptIds").Inject(structureSchema.GetStructureTableName());

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

        public override void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteByIds").Inject(structureSchema.GetStructureTableName());

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

        public override IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonByIds").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql))
            {
				foreach (var idBatch in ids.Batch(MaxBatchedIdsSize))
				{
					cmd.Parameters.Clear();
                    cmd.Parameters.Add(SqlServerIdsTableParam.CreateIdsTableParam(structureSchema.IdAccessor.IdType, idBatch));

				    foreach (var data in YieldJson(structureSchema, cmd))
				        yield return data;
				}
            }
        }
    }
}