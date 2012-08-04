using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.CH.Primitives;

namespace Osm.Routing.CH.PreProcessing.Ordering
{
    /// <summary>
    /// The edge difference calculator.
    /// </summary>
    public class EdgeDifferenceContractedSearchSpace : INodeWeightCalculator
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private INodeWitnessCalculator _witness_calculator;

        /// <summary>
        /// Holds the contracted count.
        /// </summary>
        private Dictionary<long, long> _contraction_count;

        /// <summary>
        /// Holds the depth.
        /// </summary>
        private Dictionary<long, long> _depth;

        /// <summary>
        /// Creates a new edge difference calculator.
        /// </summary>
        /// <param name="graph"></param>
        public EdgeDifferenceContractedSearchSpace(INodeWitnessCalculator witness_calculator)
        {
            _witness_calculator = witness_calculator;
            _contraction_count = new Dictionary<long, long>();
            _depth = new Dictionary<long, long>();
        }

        /// <summary>
        /// Calculates the edge-difference if u would be contracted.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public float Calculate(int level, CHVertex u)
        {
            long contracted = 0;
            _contraction_count.TryGetValue(u.Id, out contracted);

            // simulate the construction of new edges.
            int new_edges = 0;
            int removed = u.BackwardNeighbours.Count + u.ForwardNeighbours.Count;
            foreach (CHVertexNeighbour from in u.BackwardNeighbours)
            { // loop over all incoming neighbours
                foreach (CHVertexNeighbour to in u.ForwardNeighbours)
                { // loop over all outgoing neighbours
                    if (to.Id != from.Id)
                    { // the neighbours point to different vertices.
                        // a new edge is needed.
                        if (!_witness_calculator.Exists(level, from.Id, to.Id, u.Id,
                            from.Weight + to.Weight))
                        { // no witness exists.
                            new_edges++;
                        }
                    }
                }
            }

            // get the depth.                    
            long depth = 0;
            _depth.TryGetValue(u.Id, out depth);
            //return (new_edges - removed) + (contracted * 3) + depth;
            return (new_edges - removed) + depth;
        }

        /// <summary>
        /// Notifies this calculator that the vertex was contracted.
        /// </summary>
        /// <param name="vertex_id"></param>
        public void NotifyContracted(CHVertex vertex)
        {
            _contraction_count.Remove(vertex.Id);

            foreach (CHVertexNeighbour neighbour in vertex.ForwardNeighbours)
            {
                long count;
                if (!_contraction_count.TryGetValue(neighbour.Id, out count))
                {
                    _contraction_count[neighbour.Id] = 1;
                }
                else
                {
                    _contraction_count[neighbour.Id] = count++;
                }
            }

            foreach (CHVertexNeighbour neighbour in vertex.BackwardNeighbours)
            {
                long count;
                if (!_contraction_count.TryGetValue(neighbour.Id, out count))
                {
                    _contraction_count[neighbour.Id] = 1;
                }
                else
                {
                    _contraction_count[neighbour.Id] = count++;
                }
            }

            long vertex_depth = 0;
            _depth.TryGetValue(vertex.Id, out vertex_depth);
            _depth.Remove(vertex.Id);
            vertex_depth++;

            foreach (CHVertexNeighbour neighbour in vertex.ForwardNeighbours)
            {
                if (!_contraction_count.ContainsKey(neighbour.Id))
                {
                    long depth = 0;
                    _depth.TryGetValue(neighbour.Id, out depth);
                    if (vertex_depth > depth)
                    {
                        _depth[neighbour.Id] = depth;
                    }
                    else
                    {
                        _depth[neighbour.Id] = vertex_depth;
                    }
                }
            }
                
            foreach (CHVertexNeighbour neighbour in vertex.BackwardNeighbours)
            {
                if (!_contraction_count.ContainsKey(neighbour.Id))
                {
                    long depth = 0;
                    _depth.TryGetValue(neighbour.Id, out depth);
                    if (vertex_depth > depth)
                    {
                        _depth[neighbour.Id] = depth;
                    }
                    else
                    {
                        _depth[neighbour.Id] = vertex_depth;
                    }
                }
            }
        }
    }
}
