using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Osm.Core;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Osm.Routing.Interpreter;
using System.Reflection;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.Core.Graph.Path;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Osm.Routing.Test.CH;

namespace OsmSharp.UnitTests.Routing.CH
{
    /// <summary>
    /// Executes the CH contractions while verifying each step.
    /// </summary>
    [TestClass]
    public class CHVerifiedContractionTests : CHVerifiedContractionBaseTests
    {
        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        [TestMethod]
        public void TestCHVerifiedContractionTestNetwork()
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network.osm"));
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        [TestMethod]
        public void TestCHVerifiedContractionTestNetworkReal()
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network_real1.osm"));
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        [TestMethod]
        public void TestCHVerifiedContractionTestNetworkOneWay()
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network_oneway.osm"));
        }
    }
}
