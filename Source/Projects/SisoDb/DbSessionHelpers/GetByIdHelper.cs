using System;
using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.EnsureThat;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal class GetByIdHelper : DbSessionHelper
    {
        public GetByIdHelper(DbSession session)
            : base(session)
        {
        }

        public object GetById(Type structureType, object id)
        {
            var structureSchema = PrepareGetById(structureType, id);

            return Db.CacheProvider.Consume(
                structureSchema,
                StructureId.ConvertFrom(id),
                sid => Db.Serializer.Deserialize(DbClient.GetJsonById(sid, structureSchema), structureType),
                CacheConsumeMode);
        }

        public async Task<object> GetByIdAsync(Type structureType, object id)
        {
            var structureSchema = PrepareGetById(structureType, id);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                StructureId.ConvertFrom(id),
                async sid => Db.Serializer.Deserialize(await DbClient.GetJsonByIdAsync(sid, structureSchema), structureType),
                CacheConsumeMode);
        }

        public TOut GetByIdAs<TOut>(Type structureType, object id)
           where TOut : class
        {
            var structureSchema = PrepareGetById(structureType, id);

            return Db.CacheProvider.Consume(
                structureSchema,
                StructureId.ConvertFrom(id),
                sid => Db.Serializer.Deserialize<TOut>(DbClient.GetJsonById(sid, structureSchema)),
                CacheConsumeMode);
        }

        public async Task<TOut> GetByIdAsAsync<TOut>(Type structureType, object id)
            where TOut : class
        {
            var structureSchema = PrepareGetById(structureType, id);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                StructureId.ConvertFrom(id),
                async sid => Db.Serializer.Deserialize<TOut>(await DbClient.GetJsonByIdAsync(sid, structureSchema)),
                CacheConsumeMode);
        }

        public string GetByIdAsJson(Type structureType, object id)
        {
            var structureSchema = PrepareGetById(structureType, id);

            var structureId = StructureId.ConvertFrom(id);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return DbClient.GetJsonById(structureId, structureSchema);

            var item = Db.CacheProvider.Consume(
                structureSchema,
                structureId,
                sid => Db.Serializer.Deserialize(
                    DbClient.GetJsonById(sid, structureSchema),
                    structureSchema.Type.Type),
                CacheConsumeMode);

            return Db.Serializer.Serialize(item);
        }

        public async Task<string> GetByIdAsJsonAsync(Type structureType, object id)
        {
            var structureSchema = PrepareGetById(structureType, id);

            var structureId = StructureId.ConvertFrom(id);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return await DbClient.GetJsonByIdAsync(structureId, structureSchema);

            var item = await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                structureId,
                async sid => Db.Serializer.Deserialize(
                    await DbClient.GetJsonByIdAsync(sid, structureSchema),
                    structureSchema.Type.Type),
                CacheConsumeMode);

            return Db.Serializer.Serialize(item);
        }

        private IStructureSchema PrepareGetById(Type structureType, object id)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(id, "id").IsNotNull();

            var structureSchema = UpsertStructureSchema(structureType);
            return structureSchema;
        }
    }
}