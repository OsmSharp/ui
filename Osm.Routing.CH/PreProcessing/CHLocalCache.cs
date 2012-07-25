using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.CH.Primitives;
using Osm.Data.Core.CH;

namespace Osm.Routing.CH.PreProcessing
{
    internal class CHLocalCache
    {
        private Dictionary<long, CHVertex> _cache;

        public CHLocalCache()
        {
            _cache = new Dictionary<long, CHVertex>();
        }

        public CHVertex Get(ICHData data, long vertex_id)
        {
            CHVertex vertex;
            if (!_cache.TryGetValue(vertex_id, out vertex))
            {
                vertex = data.GetCHVertex(vertex_id);
                _cache.Add(vertex_id, vertex);
            }
            return vertex;
        }

        public IEnumerable<CHVertex> Vertices
        {
            get
            {
                return _cache.Values;
            }
        }

        public IEnumerable<long> Keys
        {
            get
            {
                return _cache.Keys;
            }
        }
    }
}
