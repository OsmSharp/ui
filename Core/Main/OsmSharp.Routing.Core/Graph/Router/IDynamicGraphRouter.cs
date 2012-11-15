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
using OsmSharp.Routing.Core.Graph.Path;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Tools.Math;

namespace OsmSharp.Routing.Core.Graph.Router
{
    /// <summary>
    /// Abstract a router that works on a dynamic graph.
    /// </summary>
    public interface IDynamicGraphRouter<EdgeData>
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Calculates a shortest path between two given vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        PathSegment<long> Calculate(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, PathSegmentVisitList target, double max);


        /// <summary>
        /// Calculates the weight of the shortest path between two given vertices.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        double CalculateWeight(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, PathSegmentVisitList target, double max);

        /// <summary>
        /// Calculates a shortest path between the source vertex and any of the targets and returns the shortest.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        PathSegment<long> CalculateToClosest(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max);

        /// <summary>
        /// Calculates all routes from a given source to all given targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        double[] CalculateOneToManyWeight(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max);

        /// <summary>
        /// Calculates all routes from a given sources to all given targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        double[][] CalculateManyToManyWeight(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double max);

        /// <summary>
        /// Returns true if range calculation is supported.
        /// </summary>
        bool IsCalculateRangeSupported { get; }

        /// <summary>
        /// Calculates all points that are at or close to the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        HashSet<long> CalculateRange(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, double weight);

        /// <summary>
        /// Returns true if the search can move beyond the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        bool CheckConnectivity(IDynamicGraphReadOnly<EdgeData> graph, IRoutingInterpreter interpreter,
            PathSegmentVisitList source, double weight);
        }
}