using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Tools.TSPLIB.Parser;
using System.Reflection;
using System.IO;
using OsmSharp.Tools.Math.TSP.BruteForce;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.UnitTests.TSP
{
    /// <summary>
    /// A small TSP test.
    /// </summary>
    [TestFixture]
    public class BruteForceTSPTest
    {
        /// <summary>
        /// Tests a small TSP 5.
        /// </summary>
        [Test]
        public void TestBruteForceTSP5()
        {
            // load the problem.
            OsmSharp.Tools.TSPLIB.Problems.TSPLIBProblem problem = TSPLIBProblemParser.ParseFrom(new StreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_tsp_5.tsp")));

            // solve using the brute force solver.
            BruteForceSolver solver = new BruteForceSolver();
            IRoute route = solver.Solve(problem);

            int[] route_array = route.ToArray();
            Assert.AreEqual(0, route_array[0]);
            Assert.AreEqual(3, route_array[1]);
            Assert.AreEqual(4, route_array[2]);
            Assert.AreEqual(1, route_array[3]);
            Assert.AreEqual(2, route_array[4]);
        }

        /// <summary>
        /// Tests a small TSP 6.
        /// </summary>
        [Test]
        public void TestBruteForceTSP6()
        {
            // load the problem.
            OsmSharp.Tools.TSPLIB.Problems.TSPLIBProblem problem = TSPLIBProblemParser.ParseFrom(new StreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_tsp_6.tsp")));

            // solve using the brute force solver.
            BruteForceSolver solver = new BruteForceSolver();
            IRoute route = solver.Solve(problem);

            int[] route_array = route.ToArray();
            Assert.AreEqual(0, route_array[0]);
            Assert.AreEqual(4, route_array[1]);
            Assert.AreEqual(5, route_array[2]);
            Assert.AreEqual(3, route_array[3]);
            Assert.AreEqual(1, route_array[4]);
            Assert.AreEqual(2, route_array[5]);
        }
    }
}
