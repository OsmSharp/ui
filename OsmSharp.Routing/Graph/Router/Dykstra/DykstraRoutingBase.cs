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
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Interpreter;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph.Router.Dykstra
{
    /// <summary>
    /// Contains generic fuctions common to all dykstra routers.
    /// </summary>
    public abstract class DykstraRoutingBase<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
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
        public SearchClosestResult<TEdgeData> SearchClosest(IBasicRouterDataSource<TEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollectionBase pointTags)
        {
            return this.SearchClosest(graph, interpreter, vehicle, coordinate, delta, matcher, pointTags, false);
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
        public SearchClosestResult<TEdgeData> SearchClosest(IBasicRouterDataSource<TEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollectionBase pointTags, bool verticesOnly)
        {
            var closestWithMatch = new SearchClosestResult<TEdgeData>(double.MaxValue, 0);
            var closestWithoutMatch = new SearchClosestResult<TEdgeData>(double.MaxValue, 0);

            double searchBoxSize = delta;
            // create the search box.
            var searchBox = new GeoCoordinateBox(new GeoCoordinate(
                coordinate.Latitude - searchBoxSize, coordinate.Longitude - searchBoxSize),
                                                               new GeoCoordinate(
                coordinate.Latitude + searchBoxSize, coordinate.Longitude + searchBoxSize));

            // get the arcs from the data source.
            KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>[] arcs = graph.GetArcs(searchBox);

            if (!verticesOnly)
            { // find both closest arcs and vertices.
                // loop over all.
                foreach (KeyValuePair<uint, KeyValuePair<uint, TEdgeData>> arc in arcs)
                {
                    TagsCollectionBase arcTags = graph.TagsIndex.Get(arc.Value.Value.Tags);
                    bool canBeTraversed = vehicle.CanTraverse(arcTags);
                    if (canBeTraversed)
                    { // the edge can be traversed.
                        // test the two points.
                        float fromLatitude, fromLongitude;
                        float toLatitude, toLongitude;
                        double distance;
                        if (graph.GetVertex(arc.Key, out fromLatitude, out fromLongitude) &&
                            graph.GetVertex(arc.Value.Key, out toLatitude, out toLongitude))
                        { // return the vertex.
                            var fromCoordinates = new GeoCoordinate(fromLatitude, fromLongitude);
                            distance = coordinate.Distance(fromCoordinates);

                            if (distance < 0.00001)
                            { // the distance is smaller than the tolerance value.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arc.Key);
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arc.Key);
                                    break;
                                }
                            }

                            if (distance < closestWithoutMatch.Distance)
                            { // the distance is smaller for the without match.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arc.Key);
                            }
                            if (distance < closestWithMatch.Distance)
                            { // the distance is smaller for the with match.
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, graph.TagsIndex.Get(arc.Value.Value.Tags)))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arc.Key);
                                }
                            }
                            var toCoordinates = new GeoCoordinate(toLatitude, toLongitude);
                            distance = coordinate.Distance(toCoordinates);

                            if (distance < closestWithoutMatch.Distance)
                            { // the distance is smaller for the without match.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arc.Value.Key);
                            }
                            if (distance < closestWithMatch.Distance)
                            { // the distance is smaller for the with match.
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arc.Value.Key);
                                }
                            }

                            // search along the line.
                            double distanceTotal = 0;
                            var previous = fromCoordinates;
                            if (arc.Value.Value.Coordinates != null)
                            { // calculate distance along all coordinates.
                                for(int idx = 0; idx < arc.Value.Value.Coordinates.Length; idx++)
                                {
                                    var current = new GeoCoordinate(arc.Value.Value.Coordinates[idx].Latitude, arc.Value.Value.Coordinates[idx].Longitude);
                                    distanceTotal = distanceTotal + current.Distance(previous);
                                    previous = current;
                                }
                            }
                            distanceTotal = distanceTotal + toCoordinates.Distance(previous);
                            if (distanceTotal > 0)
                            { // the from/to are not the same location.
                                // loop over all edges that are represented by this arc (counting intermediate coordinates).
                                previous = fromCoordinates;
                                GeoCoordinateLine line;
                                double distanceToSegment = 0;
                                if (arc.Value.Value.Coordinates != null)
                                {
                                    for (int idx = 0; idx < arc.Value.Value.Coordinates.Length; idx++)
                                    {
                                        var current = new GeoCoordinate(
                                            arc.Value.Value.Coordinates[idx].Latitude, arc.Value.Value.Coordinates[idx].Longitude);
                                        line = new GeoCoordinateLine(previous, current, true, true);

                                        distance = line.Distance(coordinate);

                                        if (distance < closestWithoutMatch.Distance)
                                        { // the distance is smaller.
                                            PointF2D projectedPoint =
                                                line.ProjectOn(coordinate);

                                            // calculate the position.
                                            if (projectedPoint != null)
                                            { // calculate the distance
                                                double distancePoint = previous.Distance(projectedPoint) + distanceToSegment;
                                                double position = distancePoint / distanceTotal;

                                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                                    distance, arc.Key, arc.Value.Key, position, arc.Value.Value);
                                            }
                                        }
                                        if (distance < closestWithMatch.Distance)
                                        {
                                            PointF2D projectedPoint =
                                                line.ProjectOn(coordinate);

                                            // calculate the position.
                                            if (projectedPoint != null)
                                            { // calculate the distance
                                                double distancePoint = previous.Distance(projectedPoint) + distanceToSegment;
                                                double position = distancePoint / distanceTotal;

                                                if (matcher == null ||
                                                    (pointTags == null || pointTags.Count == 0) ||
                                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                                {

                                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                                        distance, arc.Key, arc.Value.Key, position, arc.Value.Value);
                                                }
                                            }
                                        }

                                        // add current segment distance to distanceToSegment for the next segment.
                                        distanceToSegment = distanceToSegment + line.Length;

                                        // set previous.
                                        previous = current;
                                    }
                                }

                                // check the last segment.
                                line = new GeoCoordinateLine(previous, toCoordinates, true, true);

                                distance = line.Distance(coordinate);

                                if (distance < closestWithoutMatch.Distance)
                                { // the distance is smaller.
                                    PointF2D projectedPoint =
                                        line.ProjectOn(coordinate);

                                    // calculate the position.
                                    if (projectedPoint != null)
                                    { // calculate the distance
                                        double distancePoint = previous.Distance(projectedPoint) + distanceToSegment;
                                        double position = distancePoint / distanceTotal;

                                        closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                            distance, arc.Key, arc.Value.Key, position, arc.Value.Value);
                                    }
                                }
                                if (distance < closestWithMatch.Distance)
                                {
                                    PointF2D projectedPoint =
                                        line.ProjectOn(coordinate);

                                    // calculate the position.
                                    if (projectedPoint != null)
                                    { // calculate the distance
                                        double distancePoint = previous.Distance(projectedPoint) + distanceToSegment;
                                        double position = distancePoint / distanceTotal;

                                        if (matcher == null ||
                                            (pointTags == null || pointTags.Count == 0) ||
                                            matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                        {

                                            closestWithMatch = new SearchClosestResult<TEdgeData>(
                                                distance, arc.Key, arc.Value.Key, position, arc.Value.Value);
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
                foreach (KeyValuePair<uint, KeyValuePair<uint, TEdgeData>> arc in arcs)
                {
                    float fromLatitude, fromLongitude;
                    float toLatitude, toLongitude;
                    if (graph.GetVertex(arc.Key, out fromLatitude, out fromLongitude) &&
                        graph.GetVertex(arc.Value.Key, out toLatitude, out toLongitude))
                    {
                        var vertexCoordinate = new GeoCoordinate(fromLatitude, fromLongitude);
                        double distance = coordinate.Distance(vertexCoordinate);
                        if (distance < closestWithoutMatch.Distance)
                        { // the distance found is closer.
                            closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                distance, arc.Key);
                        }

                        vertexCoordinate = new GeoCoordinate(toLatitude, toLongitude);
                        distance = coordinate.Distance(vertexCoordinate);
                        if (distance < closestWithoutMatch.Distance)
                        { // the distance found is closer.
                            closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                distance, arc.Value.Key);
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