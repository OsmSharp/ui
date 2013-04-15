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

namespace OsmSharp.Tools.Math.TSP.LK
{
    class EdgeList
    {
        private double _weight;

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

        public double Weight
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
