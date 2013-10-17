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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph;

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
        /// <param name="vertex"></param>
        /// <returns></returns>
        public float Calculate(uint vertex)
        {
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(vertex);

            // check the proper conditions.
            if (neighbours.Length == 2)
            {
                return -1;
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