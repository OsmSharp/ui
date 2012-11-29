using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed
{
    public class SimpleWeighedDynamicGraph : IDynamicGraph<SimpleWeighedEdge>
    {

        public uint AddVertex(float latitude, float longitude)
        {
            throw new NotImplementedException();
        }

        public void AddArc(uint from, uint to, SimpleWeighedEdge data)
        {
            throw new NotImplementedException();
        }

        public void DeleteArc(uint from, uint to)
        {
            throw new NotImplementedException();
        }

        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<uint> GetVertices()
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<uint, SimpleWeighedEdge>[] GetArcs(uint vertex)
        {
            throw new NotImplementedException();
        }

        public bool HasNeighbour(uint vertex, uint neighbour)
        {
            throw new NotImplementedException();
        }

        public uint VertexCount
        {
            get { throw new NotImplementedException(); }
        }

        private long[] _coordinates;

        private Vertex[] _vertices;

        private struct Vertex
        {
            public int EdgeIdx { get; set; }
        }
    }
}
