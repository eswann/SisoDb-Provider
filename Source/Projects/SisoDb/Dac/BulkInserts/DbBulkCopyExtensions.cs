using System.Collections.Generic;
using System.Linq;
using SisoDb.DbSchema;
using SisoDb.Structures;

namespace SisoDb.Dac.BulkInserts
{
    public static class DbBulkCopyExtensions
    {
        public static void PrepForIndexesInsert(this IDbBulkCopy bulkInserter, IndexesReader reader)
        {
            bulkInserter.DestinationTableName = reader.StorageSchema.Name;
            bulkInserter.BatchSize = reader.RecordsAffected;

            var fieldsToSkip = new HashSet<SchemaField> { IndexStorageSchema.Fields.RowId };

            if (!(reader is ValueTypeIndexesReader))
                fieldsToSkip.Add(IndexStorageSchema.Fields.StringValue);

            var fields = reader.StorageSchema.GetFieldsOrderedByIndex().Except(fieldsToSkip).ToArray();

            foreach (var field in fields)
                bulkInserter.AddColumnMapping(field.Name, field.Name);
        }

        public static void PrepForStructuresInsert(this IDbBulkCopy bulkInserter, StructuresReader reader, IStructure[] structures)
        {
            bulkInserter.DestinationTableName = reader.StorageSchema.Name;
            bulkInserter.BatchSize = structures.Length;

            var fields = reader.StorageSchema.GetFieldsOrderedByIndex().Where(f => !f.Equals(StructureStorageSchema.Fields.RowId)).ToArray();

            foreach (var field in fields)
                bulkInserter.AddColumnMapping(field.Name, field.Name);
        }

        public static void PrepForUniquesInsert(this IDbBulkCopy bulkInserter, UniquesReader reader, IStructureIndex[] uniques)
        {
            bulkInserter.DestinationTableName = reader.StorageSchema.Name;
            bulkInserter.BatchSize = uniques.Length;

            var fields = reader.StorageSchema.GetFieldsOrderedByIndex().Where(f => !f.Equals(StructureStorageSchema.Fields.RowId)).ToArray();
            foreach (var field in fields)
                bulkInserter.AddColumnMapping(field.Name, field.Name);
        }

    }
}