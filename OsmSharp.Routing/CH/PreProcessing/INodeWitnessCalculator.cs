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
using OsmSharp.Routing.Graph.Router;

namespace OsmSharp.Routing.CH.PreProcessing
{
    /// <summary>
    /// A witness calculator.
    /// </summary>
    public interface INodeWitnessCalculator
    {
        /// <summary>
        /// Return true if a witness exists for the given graph vertex 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxWeight"></param>
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        bool Exists(IBasicRouterDataSource<CHEdgeData> graph, uint from, uint to, float maxWeight, int maxSettles);

        /// <summary>
        /// Calculates all witnesses from one source to multiple targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        /// <param name="tosWeights"></param>
        /// <param name="maxSettles"></param>
        /// <param name="exists"></param>
        void Exists(IBasicRouterDataSource<CHEdgeData> graph, uint from, List<uint> tos, List<float> tosWeights, int maxSettles, ref bool[] exists);
    }
}