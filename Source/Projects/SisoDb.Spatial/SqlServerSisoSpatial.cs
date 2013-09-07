using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlServer.Types;
using SisoDb.Dac;
using SisoDb.NCore;
using SisoDb.DbSchema;
using SisoDb.Spatial.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Spatial
{
    public class SqlServerSisoSpatial : ISisoSpatial
    {
        private readonly IGeoParameterFactory _geoParamFactory;

        protected readonly ISessionExecutionContext ExecutionContext;
        protected ISession Session { get { return ExecutionContext.Session; } }
        protected readonly ISqlStatements SqlStatements;

        protected internal SqlServerSisoSpatial(ISessionExecutionContext executionContext)
        {
            ExecutionContext = executionContext;
            SqlStatements = SpatialSqlStatements.Instance;
            _geoParamFactory = new GeoParameterFactory(Session);
        }

        public virtual void EnableFor<T>() where T : class
        {
            ExecutionContext.Try(() =>
            {
                if (!Session.Db.Settings.AllowDynamicSchemaCreation) return;
                var schema = Session.GetStructureSchema<T>();
                var sql = GetUpsertTableSql(schema);
                Session.DbClient.ExecuteNonQuery(sql);
            });
        }

        public virtual async Task EnableForAsync<T>() where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                if (!Session.Db.Settings.AllowDynamicSchemaCreation) return;
                var schema = Session.GetStructureSchema<T>();
                var sql = GetUpsertTableSql(schema);
                await Session.DbClient.ExecuteNonQueryAsync(sql);
            });
        }

        public virtual void RemoveFor<T>() where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sql = SqlStatements.GetSql(SpatialQueryNames.DropSpatialTable).Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql);
            });
        }

        public virtual async Task RemoveForAsync<T>() where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                GeoParameters geoParams = _geoParamFactory.CreateGeoParameters<T>(SpatialQueryNames.DropSpatialTable);

                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql);
            });
        }

        public virtual void DeleteGeoFor<T>(object id) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.DeleteGeo);

                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam);
            });
        }

        public virtual async Task DeleteGeoForAsync<T>(object id) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.DeleteGeo);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam);
            });
        }

        public virtual void InsertPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.InsertGeo);
                var shapeParam = _geoParamFactory.CreatePointParam(coords, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task InsertPointAsync<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.InsertGeo);
                var shapeParam = _geoParamFactory.CreatePointParam(coords, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void UpdatePoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.UpdateGeo);
                var shapeParam = _geoParamFactory.CreatePointParam(coords, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task UpdatePointAsync<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.UpdateGeo);
                var shapeParam = _geoParamFactory.CreatePointParam(coords, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void SetPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.SetGeo);
                var shapeParam = _geoParamFactory.CreatePointParam(coords, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task SetPointAsync<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.SetGeo);
                var shapeParam = _geoParamFactory.CreatePointParam(coords, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void InsertCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.InsertGeo);
                var shapeParam = _geoParamFactory.CreateCirlceParam(center, radiusInMetres, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task InsertCircleAsync<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.InsertGeo);
                var shapeParam = _geoParamFactory.CreateCirlceParam(center, radiusInMetres, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void UpdateCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.UpdateGeo);
                var shapeParam = _geoParamFactory.CreateCirlceParam(center, radiusInMetres, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task UpdateCircleAsync<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.UpdateGeo);
                var shapeParam = _geoParamFactory.CreateCirlceParam(center, radiusInMetres, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void SetCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.SetGeo);
                var shapeParam = _geoParamFactory.CreateCirlceParam(center, radiusInMetres, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task SetCircleAsync<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.SetGeo);
                var shapeParam = _geoParamFactory.CreateCirlceParam(center, radiusInMetres, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void InsertPolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.InsertGeo);
                var shapeParam = _geoParamFactory.CreatePolygonParam(coords, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task InsertPolygonAsync<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.InsertGeo);
                var shapeParam = _geoParamFactory.CreatePolygonParam(coords, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void UpdatePolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.UpdateGeo);
                var shapeParam = _geoParamFactory.CreatePolygonParam(coords, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task UpdatePolygonAsync<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.UpdateGeo);
                var shapeParam = _geoParamFactory.CreatePolygonParam(coords, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void SetPolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.SetGeo);
                var shapeParam = _geoParamFactory.CreatePolygonParam(coords, srid);
                Session.DbClient.ExecuteNonQuery(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual async Task SetPolygonAsync<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.SetGeo);
                var shapeParam = _geoParamFactory.CreatePolygonParam(coords, srid);
                await Session.DbClient.ExecuteNonQueryAsync(geoParams.Sql, geoParams.SidParam, shapeParam);
            });
        }

        public virtual void MakeValid<T>(object id, int srid = SpatialReferenceId.Wsg84) where T : class 
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = _geoParamFactory.CreateStructureIdParam<T>(id);
                if (Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008)
                {
                    var sql = SqlStatements.GetSql(SpatialQueryNames.GetGeo).Inject(schema.GetSpatialTableName());
                    var geo = ReadGeograpy(sql, sidParam);
                    if (geo.STIsValid())
                        return;

                    geo = geo.MakeValid();

                    var geoParam = _geoParamFactory.CreateSqlGeoParam(geo);
                    sql = SqlStatements.GetSql(SpatialQueryNames.UpdateGeo).Inject(schema.GetSpatialTableName());
                    Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
                }
                else
                {
                    var sql = SqlStatements.GetSql(SpatialQueryNames.MakeGeoValid).Inject(schema.GetSpatialTableName());
                    Session.DbClient.ExecuteNonQuery(sql, sidParam);
                }
            });
        }

        public virtual async Task MakeValidAsync<T>(object id, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            await ExecutionContext.TryAsync(async () =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = _geoParamFactory.CreateStructureIdParam<T>(id);
                if (Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008)
                {
                    var sql = SqlStatements.GetSql(SpatialQueryNames.GetGeo).Inject(schema.GetSpatialTableName());
                    var geo = await ReadGeograpyAsync(sql, sidParam);
                    if (geo.STIsValid())
                        return;

                    geo = geo.MakeValid();

                    var geoParam = _geoParamFactory.CreateSqlGeoParam(geo);
                    sql = SqlStatements.GetSql(SpatialQueryNames.UpdateGeo).Inject(schema.GetSpatialTableName());
                    await Session.DbClient.ExecuteNonQueryAsync(sql, sidParam, geoParam);
                }
                else
                {
                    var sql = SqlStatements.GetSql(SpatialQueryNames.MakeGeoValid).Inject(schema.GetSpatialTableName());
                    await Session.DbClient.ExecuteNonQueryAsync(sql, sidParam);
                }
            });
        }


        public virtual Coordinates[] GetCoordinatesIn<T>(object id) where T : class
        {
            return ExecutionContext.Try(() =>
            {
                GeoParameters geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.GetGeo);

                return ExtractPoints(ReadGeograpy(geoParams.Sql, geoParams.SidParam)).Select(p => new Coordinates
                {
                    Latitude = p.Lat.Value,
                    Longitude = p.Long.Value
                }).ToArray();
            });
        }

        public virtual async Task<Coordinates[]> GetCoordinatesInAsync<T>(object id) where T : class
        {
            return await ExecutionContext.TryAsync(async () =>
            {
                GeoParameters geoParams = _geoParamFactory.CreateGeoParameters<T>(id, SpatialQueryNames.GetGeo);

                return ExtractPoints(await ReadGeograpyAsync(geoParams.Sql, geoParams.SidParam)).Select(p => new Coordinates
                {
                    Latitude = p.Lat.Value,
                    Longitude = p.Long.Value
                }).ToArray();
            });
        }

        protected virtual SqlGeography ReadGeograpy(string sql, params IDacParameter[] parameters)
        {
            //I know, ugly. Thank Microsoft for that: http://msdn.microsoft.com/en-us/library/ms143179.aspx
            SqlGeography geo = null;
            Session.DbClient.Read(sql, dr =>
            {
                var d = (SqlDataReader)dr;
                geo = SqlGeography.Deserialize(d.GetSqlBytes(0));
            }, parameters);
            return geo;
        }

        protected virtual async Task<SqlGeography> ReadGeograpyAsync(string sql, params IDacParameter[] parameters)
        {
            //I know, ugly. Thank Microsoft for that: http://msdn.microsoft.com/en-us/library/ms143179.aspx
            SqlGeography geo = null;
            await Session.DbClient.ReadAsync(sql, dr =>
            {
                var d = (SqlDataReader)dr;
                geo = SqlGeography.Deserialize(d.GetSqlBytes(0));
            }, parameters);
            return geo;
        }

        protected virtual SqlGeography[] ExtractPoints(SqlGeography geo)
        {
            if(geo == null || geo.IsNull)
                return new SqlGeography[0];

            var numOfPoints = geo.STNumPoints();
            if (numOfPoints.IsNull || numOfPoints.Value == 0)
                return new SqlGeography[0];

            var points = new SqlGeography[numOfPoints.Value];
            for (var i = 0; i < numOfPoints.Value; i++)
                points[i] = geo.STPointN(i+1);

            return points;
        }

        public virtual bool ContainsPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            return ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = _geoParamFactory.CreateStructureIdParam<T>(id);
                var geoParam = _geoParamFactory.CreatePointParam(coords, srid);

                var sql = (Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008)
                    ? SqlStatements.GetSql(SpatialQueryNames.ContainsPoint2008).Inject(schema.GetSpatialTableName())
                    : SqlStatements.GetSql(SpatialQueryNames.ContainsPoint).Inject(schema.GetSpatialTableName());

                return Session.DbClient.ExecuteScalar<bool>(sql, sidParam, geoParam);
            });
        }

        public virtual async Task<bool> ContainsPointAsync<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            return await ExecutionContext.TryAsync(async () =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = _geoParamFactory.CreateStructureIdParam<T>(id);
                var geoParam = _geoParamFactory.CreatePointParam(coords, srid);

                var sql = (Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008)
                    ? SqlStatements.GetSql(SpatialQueryNames.ContainsPoint2008).Inject(schema.GetSpatialTableName())
                    : SqlStatements.GetSql(SpatialQueryNames.ContainsPoint).Inject(schema.GetSpatialTableName());

                return await Session.DbClient.ExecuteScalarAsync<bool>(sql, sidParam, geoParam);
            });
        }

        public virtual bool ContainsPointAfterExpand<T>(object id, Coordinates coords, double expandWithMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            return ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = _geoParamFactory.CreateStructureIdParam<T>(id);
                var geoParam = _geoParamFactory.CreatePointParam(coords, srid);
                var bufferParam = _geoParamFactory.CreateBufferParam(expandWithMetres);
                var sql = (Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008)
                    ? SqlStatements.GetSql(SpatialQueryNames.ContainsPointWithBuffer2008).Inject(schema.GetSpatialTableName())
                    : SqlStatements.GetSql(SpatialQueryNames.ContainsPointWithBuffer).Inject(schema.GetSpatialTableName());

                return Session.DbClient.ExecuteScalar<bool>(sql, sidParam, geoParam, bufferParam);
            });
        }

        public virtual async Task<bool> ContainsPointAfterExpandAsync<T>(object id, Coordinates coords, double expandWithMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            return await ExecutionContext.TryAsync(async() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = _geoParamFactory.CreateStructureIdParam<T>(id);
                var geoParam = _geoParamFactory.CreatePointParam(coords, srid);
                var bufferParam = _geoParamFactory.CreateBufferParam(expandWithMetres);
                var sql = (Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008)
                    ? SqlStatements.GetSql(SpatialQueryNames.ContainsPointWithBuffer2008).Inject(schema.GetSpatialTableName())
                    : SqlStatements.GetSql(SpatialQueryNames.ContainsPointWithBuffer).Inject(schema.GetSpatialTableName());

                return await Session.DbClient.ExecuteScalarAsync<bool>(sql, sidParam, geoParam, bufferParam);
            });
        }

        protected virtual string GetUpsertTableSql(IStructureSchema structureSchema)
        {
            var tableName = structureSchema.GetSpatialTableName();
            var structureTableName = structureSchema.GetStructureTableName();

            if (structureSchema.IdAccessor.IdType.IsIdentity())
                return SqlStatements.GetSql(SpatialQueryNames.UpsertSpatialTableWithIdentity).Inject(tableName, structureTableName);

            if (structureSchema.IdAccessor.IdType.IsGuid())
                return SqlStatements.GetSql(SpatialQueryNames.UpsertSpatialTableWithGuid).Inject(tableName, structureTableName);

            if (structureSchema.IdAccessor.IdType.IsString())
                return SqlStatements.GetSql(SpatialQueryNames.UpsertSpatialTableWithString).Inject(tableName, structureTableName);

            throw new SisoDbException(ExceptionMessages.IdTypeNotSupported.Inject(structureSchema.IdAccessor.IdType));
        }


    }
}