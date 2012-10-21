// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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

namespace Osm.Routing.HH.Primitives
{
    /// <summary>
    /// An highway vertex.
    /// </summary>
    public class HighwayVertex
    {
        /// <summary>
        /// Holds the id.
        /// </summary>
        public long Id { get; set; }

        ///// <summary>
        ///// Holds the highest level.
        ///// </summary>
        //public int Level { get; set; }

        ///// <summary>
        ///// Holds the highest core level.
        ///// </summary>
        //public int LevelCore { get; set; }

        ///// <summary>
        ///// Holds the highest contracted level.
        ///// </summary>
        //public int LevelContracted { get; set; }

        /// <summary>
        /// Holds the node's coordinates.
        /// </summary>
        public double[] Coordinate { get; set; }

        /// <summary>
        /// The tags of this vertex.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Holds all the edges.
        /// </summary>
        public HighwayEdge[] Edges { get; set; }

        #region Calculations

        /// <summary>
        /// Returns all the neighbours of this node.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public HashSet<HighwayVertexNeighbour> GetNeighbours(int level, HashSet<long> exceptions, bool core)
        {
            HashSet<HighwayVertexNeighbour> neighbours = new HashSet<HighwayVertexNeighbour>();
            // evaluate the neighbour.
            if (core)
            { // only return core edges.
                // loop over all edges.
                foreach (HighwayEdge edge in this.Edges)
                {
                    if (edge.Forward && 
                        edge.LevelCore >= level &&
                        edge.LevelContracted > level)
                    { // add the neighbour.
                        neighbours.Add(new HighwayVertexNeighbour()
                            {
                                VertexId = edge.VertexId,
                                Weight = edge.Weight
                            });
                    }
                }
            }
            else
            { // return non-core edges.
                // loop over all edges.
                foreach (HighwayEdge edge in this.Edges)
                {
                    if (edge.Forward &&
                        edge.Level >= level &&
                        edge.LevelContracted > level)
                    {
                        neighbours.Add(new HighwayVertexNeighbour()
                        {
                            VertexId = edge.VertexId,
                            Weight = edge.Weight
                        });
                    }
                }
            }
            return neighbours;
        }
        
        /// <summary>
        /// Returns all the neighbours of this node.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public HashSet<HighwayVertexNeighbour> GetNeighboursBackward(int level, HashSet<long> exceptions, bool core)
        {
            HashSet<HighwayVertexNeighbour> neighbours = new HashSet<HighwayVertexNeighbour>();
            // evaluate the neighbour.
            if (core)
            { // only return core edges.
                // loop over all edges.
                foreach (HighwayEdge edge in this.Edges)
                {
                    if (edge.Backward &&
                        edge.LevelCore >= level &&
                        edge.LevelContracted > level)
                    {
                        neighbours.Add(new HighwayVertexNeighbour()
                        {
                            VertexId = edge.VertexId,
                            Weight = edge.Weight
                        });
                    }
                }
            }
            else
            { // return non-core edges.
                // loop over all edges.
                foreach (HighwayEdge edge in this.Edges)
                {
                    if (edge.Backward &&
                        edge.Level >= level &&
                        edge.LevelContracted > level)
                    {
                        neighbours.Add(new HighwayVertexNeighbour()
                        {
                            VertexId = edge.VertexId,
                            Weight = edge.Weight
                        });
                    }
                }
            }
            return neighbours;
        }

        #endregion
    }

    /// <summary>
    /// An highway vertex neighbour.
    /// </summary>
    public class HighwayVertexNeighbour
    {
        /// <summary>
        /// The neighbour.
        /// </summary>
        public long VertexId { get; set; }

        /// <summary>
        /// The weight to the neighbour.
        /// </summary>
        public float Weight { get; set; }
    }
}
