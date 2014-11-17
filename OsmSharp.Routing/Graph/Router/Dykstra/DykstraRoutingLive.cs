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

using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Collections.Tags;
using OsmSharp.Logging;
using OsmSharp.Routing.Constraints;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.Graph.Router.Dykstra
{
    /// <summary>
    /// A class containing a dykstra implementation suitable for a simple graph.
    /// </summary>
    public class DykstraRoutingLive : DykstraRoutingBase<LiveEdge>, IBasicRouter<LiveEdge>
    {
        /// <summary>
        /// Creates a new dykstra routing object.
        /// </summary>
        public DykstraRoutingLive()
        {

        }

        /// <summary>
        /// Gets the weight type.
        /// </summary>
        public RouterWeightType WeightType
        {
            get
            {
                return RouterWeightType.Time;
            }
        }

        /// <summary>
        /// Calculates the shortest path from the given vertex to the given vertex given the weights in the graph.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="max"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public PathSegment<long> Calculate(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, PathSegmentVisitList from, PathSegmentVisitList to, double max, Dictionary<string, object> parameters)
        {
            return this.CalculateToClosest(graph, interpreter, vehicle, from,
                new PathSegmentVisitList[] { to }, max, null);
        }

        /// <summary>
        /// Calculates the shortest path from all sources to all targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="maxSearch"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public PathSegment<long>[][] CalculateManyToMany(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double maxSearch, Dictionary<string, object> parameters)
        {
            var results = new PathSegment<long>[sources.Length][];
            for (int sourceIdx = 0; sourceIdx < sources.Length; sourceIdx++)
            {
                results[sourceIdx] = this.DoCalculation(graph, interpreter, vehicle,
                   sources[sourceIdx], targets, maxSearch, false, false, parameters);
            }
            return results;
        }

        /// <summary>
        /// Calculates the shortest path from the given vertex to the given vertex given the weights in the graph.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="max"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public double CalculateWeight(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            PathSegmentVisitList from, PathSegmentVisitList to, double max, Dictionary<string, object> parameters)
        {
            PathSegment<long> closest = this.CalculateToClosest(graph, interpreter, vehicle, from,
                new PathSegmentVisitList[] { to }, max, null);
            if (closest != null)
            {
                return closest.Weight;
            }
            return double.MaxValue;
        }

        /// <summary>
        /// Calculates a shortest path between the source vertex and any of the targets and returns the shortest.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public PathSegment<long> CalculateToClosest(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, PathSegmentVisitList from, PathSegmentVisitList[] targets, double max, Dictionary<string, object> parameters)
        {
            PathSegment<long>[] result = this.DoCalculation(graph, interpreter, vehicle,
                from, targets, max, false, false, parameters);
            if (result != null && result.Length == 1)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// Calculates all routes from a given source to all given targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public double[] CalculateOneToManyWeight(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            PathSegmentVisitList source, PathSegmentVisitList[] targets, double max, Dictionary<string, object> parameters)
        {
            PathSegment<long>[] many = this.DoCalculation(graph, interpreter, vehicle,
                   source, targets, max, false, false, null);

            var weights = new double[many.Length];
            for (int idx = 0; idx < many.Length; idx++)
            {
                if (many[idx] != null)
                {
                    weights[idx] = many[idx].Weight;
                }
                else
                {
                    weights[idx] = double.MaxValue;
                }
            }
            return weights;
        }

        /// <summary>
        /// Calculates all routes from a given sources to all given targets.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public double[][] CalculateManyToManyWeight(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, PathSegmentVisitList[] sources, PathSegmentVisitList[] targets, double max, Dictionary<string, object> parameters)
        {
            var results = new double[sources.Length][];
            for (int idx = 0; idx < sources.Length; idx++)
            {
                results[idx] = this.CalculateOneToManyWeight(graph, interpreter, vehicle, sources[idx], targets, max, null);

                OsmSharp.Logging.Log.TraceEvent("DykstraRoutingLive", TraceEventType.Information, "Calculating weights... {0}%",
                    (int)(((float)idx / (float)sources.Length) * 100));
            }
            return results;
        }

        /// <summary>
        /// Returns true, range calculation is supported.
        /// </summary>
        public bool IsCalculateRangeSupported
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// Calculates all points that are at or close to the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HashSet<long> CalculateRange(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, PathSegmentVisitList source, double weight, Dictionary<string, object> parameters)
        {
            return this.CalculateRange(graph, interpreter, vehicle, source, weight, true, null);
        }

        /// <summary>
        /// Calculates all points that are at or close to the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <param name="forward"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HashSet<long> CalculateRange(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, PathSegmentVisitList source, double weight, bool forward, Dictionary<string, object> parameters)
        {
            PathSegment<long>[] result = this.DoCalculation(graph, interpreter, vehicle,
                   source, new PathSegmentVisitList[0], weight, false, true, forward, parameters);

            var resultVertices = new HashSet<long>();
            for (int idx = 0; idx < result.Length; idx++)
            {
                resultVertices.Add(result[idx].VertexId);
            }
            return resultVertices;
        }

        /// <summary>
        /// Returns true if the search can move beyond the given weight.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="weight"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool CheckConnectivity(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            PathSegmentVisitList source, double weight, Dictionary<string, object> parameters)
        {
            HashSet<long> range = this.CalculateRange(graph, interpreter, vehicle, source, weight, true, null);

            if (range.Count > 0)
            {
                range = this.CalculateRange(graph, interpreter, vehicle, source, weight, false, null);
                if (range.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        #region Implementation

        /// <summary>
        /// Does forward dykstra calculation(s) with several options.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="weight"></param>
        /// <param name="stopAtFirst"></param>
        /// <param name="returnAtWeight"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private PathSegment<long>[] DoCalculation(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter, 
            Vehicle vehicle, PathSegmentVisitList source, PathSegmentVisitList[] targets, double weight,
            bool stopAtFirst, bool returnAtWeight, Dictionary<string, object> parameters)
        {
            return this.DoCalculation(graph, interpreter, vehicle, source, targets, weight, stopAtFirst, returnAtWeight, true, parameters);
        }

        /// <summary>
        /// Does dykstra calculation(s) with several options.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="sourceList"></param>
        /// <param name="targetList"></param>
        /// <param name="weight"></param>
        /// <param name="stopAtFirst"></param>
        /// <param name="returnAtWeight"></param>
        /// <param name="forward"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private PathSegment<long>[] DoCalculation(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            PathSegmentVisitList sourceList, PathSegmentVisitList[] targetList, double weight,
            bool stopAtFirst, bool returnAtWeight, bool forward, Dictionary<string, object> parameters)
        {
            var speeds = new Dictionary<uint, Speed>();

            // make copies of the target and source visitlist.
            var source = sourceList.Clone() as PathSegmentVisitList;
            var targets = new PathSegmentVisitList[targetList.Length];
            var targetsCount = new int[targetList.Length];
            for (int targetIdx = 0; targetIdx < targetList.Length; targetIdx++)
            {
                targets[targetIdx] = targetList[targetIdx].Clone() as PathSegmentVisitList;
                targetsCount[targetIdx] = targetList[targetIdx].Count;
            }

            //  initialize the result data structures.
            var segmentsAtWeight = new List<PathSegment<long>>();
            var segmentsToTarget = new PathSegment<long>[targets.Length]; // the resulting target segments.

            // intialize dykstra data structures.
            var heap = new BinairyHeap<PathSegment<long>>(100);
            var visitList = new DykstraVisitList();
            var labels = new Dictionary<long, IList<RoutingLabel>>();
            foreach (long vertex in source.GetVertices())
            {
                labels[vertex] = new List<RoutingLabel>();

                PathSegment<long> path = source.GetPathTo(vertex);
                heap.Push(path, (float)path.Weight);
            }

            // set the from node as the current node and put it in the correct data structures.
            // initialize the source's neighbors.
            PathSegment<long> current = heap.Pop();
            while (current != null &&
                visitList.HasBeenVisited(current))
            { // keep dequeuing.
                current = heap.Pop();
            }

            // test each target for the source.
            // test each source for any of the targets.
            var pathsFromSource = new Dictionary<long, PathSegment<long>>();
            foreach (long sourceVertex in source.GetVertices())
            { // get the path to the vertex.
                PathSegment<long> sourcePath = source.GetPathTo(sourceVertex); // get the source path.
                sourcePath = sourcePath.From;
                while (sourcePath != null)
                { // add the path to the paths from source.
                    pathsFromSource[sourcePath.VertexId] = sourcePath;
                    sourcePath = sourcePath.From;
                }
            }
            // loop over all targets
            for (int idx = 0; idx < targets.Length; idx++)
            { // check for each target if there are paths to the source.
                foreach (long targetVertex in new List<long>(targets[idx].GetVertices()))
                {
                    PathSegment<long> targetPath = targets[idx].GetPathTo(targetVertex); // get the target path.
                    targetPath = targetPath.From;
                    while (targetPath != null)
                    { // add the path to the paths from source.
                        PathSegment<long> pathFromSource;
                        if (pathsFromSource.TryGetValue(targetPath.VertexId, out pathFromSource))
                        { // a path is found.
                            // get the existing path if any.
                            PathSegment<long> existing = segmentsToTarget[idx];
                            if (existing == null)
                            { // a path did not exist yet!
                                segmentsToTarget[idx] = targetPath.Reverse().ConcatenateAfter(pathFromSource);
                                targets[idx].Remove(targetVertex);
                            }
                            else if (existing.Weight > targetPath.Weight + pathFromSource.Weight)
                            { // a new path is found with a lower weight.
                                segmentsToTarget[idx] = targetPath.Reverse().ConcatenateAfter(pathFromSource);
                            }
                        }
                        targetPath = targetPath.From;
                    }
                }
            }
            if (targets.Length > 0 && targets.All(x => x.Count == 0))
            { // routing is finished!
                return segmentsToTarget.ToArray();
            }

            if (stopAtFirst)
            { // only one entry is needed.
                var oneFound = false;
                for (int idx = 0; idx < targets.Length; idx++)
                {
                    if(targets[idx].Count < targetsCount[idx])
                    {
                        oneFound = true;
                        break;
                    }
                }

                if (oneFound)
                { // targets found, return the shortest!
                    PathSegment<long> shortest = null;
                    foreach (PathSegment<long> foundTarget in segmentsToTarget)
                    {
                        if (shortest == null)
                        {
                            shortest = foundTarget;
                        }
                        else if (foundTarget != null &&
                            shortest.Weight > foundTarget.Weight)
                        {
                            shortest = foundTarget;
                        }
                    }
                    segmentsToTarget = new PathSegment<long>[1];
                    segmentsToTarget[0] = shortest;
                    return segmentsToTarget;
                }
                else
                { // not targets found yet!
                    segmentsToTarget = new PathSegment<long>[1];
                }
            }

            // test for identical start/end point.
            for (int idx = 0; idx < targets.Length; idx++)
            {
                var target = targets[idx];
                if (returnAtWeight)
                { // add all the reached vertices larger than weight to the results.
                    if (current.Weight > weight)
                    {
                        var toPath = target.GetPathTo(current.VertexId);
                        toPath.Reverse();
                        toPath = toPath.ConcatenateAfter(current);
                        segmentsAtWeight.Add(toPath);
                    }
                }
                else if (target.Contains(current.VertexId))
                { // the current is a target!
                    var toPath = target.GetPathTo(current.VertexId);
                    toPath = toPath.Reverse();
                    toPath = toPath.ConcatenateAfter(current);

                    if (stopAtFirst)
                    { // stop at the first occurrence.
                        segmentsToTarget[0] = toPath;
                        return segmentsToTarget;
                    }
                    else
                    { // normal one-to-many; add to the result.
                        // check if routing is finished.
                        if (segmentsToTarget[idx] == null)
                        { // make sure only the first route is set.
                            segmentsToTarget[idx] = toPath;
                            if (targets.All(x => x.Count == 0))
                            { // routing is finished!
                                return segmentsToTarget.ToArray();
                            }
                        }
                        else if (segmentsToTarget[idx].Weight > toPath.Weight)
                        { // check if the second, third or later is shorter.
                            segmentsToTarget[idx] = toPath;
                        }
                    }
                }
            }

            // start OsmSharp.Routing.
            var arcs = graph.GetEdges(Convert.ToUInt32(current.VertexId));
            // chosenVertices.Add(current.VertexId);
            visitList.SetVisited(current);

            // loop until target is found and the route is the shortest!
            var noSpeed = new Speed() { Direction = null, MeterPerSecond = 0 };
            while (true)
            {
                // get the current labels list (if needed).
                IList<RoutingLabel> currentLabels = null;
                if (interpreter.Constraints != null)
                { // there are constraints, get the labels.
                    currentLabels = labels[current.VertexId];
                    labels.Remove(current.VertexId);
                }

                // check turn-restrictions.
                List<uint[]> restrictions = null;
                bool isRestricted = false;
                if (current.From != null &&
                    current.From.VertexId > 0 &&
                    graph.TryGetRestrictionAsStart(vehicle, (uint)current.From.VertexId, out restrictions))
                { // there are restrictions!
                    // search for a restriction that ends in the currently selected vertex.
                    for(int idx = 0; idx < restrictions.Count; idx++)
                    {
                        var restriction = restrictions[idx];
                        if(restriction[restriction.Length - 1] == current.VertexId)
                        { // oeps, do not consider the neighbours of this vertex.
                            isRestricted = true;
                            break;
                        }

                        for(int restrictedIdx = 0; restrictedIdx < restriction.Length; restrictedIdx++)
                        { // make sure the restricted vertices can be choosen multiple times.
                            // restrictedVertices.Add(restriction[restrictedIdx]);
                            visitList.SetRestricted(restriction[restrictedIdx]);
                        }
                    }
                }
                if (!isRestricted)
                {
                    // update the visited nodes.
                    while (arcs.MoveNext())
                    {
                        var neighbour = arcs;
                        if(visitList.HasBeenVisited(neighbour.Neighbour, current.VertexId))
                        { // has areadly been choosen.
                            continue;
                        }

                        // prevent u-turns.
                        if(current.From != null)
                        { // a possible u-turn.
                            if(current.From.VertexId == neighbour.Neighbour)
                            { // a u-turn, don't do this please!
                                continue;
                            }
                        }

                        // get the speed from cache or calculate.
                        Speed speed = noSpeed;
                        if (!speeds.TryGetValue(neighbour.EdgeData.Tags, out speed))
                        { // speed not there, calculate speed.
                            var tags = graph.TagsIndex.Get(neighbour.EdgeData.Tags);
                            speed = noSpeed;
                            if (vehicle.CanTraverse(tags))
                            { // can traverse, speed not null!
                                speed = new Speed()
                                {
                                    MeterPerSecond = ((OsmSharp.Units.Speed.MeterPerSecond)vehicle.ProbableSpeed(tags)).Value,
                                    Direction = vehicle.IsOneWay(tags)
                                };
                            }
                            speeds.Add(neighbour.EdgeData.Tags, speed);
                        }

                        // check the tags against the interpreter.
                        if (speed.MeterPerSecond > 0 && (!speed.Direction.HasValue || speed.Direction.Value == neighbour.EdgeData.Forward))
                        //if (vehicle.CanTraverse(tags))
                        { // it's ok; the edge can be traversed by the given vehicle.
                            if ((current.From == null ||
                                interpreter.CanBeTraversed(current.From.VertexId, current.VertexId, neighbour.Neighbour)))
                            { // the neighbour is forward and is not settled yet!
                                bool restrictionsOk = true;
                                if (restrictions != null)
                                { // search for a restriction that ends in the currently selected neighbour and check if it's via-vertex matches.
                                    for (int idx = 0; idx < restrictions.Count; idx++)
                                    {
                                        var restriction = restrictions[idx];
                                        if (restriction[restriction.Length - 1] == neighbour.Neighbour)
                                        { // oeps, do not consider the neighbours of this vertex.
                                            if (restriction[restriction.Length - 2] == current.VertexId)
                                            { // damn this route-part is restricted!
                                                restrictionsOk = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // check the labels (if needed).
                                bool constraintsOk = true;
                                if (restrictionsOk && interpreter.Constraints != null)
                                { // check if the label is ok.
                                    var neighbourLabel = interpreter.Constraints.GetLabelFor(
                                        graph.TagsIndex.Get(neighbour.EdgeData.Tags));

                                    // only test labels if there is a change.
                                    if (currentLabels.Count == 0 || !neighbourLabel.Equals(currentLabels[currentLabels.Count - 1]))
                                    { // labels are different, test them!
                                        constraintsOk = interpreter.Constraints.ForwardSequenceAllowed(currentLabels,
                                            neighbourLabel);

                                        if (constraintsOk)
                                        { // update the labels.
                                            var neighbourLabels = new List<RoutingLabel>(currentLabels);
                                            neighbourLabels.Add(neighbourLabel);

                                            labels[neighbour.Neighbour] = neighbourLabels;
                                        }
                                    }
                                    else
                                    { // set the same label(s).
                                        labels[neighbour.Neighbour] = currentLabels;
                                    }
                                }

                                if (constraintsOk && restrictionsOk)
                                { // all constraints are validated or there are none.
                                    // calculate neighbors weight.
                                    double totalWeight = current.Weight + (neighbour.EdgeData.Distance / speed.MeterPerSecond);
                                    //double totalWeight = current.Weight + neighbour.Value.Distance;

                                    // update the visit list;
                                    var neighbourRoute = new PathSegment<long>(neighbour.Neighbour, totalWeight, current);
                                    heap.Push(neighbourRoute, (float)neighbourRoute.Weight);
                                }
                            }
                        }
                    }
                }

                // while the visit list is not empty.
                current = null;
                if (heap.Count > 0)
                { // choose the next vertex.
                    current = heap.Pop();
                    while (current != null &&
                        // chosenVertices.Contains(current.VertexId))
                        visitList.HasBeenVisited(current))
                    { // keep dequeuing.
                        current = heap.Pop();
                    }
                    if (current != null)
                    {
                        // chosenVertices.Add(current.VertexId);
                        visitList.SetVisited(current);
                    }
                }
                while (current != null && current.Weight > weight)
                {
                    if (returnAtWeight)
                    { // add all the reached vertices larger than weight to the results.
                        segmentsAtWeight.Add(current);
                    }

                    // choose the next vertex.
                    current = heap.Pop();
                    while (current != null &&
                        // chosenVertices.Contains(current.VertexId))
                        visitList.HasBeenVisited(current))
                    { // keep dequeuing.
                        current = heap.Pop();
                    }
                }

                if (current == null)
                { // route is not found, there are no vertices left
                    // or the search went outside of the max bounds.
                    break;
                }

                // check target.
                for (int idx = 0; idx < targets.Length; idx++)
                {
                    PathSegmentVisitList target = targets[idx];
                    if (target.Contains(current.VertexId))
                    { // the current is a target!
                        var toPath = target.GetPathTo(current.VertexId);
                        toPath = toPath.Reverse();
                        toPath = toPath.ConcatenateAfter(current);

                        if (stopAtFirst)
                        { // stop at the first occurrence.
                            segmentsToTarget[0] = toPath;
                            return segmentsToTarget;
                        }
                        else
                        { // normal one-to-many; add to the result.
                            // check if routing is finished.
                            if (segmentsToTarget[idx] == null)
                            { // make sure only the first route is set.
                                segmentsToTarget[idx] = toPath;
                            }
                            else if (segmentsToTarget[idx].Weight > toPath.Weight)
                            { // check if the second, third or later is shorter.
                                segmentsToTarget[idx] = toPath;
                            }

                            // remove this vertex from this target's paths.
                            target.Remove(current.VertexId);

                            // if this target is empty it's optimal route has been found.
                            if (target.Count == 0)
                            { // now the shortest route has been found for sure!
                                if (targets.All(x => x.Count == 0))
                                { // routing is finished!
                                    return segmentsToTarget.ToArray();
                                }
                            }
                        }
                    }
                }

                // get the neighbors of the current node.
                arcs = graph.GetEdges(Convert.ToUInt32(current.VertexId));
            }

            // return the result.
            if (!returnAtWeight)
            {
                return segmentsToTarget.ToArray();
            }
            return segmentsAtWeight.ToArray();
        }

        private struct Speed
        {
            public double MeterPerSecond { get; set; }

            public bool? Direction { get; set; }
        }

        #endregion
    }
}