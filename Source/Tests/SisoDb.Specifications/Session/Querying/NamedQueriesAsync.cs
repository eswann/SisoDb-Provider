﻿using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
#if SqlAzureProvider || Sql2005Provider || Sql2008Provider || Sql2012Provider || SqlProfilerProvider
    class NamedQueriesAsync
    {
        [Subject(typeof(ISession), "Named Query Async Async")]
        public class when_named_query_async_async_returns_no_result : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                TestContext.DbHelper.DropProcedure(ProcedureName);
                TestContext.DbHelper.CreateProcedure(@"create procedure [" + ProcedureName + "] as begin select '{}' as Json where 1=2; end");
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of = () =>
            {
                var query = new NamedQuery(ProcedureName);
                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructures = session.Advanced.NamedQueryAsync<QueryGuidItem>(query).Result.ToList();
                }
            };

            It should_have_fetched_0_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private const string ProcedureName = "NamedQueryReturningNullTestAsync";
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Named Query as Json")]
        public class when_named_query_async_returns_no_json_result : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                TestContext.DbHelper.CreateProcedure(@"create procedure [" + ProcedureName + "] as begin select '{}' as Json where 1=2; end");
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of = () =>
            {
                var query = new NamedQuery(ProcedureName);
                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructures = session.Advanced.NamedQueryAsJsonAsync<QueryGuidItem>(query).Result.ToList();
                }
            };

            It should_have_fetched_0_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private const string ProcedureName = "NamedQueryAsJsonReturningNullTestAsync";
            private static IList<string> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Named Query")]
        public class when_named_query_async_with_parameters_returning_entities_is_invoked_via_manual_named_query_async : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                using(var session = TestContext.Database.BeginSession())
                {
                    session.Advanced.UpsertNamedQuery<QueryGuidItem>(
                        ProcedureName, 
                        qb => qb.Where(x => x.SortOrder >= 0 && x.SortOrder <= 0));
                }
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of = () =>
            {
                var query = new NamedQuery(ProcedureName);
                query.Add(
                    new DacParameter("p0", _structures[1].SortOrder), 
                    new DacParameter("p1", _structures[2].SortOrder));

                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructures = session.Advanced.NamedQueryAsync<QueryGuidItem>(query).Result.ToList();
                }
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private const string ProcedureName = "NamedQueryTestAsync";
            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Named Query as Json")]
        public class when_named_query_async_with_parameters_returning_json_is_invoked_via_manual_named_query_async : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                using (var session = TestContext.Database.BeginSession())
                {
                    session.Advanced.UpsertNamedQuery<QueryGuidItem>(
                        ProcedureName,
                        qb => qb.Where(x => x.SortOrder >= 0 && x.SortOrder <= 0));
                }
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of = () =>
            {
                var query = new NamedQuery(ProcedureName);
                query.Add(
                    new DacParameter("p0", _structures[1].SortOrder),
                    new DacParameter("p1", _structures[2].SortOrder));

                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructures = session.Advanced.NamedQueryAsJsonAsync<QueryGuidItem>(query).Result.ToList();
                }
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private const string ProcedureName = "NamedQueryAsJsonTestAsync";
            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
		}

        [Subject(typeof(ISession), "Named Query")]
        public class when_named_query_async_with_parameters_returning_entities_is_invoked_via_expression : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                using (var session = TestContext.Database.BeginSession())
                {
                    session.Advanced.UpsertNamedQuery<QueryGuidItem>(ProcedureName, qb => qb.Where(x => x.SortOrder >= 0 && x.SortOrder <= 0));
                }
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of = () =>
            {
                var minSortOrder = _structures[1].SortOrder;
                var maxSortOrder = _structures[2].SortOrder;

                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructures = session.Advanced.NamedQueryAsync<QueryGuidItem>(
                        ProcedureName, 
                        x => x.SortOrder >= minSortOrder && x.SortOrder <= maxSortOrder).Result.ToList();
                }
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
                _fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
            };

            private const string ProcedureName = "NamedQueryTestAsync";
            private static IList<QueryGuidItem> _structures;
            private static IList<QueryGuidItem> _fetchedStructures;
        }

        [Subject(typeof(ISession), "Named Query as Json")]
        public class when_named_query_async_with_parameters_returning_json_is_invoked_via_expression : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
                using (var session = TestContext.Database.BeginSession())
                {
                    session.Advanced.UpsertNamedQuery<QueryGuidItem>(
                        ProcedureName,
                        qb => qb.Where(x => x.SortOrder >= 0 && x.SortOrder <= 0));
                }
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of = () =>
            {
                var minSortOrder = _structures[1].SortOrder;
                var maxSortOrder = _structures[2].SortOrder;

                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructures = session.Advanced.NamedQueryAsJsonAsync<QueryGuidItem>(
                        ProcedureName,
                        x => x.SortOrder >= minSortOrder && x.SortOrder <= maxSortOrder).Result.ToList();
                }
            };

            It should_have_fetched_two_structures =
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_the_two_middle_structures = () =>
            {
                _fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
                _fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
            };

            private const string ProcedureName = "NamedQueryAsJsonTestAsync";
            private static IList<QueryGuidItem> _structures;
            private static IList<string> _fetchedStructures;
        }
	}
#endif
}