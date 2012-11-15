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
using OsmSharp.Routing.Core.Graph;

namespace OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering
{
    /// <summary>
    /// Orders the vertices putting the sparse vertices first, any other float.MaxValue.
    /// 
    /// This should result in a sparser graph without nodes with exactly 2 neighbours.
    /// </summary>
    public class SparseOrdering : INodeWeightCalculator
    {
        /// <summary>
        /// Holds the data source.
        /// </summary>
        private IDynamicGraph<CHEdgeData> _data;

        /// <summary>
        /// Creates a new sparse ordering calculator.
        /// </summary>
        /// <param name="data"></param>
        public SparseOrdering(IDynamicGraph<CHEdgeData> data)
        {
            _data = data;
        }
        /// <summary>
        /// Calculates the ordering.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public float Calculate(uint vertex)
        {
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(vertex);

            Dictionary<uint, KeyValuePair<bool, bool>> directions_per_neighbour = new Dictionary<uint, KeyValuePair<bool, bool>>();
            List<uint> neighbours_list = new List<uint>();
            foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
            {
                // get the existing direction.
                KeyValuePair<bool, bool> directions;
                if (!directions_per_neighbour.TryGetValue(neighbour.Key, out directions))
                {
                    directions = new KeyValuePair<bool, bool>(false, false);
                }

                // create the new directions.
                directions = new KeyValuePair<bool,bool>(directions.Key || neighbour.Value.Forward,
                    directions.Value || neighbour.Value.Backward);

                // keep the direction settings.
                directions_per_neighbour[neighbour.Key] = directions;

                // if there are more than two return.
                if (directions_per_neighbour.Count > 2)
                { // it is impossible that this is sparse.
                    return float.MaxValue;
                }

                // add to the list.
                if(!neighbours_list.Contains(neighbour.Key))
                {
                    neighbours_list.Add(neighbour.Key);
                }
            }


            // check the proper conditions.
            if (neighbours_list.Count == 2)
            {
                // check if one of the neighbours is only forward.
                foreach (uint neighbour in neighbours_list)
                {
                    KeyValuePair<bool, bool> direction_for_neighbour;
                    if (directions_per_neighbour.TryGetValue(neighbour, out direction_for_neighbour))
                    { // directions exist.
                        if (direction_for_neighbour.Key && !direction_for_neighbour.Value)
                        { // the neighbour has only a forward connection check at it's neighbour if there is a connection back.
                            KeyValuePair<uint, CHEdgeData>[] neighbour_neigbours = _data.GetArcs(neighbour);
                            foreach (KeyValuePair<uint, CHEdgeData> neighbour_neigbour in neighbour_neigbours)
                            {
                                if (neighbour_neigbour.Key == vertex &&
                                    neighbour_neigbour.Value.Forward)
                                { // there is also a backward direction.
                                    directions_per_neighbour[neighbour] = new KeyValuePair<bool, bool>(true, true);
                                }
                            }
                        }
                    }
                }

                KeyValuePair<bool, bool> directions1 = directions_per_neighbour[neighbours_list[0]];
                KeyValuePair<bool, bool> directions2 = directions_per_neighbour[neighbours_list[1]];

                if (directions1.Key && directions2.Key && directions1.Value && directions2.Value)
                { // this one can be contracted.
                    return -1;
                }
                else if (directions1.Key && directions2.Key && !directions1.Value && !directions2.Value)
                { // this one can be contracted.
                    return -1;
                }
                else if (!directions1.Key && !directions2.Key && directions1.Value && directions2.Value)
                { // this one can be contracted.
                    return -1;
                }
            }
            return float.MaxValue;
        }

        /// <summary>
        /// Do nothing with this here!
        /// </summary>
        /// <param name="vertex"></param>
        public void NotifyContracted(uint vertex)
        {

        }
    }
}