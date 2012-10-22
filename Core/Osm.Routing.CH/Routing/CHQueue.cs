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

namespace Osm.Routing.CH.Routing
{
    public class CHQueue
    {
        private Dictionary<uint, CHPathSegment> _forward;

        private Dictionary<uint, CHPathSegment> _backward;

        private Dictionary<uint, float> _intersection;

        public CHQueue()
        {
            _intersection = new Dictionary<uint, float>();

            _forward = new Dictionary<uint, CHPathSegment>();
            _backward = new Dictionary<uint, CHPathSegment>();
        }

        public Dictionary<uint, float> Intersection
        {
            get
            {
                return _intersection;
            }
        }

        public Dictionary<uint, CHPathSegment> Forward
        {
            get
            {
                return _forward;
            }
        }

        public Dictionary<uint, CHPathSegment> Backward
        {
            get
            {
                return _backward;
            }
        }

        public void AddForward(CHPathSegment segment)
        {
            _forward[segment.VertexId] = segment;

            CHPathSegment backward;
            if (_backward.TryGetValue(segment.VertexId, out backward))
            {
                _intersection.Add(segment.VertexId, backward.Weight + segment.Weight);
            }
        }

        public void AddBackward(CHPathSegment segment)
        {
            _backward[segment.VertexId] = segment;

            CHPathSegment forward;
            if (_forward.TryGetValue(segment.VertexId, out forward))
            {
                _intersection.Add(segment.VertexId, forward.Weight + segment.Weight);
            }
        }
    }
}
