using System.Collections.Generic;
using SisoDb.Querying.Sql;

namespace SisoDb.Querying.Lambdas.Converters.Sql
{
    public interface ILambdaToSqlWhereConverter : IParsedLambdaConverter<IList<SqlWhere>>
    {}
}