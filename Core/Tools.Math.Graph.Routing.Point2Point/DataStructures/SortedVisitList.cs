using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph.Routing.DataStructures
{
    /// <summary>
    /// Internal data structure reprenting a visit list,
    /// </summary>
    public class SortedVisitList
    {
        /// <summary>
        /// Holds all visited nodes sorted by weight.
        /// </summary>
        private SortedList<float, Dictionary<long, RouteLinked>> _visit_list;

        /// <summary>
        /// Holds all visited vertices.
        /// </summary>
        private Dictionary<long, float> _visited;

        /// <summary>
        /// Creates a new visit list.
        /// </summary>
        public SortedVisitList()
        {
            _visit_list = new SortedList<float, Dictionary<long, RouteLinked>>();
            _visited = new Dictionary<long, float>();
        }

        /// <summary>
        /// Updates a vertex in this visit list.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="weight"></param>
        public void UpdateVertex(RouteLinked route)
        {
            float current_weight;
            if (_visited.TryGetValue(route.VertexId, out current_weight))
            { // the vertex was already in this list.
                Dictionary<long, RouteLinked> current_weight_vertices = _visit_list[current_weight];
                current_weight_vertices.Remove(route.VertexId);
                if (current_weight_vertices.Count == 0)
                {
                    _visit_list.Remove(current_weight);
                }                
            }

            // add/update everthing.
            Dictionary<long, RouteLinked> vertices_at_weight;
            if (!_visit_list.TryGetValue(route.Weight, out vertices_at_weight))
            {
                vertices_at_weight = new Dictionary<long, RouteLinked>();
                _visit_list.Add(route.Weight, vertices_at_weight);
            }
            vertices_at_weight.Add(route.VertexId, route);
            _visited[route.VertexId] = route.Weight;
        }
        
        /// <summary>
        /// Returns the vertex with the lowest weight and removes it.
        /// </summary>
        /// <returns></returns>
        public RouteLinked GetFirst()
        {
            if (_visit_list.Count > 0)
            {
                float weight = _visit_list.Keys[0];
                Dictionary<long, RouteLinked> first_set = _visit_list[weight];
                KeyValuePair<long, RouteLinked> first_pair =
                    first_set.First<KeyValuePair<long, RouteLinked>>();
                long vertex_id = first_pair.Key;

                // remove the vertex.
                first_set.Remove(vertex_id);
                if (first_set.Count == 0)
                {
                    _visit_list.Remove(weight);
                }
                _visited.Remove(vertex_id);

                return first_pair.Value;
            }
            return null;
        }
        
        /// <summary>
        /// Returns the element count in this list.
        /// </summary>
        public int Count
        {
            get
            {
                return _visited.Count;
            }
        }
    }
}
