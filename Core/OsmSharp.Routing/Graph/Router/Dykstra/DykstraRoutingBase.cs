using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph.DynamicGraph;
using OsmSharp.Routing.Router;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.Graph.Router.Dykstra
{
    /// <summary>
    /// Contains generic fuctions common to all dykstra routers.
    /// </summary>
    public abstract class DykstraRoutingBase<EdgeData>
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private ITagsIndex _tags_index;

        /// <summary>
        /// Creates a new basic dykstra router.
        /// </summary>
        /// <param name="tags_index"></param>
        protected DykstraRoutingBase(ITagsIndex tags_index)
        {
            _tags_index = tags_index;
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        protected ITagsIndex TagsIndex
        {
            get
            {
                return _tags_index;
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
        /// <param name="point_tags"></param>
        /// <param name="interpreter"></param>
        public SearchClosestResult SearchClosest(IBasicRouterDataSource<EdgeData> graph, IRoutingInterpreter interpreter, VehicleEnum vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, IDictionary<string, string> point_tags)
        {
            double search_box_size = delta;
            // create the search box.
            GeoCoordinateBox search_box = new GeoCoordinateBox(new GeoCoordinate(
                coordinate.Latitude - search_box_size, coordinate.Longitude - search_box_size),
                                                               new GeoCoordinate(
                coordinate.Latitude + search_box_size, coordinate.Longitude + search_box_size));

            // get the arcs from the data source.
            KeyValuePair<uint, KeyValuePair<uint, EdgeData>>[] arcs = graph.GetArcs(search_box);

            // loop over all.
            SearchClosestResult closest_with_match = new SearchClosestResult(double.MaxValue, 0);
            SearchClosestResult closest_without_match = new SearchClosestResult(double.MaxValue, 0);
            foreach (KeyValuePair<uint, KeyValuePair<uint, EdgeData>> arc in arcs)
            {
                IDictionary<string, string> arc_tags = _tags_index.Get(arc.Value.Value.Tags);
                bool can_be_traversed = interpreter.EdgeInterpreter.CanBeTraversedBy(arc_tags, vehicle);
                if (can_be_traversed)
                { // the edge can be traversed.
                    // test the two points.
                    float from_latitude, from_longitude;
                    float to_latitude, to_longitude;
                    double distance;
                    if (graph.GetVertex(arc.Key, out from_latitude, out from_longitude) &&
                        graph.GetVertex(arc.Value.Key, out to_latitude, out to_longitude))
                    { // return the vertex.
                        GeoCoordinate from_coordinates = new GeoCoordinate(from_latitude, from_longitude);
                        distance = coordinate.Distance(from_coordinates);

                        if (distance < 0.00001)
                        { // the distance is smaller than the tolerance value.
                            closest_without_match = new SearchClosestResult(
                                distance, arc.Key);
                            if (matcher == null ||
                                (point_tags == null || point_tags.Count == 0) ||
                                matcher.MatchWithEdge(vehicle, point_tags, arc_tags))
                            {
                                closest_with_match = new SearchClosestResult(
                                    distance, arc.Key);
                                break;
                            }
                        }

                        if (distance < closest_without_match.Distance)
                        { // the distance is smaller for the without match.
                            closest_without_match = new SearchClosestResult(
                                distance, arc.Key);
                        }
                        if (distance < closest_with_match.Distance)
                        { // the distance is smaller for the with match.
                            if (matcher == null ||
                                (point_tags == null || point_tags.Count == 0) ||
                                matcher.MatchWithEdge(vehicle, point_tags, _tags_index.Get(arc.Value.Value.Tags)))
                            {
                                closest_with_match = new SearchClosestResult(
                                    distance, arc.Key);
                            }
                        }
                        GeoCoordinate to_coordinates = new GeoCoordinate(to_latitude, to_longitude);
                        distance = coordinate.Distance(to_coordinates);

                        if (distance < closest_without_match.Distance)
                        { // the distance is smaller for the without match.
                            closest_without_match = new SearchClosestResult(
                                distance, arc.Value.Key);
                        }
                        if (distance < closest_with_match.Distance)
                        { // the distance is smaller for the with match.
                            if (matcher == null ||
                                (point_tags == null || point_tags.Count == 0) ||
                                matcher.MatchWithEdge(vehicle, point_tags, arc_tags))
                            {
                                closest_with_match = new SearchClosestResult(
                                    distance, arc.Value.Key);
                            }
                        }

                        // create a line.
                        double distance_total = from_coordinates.Distance(to_coordinates);
                        if (distance_total > 0)
                        { // the from/to are not the same location.
                            GeoCoordinateLine line = new GeoCoordinateLine(from_coordinates, to_coordinates, true, true);
                            distance = line.Distance(coordinate);

                            if (distance < closest_without_match.Distance)
                            { // the distance is smaller.
                                PointF2D projected_point =
                                    line.ProjectOn(coordinate);

                                // calculate the position.
                                if (projected_point != null)
                                { // calculate the distance
                                    double distance_point = from_coordinates.Distance(projected_point);
                                    double position = distance_point / distance_total;

                                    closest_without_match = new SearchClosestResult(
                                        distance, arc.Key, arc.Value.Key, position);
                                }
                            }
                            if (distance < closest_with_match.Distance)
                            {
                                PointF2D projected_point =
                                    line.ProjectOn(coordinate);

                                // calculate the position.
                                if (projected_point != null)
                                { // calculate the distance
                                    double distance_point = from_coordinates.Distance(projected_point);
                                    double position = distance_point / distance_total;

                                    if (matcher == null ||
                                        (point_tags == null || point_tags.Count == 0) ||
                                        matcher.MatchWithEdge(vehicle, point_tags, arc_tags))
                                    {
                                        closest_with_match = new SearchClosestResult(
                                            distance, arc.Key, arc.Value.Key, position);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // return the best result.
            if (closest_with_match.Distance < double.MaxValue)
            {
                return closest_with_match;
            }
            return closest_without_match;
        }

        #endregion
    }
}
