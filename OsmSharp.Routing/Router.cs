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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Osm.Data.Streams.Filters;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Routers;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math;
using OsmSharp.Collections;
using OsmSharp.Osm.Data.Core.Processor;

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
        private Router(ITypedRouter router)
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
        public static Router CreateLiveFrom(OsmStreamSource reader, IRoutingInterpreter interpreter)
        {
            var tagsIndex = new SimpleTagsIndex(); // creates a tagged index.

            // read from the OSM-stream.
            var memoryData = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamWriter(memoryData, interpreter, tagsIndex);
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(reader);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            // creates the live edge router.
            var liveEdgeRouter = new TypedRouterLiveEdge(
                memoryData, interpreter, new DykstraRoutingLive(tagsIndex));

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
                data, interpreter, new DykstraRoutingLive(data.TagsIndex));

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
            return _router.Calculate(vehicle, source, target);
        }

        /// <summary>
        /// Calculates a route between two given points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="max">The maximum weight to stop the calculation.</param>
        /// <returns></returns>
        public Route Calculate(Vehicle vehicle, RouterPoint source, RouterPoint target, float max)
        {
            return _router.Calculate(vehicle, source, target, max);
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
            return _router.CalculateToClosest(vehicle, source, targets);
        }

        /// <summary>
        /// Calculates a shortest route from a given point to any of the targets points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source">The source point.</param>
        /// <param name="targets">The target point(s).</param>
        /// <param name="max">The maximum weight to stop the calculation.</param>
        /// <returns></returns>
        public Route CalculateToClosest(Vehicle vehicle, RouterPoint source, RouterPoint[] targets, float max)
        {
            return _router.CalculateToClosest(vehicle, source, targets, max);
        }

        /// <summary>
        /// Calculates all routes between one source and many target points.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public Route[] CalculateOneToMany(Vehicle vehicle, RouterPoint source, RouterPoint[] targets)
        {
            return _router.CalculateOneToMany(vehicle, source, targets);
        }

        /// <summary>
        /// Calculates all routes between many sources/targets.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public Route[][] CalculateManyToMany(Vehicle vehicle, RouterPoint[] sources, RouterPoint[] targets)
        {
            return _router.CalculateManyToMany(vehicle, sources, targets);
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
            return _router.CheckConnectivity(vehicle, point, weight);
        }

        /// <summary>
        /// Returns true if the given point is connected for a radius of at least the given weight.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool[] CheckConnectivity(Vehicle vehicle, RouterPoint[] point, float weight)
        {
            return _router.CheckConnectivity(vehicle, point, weight);
        }

        /// <summary>
        /// Resolves a point.
        /// </summary>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="coordinate">The location of the point to resolve.</param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate)
        {
            return _router.Resolve(vehicle, coordinate);
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
            TagsCollection matchingTags)
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
            IEdgeMatcher matcher, TagsCollection matchingTags)
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
            TagsCollection[] matchingTags)
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
            IEdgeMatcher matcher, TagsCollection[] matchingTags)
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
