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
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Instructions;
using OsmSharp.Routing.Instructions.LanguageGeneration;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Units.Distance;
using OsmSharp.Units.Angle;

namespace OsmSharp.Routing.Navigation
{
    /// <summary>
    /// A tracker to track the progress along a route.
    /// </summary>
    public class RouteTracker
    {
        /// <summary>
        /// Holds the route to track.
        /// </summary>
        private readonly Route _route;

        /// <summary>
        /// Holds the instructions list.
        /// </summary>
        private readonly List<Instruction> _instructions;

        /// <summary>
        /// Creates a route tracker that tracks the given route and it's instructions.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="interpreter"></param>
        public RouteTracker(Route route, IRoutingInterpreter interpreter)
        {
            _route = route;
            _instructions = InstructionGenerator.Generate(route, interpreter);
        }

        /// <summary>
        /// Creates a route tracker that tracks the given route and it's instructions.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="interpreter"></param>
        /// <param name="languageGenerator"></param>
        public RouteTracker(Route route, IRoutingInterpreter interpreter, ILanguageGenerator languageGenerator)
        {
            _route = route;
            _instructions = InstructionGenerator.Generate(route, interpreter, languageGenerator);
        }

        /// <summary>
        /// Holds the current position along the given route. 
        /// </summary>
        private GeoCoordinate _currentRoutePosition;

        /// <summary>
        /// Holds the current position.
        /// </summary>
        private GeoCoordinate _currentPosition;

        /// <summary>
        /// Holds the index of the next instruction.
        /// </summary>
        private int _nextInstructionIdx = -1;

        /// <summary>
        /// Returns the position on the route closest to the current position.
        /// </summary>
        public GeoCoordinate PositionRoute
        {
            get
            {
                return _currentRoutePosition;
            }
        }

        /// <summary>
        /// Returns the current position.
        /// </summary>
        public GeoCoordinate Position
        {
            get
            {
                return _currentPosition;
            }
        }

        /// <summary>
        /// Returns the position on the route of the next instruction.
        /// </summary>
        public GeoCoordinate PositionNextInstruction
        {
            get
            {
                if (this.NextInstruction != null)
                { // the next instruction exists.
                    RoutePointEntry entry = _route.Entries[this.NextInstruction.EntryIdx];
                    if (entry != null)
                    { // entry found.
                        return new GeoCoordinate(entry.Latitude, entry.Longitude);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Holds the distance from the start location.
        /// </summary>
        private Meter _distanceFromStart;

        /// <summary>
        /// Returns the distance between the start position and the current position.
        /// </summary>
        public Meter DistanceFromStart
        {
            get
            {
                return _distanceFromStart;
            }
        }

        /// <summary>
        /// Returns the position after the given distance is travelled relative to the current position.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public GeoCoordinate PositionIn(Meter distance)
        {
            return _route.PositionAfter(_distanceFromStart + distance);
        }

        /// <summary>
        /// Holds the distance to the next instruction.
        /// </summary>
        private Meter _distanceNextInstruction;

        /// <summary>
        /// Returns the distance between the current position and the route.
        /// </summary>
        public Meter DistanceNextInstruction
        {
            get
            {
                return _distanceNextInstruction;
            }
        }

        /// <summary>
        /// Returns the next instruction.
        /// </summary>
        public Instruction NextInstruction
        {
            get
            {
                if (_nextInstructionIdx > 0 && _nextInstructionIdx < _instructions.Count)
                {
                    return _instructions[_nextInstructionIdx];
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the next instruction index.
        /// </summary>
        public int NextInstructionIdx
        {
            get
            {
                return _nextInstructionIdx;
            }
        }

        /// <summary>
        /// Returns the instruction list that 
        /// </summary>
        public List<Instruction> NextInstructionList
        {
            get
            {
                return _instructions;
            }
        }

        /// <summary>
        /// Updates the tracker with the given location.
        /// </summary>
        /// <param name="location">The measured location.</param>
        public void Track(GeoCoordinate location)
        {
            // project onto the route.
            KeyValuePair<int, GeoCoordinate> projectedResult = this.ProjectOn(_route, location);

            // set the current/route position.
            _currentPosition = location;
            _currentRoutePosition = projectedResult.Value;

            // find the next instruction.
            for (int instructionIdx = 0; instructionIdx < _instructions.Count; instructionIdx++)
            {
                Instruction instruction = _instructions[instructionIdx];
                if (instruction.EntryIdx >= projectedResult.Key)
                { // stop here!
                    _nextInstructionIdx = instructionIdx;
                    break;
                }
            }

            // calculate the distance to the next instruction.
            GeoCoordinate previous = (new GeoCoordinate(_route.Entries[projectedResult.Key].Latitude, 
                _route.Entries[projectedResult.Key].Longitude));
            Meter distance = previous.DistanceReal(projectedResult.Value);
            for (int idx = projectedResult.Key; idx < _instructions[_nextInstructionIdx].EntryIdx - 1; idx++)
            {
                GeoCoordinate next = (new GeoCoordinate(_route.Entries[idx + 1].Latitude, _route.Entries[idx + 1].Longitude));
                distance = distance + previous.DistanceReal(next);
                previous = next;
            }
            _distanceNextInstruction = distance;

            // calculate the distance from start.
            previous = (new GeoCoordinate(_route.Entries[0].Latitude, _route.Entries[0].Longitude));
            distance = 0;
            for (int idx = 0; idx < projectedResult.Key - 1; idx++)
            {
                GeoCoordinate next = (new GeoCoordinate(_route.Entries[idx + 1].Latitude, _route.Entries[idx + 1].Longitude));
                distance = distance + previous.DistanceReal(next);
                previous = next;
            }
            distance = distance + previous.DistanceReal(projectedResult.Value);
            _distanceFromStart = distance;
        }

        /// <summary>
        /// Project on route and return the next entry index and coordinate.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private KeyValuePair<int, GeoCoordinate> ProjectOn(Route route, GeoCoordinate coordinates)
        {
            double distance = double.MaxValue;
            GeoCoordinate closest = null;
            int closestIdx = -1;
            List<GeoCoordinate> points = route.GetPoints();
            for (int idx = 0; idx < points.Count - 1; idx++)
            {
                GeoCoordinateLine line = new GeoCoordinateLine(points[idx], points[idx + 1], true, true);
				PointF2D projectedPoint = line.ProjectOn(coordinates);
				GeoCoordinate projected;
				double currentDistance;
				if (projectedPoint != null) {
					projected = new GeoCoordinate(projectedPoint[1], projectedPoint[0]);
					currentDistance = coordinates.Distance(projected);
					if (currentDistance < distance)
					{
						closest = projected;
						closestIdx = idx + 1;
						distance = currentDistance;
					}
				}
				projected = points[idx];
				currentDistance = coordinates.Distance(projected);
				if (currentDistance < distance)
				{
					closest = projected;
					closestIdx = idx;
					distance = currentDistance;
				}

            }
            return new KeyValuePair<int,GeoCoordinate>(closestIdx, closest);
        }
    }
}