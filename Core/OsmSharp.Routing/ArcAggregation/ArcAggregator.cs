// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Routing.ArcAggregation.Output;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Units.Angle;
using OsmSharp.Tools.Math.Geo.Meta;
using OsmSharp.Tools.Math.Units.Distance;
using OsmSharp.Routing.Route;
using OsmSharp.Routing;
using OsmSharp.Routing.Interpreter;

namespace OsmSharp.Routing.ArcAggregation
{
    /// <summary>
    /// An arc aggregator.
    /// </summary>
    public class ArcAggregator
    {
        private AggregatedPoint previous_point = null;
        private AggregatedArc previous_arc = null;
        private AggregatedPoint p = null;

        /// <summary>
        /// Holds the routing interpreter.
        /// </summary>
        private IRoutingInterpreter _interpreter;

        /// <summary>
        /// Creates a new arc aggregator.
        /// </summary>
        /// <param name="interpreter"></param>
        public ArcAggregator(IRoutingInterpreter interpreter)
        {
            _interpreter = interpreter;
        }

        /// <summary>
        /// Aggregates a route by remove information useless to the generation of routing instructions.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public AggregatedPoint Aggregate(OsmSharpRoute route)
        {
            // create the enumerator.
            AggregatedPointEnumerator enumerator = new AggregatedPointEnumerator(route);

            AggregatedRoutePoint previous = null;
            AggregatedRoutePoint current = null;
            AggregatedRoutePoint next = null;

            // loop over all aggregated points.
            while (enumerator.MoveNext())
            {
                // get the next point.
                next = enumerator.Current;

                // process 
                this.Process(route ,previous, current, next);

                // make the next, current and the current previous.
                previous = current;
                current = next;
                next = null;
            }

            // process once more, the current current has not been processed.
            this.Process(route, previous, current, next);

            return p;
        }

        /// <summary>
        /// Processes a part of the route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="previous"></param>
        /// <param name="current"></param>
        /// <param name="next"></param>
        private void Process(OsmSharpRoute route, AggregatedRoutePoint previous, AggregatedRoutePoint current, AggregatedRoutePoint next)
        {
            // process the current point.
            if (current != null)
            {
                if (previous == null)
                { // point is always significant, it is the starting point!
                    // create point.
                    p = new AggregatedPoint();
                    p.Angle = null;
                    p.ArcsNotTaken = null;
                    p.Location = new GeoCoordinate(current.Entry.Latitude, current.Entry.Longitude);
                    p.Points = new List<PointPoi>();

                    if (current.Entry.Points != null)
                    {
                        foreach (RoutePoint route_point in current.Entry.Points)
                        {
                            PointPoi poi = new PointPoi();
                            poi.Name = route_point.Name;
                            poi.Tags = route_point.Tags.ConvertTo();
                            poi.Location = new GeoCoordinate(route_point.Latitude, route_point.Longitude);
                            poi.Angle = null; // there is no previous point; no angle is specified.
                            p.Points.Add(poi);
                        }
                    }

                    previous_point = p;
                }
                else
                { // test if point is significant.
                    AggregatedArc next_arc = this.CreateArcAndPoint(previous, current, next);

                    // test if the next point is significant.
                    if (previous_arc == null)
                    { // this arc is always significant; it is the first arc.
                        previous_point.Next = next_arc;
                        previous_arc = next_arc;
                    }
                    else
                    { // there is a previous arc; a test can be done if the current point is significant.
                        if (this.IsSignificant(route.Vehicle, previous_arc, next_arc))
                        { // the arc is significant; append it to the previous arc.
                            previous_arc.Next.Next = next_arc;
                            previous_arc = next_arc;
                            previous_point = next_arc.Next;
                        }
                        else
                        { // if the arc is not significant compared to the previous one, the previous one can extend until the next point.
                            // THIS IS THE AGGREGATION STEP!

                            // add distance.
                            Meter distance_to_next = previous_arc.Next.Location.DistanceReal(next_arc.Next.Location);
                            previous_arc.Distance = previous_arc.Distance + distance_to_next;

                            // set point.
                            previous_arc.Next = next_arc.Next;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the point between the two arcs represents a significant step in the route.
        /// </summary>
        /// <param name="previous_arc"></param>
        /// <param name="next_arc"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        private bool IsSignificant(VehicleEnum vehicle, AggregatedArc previous_arc, AggregatedArc next_arc)
        {
            if (previous_arc.Next.Points != null && previous_arc.Next.Points.Count > 0)
            { // the point has at least one important point.
                return true;
            }
            if (previous_arc.Next.ArcsNotTaken != null && previous_arc.Next.ArcsNotTaken.Count > 0)
            { // the point has at least one arc not taken.
                return true;
            }
            // create tag interpreters for arcs to try and work out if the arcs are different for the given vehicle.
            Dictionary<string, string> previous_tags_dic = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in previous_arc.Tags)
            {
                previous_tags_dic.Add(pair.Key, pair.Value);
            }
            Dictionary<string, string> next_tags_dic = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in next_arc.Tags)
            {
                next_tags_dic.Add(pair.Key, pair.Value);
            }
            if (!_interpreter.EdgeInterpreter.IsEqualFor(vehicle, previous_tags_dic, next_tags_dic))
            { // the previous and the next edge do not represent a change for the given vehicle.
                //RoadTagsInterpreterBase previous_interpreter = new RoadTagsInterpreterBase(previous_tags_dic);
                //RoadTagsInterpreterBase next_interpreter = new RoadTagsInterpreterBase(next_tags_dic);
                //if (!previous_interpreter.IsEqualForVehicle(vehicle, next_interpreter))
                //{
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates an arc and it's next point from the current aggregated point.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        internal AggregatedArc CreateArcAndPoint(AggregatedRoutePoint previous, AggregatedRoutePoint current, AggregatedRoutePoint next)
        {
            // create the arc.
            AggregatedArc a = new AggregatedArc();
            a.Name = current.Entry.WayFromName;
            a.Names = current.Entry.WayFromNames.ConvertTo();
            a.Tags = current.Entry.Tags.ConvertTo();
            if (previous != null)
            {
                GeoCoordinate previous_coordinate =
                    new GeoCoordinate(previous.Entry.Latitude, previous.Entry.Longitude);
                GeoCoordinate current_coordinate = new GeoCoordinate(current.Entry.Latitude, current.Entry.Longitude);

                Meter distance = previous_coordinate.DistanceReal(current_coordinate);
                a.Distance = distance;
            }


            // create the point.
            AggregatedPoint p = new AggregatedPoint();
            p.Location = new GeoCoordinate(current.Entry.Latitude, current.Entry.Longitude);
            p.Points = new List<PointPoi>();
            if (previous != null && next != null && next.Entry != null)
            {
                GeoCoordinate previous_coordinate =
                    new GeoCoordinate(previous.Entry.Latitude, previous.Entry.Longitude);
                GeoCoordinate next_coordinate =
                    new GeoCoordinate(next.Entry.Latitude, next.Entry.Longitude);

                p.Angle = RelativeDirectionCalculator.Calculate(previous_coordinate, p.Location, next_coordinate);
            }
            if (current.Entry.SideStreets != null && current.Entry.SideStreets.Length > 0)
            {
                p.ArcsNotTaken = new List<KeyValuePair<RelativeDirection, AggregatedArc>>();
                foreach (RoutePointEntrySideStreet side_street in current.Entry.SideStreets)
                {
                    AggregatedArc side = new AggregatedArc();
                    side.Name = side_street.WayName;
                    side.Names = side_street.WayNames.ConvertTo();
                    side.Tags = side_street.Tags.ConvertTo();

                    RelativeDirection side_direction = null;
                    if (previous != null)
                    {
                        GeoCoordinate previous_coordinate =
                            new GeoCoordinate(previous.Entry.Latitude, previous.Entry.Longitude);
                        GeoCoordinate next_coordinate =
                            new GeoCoordinate(side_street.Latitude, side_street.Longitude);

                        side_direction = RelativeDirectionCalculator.Calculate(previous_coordinate, p.Location, next_coordinate);
                    }

                    p.ArcsNotTaken.Add(new KeyValuePair<RelativeDirection, AggregatedArc>(side_direction, side));
                }
            }
            if (current.Entry.Points != null)
            {
                foreach (RoutePoint route_point in current.Entry.Points)
                {
                    PointPoi poi = new PointPoi();
                    poi.Name = route_point.Name;
                    poi.Tags = route_point.Tags.ConvertTo();
                    poi.Location = new GeoCoordinate(route_point.Latitude, route_point.Longitude);

                    GeoCoordinate previous_coordinate =
                        new GeoCoordinate(previous.Entry.Latitude, previous.Entry.Longitude);
                    GeoCoordinate current_coordinate = new GeoCoordinate(current.Entry.Latitude, current.Entry.Longitude);
                    poi.Angle = RelativeDirectionCalculator.Calculate(previous_coordinate, current_coordinate, poi.Location);

                    p.Points.Add(poi);
                }
            }

            // link the arc to the point.
            a.Next = p;

            return a;
        }
    }

    /// <summary>
    /// Enumerates all aggregated points.
    /// </summary>
    internal class AggregatedPointEnumerator : IEnumerator<AggregatedRoutePoint>
    {
        private int _idx;

        private OsmSharpRoute _route;

        public AggregatedPointEnumerator(OsmSharpRoute route)
        {
            _idx = -1;

            _route = route;
            _current = null;
        }

        #region IEnumerator<AggregatedRoutePoint> Members

        private AggregatedRoutePoint _current;

        public AggregatedRoutePoint Current
        {
            get 
            {
                if (_current == null)
                {
                    _current = new AggregatedRoutePoint();
                    _current.Entry = _route.Entries[_idx];                    
                }
                return _current;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            _current = null;
            if (_route.Entries == null)
            {
                return false;
            }
            _idx++;
            return _route.Entries.Length > _idx;
        }

        public void Reset()
        {
            _current = null;

            _idx = -1;
        }

        #endregion
    }

    /// <summary>
    /// Represents an aggregated point.
    /// </summary>
    internal class AggregatedRoutePoint
    {
        public RoutePointEntry Entry { get; set; }
    }
}
