using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public class DbSessionAdvanced: IAdvanced
    {
        protected readonly ISessionExecutionContext ExecutionContext;
        protected ISisoDatabase Db { get { return ExecutionContext.Session.Db; } }
        protected IDbClient DbClient { get { return ExecutionContext.Session.DbClient; } }
        protected readonly IDbQueryGenerator QueryGenerator;
        protected readonly ISqlExpressionBuilder SqlExpressionBuilder;
        
        public DbSessionAdvanced(ISessionExecutionContext executionContext, IDbQueryGenerator queryGenerator, ISqlExpressionBuilder sqlExpressionBuilder)
        {
            Ensure.That(queryGenerator, "queryGenerator").IsNotNull();
            Ensure.That(sqlExpressionBuilder, "sqlExpressionBuilder").IsNotNull();
            Ensure.That(executionContext, "executionContext").IsNotNull();

            ExecutionContext = executionContext;
            QueryGenerator = queryGenerator;
            SqlExpressionBuilder = sqlExpressionBuilder;
        }

        protected virtual void Try(Action action)
        {
            ExecutionContext.Try(action);
        }

        protected virtual T Try<T>(Func<T> function)
        {
            return ExecutionContext.Try(function);
        }

        protected virtual async Task TryAsync(Func<Task> action)
        {
            await ExecutionContext.TryAsync(action);
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

        public virtual void NonQuery(string sql, params IDacParameter[] parameters)
        {
            Try(() =>
            {
                Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
                DbClient.ExecuteNonQuery(sql, parameters);
            });
        }

        public virtual async Task NonQueryAsync(string sql, params IDacParameter[] parameters)
        {
            await TryAsync(async () =>
            {
                Ensure.That(sql, "sql").IsNotNullOrWhiteSpace();
                await DbClient.ExecuteNonQueryAsync(sql, parameters);
            });
        }

        public virtual void UpsertNamedQuery<T>(string name, Action<IQueryBuilder<T>> spec) where T : class
        {
            Try(() =>
            {
                Ensure.That(name, "name").IsNotNullOrWhiteSpace();
                Ensure.That(spec, "spec").IsNotNull();

                var generator = Db.ProviderFactory.GetNamedQueryGenerator<T>(Db.StructureSchemas);
                DbClient.UpsertSp(name, generator.Generate(name, spec));
            });
        }


        public virtual IEnumerable<T> NamedQuery<T>(INamedQuery query) where T : class
        {
            return Try(() => OnNamedQueryAs<T, T>(query));
        }

        public virtual async Task<IEnumerable<T>> NamedQueryAsync<T>(INamedQuery query) where T : class
        {
            return await TryAsync(async () => await OnNamedQueryAsAsync<T, T>(query));
        }

        public virtual IEnumerable<T> NamedQuery<T>(string name, Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return OnNamedQueryAs<T, T>(namedQuery);
            });
        }

        public virtual async Task<IEnumerable<T>> NamedQueryAsync<T>(string name, Expression<Func<T, bool>> predicate) where T : class
        {
            return await TryAsync(async () =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return await OnNamedQueryAsAsync<T, T>(namedQuery);
            });
        }

        public virtual IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query) where TContract : class where TOut : class
        {
            return Try(() => OnNamedQueryAs<TContract, TOut>(query));
        }

        public virtual async Task<IEnumerable<TOut>> NamedQueryAsAsync<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class
        {
            return await TryAsync(async () => await OnNamedQueryAsAsync<TContract, TOut>(query));
        }

        public virtual IEnumerable<TOut> NamedQueryAs<TContract, TOut>(string name, Expression<Func<TContract, bool>> predicate) where TContract : class where TOut : class
        {
            return Try(() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<TContract>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return OnNamedQueryAs<TContract, TOut>(namedQuery);
            });
        }

        public virtual async Task<IEnumerable<TOut>> NamedQueryAsAsync<TContract, TOut>(string name, Expression<Func<TContract, bool>> predicate)
            where TContract : class
            where TOut : class
        {
            return await TryAsync(async() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<TContract>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return await OnNamedQueryAsAsync<TContract, TOut>(namedQuery);
            });
        }

        protected virtual IEnumerable<TOut> OnNamedQueryAs<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<TContract>();

            return Db.Serializer.DeserializeMany<TOut>(DbClient.ReadJsonBySp(structureSchema, query.Name, query.Parameters));
        }

        protected virtual async Task<IEnumerable<TOut>> OnNamedQueryAsAsync<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<TContract>();

            return Db.Serializer.DeserializeMany<TOut>(await DbClient.ReadJsonBySpAsync(structureSchema, query.Name, query.Parameters));
        }

        public virtual IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query) where T : class
        {
            return Try(() => OnNamedQueryAsJson<T>(query));
        }

        public virtual async Task<IEnumerable<string>> NamedQueryAsJsonAsync<T>(INamedQuery query) where T : class
        {
            return await TryAsync(async() => await OnNamedQueryAsJsonAsync<T>(query));
        }

        public virtual IEnumerable<string> NamedQueryAsJson<T>(string name, Expression<Func<T, bool>> predicate) where T : class
        {
            return Try(() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return OnNamedQueryAsJson<T>(namedQuery);
            });
        }

        public virtual async Task<IEnumerable<string>> NamedQueryAsJsonAsync<T>(string name, Expression<Func<T, bool>> predicate) where T : class
        {
            return await TryAsync(async() =>
            {
                var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
                queryBuilder.Where(predicate);
                var query = queryBuilder.Build();
                var sqlExpression = SqlExpressionBuilder.Process(query);

                var namedQuery = new NamedQuery(name);
                namedQuery.Add(sqlExpression.WhereCriteria.Parameters);

                return await OnNamedQueryAsJsonAsync<T>(namedQuery);
            });
        }

        protected virtual IEnumerable<string> OnNamedQueryAsJson<T>(INamedQuery query) where T : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<T>();

            return DbClient.ReadJsonBySp(structureSchema, query.Name, query.Parameters);
        }

        protected virtual async Task<IEnumerable<string>> OnNamedQueryAsJsonAsync<T>(INamedQuery query) where T : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<T>();

            return await DbClient.ReadJsonBySpAsync(structureSchema, query.Name, query.Parameters);
        }

        public virtual IEnumerable<T> RawQuery<T>(IRawQuery query) where T : class
        {
            return Try(() => OnRawQueryAs<T, T>(query));
        }

        public virtual async Task<IEnumerable<T>> RawQueryAsync<T>(IRawQuery query) where T : class
        {
            return await TryAsync(async() => await OnRawQueryAsAsync<T, T>(query));
        }

        public virtual IEnumerable<TOut> RawQueryAs<TContract, TOut>(IRawQuery query) where TContract : class where TOut : class
        {
            return Try(() => OnRawQueryAs<TContract, TOut>(query));
        }

        public virtual async Task<IEnumerable<TOut>> RawQueryAsAsync<TContract, TOut>(IRawQuery query)
            where TContract : class
            where TOut : class
        {
            return await TryAsync(async () => await OnRawQueryAsAsync<TContract, TOut>(query));
        }

        protected virtual IEnumerable<TOut> OnRawQueryAs<TContract, TOut>(IRawQuery query)
            where TContract : class
            where TOut : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<TContract>();

            return Db.Serializer.DeserializeMany<TOut>(DbClient.ReadJson(structureSchema, query.QueryString, query.Parameters));
        }

        protected virtual async Task<IEnumerable<TOut>> OnRawQueryAsAsync<TContract, TOut>(IRawQuery query)
            where TContract : class
            where TOut : class
        {
            Ensure.That(query, "query").IsNotNull();

            var structureSchema = OnUpsertStructureSchema<TContract>();

            return Db.Serializer.DeserializeMany<TOut>(await DbClient.ReadJsonAsync(structureSchema, query.QueryString, query.Parameters));
        }

        public virtual IEnumerable<string> RawQueryAsJson<T>(IRawQuery query) where T : class
        {
            return Try(() =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = OnUpsertStructureSchema<T>();

                return DbClient.ReadJson(structureSchema, query.QueryString, query.Parameters);
            });
        }

        public virtual async Task<IEnumerable<string>> RawQueryAsJsonAsync<T>(IRawQuery query) where T : class
        {
            return await TryAsync(async () =>
            {
                Ensure.That(query, "query").IsNotNull();

                var structureSchema = OnUpsertStructureSchema<T>();

                return await DbClient.ReadJsonAsync(structureSchema, query.QueryString, query.Parameters);
            });
        }
    }
}