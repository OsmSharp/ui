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
//using OsmSharp.Osm;
//using OsmSharp.Routing.Osm.Graphs;

//namespace OsmSharp.Routing.Osm.Graphs
//{
//    /// <summary>
//    /// Class representing a vertex that can be reached along an edge.
//    /// </summary>
//    /// <typeparam name="EdgeType"></typeparam>
//    /// <typeparam name="VertexType"></typeparam>
//    public class VertexAlongEdge
//    {
//        /// <summary>
//        /// Creates a new VertexAlongEdge
//        /// </summary>
//        /// <param name="point"></param>
//        public VertexAlongEdge(
//            Way way,
//            GraphVertex point,
//            float weight)
//        {
//            _vertex = point;
//            _edge = way;
//            _weight = weight;
//        }

//        /// <summary>
//        /// Holds the vertex.
//        /// </summary>
//        private GraphVertex _vertex;

//        /// <summary>
//        /// Returns the vertex.
//        /// </summary>
//        public GraphVertex Vertex
//        {
//            get
//            {
//                return _vertex;
//            }
//        }

//        /// <summary>
//        /// Holds the edge.
//        /// </summary>
//        private Way _edge;

//        /// <summary>
//        /// Returns the way.
//        /// </summary>
//        public Way Edge
//        {
//            get
//            {
//                return _edge;
//            }
//        }

//        /// <summary>
//        /// Holds the weight.
//        /// </summary>
//        private float _weight;

//        /// <summary>
//        /// Returns the weight.
//        /// </summary>
//        public float Weight
//        {
//            get
//            {
//                return _weight;
//            }
//        }

//        /// <summary>
//        /// Compares the current object with another object of the same type.
//        /// </summary>
//        /// <param name="other">An object to compare with this object.</param>
//        /// <returns>A 32-bit signed integer that indicates the relative order of the objects
//        ///     being compared. The return value has the following meanings: Value Meaning
//        ///     Less than zero This object is less than the other parameter.  Zero This object
//        ///     is equal to other. Greater than zero This object is greater than other.</returns>
//        public int CompareTo(VertexAlongEdge other)
//        {
//            return this.Weight.CompareTo(other.Weight);
//        }
//    }
//}
