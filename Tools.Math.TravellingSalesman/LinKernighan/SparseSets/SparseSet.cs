using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.TSP.LK.SparseSets
{
    class SparseSet
    {
        Dictionary<int, Dictionary<int, Edge>> _edges;

        public SparseSet()
        {
            _edges = new Dictionary<int, Dictionary<int, Edge>>();
        }

        public void Add(Edge edge)
        {
            Dictionary<int, Edge> edge_dic = null;
            if (!_edges.TryGetValue(edge.From, out edge_dic))
            {
                edge_dic = new Dictionary<int, Edge>();
                _edges.Add(edge.From, edge_dic);
            }
            edge_dic[edge.To] = edge;
            if (!_edges.TryGetValue(edge.To, out edge_dic))
            {
                edge_dic = new Dictionary<int, Edge>();
                _edges.Add(edge.To, edge_dic);
            }
            edge_dic[edge.From] = edge;
        }

        public bool Contains(Edge edge)
        {
            return this.Contains(edge.From, edge.To);
        }

        public bool Contains(int from, int to)
        {
            return (_edges.ContainsKey(from) &&
                _edges[from].ContainsKey(to)) ||
                (_edges.ContainsKey(to) &&
                _edges[to].ContainsKey(from));
        }

        public IEnumerable<Edge> GetFor(int customer)
        {
            Dictionary<int, Edge> neigbours = null;
            _edges.TryGetValue(customer, out neigbours);
            if (neigbours == null)
            {
                return new List<Edge>();
            }
            return neigbours.Values;
        }
    }
}
