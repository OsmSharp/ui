//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Osm.Data;
//using OsmSharp.Osm;
//using OsmSharp.Osm.Filters;
//using OsmSharp.Routing.Osm.Graphs.Interpreter;
//using OsmSharp.Tools.Math.Shapes;
//using OsmSharp.Tools.Math.Shapes.ResultHelpers;
//using OsmSharp.Routing.Osm.Graphs;
//using OsmSharp.Routing.Osm.Core.Resolving;
//using OsmSharp.Tools.Math;

//namespace OsmSharp.Routing.Osm.Graphs
//{
//    /// <summary>
//    /// Represents a graph with osm data in it.
//    /// </summary>
//    public class Graph : OsmSharp.Tools.Math.Graph.IGraphDirected<GraphVertex>
//    {
//        /// <summary>
//        /// Holds the data source for this graph.
//        /// </summary>
//        private IDataSourceReadOnly _source;

//        /// <summary>
//        /// Holds the list of extra positions to route to.
//        /// </summary>
//        private Dictionary<Way, Dictionary<int, List<GraphVertex>>> _resolved_list;

//        /// <summary>
//        /// Holds the list of resolved vertices per id.
//        /// </summary>
//        private Dictionary<long, GraphVertex> _resolved_vertices;

//        /// <summary>
//        /// The interpreter for this graph.
//        /// /// </summary>
//        private GraphInterpreterBase _interpreter;

//        /// <summary>
//        /// Creates a new graph.
//        /// </summary>
//        /// <param name="interpreter"></param>
//        /// <param name="source"></param>
//        public Graph(GraphInterpreterBase interpreter,
//            IDataSourceReadOnly source)
//        {
//            _source = source;
//            _resolved_list = new Dictionary<Way, Dictionary<int, List<GraphVertex>>>();
//            _resolved_vertices = new Dictionary<long, GraphVertex>();
//            _interpreter = interpreter;
//        }

//        #region IGraphDirected Implementation

//        /// <summary>
//        /// Returns the vertex with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public GraphVertex GetVertex(long id)
//        {
//            GraphVertex vertex = null;
//            if (!_resolved_vertices.TryGetValue(id, out vertex))
//            {
//                vertex = new GraphVertex(_source.GetNode(id));
//            }
//            return vertex;
//        }

//        public Dictionary<long, float> GetNeighboursReversed(long vertex_id, HashSet<long> exceptions)
//        {
//            GraphVertex vertex = this.GetVertex(vertex_id);

//            // create the data list.
//            Dictionary<long, float> neighbours = new Dictionary<long, float>();

//            // find the vertex's edges.
//            IList<Way> edges = this.GetEdgesForVertex(vertex);

//            // calculate the weights to all neighbour nodes.
//            if (edges != null)
//            {
//                foreach (Way edge in edges)
//                {
//                    // determine if the edge can be traversed for the current interpreter.
//                    if (_interpreter.CanBeTraversed(edge))
//                    {
//                        // determine all the neigbours of the vertex on the given edge.
//                        IList<GraphVertex> vertices = this.GetNeighbourVerticesOnEdge(edge, vertex);
//                        foreach (GraphVertex neighbour in vertices)
//                        {
//                            // determine if the edge can be traversed from the source vertex to the neigbour.
//                            if ((exceptions == null || !exceptions.Contains(neighbour.Id))
//                                && _interpreter.CanBeTraversed(edge, neighbour.Node, vertex.Node))
//                            {
//                                // TODO: implement turn restrictions in router!

//                                float weight = _interpreter.Weight(edge, vertex, neighbour);
//                                // add the neighbour nodes to the neighbours list.
//                                neighbours[neighbour.Id] = weight;

//                            }
//                        }
//                    }
//                }
//            }

//            return neighbours;
//        }

//        public Dictionary<long, float> GetNeighboursUndirected(long vertex_id, HashSet<long> exceptions)
//        {
//            GraphVertex vertex = this.GetVertex(vertex_id);

//            // create the initial sorted list to contain the neigbours.
//            Dictionary<long, float>
//                    neighbours = new Dictionary<long, float>();

//            // find the vertex's edges.
//            IList<Way> edges = this.GetEdgesForVertex(vertex);

//            // calculate the weights to all neighbour nodes.
//            if(edges != null)
//            {
//                foreach (Way edge in edges)
//                {
//                    if (_interpreter.CanBeTraversed(edge))
//                    {
//                        // determine all the neigbours of the vertex on the given edge.
//                        IList<GraphVertex> vertices = this.GetNeighbourVerticesOnEdge(edge, vertex);
//                        foreach (GraphVertex neighbour in vertices)
//                        {
//                            // determine if the edge can be traversed from the source vertex to the neigbour.
//                            if (exceptions == null || !exceptions.Contains(neighbour.Node.Id))
//                            {
//                                float weight;
//                                if (_interpreter.CanBeTraversed(edge, vertex.Node, neighbour.Node))
//                                { // forward.
//                                    weight = _interpreter.Weight(edge, vertex, neighbour);
//                                    // add the neighbour nodes to the neighbours list.
//                                    neighbours[neighbour.Node.Id] = weight;
//                                }
//                                else if (_interpreter.CanBeTraversed(edge, neighbour.Node, vertex.Node))
//                                {// backward.
//                                    weight = _interpreter.Weight(edge, vertex, neighbour);
//                                    // add the neighbour nodes to the neighbours list.
//                                    neighbours[neighbour.Node.Id] = -weight;

//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            return neighbours;
//        }

//        public Dictionary<long, float> GetNeighbours(long vertex_id, HashSet<long> exceptions)
//        {
//            GraphVertex vertex = this.GetVertex(vertex_id);

//            // create the data list.
//            Dictionary<long, float> neighbours = new Dictionary<long, float>();

//            // find the vertex's edges.
//            IList<Way> edges = this.GetEdgesForVertex(vertex);

//            // calculate the weights to all neighbour nodes.
//            foreach (Way edge in edges)
//            {
//                // determine if the edge can be traversed for the current interpreter.
//                if (_interpreter.CanBeTraversed(edge))
//                {
//                    // determine all the neigbours of the vertex on the given edge.
//                    IList<GraphVertex> vertices = this.GetNeighbourVerticesOnEdge(edge, vertex);
//                    foreach (GraphVertex neighbour in vertices)
//                    {
//                        // determine if the edge can be traversed from the source vertex to the neigbour.
//                        if ((exceptions == null || !exceptions.Contains(neighbour.Id))
//                            && _interpreter.CanBeTraversed(edge, vertex.Node, neighbour.Node))
//                        {
//                            // TODO: implement turn restrictions in router!

//                            float weight = _interpreter.Weight(edge, vertex, neighbour);
//                            // add the neighbour nodes to the neighbours list.
//                            neighbours[neighbour.Id] = weight;

//                        }
//                    }
//                }
//            }

//            return neighbours;
//        }

//        #endregion

//        #region Osm-Specific Implementation

//        /// <summary>
//        /// Actually does the resolving.
//        /// </summary>
//        /// <param name="coordinate"></param>
//        /// <param name="search_radius"></param>
//        /// <param name="matcher"></param>
//        /// <param name="create_vertex"></param>
//        /// <returns></returns>
//        internal GraphVertex DoResolve(GeoCoordinate coordinate, double search_radius, IResolveMatcher matcher)
//        {
//            GraphVertex vertex = null;
//            GeoCoordinate return_coordinate = null;

//            // get the delta.
//            double delta = search_radius;

//            // initialize the result and distance.
//            OsmGeo result = null;
//            double distance = double.MaxValue;

//            while (vertex == null && ((matcher != null && delta <= search_radius * 2) || (matcher == null && delta < search_radius * 10)))
//            {
//                // construct a bounding box.
//                GeoCoordinate from_top = new GeoCoordinate(
//                    coordinate.Latitude + delta,
//                    coordinate.Longitude - delta);
//                GeoCoordinate from_bottom = new GeoCoordinate(
//                    coordinate.Latitude - delta,
//                    coordinate.Longitude + delta);

//                // query datasource.
//                IList<OsmBase> query_result =
//                    _source.Get(new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(from_top, from_bottom), OsmSharp.Osm.Filters.Filter.Any());

//                // loop over all found objects.
//                foreach (OsmBase base_obj in query_result)
//                {
//                    // test if the object is a pysical geo object.
//                    if (base_obj is OsmGeo)
//                    {
//                        // calculate the distance.
//                        double dist = (base_obj as OsmGeo).Shape.Distance(coordinate);

//                        // take the node if the distance is zero.
//                        if (dist < 0.000001 && (base_obj is Node))
//                        { // distance is zero and object is a node.
//                            return new GraphVertex(base_obj as Node);
//                        }
//                        else
//                        { // distance larger than zero or object is a way.
//                            // take the current object as the new found object 
//                            // if the distance is smaller and if the interpreter 
//                            // says it is routable.
//                            if (dist < distance)
//                            {
//                                // TODO: find a way to add nodes too: otherwise invalid GraphResolved objects will be generated.
//                                if (base_obj is Way)
//                                { // object is a way; see if the way is drivable.
//                                    Way way = base_obj as Way;
//                                    if (_interpreter.CanBeTraversed(way)
//                                        && _interpreter.CanBeStoppedOn(way))
//                                    {
//                                        bool match = true;
//                                        if (matcher != null)
//                                        {
//                                            match = matcher.Match(_interpreter.RoutingInterpreter.GetWayInterpretation(way.Tags));
//                                        }

//                                        if (match)
//                                        {
//                                            distance = dist;
//                                            result = way;
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }

//                // resolve the actual position.
//                if (result != null)
//                {
//                    Way closest_way = (result as Way);

//                    ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> shape =
//                        closest_way.Shape;

//                    DistanceResult<GeoCoordinate> distance_result = shape.DistanceDetailed(coordinate);
//                    if (distance_result is LineProjectionResult<GeoCoordinate>)
//                    { // projection was done on a line.
//                        LineProjectionResult<GeoCoordinate> line_projection =
//                            (distance_result as LineProjectionResult<GeoCoordinate>);

//                        // get the actual intersection point.
//                        GeoCoordinate intersection_point = line_projection.ClosestPrimitive as GeoCoordinate;

//                        // calculate the position.
//                        double latitude_diff = closest_way.Nodes[line_projection.Idx + 1].Coordinate.Latitude - closest_way.Nodes[line_projection.Idx].Coordinate.Latitude;
//                        double latitude_diff_small = intersection_point.Latitude - closest_way.Nodes[line_projection.Idx].Coordinate.Latitude;
//                        float position = (float)System.Math.Max(System.Math.Min((latitude_diff_small / latitude_diff), 1), 0);
//                        if (latitude_diff == 0 && latitude_diff_small == 0)
//                        {
//                            position = 0;
//                        }

//                        // create the result.
//                        if (position == 0)
//                        {
//                            vertex =
//                                new GraphVertex(closest_way.Nodes[line_projection.Idx]);
//                        }
//                        else if (position == 1)
//                        {
//                            vertex =
//                                new GraphVertex(closest_way.Nodes[line_projection.Idx + 1]);
//                        }
//                        else
//                        {
//                            vertex = new GraphVertex(
//                                new GraphResolved(closest_way, line_projection.Idx, position));
//                        }
//                    }
//                    else if (distance_result is PolygonProjectionResult<GeoCoordinate>)
//                    { // projection was done on a polygon.
//                        PolygonProjectionResult<GeoCoordinate> line_projection = (distance_result as PolygonProjectionResult<GeoCoordinate>);

//                        // get the actual intersection point.
//                        GeoCoordinate intersection_point = line_projection.ClosestPrimitive as GeoCoordinate;

//                        // calculate the position.
//                        double latitude_diff = closest_way.Nodes[line_projection.Idx + 1].Coordinate.Latitude - closest_way.Nodes[line_projection.Idx].Coordinate.Latitude;
//                        double latitude_diff_small = intersection_point.Latitude - closest_way.Nodes[line_projection.Idx].Coordinate.Latitude;
//                        float position = (float)System.Math.Max(System.Math.Min((latitude_diff_small / latitude_diff), 1), 0);

//                        // create the result.
//                        if (position == 0)
//                        {
//                            vertex =
//                                new GraphVertex(closest_way.Nodes[line_projection.Idx]);
//                        }
//                        else if (position == 1)
//                        {
//                            vertex =
//                                new GraphVertex(closest_way.Nodes[line_projection.Idx + 1]);
//                        }
//                        else
//                        {
//                            vertex = new GraphVertex(
//                                new GraphResolved(closest_way, line_projection.Idx, position));
//                        }
//                    }

//                    if (vertex.Type == GraphVertexEnum.Resolved)
//                    {
//                        Dictionary<int, List<GraphVertex>> vertices_dic = null;
//                        if (!_resolved_list.TryGetValue(closest_way, out vertices_dic))
//                        {
//                            vertices_dic = new Dictionary<int, List<GraphVertex>>();
//                            _resolved_list.Add(closest_way, vertices_dic);
//                            _resolved_vertices[vertex.Id] = vertex;
//                        }
//                        List<GraphVertex> vertices = null;
//                        if (!vertices_dic.TryGetValue(vertex.Resolved.Idx, out vertices))
//                        {
//                            vertices = new List<GraphVertex>();
//                            vertices_dic.Add(vertex.Resolved.Idx, vertices);
//                        }
//                        int index_of = vertices.IndexOf(vertex);
//                        if (index_of >= 0)
//                        {
//                            vertex = vertices[index_of];
//                        }
//                        else
//                        {
//                            vertices.Add(vertex);
//                            _resolved_vertices[vertex.Id] = vertex;
//                        }
//                    }
//                    else
//                    {
//                        return_coordinate = vertex.Coordinate;
//                    }
//                }

//                delta = delta * 2;
//            }

//            return vertex;
//        }
        
//        /// <summary>
//        /// Returns all the edges that contain the given vertex.
//        /// </summary>
//        /// <param name="vertex"></param>
//        /// <returns></returns>
//        public IList<Way> GetEdgesForVertex(GraphVertex vertex)
//        {
//            if (vertex.Type == GraphVertexEnum.Node)
//            {
//                return _source.GetWaysFor(vertex.Node);
//            }
//            else
//            {
//                List<Way> ways = new List<Way>();
//                ways.Add(vertex.Resolved.Way);
//                return ways;
//            }
//        }

//        /// <summary>
//        /// Returns all the vertices that are neighbours of the vertex on the given edge.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="vertex"></param>
//        /// <returns></returns>
//        public IList<GraphVertex> GetNeighbourVerticesOnEdge(Way edge, GraphVertex vertex)
//        {
//            IList<GraphVertex> neighbours = null;

//            if (vertex.Type == GraphVertexEnum.Resolved
//                && vertex.Resolved.Way != edge)
//            { // a graph vertex of type GraphVertexEnum.Resolved only has neighbours on the edge it exists on.
//                // if not the graph vertex is invalid.

//            }
//            else
//            { // the vertex is on the same edge.
//                if (vertex.Type == GraphVertexEnum.Resolved)
//                { // the vertex was resolved.
//                    neighbours = this.GetNeighbours(edge, vertex.Resolved.Idx, vertex.Resolved.Position);
//                }
//                else if (vertex.Type == GraphVertexEnum.Node)
//                { // the vertex is a regular node.
//                    neighbours = this.GetNeighbours(edge, vertex.Node);
//                }
//            }
//            return neighbours;
//        }

//        private List<GraphVertex> GetNeighbours(Way edge, int idx, float position)
//        {
//            List<GraphVertex> neighbours = new List<GraphVertex>();
//            if (position > 0)
//            {
//                // only try vertices at the same index until the vertex at idx+1;
//                List<GraphVertex> vertices = this.GetResolvedOn(edge, idx);

//                GraphVertex neighbour_max = new GraphVertex(edge.Nodes[idx + 1]); // at position 1
//                GraphVertex neighbour_min = new GraphVertex(edge.Nodes[idx]); // at position 0

//                if (vertices != null && vertices.Count > 1)
//                { // at least one vertex.
//                    float min = 1;
//                    float max = 1;
//                    foreach (GraphVertex vertex in vertices)
//                    {
//                        if (vertex.Resolved.Position < position)
//                        {
//                            float min_diff = position - vertex.Resolved.Position;
//                            if (min > min_diff)
//                            {
//                                min = min_diff;
//                                neighbour_min = vertex;
//                            }
//                        }
//                        else if (vertex.Resolved.Position > position)
//                        {
//                            float max_diff = vertex.Resolved.Position - position;
//                            if (max > max_diff)
//                            {
//                                max = max_diff;
//                                neighbour_max = vertex;
//                            }
//                        }
//                    }
//                }
//                neighbours.Add(neighbour_min);
//                neighbours.Add(neighbour_max);
//            }
//            else
//            { // test from idx-1 to idx+1 for neighbours.
//                if (idx > 0)
//                {
//                    // bottom
//                    GraphVertex bottom = new GraphVertex(edge.Nodes[idx - 1]);

//                    List<GraphVertex> vertices = this.GetResolvedOn(edge, idx - 1);
//                    if (vertices != null)
//                    {
//                        float max = 0;
//                        foreach (GraphVertex vertex in vertices)
//                        {
//                            if (max < vertex.Resolved.Position)
//                            {
//                                max = vertex.Resolved.Position;
//                                bottom = vertex;
//                            }
//                        }
//                    }

//                    neighbours.Add(bottom);
//                }
//                if (idx < edge.Nodes.Count - 1)
//                {
//                    // search top.
//                    GraphVertex top = new GraphVertex(edge.Nodes[idx + 1]);

//                    List<GraphVertex> vertices = this.GetResolvedOn(edge, idx);
//                    if (vertices != null)
//                    {
//                        float min = 1;
//                        foreach (GraphVertex vertex in vertices)
//                        {
//                            if (min > vertex.Resolved.Position)
//                            {
//                                min = vertex.Resolved.Position;
//                                top = vertex;
//                            }
//                        }
//                    }

//                    neighbours.Add(top);
//                }
//            }

//            return neighbours;
//        }

//        private IList<GraphVertex> GetNeighbours(Way edge, Node vertex)
//        {
//            List<GraphVertex> vertices = null;
//            for (int idx = 0; idx < edge.Nodes.Count; idx++)
//            {
//                Node node = edge.Nodes[idx];
//                if (node.Id == vertex.Id)
//                { // the node was found.
//                    if (vertices == null)
//                    {
//                        vertices =
//                            this.GetNeighbours(edge, idx, 0);
//                    }
//                    else
//                    {
//                        vertices.AddRange(
//                            this.GetNeighbours(edge, idx, 0));
//                    }
//                }
//            }
//            return vertices;
//        }

//        private List<GraphVertex> GetResolvedOn(Way edge, int idx)
//        {
//            Dictionary<int, List<GraphVertex>> vertices_dic = null;
//            if (_resolved_list.TryGetValue(edge, out vertices_dic))
//            {
//                List<GraphVertex> vertices = null;
//                if (vertices_dic.TryGetValue(idx, out vertices))
//                {
//                    return vertices;
//                }
//            }
//            return null;
//        }

//        /// <summary>
//        /// Returns all the neighbours with edges.
//        /// </summary>
//        /// <returns></returns>
//        public Dictionary<long, VertexAlongEdge> GetNeighboursWithEdges(long vertex_id, HashSet<long> exceptions)
//        {
//            GraphVertex vertex = this.GetVertex(vertex_id);

//            // create the data list.
//            Dictionary<long, VertexAlongEdge> neighbours =
//                new Dictionary<long, VertexAlongEdge>();

//            // find the vertex's edges.
//            IList<Way> edges = this.GetEdgesForVertex(vertex);

//            // calculate the weights to all neighbour nodes.
//            foreach (Way edge in edges)
//            {
//                // determine if the edge can be traversed for the current interpreter.
//                if (_interpreter.CanBeTraversed(edge))
//                {
//                    // determine all the neigbours of the vertex on the given edge.
//                    IList<GraphVertex> vertices = this.GetNeighbourVerticesOnEdge(edge, vertex);
//                    foreach (GraphVertex neighbour in vertices)
//                    {
//                        // determine if the edge can be traversed from the source vertex to the neigbour.
//                        if ((exceptions == null || !exceptions.Contains(neighbour.Id))
//                            && _interpreter.CanBeTraversed(edge, vertex.Node, neighbour.Node))
//                        {
//                            // TODO: implement turn restrictions in router!
//                            float weight = _interpreter.Weight(edge, vertex, neighbour);

//                            VertexAlongEdge vertex_along = new VertexAlongEdge(
//                                edge, neighbour, weight);

//                            // add the neighbour nodes to the neighbours list.
//                            neighbours[neighbour.Id] = vertex_along;
//                        }
//                    }
//                }
//            }

//            return neighbours;
//        }


//        /// <summary>
//        /// Returns all the neighbours with edges.
//        /// </summary>
//        /// <returns></returns>
//        public Dictionary<long, VertexAlongEdge> GetNeighboursReversedWithEdges(long vertex_id, HashSet<long> exceptions)
//        {
//            GraphVertex vertex = this.GetVertex(vertex_id);

//            // create the data list.
//            Dictionary<long, VertexAlongEdge> neighbours =
//                new Dictionary<long, VertexAlongEdge>();

//            // find the vertex's edges.
//            IList<Way> edges = this.GetEdgesForVertex(vertex);

//            // calculate the weights to all neighbour nodes.
//            foreach (Way edge in edges)
//            {
//                // determine if the edge can be traversed for the current interpreter.
//                if (_interpreter.CanBeTraversed(edge))
//                {
//                    // determine all the neigbours of the vertex on the given edge.
//                    IList<GraphVertex> vertices = this.GetNeighbourVerticesOnEdge(edge, vertex);
//                    foreach (GraphVertex neighbour in vertices)
//                    {
//                        // determine if the edge can be traversed from the source vertex to the neigbour.
//                        if ((exceptions == null || !exceptions.Contains(neighbour.Id))
//                            && _interpreter.CanBeTraversed(edge, neighbour.Node, vertex.Node))
//                        {
//                            // TODO: implement turn restrictions in router!
//                            float weight = _interpreter.Weight(edge, vertex, neighbour);

//                            VertexAlongEdge vertex_along = new VertexAlongEdge(
//                                edge, neighbour, weight);

//                            // add the neighbour nodes to the neighbours list.
//                            neighbours[neighbour.Id] = vertex_along;
//                        }
//                    }
//                }
//            }

//            return neighbours;
//        }


//        public Dictionary<long, VertexAlongEdge> GetNeighboursUndirectedWithEdges(long vertex_id, HashSet<long> exceptions)
//        {
//            GraphVertex vertex = this.GetVertex(vertex_id);

//            // create the initial sorted list to contain the neigbours.
//            Dictionary<long, VertexAlongEdge>
//                    neighbours = new Dictionary<long, VertexAlongEdge>();

//            // find the vertex's edges.
//            IList<Way> edges = this.GetEdgesForVertex(vertex);

//            // calculate the weights to all neighbour nodes.
//            foreach (Way edge in edges)
//            {
//                if (_interpreter.CanBeTraversed(edge))
//                {
//                    // determine all the neigbours of the vertex on the given edge.
//                    IList<GraphVertex> vertices = this.GetNeighbourVerticesOnEdge(edge, vertex);
//                    foreach (GraphVertex neighbour in vertices)
//                    {
//                        // determine if the edge can be traversed from the source vertex to the neigbour.
//                        if (exceptions == null || !exceptions.Contains(neighbour.Node.Id))
//                        {
//                            float weight;
//                            if (_interpreter.CanBeTraversed(edge, vertex.Node, neighbour.Node))
//                            { // forward.
//                                weight = _interpreter.Weight(edge, vertex, neighbour);

//                                VertexAlongEdge vertex_along = new VertexAlongEdge(
//                                    edge, neighbour, weight);

//                                // add the neighbour nodes to the neighbours list.
//                                neighbours[neighbour.Id] = vertex_along;
//                            }
//                            else if (_interpreter.CanBeTraversed(edge, neighbour.Node, vertex.Node))
//                            {// backward.
//                                weight = _interpreter.Weight(edge, vertex, neighbour);

//                                VertexAlongEdge vertex_along = new VertexAlongEdge(
//                                    edge, neighbour, -weight);

//                                // add the neighbour nodes to the neighbours list.
//                                neighbours[neighbour.Id] = vertex_along;

//                            }
//                        }
//                    }
//                }
//            }

//            return neighbours;
//        }

        

//        #endregion

//        #region Advanced Neighbour Information

//        /// <summary>
//        /// Returns the neigbours of the given vertex with extra info.
//        /// </summary>
//        /// <param name="vertex_id"></param>
//        /// <param name="exceptions"></param>
//        /// <returns></returns>
//        internal Dictionary<long, GraphNeighbourInfo> GetNeighboursAdvancedInfo(long vertex_id, HashSet<long> exceptions)
//        {
//            GraphVertex vertex = this.GetVertex(vertex_id);

//            // create the data list.
//            Dictionary<long, GraphNeighbourInfo> neighbours = new Dictionary<long, GraphNeighbourInfo>();

//            // find the vertex's edges.
//            IList<Way> edges = this.GetEdgesForVertex(vertex);

//            // calculate the weights to all neighbour nodes.
//            foreach (Way edge in edges)
//            {
//                // determine if the edge can be traversed for the current interpreter.
//                if (_interpreter.CanBeTraversed(edge))
//                {
//                    // determine all the neigbours of the vertex on the given edge.
//                    IList<GraphVertex> vertices = this.GetNeighbourVerticesOnEdge(edge, vertex);
//                    foreach (GraphVertex neighbour in vertices)
//                    {
//                        // determine if the edge can be traversed from the source vertex to the neigbour.
//                        if ((exceptions == null || !exceptions.Contains(neighbour.Id))
//                            && _interpreter.CanBeTraversed(edge, vertex.Node, neighbour.Node))
//                        {
//                            // create GraphNeighbourInfo
//                            GraphNeighbourInfo info = new GraphNeighbourInfo();
//                            info.Weight =  _interpreter.Weight(edge, vertex, neighbour);
//                            info.Tags = edge.Tags.ToList<KeyValuePair<string, string>>();

//                            // add the neighbour nodes to the neighbours list.
//                            neighbours[neighbour.Id] = info;

//                        }
//                    }
//                }
//            }

//            return neighbours;
//        }


//        #endregion
//    }

//    /// <summary>
//    /// Represents a position on a way.
//    /// </summary>
//    internal class GraphResolved
//    {
//        /// <summary>
//        /// Creates a new position on a way.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <param name="idx"></param>
//        /// <param name="position"></param>
//        public GraphResolved(Way way, int idx, float position)
//        {
//            this.Way = way;
//            this.Idx = idx;
//            // position should always be in range ]0,1[
//            if (position == 0 || position == 1 || position < 0 || position > 1)
//            {
//                throw new ArgumentOutOfRangeException("position should always be in range ]0,1[");
//            }
//            this.Position = position;
//        }

//        /// <summary>
//        /// The position between the node at index idx and at index idx+1.
//        /// 0: exact match node at idx.
//        /// 1: exact match node at idx+1.
//        /// </summary>
//        public float Position { get; private set; }

//        /// <summary>
//        /// The index of the first node of the segment matching this position.
//        /// </summary>
//        public int Idx { get; private set; }

//        /// <summary>
//        /// The way this position is on.
//        /// </summary>
//        public Way Way { get; private set; }

//        /// <summary>
//        /// Returns the neighbouring node(s).
//        /// </summary>
//        /// <returns></returns>
//        public IList<Node> GetNodes()
//        {
//            IList<Node> nodes = new List<Node>();
//            nodes.Add(this.Way.Nodes[this.Idx]);
//            nodes.Add(this.Way.Nodes[this.Idx + 1]);
//            return nodes;
//        }

//        /// <summary>
//        /// Returns the coordinate of the first node.
//        /// </summary>
//        public GeoCoordinate CoordinateFrom
//        {
//            get
//            {
//                return this.Way.Nodes[this.Idx].Coordinate;
//            }
//        }

//        /// <summary>
//        /// Returns the coordinate of the second node.
//        /// </summary>
//        public GeoCoordinate CoordinateTo
//        {
//            get
//            {
//                return this.Way.Nodes[this.Idx + 1].Coordinate;
//            }
//        }

//        /// <summary>
//        /// Returns the coordinate of this resolved point.
//        /// </summary>
//        public GeoCoordinate Coordinate
//        {
//            get
//            {
//                double latitude = this.CoordinateFrom.Latitude;
//                double longitude = this.CoordinateFrom.Longitude;

//                latitude = latitude + ((this.CoordinateTo.Latitude - this.CoordinateFrom.Latitude) * this.Position);
//                longitude = longitude + ((this.CoordinateTo.Longitude - this.CoordinateFrom.Longitude) * this.Position);
//                return new GeoCoordinate(latitude, longitude);
//            }
//        }

//        public override bool Equals(object obj)
//        {
//            if(base.Equals(obj))
//            {
//                return true;
//            }
//            if (obj is GraphResolved)
//            {
//                GraphResolved resolved = obj as GraphResolved;
//                if (resolved.Way == this.Way
//                    && resolved.Idx == this.Idx
//                    && resolved.Position == this.Position)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        public override int GetHashCode()
//        {
//            return this.Way.GetHashCode()
//                ^ this.Idx.GetHashCode()
//                ^ this.Position.GetHashCode();
//        }
//    }

//    /// <summary>
//    /// Represents general info about a neighbour of a node.
//    /// </summary>
//    internal class GraphNeighbourInfo : ITaggedObject
//    {

//        /// <summary>
//        /// The weight.
//        /// </summary>
//        public float Weight { get; set; }

//        /// <summary>
//        /// The tags.
//        /// </summary>
//        public List<KeyValuePair<string, string>> Tags { get; set; }
//    }
//}
