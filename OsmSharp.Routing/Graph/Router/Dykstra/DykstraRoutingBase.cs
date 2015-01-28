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

using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Units.Distance;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.Graph.Router.Dykstra
{
    /// <summary>
    /// Contains generic fuctions common to all dykstra routers.
    /// </summary>
    public abstract class DykstraRoutingBase<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Creates a new basic dykstra router.
        /// </summary>
        protected DykstraRoutingBase()
        {

        }

        #region Search Closest

        /// <summary>
        /// Searches the data for a point on an edge closest to the given coordinate.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="delta"></param>
        /// <param name="matcher"></param>
        /// <param name="pointTags"></param>
        /// <param name="interpreter"></param>
        /// <param name="parameters"></param>
        public SearchClosestResult<TEdgeData> SearchClosest(IBasicRouterDataSource<TEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollectionBase pointTags, Dictionary<string, object> parameters)
        {
            return this.SearchClosest(graph, interpreter, vehicle, coordinate, delta, matcher, pointTags, false, null);
        }

        /// <summary>
        /// Searches the data for a point on an edge closest to the given coordinate.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="delta"></param>
        /// <param name="matcher"></param>
        /// <param name="pointTags"></param>
        /// <param name="interpreter"></param>
        /// <param name="verticesOnly"></param>
        /// <param name="parameters"></param>
        public SearchClosestResult<TEdgeData> SearchClosest(IBasicRouterDataSource<TEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollectionBase pointTags, bool verticesOnly, Dictionary<string, object> parameters)
        {
            Meter distanceEpsilon = .1; // 10cm is the tolerance to distinguish points.

            var closestWithMatch = new SearchClosestResult<TEdgeData>(double.MaxValue, 0);
            var closestWithoutMatch = new SearchClosestResult<TEdgeData>(double.MaxValue, 0);

            double searchBoxSize = delta;
            // create the search box.
            var searchBox = new GeoCoordinateBox(new GeoCoordinate(
                coordinate.Latitude - searchBoxSize, coordinate.Longitude - searchBoxSize),
                                                               new GeoCoordinate(
                coordinate.Latitude + searchBoxSize, coordinate.Longitude + searchBoxSize));

            // get the arcs from the data source.
            var arcs = graph.GetEdges(searchBox);

            if (!verticesOnly)
            { // find both closest arcs and vertices.
                // loop over all.
                while(arcs.MoveNext())
                {
                    if (!graph.TagsIndex.Contains(arcs.EdgeData.Tags))
                    { // skip this edge, no valid tags found.
                        continue;
                    }
                    var arcTags = graph.TagsIndex.Get(arcs.EdgeData.Tags);
                    var canBeTraversed = vehicle.CanTraverse(arcTags);
                    if (canBeTraversed)
                    { // the edge can be traversed.
                        // test the two points.
                        float fromLatitude, fromLongitude;
                        float toLatitude, toLongitude;
                        double distance;
                        if (graph.GetVertex(arcs.Vertex1, out fromLatitude, out fromLongitude) &&
                            graph.GetVertex(arcs.Vertex2, out toLatitude, out toLongitude))
                        { // return the vertex.
                            var fromCoordinates = new GeoCoordinate(fromLatitude, fromLongitude);
                            distance = coordinate.DistanceReal(fromCoordinates).Value;
                            ICoordinateCollection coordinates;
                            ICoordinate[] coordinatesArray = null;
                            if (!graph.GetEdgeShape(arcs.Vertex1, arcs.Vertex2, out coordinates))
                            {
                                coordinates = null;
                            }
                            if (coordinates != null)
                            {
                                coordinatesArray = coordinates.ToArray();
                            }

                            if (distance < distanceEpsilon.Value)
                            { // the distance is smaller than the tolerance value.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arcs.Vertex1);
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arcs.Vertex1);
                                    break;
                                }
                            }

                            if (distance < closestWithoutMatch.Distance)
                            { // the distance is smaller for the without match.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arcs.Vertex1);
                            }
                            if (distance < closestWithMatch.Distance)
                            { // the distance is smaller for the with match.
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, graph.TagsIndex.Get(arcs.EdgeData.Tags)))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arcs.Vertex1);
                                }
                            }
                            var toCoordinates = new GeoCoordinate(toLatitude, toLongitude);
                            distance = coordinate.DistanceReal(toCoordinates).Value;

                            if (distance < closestWithoutMatch.Distance)
                            { // the distance is smaller for the without match.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arcs.Vertex2);
                            }
                            if (distance < closestWithMatch.Distance)
                            { // the distance is smaller for the with match.
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arcs.Vertex2);
                                }
                            }

                            // search along the line.
                            var distanceTotal = 0.0;
                            var previous = fromCoordinates;
                            var arcValueValueCoordinates = arcs.Intermediates;
                            if (arcValueValueCoordinates != null)
                            { // calculate distance along all coordinates.
                                var arcValueValueCoordinatesArray = arcValueValueCoordinates.ToArray();
                                for (int idx = 0; idx < arcValueValueCoordinatesArray.Length; idx++)
                                {
                                    var current = new GeoCoordinate(arcValueValueCoordinatesArray[idx].Latitude, arcValueValueCoordinatesArray[idx].Longitude);
                                    distanceTotal = distanceTotal + current.DistanceReal(previous).Value;
                                    previous = current;
                                }
                            }
                            distanceTotal = distanceTotal + toCoordinates.DistanceReal(previous).Value;
                            if (distanceTotal > 0)
                            { // the from/to are not the same location.
                                // loop over all edges that are represented by this arc (counting intermediate coordinates).
                                previous = fromCoordinates;
                                GeoCoordinateLine line;
                                var distanceToSegment = 0.0;
                                if (arcValueValueCoordinates != null)
                                {
                                    var arcValueValueCoordinatesArray = arcValueValueCoordinates.ToArray();
                                    for (int idx = 0; idx < arcValueValueCoordinatesArray.Length; idx++)
                                    {
                                        var current = new GeoCoordinate(
                                            arcValueValueCoordinatesArray[idx].Latitude, arcValueValueCoordinatesArray[idx].Longitude);
                                        line = new GeoCoordinateLine(previous, current, true, true);

                                        distance = line.DistanceReal(coordinate).Value;

                                        if (distance < closestWithoutMatch.Distance)
                                        { // the distance is smaller.
                                            var projectedPoint = line.ProjectOn(coordinate);

                                            // calculate the position.
                                            if (projectedPoint != null)
                                            { // calculate the distance
                                                var distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                                var position = distancePoint / distanceTotal;

                                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                                    distance, arcs.Vertex1, arcs.Vertex2, position, arcs.EdgeData, coordinatesArray);
                                            }
                                        }
                                        if (distance < closestWithMatch.Distance)
                                        {
                                            var projectedPoint = line.ProjectOn(coordinate);

                                            // calculate the position.
                                            if (projectedPoint != null)
                                            { // calculate the distance
                                                var distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                                var position = distancePoint / distanceTotal;

                                                if (matcher == null ||
                                                    (pointTags == null || pointTags.Count == 0) ||
                                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                                {

                                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                                        distance, arcs.Vertex1, arcs.Vertex2, position, arcs.EdgeData, coordinatesArray);
                                                }
                                            }
                                        }

                                        // add current segment distance to distanceToSegment for the next segment.
                                        distanceToSegment = distanceToSegment + line.LengthReal.Value;

                                        // set previous.
                                        previous = current;
                                    }
                                }

                                // check the last segment.
                                line = new GeoCoordinateLine(previous, toCoordinates, true, true);

                                distance = line.DistanceReal(coordinate).Value;

                                if (distance < closestWithoutMatch.Distance)
                                { // the distance is smaller.
                                    var projectedPoint = line.ProjectOn(coordinate);

                                    // calculate the position.
                                    if (projectedPoint != null)
                                    { // calculate the distance
                                        double distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                        double position = distancePoint / distanceTotal;

                                        closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                            distance, arcs.Vertex1, arcs.Vertex2, position, arcs.EdgeData, coordinatesArray);
                                    }
                                }
                                if (distance < closestWithMatch.Distance)
                                {
                                    var projectedPoint = line.ProjectOn(coordinate);

                                    // calculate the position.
                                    if (projectedPoint != null)
                                    { // calculate the distance
                                        double distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                        double position = distancePoint / distanceTotal;

                                        if (matcher == null ||
                                            (pointTags == null || pointTags.Count == 0) ||
                                            matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                        {

                                            closestWithMatch = new SearchClosestResult<TEdgeData>(
                                                distance, arcs.Vertex1, arcs.Vertex2, position, arcs.EdgeData, coordinatesArray);
                                        }
                                    }
                                }                               
                            }
                        }
                    }
                }
            }
            else
            { // only find closest vertices.
                // loop over all.
                while(arcs.MoveNext())
                {
                    float fromLatitude, fromLongitude;
                    float toLatitude, toLongitude;
                    if (graph.GetVertex(arcs.Vertex1, out fromLatitude, out fromLongitude) &&
                        graph.GetVertex(arcs.Vertex2, out toLatitude, out toLongitude))
                    {
                        var vertexCoordinate = new GeoCoordinate(fromLatitude, fromLongitude);
                        double distance = coordinate.DistanceReal(vertexCoordinate).Value;
                        if (distance < closestWithoutMatch.Distance)
                        { // the distance found is closer.
                            closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                distance, arcs.Vertex1);
                        }

                        vertexCoordinate = new GeoCoordinate(toLatitude, toLongitude);
                        distance = coordinate.DistanceReal(vertexCoordinate).Value;
                        if (distance < closestWithoutMatch.Distance)
                        { // the distance found is closer.
                            closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                distance, arcs.Vertex2);
                        }

                        var arcValueValueCoordinates = arcs.Intermediates;
                        if (arcValueValueCoordinates != null)
                        { // search over intermediate points.
                            var arcValueValueCoordinatesArray = arcValueValueCoordinates.ToArray();
                            for (int idx = 0; idx < arcValueValueCoordinatesArray.Length; idx++)
                            {
                                vertexCoordinate = new GeoCoordinate(
                                    arcValueValueCoordinatesArray[idx].Latitude,
                                    arcValueValueCoordinatesArray[idx].Longitude);
                                distance = coordinate.DistanceReal(vertexCoordinate).Value;
                                if (distance < closestWithoutMatch.Distance)
                                { // the distance found is closer.
                                    closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arcs.Vertex1, arcs.Vertex2, idx, arcs.EdgeData, arcValueValueCoordinatesArray);
                                }
                            }
                        }
                    }
                }
            }

            // return the best result.
            if (closestWithMatch.Distance < double.MaxValue)
            {
                return closestWithMatch;
            }
            return closestWithoutMatch;
        }

        #endregion
    }
}