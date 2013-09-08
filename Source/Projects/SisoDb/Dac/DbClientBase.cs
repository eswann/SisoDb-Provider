using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Dac
{
    public abstract class DbClientBase : IDbClient
    {
        protected readonly IConnectionManager ConnectionManager;
        protected readonly ISqlStatements SqlStatements;
        protected readonly IDbPipe Pipe;
        protected readonly List<Action<IDbClient>> OnCompletedHandlers = new List<Action<IDbClient>>();
        protected readonly List<Action<IDbClient>> AfterCommitHandlers = new List<Action<IDbClient>>();
        protected readonly List<Action<IDbClient>> AfterRollbackHandlers = new List<Action<IDbClient>>();

        public Guid Id { get; private set; }
        public bool IsTransactional { get; private set; }
        public bool IsAborted { get; protected set; }
        public bool IsFailed { get; protected set; }
        public IAdoDriver Driver { get; private set; }
        public DbConnection Connection { get; private set; }
        public DbTransaction Transaction { get; private set; }
        protected bool HasPipe { get { return Pipe != null; } }

        public Action<IDbClient> OnCompleted
        {
            set
            {
                Ensure.That(value, "OnCompleted").IsNotNull();
                OnCompletedHandlers.Add(value);
            }
        }
        public Action<IDbClient> AfterCommit
        {
            set
            {
                Ensure.That(value, "AfterCommit").IsNotNull();
                AfterCommitHandlers.Add(value);
            }
        }
        public Action<IDbClient> AfterRollback
        {
            set
            {
                Ensure.That(value, "AfterRollback").IsNotNull();
                AfterRollbackHandlers.Add(value);
            }
        }

        protected DbClientBase(IAdoDriver driver, DbConnection connection, IConnectionManager connectionManager, ISqlStatements sqlStatements, IDbPipe pipe)
            : this(driver, connection, connectionManager, sqlStatements, false, null, pipe) { }

        protected DbClientBase(IAdoDriver driver, DbConnection connection, DbTransaction transaction, IConnectionManager connectionManager, ISqlStatements sqlStatements, IDbPipe pipe)
            : this(driver, connection, connectionManager, sqlStatements, true, transaction, pipe) { }

        private DbClientBase(IAdoDriver driver, DbConnection connection, IConnectionManager connectionManager, ISqlStatements sqlStatements, bool isTransactional, DbTransaction transaction, IDbPipe pipe)
        {
            Ensure.That(driver, "driver").IsNotNull();
            Ensure.That(connection, "connection").IsNotNull();
            Ensure.That(connectionManager, "connectionManager").IsNotNull();
            Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            Id = Guid.NewGuid();
            Driver = driver;
            ConnectionManager = connectionManager;
            Connection = connection;
            SqlStatements = sqlStatements;
            IsTransactional = isTransactional || transaction != null;
            Transaction = transaction;
            Pipe = pipe;
        }

        protected abstract IDbBulkCopy GetBulkCopy();

        public virtual void Dispose()
        {
            if(Connection == null) 
                throw new ObjectDisposedException(typeof(DbClientBase).Name, ExceptionMessages.DbClient_ObjectAllreadyDisposed);
            GC.SuppressFinalize(this);

            var ex = Try.This(TryEndTransaction, TryEndConnection);
            
            InvokeOnCompletedCallbacks();

            if (ex != null)
                throw ex;
        }

        protected virtual void TryEndTransaction()
        {
            if (Transaction == null) return;

            if (IsAborted || IsFailed)
            {
                Transaction.Rollback();
                InvokeAfterRollbackCallbacks();
            }
            else
            {
                Transaction.Commit();
                InvokeAfterCommitCallbacks();
            }

            try
            {
                Transaction.Dispose();
            }
            finally
            {
                Transaction = null;
            }
        }

        protected virtual void InvokeAfterCommitCallbacks()
        {
            try
            {
                AfterCommitHandlers.ForEach(a => a.Invoke(this));
            }
            catch {}
            finally
            {
                AfterCommitHandlers.Clear();
            }
        }

        protected virtual void InvokeAfterRollbackCallbacks()
        {
            try
            {
                AfterRollbackHandlers.ForEach(a => a.Invoke(this));
            }
            catch {}
            finally
            {
                AfterRollbackHandlers.Clear();
            }
        }

        protected virtual void TryEndConnection()
        {
            try
            {
                ConnectionManager.ReleaseClientConnection(Connection);
            }
            finally
            {
                Connection = null;
            }
        }

        protected virtual void InvokeOnCompletedCallbacks()
        {
            try
            {
                OnCompletedHandlers.ForEach(a => a.Invoke(this));
            }
            catch { }
            finally
            {
                OnCompletedHandlers.Clear();
            }
        }

        public virtual void Abort()
        {
            if(IsFailed) return;
            IsAborted = true;
        }

        public virtual void MarkAsFailed()
        {
            IsFailed = true;
        }

        public virtual void ExecuteNonQuery(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public virtual async Task ExecuteNonQueryAsync(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public virtual void ExecuteNonQuery(string[] sqls, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(string.Empty, parameters))
            {
                foreach (var sqlStatement in sqls.Where(statement => !string.IsNullOrWhiteSpace(statement)))
                {
                    cmd.CommandText = sqlStatement;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public virtual async Task ExecuteNonQueryAsync(string[] sqls, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(string.Empty, parameters))
            {
                foreach (var sqlStatement in sqls.Where(statement => !string.IsNullOrWhiteSpace(statement)))
                {
                    cmd.CommandText = sqlStatement;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public virtual T ExecuteScalar<T>(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                var value = cmd.ExecuteScalar();

                if (value == null || value == DBNull.Value)
                    return default(T);

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public virtual async Task<T> ExecuteScalarAsync<T>(string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                var value = await cmd.ExecuteScalarAsync();

                if (value == null || value == DBNull.Value)
                    return default(T);

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        public virtual void Read(string sql, Action<IDataRecord> callback, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        callback(reader);
                    }
                    reader.Close();
                }
            }
        }

        public virtual async Task ReadAsync(string sql, Action<IDataRecord> callback, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (await reader.ReadAsync())
                    {
                        callback(reader);
                    }
                    reader.Close();
                }
            }
        }

        public virtual long CheckOutAndGetNextIdentity(string entityName, int numOfIds)
        {
            var command = CreateCheckoutAndGetNextIdentityCommand(entityName, numOfIds);

            return ExecuteScalar<long>(command.Sql, command.Parameters);
        }

        public virtual async Task<long> CheckOutAndGetNextIdentityAsync(string entityName, int numOfIds)
        {
            var command = CreateCheckoutAndGetNextIdentityCommand( entityName, numOfIds);

            return await ExecuteScalarAsync<long>(command.Sql, command.Parameters);
        }

        private DacCommand CreateCheckoutAndGetNextIdentityCommand(string entityName, int numOfIds)
        {
            EnsureValidDbObjectName(entityName);

            return new DacCommand
            {
                Sql = SqlStatements.GetSql(DacCommandNames.SysIdentitiesCheckOutAndGetNextIdentity),
                Parameters = new IDacParameter[]
                                            {
                                                new DacParameter(DbSchemaInfo.Parameters.EntityNameParamPrefix, entityName),
                                                new DacParameter("numOfIds", numOfIds)
                                            }
            };
        }

        public virtual void RenameStructureSet(string oldStructureName, string newStructureName)
        {
            EnsureValidDbObjectName(oldStructureName);
            EnsureValidDbObjectName(newStructureName);

            var oldStructureTableName = DbSchemaInfo.GenerateStructureTableName(oldStructureName);
            var newStructureTableName = DbSchemaInfo.GenerateStructureTableName(newStructureName);

            var oldSpatialTableName = DbSchemaInfo.GenerateSpatialTableName(oldStructureName);
            var newSpatialTableName = DbSchemaInfo.GenerateSpatialTableName(newStructureName);

            var oldUniquesTableName = DbSchemaInfo.GenerateUniquesTableName(oldStructureName);
            var newUniquesTableName = DbSchemaInfo.GenerateUniquesTableName(newStructureName);

            var oldIndexesTableNames = new IndexesTableNames(oldStructureName);
            var newIndexesTableNames = new IndexesTableNames(newStructureName);

            if (TableExists(newStructureTableName))
                throw new SisoDbException("There allready seems to exist tables for '{0}' in the database.".Inject(newStructureTableName));

            OnBeforeRenameOfStructureSet(oldStructureTableName, oldSpatialTableName, oldUniquesTableName, oldIndexesTableNames);
            OnRenameStructureTable(oldStructureTableName, newStructureTableName);
            if (TableExists(oldSpatialTableName))
                OnRenameSpatialTable(oldSpatialTableName, newSpatialTableName, oldStructureTableName, newStructureTableName);
            OnRenameUniquesTable(oldUniquesTableName, newUniquesTableName, oldStructureTableName, newStructureTableName);
            OnRenameIndexesTables(oldIndexesTableNames, newIndexesTableNames, oldStructureTableName, newStructureTableName);
            OnAfterRenameOfStructureSet(newStructureTableName, newSpatialTableName, newUniquesTableName, newIndexesTableNames);
        }

        protected virtual void OnBeforeRenameOfStructureSet(string oldStructureTableName, string oldSpatialTableName, string oldUniquesTableName, IndexesTableNames oldIndexesTableNames) { }

        protected virtual void OnAfterRenameOfStructureSet(string newStructureTableName, string newSpatialTableName, string newUniquesTableName, IndexesTableNames newIndexesTableNames) { }

        protected virtual void OnRenameStructureTable(string oldTableName, string newTableName)
        {
            using (var cmd = CreateSpCommand(DacCommandNames.SpRename, 
                new DacParameter("objname", oldTableName), 
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT")))
            {
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Concat("PK_", oldTableName)), 
                    new DacParameter("newname", string.Concat("PK_", newTableName)), 
                    new DacParameter("objtype", "OBJECT"));
                cmd.ExecuteNonQuery();
            }
        }

        protected virtual void OnRenameSpatialTable(string oldTableName, string newTableName, string oldStructureTableName, string newStructureTableName)
        {
            using (var cmd = CreateSpCommand(DacCommandNames.SpRename,
                new DacParameter("objname", oldTableName),
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT")))
            {
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Concat("PK_", oldTableName)),
                    new DacParameter("newname", string.Concat("PK_", newTableName)),
                    new DacParameter("objtype", "OBJECT"));
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Format("FK_{0}_{1}", oldTableName, oldStructureTableName)),
                    new DacParameter("newname", string.Format("FK_{0}_{1}", newTableName, newStructureTableName)),
                    new DacParameter("objtype", "OBJECT"));
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Format("{0}.SPK_{1}", newTableName, oldTableName)),
                    new DacParameter("newname", string.Format("SPK_{0}", newTableName)),
                    new DacParameter("objtype", "INDEX"));
                cmd.ExecuteNonQuery();
            }
        }

        protected virtual void OnRenameUniquesTable(string oldTableName, string newTableName, string oldStructureTableName, string newStructureTableName)
        {
            using (var cmd = CreateSpCommand(DacCommandNames.SpRename, 
                new DacParameter("objname", oldTableName), 
                new DacParameter("newname", newTableName),
                new DacParameter("objtype", "OBJECT")))
            {
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Format("FK_{0}_{1}", oldTableName, oldStructureTableName)), 
                    new DacParameter("newname", string.Format("FK_{0}_{1}", newTableName, newStructureTableName)), 
                    new DacParameter("objtype", "OBJECT"));
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                Driver.AddCommandParametersTo(cmd,
                    new DacParameter("objname", string.Format("{0}.UQ_{1}", newTableName, oldTableName)),
                    new DacParameter("newname", string.Concat("UQ_", newTableName)),
                    new DacParameter("objtype", "INDEX"));
                cmd.ExecuteNonQuery();
            }
        }

        protected virtual void OnRenameIndexesTables(IndexesTableNames oldIndexesTableNames, IndexesTableNames newIndexesTableNames, string oldStructureTableName, string newStructureTableName)
        {
            using (var cmd = CreateSpCommand(DacCommandNames.SpRename))
            {
                for(var i = 0; i < oldIndexesTableNames.All.Length; i++)
                {
                    var oldTableName = oldIndexesTableNames[i];
                    var newTableName = newIndexesTableNames[i];

                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd,
                        new DacParameter("objname", oldTableName),
                        new DacParameter("newname", newTableName),
                        new DacParameter("objtype", "OBJECT"));
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd,
                        new DacParameter("objname", string.Format("FK_{0}_{1}", oldTableName, oldStructureTableName)),
                        new DacParameter("newname", string.Format("FK_{0}_{1}", newTableName, newStructureTableName)),
                        new DacParameter("objtype", "OBJECT"));
                    cmd.ExecuteNonQuery();

                    if (oldIndexesTableNames.HasSidIndex(oldTableName))
                    {
                        cmd.Parameters.Clear();
                        Driver.AddCommandParametersTo(cmd,
                            new DacParameter("objname", string.Format("{0}.IX_{1}_SID", newTableName, oldTableName)),
                            new DacParameter("newname", string.Format("IX_{0}_SID", newTableName)),
                            new DacParameter("objtype", "INDEX"));
                        cmd.ExecuteNonQuery();   
                    }

                    cmd.Parameters.Clear();
                    Driver.AddCommandParametersTo(cmd,
                        new DacParameter("objname", string.Format("{0}.IX_{1}_Q", newTableName, oldTableName)),
                        new DacParameter("newname", string.Format("IX_{0}_Q", newTableName)),
                        new DacParameter("objtype", "INDEX"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public virtual void Drop(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var names = new ModelTableNames(structureSchema);
            EnsureValidNames(names);

            var sql = SqlStatements.GetSql(DacCommandNames.DropStructureTables).Inject(
                names.IndexesTableNames.IntegersTableName,
                names.IndexesTableNames.FractalsTableName,
                names.IndexesTableNames.BooleansTableName,
                names.IndexesTableNames.DatesTableName,
                names.IndexesTableNames.GuidsTableName,
                names.IndexesTableNames.StringsTableName,
                names.IndexesTableNames.TextsTableName,
                names.SpatialTableName,
                names.UniquesTableName,
                names.StructureTableName);

            using (var cmd = CreateCommand(sql, new DacParameter(DbSchemaInfo.Parameters.EntityNameParamPrefix, structureSchema.Name)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void UpsertSp(string name, string createSpSql)
        {
            EnsureValidDbObjectName(name);
            Ensure.That(createSpSql, "createSpSql").IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql(DacCommandNames.DropSp).Inject(name);

            ExecuteNonQuery(sql);
            ExecuteNonQuery(createSpSql);
        }

        public virtual void Reset()
        {
            var tableNamesToDrop = new List<string>();
            var sql = SqlStatements.GetSql(DacCommandNames.GetTableNamesForAllDataTables);
            var dropTableTemplate = SqlStatements.GetSql(DacCommandNames.DropTable);

            using (var cmd = CreateCommand(sql))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                        tableNamesToDrop.Add(reader.GetString(0));
                    reader.Close();
                }

                foreach (var tableName in tableNamesToDrop)
                {
                    cmd.CommandText = dropTableTemplate.Inject(tableName);
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = SqlStatements.GetSql(DacCommandNames.TruncateSisoDbIdentities);
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void ClearQueryIndexes(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var names = new ModelTableNames(structureSchema);
            EnsureValidNames(names);

            var sql = SqlStatements.GetSql(DacCommandNames.ClearIndexesTables).Inject(
                names.IndexesTableNames.IntegersTableName,
                names.IndexesTableNames.FractalsTableName,
                names.IndexesTableNames.BooleansTableName,
                names.IndexesTableNames.DatesTableName,
                names.IndexesTableNames.GuidsTableName,
                names.IndexesTableNames.StringsTableName,
                names.IndexesTableNames.TextsTableName);

            using (var cmd = CreateCommand(sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public virtual void DeleteAll(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.DeleteAll).Inject(structureSchema.GetStructureTableName());

            ExecuteNonQuery(sql);
        }

        public virtual async Task DeleteAllAsync(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.DeleteAll).Inject(structureSchema.GetStructureTableName());

            await ExecuteNonQueryAsync(sql);
        }

        public abstract void DeleteAllExceptIds(IEnumerable<IStructureId> structureIds, IStructureSchema structureSchema);

        public abstract Task DeleteAllExceptIdsAsync(IEnumerable<IStructureId> structureIds, IStructureSchema structureSchema);

        public virtual void DeleteById(IStructureId structureId, IStructureSchema structureSchema)
        {
            var sql = CreateDeleteByIdSql(structureSchema);

            ExecuteNonQuery(sql, new DacParameter("id", structureId.Value));
        }

        public virtual async Task DeleteByIdAsync(IStructureId structureId, IStructureSchema structureSchema)
        {
            var sql = CreateDeleteByIdSql(structureSchema);

            await ExecuteNonQueryAsync(sql, new DacParameter("id", structureId.Value));
        }

        private string CreateDeleteByIdSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            return SqlStatements.GetSql(DacCommandNames.DeleteById).Inject(structureSchema.GetStructureTableName());
        }

        public abstract void DeleteByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

        public abstract Task DeleteByIdsAsync(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

        public virtual void DeleteByQuery(IDbQuery query, IStructureSchema structureSchema)
        {
            string sql = CreateDeleteByIdSql(query, structureSchema);

            ExecuteNonQuery(sql, query.Parameters);
        }

        public virtual async Task DeleteByQueryAsync(IDbQuery query, IStructureSchema structureSchema)
        {
            string sql = CreateDeleteByIdSql(query, structureSchema);

            await ExecuteNonQueryAsync(sql, query.Parameters);
        }

        private string CreateDeleteByIdSql(IDbQuery query, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            return SqlStatements.GetSql(DacCommandNames.DeleteByQuery).Inject(
                structureSchema.GetStructureTableName(),
                query.Sql);
        }

        public virtual void DeleteIndexesAndUniquesById(IStructureId structureId, IStructureSchema structureSchema)
        {
            var sql = CreateDeleteIndexesAndUniquesByIdSql(structureSchema);

            ExecuteNonQuery(sql, new DacParameter("id", structureId.Value));
        }

        public virtual async Task DeleteIndexesAndUniquesByIdAsync(IStructureId structureId, IStructureSchema structureSchema)
        {
           var sql = CreateDeleteIndexesAndUniquesByIdSql(structureSchema);

            await ExecuteNonQueryAsync(sql, new DacParameter("id", structureId.Value));
        }

        public string CreateDeleteIndexesAndUniquesByIdSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var indexesTableNames = structureSchema.GetIndexesTableNames();
            var uniquesTableName = structureSchema.GetUniquesTableName();

            var sql = SqlStatements.GetSql(DacCommandNames.DeleteIndexesAndUniquesById).Inject(
                uniquesTableName,
                indexesTableNames.BooleansTableName,
                indexesTableNames.DatesTableName,
                indexesTableNames.FractalsTableName,
                indexesTableNames.GuidsTableName,
                indexesTableNames.IntegersTableName,
                indexesTableNames.StringsTableName,
                indexesTableNames.TextsTableName);
            return sql;
        }

        public virtual bool TableExists(string name)
        {
            EnsureValidDbObjectName(name);

            var sql = SqlStatements.GetSql(DacCommandNames.TableExists);
            var value = ExecuteScalar<int>(sql, new DacParameter(DbSchemaInfo.Parameters.TableNameParamPrefix, name));

            return value > 0;
        }

        public virtual ModelTablesInfo GetModelTablesInfo(IStructureSchema structureSchema)
        {
            var names = new ModelTableNames(structureSchema);
            EnsureValidNames(names);

            return new ModelTablesInfo(names, GetModelTableStatuses(names));
        }

        public virtual ModelTableStatuses GetModelTableStatuses(ModelTableNames names)
        {
            var sql = SqlStatements.GetSql(DacCommandNames.GetModelTableStatuses);
            var parameters = names.AllTableNames.Select((n, i) => new DacParameter(DbSchemaInfo.Parameters.TableNameParamPrefix + i, n)).ToArray();
            var matchingNames = new HashSet<string>();
            Read(sql, dr => matchingNames.Add(dr.GetString(0)), parameters);

            return new ModelTableStatuses(
                matchingNames.Contains(names.StructureTableName),
                matchingNames.Contains(names.SpatialTableName),
                matchingNames.Contains(names.UniquesTableName),
                new IndexesTableStatuses(
                    matchingNames.Contains(names.IndexesTableNames.IntegersTableName),
                    matchingNames.Contains(names.IndexesTableNames.FractalsTableName),
                    matchingNames.Contains(names.IndexesTableNames.DatesTableName),
                    matchingNames.Contains(names.IndexesTableNames.BooleansTableName),
                    matchingNames.Contains(names.IndexesTableNames.GuidsTableName),
                    matchingNames.Contains(names.IndexesTableNames.StringsTableName),
                    matchingNames.Contains(names.IndexesTableNames.TextsTableName)));
        }

        public virtual IEnumerable<TId> GetStructureIds<TId>(IStructureSchema structureSchema, IDbQuery query)
        {
            var read = PrepareStructureIdQueryProcess<TId>(structureSchema, query);

            using (var cmd = CreateCommand(query.Sql, query.Parameters))
            {
                using (var reader = cmd.SingleResultSequentialReader())
                {
                    while(reader.Read())
                        yield return (TId)read(reader);
                }
            }
        }

        public virtual async Task<IEnumerable<TId>> GetStructureIdsAsync<TId>(IStructureSchema structureSchema, IDbQuery query)
        {
            var read = PrepareStructureIdQueryProcess<TId>(structureSchema, query);

            var returnIds = new List<TId>();

            using (var cmd = CreateCommand(query.Sql, query.Parameters))
            {
                using (var reader = await cmd.SingleResultSequentialReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        returnIds.Add((TId)read(reader));
                    }
                }
            }

            return returnIds;
        }

        private static Func<IDataRecord, object> PrepareStructureIdQueryProcess<TId>(IStructureSchema structureSchema, IDbQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            Func<IDataRecord, object> read = dr => dr.GetGuid(0);
            if (structureSchema.IdAccessor.IdType.IsString())
                read = dr => dr.GetString(0);
            else
                switch (structureSchema.IdAccessor.IdType)
                {
                    case StructureIdTypes.Identity:
                        read = dr => (int) dr.GetInt64(0);
                        break;
                    case StructureIdTypes.BigIdentity:
                        read = dr => dr.GetInt64(0);
                        break;
                }
            return read;
        }

        public virtual int RowCount(IStructureSchema structureSchema)
        {
            var sql = CreateRowCountSql(structureSchema);

            return ExecuteScalar<int>(sql);
        }

        public virtual async Task<int> RowCountAsync(IStructureSchema structureSchema)
        {
            var sql = CreateRowCountSql(structureSchema);

            return await ExecuteScalarAsync<int>(sql);
        }

        private string CreateRowCountSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.RowCount).Inject(structureSchema.GetStructureTableName());
            return sql;
        }

        public virtual int RowCountByQuery(IStructureSchema structureSchema, IDbQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            return ExecuteScalar<int>(query.Sql, query.Parameters);
        }

        public virtual async Task<int> RowCountByQueryAsync(IStructureSchema structureSchema, IDbQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            return await ExecuteScalarAsync<int>(query.Sql, query.Parameters);
        }

        public virtual bool Any(IStructureSchema structureSchema)
        {
            return RowCount(structureSchema) > 0;
        }

        public virtual async Task<bool> AnyAsync(IStructureSchema structureSchema)
        {
            return await RowCountAsync(structureSchema) > 0;
        }

        public virtual bool Any(IStructureSchema structureSchema, IDbQuery query)
        {
            return RowCountByQuery(structureSchema, query) > 0;
        }

        public virtual async Task<bool> AnyAsync(IStructureSchema structureSchema, IDbQuery query)
        {
            return await RowCountByQueryAsync(structureSchema, query) > 0;
        }

        public virtual bool Exists(IStructureSchema structureSchema, IStructureId structureId)
        {
            var sql = GetExistsSql(structureSchema, structureId);

            return ExecuteScalar<int>(sql, new DacParameter("id", structureId.Value)) > 0;
        }

        public virtual async Task<bool> ExistsAsync(IStructureSchema structureSchema, IStructureId structureId)
        {
            var sql = GetExistsSql(structureSchema, structureId);

            return await ExecuteScalarAsync<int>(sql, new DacParameter("id", structureId.Value)) > 0;
        }

        private string GetExistsSql(IStructureSchema structureSchema, IStructureId structureId)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();
            Ensure.That(structureId, "structureId").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.ExistsById).Inject(structureSchema.GetStructureTableName());
            return sql;
        }

        public virtual string GetJsonById(IStructureId structureId, IStructureSchema structureSchema)
        {
            var sql = CreateGetJsonByIdSql(structureSchema);

            var json = ExecuteScalar<string>(sql, new DacParameter("id", structureId.Value));

            return ProcessJsonCommandResult(structureSchema, json);
        }

        public virtual async Task<string> GetJsonByIdAsync(IStructureId structureId, IStructureSchema structureSchema)
        {
            var sql = CreateGetJsonByIdSql(structureSchema);

            var json = await ExecuteScalarAsync<string>(sql, new DacParameter("id", structureId.Value));

            return ProcessJsonCommandResult(structureSchema, json);
        }

        private string CreateGetJsonByIdSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.GetJsonById).Inject(structureSchema.GetStructureTableName());
            return sql;
        }

        private string ProcessJsonCommandResult(IStructureSchema structureSchema, string json)
        {
            if (HasPipe)
                return Pipe.Reading(structureSchema, json);

            return json;
        }

        public virtual string GetJsonByIdWithLock(IStructureId structureId, IStructureSchema structureSchema)
        {
            var sql = GetJsonIdWithLockSql(structureSchema);

            var json = ExecuteScalar<string>(sql, new DacParameter("id", structureId.Value));

            return ProcessJsonCommandResult(structureSchema, json);
        }

        public virtual async Task<string> GetJsonByIdWithLockAsync(IStructureId structureId, IStructureSchema structureSchema)
        {
            var sql = GetJsonIdWithLockSql(structureSchema);

            var json = await ExecuteScalarAsync<string>(sql, new DacParameter("id", structureId.Value));

            return ProcessJsonCommandResult(structureSchema, json);
        }

        private string GetJsonIdWithLockSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.GetJsonByIdWithLock).Inject(structureSchema.GetStructureTableName());
            return sql;
        }

        public virtual IEnumerable<string> GetJsonOrderedByStructureId(IStructureSchema structureSchema)
        {
            var sql = GetJsonOrderedByStructureIdSql(structureSchema);

            return ReadJson(structureSchema, sql);
        }

        public virtual async Task<IEnumerable<string>> GetJsonOrderedByStructureIdAsync(IStructureSchema structureSchema)
        {
            var sql = GetJsonOrderedByStructureIdSql(structureSchema);

            return await ReadJsonAsync(structureSchema, sql);
        }

        private string GetJsonOrderedByStructureIdSql(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql(DacCommandNames.GetAllJson).Inject(structureSchema.GetStructureTableName());
            return sql;
        }

        public abstract IEnumerable<string> GetJsonByIds(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

        public abstract Task<IEnumerable<string>> GetJsonByIdsAsync(IEnumerable<IStructureId> ids, IStructureSchema structureSchema);

        public virtual IEnumerable<string> ReadJson(IStructureSchema structureSchema, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                return RetreiveJson(structureSchema, cmd);
            }
        }

        public virtual async Task<IEnumerable<string>> ReadJsonAsync(IStructureSchema structureSchema, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(sql, parameters))
            {
                return await RetreiveJsonAsync(structureSchema, cmd);
            }
        }

        public virtual IEnumerable<string> ReadJsonBySp(IStructureSchema structureSchema, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateSpCommand(sql, parameters))
            {
                return RetreiveJson(structureSchema, cmd);
            }
        }

        public virtual async Task<IEnumerable<string>> ReadJsonBySpAsync(IStructureSchema structureSchema, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateSpCommand(sql, parameters))
            {
                return await RetreiveJsonAsync(structureSchema, cmd);
            }
        }

        protected virtual IEnumerable<string> RetreiveJson(IStructureSchema structureSchema, DbCommand cmd)
        {
            using (var reader = cmd.SingleResultSequentialReader())
            {
                var i = reader.FieldCount - 1;
                if (HasPipe)
                {
                    while (reader.Read())
                        yield return Pipe.Reading(structureSchema, reader.GetString(i));
                }
                else
                {
                    while (reader.Read())
                        yield return reader.GetString(i);
                }
                reader.Close();
            }
        }

        protected virtual async Task<IEnumerable<string>> RetreiveJsonAsync(IStructureSchema structureSchema, DbCommand cmd)
        {
            var jsonResults = new List<string>();

            using (var reader = await cmd.SingleResultSequentialReaderAsync())
            {
                var i = reader.FieldCount - 1;
                if (HasPipe)
                {
                    while (await reader.ReadAsync())
                        jsonResults.Add(Pipe.Reading(structureSchema, reader.GetString(i)));
                }
                else
                {
                    while (await reader.ReadAsync())
                        jsonResults.Add(reader.GetString(i));
                }
                reader.Close();
            }

            return jsonResults;
        }

        public virtual void BulkInsertStructures(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (structures.Length == 0)
                return;

            if (HasPipe)
            {
                foreach (var structure in structures)
                    structure.Data = Pipe.Writing(structureSchema, structure.Data);
            }

            using (var reader = new StructuresReader(new StructureStorageSchema(structureSchema, structureSchema.GetStructureTableName()), structures))
            {
                using (var bulkInserter = GetBulkCopy())
                {
                    bulkInserter.PrepForStructuresInsert(reader, structures);
                    bulkInserter.Write(reader);
                }
            }
        }

        public virtual async Task BulkInsertStructuresAsync(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (structures.Length == 0)
                return;

            if (HasPipe)
            {
                foreach (var structure in structures)
                    structure.Data = Pipe.Writing(structureSchema, structure.Data);
            }

            using (var reader = new StructuresReader(new StructureStorageSchema(structureSchema, structureSchema.GetStructureTableName()), structures))
            {
                using (var bulkInserter = GetBulkCopy())
                {
                    bulkInserter.PrepForStructuresInsert(reader, structures);
                    await bulkInserter.WriteAsync(reader);
                }
            }
        }

        public virtual void BulkInsertIndexes(IndexesReader reader)
        {
            using (reader)
            {
                if (reader.RecordsAffected < 1)
                    return;

                using (var bulkInserter = GetBulkCopy())
                {
                    bulkInserter.PrepForIndexesInsert(reader);
                    bulkInserter.Write(reader);
                }
            }
        }

        public virtual async Task BulkInsertIndexesAsync(IndexesReader reader)
        {
            using (reader)
            {
                if (reader.RecordsAffected < 1)
                    return;

                using (var bulkInserter = GetBulkCopy())
                {
                    bulkInserter.PrepForIndexesInsert(reader);
                    await bulkInserter.WriteAsync(reader);
                }
            }
        }


        public virtual void BulkInsertUniques(IStructureSchema structureSchema, IStructureIndex[] uniques)
        {
            if (!uniques.Any())
                return;

            using (var reader = new UniquesReader(new UniqueStorageSchema(structureSchema, structureSchema.GetUniquesTableName()), uniques))
            {
                using (var bulkInserter = GetBulkCopy())
                {
                    bulkInserter.PrepForUniquesInsert(reader, uniques);

                    bulkInserter.Write(reader);
                }
            }
        }

        public virtual async Task BulkInsertUniquesAsync(IStructureSchema structureSchema, IStructureIndex[] uniques)
        {
            if (!uniques.Any())
                return;

            using (var reader = new UniquesReader(new UniqueStorageSchema(structureSchema, structureSchema.GetUniquesTableName()), uniques))
            {
                using (var bulkInserter = GetBulkCopy())
                {
                    bulkInserter.PrepForUniquesInsert(reader, uniques);

                    await bulkInserter.WriteAsync(reader);
                }
            }
        }

        public virtual void SingleInsertStructure(IStructure structure, IStructureSchema structureSchema)
        {
            var command = CreateSingleInsertStructureQuery(structure, structureSchema);

            ExecuteNonQuery(command.Sql, command.Parameters);
        }

        public virtual async Task SingleInsertStructureAsync(IStructure structure, IStructureSchema structureSchema)
        {
            var command = CreateSingleInsertStructureQuery(structure, structureSchema);

            await ExecuteNonQueryAsync(command.Sql, command.Parameters);
        }

        private DacCommand CreateSingleInsertStructureQuery(IStructure structure, IStructureSchema structureSchema)
        {
            if (HasPipe)
                structure.Data = Pipe.Writing(structureSchema, structure.Data);

            return new DacCommand
            {
                Sql = SqlStatements.GetSql(DacCommandNames.SingleInsertStructure).Inject(
                    structureSchema.GetStructureTableName(),
                    StructureStorageSchema.Fields.Id.Name,
                    StructureStorageSchema.Fields.Json.Name),
                Parameters = new IDacParameter[]{
                                                     new DacParameter(StructureStorageSchema.Fields.Id.Name, structure.Id.Value),
                                                     new DacParameter(StructureStorageSchema.Fields.Json.Name, structure.Data)
                                                 }
            };
        }

        public virtual void SingleInsertOfValueTypeIndex(IStructureIndex structureIndex, string valueTypeIndexesTableName)
        {
            EnsureValidDbObjectName(valueTypeIndexesTableName);

            var command = CreateSingleInsertOfValueTypeIndexQuery(structureIndex, valueTypeIndexesTableName);

            ExecuteNonQuery(command.Sql, command.Parameters);
        }

        public virtual async Task SingleInsertOfValueTypeIndexAsync(IStructureIndex structureIndex, string valueTypeIndexesTableName)
        {
            EnsureValidDbObjectName(valueTypeIndexesTableName);

            var command = CreateSingleInsertOfValueTypeIndexQuery(structureIndex, valueTypeIndexesTableName);

            await ExecuteNonQueryAsync(command.Sql, command.Parameters);
        }

        private DacCommand CreateSingleInsertOfValueTypeIndexQuery(IStructureIndex structureIndex, string valueTypeIndexesTableName)
        {
            return new DacCommand
            {
                Sql = SqlStatements.GetSql(DacCommandNames.SingleInsertOfValueTypeIndex).Inject(
                valueTypeIndexesTableName,
                IndexStorageSchema.Fields.StructureId.Name,
                IndexStorageSchema.Fields.MemberPath.Name,
                IndexStorageSchema.Fields.Value.Name,
                IndexStorageSchema.Fields.StringValue.Name),
                Parameters = new IDacParameter[]{
                                                    new DacParameter(IndexStorageSchema.Fields.StructureId.Name, structureIndex.StructureId.Value),
                                                    new DacParameter(IndexStorageSchema.Fields.MemberPath.Name, structureIndex.Path),
                                                    new DacParameter(IndexStorageSchema.Fields.Value.Name, structureIndex.Value),
                                                    new DacParameter(IndexStorageSchema.Fields.StringValue.Name, SisoEnvironment.StringConverter.AsString(structureIndex.Value))
                                                 }
            };
        }

        public virtual void SingleInsertOfStringTypeIndex(IStructureIndex structureIndex, string stringishIndexesTableName)
        {
            EnsureValidDbObjectName(stringishIndexesTableName);

            var query = CreateSingleInsertOfStringTypeIndexQuery(structureIndex, stringishIndexesTableName);

            ExecuteNonQuery(query.Sql, query.Parameters);
        }

        public virtual async Task SingleInsertOfStringTypeIndexAsync(IStructureIndex structureIndex, string stringishIndexesTableName)
        {
            EnsureValidDbObjectName(stringishIndexesTableName);

            var query = CreateSingleInsertOfStringTypeIndexQuery(structureIndex, stringishIndexesTableName);

            await ExecuteNonQueryAsync(query.Sql, query.Parameters);
        }

        private DacCommand CreateSingleInsertOfStringTypeIndexQuery(IStructureIndex structureIndex, string stringishIndexesTableName)
        {
            return new DacCommand
            {
                Sql = SqlStatements.GetSql(DacCommandNames.SingleInsertOfStringTypeIndex).Inject(
                    stringishIndexesTableName,
                    IndexStorageSchema.Fields.StructureId.Name,
                    IndexStorageSchema.Fields.MemberPath.Name,
                    IndexStorageSchema.Fields.Value.Name),
                Parameters = new IDacParameter[]{
                                                    new DacParameter(IndexStorageSchema.Fields.StructureId.Name, structureIndex.StructureId.Value),
                                                    new DacParameter(IndexStorageSchema.Fields.MemberPath.Name, structureIndex.Path),
                                                    new DacParameter(IndexStorageSchema.Fields.Value.Name, structureIndex.Value == null ? null : structureIndex.Value.ToString())
                                                }
            };
        }


        public virtual void SingleInsertOfUniqueIndex(IStructureIndex uniqueStructureIndex, IStructureSchema structureSchema)
        {
            var query = CreateSingleInsertOfUniqueIndexQuery(uniqueStructureIndex, structureSchema);

            ExecuteNonQuery(query.Sql, query.Parameters);
        }

        public virtual async Task SingleInsertOfUniqueIndexAsync(IStructureIndex uniqueStructureIndex, IStructureSchema structureSchema)
        {
            var query = CreateSingleInsertOfUniqueIndexQuery(uniqueStructureIndex, structureSchema);

            await ExecuteNonQueryAsync(query.Sql, query.Parameters);
        }

        private DacCommand CreateSingleInsertOfUniqueIndexQuery(IStructureIndex uniqueStructureIndex, IStructureSchema structureSchema)
        {
            return new DacCommand
            {
                Sql = SqlStatements.GetSql(DacCommandNames.SingleInsertOfUniqueIndex).Inject(
                    structureSchema.GetUniquesTableName(),
                    UniqueStorageSchema.Fields.StructureId.Name,
                    UniqueStorageSchema.Fields.UqStructureId.Name,
                    UniqueStorageSchema.Fields.UqMemberPath.Name,
                    UniqueStorageSchema.Fields.UqValue.Name),
                Parameters = new IDacParameter[]{
                                                    new DacParameter(UniqueStorageSchema.Fields.StructureId.Name, uniqueStructureIndex.StructureId.Value),
                                                    (uniqueStructureIndex.IndexType == StructureIndexType.UniquePerType)
                                                        ? new DacParameter(UniqueStorageSchema.Fields.UqStructureId.Name, DBNull.Value)
                                                        : new DacParameter(UniqueStorageSchema.Fields.UqStructureId.Name, uniqueStructureIndex.StructureId.Value),
                                                    new DacParameter(UniqueStorageSchema.Fields.UqMemberPath.Name, uniqueStructureIndex.Path),
                                                    new DacParameter(UniqueStorageSchema.Fields.UqValue.Name, UniquesChecksumGenerator.Instance.Generate(uniqueStructureIndex))
                                                }
            };
        }

        public virtual void SingleUpdateOfStructure(IStructure structure, IStructureSchema structureSchema)
        {
            var command = CreateSingleUpdateOfStructureQuery(structure, structureSchema);

            ExecuteNonQuery(command.Sql, command.Parameters);
        }

        public virtual async Task SingleUpdateOfStructureAsync(IStructure structure, IStructureSchema structureSchema)
        {
            var command = CreateSingleUpdateOfStructureQuery(structure, structureSchema);

            await ExecuteNonQueryAsync(command.Sql, command.Parameters);
        }

        private DacCommand CreateSingleUpdateOfStructureQuery(IStructure structure, IStructureSchema structureSchema)
        {
            if (HasPipe)
                structure.Data = Pipe.Writing(structureSchema, structure.Data);

            return new DacCommand
            {
                Sql = SqlStatements.GetSql(DacCommandNames.SingleUpdateOfStructure).Inject(
                structureSchema.GetStructureTableName(),
                StructureStorageSchema.Fields.Json.Name,
                StructureStorageSchema.Fields.Id.Name),
                Parameters = new IDacParameter[]{
                                                    new DacParameter(StructureStorageSchema.Fields.Json.Name, structure.Data),
                                                    new DacParameter(StructureStorageSchema.Fields.Id.Name, structure.Id.Value)
                                                }
            };
        }

        protected virtual void EnsureValidNames(ModelTableNames names)
        {
            foreach (var tableName in names.AllTableNames)
                EnsureValidDbObjectName(tableName);
        }

        protected virtual void EnsureValidDbObjectName(string dbObjectName)
        {
            DbObjectNameValidator.EnsureValid(dbObjectName);
        }

        protected virtual DbCommand CreateCommand(string sql, params IDacParameter[] parameters)
        {
            return Driver.CreateCommand(Connection, sql, Transaction, parameters);
        }

        protected virtual DbCommand CreateSpCommand(string spName, params IDacParameter[] parameters)
        {
            return Driver.CreateSpCommand(Connection, spName, Transaction, parameters);
        }
    }
}