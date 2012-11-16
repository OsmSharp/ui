using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using System.Reflection;
using OsmSharp.Osm.Data.Core.Processor.Default;

namespace OsmSharp.Osm.UnitTests.Data.Processing
{
    /// <summary>
    /// Summary description for XmlDataProcessorSourceTests
    /// </summary>
    [TestClass]
    public class XmlDataProcessorSourceTests
    {
        /// <summary>
        /// A regression test in resetting and XML data source.
        /// </summary>
        [TestMethod]
        public void XmlDataProcessorSourceReset()
        {
            // generate the source.
            XmlDataProcessorSource source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.api.osm"));

            // pull the data out.
            DataProcessorTargetEmpty target = new DataProcessorTargetEmpty();
            target.RegisterSource(source);
            target.Pull();

            // reset the source.
            if (source.CanReset)
            {
                source.Reset();

                // pull the data again.
                target.Pull();
            }
        }
    }
}
