using SisoDb.Dac;
using SisoDb.Querying;

namespace SisoDb.DbSessionHelpers
{
    internal abstract class ModifyHelper : ReadHelper
    {
        protected ModifyHelper(ISisoDatabase db, IDbClient dbClient, IDbQueryGenerator queryGenerator, ISession session, SessionEvents internalEvents) : base(db, dbClient, queryGenerator)
        {
            Session = session;
            InternalEvents = internalEvents;
        }

        public ISession Session { get; private set; }
        public SessionEvents InternalEvents { get; private set; } 
    }
}