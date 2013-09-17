using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.EnsureThat;
using SisoDb.Querying.Sql;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal class GetIdsHelper : DbSessionHelper
    {
        public GetIdsHelper(DbSession session)
            : base(session)
        {
        }

        public IEnumerable<TId> GetIds<T, TId>(Expression<Func<T, bool>> predicate) where T : class
        {
            var structureSchema = PrepareGetByIds(predicate);
            var sql = GenerateSql(predicate);

            return DbClient.GetStructureIds<TId>(structureSchema, sql);
        }

        public async Task<IEnumerable<TId>> GetIdsAsync<T, TId>(Expression<Func<T, bool>> predicate) where T : class
        {
            var structureSchema = PrepareGetByIds(predicate);
            var sql = GenerateSql(predicate);

            return await DbClient.GetStructureIdsAsync<TId>(structureSchema, sql);
        }

        private IDbQuery GenerateSql<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var queryBuilder = Db.ProviderFactory.GetQueryBuilder<T>(Db.StructureSchemas);
            queryBuilder.Where(predicate);

            var query = queryBuilder.Build();
            var sql = QueryGenerator.GenerateQueryReturningStructureIds(query);
            return sql;
        }

        private IStructureSchema PrepareGetByIds<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            Ensure.That(predicate, "predicate").IsNotNull();
            var structureSchema = UpsertStructureSchema<T>();
            return structureSchema;
        }
    }
}