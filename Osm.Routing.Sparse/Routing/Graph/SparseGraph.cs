using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Graph;
using Tools.Math.Geo;
using Tools.Math.Shapes;
using Tools.Math.Geo.Factory;
using Tools.Math.Shapes.ResultHelpers;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Data.Core.Sparse;

namespace Osm.Routing.Sparse.Routing.Graph
{
    /// <summary>
    /// Sparse graph class.
    /// </summary>
    public class SparseGraph : IGraphDirected<SparseVertex>
    {
        /// <summary>
        /// Holds the data.
        /// </summary>
        private ISparseData _data;

        /// <summary>
        /// Creates a sparse graph based on the given data.
        /// </summary>
        /// <param name="data"></param>
        public SparseGraph(ISparseData data)
        {
            _resolved_vertices = new Dictionary<long, SparseVertex>();
            _resolved_simple_vertices = new Dictionary<long, SparseSimpleVertex>();
            _data = data;
        }

        #region Resolve Points

        /// <summary>
        /// Holds a counter 
        /// </summary>
        private int _next_id = -1;

        /// <summary>
        /// Holds the vertices that have been resolved.
        /// </summary>
        private IDictionary<long, SparseVertex> _resolved_vertices;

        /// <summary>
        /// Holds the simple vertices that have been changed by resolving.
        /// </summary>
        private IDictionary<long, SparseSimpleVertex> _resolved_simple_vertices;

        /// <summary>
        /// Returns the simple vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private SparseSimpleVertex GetResolvedSimpleVertex(long id)
        {
            SparseSimpleVertex found_vertex;
            if (!_resolved_simple_vertices.TryGetValue(id, out found_vertex))
            {
                found_vertex = _data.GetSparseSimpleVertex(id);
            }
            return found_vertex;
        }

        /// <summary>
        /// Returns the sparse vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private SparseVertex GetResolvedSparseVertex(long id)
        {
            SparseVertex vertex = null;
            if (!_resolved_vertices.TryGetValue(id, out vertex))
            {
                vertex = _data.GetSparseVertex(id);
            }
            return vertex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        private void AddResolvedSparseVertex(SparseVertex vertex)
        {
            _resolved_vertices[vertex.Id] = vertex;

            SparseSimpleVertex vertex_new_simple = new SparseSimpleVertex();
            vertex_new_simple.Id = vertex.Id;
            vertex_new_simple.Latitude = vertex.Location.Latitude;
            vertex_new_simple.Longitude = vertex.Location.Longitude;
            vertex_new_simple.Neighbour1 = 0;
            vertex_new_simple.Neighbour2 = 0;

            _resolved_simple_vertices[vertex_new_simple.Id] = vertex_new_simple;
        }

        /// <summary>
        /// Resolves a point and returns the closest vertex.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public SparseVertex Resolve(GeoCoordinate coordinate, double search_radius, ISparseVertexMatcher matcher)
        {
            // get the delta.
            double delta = search_radius;

            // initialize the result and distance.
            PotentialResolvedHit best_hit = new PotentialResolvedHit();
            best_hit.Distance = double.MaxValue;

            // keep searching until at least one closeby hit is found.
            while (best_hit.Distance == double.MaxValue && ((matcher != null && delta <= search_radius * 2) 
                || (matcher == null && delta < search_radius * 200)))
            {
                // construct a bounding box.
                GeoCoordinate from_top = new GeoCoordinate(
                    coordinate.Latitude + delta,
                    coordinate.Longitude - delta);
                GeoCoordinate from_bottom = new GeoCoordinate(
                    coordinate.Latitude - delta,
                    coordinate.Longitude + delta);

                // query datasource.
                IList<SparseSimpleVertex> query_result =
                    this.GetSimpleVertices(new Tools.Math.Geo.GeoCoordinateBox(from_top, from_bottom));
                foreach (SparseSimpleVertex query_vertex in query_result)
                {
                    // make sure to have the local version.
                    SparseSimpleVertex found_vertex = this.GetResolvedSimpleVertex(query_vertex.Id);

                    // get the neigbours.
                    SparseVertex vertex1 = null;
                    if (found_vertex.Neighbour1 != 0 && found_vertex.Neighbour2 != 0)
                    { // the vertex found is not sparse.

                        // resolve between the two neighbours.
                        PotentialResolvedHit potential_hit = this.ResolveBetween(coordinate, found_vertex.Neighbour1, found_vertex.Neighbour2,
                            null, null);

                        // keep the result only if it is better.
                        if (potential_hit.Distance < best_hit.Distance)
                        {
                            best_hit = potential_hit;
                        }
                    }
                    else
                    { // the vertex found is sparse.
                        vertex1 = this.GetResolvedSparseVertex(query_vertex.Id);

                        // resolve the vertex itself.
                        double result_distance = vertex1.Location.DistanceEstimate(coordinate).Value;
                        if (result_distance <= best_hit.Distance)
                        {
                            best_hit = new PotentialResolvedHit();
                            best_hit.Distance = result_distance;
                            best_hit.Vertex = vertex1;
                        }

                        // resolve all paths to all it's neighbours.
                        foreach (SparseVertexNeighbour neighbour in vertex1.Neighbours)
                        {
                            PotentialResolvedHit potential_hit = this.ResolveBetween(coordinate,
                                vertex1.Id, neighbour.Id,
                                vertex1, null);

                            // keep the result only if it is better.
                            if (potential_hit.Distance < best_hit.Distance)
                            {
                                best_hit = potential_hit;
                            }
                        }
                    }
                }

                // get the resolved versions.

                delta = delta * 2;
            }

            // process the best result.
            SparseVertex result;
            if (best_hit.Vertex != null)
            { // the best result was a sparse vertex.
                result = best_hit.Vertex;
            }
            else
            { // the best result was somewhere in between sparse vertices.
                LineProjectionResult<GeoCoordinate> line_projection = best_hit.LineProjection;
                SparseVertex neighbour1 = best_hit.Neighbour1;
                SparseVertexNeighbour neighbour1_neighbour = best_hit.Neighbour1Neighbour;
                SparseVertex neighbour2 = best_hit.Neighbour2;

                // get the actual intersection point.
                GeoCoordinate intersection_point = line_projection.ClosestPrimitive as GeoCoordinate;

                // rebuild the points list.
                List<GeoCoordinate> points = new List<GeoCoordinate>();
                points.Add(neighbour1.Location);
                foreach (long to_id in neighbour1_neighbour.Nodes)
                {
                    // get the to_vertex.
                    SparseSimpleVertex to_vertex = this.GetResolvedSimpleVertex(to_id);
                    GeoCoordinate to = new GeoCoordinate(to_vertex.Latitude, to_vertex.Longitude);
                    points.Add(to);
                }
                points.Add(neighbour2.Location);

                // process the projection.
                double latitude_diff = points[line_projection.Idx + 1].Latitude - points[line_projection.Idx].Latitude;
                double latitude_diff_small = intersection_point.Latitude - points[line_projection.Idx].Latitude;
                float position = (float)System.Math.Max(System.Math.Min((latitude_diff_small / latitude_diff), 1), 0);
                if (latitude_diff == 0 && latitude_diff_small == 0)
                {
                    position = 0;
                }

                // create the result.
                if (position == 0)
                {
                    // the position is at the first node @ line_projection.Idx.
                    if (line_projection.Idx == 0)
                    { // the closest one is the sparse vertex.
                        result = neighbour1;
                    }
                    else
                    { // the closest one is another vertex.
                        // get the neighbour2_neighbour for the bidirectional case.
                        SparseVertexNeighbour neighbour2_neighbour = neighbour2.GetSparseVertexNeighbour(neighbour1.Id);

                        // build the sparse vertex.
                        result = new SparseVertex();
                        result.Id = neighbour1_neighbour.Nodes[line_projection.Idx - 1];
                        result.Coordinates = new double[2];
                        result.Coordinates[0] = intersection_point.Latitude;
                        result.Coordinates[1] = intersection_point.Longitude;

                        // add the neighbours.
                        List<SparseVertexNeighbour> neighbours = this.BuildNeighbours(neighbour1, neighbour2,
                            neighbour1_neighbour, neighbour2_neighbour, result.Id);
                        result.Neighbours = neighbours.ToArray();

                        // add to the resolved list.
                        this.AddResolvedSparseVertex(result);
                        //_resolved_vertices.Add(result.Id, result);

                        // adapt the two neighbours.
                        SparseVertex neighbour1_new = this.AdaptVertex(neighbour1, neighbour1_neighbour, result.Id);
                        this.AddResolvedSparseVertex(neighbour1_new);
                        //_resolved_vertices[neighbour1_new.Id] = neighbour1_new;

                        //SimpleVertex neighbour1_new_simple = new SimpleVertex();
                        //neighbour1_new_simple.Id = neighbour1_new.Id;
                        //neighbour1_new_simple.Latitude = neighbour1_new.Location.Latitude;
                        //neighbour1_new_simple.Longitude = neighbour1_new.Location.Longitude;
                        //neighbour1_new_simple.Neighbour1 = 0;
                        //neighbour1_new_simple.Neighbour2 = 0;

                        //_resolved_simple_vertices[neighbour1_new_simple.Id] = neighbour1_new_simple;

                        if (neighbour2_neighbour != null)
                        {
                            SparseVertex neighbour2_new = this.AdaptVertex(neighbour2, neighbour2_neighbour, result.Id);
                            this.AddResolvedSparseVertex(neighbour2_new);
                            //_resolved_vertices[neighbour2_new.Id] = neighbour2_new;

                            //SimpleVertex neighbour2_new_simple = new SimpleVertex();
                            //neighbour2_new_simple.Id = neighbour2_new.Id;
                            //neighbour2_new_simple.Latitude = neighbour2_new.Location.Latitude;
                            //neighbour2_new_simple.Longitude = neighbour2_new.Location.Longitude;
                            //neighbour2_new_simple.Neighbour1 = 0;
                            //neighbour2_new_simple.Neighbour2 = 0;

                            //_resolved_simple_vertices[neighbour2_new_simple.Id] = neighbour2_new_simple;
                        }
                    }
                }
                else if (position == 1)
                {
                    // the position is at the first node @ line_projection.Idx + 1.
                    if (line_projection.Idx == neighbour1_neighbour.Nodes.Length)
                    { // the closest node is neighbour2
                        result = neighbour2;
                    }
                    else
                    { // the closest one is another vertex.
                        // get the neighbour2_neighbour for the bidirectional case.
                        SparseVertexNeighbour neighbour2_neighbour = neighbour2.GetSparseVertexNeighbour(neighbour1.Id);

                        // build the sparse vertex.
                        result = new SparseVertex();
                        result.Id = neighbour1_neighbour.Nodes[line_projection.Idx];
                        result.Coordinates = new double[2];
                        result.Coordinates[0] = intersection_point.Latitude;
                        result.Coordinates[1] = intersection_point.Longitude;
                        
                        // add the neighbours.
                        List<SparseVertexNeighbour> neighbours = this.BuildNeighbours(neighbour1, neighbour2,
                            neighbour1_neighbour, neighbour2_neighbour, result.Id);
                        result.Neighbours = neighbours.ToArray();

                        // add to the resolved list.
                        this.AddResolvedSparseVertex(result);
                        //_resolved_vertices.Add(result.Id, result);

                        // adapt the two neighbours.
                        SparseVertex neighbour1_new = this.AdaptVertex(neighbour1, neighbour1_neighbour, result.Id);
                        this.AddResolvedSparseVertex(neighbour1_new);
                        //_resolved_vertices[neighbour1_new.Id] = neighbour1_new;

                        //SimpleVertex neighbour1_new_simple = new SimpleVertex();
                        //neighbour1_new_simple.Id = neighbour1_new.Id;
                        //neighbour1_new_simple.Latitude = neighbour1_new.Location.Latitude;
                        //neighbour1_new_simple.Longitude = neighbour1_new.Location.Longitude;
                        //neighbour1_new_simple.Neighbour1 = 0;
                        //neighbour1_new_simple.Neighbour2 = 0;

                        //_resolved_simple_vertices[neighbour1_new_simple.Id] = neighbour1_new_simple;

                        if (neighbour2_neighbour != null)
                        {
                            SparseVertex neighbour2_new = this.AdaptVertex(neighbour2, neighbour2_neighbour, result.Id);
                            this.AddResolvedSparseVertex(neighbour2_new);
                            //_resolved_vertices[neighbour2_new.Id] = neighbour2_new;

                            //SimpleVertex neighbour2_new_simple = new SimpleVertex();
                            //neighbour2_new_simple.Id = neighbour2_new.Id;
                            //neighbour2_new_simple.Latitude = neighbour2_new.Location.Latitude;
                            //neighbour2_new_simple.Longitude = neighbour2_new.Location.Longitude;
                            //neighbour2_new_simple.Neighbour1 = 0;
                            //neighbour2_new_simple.Neighbour2 = 0;

                            //_resolved_simple_vertices[neighbour2_new_simple.Id] = neighbour2_new_simple;
                        }
                    }
                }
                else
                { // the best location is not located at a simple vertex nor at a sparse vertex.
                    // get the neighbour2_neighbour for the bidirectional case.
                    SparseVertexNeighbour neighbour2_neighbour = neighbour2.GetSparseVertexNeighbour(neighbour1.Id);

                    // the position is somewhere in between.
                    result = new SparseVertex();
                    result.Id = _next_id;
                    _next_id--;
                    result.Coordinates = new double[2];
                    result.Coordinates[0] = intersection_point.Latitude;
                    result.Coordinates[1] = intersection_point.Longitude;

                    // get the node_before and node_after.
                    long node_before;
                    if (line_projection.Idx == 0)
                    {
                        node_before = neighbour1.Id;
                    }
                    else
                    {
                        node_before = neighbour1_neighbour.Nodes[line_projection.Idx - 1];
                    }
                    long node_after;
                    if (line_projection.Idx == neighbour1_neighbour.Nodes.Length)
                    {
                        node_after = neighbour2.Id;
                    }
                    else
                    {
                        node_after = neighbour1_neighbour.Nodes[line_projection.Idx];
                    }

                    // add the neighbours.
                    List<SparseVertexNeighbour> neighbours = this.BuildNeighbours(neighbour1, neighbour2,
                        neighbour1_neighbour, neighbour2_neighbour, node_before, node_after, position, result.Id);
                    result.Neighbours = neighbours.ToArray();

                    // add to the resolved list.
                    //_resolved_vertices.Add(result.Id, result);
                    this.AddResolvedSparseVertex(result);

                    // adapt the two neighbours.
                    SparseVertex neighbour1_new = this.AdaptVertex(neighbour1, neighbour1_neighbour, node_before, node_after, position, result.Id);
                    this.AddResolvedSparseVertex(neighbour1_new);
                    //_resolved_vertices[neighbour1_new.Id] = neighbour1_new;

                    //SimpleVertex neighbour1_new_simple = new SimpleVertex();
                    //neighbour1_new_simple.Id = neighbour1_new.Id;
                    //neighbour1_new_simple.Latitude = neighbour1_new.Location.Latitude;
                    //neighbour1_new_simple.Longitude = neighbour1_new.Location.Longitude;
                    //neighbour1_new_simple.Neighbour1 = 0;
                    //neighbour1_new_simple.Neighbour2 = 0;

                    //_resolved_simple_vertices[neighbour1_new_simple.Id] = neighbour1_new_simple;

                    if (neighbour2_neighbour != null)
                    {
                        SparseVertex neighbour2_new = this.AdaptVertex(neighbour2, neighbour2_neighbour, node_after, node_before, 1.0 - position, result.Id);
                        this.AddResolvedSparseVertex(neighbour2_new);
                        //_resolved_vertices[neighbour2_new.Id] = neighbour2_new;

                        //SimpleVertex neighbour2_new_simple = new SimpleVertex();
                        //neighbour2_new_simple.Id = neighbour2_new.Id;
                        //neighbour2_new_simple.Latitude = neighbour2_new.Location.Latitude;
                        //neighbour2_new_simple.Longitude = neighbour2_new.Location.Longitude;
                        //neighbour2_new_simple.Neighbour1 = 0;
                        //neighbour2_new_simple.Neighbour2 = 0;

                        //_resolved_simple_vertices[neighbour2_new_simple.Id] = neighbour2_new_simple;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Queries for nearby vertices and make sure the resolved versions get priority.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        private IList<SparseSimpleVertex> GetSimpleVertices(GeoCoordinateBox box)
        {
            // query the datasource.
            IList<SparseSimpleVertex> query_result = _data.GetSparseSimpleVertices(box);

            // query the local resolved sources.
            Dictionary<long, SparseSimpleVertex> found_vertices = new Dictionary<long, SparseSimpleVertex>();
            foreach (KeyValuePair<long, SparseSimpleVertex> pair in _resolved_simple_vertices)
            {
                if (box.IsInside(new GeoCoordinate(pair.Value.Latitude,
                    pair.Value.Longitude)))
                { // the element is also inside of the query range.
                    found_vertices[pair.Key] = pair.Value;
                }
            }

            // loop over all query results and 
            List<SparseSimpleVertex> vertices = new List<SparseSimpleVertex>(query_result.Count);
            foreach (SparseSimpleVertex vertex in query_result)
            {
                SparseSimpleVertex found;
                if (found_vertices.TryGetValue(vertex.Id, out found))
                {
                    found_vertices.Remove(vertex.Id);
                    vertices.Add(found);
                }
                else if (_resolved_simple_vertices.TryGetValue(vertex.Id, out found))
                {
                    vertices.Add(found);
                }
                else
                {
                    vertices.Add(vertex);
                }
            }

            // add the un-added found vertices.
            foreach (KeyValuePair<long, SparseSimpleVertex> pair in found_vertices)
            {
                vertices.Add(pair.Value);
            }

            return vertices;
        }

        /// <summary>
        /// Resolves the closest point between the given neighbours.
        /// </summary>
        /// <param name="neighbour1_id"></param>
        /// <param name="neighbour2_id"></param>
        /// <param name="neighbour1"></param>
        /// <param name="neighbour2"></param>
        /// <returns></returns>
        private PotentialResolvedHit ResolveBetween(GeoCoordinate coordinate, long neighbour1_id, long neighbour2_id,
            SparseVertex neighbour1, SparseVertex neighbour2)
        {
            // first neighbour.
            if (neighbour1 == null)
            {
                neighbour1 = this.GetResolvedSparseVertex(neighbour1_id);
            }
            SparseVertexNeighbour neighbour1_neighbour = neighbour1.GetSparseVertexNeighbour(neighbour2_id);

            // second neighbour.
            if (neighbour2 == null)
            {
                neighbour2 = this.GetResolvedSparseVertex(neighbour2_id);
            }

            if (neighbour1_neighbour != null && neighbour2 != null)
            {
                // intersect with the neighbours line.
                List<GeoCoordinate> points = new List<GeoCoordinate>();
                points.Add(neighbour1.Location);
                foreach (long to_id in neighbour1_neighbour.Nodes)
                {
                    // get the to_vertex.
                    SparseSimpleVertex to_vertex = this.GetResolvedSimpleVertex(to_id);
                    GeoCoordinate to = new GeoCoordinate(to_vertex.Latitude, to_vertex.Longitude);
                    points.Add(to);
                }
                points.Add(neighbour2.Location);

                // define the line and intersect.
                ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line =
                    new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(PrimitiveGeoFactory.Instance,
                        points.ToArray());
                LineProjectionResult<GeoCoordinate> line_projection =
                    (line.DistanceDetailed(coordinate) as LineProjectionResult<GeoCoordinate>);

                PotentialResolvedHit best_hit = new PotentialResolvedHit();
                best_hit.Distance = line_projection.Distance;
                best_hit.LineProjection = line_projection;
                //best_hit.SimpleVertex = found_vertex;
                best_hit.Neighbour1 = neighbour1;
                best_hit.Neighbour1Neighbour = neighbour1_neighbour;
                best_hit.Neighbour2 = neighbour2;
                return best_hit;
            }
            else
            {
                PotentialResolvedHit best_hit = new PotentialResolvedHit();
                best_hit.Distance = double.MaxValue;

                return best_hit;
            }
        }


        private struct PotentialResolvedHit
        {
            public double Distance { get; set; }

            public SparseVertex Vertex { get; set; }

            #region Non-Direct hits

            public SparseSimpleVertex SimpleVertex { get; set; }

            public LineProjectionResult<GeoCoordinate> LineProjection { get; set; }

            public SparseVertex Neighbour1 { get; set; }

            public SparseVertexNeighbour Neighbour1Neighbour { get; set; }

            public SparseVertex Neighbour2 { get; set; }

            #endregion
        }

        /// <summary>
        /// Adapts the given vertex and it's neighbour to replace the old neighbour to the new one.
        /// </summary>
        /// <param name="neighbour1"></param>
        /// <param name="neighbour1_neighbour"></param>
        /// <param name="node_id"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private SparseVertex AdaptVertex(SparseVertex neighbour1, SparseVertexNeighbour neighbour1_neighbour, 
            long node_id)
        {
            SparseVertex vertex = new SparseVertex();
            vertex.Id = neighbour1.Id;
            vertex.Coordinates = new double[2];
            vertex.Coordinates[0] = neighbour1.Coordinates[0];
            vertex.Coordinates[1] = neighbour1.Coordinates[1];
            vertex.Neighbours = new SparseVertexNeighbour[neighbour1.Neighbours.Length];
            for (int idx = 0; idx < neighbour1.Neighbours.Length; idx++)
            {
                if (neighbour1.Neighbours[idx].Id == neighbour1_neighbour.Id)
                {
                    // adjust this neighbour.
                    SparseVertexNeighbour neighbour = new SparseVertexNeighbour();
                    neighbour.Id = node_id;
                    neighbour.Tags = neighbour1.Neighbours[idx].Tags;                    

                    // copy the nodes.
                    double distance_total = 0;
                    double distance_local = 0;
                    bool add = true;
                    List<long> nodes = new List<long>();
                    GeoCoordinate previous_coordinate = neighbour1.Location;
                    for (int node_idx = 0; node_idx < neighbour1.Neighbours[idx].Nodes.Length; node_idx++)
                    {
                        long node = neighbour1.Neighbours[idx].Nodes[node_idx];
                        SparseSimpleVertex node_vertex = this.GetResolvedSimpleVertex(node);
                        GeoCoordinate local_coordinate = new GeoCoordinate(node_vertex.Latitude, node_vertex.Longitude);
                        double distance = previous_coordinate.DistanceEstimate(local_coordinate).Value;
                        distance_total = distance_total + distance;
                        if (node == node_id)
                        {
                            add = false;
                        }
                        if (add)
                        {
                            distance_local = distance_local + distance;
                            nodes.Add(node);
                        }
                    }

                    double ratio = 0;
                    if (distance_total > 0)
                    {
                        ratio = distance_local / distance_total;
                    }

                    neighbour.Nodes = nodes.ToArray();
                    neighbour.Weight = neighbour1.Neighbours[idx].Weight * ratio;

                    vertex.Neighbours[idx] = neighbour;
                }
                else
                {
                    vertex.Neighbours[idx] = neighbour1.Neighbours[idx];
                }
            }
            return vertex;
        }

        private SparseVertex AdaptVertex(SparseVertex neighbour1, SparseVertexNeighbour neighbour1_neighbour, 
            long node_before_id, long node_after_id, double position, long node_id)
        {
            // calculate the distance between.
            SparseSimpleVertex node_before = this.GetResolvedSimpleVertex(node_before_id);
            SparseSimpleVertex node_after = this.GetResolvedSimpleVertex(node_after_id);
            double weight_between = (new GeoCoordinate(node_before.Latitude, node_before.Longitude)).DistanceEstimate(
                new GeoCoordinate(node_after.Latitude, node_after.Longitude)).Value;

            SparseVertex vertex = new SparseVertex();
            vertex.Id = neighbour1.Id;
            vertex.Coordinates = new double[2];
            vertex.Coordinates[0] = neighbour1.Coordinates[0];
            vertex.Coordinates[1] = neighbour1.Coordinates[1];
            vertex.Neighbours = new SparseVertexNeighbour[neighbour1.Neighbours.Length];
            for (int idx = 0; idx < neighbour1.Neighbours.Length; idx++)
            {
                if (neighbour1.Neighbours[idx].Id == neighbour1_neighbour.Id)
                {
                    // adjust this neighbour.
                    SparseVertexNeighbour neighbour = new SparseVertexNeighbour();
                    neighbour.Id = node_id;
                    neighbour.Tags = neighbour1.Neighbours[idx].Tags;

                    // copy the nodes.
                    double distance_total = 0;
                    double distance_local = 0;
                    bool add = (node_before_id != neighbour1.Id);
                    List<long> nodes = new List<long>();
                    GeoCoordinate previous_coordinate = neighbour1.Location;
                    for (int node_idx = 0; node_idx < neighbour1.Neighbours[idx].Nodes.Length; node_idx++)
                    {
                        long node = neighbour1.Neighbours[idx].Nodes[node_idx];
                        SparseSimpleVertex node_vertex = this.GetResolvedSimpleVertex(node);
                        GeoCoordinate local_coordinate = new GeoCoordinate(node_vertex.Latitude, node_vertex.Longitude);
                        double distance = previous_coordinate.DistanceEstimate(local_coordinate).Value;
                        distance_total = distance_total + distance;
                        if (add)
                        {
                            distance_local = distance_local + distance;
                            nodes.Add(node);
                        }

                        if (node == node_before_id)
                        {
                            add = false;
                        }

                        previous_coordinate = local_coordinate;
                    }

                    double ratio = 0;
                    double ratio_position = 0;
                    if (distance_total > 0)
                    {
                        ratio = distance_local / distance_total;
                        ratio_position = weight_between / distance_total;
                    }

                    neighbour.Nodes = nodes.ToArray();
                    neighbour.Weight = (neighbour1.Neighbours[idx].Weight * ratio) + ((neighbour1.Neighbours[idx].Weight * ratio_position) * position);

                    vertex.Neighbours[idx] = neighbour;
                }
                else
                {
                    vertex.Neighbours[idx] = neighbour1.Neighbours[idx];
                }
            }
            return vertex;
        }
        
        
        /// <summary>
        /// Builds a sparse neighbours to a node between the from and to vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="neighbour_from_to"></param>
        /// <returns></returns>
        private List<SparseVertexNeighbour> BuildNeighbours(SparseVertex from, SparseVertex to, SparseVertexNeighbour from_to_neighbour, SparseVertexNeighbour to_from_neighbour, 
            long node_id)
        {
            List<SparseVertexNeighbour> neighbours = new List<SparseVertexNeighbour>();

            List<long> nodes; ;
            bool add;
            GeoCoordinate previous_location;
            double total_distance;
            double cut_distance;
            double ratio;

            // calculate the sparseneighbour from to to from (if any).
            if (to_from_neighbour != null)
            { // the edge(s) between to and from are not directed.
                // build the neighbour that start a the given node to the from node.
                SparseVertexNeighbour result_neighbour1 = new SparseVertexNeighbour();
                result_neighbour1.Id = from.Id;
                result_neighbour1.Tags = to_from_neighbour.Tags; // copy the same tags.
                
                // initialize all variables.
                nodes = new List<long>();
                add = false;
                total_distance = 0;
                cut_distance = 0;
                previous_location = to.Location;
                
                // loop voer all nodes; calculate the nodes list and the distances.
                for (int idx = 0; idx < to_from_neighbour.Nodes.Length; idx++)
                {
                    // get the simple vertex.
                    SparseSimpleVertex local_vertex = this.GetResolvedSimpleVertex(to_from_neighbour.Nodes[idx]);
                    GeoCoordinate local_position = new GeoCoordinate(local_vertex.Latitude, local_vertex.Longitude);

                    // calculate the distance.
                    double local_distance = previous_location.DistanceReal(local_position).Value;
                    previous_location = local_position;
                    total_distance = total_distance + local_distance;
                    if (to_from_neighbour.Nodes[idx] == node_id)
                    {
                        add = true;
                    }
                    if (add)
                    {
                        cut_distance = local_distance + cut_distance;
                        nodes.Add(to_from_neighbour.Nodes[idx]);

                        SparseSimpleVertex simple_vertex = new SparseSimpleVertex();
                        simple_vertex.Id = to_from_neighbour.Nodes[idx];
                        simple_vertex.Latitude = local_position.Latitude;
                        simple_vertex.Longitude = local_position.Longitude;
                        simple_vertex.Neighbour1 = node_id;
                        simple_vertex.Neighbour2 = result_neighbour1.Id;

                        _resolved_simple_vertices[simple_vertex.Id] = simple_vertex;
                    }
                }
                total_distance = total_distance + previous_location.DistanceReal(from.Location).Value;
                result_neighbour1.Nodes = nodes.ToArray();

                // update the weight assuming it is lineair to the distance.
                ratio = 0;
                if (total_distance != 0)
                {
                    ratio = cut_distance / total_distance;
                }
                result_neighbour1.Weight = to_from_neighbour.Weight * ratio;

                neighbours.Add(result_neighbour1);
            }

            // calculate the sparseneighbour from from to to.
            SparseVertexNeighbour result_neighbour2 = new SparseVertexNeighbour();
            result_neighbour2.Id = to.Id;
            result_neighbour2.Tags = from_to_neighbour.Tags; // copy the same tags.

            // initialize all variables.
            nodes = new List<long>();
            add = false;
            total_distance = 0;
            cut_distance = 0;
            previous_location = from.Location;

            // loop voer all nodes; calculate the nodes list and the distances.
            for (int idx = 0; idx < from_to_neighbour.Nodes.Length; idx++)
            {
                // get the simple vertex.
                SparseSimpleVertex local_vertex = this.GetResolvedSimpleVertex(from_to_neighbour.Nodes[idx]);
                GeoCoordinate local_position = new GeoCoordinate(local_vertex.Latitude, local_vertex.Longitude);

                // calculate the distance.
                double local_distance = previous_location.DistanceReal(local_position).Value;
                previous_location = local_position;
                total_distance = total_distance + local_distance;
                if (from_to_neighbour.Nodes[idx] == node_id)
                {
                    add = true;
                }
                if (add)
                {
                    cut_distance = local_distance + cut_distance;
                    nodes.Add(from_to_neighbour.Nodes[idx]);

                    SparseSimpleVertex simple_vertex = new SparseSimpleVertex();
                    simple_vertex.Id = from_to_neighbour.Nodes[idx];
                    simple_vertex.Latitude = local_position.Latitude;
                    simple_vertex.Longitude = local_position.Longitude;
                    simple_vertex.Neighbour1 = node_id;
                    simple_vertex.Neighbour2 = result_neighbour2.Id;

                    _resolved_simple_vertices[simple_vertex.Id] = simple_vertex;
                }
            }
            total_distance = total_distance + previous_location.DistanceReal(from.Location).Value;
            result_neighbour2.Nodes = nodes.ToArray();

            // update the weight assuming it is lineair to the distance.
            ratio = 0;
            if (total_distance != 0)
            {
                ratio = cut_distance / total_distance;
            }
            result_neighbour2.Weight = from_to_neighbour.Weight * ratio;

            neighbours.Add(result_neighbour2);

            return neighbours;
        }


        /// <summary>
        /// Builds a sparse neighbours to a node between the from and to vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="neighbour_from_to"></param>
        /// <returns></returns>
        private List<SparseVertexNeighbour> BuildNeighbours(SparseVertex from, SparseVertex to, SparseVertexNeighbour from_to_neighbour, SparseVertexNeighbour to_from_neighbour,
            long node_before_id, long node_after_id, double position, long node_id)
        {
            List<SparseVertexNeighbour> neighbours = new List<SparseVertexNeighbour>();

            // calculate the distance between.
            SparseSimpleVertex node_before = this.GetResolvedSimpleVertex(node_before_id);
            SparseSimpleVertex node_after = this.GetResolvedSimpleVertex(node_after_id);
            double weight_between = (new GeoCoordinate(node_before.Latitude, node_before.Longitude)).DistanceEstimate(
                new GeoCoordinate(node_after.Latitude, node_after.Longitude)).Value;

            List<long> nodes;
            bool add;
            GeoCoordinate previous_location;
            double total_distance;
            double cut_distance;
            double ratio;
            double ratio_position;

            // calculate the sparseneighbour from to to from (if any).
            if (to_from_neighbour != null)
            { // the edge(s) between to and from are not directed.
                // build the neighbour that start a the given node to the from node.
                SparseVertexNeighbour result_neighbour1 = new SparseVertexNeighbour();
                result_neighbour1.Id = from.Id;
                result_neighbour1.Tags = to_from_neighbour.Tags; // copy the same tags.

                // initialize all variables.
                nodes = new List<long>();
                add = false;
                total_distance = 0;
                cut_distance = 0;
                previous_location = to.Location;

                // loop voer all nodes; calculate the nodes list and the distances.
                for (int idx = 0; idx < to_from_neighbour.Nodes.Length; idx++)
                {
                    // get the simple vertex.
                    SparseSimpleVertex local_vertex = this.GetResolvedSimpleVertex(to_from_neighbour.Nodes[idx]);
                    GeoCoordinate local_position = new GeoCoordinate(local_vertex.Latitude, local_vertex.Longitude);

                    // calculate the distance.
                    double local_distance = previous_location.DistanceReal(local_position).Value;
                    previous_location = local_position;
                    total_distance = total_distance + local_distance;
                    if (to_from_neighbour.Nodes[idx] == node_before_id)
                    {
                        add = true;
                    }
                    if (add)
                    {
                        cut_distance = local_distance + cut_distance;
                        nodes.Add(to_from_neighbour.Nodes[idx]);

                        SparseSimpleVertex simple_vertex = new SparseSimpleVertex();
                        simple_vertex.Id = to_from_neighbour.Nodes[idx];
                        simple_vertex.Latitude = local_position.Latitude;
                        simple_vertex.Longitude = local_position.Longitude;
                        simple_vertex.Neighbour1 = node_id;
                        simple_vertex.Neighbour2 = result_neighbour1.Id;

                        _resolved_simple_vertices[simple_vertex.Id] = simple_vertex;
                    }
                }
                total_distance = total_distance + previous_location.DistanceReal(from.Location).Value;
                result_neighbour1.Nodes = nodes.ToArray();

                // update the weight assuming it is lineair to the distance.
                ratio = 0;
                ratio_position = 0;
                if (total_distance != 0)
                {
                    ratio = cut_distance / total_distance;
                    ratio_position = weight_between / total_distance;
                }

                result_neighbour1.Weight = to_from_neighbour.Weight * ratio + (to_from_neighbour.Weight * ratio_position) * (1.0 - position);

                neighbours.Add(result_neighbour1);
            }

            // calculate the sparseneighbour from from to to.
            SparseVertexNeighbour result_neighbour2 = new SparseVertexNeighbour();
            result_neighbour2.Id = to.Id;
            result_neighbour2.Tags = from_to_neighbour.Tags; // copy the same tags.

            // initialize all variables.
            nodes = new List<long>();
            add = false;
            total_distance = 0;
            cut_distance = 0;
            previous_location = from.Location;

            // loop voer all nodes; calculate the nodes list and the distances.
            for (int idx = 0; idx < from_to_neighbour.Nodes.Length; idx++)
            {
                // get the simple vertex.
                SparseSimpleVertex local_vertex = this.GetResolvedSimpleVertex(from_to_neighbour.Nodes[idx]);
                GeoCoordinate local_position = new GeoCoordinate(local_vertex.Latitude, local_vertex.Longitude);

                // calculate the distance.
                double local_distance = previous_location.DistanceReal(local_position).Value;
                previous_location = local_position;
                total_distance = total_distance + local_distance;
                if (from_to_neighbour.Nodes[idx] == node_after_id)
                {
                    add = true;
                }
                if (add)
                {
                    cut_distance = local_distance + cut_distance;
                    nodes.Add(from_to_neighbour.Nodes[idx]);

                    SparseSimpleVertex simple_vertex = new SparseSimpleVertex();
                    simple_vertex.Id = from_to_neighbour.Nodes[idx];
                    simple_vertex.Latitude = local_position.Latitude;
                    simple_vertex.Longitude = local_position.Longitude;
                    simple_vertex.Neighbour1 = node_id;
                    simple_vertex.Neighbour2 = result_neighbour2.Id;

                    _resolved_simple_vertices[simple_vertex.Id] = simple_vertex;
                }
            }
            total_distance = total_distance + previous_location.DistanceReal(from.Location).Value;
            result_neighbour2.Nodes = nodes.ToArray();

            // update the weight assuming it is lineair to the distance.
            ratio = 0;
            ratio_position = 0;
            if (total_distance != 0)
            {
                ratio = cut_distance / total_distance;
                ratio_position = weight_between / total_distance;
            }
            result_neighbour2.Weight = from_to_neighbour.Weight * ratio + (from_to_neighbour.Weight * ratio_position) * (position);

            neighbours.Add(result_neighbour2);

            return neighbours;
        }

        #endregion

        #region IGraphDirected<ISparseVertex>

        /// <summary>
        /// Returns all neighbours reversed.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public Dictionary<long, float> GetNeighboursReversed(long vertex_id, HashSet<long> exceptions)
        {
            Dictionary<long, float> neighbours = new Dictionary<long, float>();

            SparseVertex vertex = null;
            if (!_resolved_vertices.TryGetValue(vertex_id, out vertex))
            {
                vertex = _data.GetSparseVertex(vertex_id);
            }

            // get the non-resolved neighbours.
            foreach (SparseVertexNeighbour neighbour in vertex.Neighbours)
            { // evaluate each neighbour seperatly.
                if (neighbour.Weight <= 0)
                {
                    if ((exceptions == null || !exceptions.Contains(neighbour.Id)))
                    { // the neighbour is in the correct direction.
                        float current;
                        if (neighbours.TryGetValue(neighbour.Id, out current))
                        {
                            if (current > neighbour.Weight)
                            {
                                neighbours[neighbour.Id] = (float)neighbour.Weight;
                            }
                        }
                        else
                        {
                            neighbours[neighbour.Id] = (float)neighbour.Weight;
                        }
                    }
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Returns all neighbours undirected.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public Dictionary<long, float> GetNeighboursUndirected(long vertex_id, HashSet<long> exceptions)
        {
            Dictionary<long, float> neighbours = new Dictionary<long,float>();

            SparseVertex vertex = null;
            if (!_resolved_vertices.TryGetValue(vertex_id, out vertex))
            {
                vertex = _data.GetSparseVertex(vertex_id);
            }

            // get the non-resolved neighbours.
            foreach (SparseVertexNeighbour neighbour in vertex.Neighbours)
            { // evaluate each neighbour seperatly.
                if ((exceptions == null || !exceptions.Contains(neighbour.Id)))
                { // the neighbour is in the correct direction.
                    float current;
                    if (neighbours.TryGetValue(neighbour.Id, out current))
                    {
                        if (current > neighbour.Weight)
                        {
                            neighbours[neighbour.Id] = (float)neighbour.Weight;
                        }
                    }
                    else
                    {
                        neighbours[neighbour.Id] = (float)neighbour.Weight;
                    }
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Returns all neighbours directed.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public Dictionary<long, float> GetNeighbours(long vertex_id, HashSet<long> exceptions)
        {
            Dictionary<long, float> neighbours = new Dictionary<long, float>();

            SparseVertex vertex = null;
            if (!_resolved_vertices.TryGetValue(vertex_id, out vertex))
            {
                vertex = _data.GetSparseVertex(vertex_id);
            }

            // get the non-resolved neighbours.
            if (vertex != null)
            {
                foreach (SparseVertexNeighbour neighbour in vertex.Neighbours)
                { // evaluate each neighbour seperatly.
                    if (neighbour.Weight >= 0)
                    {
                        if ((exceptions == null || !exceptions.Contains(neighbour.Id)))
                        { // the neighbour is in the correct direction.
                            float current;
                            if (neighbours.TryGetValue(neighbour.Id, out current))
                            {
                                if (current > neighbour.Weight)
                                {
                                    neighbours[neighbour.Id] = (float)neighbour.Weight;
                                }
                            }
                            else
                            {
                                neighbours[neighbour.Id] = (float)neighbour.Weight;
                            }
                        }
                    }
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Returns the vertex with the given id.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <returns></returns>
        public SparseVertex GetVertex(long vertex_id)
        {
            SparseVertex vertex = null;
            if (!_resolved_vertices.TryGetValue(vertex_id, out vertex))
            {
                vertex = _data.GetSparseVertex(vertex_id);
            }
            return vertex;
        }

        #endregion
    }
}
