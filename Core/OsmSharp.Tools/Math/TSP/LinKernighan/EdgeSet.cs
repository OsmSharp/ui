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
    internal class EdgeSet
    {
        Dictionary<int, Dictionary<int, Edge>> _edges;

        public EdgeSet()
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
    }
}
