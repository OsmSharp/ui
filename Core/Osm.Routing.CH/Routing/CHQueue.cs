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
