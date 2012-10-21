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
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data;
using Tools.Xml.Sources;
using Osm.Core.Xml;
using System.IO;
using Osm.Core;
using Osm.Routing.HH.Neigbourhoods;
using Osm.Routing.Raw.Graphs.Interpreter;
using Osm.Routing.Core;
using Osm.Routing.Raw.Graphs;
using Osm.Data.Raw.XML.OsmSource;
using Osm.Routing.Core.Interpreter.Default;

namespace Osm.Routing.Test.HH
{
    class HHTest
    {
        public static void Test(string name, int test_count)
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            // initialize data.
            OsmDataSource data_source = new OsmDataSource(
                new OsmDocument(new XmlFileSource(info.FullName + string.Format("\\HH\\{0}.osm", name))));

            GraphInterpreterBase interpreter = new GraphInterpreterTime(new DefaultVehicleInterpreter(VehicleEnum.Car), data_source, VehicleEnum.Car);
            Graph graph = new Graph(interpreter, data_source);



            IList<long> ids = new List<long>();
            ids.Add(1073131732);
            ids.Add(441874891);
            ids.Add(1073131730);
            ids.Add(1073131728);
            ids.Add(291748880);
            ids.Add(441874888);
            ids.Add(291738783);
            ids.Add(1131414408);
            ids.Add(291738782);
            ids.Add(1131414572);
            ids.Add(441874893);

            foreach(Node node in data_source.GetNodes(ids))
            {
                GraphVertex start = new GraphVertex(node);

                //HashSet<HighwayEdge> edges = new HashSet<HighwayEdge>();

                //Osm.Routing.HH.Highways.Construction<Way>.Construct(edges, graph, start);
            }
        }
    }
}
