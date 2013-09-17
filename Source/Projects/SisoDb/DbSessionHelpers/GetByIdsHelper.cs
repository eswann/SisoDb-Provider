using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.EnsureThat;
using SisoDb.NCore.Collections;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal class GetByIdsHelper : DbSessionHelper
    {
        public GetByIdsHelper(DbSession session)
            : base(session)
        {
        }

        public IEnumerable<object> GetByIds(Type structureType, params object[] ids)
        {
            var structureSchema = PrepareGetByIds(structureType, ids);

            var structureIds = MapStructureIds(ids);

            return Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany(DbClient.GetJsonByIds(sids, structureSchema).Where(s => s != null), structureType),
                CacheConsumeMode);
        }

        public async Task<IEnumerable<object>> GetByIdsAsync(Type structureType, params object[] ids)
        {
            var structureSchema = PrepareGetByIds(structureType, ids);

            var structureIds = MapStructureIds(ids);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                structureIds,
                async sids => Db.Serializer.DeserializeMany((await DbClient.GetJsonByIdsAsync(sids, structureSchema)).Where(s => s != null), structureType),
                CacheConsumeMode);
        }

        public IEnumerable<TOut> GetByIdsAs<TOut>(Type structureType, params object[] ids) where TOut : class
        {
            var structureSchema = PrepareGetByIds(structureType, ids);

            var structureIds = MapStructureIds(ids);

            return Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany<TOut>(DbClient.GetJsonByIds(sids, structureSchema).Where(s => s != null)),
                CacheConsumeMode);
        }

        public async Task<IEnumerable<TOut>> GetByIdsAsAsync<TOut>(Type structureType, params object[] ids) where TOut : class
        {
            var structureSchema = PrepareGetByIds(structureType, ids);

            var structureIds = MapStructureIds(ids);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                structureIds,
                async sids => Db.Serializer.DeserializeMany<TOut>((await DbClient.GetJsonByIdsAsync(sids, structureSchema)).Where(s => s != null)),
                CacheConsumeMode);
        }

        public IEnumerable<string> GetByIdsAsJson(Type structureType, params object[] ids)
        {
            var structureSchema = PrepareGetByIds(structureType, ids);

            var structureIds = MapStructureIds(ids);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return DbClient.GetJsonByIds(structureIds, structureSchema).Where(s => s != null);

            var items = Db.CacheProvider.Consume(
                structureSchema,
                structureIds,
                sids => Db.Serializer.DeserializeMany(
                    DbClient.GetJsonByIds(sids, structureSchema),
                    structureSchema.Type.Type).Where(s => s != null),
                CacheConsumeMode);

            return Db.Serializer.SerializeMany(items);
        }

        public async Task<IEnumerable<string>> GetByIdsAsJsonAsync(Type structureType, params object[] ids)
        {
            var structureSchema = PrepareGetByIds(structureType, ids);

            var structureIds = MapStructureIds(ids);

            if (!Db.CacheProvider.IsEnabledFor(structureSchema))
                return (await DbClient.GetJsonByIdsAsync(structureIds, structureSchema)).Where(s => s != null);

            var items = await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                structureIds,
                async sids => Db.Serializer.DeserializeMany(
                    await DbClient.GetJsonByIdsAsync(sids, structureSchema),
                    structureSchema.Type.Type).Where(s => s != null),
                CacheConsumeMode);

            return Db.Serializer.SerializeMany(items);
        }

        private IStructureSchema PrepareGetByIds(Type structureType, object[] ids)
        {
            Ensure.That(ids, "ids").HasItems();
            var structureSchema = UpsertStructureSchema(structureType);
            return structureSchema;
        }

        private static IStructureId[] MapStructureIds(IEnumerable<object> ids)
        {
            var structureIds = ids.Yield().Select(StructureId.ConvertFrom).ToArray();
            return structureIds;
        }
    }
}