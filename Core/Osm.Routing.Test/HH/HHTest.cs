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
