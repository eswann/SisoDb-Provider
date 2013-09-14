﻿using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class GetIdsAsync
    {
        [Subject(typeof(ISession), "GetIds (guid) Async")]
        public class when_set_with_guid_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.CreateAsync();

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryGuidItem, Guid>(i => i.IntegerValue == 42).Result;

            It should_not_fetch_any_ids =
                () => _fetchedIds.ShouldBeEmpty();

            private static Guid[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (identity)")]
        public class when_set_with_identity_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.CreateAsync();

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryIdentityItem, int>(i => i.IntegerValue == 42).Result;

            It should_not_fetch_any_ids =
                () => _fetchedIds.ShouldBeEmpty();

            private static int[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (big identity)")]
        public class when_set_with_bigidentity_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.CreateAsync();

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryBigIdentityItem, long>(i => i.IntegerValue == 42).Result;

            It should_not_fetch_any_ids =
                () => _fetchedIds.ShouldBeEmpty();

            private static long[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (string)")]
        public class when_set_with_string_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.CreateAsync();

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryStringItem, string>(i => i.IntegerValue == 42).Result;

            It should_not_fetch_any_ids =
                () => _fetchedIds.ShouldBeEmpty();

            private static string[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (guid)")]
        public class when_query_matches_two_guid_items_in_set_of_four_returned_as_object : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryGuidItem>(i => i.IntegerValue.QxIn(_structures[1].IntegerValue, _structures[2].IntegerValue)).Result;

            It should_have_fetched_two_ids =
                () => _fetchedIds.Length.ShouldEqual(2);

            It should_have_fetched_the_correct_ids = () =>
            {
                _fetchedIds.ShouldContain(_structures[1].StructureId);
                _fetchedIds.ShouldContain(_structures[2].StructureId);
            };

            private static IList<QueryGuidItem> _structures;
            private static object[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (identity)")]
        public class when_query_matches_two_identity_items_in_set_of_four : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryIdentityItem, int>(i => i.IntegerValue.QxIn(_structures[1].IntegerValue, _structures[2].IntegerValue)).Result;

            It should_have_fetched_two_ids =
                () => _fetchedIds.Length.ShouldEqual(2);

            It should_have_fetched_the_correct_ids = () =>
            {
                _fetchedIds.ShouldContain(_structures[1].StructureId);
                _fetchedIds.ShouldContain(_structures[2].StructureId);
            };

            private static IList<QueryIdentityItem> _structures;
            private static int[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (identity)")]
        public class when_query_matches_two_identity_items_in_set_of_four_returned_as_object : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryIdentityItem>(i => i.IntegerValue.QxIn(_structures[1].IntegerValue, _structures[2].IntegerValue)).Result;

            It should_have_fetched_two_ids =
                () => _fetchedIds.Length.ShouldEqual(2);

            It should_have_fetched_the_correct_ids = () =>
            {
                _fetchedIds.ShouldContain(_structures[1].StructureId);
                _fetchedIds.ShouldContain(_structures[2].StructureId);
            };

            private static IList<QueryIdentityItem> _structures;
            private static object[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (bigidentity)")]
        public class when_query_matches_two_bigidentity_items_in_set_of_four : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryBigIdentityItem, long>(i => i.IntegerValue.QxIn(_structures[1].IntegerValue, _structures[2].IntegerValue)).Result;

            It should_have_fetched_two_ids =
                () => _fetchedIds.Length.ShouldEqual(2);

            It should_have_fetched_the_correct_ids = () =>
            {
                _fetchedIds.ShouldContain(_structures[1].StructureId);
                _fetchedIds.ShouldContain(_structures[2].StructureId);
            };

            private static IList<QueryBigIdentityItem> _structures;
            private static long[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (bigidentity)")]
        public class when_query_matches_two_bigidentity_items_in_set_of_four_returned_as_object : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryBigIdentityItem>(i => i.IntegerValue.QxIn(_structures[1].IntegerValue, _structures[2].IntegerValue)).Result;

            It should_have_fetched_two_ids =
                () => _fetchedIds.Length.ShouldEqual(2);

            It should_have_fetched_the_correct_ids = () =>
            {
                _fetchedIds.ShouldContain(_structures[1].StructureId);
                _fetchedIds.ShouldContain(_structures[2].StructureId);
            };

            private static IList<QueryBigIdentityItem> _structures;
            private static object[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (string)")]
        public class when_query_matches_two_string_items_in_set_of_four : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryStringItem, string>(i => i.IntegerValue.QxIn(_structures[1].IntegerValue, _structures[2].IntegerValue)).Result;

            It should_have_fetched_two_ids =
                () => _fetchedIds.Length.ShouldEqual(2);

            It should_have_fetched_the_correct_ids = () =>
            {
                _fetchedIds.ShouldContain(_structures[1].StructureId);
                _fetchedIds.ShouldContain(_structures[2].StructureId);
            };

            private static IList<QueryStringItem> _structures;
            private static string[] _fetchedIds;
        }

        [Subject(typeof(ISession), "GetIds (string)")]
        public class when_query_matches_two_string_items_in_set_of_four_returned_as_object : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
                _fetchedIds = TestContext.Database.UseOnceTo().GetIdsAsync<QueryStringItem>(i => i.IntegerValue.QxIn(_structures[1].IntegerValue, _structures[2].IntegerValue)).Result;

            It should_have_fetched_two_ids =
                () => _fetchedIds.Length.ShouldEqual(2);

            It should_have_fetched_the_correct_ids = () =>
            {
                _fetchedIds.ShouldContain(_structures[1].StructureId);
                _fetchedIds.ShouldContain(_structures[2].StructureId);
            };

            private static IList<QueryStringItem> _structures;
            private static object[] _fetchedIds;
        }
    }
}