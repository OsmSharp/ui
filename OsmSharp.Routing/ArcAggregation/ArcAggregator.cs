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

using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Meta;
using OsmSharp.Routing.ArcAggregation.Output;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Units.Distance;

namespace OsmSharp.Routing.ArcAggregation
{
    /// <summary>
    /// An arc aggregator.
    /// </summary>
    public class ArcAggregator
    {
        private AggregatedPoint previousPoint = null;
        private AggregatedArc previousArc = null;
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
        public AggregatedPoint Aggregate(Route route)
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
                this.Process(route, previous, current, next);

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
        private void Process(Route route, AggregatedRoutePoint previous, AggregatedRoutePoint current, 
            AggregatedRoutePoint next)
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
                    p.EntryIdx = current.EntryIndex;

                    if (current.Entry.Points != null)
                    {
                        foreach (RoutePoint routePoint in current.Entry.Points)
                        {
                            PointPoi poi = new PointPoi();
                            poi.Name = routePoint.Name;
                            poi.Tags = routePoint.Tags.ConvertTo();
                            poi.Location = new GeoCoordinate(routePoint.Latitude, routePoint.Longitude);
                            poi.Angle = null; // there is no previous point; no angle is specified.
                            p.Points.Add(poi);
                        }
                    }

                    previousPoint = p;
                }
                else
                { // test if point is significant.
                    AggregatedArc nextArc = this.CreateArcAndPoint(previous, current, next);

                    // test if the next point is significant.
                    if (previousArc == null)
                    { // this arc is always significant; it is the first arc.
                        previousPoint.Next = nextArc;
                        previousArc = nextArc;
                    }
                    else
                    { // there is a previous arc; a test can be done if the current point is significant.
                        if (this.IsSignificant(route.Vehicle, previousArc, nextArc))
                        { // the arc is significant; append it to the previous arc.
                            previousArc.Next.Next = nextArc;
                            previousArc = nextArc;
                            previousPoint = nextArc.Next;
                        }
                        else
                        { // if the arc is not significant compared to the previous one, the previous one can extend until the next point.
                            // THIS IS THE AGGREGATION STEP!

                            // add distance.
                            Meter distance_to_next = previousArc.Next.Location.DistanceReal(nextArc.Next.Location);
                            previousArc.Distance = previousArc.Distance + distance_to_next;

                            // set point.
                            previousArc.Next = nextArc.Next;
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
        private bool IsSignificant(Vehicle vehicle, AggregatedArc previous_arc, AggregatedArc next_arc)
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
            var previousTagsDic = new TagsCollection();
            if (previous_arc.Tags != null)
            {
                foreach (Tag pair in previous_arc.Tags)
                {
                    previousTagsDic.Add(pair.Key, pair.Value);
                }
            }
            var nextTagsDic = new TagsCollection();
            if (next_arc.Tags != null)
            {
                foreach (Tag pair in next_arc.Tags)
                {
                    nextTagsDic.Add(pair.Key, pair.Value);
                }
            }
            if (!vehicle.IsEqualFor(previousTagsDic, nextTagsDic))
            { // the previous and the next edge do not represent a change for the given vehicle.
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
            a.Tags = current.Entry.Tags.ConvertToTagsCollection();
            if (previous != null)
            {
                GeoCoordinate previous_coordinate =
                    new GeoCoordinate(previous.Entry.Latitude, previous.Entry.Longitude);
                GeoCoordinate currentCoordinate = new GeoCoordinate(current.Entry.Latitude, current.Entry.Longitude);

                Meter distance = previous_coordinate.DistanceReal(currentCoordinate);
                a.Distance = distance;
            }


            // create the point.
            AggregatedPoint p = new AggregatedPoint();
            p.Location = new GeoCoordinate(current.Entry.Latitude, current.Entry.Longitude);
            p.Points = new List<PointPoi>();
            p.EntryIdx = current.EntryIndex;
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
                foreach (RoutePointEntrySideStreet sideStreet in current.Entry.SideStreets)
                {
                    AggregatedArc side = new AggregatedArc();
                    side.Name = sideStreet.WayName;
                    side.Names = sideStreet.WayNames.ConvertTo();
                    side.Tags = sideStreet.Tags.ConvertToTagsCollection();

                    RelativeDirection side_direction = null;
                    if (previous != null)
                    {
                        GeoCoordinate previous_coordinate =
                            new GeoCoordinate(previous.Entry.Latitude, previous.Entry.Longitude);
                        GeoCoordinate next_coordinate =
                            new GeoCoordinate(sideStreet.Latitude, sideStreet.Longitude);

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
        /// <summary>
        /// Holds the entry index.
        /// </summary>
        private int _entryIdx;

        /// <summary>
        /// Holds the route.
        /// </summary>
        private Route _route;

        /// <summary>
        /// Creates a new agregrated point enumerator.
        /// </summary>
        /// <param name="route"></param>
        public AggregatedPointEnumerator(Route route)
        {
            _entryIdx = -1;

            _route = route;
            _current = null;
        }

        #region IEnumerator<AggregatedRoutePoint> Members

        /// <summary>
        /// Holds the current point.
        /// </summary>
        private AggregatedRoutePoint _current;

        /// <summary>
        /// Returns the current point.
        /// </summary>
        public AggregatedRoutePoint Current
        {
            get 
            {
                if (_current == null)
                {
                    _current = new AggregatedRoutePoint();
                    _current.EntryIndex = _entryIdx;
                    _current.Entry = _route.Entries[_entryIdx];                    
                }
                return _current;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes all resources associated with this enumerator.
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Returns the current point.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Moves to the next point.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            _current = null;
            if (_route.Entries == null)
            {
                return false;
            }
            _entryIdx++;
            return _route.Entries.Length > _entryIdx;
        }

        /// <summary>
        /// Resets this enumerator.
        /// </summary>
        public void Reset()
        {
            _current = null;

            _entryIdx = -1;
        }

        #endregion
    }

    /// <summary>
    /// Represents an aggregated point.
    /// </summary>
    internal class AggregatedRoutePoint
    {
        /// <summary>
        /// Gets or sets the entry index.
        /// </summary>
        public int EntryIndex { get; set; }

        /// <summary>
        /// Gets or sets the route point entry.
        /// </summary>
        public RoutePointEntry Entry { get; set; }
    }
}