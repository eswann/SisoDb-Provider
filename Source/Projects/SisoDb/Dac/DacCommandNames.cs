namespace SisoDb.Dac
{
    public static class DacCommandNames
    {
        public const string SysIdentitiesCheckOutAndGetNextIdentity = "Sys_Identities_CheckOutAndGetNextIdentity";

        public const string DeleteIndexesAndUniquesById = "DeleteIndexesAndUniquesById";
        public const string DeleteByQuery = "DeleteByQuery";
        public const string DeleteById = "DeleteById";
        public const string DeleteByIds = "DeleteByIds";
        public const string DeleteAll = "DeleteAll";
        public const string DeleteAllExceptIds = "DeleteAllExceptIds";

        public const string SpRename = "sp_rename";
        public const string DropSp = "DropSp";

        public const string DropStructureTables = "DropStructureTables";
        public const string GetTableNamesForAllDataTables = "GetTableNamesForAllDataTables";
        public const string TruncateSisoDbIdentities = "TruncateSisoDbIdentities";
        public const string DropTable = "DropTable";
        public const string ClearIndexesTables = "ClearIndexesTables";
        public const string TableExists = "TableExists";
        public const string GetModelTableStatuses = "GetModelTableStatuses";

        public const string RowCount = "RowCount";
        public const string ExistsById = "ExistsById";

        public const string GetJsonById = "GetJsonById";
        public const string GetJsonByIds = "GetJsonByIds";
        public const string GetJsonByIdWithLock = "GetJsonByIdWithLock";
        public const string GetAllJson = "GetAllJson";

        public const string SingleInsertStructure = "SingleInsertStructure";
        public const string SingleInsertOfValueTypeIndex = "SingleInsertOfValueTypeIndex";
        public const string SingleInsertOfStringTypeIndex = "SingleInsertOfStringTypeIndex";
        public const string SingleInsertOfUniqueIndex = "SingleInsertOfUniqueIndex";
        public const string SingleUpdateOfStructure = "SingleUpdateOfStructure";

    }
}