//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Osm.Routing.Core.Resolving;
//using OsmSharp.Osm.Routing.Core;
//using OsmSharp.Osm.Routing.Core.Interpreter;
//using OsmSharp.Osm.Routing.Core.Constraints;
//using System.Reflection;
//using OsmSharp.Osm.Routing.Core.Constraints.Cars;
//using OsmSharp.Tools.Math.Geo;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OsmSharp.Osm.Routing.Core.Route;
//using System.IO;

//namespace OsmSharp.Osm.UnitTests.Routing
//{
//    /// <summary>
//    /// Base class with tests around IRouter<ResolvedType> objects.
//    /// </summary>
//    /// <typeparam name="ResolvedType"></typeparam>
//    public abstract class RoutingComparisonTests<ResolvedType>
//        where ResolvedType : IResolvedPoint
//    {
//        /// <summary>
//        /// Returns a router test object.
//        /// </summary>
//        /// <returns></returns>
//        public abstract IRouter<ResolvedType> BuildRouter(RoutingInterpreterBase interpreter,
//            IRoutingConstraints constraints);

//        /// <summary>
//        /// Builds a raw data source.
//        /// </summary>
//        /// <returns></returns>
//        public OsmSharp.Osm.Data.Raw.XML.OsmSource.OsmDataSource BuildRawDataSource()
//        {
//            return new OsmSharp.Osm.Data.Raw.XML.OsmSource.OsmDataSource(
//                   Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network.osm"));
//        }

//        /// <summary>
//        /// Builds a raw router to compare against.
//        /// </summary>
//        /// <returns></returns>
//        public IRouter<Osm.Routing.Raw.ResolvedPoint> BuildRawRouter(Osm.Data.Raw.XML.OsmSource.OsmDataSource osm_data, RoutingInterpreterBase interpreter,
//            IRoutingConstraints constraints)
//        {
//            // build the router.
//            return new OsmSharp.Osm.Routing.Raw.Router(osm_data, interpreter, constraints);
//        }

//        public void TestCompareAll()
//        {   
//            // get the osm data source.
//            OsmSharp.Osm.Data.Raw.XML.OsmSource.OsmDataSource data = this.BuildRawDataSource(); 

//            // build the routing settings.
//            OsmSharp.Osm.Routing.Core.Interpreter.Default.DefaultVehicleInterpreter interpreter = 
//                new OsmSharp.Osm.Routing.Core.Interpreter.Default.DefaultVehicleInterpreter(VehicleEnum.Car);
//            DefaultCarConstraints constraints =
//                new OsmSharp.Osm.Routing.Core.Constraints.Cars.DefaultCarConstraints();

//            // build the reference router.
//            IRouter<Osm.Routing.Raw.ResolvedPoint> reference_router = this.BuildRawRouter(
//                this.BuildRawDataSource(), interpreter, constraints);

//            // build the router to be tested.
//            IRouter<ResolvedType> router = this.BuildRouter(interpreter, constraints);

//            // loop over all nodes and resolve their locations.
//            ResolvedType[] resolved = router.Resolve(
//                data.GetNodes().Select<Osm.Core.Node, GeoCoordinate>(x => x.Coordinate).ToArray());
//            OsmSharp.Osm.Routing.Raw.ResolvedPoint[] resolved_reference = reference_router.Resolve(
//                data.GetNodes().Select<Osm.Core.Node, GeoCoordinate>(x => x.Coordinate).ToArray());

//            // check if the resolved points are exactly the same.
//            Assert.AreEqual(resolved_reference.Length, resolved.Length, "Resolved point count is different!");
//            for (int idx = 0; idx < resolved.Length; idx++)
//            {
//                Assert.AreEqual(resolved_reference[idx].Location.Latitude,
//                    resolved[idx].Location.Latitude, 0.0001);
//                Assert.AreEqual(resolved_reference[idx].Location.Longitude,
//                    resolved[idx].Location.Longitude, 0.0001);
//            }

//            // check all the routes having the same weight(s).
//            for (int from_idx = 0; from_idx < resolved.Length; from_idx++)
//            {
//                for (int to_idx = 0; to_idx < resolved.Length; to_idx++)
//                {
//                    OsmSharpRoute reference_route = reference_router.Calculate(
//                        resolved_reference[from_idx], resolved_reference[to_idx]);
//                    OsmSharpRoute route = router.Calculate(
//                        resolved[from_idx], resolved[to_idx]);

//                    reference_route.SaveAsGpx(new FileInfo(@"c:\temp\reference_route.gpx"));
//                    route.SaveAsGpx(new FileInfo(@"c:\temp\route.gpx"));


//                    Assert.AreEqual(reference_route.TotalDistance, route.TotalDistance, 0.0001);
//                    //Assert.AreEqual(reference_route.TotalTime, route.TotalTime, 0.0001);
//                }
//            }

//        }
//    }
//}
