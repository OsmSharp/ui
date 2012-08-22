using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.HH.Primitives;

namespace Osm.Routing.HH.Core
{
    /// <summary>
    /// Core calculator.
    /// </summary>
    internal class CoreCalculator
    {
        /// <summary>
        /// The neighbour constant.
        /// </summary>
        private float _c = 1.5f;

        /// <summary>
        /// The current level.
        /// </summary>
        private int _level;

        /// <summary>
        /// The progress reporter.
        /// </summary>
        private IHighwayHierarchyProgressReporter _progress_reporter;

        /// <summary>
        /// Creates a new core calculator.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="progress_reporter"></param>
        public CoreCalculator(int level, IHighwayHierarchyProgressReporter progress_reporter)
        {
            _level = level;
            _progress_reporter = progress_reporter;
        }

        /// <summary>
        /// Calculate the core.
        /// </summary>
        /// <param name="hh"></param>
        public void Calculate(IHighwayHierarchy hh)
        {
            ////// notify core calculations will start.
            ////hh.StartCore(_level);

            ////_progress_reporter.StartedVertex(vertex);
            //IEnumerator<HighwayVertex> enumerable = hh.GetVertices(_level).GetEnumerator();
            //HashSet<HighwayVertex> local_heap = new HashSet<HighwayVertex>();
            //HighwayVertex vertex = null;
            //if (enumerable.MoveNext())
            //{
            //    vertex = enumerable.Current;
            //}
            //while (vertex != null)
            //{
            //    //_progress_reporter.StartedVertex(vertex);

            //    // evaluate for shorcuts.
            //    HashSet<HighwayVertexNeighbour> forward_neighbours =
            //        vertex.GetNeighbours(_level, null, false);
            //    HashSet<HighwayVertexNeighbour> backward_neighbours = 
            //        vertex.GetNeighboursBackward(_level, null, false);

            //    int deg_out = forward_neighbours.Count;
            //    int deg_in = backward_neighbours.Count;

            //    if (deg_in > 0 && deg_out > 0)
            //    {
            //        HashSet<long> both = new HashSet<long>();
            //        HashSet<long> forward = new HashSet<long>();
            //        foreach (HighwayVertexNeighbour neighbour in forward_neighbours)
            //        {
            //            forward.Add(neighbour.VertexId);
            //        }
            //        foreach (HighwayVertexNeighbour neighbour in backward_neighbours)
            //        {
            //            if (forward.Contains(neighbour.VertexId))
            //            {
            //                both.Add(neighbour.VertexId);
            //            }
            //        }

            //        int deg_common = both.Count;
            //        int shortcut_count = (deg_in * deg_out) - deg_common;
            //        if (shortcut_count < (_c * ((float)deg_in + (float)deg_out)))
            //        {
            //            // build shortcuts.
            //            HashSet<HighwayVertexNeighbour> shortcuts = new HashSet<HighwayVertexNeighbour>();
            //            foreach (HighwayVertexNeighbour f in forward_neighbours)
            //            {
            //                //_progress_reporter.RemovedFromCore(f);
            //                //hh.MarkUncontracted(f.To);
            //                foreach (HighwayVertexNeighbour b in backward_neighbours)
            //                {
            //                    //_progress_reporter.RemovedFromCore(b);
            //                    if (b.VertexId != f.VertexId)
            //                    {
            //                        //hh.MarkUncontracted(b.From);

            //                        //HighwayEdge shortcut = new HighwayEdge();
            //                        //shortcut.From = b.From;
            //                        //shortcut.To = f.To;
            //                        //shortcut.Weight = f.Weight + b.Weight;
            //                        //shortcuts.Add(shortcut);

            //                        //_progress_reporter.AddedToCore(shortcut);
            //                    }
            //                }
            //            }

            //            // contract the vertex.
            //            hh.ContractVertex(vertex, shortcuts); ;
            //        }
            //        else
            //        { // mark the vertex as part of the core.
            //            hh.AddCore(_level, vertex);
            //        }
            //    }
            //    else
            //    { // mark the vertex as part of the core.
            //        hh.AddCore(_level, vertex);
            //    }

            //    // get the next uncontracted vertex.
            //    vertex = hh.GetUncontracted(_level);
            //    //System.Threading.Thread.Sleep(500);
            //}
        }
    }
}
