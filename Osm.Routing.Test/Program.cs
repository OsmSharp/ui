using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data;
using Tools.Xml.Sources;
using Osm.Core.Xml;
using Osm.Core;
using Tools.Math.Graph;
using Osm.Core.Factory;
using Tools.Math.Graph.Routing;
using System.IO;
using Osm.Routing.Instructions;
using Osm.Routing.Raw;
using Osm.Routing.Core.Route;
using Osm.Routing.Test.VRP;
using Osm.Data.Raw.XML.OsmSource;

namespace Osm.Routing.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            OsmGeo.ShapeInterperter = new SimpleShapeInterpreter();
                        
            // finished unit tests.
            //Sparse.SparseTests.DoTests();

            //Sparse.SparseTests.SparseTestRedis();

            CH.CHTests.DoTests();

            //Sparse.SparseTests.SparseTestMemory(); // tests the sparse code.
            //Sparse.SparseTests.SparseTestRedis(); // tests the sparse code.

            //NoDepotTest<ResolvedPoint>.TestEeklo();
            //Redis.RedisTest.PreProcess();

            //Connected.ConnectedTest.TestMatrix();
            //Connected.ConnectedTest.TestMatrixBad();
            //Connected.ConnectedTest.TestSteendorp();
            //Point2Point.Point2PointTest.TestSimple();
            //Point2Point.Point2PointTest.TestBerchem();
            //Point2Point.Point2PointTest.Test("eeklo", 1);
            //Sparse.SparseTest.TestSmall();
            //Tsp.TspTest.Test("matrix_big_area", 1);
            //Tsp.TspTest.Test("moscow", 1);
            //Tsp.TspTest.Test("tiny", 1);
            //Tsp.TspTest.Test("matrix", 1);
            //Matrix.MatrixTest.TestTiny();
            //Matrix.MatrixTest.TestMoscow();
            //Matrix.MatrixTest.TestAtomicSmall();
            //Matrix.MatrixTest.TestLebbeke();
            //Matrix.MatrixTest.Test("eeklo", 1);
            //KDTree.KDTreeTest.TestMatrix();
            //KDTree.KDTreeTest.TestLebbeke();
            //HH.HHTest.Test("tiny", 1);
        }

        static void MatrixCalculations()
        {

        }

        static void Point2PointCalculation()
        {
            // initialize data.
            IDataSource data_source = new OsmDataSource(
                new OsmDocument(new XmlFileSource("test1_input.osm")));

            // create router.
            Router router = new Router(data_source);

            // do routing.
            ResolvedPoint from = router.Resolve(new Tools.Math.Geo.GeoCoordinate(51.269344329834, 4.78804111480713));
            ResolvedPoint to = router.Resolve(new Tools.Math.Geo.GeoCoordinate(51.2657432556152, 4.79507493972778));
            OsmSharpRoute route = router.Calculate(from, to);
            route.SaveAsGpx(new FileInfo("route.gpx"));

            // generate instructions.
            InstructionGenerator generator = new InstructionGenerator();
            List<Instruction> instructions = generator.Generate(route);

            for (int idx = 0; idx < instructions.Count; idx++)
            {
                Instruction ints = instructions[idx];
                Tools.Core.Output.OutputTextStreamHost.WriteLine(ints.Text);
            }

            Console.ReadLine();
        }
    }
}
