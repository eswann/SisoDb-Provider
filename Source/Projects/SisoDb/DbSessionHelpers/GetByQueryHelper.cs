using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal class GetByQueryHelper : ReadHelper
    {
        public GetByQueryHelper(ISisoDatabase db, IDbClient dbClient, IDbQueryGenerator queryGenerator) : base(db, dbClient, queryGenerator)
        {
        }

        public TOut GetByQueryAs<TOut>(Type structureType, Expression<Func<TOut, bool>> predicate) where TOut : class
        {
            var structureSchema = PrepareGetByQuery(structureType, predicate);

            return Db.CacheProvider.Consume(
                structureSchema,
                predicate,
                e =>
                {
                    var sqlQuery = CreateOnGetByQuery(predicate);
                    var sourceData = DbClient.ReadJson(structureSchema, sqlQuery.Sql, sqlQuery.Parameters).SingleOrDefault();

                    return Db.Serializer.Deserialize<TOut>(sourceData);
                },
                CacheConsumeMode);
        }

        public async Task<TOut> GetByQueryAsAsync<TOut>(Type structureType, Expression<Func<TOut, bool>> predicate)
            where TOut : class
        {
            var structureSchema = PrepareGetByQuery(structureType, predicate);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                predicate,
                async e =>
                {
                    var sqlQuery = CreateOnGetByQuery(predicate);
                    var sourceData = (await DbClient.ReadJsonAsync(structureSchema, sqlQuery.Sql, sqlQuery.Parameters)).SingleOrDefault();

                    return Db.Serializer.Deserialize<TOut>(sourceData);
                },
                CacheConsumeMode);
        }

        private IStructureSchema PrepareGetByQuery<TOut>(Type structureType, Expression<Func<TOut, bool>> predicate) where TOut : class
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(predicate, "predicate").IsNotNull();

            return UpsertStructureSchema(structureType);
        }

        private IDbQuery CreateOnGetByQuery<TOut>(Expression<Func<TOut, bool>> predicate) where TOut : class
        {
            var queryBuilder = Db.ProviderFactory.GetQueryBuilder<TOut>(Db.StructureSchemas);
            queryBuilder.Where(predicate);
            var query = queryBuilder.Build();

            var sqlQuery = QueryGenerator.GenerateQuery(query);
            return sqlQuery;
        }

    }
}