using System;

namespace SisoDb.Testing
{
    public class TestConstants
    {
        public static readonly DateTime FixedDateTime = new DateTime(2012, 1, 2, 3, 4, 5, 6);

        public const string ConnectionStringNameForAzure = "SisoDb.Azure";

        public const string ConnectionStringNameForAzureAsync = "SisoDb.Azure.Async";

		public const string ConnectionStringNameForSql2005 = "SisoDb.Sql2005";

        public const string ConnectionStringNameForSql2005Temp = "SisoDb.Sql2005.Temp";

        public const string ConnectionStringNameForSql2005Async = "SisoDb.Sql2005.Async";

        public const string ConnectionStringNameForSql2008 = "SisoDb.Sql2008";

        public const string ConnectionStringNameForSql2008Async = "SisoDb.Sql2008.Async";

        public const string ConnectionStringNameForSql2008Temp = "SisoDb.Sql2008.Temp";

        public const string ConnectionStringNameForSql2012 = "SisoDb.Sql2012";

        public const string ConnectionStringNameForSql2012Temp = "SisoDb.Sql2012.Temp";

        public const string ConnectionStringNameForSql2012Async = "SisoDb.Sql2012.Async";

        public const string ConnectionStringNameForSqlCe4 = "SisoDb.SqlCe4";

        public const string ConnectionStringNameForSqlCe4Temp = "SisoDb.SqlCe4.Temp";

        public const string ConnectionStringNameForSqlCe4Async= "SisoDb.SqlCe4.Async";

        public const string ConnectionStringNameForSqlProfiler = "SisoDb.SqlProfiler";

        public const string ConnectionStringNameForSqlProfilerTemp = "SisoDb.SqlProfiler.Temp";

        public const string ConnectionStringNameForSqlProfilerAsync= "SisoDb.SqlProfiler.Async";
    }
}