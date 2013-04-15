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
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Osm;
//using OsmSharp.Tools.Math;

//namespace OsmSharp.Osm.Data.Core.Sparse.Primitives
//{
//    /// <summary>
//    /// A sparse vertex that is a crucial part of the road network.
//    /// </summary>
//    public class SparseVertex  : ILocationObject, IEquatable<SparseVertex>, ITaggedObject
//    {
//        /// <summary>
//        /// Creates a new vertex.
//        /// </summary>
//        public SparseVertex()
//        {
//            this.Tags = new List<KeyValuePair<string, string>>();
//            this.Coordinates = new double[2];
//        }

//        /// <summary>
//        /// Holds the id of the vertex.
//        /// </summary>
//        public long Id { get; set; }

//        /// <summary>
//        /// Holds the coordinates for this vertex.
//        /// </summary>
//        public double[] Coordinates { get; set; }

//        #region Neighbours

//        /// <summary>
//        /// Holds the actual neighbour nodes.
//        /// </summary>
//        public SparseVertexNeighbour[] Neighbours { get; set; }

//        #endregion

//        /// <summary>
//        /// Returns the SparseVertexNeighbour for the given id.
//        /// </summary>
//        /// <param name="neighbour_id"></param>
//        /// <returns></returns>
//        public SparseVertexNeighbour GetSparseVertexNeighbour(long neighbour_id)
//        {
//            SparseVertexNeighbour neighbour = null;
//            foreach (SparseVertexNeighbour current_neighbour in this.Neighbours)
//            {
//                if (current_neighbour.Id == neighbour_id)
//                {
//                    neighbour = current_neighbour;
//                    break;
//                }
//            }
//            return neighbour;
//        }

//        /// <summary>
//        /// Returns the SparseVertexNeighbour for the given id.
//        /// </summary>
//        /// <param name="neighbour_id"></param>
//        /// <returns></returns>
//        public SparseVertexNeighbour GetSparseVertexNeighbour(long neighbour_id, long in_between)
//        {
//            SparseVertexNeighbour neighbour = null;
//            foreach (SparseVertexNeighbour current_neighbour in this.Neighbours)
//            {
//                if (current_neighbour.Id == neighbour_id &&
//                    current_neighbour.Nodes.Contains<long>(in_between))
//                {
//                    neighbour = current_neighbour;
//                    break;
//                }
//            }
//            return neighbour;
//        }

//        /// <summary>
//        /// Returns the location of this point.
//        /// </summary>
//        public GeoCoordinate Location
//        {
//            get 
//            {
//                return new OsmSharp.Tools.Math.Geo.GeoCoordinate(
//                    this.Coordinates[0],
//                    this.Coordinates[1]);
//            }
//        }
        
//        /// <summary>
//        /// Returns true if the other is equal to this one.
//        /// </summary>
//        /// <param name="other"></param>
//        /// <returns></returns>
//        public bool Equals(SparseVertex other)
//        {
//            if (other == null)
//            {
//                return false;
//            }
//            return this.Id == other.Id;
//        }

//        /// <summary>
//        /// The tags of this vertex.
//        /// </summary>
//        public List<KeyValuePair<string, string>> Tags { get; private set; }
//    }

//    /// <summary>
//    /// A sparse vertex's neighbours.
//    /// </summary>
//    public class SparseVertexNeighbour
//    {
//        /// <summary>
//        /// The id.
//        /// </summary>
//        public long Id { get; set; }

//        /// <summary>
//        /// The weight.
//        /// </summary>
//        public double Weight { get; set; }

//        /// <summary>
//        /// The nodes in between.
//        /// </summary>
//        public long[] Nodes { get; set; }

//        /// <summary>
//        /// The neighbour is directed.
//        /// </summary>
//        public bool Directed { get; set; }

//        /// <summary>
//        /// The direction (true is along the direction of this neighbour).
//        /// </summary>
//        public bool Direction { get; set; }
        
//        /// <summary>
//        /// The actual tags.
//        /// </summary>
//        public IDictionary<string, string> Tags { get; set; }

//        ///// <summary>
//        ///// Add the given tags.
//        ///// </summary>
//        ///// <param name="tags_to_neighbour"></param>
//        //internal void AddTags(List<SparseVertexNeighbourTags> tags_to_neighbour)
//        //{
//        //    List<SparseVertexNeighbourTags> chosen_neighbours = new List<SparseVertexNeighbourTags>();
//        //    if (tags_to_neighbour != null &&
//        //        tags_to_neighbour.Count > 0)
//        //    {
//        //        SparseVertexNeighbourTags current = tags_to_neighbour[0];
//        //        chosen_neighbours.Add(current);
//        //        for (int idx = 1; idx < tags_to_neighbour.Count; idx++)
//        //        {
//        //            if (current.Tags == tags_to_neighbour[idx])
//        //            {
//        //                current = tags_to_neighbour[idx];
//        //                chosen_neighbours.Add(current);
//        //            }
//        //        }
//        //    }
//        //    this.Tags = chosen_neighbours.ToArray();
//        //}
//    }
//}
