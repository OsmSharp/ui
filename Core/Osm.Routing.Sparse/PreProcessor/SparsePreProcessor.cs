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
using Osm.Data;
using Osm.Core;
using Tools.Math.Graph;
using Tools.Math.Geo;
using Osm.Routing.Core.Roads;
using Osm.Core.Simple;
using Osm.Data.Core.Sparse;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Routing.Sparse.Routing.Graph;
using Osm.Routing.Sparse.Cache;
using Osm.Routing.Core.Roads.Tags;
using Osm.Routing.Core;
using Tools.Core;

namespace Osm.Routing.Sparse.PreProcessor
{
    /// <summary>
    /// PreProcessor for sparse edges.
    /// </summary>
    /// <typeparam name="EdgeType">The edge type of the source Graph.</typeparam>
    /// <typeparam name="VertexType">The vertex type of the source Graph.</typeparam>
    public class SparsePreProcessor : ISparsePreProcessorProgress
    {
        /// <summary>
        /// Holds the target of the data.
        /// </summary>
        private ISparseData _target;

        /// <summary>
        /// Holds the accept changes boolean.
        /// </summary>
        private bool _accept_changes;

        /// <summary>
        /// Holds the progress reporter.
        /// </summary>
        private ISparsePreProcessorProgress _progress;
        
        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="data"></param>
        public SparsePreProcessor(ISparseData target)
        {
            _target = new SparseDataCache(10000, target);
            _accept_changes = true;

            _progress = null;
        }

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="data"></param>
        public SparsePreProcessor(ISparseData target, ISparsePreProcessorProgress progress)
        {
            _target = new SparseDataCache(10000, target);
            _accept_changes = true;

            _progress = progress;
        }

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="accept_changes"></param>
        /// <param name="progress"></param>
        public SparsePreProcessor(ISparseData target, bool accept_changes, ISparsePreProcessorProgress progress)
        {
            _target = new SparseDataCache(10000, target);
            _accept_changes = accept_changes;

            _progress = progress;
        }

        #region Graph
        
        /// <summary>
        /// Holds the sparse graph instance.
        /// </summary>
        private SparseGraph _graph;

        /// <summary>
        /// Returns an instance of a sparse graph based on the target data.
        /// </summary>
        private SparseGraph Graph
        {
            get
            {
                if (_graph == null)
                {
                    _graph = new SparseGraph(_target);
                }
                return _graph;
            }
        }

        #endregion

        #region Nodes

        /// <summary>
        /// Processes a node.
        /// </summary>
        /// <param name="node"></param>
        public void Process(SimpleNode node, SimpleChangeType change)
        {
            if (!_accept_changes && (change == SimpleChangeType.Delete || change == SimpleChangeType.Modify))
            {
                throw new Exception("Preprocessor cannot accept changes!");
            }
            switch (change)
            {
                case SimpleChangeType.Create:
                    this.CreateNode(node);
                    break;
                case SimpleChangeType.Modify:
                    this.ModifyNode(node);
                    break;
                case SimpleChangeType.Delete:
                    this.DeleteNode(node);
                    break;
            }
        }

        /// <summary>
        /// A node was created, process the creation.
        /// </summary>
        /// <param name="node"></param>
        private void CreateNode(SimpleNode node)
        {
            SimpleVertex vertex = new SimpleVertex();
            vertex.Id = node.Id.Value;
            vertex.Latitude = node.Latitude.Value;
            vertex.Longitude = node.Longitude.Value;

            _target.PersistSimpleVertex(vertex);
        }

        /// <summary>
        /// A node was deleted, process the deletion.
        /// </summary>
        /// <param name="node"></param>
        private void DeleteNode(SimpleNode node)
        {
            // get the simple vertex.
            SimpleVertex simple_vertex = _target.GetSimpleVertex(node.Id.Value);
            if (simple_vertex != null)
            {
                // delete the simple vertex.
                _target.DeleteSimpleVertex(node.Id.Value);

                // get the simple node.
                SparseSimpleVertex vertex = _target.GetSparseSimpleVertex(node.Id.Value);

                if (vertex != null)
                {
                    // change and persist.
                    _target.DeleteSparseSimpleVertex(node.Id.Value);

                    // is the node sparse or not.
                    if (vertex.Neighbour1 == 0 && vertex.Neighbour2 == 0)
                    { // the node is sparse.
                        SparseVertex sparse_vertex = _target.GetSparseVertex(node.Id.Value);
                        if (sparse_vertex != null)
                        { // the sparse vertex.

                            // this vertex needs to be deleted.
                            // a new sparse vertex for each neighbour needs to be added.

                            foreach (SparseVertexNeighbour neighbour in sparse_vertex.Neighbours)
                            {
                                // get the neighbour
                                SparseVertex sparse_neighbour = _target.GetSparseVertex(neighbour.Id);

                                // get the reverse neighbour.
                                SparseVertexNeighbour reverse_neighbour = sparse_vertex.GetSparseVertexNeighbour(sparse_vertex.Id);
                                if (reverse_neighbour.Nodes == null || reverse_neighbour.Nodes.Length == 0)
                                { // the neighbour needs to be removed.
                                    sparse_neighbour.Neighbours =
                                        sparse_neighbour.Neighbours.Remove(reverse_neighbour);
                                }
                                else
                                { // there exist nodes in between, adjust the neighbour.
                                    long new_sparse_id = reverse_neighbour.Nodes[0];

                                    // adjust the non-sparse verion.
                                    SparseSimpleVertex new_simple_vertex = _target.GetSparseSimpleVertex(new_sparse_id);
                                    new_simple_vertex.Neighbour1 = 0;
                                    new_simple_vertex.Neighbour2 = 0;
                                    _target.PersistSparseSimpleVertex(new_simple_vertex);

                                    // create the new sparse vertex.
                                    List<long> nodes = new List<long>(reverse_neighbour.Nodes);
                                    nodes.RemoveAt(0);
                                    nodes.Reverse();

                                    SparseVertex new_sparse_neighbour = new SparseVertex();
                                    new_sparse_neighbour.Id = new_sparse_id;
                                    new_sparse_neighbour.Coordinates = new double[2];
                                    sparse_vertex.Coordinates[0] = new_simple_vertex.Latitude;
                                    sparse_vertex.Coordinates[1] = new_simple_vertex.Longitude;
                                    sparse_vertex.Neighbours = new SparseVertexNeighbour[1];
                                    sparse_vertex.Neighbours[0] = new SparseVertexNeighbour();
                                    sparse_vertex.Neighbours[0].Nodes = nodes.ToArray();
                                    sparse_vertex.Neighbours[0].Tags = reverse_neighbour.Tags;
                                    sparse_vertex.Neighbours[0].Directed = reverse_neighbour.Directed;
                                    sparse_vertex.Neighbours[0].Direction = reverse_neighbour.Direction;
                                    new_sparse_neighbour.ReCalculateWeight(sparse_vertex.Id, _target);

                                    _target.PersistSparseVertex(new_sparse_neighbour);
                                }
                            }

                            // remove sparse vertex.
                            _target.DeleteSparseVertex(sparse_vertex.Id);
                            _target.DeleteSparseSimpleVertex(sparse_vertex.Id);
                        }
                    }
                    else
                    { // node was moved; recalculate weights.
                        SparseVertex neighbour1 = _target.GetSparseVertex(vertex.Neighbour1);
                        SparseVertex neighbour2 = _target.GetSparseVertex(vertex.Neighbour2);

                        // remove the node.
                        SparseVertexNeighbour neigbour = neighbour1.GetSparseVertexNeighbour(neighbour2.Id);
                        neigbour.Nodes = neigbour.Nodes.Remove(node.Id.Value);

                        neigbour = neighbour2.GetSparseVertexNeighbour(neighbour1.Id);
                        neigbour.Nodes = neigbour.Nodes.Remove(node.Id.Value);

                        // recalculate weights.
                        neighbour1.ReCalculateWeight(neighbour2.Id, _target);
                        neighbour2.ReCalculateWeight(neighbour1.Id, _target);

                        _target.PersistSparseVertex(neighbour1);
                        _target.PersistSparseVertex(neighbour2);
                    }
                }
            }
        }

        /// <summary>
        /// A node was modified, process the modifications.
        /// </summary>
        /// <param name="node"></param>
        private void ModifyNode(SimpleNode node)
        {
            // get the simple vertex.
            SimpleVertex simple_vertex = _target.GetSimpleVertex(node.Id.Value);
            if (simple_vertex != null &&
                (simple_vertex.Longitude != node.Longitude ||
                 simple_vertex.Latitude != node.Latitude)) // detect on relocation changes.)
            {
                // edit and persit the simple vertex.
                simple_vertex.Longitude = node.Longitude.Value;
                simple_vertex.Latitude = node.Latitude.Value;
                _target.PersistSimpleVertex(simple_vertex);

                // get the simple node.
                SparseSimpleVertex vertex = _target.GetSparseSimpleVertex(node.Id.Value);

                if (vertex != null)
                {
                    // change and persist.
                    vertex.Latitude = node.Latitude.Value;
                    vertex.Longitude = node.Longitude.Value;
                    _target.PersistSparseSimpleVertex(vertex);

                    // is the node sparse or not.
                    if (vertex.Neighbour1 == 0 && vertex.Neighbour2 == 0)
                    { // the node is sparse.
                        SparseVertex sparse_vertex = _target.GetSparseVertex(node.Id.Value);
                        if (sparse_vertex != null)
                        { // the sparse vertex.
                            // change and persist.
                            sparse_vertex.Coordinates[0] = node.Latitude.Value;
                            sparse_vertex.Coordinates[1] = node.Longitude.Value;
                            _target.PersistSparseVertex(sparse_vertex);
                        }
                    }
                    else
                    { // node was moved; recalculate weights.
                        SparseVertex neighbour1 = _target.GetSparseVertex(vertex.Neighbour1);
                        SparseVertex neighbour2 = _target.GetSparseVertex(vertex.Neighbour2);

                        neighbour1.ReCalculateWeight(neighbour2.Id, _target);
                        neighbour2.ReCalculateWeight(neighbour1.Id, _target);

                        _target.PersistSparseVertex(neighbour1);
                        _target.PersistSparseVertex(neighbour2);
                    }
                }
            }
        }

        #endregion

        #region Ways

        /// <summary>
        /// Processes a way.
        /// </summary>
        /// <param name="way"></param>
        public void Process(SimpleWay way, SimpleChangeType change)
        {
            if (!_accept_changes && 
                (change == SimpleChangeType.Delete || change == SimpleChangeType.Modify))
            {
                throw new Exception("Preprocessor cannot accept changes!");
            }
            switch (change)
            {
                case SimpleChangeType.Create:
                    this.CreateWay(way);
                    break;
                case SimpleChangeType.Modify:
                    this.ModifyWay(way);
                    break;
                case SimpleChangeType.Delete:
                    this.DeleteWay(way);
                    break;
            }
        }

        /// <summary>
        /// A way was delete, process it.
        /// </summary>
        /// <param name="way"></param>
        private void DeleteWay(SimpleWay way)
        {
            // get and delete the simple arc.
            SimpleArc arc = _target.GetSimpleArc(way.Id.Value);
            _target.DeleteSimpleArc(way.Id.Value);

            // get all sparse vertices.
            List<SparseVertex> sparse_vertices = _target.GetSparseVertices(arc.Nodes);
            List<SparseSimpleVertex> sparse_simple_vertices = _target.GetSparseSimpleVertices(arc.Nodes);

            // get all simple vertices.
            List<SimpleVertex> simple_vertices = _target.GetSimpleVertices(arc.Nodes);
            //List<SparseSimpleVertex> sparse_simple_vertices = _target.GetSparseSimpleVertices(way.Nodes);

            if (arc.Nodes.Length > 1)
            { // the way has at least two nodes.
                for (int idx = 0; idx < simple_vertices.Count; idx++)
                {
                    SimpleVertex simple_vertex = simple_vertices[idx];

                    if (simple_vertex != null)
                    { // there is a simple vertex at least.
                        SparseVertex sparse_vertex = sparse_vertices[idx];

                        if (sparse_vertex != null)
                        { // only sparse vertices can be kept.
                            // if the sparse vertices have only one neighbour they occured only in this way.
                            if (sparse_vertex.Neighbours != null && sparse_vertex.Neighbours.Length == 1)
                            {
                                _target.DeleteSparseSimpleVertex(simple_vertex.Id);
                                _target.DeleteSparseVertex(simple_vertex.Id);
                            }
                            else
                            { // there are two or more neighbours.
                                // remove the neighbours
                                List<int> route_to_neighbour =
                                    this.GetRouteToNeighbour(simple_vertices, idx, sparse_simple_vertices, sparse_vertices);
                                List<int> route_to_neighbour_reversed =
                                    this.GetRouteToNeighbourReversed(simple_vertices, idx, sparse_simple_vertices, sparse_vertices);

                                // remove neighbours along the deleted way.
                                // remove forward neighbour.
                                SimpleVertex neighbour = simple_vertices[route_to_neighbour[route_to_neighbour.Count - 1]];
                                SparseVertexNeighbour neighbour_sparse =
                                    sparse_vertex.GetSparseVertexNeighbour(neighbour.Id);
                                sparse_vertex.Neighbours = sparse_vertex.Neighbours.Remove(neighbour_sparse);
                                // remove backward neighbour.
                                neighbour = simple_vertices[route_to_neighbour_reversed[route_to_neighbour.Count - 1]];
                                neighbour_sparse =
                                    sparse_vertex.GetSparseVertexNeighbour(neighbour.Id);
                                sparse_vertex.Neighbours = sparse_vertex.Neighbours.Remove(neighbour_sparse);

                                // descide to keep the vertex or not.
                                if (sparse_vertex.Neighbours != null &&
                                    sparse_vertex.Neighbours.Length == 2)
                                { // there are only two neighbours.
                                    
                                    SparseVertexNeighbour neighbour1 = sparse_vertex.Neighbours[0];
                                    SparseVertex sparse_neighbour1 = _target.GetSparseVertex(neighbour1.Id);
                                    SparseVertexNeighbour sparse_neighbour1_neighbour = sparse_neighbour1.GetSparseVertexNeighbour(
                                        sparse_vertex.Id);
                                    SparseVertexNeighbour neighbour2 = sparse_vertex.Neighbours[1];
                                    SparseVertex sparse_neighbour2 = _target.GetSparseVertex(neighbour2.Id);
                                    SparseVertexNeighbour sparse_neighbour2_neighbour = sparse_neighbour2.GetSparseVertexNeighbour(
                                        sparse_vertex.Id);

                                    
                                    // adjust neighbour1.
                                    sparse_neighbour1_neighbour.Nodes = sparse_neighbour1_neighbour.Nodes.Add(sparse_vertex.Id);
                                    sparse_neighbour1_neighbour.Nodes = sparse_neighbour1_neighbour.Nodes.AddRange(
                                        neighbour2.Nodes);
                                    sparse_neighbour1_neighbour.Id = neighbour2.Id;
                                    sparse_neighbour1.ReCalculateWeight(sparse_neighbour1_neighbour.Id, _target);
                                    _target.PersistSparseVertex(sparse_neighbour1);

                                    // adjust neighbour2.
                                    sparse_neighbour2_neighbour.Nodes = sparse_neighbour2_neighbour.Nodes.Add(sparse_vertex.Id);
                                    sparse_neighbour2_neighbour.Nodes = sparse_neighbour2_neighbour.Nodes.AddRange(
                                        neighbour1.Nodes);
                                    sparse_neighbour2_neighbour.Id = neighbour1.Id;
                                    sparse_neighbour2.ReCalculateWeight(sparse_neighbour2_neighbour.Id, _target);
                                    _target.PersistSparseVertex(sparse_neighbour2);

                                    _target.DeleteSparseVertex(sparse_vertex.Id);

                                    SparseSimpleVertex sparse_simple_vertex = sparse_simple_vertices[idx];
                                    sparse_simple_vertex.Neighbour1 = neighbour1.Id;
                                    sparse_simple_vertex.Neighbour2 = neighbour2.Id;
                                    _target.PersistSparseSimpleVertex(sparse_simple_vertex);
                                }
                                else
                                { // there are less or more than two neighbours.
                                    _target.PersistSparseVertex(sparse_vertex);
                                }
                            }
                        }
                        else
                        { // there is no sparse vertex here, so remove the simple one.
                            _target.DeleteSparseSimpleVertex(simple_vertex.Id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A way was modified, process it.
        /// </summary>
        /// <param name="way"></param>
        private void ModifyWay(SimpleWay way)
        {
            // first delete the way. 
            this.DeleteWay(way);

            // add the way.
            this.CreateWay(way);
        }

        /// <summary>
        /// A new way was create, process it.
        /// </summary>
        /// <param name="way"></param>
        private void CreateWay(SimpleWay way)
        {
            // add the arc.
            SimpleArc arc = new SimpleArc();
            arc.Id = way.Id.Value;
            arc.Nodes = way.Nodes.ToArray();
            _target.PersistSimpleArc(arc);

            // split ways.
            HashSet<long> nodesset = new HashSet<long>();
            foreach (long node_id in way.Nodes)
            {
                nodesset.Add(node_id);
            }
            if (nodesset.Count < way.Nodes.Count)
            {
                List<SimpleWay> ways = this.SplitSelfIntersecting(way);
                foreach (SimpleWay split_way in ways)
                {
                    this.CreateSplitWay(split_way);
                }
            }
            else
            {
                this.CreateSplitWay(way);
            }            
        }

        private void CreateSplitWay(SimpleWay way)
        {// test for road information.
            List<int> route_to_neighbour;
            List<int> route_to_neighbour_reversed;

            RoadTagsInterpreterBase interpreter = new RoadTagsInterpreterBase(way.Tags);
            if (interpreter.IsRoad())
            {
                // get all sparse vertices.
                List<SparseVertex> sparse_vertices = _target.GetSparseVertices(way.Nodes);
                List<SparseSimpleVertex> sparse_simple_vertices = _target.GetSparseSimpleVertices(way.Nodes);

                // get all simple vertices.
                List<SimpleVertex> simple_vertices = _target.GetSimpleVertices(way.Nodes);
                //List<SparseSimpleVertex> sparse_simple_vertices = _target.GetSparseSimpleVertices(way.Nodes);

                // check if all the nodes are present.
                foreach (SimpleVertex simple_vertex in simple_vertices)
                {
                    if (simple_vertex == null)
                    { // there is a vertex that is null, drop the entire way! 
                        // TODO: improve on this by still using part of the way
                        // but the risk will be updates.
                        return;
                    }
                }

                if (way.Nodes.Count > 1)
                { // the way has at least two nodes.
                    for (int idx = 0; idx < simple_vertices.Count; idx++)
                    {
                        SimpleVertex simple_vertex = simple_vertices[idx];

                        if ((idx == simple_vertices.Count - 1) &&
                            simple_vertices[0].Id == simple_vertices[simple_vertices.Count - 1].Id)
                        {
                            break;
                        }

                        if (simple_vertex != null)
                        { // there is a simple vertex at least.
                            SparseVertex sparse_vertex = sparse_vertices[idx];
                            SparseSimpleVertex sparse_simple_vertex = sparse_simple_vertices[idx];

                            route_to_neighbour =
                                this.GetRouteToNeighbour(simple_vertices, idx, sparse_simple_vertices, sparse_vertices);
                            route_to_neighbour_reversed =
                                this.GetRouteToNeighbourReversed(simple_vertices, idx, sparse_simple_vertices, sparse_vertices);

                            if (sparse_vertex != null ||
                                ((route_to_neighbour.Count == 0 || route_to_neighbour_reversed.Count == 0)) && sparse_simple_vertex == null)
                            { // there already is a sparse vertex or there is an imbalance in neighbour.
                                this.CreateSignicant(simple_vertex, sparse_vertex, interpreter, way, route_to_neighbour, 
                                    route_to_neighbour_reversed, simple_vertices);
                            }
                            else if ((idx == 0 || idx == (simple_vertices.Count - 1)) && 
                                simple_vertices[0] == simple_vertices[simple_vertices.Count - 1])
                            {
                                this.CreateSignicant(simple_vertex, sparse_vertex, interpreter, way, route_to_neighbour,
                                    route_to_neighbour_reversed, simple_vertices);
                            }
                            else // sparse vertex == null && (route_to_neighbour.Count > 0 && route_to_neighbour_reversed.Count > 0)
                            { // there is no sparse vertex and neighbours exist on both sides.
                                HashSet<long> neighbours = new HashSet<long>();
                                long neighbour1 = 0;
                                if (route_to_neighbour.Count > 0)
                                {
                                    neighbour1 = simple_vertices[route_to_neighbour[route_to_neighbour.Count - 1]].Id;
                                    neighbours.Add(neighbour1);
                                }
                                long neighbour2 = 0;
                                if (route_to_neighbour_reversed.Count > 0)
                                {
                                    neighbour2 = simple_vertices[route_to_neighbour_reversed[route_to_neighbour_reversed.Count - 1]].Id;
                                    neighbours.Add(neighbour1);
                                }

                                // this node could belong to another way making it a sparse vertex.
                                if (sparse_simple_vertices[idx] != null &&
                                     ((sparse_simple_vertices[idx] != null && !neighbours.Contains(sparse_simple_vertices[idx].Neighbour1)
                                   || (sparse_simple_vertices[idx] != null && !neighbours.Contains(sparse_simple_vertices[idx].Neighbour2)))))
                                { // sparse simple vertex exists already and with different neighbours.
                                    // we need to create a sparse vertex.

                                    // adjust the two neighbours.
                                    SparseVertex neighbour1_vertex = _target.GetSparseVertex(sparse_simple_vertices[idx].Neighbour1);
                                    SparseVertexNeighbour neighbour1_neighbour = neighbour1_vertex.GetSparseVertexNeighbour(
                                        sparse_simple_vertices[idx].Neighbour2, sparse_simple_vertices[idx].Id);
                                    List<long> neigbour1_nodes = new List<long>();
                                    neighbour1_neighbour.Id = sparse_simple_vertices[idx].Id;
                                    List<long> nodes = new List<long>();
                                    bool add = true;
                                    for (int node_idx = 0; node_idx < neighbour1_neighbour.Nodes.Length; node_idx++)
                                    {
                                        if (add)
                                        {
                                            if (neighbour1_neighbour.Nodes[node_idx] == neighbour1_neighbour.Id)
                                            {
                                                add = false;
                                            }
                                            else
                                            {
                                                nodes.Add(neighbour1_neighbour.Nodes[node_idx]);
                                            }
                                        }
                                        else
                                        {
                                            neigbour1_nodes.Add(neighbour1_neighbour.Nodes[node_idx]);
                                        }
                                    }
                                    neighbour1_neighbour.Nodes = nodes.ToArray();
                                    neighbour1_vertex.ReCalculateWeight(neighbour1_neighbour.Id, _target);
                                    _target.PersistSparseVertex(neighbour1_vertex);

                                    // get all the sparse nodes in between and reset their neighbours.
                                    List<SparseSimpleVertex> vertices_between = _target.GetSparseSimpleVertices(
                                        neighbour1_neighbour.Nodes);
                                    {
                                        foreach (SparseSimpleVertex vertex_between in vertices_between)
                                        {
                                            vertex_between.Neighbour1 = neighbour1_vertex.Id;
                                            vertex_between.Neighbour2 = neighbour1_neighbour.Id;

                                            _target.PersistSparseSimpleVertex(vertex_between);
                                        }
                                    }

                                    SparseVertex neighbour2_vertex = _target.GetSparseVertex(sparse_simple_vertices[idx].Neighbour2);
                                    SparseVertexNeighbour neighbour2_neighbour = neighbour2_vertex.GetSparseVertexNeighbour(
                                        sparse_simple_vertices[idx].Neighbour1, sparse_simple_vertices[idx].Id);
                                    List<long> neigbour2_nodes = new List<long>();
                                    neighbour2_neighbour.Id = sparse_simple_vertices[idx].Id;
                                    nodes = new List<long>();
                                    add = true;
                                    for (int node_idx = 0; node_idx < neighbour2_neighbour.Nodes.Length; node_idx++)
                                    {
                                        if (add)
                                        {
                                            if (neighbour2_neighbour.Nodes[node_idx] == neighbour2_neighbour.Id)
                                            {
                                                add = false;
                                            }
                                            else
                                            {
                                                nodes.Add(neighbour2_neighbour.Nodes[node_idx]);
                                            }
                                        }
                                        else
                                        {
                                            neigbour2_nodes.Add(neighbour2_neighbour.Nodes[node_idx]);
                                        }
                                    }
                                    neighbour2_neighbour.Nodes = nodes.ToArray();
                                    neighbour2_vertex.ReCalculateWeight(neighbour2_neighbour.Id, _target);
                                    _target.PersistSparseVertex(neighbour2_vertex);

                                    // get all the sparse nodes in between and reset their neighbours.
                                    vertices_between = _target.GetSparseSimpleVertices(
                                        neighbour2_neighbour.Nodes);
                                    {
                                        foreach (SparseSimpleVertex vertex_between in vertices_between)
                                        {
                                            vertex_between.Neighbour1 = neighbour2_vertex.Id;
                                            vertex_between.Neighbour2 = neighbour2_neighbour.Id;

                                            _target.PersistSparseSimpleVertex(vertex_between);
                                        }
                                    }

                                    // create a new sparse vertex.
                                    sparse_vertex = new SparseVertex();
                                    sparse_vertex.Id = sparse_simple_vertices[idx].Id;
                                    sparse_vertex.Coordinates = new double[2];
                                    sparse_vertex.Coordinates[0] = sparse_simple_vertices[idx].Latitude;
                                    sparse_vertex.Coordinates[1] = sparse_simple_vertices[idx].Longitude;
                                    // build the neighbours
                                    List<SparseVertexNeighbour> new_sparse_neighbours = new List<SparseVertexNeighbour>();
                                    SparseVertexNeighbour new_sparse_neighbour1 = new SparseVertexNeighbour();
                                    new_sparse_neighbour1.Id = sparse_simple_vertices[idx].Neighbour1;
                                    new_sparse_neighbour1.Tags = neighbour1_neighbour.Tags;
                                    new_sparse_neighbour1.Directed = neighbour1_neighbour.Directed;
                                    new_sparse_neighbour1.Direction = !neighbour1_neighbour.Direction;
                                    new_sparse_neighbour1.Nodes = neigbour2_nodes.ToArray();
                                    new_sparse_neighbours.Add(new_sparse_neighbour1);

                                    SparseVertexNeighbour new_sparse_neighbour2 = new SparseVertexNeighbour();
                                    new_sparse_neighbour2.Id = sparse_simple_vertices[idx].Neighbour2;
                                    new_sparse_neighbour2.Tags = neighbour2_neighbour.Tags;
                                    new_sparse_neighbour2.Directed = neighbour2_neighbour.Directed;
                                    new_sparse_neighbour2.Direction = !neighbour2_neighbour.Direction;
                                    new_sparse_neighbour2.Nodes = neigbour1_nodes.ToArray();
                                    new_sparse_neighbours.Add(new_sparse_neighbour2);

                                    sparse_vertex.Neighbours = new_sparse_neighbours.ToArray();

                                    sparse_vertex.ReCalculateWeight(new_sparse_neighbour1.Id, _target);
                                    sparse_vertex.ReCalculateWeight(new_sparse_neighbour2.Id, _target);

                                    _target.PersistSparseVertex(sparse_vertex);

                                    sparse_vertices[idx] = sparse_vertex;

                                    // adjust simple vertex.
                                    sparse_simple_vertices[idx].Neighbour1 = 0;
                                    sparse_simple_vertices[idx].Neighbour2 = 0;
                                    _target.PersistSparseSimpleVertex(sparse_simple_vertices[idx]);

                                    route_to_neighbour =
                                        this.GetRouteToNeighbour(simple_vertices, idx, sparse_simple_vertices, sparse_vertices);
                                    route_to_neighbour_reversed =
                                        this.GetRouteToNeighbourReversed(simple_vertices, idx, sparse_simple_vertices, sparse_vertices);

                                    this.CreateSignicant(simple_vertex, sparse_vertex, interpreter, way, route_to_neighbour, route_to_neighbour_reversed, simple_vertices);

                                }
                                else
                                { // just a plane insignificant sparse vertex.
                                    // adjust simple vertex.
                                    sparse_simple_vertex = new SparseSimpleVertex();
                                    sparse_simple_vertex.Id = simple_vertex.Id;
                                    sparse_simple_vertex.Latitude = simple_vertex.Latitude;
                                    sparse_simple_vertex.Longitude = simple_vertex.Longitude;
                                    sparse_simple_vertex.Neighbour1 = neighbour1;
                                    sparse_simple_vertex.Neighbour2 = neighbour2;
                                    _target.PersistSparseSimpleVertex(sparse_simple_vertex);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Splits self-intersecting ways.
        /// </summary>
        /// <param name="way"></param>
        /// <param name="nodesset"></param>
        /// <returns></returns>
        private List<SimpleWay> SplitSelfIntersecting(SimpleWay way)
        {
            HashSet<long> nodesset = new HashSet<long>();
            HashSet<long> doubles = new HashSet<long>();
            foreach (long node_id in way.Nodes)
            {
                if (nodesset.Contains(node_id))
                {
                    doubles.Add(node_id);
                }
                nodesset.Add(node_id);
            }
            List<SimpleWay> ways = new List<SimpleWay>();
            List<long> nodes = new List<long>();
            nodes.Add(way.Nodes[0]);
            for (int idx = 1; idx < way.Nodes.Count; idx++)
            {
                long node_id = way.Nodes[idx];

                nodes.Add(node_id);
                if (doubles.Contains(node_id))
                {
                    SimpleWay new_way = new SimpleWay();
                    new_way.Nodes = nodes;
                    new_way.Tags = way.Tags;
                    ways.Add(new_way);

                    nodes = new List<long>();
                    nodes.Add(node_id);
                }
                nodesset.Add(node_id);
            }
            return ways;
        }

        private void CreateSignicant(SimpleVertex simple_vertex, SparseVertex sparse_vertex, RoadTagsInterpreterBase interpreter, SimpleWay way,
            List<int> route_to_neighbour, List<int> route_to_neighbour_reversed, List<SimpleVertex> simple_vertices)
        {
            SparseVertexNeighbour neighbour;
            SimpleVertex neighbour_simple;
            List<SparseVertexNeighbour> extra_neighbours = new List<SparseVertexNeighbour>();

            // build the forward neighbour.
            if (route_to_neighbour.Count > 0)
            {
                // get the actual neighbour.
                neighbour_simple = simple_vertices[route_to_neighbour[route_to_neighbour.Count - 1]];
                route_to_neighbour.RemoveAt(route_to_neighbour.Count - 1);

                // build the nodes list.
                List<long> nodes = new List<long>();
                foreach (int vertex_idx in route_to_neighbour)
                {
                    nodes.Add(simple_vertices[vertex_idx].Id);
                }

                // create the neighbour.
                neighbour = new SparseVertexNeighbour();
                neighbour.Id = neighbour_simple.Id;
                neighbour.Nodes = nodes.ToArray();
                neighbour.Tags = way.Tags;
                neighbour.Directed = interpreter.IsOneWay() || interpreter.IsOneWayReverse();
                neighbour.Direction = interpreter.IsOneWay();
                neighbour.Weight = 0;

                extra_neighbours.Add(neighbour);
            }

            // build the backward neighbour.
            if (route_to_neighbour_reversed.Count > 0)
            {
                // get the actual neighbour.
                neighbour_simple = simple_vertices[route_to_neighbour_reversed[route_to_neighbour_reversed.Count - 1]];
                route_to_neighbour_reversed.RemoveAt(route_to_neighbour_reversed.Count - 1);

                // build the nodes list.
                List<long> nodes = new List<long>();
                foreach (int vertex_idx in route_to_neighbour_reversed)
                {
                    nodes.Add(simple_vertices[vertex_idx].Id);
                }

                // create the neighbour.
                neighbour = new SparseVertexNeighbour();
                neighbour.Id = neighbour_simple.Id;
                neighbour.Nodes = nodes.ToArray();
                neighbour.Tags = way.Tags;
                neighbour.Directed = interpreter.IsOneWay() || interpreter.IsOneWayReverse();
                neighbour.Direction = interpreter.IsOneWayReverse();
                neighbour.Weight = 0;

                extra_neighbours.Add(neighbour);
            }

            // create the sparse vertex if needed.
            if (sparse_vertex == null)
            {
                sparse_vertex = new SparseVertex();
                sparse_vertex.Id = simple_vertex.Id;
                sparse_vertex.Coordinates = new double[2];
                sparse_vertex.Coordinates[0] = simple_vertex.Latitude;
                sparse_vertex.Coordinates[1] = simple_vertex.Longitude;
                sparse_vertex.Neighbours = extra_neighbours.ToArray();
            }
            else
            {
                sparse_vertex.Neighbours = sparse_vertex.Neighbours.AddRange(extra_neighbours);
            }

            // calculate the weights.
            foreach (SparseVertexNeighbour extra_neighbour in extra_neighbours)
            {
                sparse_vertex.ReCalculateWeight(extra_neighbour.Id, _target);
            }

            // persist sparse vertex.
            _target.PersistSparseVertex(sparse_vertex);

            // adjust simple vertex.
            SparseSimpleVertex sparse_simple_vertex = new SparseSimpleVertex();
            sparse_simple_vertex.Id = simple_vertex.Id;
            sparse_simple_vertex.Latitude = simple_vertex.Latitude;
            sparse_simple_vertex.Longitude = simple_vertex.Longitude;
            sparse_simple_vertex.Neighbour1 = 0;
            sparse_simple_vertex.Neighbour2 = 0;
            _target.PersistSparseSimpleVertex(sparse_simple_vertex);
        }

        /// <summary>
        /// Returns the road to a neighbour only counting up in the list.
        /// </summary>
        /// <param name="simple_vertices"></param>
        /// <param name="p"></param>
        /// <param name="sparse_vertices"></param>
        /// <returns></returns>
        private List<int> GetRouteToNeighbour(List<SimpleVertex> simple_vertices, int vertex_idx, 
             List<SparseSimpleVertex> sparse_simple_vertices, List<SparseVertex> sparse_vertices)
        {
            HashSet<long> neighbours = new HashSet<long>();
            neighbours.Add(simple_vertices[vertex_idx].Id);
            List<int> route = new List<int>();
            // process the sparse nodes.
            for (int idx = vertex_idx + 1; idx < simple_vertices.Count; idx++)
            {
                route.Add(idx); // the next simple vertex.
                if (sparse_vertices[idx] != null)
                { // stop at a sparse vertex.
                    break;
                }
                if (sparse_simple_vertices[idx] != null)
                { // a sparse vertex already exists, be careful here!
                    if (!(neighbours.Contains(sparse_simple_vertices[idx].Neighbour1) ||
                        neighbours.Contains(sparse_simple_vertices[idx].Neighbour2)))
                    { // one of the two neighbours has to be the start vertex or there is something wrong!
                        break;
                    }
                }
            }
            // continue the search if the way is closed.
            if (vertex_idx == simple_vertices.Count - 1 && simple_vertices[0] == simple_vertices[simple_vertices.Count - 1])
            { // the way is closed.
                for (int idx = 1; idx < vertex_idx + 1; idx++)
                {
                    route.Add(idx); // the next simple vertex.
                    if (sparse_vertices[idx] != null)
                    { // stop at a sparse vertex.
                        break;
                    }
                    if (sparse_simple_vertices[idx] != null)
                    { // a sparse vertex already exists, be careful here!
                        if (!(neighbours.Contains(sparse_simple_vertices[idx].Neighbour1) ||
                            neighbours.Contains(sparse_simple_vertices[idx].Neighbour2)))
                        { // one of the two neighbours has to be the start vertex or there is something wrong!
                            break;
                        }
                    }
                }
            }
            if (route.Count > 0)
            {
                neighbours.Add(simple_vertices[route[route.Count - 1]].Id);

                // create new route.
                List<int> old_route = route;
                route = new List<int>();
                // post process the simple nodes.
                for (int idx = 0; idx < old_route.Count; idx++)
                {
                    route.Add(old_route[idx]);
                    if (sparse_simple_vertices[old_route[idx]] != null)
                    { // there is a sparse vertex and a potential hazard.
                        if (!(neighbours.Contains(sparse_simple_vertices[old_route[idx]].Neighbour1) &&
                            neighbours.Contains(sparse_simple_vertices[old_route[idx]].Neighbour2)))
                        {
                            break;
                        }
                    }
                }
            }
            return route;
        }

        /// <summary>
        /// Returns the road to a neighbour only counting up in the list.
        /// </summary>
        /// <param name="simple_vertices"></param>
        /// <param name="p"></param>
        /// <param name="sparse_vertices"></param>
        /// <returns></returns>
        private List<int> GetRouteToNeighbourReversed(List<SimpleVertex> simple_vertices, int vertex_idx,
             List<SparseSimpleVertex> sparse_simple_vertices, List<SparseVertex> sparse_vertices)
        {
            HashSet<long> neighbours = new HashSet<long>();
            neighbours.Add(simple_vertices[vertex_idx].Id);
            List<int> route = new List<int>();
            // process the sparse nodes.
            for (int idx = vertex_idx - 1; idx >= 0; idx--)
            {
                route.Add(idx); // the next simple vertex.
                if (sparse_vertices[idx] != null)
                { // stop at a sparse vertex.
                    break;
                }
                if (sparse_simple_vertices[idx] != null)
                { // a sparse vertex already exists, be careful here!
                    if (!(neighbours.Contains(sparse_simple_vertices[idx].Neighbour1) ||
                        neighbours.Contains(sparse_simple_vertices[idx].Neighbour2)))
                    { // one of the two neighbours has to be the start vertex or there is something wrong!
                        break;
                    }
                }
            }
            // continue the search if the way is closed.
            if (vertex_idx == 0 && simple_vertices[0] == simple_vertices[simple_vertices.Count - 1])
            { // the way is closed.
                for (int idx = simple_vertices.Count - 2; idx >= vertex_idx; idx--)
                {
                    route.Add(idx); // the next simple vertex.
                    if (sparse_vertices[idx] != null)
                    { // stop at a sparse vertex.
                        break;
                    }
                    if (sparse_simple_vertices[idx] != null)
                    { // a sparse vertex already exists, be careful here!
                        if (!(neighbours.Contains(sparse_simple_vertices[idx].Neighbour1) ||
                            neighbours.Contains(sparse_simple_vertices[idx].Neighbour2)))
                        { // one of the two neighbours has to be the start vertex or there is something wrong!
                            break;
                        }
                    }
                }
            }
            if (route.Count > 0)
            {
                neighbours.Add(simple_vertices[route[route.Count - 1]].Id);

                // create new route.
                List<int> old_route = route;
                route = new List<int>();
                // post process the simple nodes.
                for (int idx = 0; idx < old_route.Count; idx++)
                {
                    route.Add(old_route[idx]);
                    if (sparse_simple_vertices[old_route[idx]] != null)
                    { // there is a sparse vertex and a potential hazard.
                        if (!(neighbours.Contains(sparse_simple_vertices[old_route[idx]].Neighbour1) &&
                            neighbours.Contains(sparse_simple_vertices[old_route[idx]].Neighbour2)))
                        {
                            break;
                        }
                    }
                }
            }
            return route;
        }

        #endregion

        #region ISparsePreProcessorProgress Implementation

        /// <summary>
        /// Reports that processing of a vertex started.
        /// </summary>
        /// <param name="vertex_id"></param>
        void ISparsePreProcessorProgress.StartVertex(long vertex_id)
        {
            if (_progress != null)
            {
                _progress.StartVertex(vertex_id);
            }
        }

        /// <summary>
        /// Reports that a vertex has been processed.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="deleted"></param>
        void ISparsePreProcessorProgress.ProcessedVertex(SparseVertex vertex, bool deleted)
        {
            if (_progress != null)
            {
                _progress.ProcessedVertex(vertex, deleted);
            }
        }

        /// <summary>
        /// Reports that a bypassed vertex has been reported.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="coordinate"></param>
        /// <param name="neighbour1"></param>
        /// <param name="neighbour2"></param>
        void ISparsePreProcessorProgress.PersistedBypassed(long vertex_id, GeoCoordinate coordinate, long neighbour1, long neighbour2)
        {
            if (_progress != null)
            {
                _progress.PersistedBypassed(vertex_id, coordinate, neighbour1, neighbour2);
            }
        }

        #endregion
    }
}
