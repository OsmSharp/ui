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
using OsmSharp.Routing.Core.Graph.Path;

namespace OsmSharp.Routing.CH.Routing
{
    public class CHQueue
    {
        private Dictionary<long, PathSegment<long>> _forward;

        private Dictionary<long, PathSegment<long>> _backward;

        private Dictionary<long, double> _intersection;

        public CHQueue()
        {
            _intersection = new Dictionary<long, double>();

            _forward = new Dictionary<long, PathSegment<long>>();
            _backward = new Dictionary<long, PathSegment<long>>();
        }

        public Dictionary<long, double> Intersection
        {
            get
            {
                return _intersection;
            }
        }

        public Dictionary<long, PathSegment<long>> Forward
        {
            get
            {
                return _forward;
            }
        }

        public Dictionary<long, PathSegment<long>> Backward
        {
            get
            {
                return _backward;
            }
        }

        public void AddForward(PathSegment<long> segment)
        {
            _forward[segment.VertexId] = segment;

            PathSegment<long> backward;
            if (_backward.TryGetValue(segment.VertexId, out backward))
            {
                _intersection.Add(segment.VertexId, backward.Weight + segment.Weight);
            }
        }

        public void AddBackward(PathSegment<long> segment)
        {
            _backward[segment.VertexId] = segment;

            PathSegment<long> forward;
            if (_forward.TryGetValue(segment.VertexId, out forward))
            {
                _intersection.Add(segment.VertexId, forward.Weight + segment.Weight);
            }
        }
    }
}
