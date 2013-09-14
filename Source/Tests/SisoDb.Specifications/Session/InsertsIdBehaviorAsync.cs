﻿using System;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.Session
{
	class InsertsIdBehaviorAsync
    {
        [Subject(typeof(ISession), "Insert (id behavior) Async")]
        public class when_guiditem_with_private_unassigned_id_setter_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new GuidItemWithPrivateIdSetter();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<GuidItemWithPrivateIdSetter>(1);

            It should_have_assigned_id_of_the_structure_in_memory = 
                () => _structure.StructureId.ShouldNotEqual(Guid.Empty);

            It should_have_assigned_id_of_the_structures_in_database = 
                () => TestContext.Database.should_have_valid_structures<GuidItemWithPrivateIdSetter>(s => s.StructureId.ShouldNotEqual(Guid.Empty));

            private static GuidItemWithPrivateIdSetter _structure;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_guiditem_with_private_assigned_id_setter_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new GuidItemWithPrivateIdSetter(Guid.Parse("702BDF42-47E9-42E9-9132-C11CC0896458"));
                _orgId = _structure.StructureId;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<GuidItemWithPrivateIdSetter>(1);

            It should_have_assigned_new_id_of_the_structure_in_memory =
                () => _structure.StructureId.ShouldNotEqual(_orgId);

            It should_have_assigned_new_id_of_the_structure_in_database =
                () => TestContext.Database.should_have_valid_structures<GuidItemWithPrivateIdSetter>(s => s.StructureId.ShouldNotEqual(_orgId));

            private static GuidItemWithPrivateIdSetter _structure;
            private static Guid _orgId;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_guiditem_with_unassigned_nullable_id_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new GuidItemWithNullableId();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<GuidItemWithNullableId>(1);

            It should_have_assigned_id_of_the_structure_in_memory = () =>
            {
                _structure.StructureId.ShouldNotBeNull();
                _structure.StructureId.ShouldNotEqual(Guid.Empty);
            };

            It should_have_assigned_id_of_the_structures_in_database = () =>
                TestContext.Database.should_have_valid_structures<GuidItemWithNullableId>(s =>
                {
                    s.StructureId.ShouldNotBeNull();
                    s.StructureId.ShouldNotEqual(Guid.Empty);
                });

            private static GuidItemWithNullableId _structure;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_guiditem_with_assigned_nullable_id_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new GuidItemWithNullableId { StructureId = Guid.Parse("702BDF42-47E9-42E9-9132-C11CC0896458") };
                _orgId = _structure.StructureId;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<GuidItemWithNullableId>(1);

            It should_have_assigned_new_id_of_the_structure_in_memory =
                () => _structure.StructureId.ShouldNotEqual(_orgId);

            It should_have_assigned_new_id_of_the_structure_in_database =
                () => TestContext.Database.should_have_valid_structures<GuidItemWithNullableId>(s => s.StructureId.ShouldNotEqual(_orgId));

            private static GuidItemWithNullableId _structure;
            private static Guid? _orgId;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_identityitem_with_private_unassigned_id_setter_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new IdentityItemWithPrivateIdSetter();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItemWithPrivateIdSetter>(1);

            It should_have_assigned_id_of_the_structure_in_memory =
                () => _structure.StructureId.ShouldNotEqual(0);

            It should_have_assigned_id_of_the_structures_in_database =
                () => TestContext.Database.should_have_valid_structures<IdentityItemWithPrivateIdSetter>(s => s.StructureId.ShouldNotEqual(0));

            private static IdentityItemWithPrivateIdSetter _structure;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_identityitem_with_private_assigned_id_setter_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new IdentityItemWithPrivateIdSetter(42);
                _orgId = _structure.StructureId;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItemWithPrivateIdSetter>(1);

            It should_have_assigned_new_id_of_the_structure_in_memory =
                () => _structure.StructureId.ShouldNotEqual(_orgId);

            It should_have_assigned_new_id_of_the_structure_in_database =
                () => TestContext.Database.should_have_valid_structures<IdentityItemWithPrivateIdSetter>(s => s.StructureId.ShouldNotEqual(_orgId));

            private static IdentityItemWithPrivateIdSetter _structure;
            private static int _orgId;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_identityitem_with_unassigned_nullable_id_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new IdentityItemWithNullableId();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItemWithNullableId>(1);

            It should_have_assigned_id_of_the_structure_in_memory = () =>
            {
                _structure.StructureId.ShouldNotBeNull();
                _structure.StructureId.ShouldNotEqual(0);
            };

            It should_have_assigned_id_of_the_structures_in_database = () =>
                TestContext.Database.should_have_valid_structures<IdentityItemWithNullableId>(s =>
                {
                    s.StructureId.ShouldNotBeNull();
                    s.StructureId.ShouldNotEqual(0);
                });

            private static IdentityItemWithNullableId _structure;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_identityitem_with_assigned_nullable_id_Async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new IdentityItemWithNullableId { StructureId = 42 };
                _orgId = _structure.StructureId;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<IdentityItemWithNullableId>(1);

            It should_have_assigned_new_id_of_the_structure_in_memory =
                () => _structure.StructureId.ShouldNotEqual(_orgId);

            It should_have_assigned_new_id_of_the_structure_in_database =
                () => TestContext.Database.should_have_valid_structures<IdentityItemWithNullableId>(s => s.StructureId.ShouldNotEqual(_orgId));

            private static IdentityItemWithNullableId _structure;
            private static int? _orgId;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_bigidentityitem_with_private_unassigned_id_setter_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new BigIdentityItemWithPrivateIdSetter();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItemWithPrivateIdSetter>(1);

            It should_have_assigned_id_of_the_structure_in_memory =
                () => _structure.StructureId.ShouldNotEqual(0);

            It should_have_assigned_id_of_the_structures_in_database =
                () => TestContext.Database.should_have_valid_structures<BigIdentityItemWithPrivateIdSetter>(s => s.StructureId.ShouldNotEqual(0));

            private static BigIdentityItemWithPrivateIdSetter _structure;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_bigidentityitem_with_private_assigned_id_setter_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new BigIdentityItemWithPrivateIdSetter(42);
                _orgId = _structure.StructureId;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItemWithPrivateIdSetter>(1);

            It should_have_assigned_new_id_of_the_structure_in_memory =
                () => _structure.StructureId.ShouldNotEqual(_orgId);

            It should_have_assigned_new_id_of_the_structure_in_database =
                () => TestContext.Database.should_have_valid_structures<BigIdentityItemWithPrivateIdSetter>(s => s.StructureId.ShouldNotEqual(_orgId));

            private static BigIdentityItemWithPrivateIdSetter _structure;
            private static long _orgId;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_bigidentityitem_with_unassigned_nullable_id_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new BigIdentityItemWithNullableId();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItemWithNullableId>(1);

            It should_have_assigned_id_of_the_structure_in_memory = () =>
            {
                _structure.StructureId.ShouldNotBeNull();
                _structure.StructureId.ShouldNotEqual(0);
            };

            It should_have_assigned_id_of_the_structures_in_database = () =>
                TestContext.Database.should_have_valid_structures<BigIdentityItemWithNullableId>(s =>
                {
                    s.StructureId.ShouldNotBeNull();
                    s.StructureId.ShouldNotEqual(0);
                });

            private static BigIdentityItemWithNullableId _structure;
        }

        [Subject(typeof(ISession), "Insert (id behavior)")]
        public class when_bigidentityitem_with_assigned_nullable_id_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new BigIdentityItemWithNullableId { StructureId = 42 };
                _orgId = _structure.StructureId;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertAsync(_structure).Wait();
                }
            };

            It should_have_inserted_item =
                () => TestContext.Database.should_have_X_num_of_items<BigIdentityItemWithNullableId>(1);

            It should_have_assigned_new_id_of_the_structure_in_memory =
                () => _structure.StructureId.ShouldNotEqual(_orgId);

            It should_have_assigned_new_id_of_the_structure_in_database =
                () => TestContext.Database.should_have_valid_structures<BigIdentityItemWithNullableId>(s => s.StructureId.ShouldNotEqual(_orgId));

            private static BigIdentityItemWithNullableId _structure;
            private static long? _orgId;
        }
    }
}