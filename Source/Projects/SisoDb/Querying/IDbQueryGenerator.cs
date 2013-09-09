using SisoDb.Querying.Sql;

namespace SisoDb.Querying
{
    public interface IDbQueryGenerator
    {
        IDbQuery GenerateQuery(IQuery query);
        IDbQuery GenerateQueryReturningStructureIds(IQuery query);
        IDbQuery GenerateQueryReturningCountOfStrutureIds(IQuery query);
    }
}