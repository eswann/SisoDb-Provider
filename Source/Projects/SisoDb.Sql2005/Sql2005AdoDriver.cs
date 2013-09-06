﻿using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.NCore;
using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    public class Sql2005AdoDriver : SqlServerAdoDriver
    {
        protected override DbParameter OnParameterCreated(DbParameter parameter, IDacParameter dacParameter)
        {
            var dbParam = (SqlParameter)parameter;
            var setSize = false;

            if (DbSchemaInfo.Parameters.ShouldBeDateTime(dacParameter))
            {
                dbParam.DbType = DbType.DateTime;
                return dbParam;
            }

            if (DbSchemaInfo.Parameters.ShouldBeNonUnicodeString(dacParameter))
            {
                dbParam.SqlDbType = SqlDbType.VarChar;
                setSize = true;
            }
            else if (DbSchemaInfo.Parameters.ShouldBeUnicodeString(dacParameter))
            {
                dbParam.SqlDbType = SqlDbType.NVarChar;
                setSize = true;
            }

            if (setSize)
            {
                dbParam.Size = (dacParameter.Value.ToStringOrNull() ?? string.Empty).Length;
                return dbParam;
            }

            return dbParam;
        }
    }
}