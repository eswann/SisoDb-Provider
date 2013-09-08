namespace SisoDb.Spatial
{
    public static class SpatialCommandNames
    {
        public const string InsertGeo = "InsertGeo";
        public const string UpdateGeo = "UpdateGeo";
        public const string DeleteGeo = "DeleteGeo";
        public const string SetGeo = "SetGeo";
        public const string GetGeo = "GetGeo";

        public const string MakeGeoValid = "MakeGeoValid";
        public const string DropSpatialTable = "DropSpatialTable";

        public const string ContainsPoint = "ContainsPoint";
        public const string ContainsPoint2008 = "ContainsPoint2008";
        public const string ContainsPointWithBuffer = "ContainsPointWithBuffer";
        public const string ContainsPointWithBuffer2008 = "ContainsPointWithBuffer2008";

        public const string UpsertSpatialTableWithIdentity = "UpsertSpatialTableWithIdentity";
        public const string UpsertSpatialTableWithGuid = "UpsertSpatialTableWithGuid";
        public const string UpsertSpatialTableWithString = "UpsertSpatialTableWithString";
    }
}