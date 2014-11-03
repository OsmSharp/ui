// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Streams;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Graphs.Serialization;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharp.Routing.Routers;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing
{
    /// <summary>
    /// Interface representing a router.
    /// </summary>
    public class Router : ITypedRouter
    {
        /// <summary>
        /// Holds the basic router implementation.
        /// </summary>
        private readonly ITypedRouter _router;

        /// <summary>
        /// Creates a new router with the given router implementation.
        /// </summary>
        /// <param name="router"></param>
        public Router(ITypedRouter router)
        {
            _router = router;
        }

        #region Static Creation Methods

        /// <summary>
        /// Creates a router using live interpreted edges.
        /// </summary>
        /// <param name="reader">The OSM-stream reader.</param>
        /// <param name="interpreter">The routing interpreter.</param>
        /// <returns></returns>
        public static Router CreateLiveFrom(OsmStreamSource reader, IOsmRoutingInterpreter interpreter)
        {
            var tagsIndex = new TagsTableCollectionIndex(); // creates a tagged index.

            // read from the OSM-stream.
            var memoryData = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            memoryData.DropVertexIndex();
            var targetData = new LiveGraphOsmStreamTarget(memoryData, interpreter, tagsIndex);
            targetData.RegisterSource(reader);
            targetData.Pull();
            memoryData.RebuildVertexIndex();

            // creates the live edge router.
            var liveEdgeRouter = new TypedRouterLiveEdge(
                memoryData, interpreter, new DykstraRoutingLive());

            return new Router(liveEdgeRouter); // create the actual router.
        }

        /// <summary>
        /// Creates a router using live interpreted edges.
        /// </summary>
        /// <param name="data">The data to route on.</param>
        /// <param name="interpreter">The routing interpreter.</param>
        /// <returns></returns>
        public static Router CreateLiveFrom(IBasicRouterDataSource<LiveEdge> data, IRoutingInterpreter interpreter)
        {
            // creates the live edge router.
            var liveEdgeRouter = new TypedRouterLiveEdge(
                data, interpreter, new DykstraRoutingLive());

            return new Router(liveEdgeRouter); // create the actual router.
        }

        /// <summary>
        /// Creates a router using live interpreted edges.
        /// </summary>
        /// <param name="data">The data to route on.</param>
        /// <param name="basicRouter">A custom routing implementation.</param>
        /// <param name="interpreter">The routing interpreter.</param>
        /// <returns></returns>
        public static Router CreateLiveFrom(IBasicRouterDataSource<LiveEdge> data, IBasicRouter<LiveEdge> basicRouter, 
            IRoutingInterpreter interpreter)
        {
            // creates the live edge router.
            var liveEdgeRouter = new TypedRouterLiveEdge(
                data, interpreter, basicRouter);

            return new Router(liveEdgeRouter); // create the actual router.
        }

        /// <summary>
        /// Creates a new router.
        /// </summary>
        /// <param name="flatFile">The flatfile containing the graph.</param>
        /// <param name="interpreter">The routing interpreter.</param>
        /// <returns></returns>
        public static Router CreateLiveFrom(Stream flatFile, IOsmRoutingInterpreter interpreter)
        {
            var serializer = new LiveEdgeFlatfileSerializer();
            var source = serializer.Deserialize(flatFile, false) as DynamicGraphRouterDataSource<LiveEdge>;

            // creates the live edge router.
            var liveEdgeRouter = new TypedRouterLiveEdge(
                source, interpreter, new DykstraRoutingLive());

            return new Router(liveEdgeRouter); // create the actual router.
        }

        /// <summary>
        /// Creates a router using live interpreted edges.
        /// </summary>
        /// <param name="data">The data to route on.</param>
        /// <param name="basicRouter">A custom routing implementation.</param>
        /// <param name="interpreter">The routing interpreter.</param>
        /// <returns></returns>
        public static Router CreateCHFrom(IBasicRouterDataSource<CHEdgeData> data, IBasicRouter<CHEdgeData> basicRouter, 
            IRoutingInterpreter interpreter)
        {
            // creates the live edge router.
            var liveEdgeRouter = new TypedRouterCHEdge(
                data, interpreter, basicRouter);

            return new Router(liveEdgeRouter); // create the actual router.
        }

        /// <summary>
        /// Creates a router using live interpreted edges.
        /// </summary>
        /// <param name="reader">The data to route on.</param>
        /// <param name="interpreter">The routing interpreter.</param>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <returns></returns>
        public static Router CreateCHFrom(OsmStreamSource reader,
            IOsmRoutingInterpreter interpreter, Vehicle vehicle)
        {
            var tagsIndex = new TagsTableCollectionIndex(); // creates a tagged index.

            // read from the OSM-stream.
            DynamicGraphRouterDataSource<CHEdgeData> data = CHEdgeGraphOsmStreamTarget.Preprocess(
                reader, interpreter, vehicle);

            // creates the live edge router.
            var liveEdgeRouter = new TypedRouterCHEdge(
                data, interpreter, new CHRouter());

            return new Router(liveEdgeRouter); // create the actual router.
        }

        #endregion

        #region ITypedRouter

        /// <summary>
        /// Returns true if the given vehicle type is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsVehicle(Vehicle vehicle)
        {
            return _router.SupportsVehicle(vehicle);
        }

        /// <summary>
        /// Calculates a route between two given points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <returns></returns>
        public Route Calculate(Vehicle vehicle, RouterPoint source, RouterPoint target)
        {
            if(source.Vehicle.UniqueName != target.Vehicle.UniqueName)
            { // vehicles are different.
                throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                    source.Vehicle.UniqueName, target.Vehicle.UniqueName));
            }
            if(vehicle.UniqueName != target.Vehicle.UniqueName)
            { // vehicles are different.
                throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                    vehicle.UniqueName, target.Vehicle.UniqueName));
            }

            return _router.Calculate(vehicle, source, target);
        }

        /// <summary>
        /// Calculates a route between two given points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="max">The maximum weight to stop the calculation.</param>
        /// <param name="geometryOnly">Returns the geometry only when true.</param>
        /// <returns></returns>
        public Route Calculate(Vehicle vehicle, RouterPoint source, RouterPoint target, float max = float.MaxValue, bool geometryOnly = false)
        {
            if (source.Vehicle.UniqueName != target.Vehicle.UniqueName)
            { // vehicles are different.
                throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                    source.Vehicle.UniqueName, target.Vehicle.UniqueName));
            }
            if (vehicle.UniqueName != target.Vehicle.UniqueName)
            { // vehicles are different.
                throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                    vehicle.UniqueName, target.Vehicle.UniqueName));
            }

            return _router.Calculate(vehicle, source, target, max, geometryOnly);
        }

        /// <summary>
        /// Calculates a shortest route from a given point to any of the targets points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source">The source point.</param>
        /// <param name="targets">The target point(s).</param>
        /// <returns></returns>
        public Route CalculateToClosest(Vehicle vehicle, RouterPoint source, RouterPoint[] targets)
        {
            foreach (var target in targets)
            {
                if (source.Vehicle.UniqueName != target.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                        source.Vehicle.UniqueName, target.Vehicle.UniqueName));
                }
                if (vehicle.UniqueName != target.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                        vehicle.UniqueName, target.Vehicle.UniqueName));
                }
            }

            return _router.CalculateToClosest(vehicle, source, targets);
        }

        /// <summary>
        /// Calculates a shortest route from a given point to any of the targets points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source">The source point.</param>
        /// <param name="targets">The target point(s).</param>
        /// <param name="max">The maximum weight to stop the calculation.</param>
        /// <param name="geometryOnly">Flag to return .</param>
        /// <returns></returns>
        public Route CalculateToClosest(Vehicle vehicle, RouterPoint source, RouterPoint[] targets, float max = float.MaxValue, bool geometryOnly = false)
        {
            foreach (var target in targets)
            {
                if (source.Vehicle.UniqueName != target.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                        source.Vehicle.UniqueName, target.Vehicle.UniqueName));
                }
                if (vehicle.UniqueName != target.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                        vehicle.UniqueName, target.Vehicle.UniqueName));
                }
            }

            return _router.CalculateToClosest(vehicle, source, targets, max);
        }

        /// <summary>
        /// Calculates all routes between one source and many target points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max">The maximum weight to stop the calculation.</param>
        /// <param name="geometryOnly">Flag to return .</param>
        /// <returns></returns>
        public Route[] CalculateOneToMany(Vehicle vehicle, RouterPoint source, RouterPoint[] targets, float max = float.MaxValue, bool geometryOnly = false)
        {
            foreach (var target in targets)
            {
                if (source.Vehicle.UniqueName != target.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                        source.Vehicle.UniqueName, target.Vehicle.UniqueName));
                }
                if (vehicle.UniqueName != target.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                        vehicle.UniqueName, target.Vehicle.UniqueName));
                }
            }

            return _router.CalculateOneToMany(vehicle, source, targets, max, geometryOnly);
        }

        /// <summary>
        /// Calculates all routes between many sources/targets.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="max">The maximum weight to stop the calculation.</param>
        /// <param name="geometryOnly">Flag to return .</param>
        /// <returns></returns>
        public Route[][] CalculateManyToMany(Vehicle vehicle, RouterPoint[] sources, RouterPoint[] targets, float max = float.MaxValue, bool geometryOnly = false)
        {
            foreach (var source in sources)
            {
                foreach (var target in targets)
                {
                    if (source.Vehicle.UniqueName != target.Vehicle.UniqueName)
                    { // vehicles are different.
                        throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                            source.Vehicle.UniqueName, target.Vehicle.UniqueName));
                    }
                    if (vehicle.UniqueName != target.Vehicle.UniqueName)
                    { // vehicles are different.
                        throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                            vehicle.UniqueName, target.Vehicle.UniqueName));
                    }
                }
            }

            return _router.CalculateManyToMany(vehicle, sources, targets, max, geometryOnly);
        }

        /// <summary>
        /// Calculates the weight between two given points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public double CalculateWeight(Vehicle vehicle, RouterPoint source, RouterPoint target)
        {
            if (source.Vehicle.UniqueName != target.Vehicle.UniqueName)
            { // vehicles are different.
                throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                    source.Vehicle.UniqueName, target.Vehicle.UniqueName));
            }
            if (vehicle.UniqueName != target.Vehicle.UniqueName)
            { // vehicles are different.
                throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                    vehicle.UniqueName, target.Vehicle.UniqueName));
            }

            return _router.CalculateWeight(vehicle, source, target);
        }

        /// <summary>
        /// Calculates a route between one source and many target points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public double[] CalculateOneToManyWeight(Vehicle vehicle, RouterPoint source, RouterPoint[] targets)
        {
            foreach (var target in targets)
            {
                if (source.Vehicle.UniqueName != target.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                        source.Vehicle.UniqueName, target.Vehicle.UniqueName));
                }
                if (vehicle.UniqueName != target.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                        vehicle.UniqueName, target.Vehicle.UniqueName));
                }
            }

            return _router.CalculateOneToManyWeight(vehicle, source, targets);
        }

        /// <summary>
        /// Calculates all routes between many sources/targets.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public double[][] CalculateManyToManyWeight(Vehicle vehicle, RouterPoint[] sources, RouterPoint[] targets)
        {
            foreach (var source in sources)
            {
                foreach (var target in targets)
                {
                    if (source.Vehicle.UniqueName != target.Vehicle.UniqueName)
                    { // vehicles are different.
                        throw new ArgumentException(string.Format("Not all vehicle profiles match, {0} and {1} are given, expecting identical profiles.",
                            source.Vehicle.UniqueName, target.Vehicle.UniqueName));
                    }
                    if (vehicle.UniqueName != target.Vehicle.UniqueName)
                    { // vehicles are different.
                        throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                            vehicle.UniqueName, target.Vehicle.UniqueName));
                    }
                }
            }

            return _router.CalculateManyToManyWeight(vehicle, sources, targets);
        }

        /// <summary>
        /// Returns true if range calculation is supported.
        /// </summary>
        public bool IsCalculateRangeSupported
        {
            get { return _router.IsCalculateRangeSupported; }
        }

        /// <summary>
        /// Returns all points located at a given weight (distance/time) from the orgin.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="orgine"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public HashSet<GeoCoordinate> CalculateRange(Vehicle vehicle, RouterPoint orgine, float weight)
        {
            if (vehicle.UniqueName != orgine.Vehicle.UniqueName)
            { // vehicles are different.
                throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                    vehicle.UniqueName, orgine.Vehicle.UniqueName));
            }

            return _router.CalculateRange(vehicle, orgine, weight);
        }

        /// <summary>
        /// Returns true if the given point is connected for a radius of at least the given weight.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool CheckConnectivity(Vehicle vehicle, RouterPoint point, float weight)
        {
            if (vehicle.UniqueName != point.Vehicle.UniqueName)
            { // vehicles are different.
                throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                    vehicle.UniqueName, point.Vehicle.UniqueName));
            }

            return _router.CheckConnectivity(vehicle, point, weight);
        }

        /// <summary>
        /// Returns true if the given point is connected for a radius of at least the given weight.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="points"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool[] CheckConnectivity(Vehicle vehicle, RouterPoint[] points, float weight)
        {
            foreach (var point in points)
            {
                if (vehicle.UniqueName != point.Vehicle.UniqueName)
                { // vehicles are different.
                    throw new ArgumentException(string.Format("Given vehicle profile does not match resolved points, {0} and {1} are given, expecting identical profiles.",
                        vehicle.UniqueName, point.Vehicle.UniqueName));
                }
            }

            return _router.CheckConnectivity(vehicle, points, weight);
        }

        /// <summary>
        /// Resolves a point.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="coordinate">The location of the point to resolve.</param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate)
        {
            return _router.Resolve(vehicle, coordinate, false);
        }

        /// <summary>
        /// Resolves a point.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="coordinate">The location of the point to resolve.</param>
        /// <param name="verticesOnly">Returns vertices only..</param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate, bool verticesOnly)
        {
            return _router.Resolve(vehicle, coordinate, verticesOnly);
        }

        /// <summary>
        /// Resolves a point.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="delta">The size of the box to search in.</param>
        /// <param name="coordinate">The location of the point to resolve.</param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, float delta, GeoCoordinate coordinate)
        {
            return _router.Resolve(vehicle, delta, coordinate);
        }

        /// <summary>
        /// Resolves a point.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="coordinate">The location of the point to resolve.</param>
        /// <param name="matcher">The matcher containing some matching algorithm.</param>
        /// <param name="matchingTags">Extra matching data.</param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate, IEdgeMatcher matcher, 
            TagsCollectionBase matchingTags)
        {
            return _router.Resolve(vehicle, coordinate, matcher, matchingTags);
        }

        /// <summary>
        /// Resolves a point.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="delta">The size of the box to search in.</param>
        /// <param name="coordinate">The location of the point to resolve.</param>
        /// <param name="matcher">The matcher containing some matching algorithm.</param>
        /// <param name="matchingTags">Extra matching data.</param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, float delta, GeoCoordinate coordinate, 
            IEdgeMatcher matcher, TagsCollectionBase matchingTags)
        {
            return _router.Resolve(vehicle, delta, coordinate, matcher, matchingTags);
        }

        /// <summary>
        /// Resolves all the given points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="coordinate">The location of the point to resolve.</param>
        /// <returns></returns>
        public RouterPoint[] Resolve(Vehicle vehicle, GeoCoordinate[] coordinate)
        {
            return _router.Resolve(vehicle, coordinate);
        }

        /// <summary>
        /// Resolves all the given points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="delta">The size of the box to search in.</param>
        /// <param name="coordinate">The location of the point to resolve.</param>
        /// <returns></returns>
        public RouterPoint[] Resolve(Vehicle vehicle, float delta, GeoCoordinate[] coordinate)
        {
            return _router.Resolve(vehicle, delta, coordinate);
        }

        /// <summary>
        /// Resolves all the given points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="coordinates">The location of the points to resolve.</param>
        /// <param name="matcher">The matcher containing some matching algorithm.</param>
        /// <param name="matchingTags">Extra matching data.</param>
        /// <returns></returns>
        public RouterPoint[] Resolve(Vehicle vehicle, GeoCoordinate[] coordinates, IEdgeMatcher matcher, 
            TagsCollectionBase[] matchingTags)
        {
            return _router.Resolve(vehicle, coordinates, matcher, matchingTags);
        }

        /// <summary>
        /// Resolves all the given points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="delta">The size of the box to search in.</param>
        /// <param name="coordinates">The location of the points to resolve.</param>
        /// <param name="matcher">The matcher containing some matching algorithm.</param>
        /// <param name="matchingTags">Extra matching data.</param>
        /// <returns></returns>
        public RouterPoint[] Resolve(Vehicle vehicle, float delta, GeoCoordinate[] coordinates, 
            IEdgeMatcher matcher, TagsCollectionBase[] matchingTags)
        {
            return _router.Resolve(vehicle, delta, coordinates, matcher, matchingTags);
        }

        /// <summary>
        /// Searches for a closeby link to the road network.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="coordinate">The location of the point to search.</param>
        /// <returns></returns>
        /// <remarks>Similar to resolve except no resolved point is created.</remarks>
        public GeoCoordinate Search(Vehicle vehicle, GeoCoordinate coordinate)
        {
            return _router.Search(vehicle, coordinate);
        }

        /// <summary>
        /// Searches for a closeby link to the road network.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="delta">The size of the box to search in.</param>
        /// <param name="coordinate">The location of the point to search.</param>
        /// <returns></returns>
        /// <remarks>Similar to resolve except no resolved point is created.</remarks>
        public GeoCoordinate Search(Vehicle vehicle, float delta, GeoCoordinate coordinate)
        {
            return _router.Search(vehicle, delta, coordinate);
        }

        #endregion
    }
}
