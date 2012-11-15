using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using System.Reflection;
using OsmSharp.Osm.Data.Core.Processor.Default;

namespace OsmSharp.Osm.UnitTests.Data.Processing
{
    /// <summary>
    /// Summary description for PBFDataProcessorSourceTests
    /// </summary>
    [TestClass]
    public class PBFDataProcessorSourceTests
    {
        [TestMethod]
        public void PBFDataProcessorSourceReset()
        {
            // generate the source.
            PBFDataProcessorSource source = new PBFDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Osm.UnitTests.api.osm.pbf"));

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
