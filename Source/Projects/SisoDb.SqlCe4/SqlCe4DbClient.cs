﻿using System.Collections.Generic;
using System.Data;
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

namespace SisoDb.SqlCe4
{
    public class SqlCe4DbClient : SqlServerDbClient
    {
        protected override int MaxBatchedIdsSize
        {
            get { return 32; }
        }

        public SqlCe4DbClient(IAdoDriver driver, DbConnection connection, IConnectionManager connectionManager, ISqlStatements sqlStatements, IDbPipe pipe)
            : base(driver, connection, connectionManager, sqlStatements, pipe) { }

        public SqlCe4DbClient(IAdoDriver driver, DbConnection connection, DbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements, IDbPipe pipe)
            : base(driver, connection, transaction, connectionManager, sqlStatements, pipe) { }

        protected override IDbBulkCopy GetBulkCopy()
        {
            return new SqlCe4DbBulkCopy(this);
        }

        public override void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
        {
            if (!sql.IsMultiStatement())
                base.ExecuteNonQuery(sql, parameters);
            else
                base.ExecuteNonQuery(sql.ToSqlStatements(), parameters);
        }

        public override long CheckOutAndGetNextIdentity(string entityName, int numOfIds)
        {
            Ensure.That(entityName, "entityName").IsNotNullOrWhiteSpace();

            var nextId = ExecuteScalar<long>(SqlStatements.GetSql("Sys_Identities_GetNext"), new DacParameter(DbSchemaInfo.Parameters.EntityNameParamPrefix, entityName));

            ExecuteNonQuery(SqlStatements.GetSql("Sys_Identities_Increase"),
                new DacParameter(DbSchemaInfo.Parameters.EntityNameParamPrefix, entityName),
                new DacParameter("numOfIds", numOfIds));

            return nextId;
        }

        protected override void OnBeforeRenameOfStructureSet(string oldStructureTableName, string oldSpatialTableName, string oldUniquesTableName, IndexesTableNames oldIndexesTableNames)
        {
            var dropFkContraintSqlFormat = SqlStatements.GetSql("DropFkContraint");

            if(TableExists(oldSpatialTableName))
                ExecuteNonQuery(dropFkContraintSqlFormat.Inject(oldSpatialTableName, oldStructureTableName));
            ExecuteNonQuery(dropFkContraintSqlFormat.Inject(oldUniquesTableName, oldStructureTableName));
            
            foreach (var oldIndexTableName in oldIndexesTableNames.All)
                ExecuteNonQuery(dropFkContraintSqlFormat.Inject(oldIndexTableName, oldStructureTableName));
        }

        protected override void OnAfterRenameOfStructureSet(string newStructureTableName, string newSpatialTableName, string newUniquesTableName, IndexesTableNames newIndexesTableNames)
        {
            var addFkContraintSqlFormat = SqlStatements.GetSql("AddFkContraintAgainstStructureId");

            if (TableExists(newSpatialTableName))
                ExecuteNonQuery(addFkContraintSqlFormat.Inject(newSpatialTableName, newStructureTableName));
            ExecuteNonQuery(addFkContraintSqlFormat.Inject(newUniquesTableName, newStructureTableName));

            foreach (var newIndexTableName in newIndexesTableNames.All)
                ExecuteNonQuery(addFkContraintSqlFormat.Inject(newIndexTableName, newStructureTableName));
        }

        protected override void OnRenameStructureTable(string oldTableName, string newTableName)
        {
            var dropPkContraintSqlFormat = SqlStatements.GetSql("DropPkConstraint");
            var addPkConstraintSqlFormat = SqlStatements.GetSql("AddPkConstraintAgainstStructureId");

            ExecuteNonQuery(dropPkContraintSqlFormat.Inject(oldTableName));
            ExecuteNonQuery("sp_rename @objname=@objname, @newname=@newname, @objtype=@objtype",
                new DacParameter("objname", oldTableName),
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT"));
            ExecuteNonQuery(addPkConstraintSqlFormat.Inject(newTableName));
        }

        protected override void OnRenameSpatialTable(string oldTableName, string newTableName, string oldStructureTableName, string newStructureTableName)
        {
            ExecuteNonQuery("sp_rename @objname=@objname, @newname=@newname, @objtype=@objtype",
                new DacParameter("objname", oldTableName),
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT"));
        }

        protected override void OnRenameUniquesTable(string oldTableName, string newTableName, string oldStructureTableName, string newStructureTableName)
        {
            ExecuteNonQuery("sp_rename @objname=@objname, @newname=@newname, @objtype=@objtype",
                new DacParameter("objname", oldTableName),
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT"));
        }

        protected override void OnRenameIndexesTables(IndexesTableNames oldIndexesTableNames, IndexesTableNames newIndexesTableNames, string oldStructureTableName, string newStructureTableName)
        {
            using (var cmd = CreateCommand(null))
            {
                for (var i = 0; i < oldIndexesTableNames.All.Length; i++)
                {
                    var oldTableName = oldIndexesTableNames[i];
                    var newTableName = newIndexesTableNames[i];

                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd,
                        new DacParameter("objname", oldTableName),
                        new DacParameter("newname", newTableName),
                        new DacParameter("objtype", "OBJECT"));
                    cmd.CommandText = "sp_rename @objname=@objname, @newname=@newname, @objtype=@objtype";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public override void Drop(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var modelInfo = GetModelTablesInfo(structureSchema);

            var sqlDropTableFormat = SqlStatements.GetSql("DropTable");

            using (var cmd = CreateCommand(string.Empty, new DacParameter(DbSchemaInfo.Parameters.EntityNameParamPrefix, structureSchema.Name)))
            {
                DropIndexesTables(cmd, modelInfo.Statuses.IndexesTableStatuses, modelInfo.Names.IndexesTableNames);

                if (modelInfo.Statuses.UniquesTableExists)
                {
                    cmd.CommandText = sqlDropTableFormat.Inject(structureSchema.GetUniquesTableName());
                    cmd.ExecuteNonQuery();
                }

                if (modelInfo.Statuses.SpatialTableExists)
                {
                    cmd.CommandText = sqlDropTableFormat.Inject(structureSchema.GetStructureTableName());
                    cmd.ExecuteNonQuery();
                }

                if (modelInfo.Statuses.StructureTableExists)
                {
                    cmd.CommandText = sqlDropTableFormat.Inject(structureSchema.GetStructureTableName());
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = SqlStatements.GetSql("DeleteStructureFromSisoDbIdentitiesTable");
                cmd.ExecuteNonQuery();
            }
        }

        private void DropIndexesTables(IDbCommand cmd, IndexesTableStatuses statuses, IndexesTableNames names)
        {
            var sqlDropTableFormat = SqlStatements.GetSql("DropTable");

            if (statuses.IntegersTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.IntegersTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.FractalsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.FractalsTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.BooleansTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.BooleansTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.DatesTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.DatesTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.GuidsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.GuidsTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.StringsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.StringsTableName);
                cmd.ExecuteNonQuery();
            }

            if (statuses.TextsTableExists)
            {
                cmd.CommandText = sqlDropTableFormat.Inject(names.TextsTableName);
                cmd.ExecuteNonQuery();
            }
        }

        public override void UpsertSp(string name, string createSpSql)
        {
            throw new SisoDbException("SQL CE 4 does not support stored procedures.");
        }

        public override void ClearQueryIndexes(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sqlFormat = SqlStatements.GetSql("EmptyTable");
            var indexesTables = structureSchema.GetIndexesTableNames();

            using(var cmd = CreateCommand(null))
            {
                foreach (var tableName in indexesTables.All)
                {
                    cmd.CommandText = sqlFormat.Inject(tableName);
                    cmd.ExecuteNonQuery();
                }   
            }
        }

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