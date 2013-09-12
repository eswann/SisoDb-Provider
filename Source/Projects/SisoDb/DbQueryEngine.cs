using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Querying;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public class DbQueryEngine : IQueryEngine
    {
        protected readonly ISessionExecutionContext ExecutionContext;
        protected ISisoDatabase Db { get { return ExecutionContext.Session.Db; } }
        protected IDbClient DbClient { get { return ExecutionContext.Session.DbClient; } }
        protected readonly IDbQueryGenerator QueryGenerator;

        public DbQueryEngine(ISessionExecutionContext executionContext, IDbQueryGenerator queryGenerator)
        {
            Ensure.That(queryGenerator, "queryGenerator").IsNotNull();
            Ensure.That(executionContext, "executionContext").IsNotNull();

            ExecutionContext = executionContext;
            QueryGenerator = queryGenerator;
        }

        protected virtual T Try<T>(Func<T> action)
        {
            return ExecutionContext.Try(action);
        }

        protected virtual async Task<T> TryAsync<T>(Func<Task<T>> function)
        {
            return await ExecutionContext.TryAsync(function);
        }

        protected virtual IStructureSchema OnUpsertStructureSchema<T>() where T : class
        {
            return OnUpsertStructureSchema(typeof(T));
        }

        protected virtual IStructureSchema OnUpsertStructureSchema(Type structuretype)
        {
            var structureSchema = Db.StructureSchemas.GetSchema(structuretype);
            Db.DbSchemas.Upsert(structureSchema, DbClient);

            return structureSchema;
        }

        public virtual bool Any<T>() where T : class
        {
            return Try(() => OnAny(typeof(T)));
        }

        public virtual async Task<bool> AnyAsync<T>() where T : class
        {
            return await TryAsync(async () => await OnAnyAsync(typeof(T)));
        }

        public virtual bool Any(Type structureType)
        {
            return Try(() => OnAny(structureType));
        }

        public virtual async Task<bool> AnyAsync(Type structureType)
        {
            return await TryAsync(async () => await OnAnyAsync(structureType));
        }

        protected virtual bool OnAny(Type structureType)
        {
            var structureSchema = PrepareOnAny(structureType);

            return DbClient.Any(structureSchema);
        }

        protected virtual async Task<bool> OnAnyAsync(Type structureType)
        {
            var structureSchema = PrepareOnAny(structureType);

            return await DbClient.AnyAsync(structureSchema);
        }

        private IStructureSchema PrepareOnAny(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            return structureSchema;
        }

        public virtual bool Any<T>(IQuery query) where T : class
        {
            return Try(() => OnAny(typeof(T), query));
        }

        public virtual async Task<bool> AnyAsync<T>(IQuery query) where T : class
        {
            return await TryAsync(async () => await OnAnyAsync(typeof(T), query));
        }

        public virtual bool Any(Type structureType, IQuery query)
        {
            return Try(() => OnAny(structureType, query));
        }

        public virtual async Task<bool> AnyAsync(Type structureType, IQuery query)
        {
            return await TryAsync(async () => await OnAnyAsync(structureType, query));
        }

        protected virtual bool OnAny(Type structureType, IQuery query)
        {
            var structureSchema = PrepareOnAny(structureType, query);

            if (!query.HasWhere)
            {
                return Db.CacheProvider.Any(structureSchema, s => DbClient.Any(s));
            }

            var whereSql = QueryGenerator.GenerateQueryReturningCountOfStrutureIds(query);
            return DbClient.Any(structureSchema, whereSql);
        }

        protected virtual async Task<bool> OnAnyAsync(Type structureType, IQuery query)
        {
            var structureSchema = PrepareOnAny(structureType, query);

            if (!query.HasWhere)
            {
                return await Db.CacheProvider.AnyAsync(
                    structureSchema,
                    async s => await DbClient.AnyAsync(s));
            }

            var whereSql = QueryGenerator.GenerateQueryReturningCountOfStrutureIds(query);
            return await DbClient.AnyAsync(structureSchema, whereSql);
        }

        private IStructureSchema PrepareOnAny(Type structureType, IQuery query)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            return structureSchema;
        }

        public virtual int Count<T>() where T : class
        {
            return Try(() => OnCount(typeof(T)));
        }

        public virtual async Task<int> CountAsync<T>() where T : class
        {
            return await TryAsync(async () => await OnCountAsync(typeof(T)));
        }

        public virtual int Count(Type structureType)
        {
            return Try(() => OnCount(structureType));
        }

        public virtual async Task<int> CountAsync(Type structureType)
        {
            return await TryAsync(async () => await OnCountAsync(structureType));
        }

        protected virtual int OnCount(Type structureType)
        {
            var structureSchema = PrepareOnCount(structureType);

            return DbClient.RowCount(structureSchema);
        }

        protected async virtual Task<int> OnCountAsync(Type structureType)
        {
            var structureSchema = PrepareOnCount(structureType);

            return await DbClient.RowCountAsync(structureSchema);
        }

        private IStructureSchema PrepareOnCount(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            return structureSchema;
        }

        public virtual int Count<T>(IQuery query) where T : class
        {
            return Try(() => OnCount(typeof(T), query));
        }

        public virtual async Task<int> CountAsync<T>(IQuery query) where T : class
        {
            return await TryAsync(async () => await OnCountAsync(typeof(T), query));
        }

        public virtual int Count(Type structureType, IQuery query)
        {
            return Try(() => OnCount(structureType, query));
        }

        public virtual async Task<int> CountAsync(Type structureType, IQuery query)
        {
            return await TryAsync(async () => await OnCountAsync(structureType, query));
        }

        protected virtual int OnCount(Type structureType, IQuery query)
        {
            var structureSchema = PrepareOnCount(structureType, query);

            if (!query.HasWhere)
                return DbClient.RowCount(structureSchema);

            var whereSql = QueryGenerator.GenerateQueryReturningCountOfStrutureIds(query);
            return DbClient.RowCountByQuery(structureSchema, whereSql);
        }

        protected virtual async Task<int> OnCountAsync(Type structureType, IQuery query)
        {
            var structureSchema = PrepareOnCount(structureType, query);

            if (!query.HasWhere)
                return await DbClient.RowCountAsync(structureSchema);

            var whereSql = QueryGenerator.GenerateQueryReturningCountOfStrutureIds(query);
            return await DbClient.RowCountByQueryAsync(structureSchema, whereSql);
        }

        private IStructureSchema PrepareOnCount(Type structureType, IQuery query)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            return structureSchema;
        }

        public virtual bool Exists<T>(object id) where T : class
        {
            return Try(() => OnExists(typeof(T), id));
        }

        public virtual async Task<bool> ExistsAsync<T>(object id) where T : class
        {
            return await TryAsync(async () => await OnExistsAsync(typeof(T), id));
        }

        public virtual bool Exists(Type structureType, object id)
        {
            return Try(() => OnExists(structureType, id));
        }

        public virtual async Task<bool> ExistsAsync(Type structureType, object id)
        {
            return await TryAsync( async () => await OnExistsAsync(structureType, id));
        }

        protected virtual bool OnExists(Type structureType, object id)
        {
            return Try(() =>
            {
                var structureSchema = PrepareOnExists(structureType, id);
                var structureId = StructureId.ConvertFrom(id);

                return Db.CacheProvider.Exists(structureSchema, structureId, sid => DbClient.Exists(structureSchema, sid));
            });
        }

        protected virtual async Task<bool> OnExistsAsync(Type structureType, object id)
        {
            return await TryAsync( async () =>
            {
                var structureSchema = PrepareOnExists(structureType, id);
                var structureId = StructureId.ConvertFrom(id);

                return await Db.CacheProvider.ExistsAsync(structureSchema, structureId, async sid => await DbClient.ExistsAsync(structureSchema, sid));
            });
        }

        private IStructureSchema PrepareOnExists(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            return structureSchema;
        }

        public virtual IEnumerable<T> Query<T>(IQuery query) where T : class
        {
            return Try(() => OnQueryAs<T, T>(query));
        }

        public virtual async Task<IEnumerable<T>> QueryAsync<T>(IQuery query) where T : class
        {
            return await TryAsync(async () => await OnQueryAsAsync<T, T>(query));
        }

        public virtual IEnumerable<object> Query(IQuery query, Type structureType)
        {
            return Try(() => OnQuery(query, structureType));
        }

        public virtual async Task<IEnumerable<object>> QueryAsync(IQuery query, Type structureType)
        {
            return await TryAsync(async () => await OnQueryAsync(query, structureType));
        }

        protected virtual IEnumerable<object> OnQuery(IQuery query, Type structureType)
        {
            var structureSchema = PrepareOnQuery(query, structureType);

            if (query.IsEmpty)
                return Db.Serializer.DeserializeMany(DbClient.GetJsonOrderedByStructureId(structureSchema), structureType);

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return Db.CacheProvider.Consume(
                structureSchema, 
                sqlQuery, 
                q =>
                {
                    var sourceData = DbClient.ReadJson(structureSchema, sqlQuery.Sql, sqlQuery.Parameters);
                    return Db.Serializer.DeserializeMany(sourceData, structureType);
                }, 
                ExecutionContext.Session.CacheConsumeMode);
        }

        protected virtual async Task<IEnumerable<object>> OnQueryAsync(IQuery query, Type structureType)
        {
            var structureSchema = PrepareOnQuery(query, structureType);

            if (query.IsEmpty)
                return Db.Serializer.DeserializeMany(DbClient.GetJsonOrderedByStructureId(structureSchema), structureType);

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                sqlQuery,
                async q =>
                {
                    var sourceData = await DbClient.ReadJsonAsync(structureSchema, sqlQuery.Sql, sqlQuery.Parameters);
                    return Db.Serializer.DeserializeMany(sourceData, structureType);
                },
                ExecutionContext.Session.CacheConsumeMode);
        }

        private IStructureSchema PrepareOnQuery(IQuery query, Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            return structureSchema;
        }

        public virtual IEnumerable<TResult> QueryAs<T, TResult>(IQuery query)
            where T : class
            where TResult : class
        {
            return Try(() => OnQueryAs<T, TResult>(query));
        }

        public virtual async Task<IEnumerable<TResult>> QueryAsAsync<T, TResult>(IQuery query)
            where T : class
            where TResult : class
        {
            return await TryAsync(async () => await OnQueryAsAsync<T, TResult>(query));
        }

        public virtual IEnumerable<object> QueryAs(IQuery query, Type structureType, Type resultType)
        {
            return Try(() => OnQueryAs(query, structureType, resultType));
        }

        public virtual async Task<IEnumerable<object>> QueryAsAsync(IQuery query, Type structureType, Type resultType)
        {
            return await TryAsync(async () => await OnQueryAsAsync(query, structureType, resultType));
        }

        protected virtual IEnumerable<object> OnQueryAs(IQuery query, Type structureType, Type resultType)
        {
            var structureSchema = PrepareOnQueryAs(query, structureType, resultType);

            if (query.IsEmpty)
                return Db.Serializer.DeserializeMany(DbClient.GetJsonOrderedByStructureId(structureSchema), resultType);

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return Db.CacheProvider.Consume(
                structureSchema,
                sqlQuery,
                q =>
                {
                    var sourceData = DbClient.ReadJson(structureSchema, sqlQuery.Sql, sqlQuery.Parameters);
                    return Db.Serializer.DeserializeMany(sourceData, resultType);
                },
                ExecutionContext.Session.CacheConsumeMode);
        }

        protected virtual async Task<IEnumerable<object>> OnQueryAsAsync(IQuery query, Type structureType, Type resultType)
        {
            var structureSchema = PrepareOnQueryAs(query, structureType, resultType);

            if (query.IsEmpty)
                return Db.Serializer.DeserializeMany(DbClient.GetJsonOrderedByStructureId(structureSchema), resultType);

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                sqlQuery,
                async q =>
                {
                    var sourceData = await DbClient.ReadJsonAsync(structureSchema, sqlQuery.Sql, sqlQuery.Parameters);
                    return Db.Serializer.DeserializeMany(sourceData, resultType);
                },
                ExecutionContext.Session.CacheConsumeMode);
        }

        private IStructureSchema PrepareOnQueryAs(IQuery query, Type structureType, Type resultType)
        {
            Ensure.That(query, "query").IsNotNull();
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(resultType, "resultType").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structureType);
            return structureSchema;
        }

        protected virtual IEnumerable<TResult> OnQueryAs<T, TResult>(IQuery query)
            where T : class
            where TResult : class
        {
            var structureSchema = PrepareOnQueryAs<T>(query);

            if (query.IsEmpty)
                return Db.Serializer.DeserializeMany<TResult>(DbClient.GetJsonOrderedByStructureId(structureSchema));

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return Db.CacheProvider.Consume(
                structureSchema,
                sqlQuery,
                q =>
                {
                    var sourceData = DbClient.ReadJson(structureSchema, sqlQuery.Sql, sqlQuery.Parameters);
                    return Db.Serializer.DeserializeMany<TResult>(sourceData);
                },
                ExecutionContext.Session.CacheConsumeMode);
        }

        protected virtual async Task<IEnumerable<TResult>> OnQueryAsAsync<T, TResult>(IQuery query)
            where T : class
            where TResult : class
        {
            var structureSchema = PrepareOnQueryAs<T>(query);

            if (query.IsEmpty)
                return Db.Serializer.DeserializeMany<TResult>(await DbClient.GetJsonOrderedByStructureIdAsync(structureSchema));

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                sqlQuery,
                async q =>
                {
                    var sourceData = await DbClient.ReadJsonAsync(structureSchema, sqlQuery.Sql, sqlQuery.Parameters);
                    return Db.Serializer.DeserializeMany<TResult>(sourceData);
                },
                ExecutionContext.Session.CacheConsumeMode);
        }

        private IStructureSchema PrepareOnQueryAs<T>(IQuery query) where T : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<T>();
            return structureSchema;
        }

        public virtual IEnumerable<string> QueryAsJson<T>(IQuery query) where T : class
        {
            return Try(() => OnQueryAsJson(typeof(T), query));
        }

        public virtual async Task<IEnumerable<string>> QueryAsJsonAsync<T>(IQuery query) where T : class
        {
            return await TryAsync(async () => await OnQueryAsJsonAsync(typeof(T), query));
        }

        public virtual IEnumerable<string> QueryAsJson(IQuery query, Type structuretype)
        {
            return Try(() => OnQueryAsJson(structuretype, query));
        }

        public virtual async Task<IEnumerable<string>> QueryAsJsonAsync(IQuery query, Type structuretype)
        {
            return await TryAsync(async () => await OnQueryAsJsonAsync(structuretype, query));
        }

        protected virtual IEnumerable<string> OnQueryAsJson(Type structuretype, IQuery query)
        {
            var structureSchema = PrepareOnQueryAsJson(structuretype, query);

            if (query.IsEmpty)
                return DbClient.GetJsonOrderedByStructureId(structureSchema);

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return DbClient.ReadJson(structureSchema, sqlQuery.Sql, sqlQuery.Parameters);
        }

        protected virtual async Task<IEnumerable<string>> OnQueryAsJsonAsync(Type structuretype, IQuery query)
        {
            var structureSchema = PrepareOnQueryAsJson(structuretype, query);

            if (query.IsEmpty)
                return await DbClient.GetJsonOrderedByStructureIdAsync(structureSchema);

            var sqlQuery = QueryGenerator.GenerateQuery(query);

            return await DbClient.ReadJsonAsync(structureSchema, sqlQuery.Sql, sqlQuery.Parameters);
        }

        private IStructureSchema PrepareOnQueryAsJson(Type structuretype, IQuery query)
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema(structuretype);
            return structureSchema;
        }
    }
}