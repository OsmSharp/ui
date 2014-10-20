using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing.Graph.Router.Dykstra
{
    /// <summary>
    /// Represents a visit list keeping track of vertices that have been visited.
    /// </summary>
    public class DykstraVisitList
    {
        /// <summary>
        /// Holds the set of visited vertices.
        /// </summary>
        private HashSet<long> _visited;

        /// <summary>
        /// Holds the set of restricted vertices.
        /// </summary>
        private HashSet<long> _restricted;

        /// <summary>
        /// Holds the restricted visits.
        /// </summary>
        private Dictionary<long, HashSet<long>> _restrictedVisits;

        /// <summary>
        /// Creates a new visit list.
        /// </summary>
        public DykstraVisitList()
        {
            _visited = new HashSet<long>();
            _restricted = new HashSet<long>();
            _restrictedVisits = new Dictionary<long, HashSet<long>>();
        }

        /// <summary>
        /// Returns true if the given vertex has been visited already.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool HasBeenVisited(PathSegment<long> vertex)
        {
            if(vertex.From != null)
            { // there is a previous vertex, check it.
                return this.HasBeenVisited(vertex.VertexId, vertex.From.VertexId);
            }
            return _visited.Contains(vertex.VertexId);
        }

        /// <summary>
        /// Returns true if the given vertex has been visited already.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="fromVertex"></param>
        /// <returns></returns>
        public bool HasBeenVisited(long vertex, long fromVertex)
        {
            if (!_restricted.Contains(vertex))
            { // not restricted.
                return _visited.Contains(vertex);
            }
            else
            { // check restricted.
                HashSet<long> froms;
                if (_restrictedVisits.TryGetValue(vertex, out froms))
                {
                    return froms.Contains(fromVertex);
                }
                return false;
            }
        }


        /// <summary>
        /// Sets the vertex as visited coming from the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public void SetVisited(PathSegment<long> vertex)
        {
            if (vertex.From != null)
            { // there is a previous vertex, check it.
                this.SetVisited(vertex.VertexId, vertex.From.VertexId);
            }
            else
            {
                _visited.Add(vertex.VertexId);
            }
        }

        /// <summary>
        /// Sets the vertex as visited coming from the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="fromVertex"></param>
        public void SetVisited(long vertex, long fromVertex)
        {
            if (!_restricted.Contains(vertex))
            { // not restricted.
                _visited.Add(vertex);
            }
            else
            { // check restricted.
                HashSet<long> froms;
                if (!_restrictedVisits.TryGetValue(vertex, out froms))
                {
                    froms = new HashSet<long>();
                    _restrictedVisits.Add(vertex, froms);
                }
                froms.Add(fromVertex);
            }
        }

        /// <summary>
        /// Sets the given vertex as restricted.
        /// </summary>
        /// <param name="vertex"></param>
        public void SetRestricted(long vertex)
        {
            _restricted.Add(vertex);
            _visited.Remove(vertex);
        }
    }
}