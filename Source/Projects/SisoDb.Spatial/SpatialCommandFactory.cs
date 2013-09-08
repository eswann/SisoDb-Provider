using Microsoft.SqlServer.Types;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.NCore;

namespace SisoDb.Spatial
{
    public class SpatialCommandFactory : ISpatialCommandFactory
    {
        private const string _geoParamName = "geo";
        private readonly ISession _session;
        private readonly ISqlStatements _sqlStatements = SpatialSqlStatements.Instance;

        public SpatialCommandFactory(ISession session)
        {
            _session = session;
        }

        public SpatialCommand CreateQuery<T>(object id, string queryName) where T : class
        {
            var sid = CreateStructureIdParam<T>(id);

            return CreateQuery<T>(sid, queryName);
        }

        public SpatialCommand CreateQuery<T>(string queryName) where T : class
        {
            return CreateQuery<T>(null, queryName);
        }

        public SpatialCommand CreateQuery<T>(IDacParameter sid, string queryName) where T : class
        {
            var schema = _session.GetStructureSchema<T>();

            return new SpatialCommand
            {
                SidParam = sid,
                Sql = _sqlStatements.GetSql(queryName).Inject(schema.GetSpatialTableName())
            };
        }

        public IDacParameter CreateStructureIdParam<T>(object id) where T : class
        {
            return new DacParameter("id", id);
        }

        public IDacParameter CreateBufferParam(double metres)
        {
            return new DacParameter("buffer", metres);
        }

        public IDacParameter CreateSqlGeoParam(SqlGeography sqlGeo)
        {
            return new GeographyDacParameter(_geoParamName, sqlGeo);
        }

        public GeographyDacParameter CreatePointParam(Coordinates coords, int srid)
        {
            return new GeographyDacParameter(_geoParamName, SqlGeography.Point(coords.Latitude, coords.Longitude, srid));
        }

        public GeographyDacParameter CreateCirlceParam(Coordinates coords, double radiusInMetres, int srid)
        {
            return new GeographyDacParameter(_geoParamName, SqlGeography.Point(coords.Latitude, coords.Longitude, srid).STBuffer(radiusInMetres));
        }

        public GeographyDacParameter CreatePolygonParam(Coordinates[] coords, int srid)
        {
            return new GeographyDacParameter(_geoParamName, GeographyFactory.CreatePolygon(coords, srid));
        }
    }
}