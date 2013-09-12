﻿using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.Session
{
    class DeletesAsync
    {
        [Subject(typeof(ISession), "Delete by query")]
        public class when_guiditem_and_deleting_async_two_of_four_items_using_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using(var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByQueryAsync<GuidItem>(i => i.Value >= _structures[1].Value && i.Value <= _structures[2].Value).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by query")]
        public class when_stringitem_and_deleting_async_two_of_four_items_using_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertStringItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<StringItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByQueryAsync<StringItem>(i => i.Value >= _structures[1].Value && i.Value <= _structures[2].Value).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<StringItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<StringItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by query")]
        public class when_identityitem_and_deleting_async_two_of_four_items_using_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByQueryAsync<IdentityItem>(i => i.Value >= _structures[1].Value && i.Value <= _structures[2].Value).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by query")]
        public class when_bigidentityitem_and_deleting_async_two_of_four_items_using_query : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertBigIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<BigIdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByQueryAsync<BigIdentityItem>(i => i.Value >= _structures[1].Value && i.Value <= _structures[2].Value).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<BigIdentityItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id")]
        public class when_guiditem_and_deleting_async_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdAsync<GuidItem>(_structures[1].StructureId).Wait();
                    session.DeleteByIdAsync<GuidItem>(_structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id (uniques)")]
        public class when_uniqueguiditem_and_deleting_async_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertUniqueGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueGuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdAsync<UniqueGuidItem>(_structures[1].StructureId).Wait();
                    session.DeleteByIdAsync<UniqueGuidItem>(_structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<UniqueGuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<UniqueGuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id")]
        public class when_stringitem_and_deleting_async_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertStringItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<StringItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdAsync<StringItem>(_structures[1].StructureId).Wait();
                    session.DeleteByIdAsync<StringItem>(_structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<StringItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<StringItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id (uniques)")]
        public class when_uniquestringitem_and_deleting_async_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertUniqueStringItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueStringItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdAsync<UniqueStringItem>(_structures[1].StructureId).Wait();
                    session.DeleteByIdAsync<UniqueStringItem>(_structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<UniqueStringItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<UniqueStringItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id")]
        public class when_identityitem_and_deleting_async_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdAsync<IdentityItem>(_structures[1].StructureId).Wait();
                    session.DeleteByIdAsync<IdentityItem>(_structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItem>(2);

            It should_have_first_and_last_item_left = 
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id (uniques)")]
        public class when_uniqueidentityitem_and_deleting_async_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertUniqueIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueIdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdAsync<UniqueIdentityItem>(_structures[1].StructureId).Wait();
                    session.DeleteByIdAsync<UniqueIdentityItem>(_structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<UniqueIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<UniqueIdentityItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id")]
        public class when_bigidentityitem_and_deleting_async_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertBigIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<BigIdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdAsync<BigIdentityItem>(_structures[1].StructureId).Wait();
                    session.DeleteByIdAsync<BigIdentityItem>(_structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<BigIdentityItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id (uniques)")]
        public class when_uniquebigidentityitem_and_deleting_async_two_of_four_items_using_id : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertUniqueBigIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UniqueBigIdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdAsync<UniqueBigIdentityItem>(_structures[1].StructureId).Wait();
                    session.DeleteByIdAsync<UniqueBigIdentityItem>(_structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<UniqueBigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_uniques_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_uniques_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_uniques_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_uniques_table(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<UniqueBigIdentityItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by id")]
        public class when_guiditem_and_deleting_async_item_by_id_in_empty_set : SpecificationBase
        {
            Establish context = 
                () => TestContext = TestContextFactory.Create();
            
            Because of = () =>
            {
                CaughtException = Catch.Exception(() => 
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        session.DeleteByIdAsync<GuidItem>(Guid.Parse("F875F861-24DC-420C-988A-196977A21C43")).Wait();
                    }
                });
            };

            It should_not_have_failed =
                () => CaughtException.ShouldBeNull();
        }

        [Subject(typeof(ISession), "Delete by id")]
        public class when_stringitem_and_deleting_async_item_by_id_in_empty_set : SpecificationBase
        {
            Establish context =
                () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        session.DeleteByIdAsync<StringItem>("foo").Wait();
                    }
                });
            };

            It should_not_have_failed =
                () => CaughtException.ShouldBeNull();
        }

        [Subject(typeof(ISession), "Delete by id")]
        public class when_identityitem_and_deleting_async_item_by_id_in_empty_set : SpecificationBase
        {
            Establish context =
                () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        session.DeleteByIdAsync<IdentityItem>(1).Wait();
                    }
                });
            };

            It should_not_have_failed =
                () => CaughtException.ShouldBeNull();
        }

        [Subject(typeof(ISession), "Delete by ids")]
        public class when_guiditem_and_deleting_async_two_of_four_items_using_ids : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdsAsync<GuidItem>(_structures[1].StructureId, _structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by ids")]
        public class when_stringitem_and_deleting_async_two_of_four_items_using_ids : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertStringItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<StringItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdsAsync<StringItem>(_structures[1].StructureId, _structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<StringItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<StringItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by ids")]
        public class when_identityitem_and_deleting_async_two_of_four_items_using_ids : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdsAsync<IdentityItem>(_structures[1].StructureId, _structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<IdentityItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by ids")]
        public class when_bigidentityitem_and_deleting_async_two_of_four_items_using_ids : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertBigIdentityItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<BigIdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByIdsAsync<BigIdentityItem>(_structures[1].StructureId, _structures[2].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<BigIdentityItem> _structures;
        }

        [Subject(typeof(ISession), "Delete all Except ids")]
        public class when_four_guiditems_and_deleting_async_all_except_first_and_last_using_generic_version : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteAllExceptIdsAsync<GuidItem>(_structures[0].StructureId, _structures[3].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_items_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete all Except ids")]
        public class when_four_guiditems_and_deleting_async_all_except_first_and_last_using_non_generic_version : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteAllExceptIdsAsync(typeof(GuidItem), _structures[0].StructureId, _structures[3].StructureId).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_items_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete all Except ids")]
        public class when_four_guiditems_and_deleting_async_all_except_first_and_last_using_useonceto_generic_version : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () => TestContext.Database.UseOnceTo()
                .DeleteAllExceptIdsAsync<GuidItem>(_structures[0].StructureId, _structures[3].StructureId).Wait();

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_items_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete all Except ids")]
        public class when_four_guiditems_and_deleting_async_all_except_first_and_last_using_useonceto_non_generic_version : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () => TestContext.Database.UseOnceTo()
                .DeleteAllExceptIdsAsync(typeof(GuidItem), _structures[0].StructureId, _structures[3].StructureId).Wait();

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_items_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by query")]
        public class when_guiditem_and_deleting_async_two_of_four_items_using_query_with_qxin_on_int : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByQueryAsync<GuidItem>(i => i.Value.QxIn(_structures[1].Value, _structures[2].Value)).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by query")]
        public class when_guiditem_and_deleting_async_two_of_four_items_using_query_with_qxin_on_guid : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.DeleteByQueryAsync<GuidItem>(i => i.GuidValue.QxIn(_structures[1].GuidValue, _structures[2].GuidValue)).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }

        [Subject(typeof(ISession), "Delete by query")]
        public class when_guiditem_and_deleting_async_two_of_four_items_using_query_with_qxin_on_guid_using_array : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = TestContext.Database.InsertGuidItems(4);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<GuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var values = new[] { _structures[1].GuidValue, _structures[2].GuidValue };
                    session.DeleteByQueryAsync<GuidItem>(i => i.GuidValue.QxIn(values)).Wait();
                }
            };

            It should_only_have_two_items_left =
                () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);

            It should_have_first_and_last_item_left =
                () => TestContext.Database.should_have_first_and_last_item_left(_structures);

            It should_not_have_deleted_first_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_structures_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_structures_table(_structureSchema, _structures[3].StructureId);

            It should_not_have_deleted_first_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[0].StructureId);

            It should_not_have_deleted_last_item_from_indexes_table =
                () => TestContext.DbHelper.should_not_have_been_deleted_from_indexes_tables(_structureSchema, _structures[3].StructureId);

            It should_have_deleted_second_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_structures_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_structures_table(_structureSchema, _structures[2].StructureId);

            It should_have_deleted_second_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[1].StructureId);

            It should_have_deleted_third_item_from_indexes_table =
                () => TestContext.DbHelper.should_have_been_deleted_from_indexes_tables(_structureSchema, _structures[2].StructureId);

            private static IStructureSchema _structureSchema;
            private static IList<GuidItem> _structures;
        }
    }
}