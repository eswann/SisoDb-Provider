using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.NCore.Collections;
using SisoDb.Querying;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal class DeleteHelper : ModifyHelper
    {
        public DeleteHelper(ISisoDatabase db, IDbClient dbClient, IDbQueryGenerator queryGenerator, ISession session, SessionEvents internalEvents) 
            : base(db, dbClient, queryGenerator, session, internalEvents)
        {
        }

        public void Clear(Type structureType)
        {
            var structureSchema = PrepareClear(structureType);

            DbClient.DeleteAll(structureSchema);
        }

        public async Task ClearAsync(Type structureType)
        {
            var structureSchema = PrepareClear(structureType);

            await DbClient.DeleteAllAsync(structureSchema);
        }

        public void DeleteAllExceptIds(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();
            var structureSchema = PrepareClear(structureType);

            var structureIds = MapStructureIds(ids);

            DbClient.DeleteAllExceptIds(structureIds, structureSchema);
        }

        public async Task DeleteAllExceptIdsAsync(Type structureType, params object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();
            var structureSchema = PrepareClear(structureType);

            var structureIds = MapStructureIds(ids);

            await DbClient.DeleteAllExceptIdsAsync(structureIds, structureSchema);
        }

        public void DeleteById(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = PrepareDeleteById(structureType, structureId);

            DbClient.DeleteById(structureId, structureSchema);
            InternalEvents.NotifyDeleted(Session, structureSchema, structureId);
        }

        public async Task DeleteByIdAsync(Type structureType, object id)
        {
            Ensure.That(id, "id").IsNotNull();

            var structureId = StructureId.ConvertFrom(id);
            var structureSchema = PrepareDeleteById(structureType, structureId);

            await DbClient.DeleteByIdAsync(structureId, structureSchema);
            InternalEvents.NotifyDeleted(Session, structureSchema, structureId);
        }

        public void DeleteByIds(Type structureType, params object[] ids)
        {
            var structureSchema = PrepareDeleteByIds(structureType, ids);

            var structureIds = MapStructureIds(ids);
            Db.CacheProvider.Remove(structureSchema, new HashSet<IStructureId>(structureIds));

            DbClient.DeleteByIds(structureIds, structureSchema);
            InternalEvents.NotifyDeleted(Session, structureSchema, structureIds);
        }

        public async Task DeleteByIdsAsync(Type structureType, params object[] ids)
        {
            var structureSchema = PrepareDeleteByIds(structureType, ids);

            var structureIds = MapStructureIds(ids);
            Db.CacheProvider.Remove(structureSchema, new HashSet<IStructureId>(structureIds));

            await DbClient.DeleteByIdsAsync(structureIds, structureSchema);
            InternalEvents.NotifyDeleted(Session, structureSchema, structureIds);
        }

        public void DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var structureSchema = PrepareDeleteByQuery(predicate);

            var query = BuildDeleteByQuery(predicate);
            var sql = QueryGenerator.GenerateQueryReturningStructureIds(query);

            DbClient.DeleteByQuery(sql, structureSchema);
            InternalEvents.NotifyDeleted(Session, structureSchema, query);
        }

        public async Task DeleteByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var structureSchema = PrepareDeleteByQuery(predicate);

            var query = BuildDeleteByQuery(predicate);
            var sql = QueryGenerator.GenerateQueryReturningStructureIds(query);

            await DbClient.DeleteByQueryAsync(sql, structureSchema);
            InternalEvents.NotifyDeleted(Session, structureSchema, query);
        }

        private IQuery BuildDeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
            queryBuilder.Where(predicate);
            var query = queryBuilder.Build();
            return query;
        }

        private IStructureSchema PrepareDeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            var structureSchema = UpsertStructureSchema<T>();
            Db.CacheProvider.ClearByType(structureSchema);
            return structureSchema;
        }

        private IStructureSchema PrepareClear(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.ClearByType(structureType);

            var structureSchema = UpsertStructureSchema(structureType);
            return structureSchema;
        }

        private IStructureSchema PrepareDeleteByIds(Type structureType, object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureSchema = UpsertStructureSchema(structureType);
            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            return structureSchema;
        }

        private IStructureSchema PrepareDeleteById(Type structureType, IStructureId structureId)
        {
            var structureSchema = UpsertStructureSchema(structureType);

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.Remove(structureSchema, structureId);
            return structureSchema;
        }

        private static IStructureId[] MapStructureIds(IEnumerable<object> ids)
        {
            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            return structureIds;
        }
    }
}