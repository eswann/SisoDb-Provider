using SisoDb.Dac;
using SisoDb.Querying;

namespace SisoDb.DbSessionHelpers
{
    internal abstract class ReadHelper : DbSessionHelper
    {
        protected ReadHelper(ISisoDatabase db, IDbClient dbClient, IDbQueryGenerator queryGenerator) : base(db, dbClient)
        {
            QueryGenerator = queryGenerator;
        }

        public IDbQueryGenerator QueryGenerator { get; private set; }
    }
}