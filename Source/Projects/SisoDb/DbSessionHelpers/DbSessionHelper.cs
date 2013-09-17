using System;
using SisoDb.Caching;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal abstract class DbSessionHelper
    {
        private readonly DbSession _session;

        protected DbSessionHelper(DbSession session)
        {
            _session = session;
        }

        public ISisoDatabase Db { get { return _session.Db; } }

        public IDbClient DbClient { get { return _session.DbClient; } }

        public ISession Session { get { return _session; } }

        public SessionEvents InternalEvents { get { return _session.InternalEvents; } } 

        public CacheConsumeModes CacheConsumeMode
        {
            get { return _session.CacheConsumeMode; }
            protected set { _session.CacheConsumeMode = value; }
        }

        public IDbQueryGenerator QueryGenerator { get { return _session.QueryGenerator; } }

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