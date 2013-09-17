using System.Threading.Tasks;
using SisoDb.Caching;
using SisoDb.EnsureThat;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSessionHelpers
{
    internal class CheckoutByIdHelper : DbSessionHelper
    {
        public CheckoutByIdHelper(DbSession session)
            : base(session)
        {
        }

        internal T CheckoutById<T>(object id) where T : class
        {
            var structureSchema = PrepareCheckoutById<T>(id);

            return Db.CacheProvider.Consume(
                structureSchema,
                StructureId.ConvertFrom(id),
                sid => Db.Serializer.Deserialize<T>(DbClient.GetJsonByIdWithLock(sid, structureSchema)),
                CacheConsumeMode);
        }

        internal async Task<T> CheckoutByIdAsync<T>(object id) where T : class
        {
            var structureSchema = PrepareCheckoutById<T>(id);

            return await Db.CacheProvider.ConsumeAsync(
                structureSchema,
                StructureId.ConvertFrom(id),
                async sid => Db.Serializer.Deserialize<T>(await DbClient.GetJsonByIdWithLockAsync(sid, structureSchema)),
                CacheConsumeMode);
        }

        private IStructureSchema PrepareCheckoutById<T>(object id) where T : class
        {
            Ensure.That(id, "id").IsNotNull();
            return UpsertStructureSchema<T>();
        }
    }
}
