using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.EnsureThat;
using SisoDb.NCore.Collections;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal class InsertHelper : DbSessionHelper
    {
        public InsertHelper(DbSession session)
            : base(session)
        {
        }

        public virtual void Insert(Type structureType, object item)
        {
            var structureSchema = PrepareInsert(structureType, item);

            var structure = BuildStructure(item, structureSchema);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            structureInserter.Insert(structureSchema, new[] { structure });
            InternalEvents.NotifyInserted(Session, structureSchema, structure, item);
        }

        public async Task InsertAsync(Type structureType, object item)
        {
            var structureSchema = PrepareInsert(structureType, item);

            var structure = BuildStructure(item, structureSchema);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            await structureInserter.InsertAsync(structureSchema, new[] { structure });

            InternalEvents.NotifyInserted(Session, structureSchema, structure, item);
        }

        public void InsertAs(Type structureType, object item)
        {
            var structureSchema = PrepareInsert(structureType, item);

            var structure = BuildStructure(item, structureSchema, structureType);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            structureInserter.Insert(structureSchema, new[] { structure });
            InternalEvents.NotifyInserted(Session, structureSchema, structure, item);
        }

        public async Task InsertAsAsync(Type structureType, object item)
        {
            var structureSchema = PrepareInsert(structureType, item);

            var structure = BuildStructure(item, structureSchema, structureType);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            await structureInserter.InsertAsync(structureSchema, new[] { structure });

            InternalEvents.NotifyInserted(Session, structureSchema, structure, item);
        }

        public string InsertJson(Type structureType, string json)
        {
            var structureSchema = PrepareInsertJson(structureType, json);
            
            var item = Db.Serializer.Deserialize(json, structureType);
            var structure = BuildStructure(item, structureSchema);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            structureInserter.Insert(structureSchema, new[] { structure });
            InternalEvents.NotifyInserted(Session, structureSchema, structure, item);

            return structure.Data;
        }

        public async Task<string> InsertJsonAsync(Type structureType, string json)
        {
            var structureSchema = PrepareInsertJson(structureType, json);

            var item = Db.Serializer.Deserialize(json, structureType);
            var structure = BuildStructure(item, structureSchema);

            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);
            await structureInserter.InsertAsync(structureSchema, new[] { structure });
            
            InternalEvents.NotifyInserted(Session, structureSchema, structure, item);

            return structure.Data;
        }

        public void InsertMany(Type structureType, IEnumerable<object> items)
        {
            var structureSchema = PrepareInsert(structureType, items);

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

            foreach (var itemsBatch in items.Batch(Db.Settings.MaxInsertManyBatchSize))
            {
                var structures = structureBuilder.CreateStructures(itemsBatch, structureSchema);
                structureInserter.Insert(structureSchema, structures);
                InternalEvents.NotifyInserted(Session, structureSchema, structures, itemsBatch);
            }
        }

        public async Task InsertManyAsync(Type structureType, IEnumerable<object> items)
        {
            var structureSchema = PrepareInsert(structureType, items);

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

            foreach (var itemsBatch in items.Batch(Db.Settings.MaxInsertManyBatchSize))
            {
                var structures = structureBuilder.CreateStructures(itemsBatch, structureSchema);
                await structureInserter.InsertAsync(structureSchema, structures);
                InternalEvents.NotifyInserted(Session, structureSchema, structures, itemsBatch);
            }
        }

        public void InsertManyJson(Type structureType, IEnumerable<string> json)
        {
            var structureSchema = PrepareInsert(structureType, json, "json");

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

            foreach (var itemsBatch in Db.Serializer.DeserializeMany(json, structureSchema.Type.Type).Batch(Db.Settings.MaxInsertManyBatchSize))
            {
                var structures = structureBuilder.CreateStructures(itemsBatch, structureSchema);
                structureInserter.Insert(structureSchema, structures);
                InternalEvents.NotifyInserted(Session, structureSchema, structures, itemsBatch);
            }
        }

        public async Task InsertManyJsonAsync(Type structureType, IEnumerable<string> json)
        {
            var structureSchema = PrepareInsert(structureType, json, "json");

            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structureInserter = Db.ProviderFactory.GetStructureInserter(DbClient);

            foreach (var itemsBatch in Db.Serializer.DeserializeMany(json, structureSchema.Type.Type).Batch(Db.Settings.MaxInsertManyBatchSize))
            {
                var structures = structureBuilder.CreateStructures(itemsBatch, structureSchema);
                await structureInserter.InsertAsync(structureSchema, structures);
                InternalEvents.NotifyInserted(Session, structureSchema, structures, itemsBatch);
            }
        }

        private IStructureSchema PrepareInsert(Type structureType, object item, string itemName = "item")
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(item, itemName).IsNotNull();

            var structureSchema = UpsertStructureSchema(structureType);

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);

            return structureSchema;
        }

        private IStructureSchema PrepareInsertJson(Type structureType, string json)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            var structureSchema = UpsertStructureSchema(structureType);

            CacheConsumeMode = CacheConsumeModes.DoNotUpdateCacheWithDbResult;
            Db.CacheProvider.CleanQueriesFor(structureSchema);

            return structureSchema;
        }

        private IStructure BuildStructure(object item, IStructureSchema structureSchema, Type structureType)
        {
            var json = Db.Serializer.Serialize(item);
            var realItem = Db.Serializer.Deserialize(json, structureType);
            var structure = BuildStructure(realItem, structureSchema);

            return structure;
        }

        private IStructure BuildStructure(object item, IStructureSchema structureSchema)
        {
            var structureBuilder = Db.StructureBuilders.ResolveBuilderForInsert(structureSchema, DbClient);
            var structure = structureBuilder.CreateStructure(item, structureSchema);
            return structure;
        }
    }
}