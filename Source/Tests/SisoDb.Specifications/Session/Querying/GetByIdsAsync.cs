using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class GetByIdsAsync
    {
        [Subject(typeof(ISession), "Get by Ids (guids)")]
        public class when_guid_id_array_matches_two_of_four_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = Guid.Parse("81EC4983-F58B-4459-84F8-0D000F06F43D");
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsync<QueryGuidItem>(new[] { _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId }).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static Guid _nonMatchingId;
            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (guids)")]
        public class when_guid_id_array_matches_two_of_four_items_using_non_generic_api_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = Guid.Parse("81EC4983-F58B-4459-84F8-0D000F06F43D");
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .GetByIdsAsync(typeof(QueryGuidItem), new[] { _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId }).Result
                .Cast<QueryGuidItem>()
                .ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static Guid _nonMatchingId;
            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (guids)")]
        public class when_guid_id_set_matches_two_of_four_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = Guid.Parse("81EC4983-F58B-4459-84F8-0D000F06F43D");
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsync<QueryGuidItem>(_nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static Guid _nonMatchingId;
            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (guids)")]
        public class when_guid_id_set_matches_two_of_four_items_using_non_generic_api_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = Guid.Parse("81EC4983-F58B-4459-84F8-0D000F06F43D");
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .GetByIdsAsync(typeof(QueryGuidItem), _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result
                .Cast<QueryGuidItem>()
                .ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static Guid _nonMatchingId;
            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (strings)")]
        public class when_string_id_array_matches_two_of_four_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = "FooBar";
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsync<QueryStringItem>(new[] { _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId }).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static string _nonMatchingId;
            private static IList<QueryStringItem> _structures;
            private static IList<QueryStringItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (strings)")]
        public class when_string_id_array_matches_two_of_four_items_using_non_generic_api_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = "FooBar";
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .GetByIdsAsync(typeof(QueryStringItem), new[] { _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId }).Result
                .Cast<QueryStringItem>()
                .ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static string _nonMatchingId;
            private static IList<QueryStringItem> _structures;
            private static IList<QueryStringItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (strings)")]
        public class when_string_id_set_matches_two_of_four_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = "FooBar";
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsync<QueryStringItem>(_nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static string _nonMatchingId;
            private static IList<QueryStringItem> _structures;
            private static IList<QueryStringItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (strings)")]
        public class when_string_id_set_matches_two_of_four_items_using_non_generic_api_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = "FooBar";
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .GetByIdsAsync(typeof(QueryStringItem), _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result
                .Cast<QueryStringItem>()
                .ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static string _nonMatchingId;
            private static IList<QueryStringItem> _structures;
            private static IList<QueryStringItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (identity)")]
        public class when_identity_id_array_matches_two_of_four_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = 9999;
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsync<QueryIdentityItem>(new[] { _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId }).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static int _nonMatchingId;
            private static IList<QueryIdentityItem> _structures;
            private static IList<QueryIdentityItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (identity)")]
        public class when_identity_id_array_matches_two_of_four_items_using_non_generic_api_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = 9999;
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .GetByIdsAsync(typeof(QueryIdentityItem), new[] { _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId }).Result
                .Cast<QueryIdentityItem>()
                .ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static int _nonMatchingId;
            private static IList<QueryIdentityItem> _structures;
            private static IList<QueryIdentityItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (identity)")]
        public class when_identity_id_set_matches_two_of_four_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = 9999;
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsync<QueryIdentityItem>(_nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static int _nonMatchingId;
            private static IList<QueryIdentityItem> _structures;
            private static IList<QueryIdentityItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (identity)")]
        public class when_identity_id_set_matches_two_of_four_items_using_non_generic_api_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = 9999;
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .GetByIdsAsync(typeof(QueryIdentityItem), _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result
                .Cast<QueryIdentityItem>()
                .ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static int _nonMatchingId;
            private static IList<QueryIdentityItem> _structures;
            private static IList<QueryIdentityItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (big identity)")]
        public class when_big_identity_id_array_matches_two_of_four_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = 9999;
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsync<QueryBigIdentityItem>(new[] { _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId }).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static long _nonMatchingId;
            private static IList<QueryBigIdentityItem> _structures;
            private static IList<QueryBigIdentityItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (big identity)")]
        public class when_big_identity_id_array_matches_two_of_four_items_using_non_generic_api_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = 9999;
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .GetByIdsAsync(typeof(QueryBigIdentityItem), new[] { _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId }).Result
                .Cast<QueryBigIdentityItem>()
                .ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static long _nonMatchingId;
            private static IList<QueryBigIdentityItem> _structures;
            private static IList<QueryBigIdentityItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (big identity)")]
        public class when_big_identity_id_set_matches_two_of_four_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = 9999;
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsync<QueryBigIdentityItem>(_nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static long _nonMatchingId;
            private static IList<QueryBigIdentityItem> _structures;
            private static IList<QueryBigIdentityItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (big identity)")]
        public class when_big_identity_id_set_matches_two_of_four_items_using_non_generic_api_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = 9999;
            };

            Because of = () => _fetchedStructures = TestContext.Database.UseOnceTo()
                .GetByIdsAsync(typeof(QueryBigIdentityItem), _nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result
                .Cast<QueryBigIdentityItem>()
                .ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static long _nonMatchingId;
            private static IList<QueryBigIdentityItem> _structures;
            private static IList<QueryBigIdentityItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids as Json (guids)")]
        public class when_guid_id_set_matches_two_of_four_json_items_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                _nonMatchingId = Guid.Parse("81EC4983-F58B-4459-84F8-0D000F06F43D");
            };

            Because of = () =>
                _fetchedStructures = TestContext.Database.UseOnceTo().GetByIdsAsJsonAsync<QueryGuidItem>(_nonMatchingId, _structures[1].StructureId, _structures[2].StructureId).Result.ToList();

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static Guid _nonMatchingId;
            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids (guids)")]
        public class when_ids_matches_two_of_four_items_that_is_are_uncommitted_mode_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertMany(_structures);

                    _fetchedStructures = session.GetByIdsAsync<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).Result.ToList();
                }
            };

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Get by Ids as Json (guids)")]
        public class when_ids_matches_two_of_four_json_items_that_are_in_uncommitted_mode_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertMany(_structures);

                    _fetchedStructures = session.GetByIdsAsJsonAsync<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).Result.ToList();
                }
            };

            It should_fetch_2_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_fetch_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }
    }
}