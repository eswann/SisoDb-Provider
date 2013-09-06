﻿using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            var sqlFormat = SqlStatements.GetSql("DeleteAllExceptIds").Inject(structureSchema.GetStructureTableName(), "{0}");

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

        public override void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            var sqlFormat = SqlStatements.GetSql("DeleteByIds").Inject(structureSchema.GetStructureTableName(), "{0}");

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

        public override IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            var sqlFormat = SqlStatements.GetSql("GetJsonByIds").Inject(structureSchema.GetStructureTableName(), "{0}");

            using (var cmd = CreateCommand(string.Empty))
            {
                foreach (var batchedIds in ids.Batch<IStructureId, IDacParameter>(MaxBatchedIdsSize, (id, batchCount) => new DacParameter(string.Concat("id", batchCount), id.Value)))
                {
                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd, batchedIds);

                    var paramsString = string.Join(",", batchedIds.Select(p => p.Name));
                    cmd.CommandText = sqlFormat.Inject(paramsString);

                    foreach (var data in YieldJson(structureSchema, cmd))
                        yield return data;
                }
            }
        }
    }
}