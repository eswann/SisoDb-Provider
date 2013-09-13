using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SisoDb.DbSchema;
using SisoDb.NCore.Collections;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Dac.BulkInserts
{
    public class DbStructureInserter : IStructureInserter
    {
        protected class IndexInsertAction
        {
            public IStructureIndex[] Data;
            public Action<IStructureIndex[], IDbClient> Action;
            public Func<IStructureIndex[], IDbClient, Task> AsyncAction;
            public bool HasData
            {
                get { return Data != null && Data.Length > 0; }
            }
        }

        protected const int MaxNumOfStructuresBeforeParallelEscalation = 10;
        protected readonly IDbClient MainDbClient;

        public DbStructureInserter(IDbClient mainDbClient)
        {
            MainDbClient = mainDbClient;
        }

        public virtual void Insert(IStructureSchema structureSchema, IStructure[] structures)
        {
            var groupedIndexInsertActions = new IndexInsertAction[0];

            Task task = null;
            try
            {
                task = Task.Factory.StartNew(
                    () => groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, structures, false));

                InsertStructures(structureSchema, structures);
                InsertUniques(structureSchema, structures);

                Task.WaitAll(task);
            }
            finally
            {
                if (task != null && task.Status == TaskStatus.RanToCompletion)
                    task.Dispose();
            }

            if (groupedIndexInsertActions.Length == 0)
                return;

            InsertIndexes(groupedIndexInsertActions);
        }

        public virtual async Task InsertAsync(IStructureSchema structureSchema, IStructure[] structures)
        {
            await InsertStructuresAsync(structureSchema, structures);
            await InsertUniquesAsync(structureSchema, structures);

            var groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, structures, true);
            if (groupedIndexInsertActions.Length > 0)
                await InsertIndexesAsync(groupedIndexInsertActions);

        }

        public virtual void InsertIndexesOnly(IStructureSchema structureSchema, IStructure[] structures)
        {
            var groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, structures, false);
            if (groupedIndexInsertActions.Length == 0)
                return;

            InsertIndexes(groupedIndexInsertActions);
        }

        public virtual async Task InsertIndexesOnlyAsync(IStructureSchema structureSchema, IStructure[] structures)
        {
            var groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, structures, true);
            if (groupedIndexInsertActions.Length == 0)
                return;

            await InsertIndexesAsync(groupedIndexInsertActions);
        }

        public virtual void Replace(IStructureSchema structureSchema, IStructure structure)
        {
            var structures = new[] { structure };
            MainDbClient.SingleUpdateOfStructure(structure, structureSchema);
            InsertUniques(structureSchema, structures);

            var groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, new[] { structure }, false);
            if (groupedIndexInsertActions.Length == 0)
                return;

            InsertIndexes(groupedIndexInsertActions);
        }

        public virtual async Task ReplaceAsync(IStructureSchema structureSchema, IStructure structure)
        {
            var structures = new[] { structure };
            await MainDbClient.SingleUpdateOfStructureAsync(structure, structureSchema);
            await InsertUniquesAsync(structureSchema, structures);

            var groupedIndexInsertActions = CreateGroupedIndexInsertActions(structureSchema, new[] { structure }, true);
            if (groupedIndexInsertActions.Length == 0)
                return;

            await InsertIndexesAsync(groupedIndexInsertActions);
        }

        protected virtual void InsertStructures(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (structures.Length == 0)
                return;

            if (structures.Length == 1)
                MainDbClient.SingleInsertStructure(structures[0], structureSchema);
            else
                MainDbClient.BulkInsertStructures(structureSchema, structures);
        }

        protected virtual async Task InsertStructuresAsync(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (structures.Length == 0)
                return;

            if (structures.Length == 1)
                await MainDbClient.SingleInsertStructureAsync(structures[0], structureSchema);
            else
                await MainDbClient.BulkInsertStructuresAsync(structureSchema, structures);
        }

        protected virtual void InsertIndexes(IndexInsertAction[] groupedIndexInsertActions)
        {
            foreach (var groupedIndexInsertAction in groupedIndexInsertActions.Where(i => i.Action != null))
                groupedIndexInsertAction.Action(groupedIndexInsertAction.Data, MainDbClient);
        }

        protected virtual async Task InsertIndexesAsync(IndexInsertAction[] groupedIndexInsertActions)
        {
            var actionsToExecute = groupedIndexInsertActions.Where(i => i.AsyncAction != null);
            await Task.WhenAll(actionsToExecute.Select(x => x.AsyncAction(x.Data, MainDbClient)));
        }

        protected virtual ISet<SchemaField> GetStorageSchemaFieldsToSkip(bool isValueTypeIndexesReader)
        {
            var fieldsToSkip = new HashSet<SchemaField> { IndexStorageSchema.Fields.RowId };

            if (!isValueTypeIndexesReader)
                fieldsToSkip.Add(IndexStorageSchema.Fields.StringValue);
            
            return fieldsToSkip;
        }

        protected virtual void InsertUniques(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (structures.Length == 0)
                return;

            var uniques = structures.SelectMany(s => s.Uniques).ToArray();
            if (uniques.Length == 0)
                return;

            if (uniques.Length == 1)
                MainDbClient.SingleInsertOfUniqueIndex(uniques[0], structureSchema);
            else
                MainDbClient.BulkInsertUniques(structureSchema, uniques);
        }

        protected virtual async Task InsertUniquesAsync(IStructureSchema structureSchema, IStructure[] structures)
        {
            if (structures.Length == 0)
                return;

            var uniques = structures.SelectMany(s => s.Uniques).ToArray();
            if (uniques.Length == 0)
                return;

            if (uniques.Length == 1)
                await MainDbClient.SingleInsertOfUniqueIndexAsync(uniques[0], structureSchema);
            else
                await MainDbClient.BulkInsertUniquesAsync(structureSchema, uniques);
        }

        protected virtual IndexInsertAction[] CreateGroupedIndexInsertActions(IStructureSchema structureSchema, IStructure[] structures, bool async)
        {
            var indexesTableNames = structureSchema.GetIndexesTableNames();
            var insertActions = new Dictionary<DataTypeCode, IndexInsertAction>(indexesTableNames.All.Length);
            foreach (var group in structures.SelectMany(s => s.Indexes).GroupBy(i => i.DataTypeCode))
            {
                var insertAction = CreateIndexInsertActionGroup(structureSchema, indexesTableNames, group.Key, group.ToArray(), async);
                if (insertAction.HasData)
                    insertActions.Add(group.Key, insertAction);
            }

            var mergeStringsAndEnums = insertActions.ContainsKey(DataTypeCode.String) && insertActions.ContainsKey(DataTypeCode.Enum);
            if (mergeStringsAndEnums)
            {
                var strings = insertActions[DataTypeCode.String];
                var enums = insertActions[DataTypeCode.Enum];
                insertActions.Remove(DataTypeCode.Enum);

                insertActions[DataTypeCode.String] = CreateIndexInsertActionGroup(structureSchema, indexesTableNames, DataTypeCode.String, strings.Data.MergeWith(enums.Data).ToArray(), async);
            }

            return insertActions.Values.ToArray();
        }

        protected virtual IndexInsertAction CreateIndexInsertActionGroup(IStructureSchema structureSchema, IndexesTableNames indexesTableNames, 
            DataTypeCode dataTypeCode, IStructureIndex[] indexes, bool async)
        {
            var container = new IndexInsertAction { Data = indexes };

            switch (dataTypeCode)
            {
                case DataTypeCode.IntegerNumber:
                    if (async)
                    {
                        if (container.Data.Length > 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.BulkInsertIndexesAsync(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.IntegersTableName), data));
                        if (container.Data.Length == 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.SingleInsertOfValueTypeIndexAsync(data[0], indexesTableNames.IntegersTableName);
                    }
                    else
                    {
                        if (container.Data.Length > 1)
                            container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.IntegersTableName), data));
                        if (container.Data.Length == 1)
                            container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.IntegersTableName);
                    }
                    break;
                case DataTypeCode.FractalNumber:
                    if (async)
                    {
                        if (container.Data.Length > 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.BulkInsertIndexesAsync(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.FractalsTableName), data));
                        if (container.Data.Length == 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.SingleInsertOfValueTypeIndexAsync(data[0], indexesTableNames.FractalsTableName);
                    }
                    else
                    {
                        if (container.Data.Length > 1)
                            container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.FractalsTableName), data));
                        if (container.Data.Length == 1)
                            container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.FractalsTableName);
                    }

                    break;
                case DataTypeCode.Bool:
                    if (async)
                    {
                        if (container.Data.Length > 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.BulkInsertIndexesAsync(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.BooleansTableName), data));
                        if (container.Data.Length == 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.SingleInsertOfValueTypeIndexAsync(data[0], indexesTableNames.BooleansTableName);
                    }
                    else
                    {
                        if (container.Data.Length > 1)
                            container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.BooleansTableName), data));
                        if (container.Data.Length == 1)
                            container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.BooleansTableName);
                    }
                    break;
                case DataTypeCode.DateTime:
                    if (async)
                    {
                        if (container.Data.Length > 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.BulkInsertIndexesAsync(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.DatesTableName), data));
                        if (container.Data.Length == 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.SingleInsertOfValueTypeIndexAsync(data[0], indexesTableNames.DatesTableName);
                    }
                    else
                    {
                        if (container.Data.Length > 1)
                            container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.DatesTableName), data));
                        if (container.Data.Length == 1)
                            container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.DatesTableName);
                    }
                    break;
                case DataTypeCode.Guid:
                    if (async)
                    {
                        if (container.Data.Length > 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.BulkInsertIndexesAsync(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.GuidsTableName), data));
                        if (container.Data.Length == 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.SingleInsertOfValueTypeIndexAsync(data[0], indexesTableNames.GuidsTableName);
                    }
                    else
                    {
                        if (container.Data.Length > 1)
                            container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new ValueTypeIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.GuidsTableName), data));
                        if (container.Data.Length == 1)
                            container.Action = (data, dbClient) => dbClient.SingleInsertOfValueTypeIndex(data[0], indexesTableNames.GuidsTableName);
                    }
                    break;
                case DataTypeCode.String:
                    if (async)
                    {
                        if (container.Data.Length > 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.BulkInsertIndexesAsync(new StringIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.StringsTableName), data));
                        if (container.Data.Length == 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.SingleInsertOfStringTypeIndexAsync(data[0], indexesTableNames.StringsTableName);
                    }
                    else
                    {
                        if (container.Data.Length > 1)
                            container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new StringIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.StringsTableName), data));
                        if (container.Data.Length == 1)
                            container.Action = (data, dbClient) => dbClient.SingleInsertOfStringTypeIndex(data[0], indexesTableNames.StringsTableName);
                    }
                    break;
                case DataTypeCode.Enum:
                    if (async)
                    {
                        if (container.Data.Length > 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.BulkInsertIndexesAsync(new StringIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.StringsTableName), data));
                        if (container.Data.Length == 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.SingleInsertOfStringTypeIndexAsync(data[0], indexesTableNames.StringsTableName);
                    }
                    else
                    {
                        if (container.Data.Length > 1)
                            container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new StringIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.StringsTableName), data));
                        if (container.Data.Length == 1)
                            container.Action = (data, dbClient) => dbClient.SingleInsertOfStringTypeIndex(data[0], indexesTableNames.StringsTableName);
                    }
                    break;
                case DataTypeCode.Text:
                    if (async)
                    {
                        if (container.Data.Length > 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.BulkInsertIndexesAsync(new TextIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.TextsTableName), data));
                        if (container.Data.Length == 1)
                            container.AsyncAction = async (data, dbClient) => await dbClient.SingleInsertOfStringTypeIndexAsync(data[0], indexesTableNames.TextsTableName);
                    }
                    else
                    {
                        if (container.Data.Length > 1)
                            container.Action = (data, dbClient) => dbClient.BulkInsertIndexes(new TextIndexesReader(new IndexStorageSchema(structureSchema, indexesTableNames.TextsTableName), data));
                        if (container.Data.Length == 1)
                            container.Action = (data, dbClient) => dbClient.SingleInsertOfStringTypeIndex(data[0], indexesTableNames.TextsTableName);
                    }
                    break;
                                            
                default:
                    container.Action = null;
                    break;
            }

            return container;
        }
    }
}