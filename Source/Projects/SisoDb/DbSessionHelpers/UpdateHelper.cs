using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal class UpdateHelper : DbSessionHelper
    {
        public UpdateHelper(DbSession session) : base(session)
        {
        }

        public void Update(Type structureType, object item)
        {
            Ensure.That(item, "item").IsNotNull();
            var structureSchema = UpsertStructureSchema(structureType);
            var structureId = structureSchema.IdAccessor.GetValue(item);

            if (!structureSchema.HasConcurrencyToken)
            {
                var exists = DbClient.Exists(structureSchema, structureId);
                if (!exists)
                    throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));
            }
            else
                EnsureConcurrencyTokenIsValid(structureSchema, structureId, item, item.GetType());

            PrepareCacheForUpdate(structureSchema, structureId);
            DbClient.DeleteIndexesAndUniquesById(structureId, structureSchema);

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForUpdate(structureSchema);
            var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

            var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            bulkInserter.Replace(structureSchema, updatedStructure);
            InternalEvents.NotifyUpdated(Session, structureSchema, updatedStructure, item);
        }

        public async Task UpdateAsync(Type structureType, object item)
        {
            Ensure.That(item, "item").IsNotNull();
            var structureSchema = UpsertStructureSchema(structureType);
            var structureId = structureSchema.IdAccessor.GetValue(item);

            if (!structureSchema.HasConcurrencyToken)
            {
                var exists = await DbClient.ExistsAsync(structureSchema, structureId);
                if (!exists)
                    throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));
            }
            else
                await EnsureConcurrencyTokenIsValidAsync(structureSchema, structureId, item, item.GetType());

            PrepareCacheForUpdate(structureSchema, structureId);
            await DbClient.DeleteIndexesAndUniquesByIdAsync(structureId, structureSchema);

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForUpdate(structureSchema);
            var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);

            var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            await bulkInserter.ReplaceAsync(structureSchema, updatedStructure);
            InternalEvents.NotifyUpdated(Session, structureSchema, updatedStructure, item);
        }

        public void Update<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class
        {
            var structureSchema = PrepareUpdate<TContract, TImpl>(id, modifier);
            var structureId = StructureId.ConvertFrom(id);

            var existingJson = DbClient.GetJsonByIdWithLock(structureId, structureSchema);

            if (string.IsNullOrWhiteSpace(existingJson))
                throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));

            var item = Db.Serializer.Deserialize<TImpl>(existingJson);

            modifier.Invoke(item);
            if (proceed != null && !proceed.Invoke(item))
                return;

            if (structureSchema.HasConcurrencyToken)
                EnsureConcurrencyTokenIsValid(structureSchema, structureId, item, typeof(TImpl));

            PrepareCacheForUpdate(structureSchema, structureId);
            DbClient.DeleteIndexesAndUniquesById(structureId, structureSchema);

            var updatedStructure = BuildStructure(structureSchema, item);

            var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            bulkInserter.Replace(structureSchema, updatedStructure);
            InternalEvents.NotifyUpdated(Session, structureSchema, updatedStructure, item);
        }

        public async Task UpdateAsync<TContract, TImpl>(object id, Action<TImpl> modifier, Func<TImpl, bool> proceed = null)
            where TContract : class
            where TImpl : class
        {
            var structureSchema = PrepareUpdate<TContract, TImpl>(id, modifier);
            var structureId = StructureId.ConvertFrom(id);

            var existingJson = await DbClient.GetJsonByIdWithLockAsync(structureId, structureSchema);

            if (string.IsNullOrWhiteSpace(existingJson))
                throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));

            var item = Db.Serializer.Deserialize<TImpl>(existingJson);

            modifier.Invoke(item);
            if (proceed != null && !proceed.Invoke(item))
                return;

            if (structureSchema.HasConcurrencyToken)
                await EnsureConcurrencyTokenIsValidAsync(structureSchema, structureId, item, typeof(TImpl));

            PrepareCacheForUpdate(structureSchema, structureId);
            await DbClient.DeleteIndexesAndUniquesByIdAsync(structureId, structureSchema);

            var updatedStructure = BuildStructure(structureSchema, item);

            var bulkInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            await bulkInserter.ReplaceAsync(structureSchema, updatedStructure);

            InternalEvents.NotifyUpdated(Session, structureSchema, updatedStructure, item);
        }

        public void UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            var structureSchema = PrepareUpdateMany(predicate, modifier);

            var deleteIds = new HashSet<IStructureId>();
            var keepQueue = new List<T>(Db.Settings.MaxUpdateManyBatchSize);
            var structureBuilder = Db.StructureBuilders.ResolveBuilderForUpdate(structureSchema);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            var sqlQuery = BuildUpdateManySql(predicate);

            foreach (var structure in Db.Serializer.DeserializeMany<T>(
                DbClient.ReadJson(structureSchema, sqlQuery.Sql, sqlQuery.Parameters)))
            {
                var structureIdBefore = structureSchema.IdAccessor.GetValue(structure);
                modifier.Invoke(structure);
                var structureIdAfter = structureSchema.IdAccessor.GetValue(structure);

                if (!structureIdBefore.Value.Equals(structureIdAfter.Value))
                    throw new SisoDbException(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(
                            structureIdAfter.Value, structureIdBefore.Value));

                deleteIds.Add(structureIdBefore);

                keepQueue.Add(structure);
                if (keepQueue.Count < Db.Settings.MaxUpdateManyBatchSize)
                    continue;

                Db.CacheProvider.Remove(structureSchema, deleteIds);
                DbClient.DeleteByIds(deleteIds, structureSchema);
                deleteIds.Clear();

                var items = keepQueue.ToArray();
                var structures = structureBuilder.CreateStructures(items, structureSchema);
                structureInserter.Insert(structureSchema, structures);
                keepQueue.Clear();
                InternalEvents.NotifyUpdated(Session, structureSchema, structures, items);
            }

            if (keepQueue.Count > 0)
            {
                Db.CacheProvider.Remove(structureSchema, deleteIds);
                DbClient.DeleteByIds(deleteIds, structureSchema);
                deleteIds.Clear();

                var items = keepQueue.ToArray();
                var structures = structureBuilder.CreateStructures(items, structureSchema);
                structureInserter.Insert(structureSchema, structures);
                keepQueue.Clear();
                InternalEvents.NotifyUpdated(Session, structureSchema, structures, items);
            }
        }

        public async Task UpdateManyAsync<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            var structureSchema = PrepareUpdateMany(predicate, modifier);

            var deleteIds = new HashSet<IStructureId>();
            var keepQueue = new List<T>(Db.Settings.MaxUpdateManyBatchSize);
            var structureBuilder = Db.StructureBuilders.ResolveBuilderForUpdate(structureSchema);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            var sqlQuery = BuildUpdateManySql(predicate);

            foreach (var structure in Db.Serializer.DeserializeMany<T>(
                await DbClient.ReadJsonAsync(structureSchema, sqlQuery.Sql, sqlQuery.Parameters)))
            {
                var structureIdBefore = structureSchema.IdAccessor.GetValue(structure);
                modifier.Invoke(structure);
                var structureIdAfter = structureSchema.IdAccessor.GetValue(structure);

                if (!structureIdBefore.Value.Equals(structureIdAfter.Value))
                    throw new SisoDbException(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(
                            structureIdAfter.Value, structureIdBefore.Value));

                deleteIds.Add(structureIdBefore);

                keepQueue.Add(structure);
                if (keepQueue.Count < Db.Settings.MaxUpdateManyBatchSize)
                    continue;

                Db.CacheProvider.Remove(structureSchema, deleteIds);
                await DbClient.DeleteByIdsAsync(deleteIds, structureSchema);
                deleteIds.Clear();

                var items = keepQueue.ToArray();
                var structures = structureBuilder.CreateStructures(items, structureSchema);
                await structureInserter.InsertAsync(structureSchema, structures);
                keepQueue.Clear();
                InternalEvents.NotifyUpdated(Session, structureSchema, structures, items);
            }

            if (keepQueue.Count > 0)
            {
                Db.CacheProvider.Remove(structureSchema, deleteIds);
                await DbClient.DeleteByIdsAsync(deleteIds, structureSchema);
                deleteIds.Clear();

                var items = keepQueue.ToArray();
                var structures = structureBuilder.CreateStructures(items, structureSchema);
                await structureInserter.InsertAsync(structureSchema, structures);
                keepQueue.Clear();
                InternalEvents.NotifyUpdated(Session, structureSchema, structures, items);
            }
        }

        private IStructureSchema PrepareUpdate<TContract, TImpl>(object id, Action<TImpl> modifier) where TContract : class
            where TImpl : class
        {
            Ensure.That(id, "id").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            var structureSchema = UpsertStructureSchema<TContract>();
            return structureSchema;
        }

        private IStructureSchema PrepareUpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();
            Ensure.That(modifier, "modifier").IsNotNull();

            var structureSchema = UpsertStructureSchema<T>();
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);
            return structureSchema;
        }

        private IStructure BuildStructure<TImpl>(IStructureSchema structureSchema, TImpl item) where TImpl : class
        {
            var structureBuilder = Db.StructureBuilders.ResolveBuilderForUpdate(structureSchema);
            var updatedStructure = structureBuilder.CreateStructure(item, structureSchema);
            return updatedStructure;
        }

        private void PrepareCacheForUpdate(IStructureSchema structureSchema, IStructureId structureId)
        {
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);
            Db.CacheProvider.Remove(structureSchema, structureId);
        }

        private IDbQuery BuildUpdateManySql<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
            var query = queryBuilder.Where(predicate).Build();
            var sqlQuery = QueryGenerator.GenerateQuery(query);
            return sqlQuery;
        }

        private void EnsureConcurrencyTokenIsValid(IStructureSchema structureSchema, IStructureId structureId, object newItem, Type typeForDeserialization)
        {
            string existingJson = DbClient.GetJsonById(structureId, structureSchema);

            EnsureConcurrencyTokenIsValid(existingJson, structureSchema, structureId, newItem, typeForDeserialization);
        }

        private async Task EnsureConcurrencyTokenIsValidAsync(IStructureSchema structureSchema, IStructureId structureId, object newItem, Type typeForDeserialization)
        {
            string existingJson = await DbClient.GetJsonByIdAsync(structureId, structureSchema);

            EnsureConcurrencyTokenIsValid(existingJson, structureSchema, structureId, newItem, typeForDeserialization);
        }

        private void EnsureConcurrencyTokenIsValid(string existingJson, IStructureSchema structureSchema, IStructureId structureId,
                                                             object newItem, Type typeForDeserialization)
        {
            if (string.IsNullOrWhiteSpace(existingJson))
                throw new SisoDbException(ExceptionMessages.WriteSession_NoItemExistsForUpdate.Inject(structureSchema.Name, structureId.Value));

            var existingItem = Db.Serializer.Deserialize(existingJson, typeForDeserialization);
            var existingToken = structureSchema.ConcurrencyTokenAccessor.GetValue(existingItem);
            var updatingToken = structureSchema.ConcurrencyTokenAccessor.GetValue(newItem);

            if (!Equals(updatingToken, existingToken))
                throw new SisoDbConcurrencyException(structureId.Value, structureSchema.Name, ExceptionMessages.ConcurrencyException);

            if (existingToken is Guid)
            {
                structureSchema.ConcurrencyTokenAccessor.SetValue(newItem, Guid.NewGuid());
                return;
            }

            if (existingToken is int)
            {
                var existingNumericToken = (int)existingToken;
                structureSchema.ConcurrencyTokenAccessor.SetValue(newItem, existingNumericToken + 1);
                return;
            }

            if (existingToken is long)
            {
                var existingNumericToken = (long)existingToken;
                structureSchema.ConcurrencyTokenAccessor.SetValue(newItem, existingNumericToken + 1);
                return;
            }

            throw new SisoDbException(ExceptionMessages.ConcurrencyTokenIsOfWrongType);
        }

    }
}