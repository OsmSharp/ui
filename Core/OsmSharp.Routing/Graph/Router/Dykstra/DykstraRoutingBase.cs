// OsmSharp - OpenStreetMap tools & library.
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
using System.Linq;
using System.Text;
using OsmSharp.Tools.Collections.Tags;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.Graph.Router.Dykstra
{
    /// <summary>
    /// Contains generic fuctions common to all dykstra routers.
    /// </summary>
    public abstract class DykstraRoutingBase<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private readonly ITagsIndex _tagsIndex;

        /// <summary>
        /// Creates a new basic dykstra router.
        /// </summary>
        /// <param name="tagsIndex"></param>
        protected DykstraRoutingBase(ITagsIndex tagsIndex)
        {
            _tagsIndex = tagsIndex;
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        protected ITagsIndex TagsIndex
        {
            get
            {
                return _tagsIndex;
            }
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
        public SearchClosestResult SearchClosest(IBasicRouterDataSource<TEdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollection pointTags)
        {
            double searchBoxSize = delta;
            // create the search box.
            var searchBox = new GeoCoordinateBox(new GeoCoordinate(
                coordinate.Latitude - searchBoxSize, coordinate.Longitude - searchBoxSize),
                                                               new GeoCoordinate(
                coordinate.Latitude + searchBoxSize, coordinate.Longitude + searchBoxSize));

            // get the arcs from the data source.
            KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>[] arcs = graph.GetArcs(searchBox);

            // loop over all.
            var closestWithMatch = new SearchClosestResult(double.MaxValue, 0);
            var closestWithoutMatch = new SearchClosestResult(double.MaxValue, 0);
            foreach (KeyValuePair<uint, KeyValuePair<uint, TEdgeData>> arc in arcs)
            {
                TagsCollection arcTags = _tagsIndex.Get(arc.Value.Value.Tags);
                bool canBeTraversed = interpreter.EdgeInterpreter.CanBeTraversedBy(arcTags, vehicle);
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
                            closestWithoutMatch = new SearchClosestResult(
                                distance, arc.Key);
                            if (matcher == null ||
                                (pointTags == null || pointTags.Count == 0) ||
                                matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                            {
                                closestWithMatch = new SearchClosestResult(
                                    distance, arc.Key);
                                break;
                            }
                        }

                        if (distance < closestWithoutMatch.Distance)
                        { // the distance is smaller for the without match.
                            closestWithoutMatch = new SearchClosestResult(
                                distance, arc.Key);
                        }
                        if (distance < closestWithMatch.Distance)
                        { // the distance is smaller for the with match.
                            if (matcher == null ||
                                (pointTags == null || pointTags.Count == 0) ||
                                matcher.MatchWithEdge(vehicle, pointTags, _tagsIndex.Get(arc.Value.Value.Tags)))
                            {
                                closestWithMatch = new SearchClosestResult(
                                    distance, arc.Key);
                            }
                        }
                        var toCoordinates = new GeoCoordinate(toLatitude, toLongitude);
                        distance = coordinate.Distance(toCoordinates);

                        if (distance < closestWithoutMatch.Distance)
                        { // the distance is smaller for the without match.
                            closestWithoutMatch = new SearchClosestResult(
                                distance, arc.Value.Key);
                        }
                        if (distance < closestWithMatch.Distance)
                        { // the distance is smaller for the with match.
                            if (matcher == null ||
                                (pointTags == null || pointTags.Count == 0) ||
                                matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                            {
                                closestWithMatch = new SearchClosestResult(
                                    distance, arc.Value.Key);
                            }
                        }

                        // create a line.
                        double distanceTotal = fromCoordinates.Distance(toCoordinates);
                        if (distanceTotal > 0)
                        { // the from/to are not the same location.
                            var line = new GeoCoordinateLine(fromCoordinates, toCoordinates, true, true);
                            distance = line.Distance(coordinate);

                            if (distance < closestWithoutMatch.Distance)
                            { // the distance is smaller.
                                PointF2D projectedPoint =
                                    line.ProjectOn(coordinate);

                                // calculate the position.
                                if (projectedPoint != null)
                                { // calculate the distance
                                    double distancePoint = fromCoordinates.Distance(projectedPoint);
                                    double position = distancePoint / distanceTotal;

                                    closestWithoutMatch = new SearchClosestResult(
                                        distance, arc.Key, arc.Value.Key, position);
                                }
                            }
                            if (distance < closestWithMatch.Distance)
                            {
                                PointF2D projectedPoint =
                                    line.ProjectOn(coordinate);

                                // calculate the position.
                                if (projectedPoint != null)
                                { // calculate the distance
                                    double distancePoint = fromCoordinates.Distance(projectedPoint);
                                    double position = distancePoint / distanceTotal;

                                    if (matcher == null ||
                                        (pointTags == null || pointTags.Count == 0) ||
                                        matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                    {
                                        closestWithMatch = new SearchClosestResult(
                                            distance, arc.Key, arc.Value.Key, position);
                                    }
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