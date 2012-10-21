// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
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
        public static void SimpleTestServiceRoads()
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            string xml = string.Format("{0}\\Constraints\\{1}.osm", info.FullName, "constrained1");

            // create the car interpreter.
            DefaultCarConstraints car_constraints = new DefaultCarConstraints();

            // create the raw router.
            OsmDataSource osm_data = new OsmDataSource(
                new Osm.Core.Xml.OsmDocument(new XmlFileSource(xml)));
            Osm.Routing.Raw.Router raw_router = new Osm.Routing.Raw.Router(osm_data,
                new Routing.Core.Interpreter.Default.DefaultVehicleInterpreter(VehicleEnum.Car), car_constraints);

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

        public static void SimpleTestPedestrianRoads()
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            string xml = string.Format("{0}\\Constraints\\{1}.osm", info.FullName, "constrained2");

            // create the car interpreter.
            DefaultCarConstraints car_constraints = new DefaultCarConstraints();

            // create the raw router.
            OsmDataSource osm_data = new OsmDataSource(
                new Osm.Core.Xml.OsmDocument(new XmlFileSource(xml)));
            Osm.Routing.Raw.Router raw_router = new Osm.Routing.Raw.Router(osm_data,
                new Routing.Core.Interpreter.Default.DefaultVehicleInterpreter(VehicleEnum.Car), car_constraints);

            // from Local to Regular.
            ResolvedPoint from = raw_router.ResolveAt(1131349881);
            ResolvedPoint to = raw_router.ResolveAt(1128910258);

            OsmSharpRoute route = raw_router.Calculate(from, to);
            route.SaveAsGpx(new FileInfo("Constrained_Pedestrian1.gpx"));

            // from Regular to Local
            from = raw_router.ResolveAt(599296033);
            to = raw_router.ResolveAt(506355189);
            route = raw_router.Calculate(from, to);
            route.SaveAsGpx(new FileInfo("Constrained_Pedestrian2.gpx"));
        }
    }
}