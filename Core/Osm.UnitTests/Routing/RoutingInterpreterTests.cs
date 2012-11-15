//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Osm.Routing.Core.Interpreter;
//using OsmSharp.Osm.Data.Raw.XML.OsmSource;
//using OsmSharp.Osm.Core;
//using OsmSharp.Tools.Math.Geo;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace OsmSharp.Osm.UnitTests.Routing
//{
//    /// <summary>
//    /// A basic test class for testing routing interpreters.
//    /// </summary>
//    public abstract class RoutingInterpreterTests
//    {
//        /// <summary>
//        /// The default tolerance value.
//        /// </summary>
//        private static double _DEFAULT_TOLERANCE = 0.0001;

//        /// <summary>
//        /// Holds the data source for this test class.
//        /// </summary>
//        private OsmDataSource _data_source;

//        /// <summary>
//        /// Holds the interpreter to test.
//        /// </summary>
//        private RoutingInterpreterBase _interpreter;

//        /// <summary>
//        /// Creates a new routing interpreter test class.
//        /// </summary>
//        public RoutingInterpreterTests()
//        {
//            // create the data source.
//            _data_source = this.CreateDataSource();

//            // create the interpreter.
//            _interpreter = this.CreateInterpreter();
//        }

//        /// <summary>
//        /// Creates an osm data source.
//        /// </summary>
//        /// <returns></returns>
//        public abstract OsmDataSource CreateDataSource();

//        /// <summary>
//        /// Creates an interpreter.
//        /// </summary>
//        /// <returns></returns>
//        public abstract RoutingInterpreterBase CreateInterpreter();

//        #region Test Methods 

//        /// <summary>
//        /// Tests if the weight between the two given nodes matches the expected weight.
//        /// </summary>
//        /// <param name="from_id"></param>
//        /// <param name="to_id"></param>
//        /// <param name="expected"></param>
//        public void TestWeight(int from_id, int to_id, double expected)
//        {
//            this.TestWeight(from_id, to_id, expected, _DEFAULT_TOLERANCE);
//        }

//        /// <summary>
//        /// Tests if the weight between the two given nodes matches the expected weight.
//        /// </summary>
//        /// <param name="from_id"></param>
//        /// <param name="to_id"></param>
//        /// <param name="expected"></param>
//        /// <param name="tolerance"></param>
//        public void TestWeight(int from_id, int to_id, double expected, double tolerance)
//        {
//            Node from = _data_source.GetNode(from_id);
//            Node to = _data_source.GetNode(to_id);

//            IList<Way> ways_from = _data_source.GetWaysFor(from);
//            IList<Way> ways_to = _data_source.GetWaysFor(to);

//            foreach (Way way in ways_from.Intersect<Way>(ways_to))
//            { // for each found way test the weight.
//                this.TestWeight(way.Tags, from.Coordinate, to.Coordinate, expected, tolerance);
//            }
//        }

//        /// <summary>
//        /// Tests if the weight between two points on a way with the given tags has a certain weight.
//        /// </summary>
//        /// <param name="tags"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <param name="expected"></param>
//        public void TestWeight(IDictionary<string, string> tags, GeoCoordinate from, GeoCoordinate to,
//            double expected)
//        {
//            this.TestWeight(tags, from, to, expected, _DEFAULT_TOLERANCE);
//        }

//        /// <summary>
//        /// Tests if the weight between two points on a way with the given tags has a certain weight.
//        /// </summary>
//        /// <param name="tags"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <param name="expected"></param>
//        /// <param name="tolerance"></param>
//        public void TestWeight(IDictionary<string, string> tags, GeoCoordinate from, GeoCoordinate to,
//            double expected, double tolerance)
//        {
//            //Assert.AreEqual(expected, _interpreter.GetWayInterpretation(tags))
//        }


//        #endregion
//    }
//}
