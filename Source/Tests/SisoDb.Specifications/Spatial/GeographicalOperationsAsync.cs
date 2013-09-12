using System;
using System.Linq;
using Machine.Specifications;
using SisoDb.NCore;
using SisoDb.Spatial;
using SisoDb.Spatial.Resources;
using SisoDb.Specifications.Model;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Spatial
{
#if SqlAzureProvider || Sql2008Provider || Sql2012Provider
    class GeographicalOperationsAsync
    {

        [Subject(typeof(ISisoSpatial), "SetPolygon")]
        public class when_setting_polygon_async_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.DefaultPolygon();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetPolygonAsync<SpatialGuidItem>(_item.StructureId, _coordinates).Wait();
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "InsertPolygon")]
        public class when_inserting_polygon_async_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.DefaultPolygon();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertPolygonAsync<SpatialGuidItem>(_item.StructureId, _coordinates).Wait();
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _coordinates;
        }

      
        [Subject(typeof(ISisoSpatial), "DeleteGeoFor")]
        public class when_deleting_existing_polygon_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());

                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.DeleteGeoForAsync<SpatialGuidItem>(_item.StructureId).Wait();
                }
            };

            It should_have_no_geo_record_left =
                () => TestContext.DbHelper.should_have_been_deleted_from_spatial_table(_structureSchema, _item.StructureId);

            It should_now_return_empty_array_of_coordinates = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.ShouldBeValueEqualTo(new Coordinates[0]);
                }
            };

            private static SpatialGuidItem _item;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "DeleteGeoFor")]
        public class when_deleting_non_existing_polygon_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.DeleteGeoForAsync<SpatialGuidItem>(_item.StructureId).Wait();
                }
            };

            It should_have_no_geo_record_left =
                () => TestContext.DbHelper.should_have_been_deleted_from_spatial_table(_structureSchema, _item.StructureId);

            It should_now_return_empty_array_of_coordinates = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.ShouldBeValueEqualTo(new Coordinates[0]);
                }
            };

            private static SpatialGuidItem _item;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "SetPolygon")]
        public class when_setting_polygon_async_and_one_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                var orgCoordinates = SpatialDataFactory.DefaultPolygon();
                _newCoordinates = SpatialDataFactory.SmallerPolygon();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPolygon<SpatialGuidItem>(_item.StructureId, orgCoordinates);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetPolygonAsync<SpatialGuidItem>(_item.StructureId, _newCoordinates).Wait();
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _newCoordinates;
        }

        [Subject(typeof(ISisoSpatial), "UpdatePolygon")]
        public class when_updating_an_existing_polygon_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                var orgCoordinates = SpatialDataFactory.DefaultPolygon();
                _newCoordinates = SpatialDataFactory.SmallerPolygon();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPolygon<SpatialGuidItem>(_item.StructureId, orgCoordinates);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.UpdatePolygonAsync<SpatialGuidItem>(_item.StructureId, _newCoordinates).Wait();
                }
            };

            It should_have_stored_a_new_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _newCoordinates;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPoint")]
        public class when_checking_if_polygon_contains_point_async_that_is_within_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPointAsync<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.PointWithinDefaultPolygon).Result;
                }
            };

            It should_be_contained =
                () => _contains.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPoint")]
        public class when_checking_if_polygon_contains_point_async_that_is_outside_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPointAsync<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.PointOutsideDefaultPolygon).Result;
                }
            };

            It should_not_be_contained =
                () => _contains.ShouldBeFalse();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPointAfterExpand")]
        public class when_checking_if_a_expanded_polygon_async_contains_point_that_normally_is_outside_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPointAfterExpandAsync<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.PointOutsideDefaultPolygon, 4000d).Result;
                }
            };

            It should_be_contained_after_expanding =
                () => _contains.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "SetPoint")]
        public class when_setting_point_async_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.PointWithinDefaultPolygon;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetPointAsync<SpatialGuidItem>(_item.StructureId, _coordinates).Wait();
                }
            };

            It should_have_inserted_one_record_with_the_point = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.Single().ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "InsertPoint")]
        public class when_inserting_point_async_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.PointWithinDefaultPolygon;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertPointAsync<SpatialGuidItem>(_item.StructureId, _coordinates).Wait();
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.Single().ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "SetPoint")]
        public class when_setting_point_async_and_one_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPoint<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Point1);
                }
                _newCoordinates = SpatialDataFactory.Point2;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetPointAsync<SpatialGuidItem>(_item.StructureId, _newCoordinates).Wait();
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.Single().ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _newCoordinates;
        }

        [Subject(typeof(ISisoSpatial), "UpdatePoint")]
        public class when_updating_an_existing_point_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPoint<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Point1);
                }
                _newCoordinates = SpatialDataFactory.Point2;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.UpdatePointAsync<SpatialGuidItem>(_item.StructureId, _newCoordinates).Wait();
                }
            };

            It should_have_stored_a_new_point = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.Single().ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _newCoordinates;
        }

        [Subject(typeof(ISisoSpatial), "SetCircle")]
        public class when_setting_circle_async_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.Circle1;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetCircleAsync<SpatialGuidItem>(_item.StructureId, _coordinates, 10d).Wait();
                }
            };

            It should_have_inserted_one_record_as_polygon_with_points_forming_circle = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    var c = s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result;
                    c.Length.ShouldEqual(129);
                    c[0].Latitude.ShouldEqual(47.653063500926457d);
                    c[0].Longitude.ShouldEqual(-122.35790573151685d);
                    c[60].Latitude.ShouldEqual(47.652950145234975d);
                    c[60].Longitude.ShouldEqual(-122.35811079200715d);
                    c[128].Latitude.ShouldEqual(47.653063500926457d);
                    c[128].Longitude.ShouldEqual(-122.35790573151685d);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "InsertCircle")]
        public class when_inserting_circle_async_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.Circle1;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertCircleAsync<SpatialGuidItem>(_item.StructureId, _coordinates, 10d).Wait();
                }
            };

            It should_have_inserted_one_record_as_polygon_with_points_forming_circle = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    var c = s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result;
                    c.Length.ShouldEqual(129);
                    c[0].Latitude.ShouldEqual(47.653063500926457d);
                    c[0].Longitude.ShouldEqual(-122.35790573151685d);
                    c[60].Latitude.ShouldEqual(47.652950145234975d);
                    c[60].Longitude.ShouldEqual(-122.35811079200715d);
                    c[128].Latitude.ShouldEqual(47.653063500926457d);
                    c[128].Longitude.ShouldEqual(-122.35790573151685d);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "SetCircle")]
        public class when_setting_circle_async_and_one_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 10d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetCircleAsync<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d).Wait();
                }
            };

            It should_have_inserted_one_record_as_polygon_with_points_forming_circle = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    var c = s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result;
                    c.Length.ShouldEqual(129);
                    c[0].Latitude.ShouldEqual(47.653031750472969d);
                    c[0].Longitude.ShouldEqual(-122.35795286578694d);
                    c[60].Latitude.ShouldEqual(47.652975072630916d);
                    c[60].Longitude.ShouldEqual(-122.35805539602994d);
                    c[128].Latitude.ShouldEqual(47.653031750472969d);
                    c[128].Longitude.ShouldEqual(-122.35795286578694d);
                }
            };

            private static SpatialGuidItem _item;
        }

        [Subject(typeof(ISisoSpatial), "UpdateCircle")]
        public class when_updating_an_existing_circle_async : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.UpdateCircleAsync<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 10d).Wait();
                }
            };

            It should_have_stored_a_new_circle = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    var c = s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result;
                    c.Length.ShouldEqual(129);
                    c[0].Latitude.ShouldEqual(47.653063500926457d);
                    c[0].Longitude.ShouldEqual(-122.35790573151685d);
                    c[60].Latitude.ShouldEqual(47.652950145234975d);
                    c[60].Longitude.ShouldEqual(-122.35811079200715d);
                    c[128].Latitude.ShouldEqual(47.653063500926457d);
                    c[128].Longitude.ShouldEqual(-122.35790573151685d);
                }
            };

            private static SpatialGuidItem _item;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPoint")]
        public class when_checking_if_circle_contains_point_async_that_is_within_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPointAsync<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1).Result;
                }
            };

            It should_be_contained =
                () => _contains.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPoint")]
        public class when_checking_if_circle_contains_point_async_that_is_outside_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPointAsync<SpatialGuidItem>(_item.StructureId, new Coordinates { Latitude = SpatialDataFactory.Circle1.Latitude + 0.05 }).Result;
                }
            };

            It should_not_be_contained =
                () => _contains.ShouldBeFalse();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPointAfterExpand")]
        public class when_checking_if_a_expanded_circle_contains_point_async_that_normally_is_outside_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPointAfterExpandAsync<SpatialGuidItem>(_item.StructureId, new Coordinates
                    {
                        Latitude = SpatialDataFactory.Circle1.Latitude + 0.05,
                        Longitude = SpatialDataFactory.Circle1.Longitude
                    }, 5600d).Result;
                }
            };

            It should_be_contained =
                () => _contains.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "MakeValid")]
        public class when_makevalid_async_on_correctly_inserted_polygon : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.MakeValidAsync<SpatialGuidItem>(_item.StructureId).Wait();
                }
            };

            It should_still_have_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesInAsync<SpatialGuidItem>(_item.StructureId).Result.ShouldBeValueEqualTo(SpatialDataFactory.DefaultPolygon());
                }
            };

            private static SpatialGuidItem _item;
        }

    [Subject(typeof(ISisoSpatial), "InsertPolygon")]
    public class when_trying_to_insert_invalid_polygon_async : SpecificationBase
    {
        Establish context = () =>
        {
            TestContext = TestContextFactory.Create();
            using (var session = TestContext.Database.BeginSession())
            {
                _item = new SpatialGuidItem();
                session.Insert(_item);

                var s = session.Spatial();
                s.EnableFor<SpatialGuidItem>();
            }
        };

        Because of = () =>
        {
            CaughtException = Catch.Exception(() =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertPolygonAsync<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.InvalidPolygonCauseOfMultiPolygon()).Wait();
                }
            });
        };

        It should_have_caused_an_exception = () =>
        {
            CaughtException.ShouldNotBeNull();
            CaughtException.ShouldBeOfType<AggregateException>();
            CaughtException.InnerException.ShouldBeOfType<SisoDbException>();
            CaughtException.InnerException.Message.ShouldStartWith(ExceptionMessages.NotAValidPolygon.Inject("24409: Not valid because some portion of polygon ring (1) lies in the interior of a polygon."));
        };

        It should_not_have_inserted_the_polygon = () =>
        {
            using (var session = TestContext.Database.BeginSession())
            {
                var s = session.Spatial();
                s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeEmpty();
            }
        };

        private static SpatialGuidItem _item;
    }
    
#endif
}
