// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Collections.Tags.Index;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Osm.Interpreter;
using System.IO;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.Routing.Osm.Streams;
using OsmSharp.Routing.Vehicles;

namespace OsmSharp.Test.Performance.Routing.CH
{
	/// <summary>
	/// Contains test for the CH routing.
	/// </summary>
	public static class CHRoutingTest
	{
		/// <summary>
		/// Tests the routing.
		/// </summary>
		public static void Test()
		{
			CHRoutingTest.TestRouting("CHRouting", "kempen-big.osm.pbf", 10000);
		}

		/// <summary>
		/// Tests routing from a serialized routing file.
		/// </summary>
		public static void Test(Stream stream, int testCount)
		{
			CHRoutingTest.TestRouting("CHRouting", stream, testCount);
		}

		/// <summary>
		/// Tests routing from a serialized routing file.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="testCount"></param>
		public static void TestSerializedResolved(Stream stream, bool lazy = true, int testCount = 100)
		{
			var routingSerializer = new CHEdgeSerializer();
			var data = routingSerializer.Deserialize(stream, lazy);

			CHRoutingTest.TestSerializedResolved(data, new GeoCoordinateBox(new GeoCoordinate(51.20190, 4.66540),
				new GeoCoordinate(51.30720, 4.89820)), testCount);
		}

		/// <summary>
		/// Tests routing from a serialized routing file.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="box"></param>
		/// <param name="testCount"></param>
		public static void TestSerializedResolved(RouterDataSource<CHEdgeData> data, 
			GeoCoordinateBox box, int testCount = 100)
		{
			var successCount = 0;
			var totalCount = testCount;

			var router = Router.CreateCHFrom(data, new CHRouter(), new OsmRoutingInterpreter());

			var performanceInfo = new PerformanceInfoConsumer("CHRouting");
			performanceInfo.Start();
			performanceInfo.Report("Routing {0} routes...", testCount);

			while (testCount > 0)
			{
				var point1 = router.Resolve(Vehicle.Car, box.GenerateRandomIn());
				var point2 = router.Resolve(Vehicle.Car, box.GenerateRandomIn());

				Route route = null;
				if (point1 != null && point2 != null)
				{
					route = router.Calculate(Vehicle.Car, point1, point2);
				}

				if (route != null)
				{
					successCount++;
				}
				testCount--;
			}

			performanceInfo.Stop();

			OsmSharp.Logging.Log.TraceEvent("CHRouting", OsmSharp.Logging.TraceEventType.Information,
				string.Format("{0}/{1} routes successfull!", successCount, totalCount));
		}

		/// <summary>
		/// Tests routing from a serialized file.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="lazy"></param>
		/// <param name="testCount"></param>
		public static void TestSerialized(Stream stream, bool lazy = true, int testCount = 100)
		{
			var routingSerializer = new CHEdgeSerializer();
			var data = routingSerializer.Deserialize(stream, lazy);

			//data.SortHilbert(1000);

			//// copy.
			//var graphCopy = new DirectedGraph<CHEdgeData>();
			//graphCopy.CopyFrom(data);
			//var dataCopy = new RouterDataSource<CHEdgeData>(graphCopy, data.TagsIndex);

			CHRoutingTest.Test(data, testCount);
		}

		/// <summary>
		/// Tests routing from a serialized file.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="box"></param>
		/// <param name="lazy"></param>
		/// <param name="testCount"></param>
		public static void TestSerializedResolved(Stream stream, GeoCoordinateBox box, bool lazy = true, int testCount = 100)
		{
			var routingSerializer = new CHEdgeSerializer();
			var data = routingSerializer.Deserialize(stream, lazy);

			CHRoutingTest.TestResolved(data, testCount, box);
		}

		/// <summary>
		/// Tests routing from a serialized routing file.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="stream"></param>
		/// <param name="testCount"></param>
		public static void TestRouting(string name, Stream stream, int testCount)
		{
			var vehicle = Vehicle.Car;

			var tagsIndex = new TagsIndex(); // creates a tagged index.

			// read from the OSM-stream.
			var source = new OsmSharp.Osm.Streams.Filters.OsmStreamFilterProgress();
			source.RegisterSource(new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(stream));
			var data = CHEdgeGraphOsmStreamTarget.Preprocess(source,
				new OsmRoutingInterpreter(), vehicle);

			//(data.Graph as DirectedGraph<CHEdgeData>).Compress(true);

			CHRoutingTest.Test(data, testCount);
		}

		/// <summary>
		/// Tests routing from a serialized routing file.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="testCount"></param>
		public static void Test(RouterDataSource<CHEdgeData> data, int testCount)
		{
			var router = new CHRouter();

			var performanceInfo = new PerformanceInfoConsumer("CHRouting");
			performanceInfo.Start();
			performanceInfo.Report("Routing {0} routes...", testCount);

			var successCount = 0;
			var totalCount = testCount;
			var latestProgress = -1.0f;
			while (testCount > 0)
			{
				var from = (uint)OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(data.VertexCount - 1) + 1;
				var to = (uint)OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(data.VertexCount - 1) + 1;

				var route = router.Calculate(data, from, to);

				if (route != null)
				{
					successCount++;
				}
				testCount--;

				// report progress.
				var progress = (float)System.Math.Round(((double)(totalCount - testCount) / (double)totalCount) * 100);
				if (progress != latestProgress)
				{
					OsmSharp.Logging.Log.TraceEvent("CHRouting", TraceEventType.Information,
						"Routing... {0}%", progress);
					latestProgress = progress;
				}
			}
			performanceInfo.Stop();

			OsmSharp.Logging.Log.TraceEvent("CHRouting", OsmSharp.Logging.TraceEventType.Information,
				string.Format("{0}/{1} routes successfull!", successCount, totalCount));
		}

		public static void TestResolved(RouterDataSource<CHEdgeData> data, int testCount, GeoCoordinateBox box)
		{
			var router = Router.CreateCHFrom(data, new CHRouter(), new OsmRoutingInterpreter());

			var performanceInfo = new PerformanceInfoConsumer("CHRouting");
			performanceInfo.Start();
			performanceInfo.Report("Routing {0} routes...", testCount);

			var successCount = 0;
			var totalCount = testCount;
			var latestProgress = -1.0f;
			while (testCount > 0)
			{
				var from = box.GenerateRandomIn();
				var to = box.GenerateRandomIn();

				var fromPoint = router.Resolve(Vehicle.Car, from);
				var toPoint = router.Resolve(Vehicle.Car, to);

				if (fromPoint != null && toPoint != null)
				{
					var route = router.Calculate(Vehicle.Car, fromPoint, toPoint);
					if (route != null)
					{
						successCount++;
					}
				}
				testCount--;

				// report progress.
				var progress = (float)System.Math.Round(((double)(totalCount - testCount) / (double)totalCount) * 100);
				if (progress != latestProgress)
				{
					OsmSharp.Logging.Log.TraceEvent("CHRouting", TraceEventType.Information,
						"Routing... {0}%", progress);
					latestProgress = progress;
				}
			}
			performanceInfo.Stop();

			OsmSharp.Logging.Log.TraceEvent("CHRouting", OsmSharp.Logging.TraceEventType.Information,
				string.Format("{0}/{1} routes successfull!", successCount, totalCount));
		}


		/// <summary>
		/// Tests routing from a serialized routing file.
		/// </summary>
		public static void TestRouting(string name, string osmPbfFile, int testCount)
		{
			var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", osmPbfFile));
			var stream = testFile.OpenRead();

			CHRoutingTest.TestRouting(name, stream, testCount);

			stream.Dispose();
		}
	}
}