using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.Core.Raw.Memory;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Data;

namespace OsmSharp.Osm.UnitTests.Data
{
    /// <summary>
    /// Containts tests for the PBF format.
    /// </summary>
    [TestClass]
    public class PBFTests
    {
        /// <summary>
        /// Tests reading PBF data from a Stream.
        /// </summary>
        [TestMethod]
        public void TestPBFReaderDefault()
        {
            this.TestReadPBF("Osm.UnitTests.api.osm.pbf");
        }

        /// <summary>
        /// Tests reading PBF data from a Stream.
        /// </summary>
        [TestMethod]
        public void TestPBFReaderNoMeta()
        {
            this.TestReadPBF("Osm.UnitTests.api_omitmetadata_true.osm.pbf");
        }

        /// <summary>
        /// Tests reading PBF data from a Stream.
        /// </summary>
        [TestMethod]
        public void TestPBFReaderNoCompress()
        {
            this.TestReadPBF("Osm.UnitTests.api_compress_none.osm.pbf");
        }

        /// <summary>
        /// Tests reading PBF data from a Stream.
        /// </summary>
        [TestMethod]
        public void TestPBFReaderUseDense()
        {
            this.TestReadPBF("Osm.UnitTests.api_usedense_false.osm.pbf");
        }

        /// <summary>
        /// Tests reading a PBF and see if the data is there.
        /// </summary>
        /// <param name="resource"></param>
        private void TestReadPBF(string resource)
        {
            // create the pbf source from a pbf in the resources of this assembly.
            PBFDataProcessorSource source = new PBFDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(resource));

            // pull the data from the source into the memory data source.
            MemoryDataSource data = new MemoryDataSource();
            data.PullFromSource(source);

            // test the data.
            this.TestData(data);
        }

        /// <summary>
        /// Tests if the data present is the data from api.osm.
        /// </summary>
        private void TestData(IDataSourceReadOnly data)
        {
            // test what data is present in the datasource.
            Node node_291738780 = data.GetNode(291738780);
            Assert.IsNotNull(node_291738780);
            Assert.AreEqual(291738780, node_291738780.Id);

            Node node_1727654333 = data.GetNode(1727654333);
            Assert.IsNotNull(node_1727654333);
            Assert.AreEqual(1727654333, node_1727654333.Id);

            Way way_87281441 = data.GetWay(87281441);
            Assert.IsNotNull(way_87281441);
            Assert.AreEqual(87281441, way_87281441.Id);

            Way way_76705106 = data.GetWay(76705106);
            Assert.IsNotNull(way_76705106);
            Assert.AreEqual(76705106, way_76705106.Id);
        }

        
    }
}
