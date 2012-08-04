using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.CH.Routing
{
    public class CHQueue
    {
        private Dictionary<long, CHPathSegment> _forward;

        private Dictionary<long, CHPathSegment> _backward;

        private Dictionary<long, float> _intersection;

        public CHQueue()
        {
            _intersection = new Dictionary<long, float>();

            _forward = new Dictionary<long, CHPathSegment>();
            _backward = new Dictionary<long, CHPathSegment>();
        }

        public Dictionary<long, float> Intersection
        {
            get
            {
                return _intersection;
            }
        }

        public Dictionary<long, CHPathSegment> Forward
        {
            get
            {
                return _forward;
            }
        }

        public Dictionary<long, CHPathSegment> Backward
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
