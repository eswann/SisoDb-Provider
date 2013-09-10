using System;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal abstract class DbSessionHelper
    {
        protected DbSessionHelper(ISisoDatabase db, IDbClient dbClient)
        {
            Db = db;
            DbClient = dbClient;
            CacheConsumeMode = CacheConsumeModes.UpdateCacheWithDbResult;
        }

        public ISisoDatabase Db { get; private set; }

        public IDbClient DbClient { get; private set; }

        public CacheConsumeModes CacheConsumeMode { get; protected set; }

        public virtual IStructureSchema UpsertStructureSchema<T>() where T : class
        {
            return UpsertStructureSchema(typeof(T));
        }

        public virtual IStructureSchema UpsertStructureSchema(Type structuretype)
        {
            var structureSchema = Db.StructureSchemas.GetSchema(structuretype);
            Db.DbSchemas.Upsert(structureSchema, DbClient);

            return structureSchema;
        }

    }
}