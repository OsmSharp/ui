using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Osm;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Osm.Interpreter;
using System.Reflection;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.Graph.Path;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.UnitTests.Routing.CH
{
    /// <summary>
    /// Executes the CH contractions while verifying each step.
    /// </summary>
    [TestFixture]
    public class CHVerifiedContractionTests : CHVerifiedContractionBaseTests
    {
        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        [Test]
        public void TestCHVerifiedContractionTestNetwork()
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network.osm"));
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        [Test]
        public void TestCHVerifiedContractionTestNetworkReal()
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network_real1.osm"));
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        [Test]
        public void TestCHVerifiedContractionTestNetworkOneWay()
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network_oneway.osm"));
        }
    }
}
