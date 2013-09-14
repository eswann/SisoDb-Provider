using System;
using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session
{
    class InsertAsAsync
    {
        [Subject(typeof(ISession), "InsertAwhen anonymous Async")]
        public class when_inserting_async_anonymous_that_partially_conforms_to_contract_using_type : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of = () =>
            {
                var item = new { IntValue = 42 };
                TestContext.Database.UseOnceTo().InsertAsAsync(typeof(Model), item).Wait();
            };

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_have_inserted_an_item_with_partial_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Model>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.IntValue.ShouldEqual(42);
                refetched.StringValue.ShouldEqual(default(string));
            };
        }

        [Subject(typeof(ISession), "InsertAs (when anonymous")]
        public class when_inserting_async_anonymous_that_partially_conforms_to_contract : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of = () =>
            {
                var item = new { IntValue = 42 };
                TestContext.Database.UseOnceTo().InsertAsAsync<Model>(item).Wait();
            };

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_have_inserted_an_item_with_partial_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Model>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.IntValue.ShouldEqual(42);
                refetched.StringValue.ShouldEqual(default(string));
            };
        }

        [Subject(typeof(ISession), "InsertAs (when other type")]
        public class when_inserting_async_type_that_partially_conforms_to_contract : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of = () =>
            {
                var item = new Foo { StringValue = "Bar" };
                TestContext.Database.UseOnceTo().InsertAsAsync<Model>(item).Wait();
            };

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_have_inserted_an_item_with_partial_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Model>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.IntValue.ShouldEqual(default(int));
                refetched.StringValue.ShouldEqual("Bar");
            };
        }

        private class Foo
        {
            public string StringValue { get; set; }
        }

        private class Model
        {
            public Guid StructureId { get; set; }

            public string StringValue { get; set; }

            public int IntValue { get; set; }
        }
    }
}