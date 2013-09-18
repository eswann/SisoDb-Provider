using System.Data.Common;

namespace SisoDb.Dac.Profiling
{
    /// <summary>
    /// This exists because SqlBulkCopy depends on SqlTransaction and that class is sealed
    /// so we can't derive from it to add profiling
    /// </summary>
    public interface IWrappedTransaction
    {
        DbTransaction GetInnerTransaction();
    }
}
