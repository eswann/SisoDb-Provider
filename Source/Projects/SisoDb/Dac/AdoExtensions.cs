using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace SisoDb.Dac
{
    public static class AdoExtensions
    {
        public static DbDataReader SingleResultSequentialReader(this DbCommand cmd)
        {
            return cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess);
        }

        public static async Task<DbDataReader> SingleResultSequentialReaderAsync(this DbCommand cmd)
        {
            return await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess);
        }
    }
}