﻿using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Resources;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session
{
    class InsertBigStructuresAsync
    {
        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_async_batch_of_items_with_single_string_index_of_4001_characters : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structures = new[]
                {
                    new Model { LongText = new string('a', 4001) }, 
                    new Model { LongText = new string('b', 4001) }, 
                    new Model { LongText = new string('c', 4001) }
                };
            };

#if SqlAzureProvider || Sql2005Provider || Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            Because of = () => 
                TestContext.Database.UseOnceTo().InsertManyAsync(_structures).Wait();

            It should_have_inserted_items = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Model>().ToArray();
                refetched.Length.ShouldEqual(3);

                refetched[0].LongText.ShouldEqual(new string('a', 4001));
                refetched[1].LongText.ShouldEqual(new string('b', 4001));
                refetched[2].LongText.ShouldEqual(new string('c', 4001));
            };
#endif

#if SqlCe4Provider
            Because of = () =>
                CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().InsertManyAsync(_structures).Wait());

            It should_have_thrown_an_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldStartWith("String truncation: max=4000");
            };
#endif
            private static Model[] _structures;
        }

        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_async_single_item_with_single_string_index_of_4001_characters : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
            };

#if SqlAzureProvider || Sql2005Provider || Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            Because of = () =>
                TestContext.Database.UseOnceTo().InsertAsync(new Model { LongText = new string('a', 4001) }).Wait();

            It should_have_one_item_inserted = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Model>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.LongText.ShouldEqual(new string('a', 4001));
            };
#endif

#if SqlCe4Provider
            Because of = () =>
                CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().InsertAsync(new Model { LongText = new string('a', 4001) }).Wait());

            It should_have_thrown_an_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldEqual(ExceptionMessages.SqlCe4_ToLongIndividualStringValue);
            };
#endif
        }

        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_async_structure_with_big_guid_collection_causing_json_to_be_longer_than_4000_chars : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateAsync();
                _structure = new ModelFoo { Guids = new List<Guid>() };

                for (var c = 0; c < 1000; c++)
                    _structure.Guids.Add(Guid.NewGuid());
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().InsertAsync(_structure).Wait();

            It should_have_inserted_the_structure = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<ModelFoo>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Guids.ShouldEqual(_structure.Guids);
            };

            private static ModelFoo _structure;
        }

        private class Model
        {
            public Guid Id { get; set; }
            public string LongText { get; set; }
        }

        private class ModelFoo
        {
            public Guid Id { get; set; }
            public List<Guid> Guids { get; set; }
        }
    }
}