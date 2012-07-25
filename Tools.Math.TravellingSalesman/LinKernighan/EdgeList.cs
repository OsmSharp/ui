using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.TSP.LK
{
    class EdgeList
    {
        private float _weight;

        private List<Edge> _edges;

        public EdgeList()
        {
            _edges = new List<Edge>();

            _weight = 0;
        }

        public EdgeList(Edge edge)
        {
            _edges = new List<Edge>();
            _edges.Add(edge);

            _weight = edge.Weight;
        }

        public void Add(Edge edge)
        {
            _edges.Add(edge);
            _weight = _weight + edge.Weight;
        }

        public Edge this[int idx]
        {
            get
            {
                return _edges[idx];
            }
        }

        public int Count
        {
            get
            {
                return _edges.Count;
            }
        }

        public bool Contains(int from, int to)
        {
            foreach (Edge e in _edges)
            {
                if ((e.From == from && e.To == to)
                    || (e.To == from && e.From == to))
                {
                    return true;
                }
            }
            return false;
        }

        public Edge Last
        {
            get
            {
                return _edges[_edges.Count - 1];
            }
        }

        public bool Contains(Edge e)
        {
            return this.Contains(e.From, e.To);
        }

        public float Weight
        {
            get
            {
                return _weight;
            }
        }

        public void RemoveLast()
        {
            _weight = _weight - this._edges[this._edges.Count - 1].Weight;
            this._edges.RemoveAt(
                this._edges.Count - 1);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}",
                _edges.Count,
                this.Weight);
        }
    }
}
