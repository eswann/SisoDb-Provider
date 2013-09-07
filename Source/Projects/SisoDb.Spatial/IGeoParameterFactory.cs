using Microsoft.SqlServer.Types;
using SisoDb.Dac;

namespace SisoDb.Spatial
{
    public interface IGeoParameterFactory
    {
        GeoParameters CreateGeoParameters<T>(object id, string queryName) where T : class;
        GeoParameters CreateGeoParameters<T>(string queryName) where T : class;
        GeoParameters CreateGeoParameters<T>(IDacParameter sid, string queryName) where T : class;

        IDacParameter CreateStructureIdParam<T>(object id) where T : class;
        IDacParameter CreateBufferParam(double metres);
        IDacParameter CreateSqlGeoParam(SqlGeography sqlGeo);

        GeographyDacParameter CreatePointParam(Coordinates coords, int srid);
        GeographyDacParameter CreateCirlceParam(Coordinates coords, double radiusInMetres, int srid);
        GeographyDacParameter CreatePolygonParam(Coordinates[] coords, int srid);
    }
}