using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class GetByQueryAsync
    {
        [Subject(typeof(ISession), "GetByQuery Async")]
        public class when_no_structures_are_present_async : SpecificationBase
        {
            Establish context = 
                () => TestContext = TestContextFactory.CreateAsync();

            Because of = 
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetByQueryAsync<QueryGuidItem>(i => i.IntegerValue == 42).Result;

            It should_return_a_null_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "GetByQuery")]
        public class when_query_matches_second_structure_in_set_of_four_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetByQueryAsync<QueryGuidItem>(i => i.IntegerValue == _structures[1].IntegerValue).Result;

            It should_have_fetched_one_struture =
                () => _fetchedStructure.ShouldNotBeNull();

            It should_fetch_the_second_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);
            
            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }
    }
}