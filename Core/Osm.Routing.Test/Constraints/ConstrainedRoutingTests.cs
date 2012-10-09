using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Osm.Data.Raw.XML.OsmSource;
using Tools.Xml.Sources;
using Osm.Routing.Core;
using Osm.Routing.Raw.Graphs.Interpreter;
using Osm.Routing.Core.Constraints.Cars;
using Osm.Routing.Raw;
using Osm.Routing.Core.Route;

namespace Osm.Routing.Test.Constraints
{
    public static class ConstrainedRoutingTests
    {
        public static void SimpleTest()
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            string xml = string.Format("{0}\\Constraints\\{1}.osm", info.FullName, "constrained1");

            // create the car interpreter.
            DefaultCarConstraints car_constraints = new DefaultCarConstraints();

            // create the raw router.
            OsmDataSource osm_data = new OsmDataSource(
                new Osm.Core.Xml.OsmDocument(new XmlFileSource(xml)));
            Osm.Routing.Raw.Router raw_router = new Osm.Routing.Raw.Router(osm_data,
                new GraphInterpreterTime(osm_data, VehicleEnum.Car), car_constraints);

            // from Local to Regular.
            ResolvedPoint from = raw_router.ResolveAt(618285258);
            ResolvedPoint to = raw_router.ResolveAt(1547266993);

            OsmSharpRoute route = raw_router.Calculate(from, to);
            route.SaveAsGpx(new FileInfo("Constrained_LocalToRegular.gpx"));

            // from Regular to Local
            from = raw_router.ResolveAt(1547266993);
            to = raw_router.ResolveAt(618285258);

            route = raw_router.Calculate(from, to);
            route.SaveAsGpx(new FileInfo("Constrained_RegularToLocal.gpx"));

            // from Regular to Local
            from = raw_router.ResolveAt(145831);
            to = raw_router.ResolveAt(1547266988);

            route = raw_router.Calculate(from, to);
            route.SaveAsGpx(new FileInfo("Constrained_RegularToRegular.gpx"));

            // from Local to Local
            from = raw_router.ResolveAt(617377176);
            to = raw_router.ResolveAt(618285258);

            route = raw_router.Calculate(from, to);
            route.SaveAsGpx(new FileInfo("Constrained_LocalToLocal.gpx"));
        }
    }
}
