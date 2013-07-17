//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Routing;
//using OsmSharp.Routing.Roads.Tags;

//namespace OsmSharp.Routing.Roads
//{
//    /// <summary>
//    /// Inteprets the properties of a way and abstracts the usage of tags in OsmSharp.Osm.
//    /// </summary>
//    public class RoadInterpreter
//    {
//        /// <summary>
//        /// The way being interpreted.
//        /// </summary>
//        private Way _way;

//        /// <summary>
//        /// Creates a new interpreter for the given way.
//        /// </summary>
//        /// <param name="way"></param>
//        public RoadInterpreter(Way way)
//        {
//            _way = way;
//        }

//        /// <summary>
//        /// Returns the way this interpreter is for.
//        /// </summary>
//        public Way Way
//        {
//            get
//            {
//                return _way;
//            }
//        }

//        /// <summary>
//        /// Creates a tags interpreter.
//        /// </summary>
//        /// <param name="tags"></param>
//        /// <returns></returns>
//        protected RoadTagsInterpreterBase CreateTagsInterpreter(IDictionary<string, string> tags)
//        {
//            return new RoadTagsInterpreterBase(tags);
//        }

//        private RoadTagsInterpreterBase _tags_interpreter;

//        public RoadTagsInterpreterBase TagsInterpreter
//        {
//            get
//            {
//                if (_tags_interpreter == null)
//                {
//                    _tags_interpreter = this.CreateTagsInterpreter(_way.Tags);
//                }
//                return _tags_interpreter;
//            }
//        }


//        /// <summary>
//        /// Returns true if this way can be travelled by the given vehicle given two ajunct nodes.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public bool CanBeTravelledByAlong(VehicleEnum vehicle, Node from, Node to)
//        {
//            // return false always when this way cannot be travelled by the vehicle in any direction.
//            if (!this.TagsInterpreter.CanBeTravelledBy(vehicle))
//            {
//                return false;
//            }

//            // test the one way direction if needed.
//            if (this.TagsInterpreter.IsOneWay())
//            {
//                if (vehicle != VehicleEnum.Pedestrian)
//                { // TODO: include support for bike and one-way streets supporting bike traffic in both directions.
//                    bool order = true;

//                    // try until the nodes are found.
//                    int from_idx = this.Way.Nodes.IndexOf(from);
//                    while (from_idx > 0)
//                    {
//                        if (from_idx > 0 && this.Way.Nodes[from_idx - 1] == to)
//                        {
//                            order = false;
//                            break;
//                        }
//                        else if (from_idx < this.Way.Nodes.Count - 1 && this.Way.Nodes[from_idx + 1] == to)
//                        {
//                            break;
//                        }
//                        else
//                        {
//                            from_idx = this.Way.Nodes.IndexOf(from, from_idx + 1);
//                        }
//                    }

//                    // check if the nodes are found as neighbours.
//                    if (from_idx < 0)
//                    {
//                        throw new Exception("CanBeTraversed can only be applied to neigbouring nodes!");
//                    }

//                    // the nodes have been found; check oneway property.
//                    if (!this.TagsInterpreter.IsOneWayReverse())
//                    {
//                        return order;
//                    }

//                    return !order;
//                }
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns true if, for the given vehicle, the other road is equal to this one.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        /// <param name="other"></param>
//        /// <returns></returns>
//        public bool IsEqualForVehicle(VehicleEnum vehicle, RoadInterpreter other)
//        {
//            if (other.Way != this.Way)
//            { // road are different, checks need to be done.
//                this.TagsInterpreter.IsEqualForVehicle(vehicle, other.TagsInterpreter);
//            }
//            return true;
//        }
//    }
//}
