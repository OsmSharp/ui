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

using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter;
using System.Collections.Generic;
using System;

namespace OsmSharp.Routing.Routers
{
    /// <summary>
    /// A version of the typedrouter using edges of type CHEdgeData.
    /// </summary>
    internal class TypedRouterCHEdge : TypedRouter<CHEdgeData>
    {
        /// <summary>
        /// Creates a new type router using edges of type CHEdgeData.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="router"></param>
        public TypedRouterCHEdge(IBasicRouterDataSource<CHEdgeData> graph, IRoutingInterpreter interpreter,
                           IBasicRouter<CHEdgeData> router)
            : base(graph, interpreter, router)
        {

        }

        /// <summary>
        /// Returns true if the given vehicle is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public override bool SupportsVehicle(Vehicle vehicle)
        {
            // TODO: ask interpreter.
            return true;
        }

        /// <summary>
        /// Returns all the arcs representing neighbours for the given vertex.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <returns></returns>
        protected override KeyValuePair<uint, CHEdgeData>[] GetNeighboursUndirected(long vertex1)
        {
            KeyValuePair<uint, CHEdgeData>[] arcs = this.Data.GetEdges(Convert.ToUInt32(vertex1));
            return arcs.KeepUncontracted();
        }
    }
}