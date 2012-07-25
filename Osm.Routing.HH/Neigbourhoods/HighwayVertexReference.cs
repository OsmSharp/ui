using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.HH.Neigbourhoods
{
    /// <summary>
    /// A vertex with a reference to another vertex.
    /// </summary>
    internal class HighwayVertexReference
    {
        /// <summary>
        /// Creates a new vertex reference.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="from"></param>
        public HighwayVertexReference(long vertex, HighwayVertexReference from, float weight)
        {
            this.VertexId = vertex;
            this.From = from;
            this.TotalWeight = this.From.TotalWeight + weight;
        }

        /// <summary>
        /// The reference this vertex is for.
        /// </summary>
        public long VertexId { get; private set; }

        /// <summary>
        /// The vertex this reference is to.
        /// </summary>
        public HighwayVertexReference From { get; private set; }

        /// <summary>
        /// The total weight of this reference.
        /// </summary>
        public float TotalWeight { get; set; }

        #region Comparer

        /// <summary>
        /// Holds the comparer.
        /// </summary>
        private static IComparer<HighwayVertexReference> _comparer
            = new HighwayVertexReferenceComparer();

        /// <summary>
        /// Returns the comparer.
        /// </summary>
        public static IComparer<HighwayVertexReference> Comparer
        {
            get
            {
                return _comparer;
            }
        }

        /// <summary>
        /// A private comparer.
        /// </summary>
        private class HighwayVertexReferenceComparer : IComparer<HighwayVertexReference>
        {
            /// <summary>
            /// Compares x and y.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(HighwayVertexReference x, HighwayVertexReference y)
            {
                return x.TotalWeight.CompareTo(y.TotalWeight);
            }
        }

        #endregion
    }
}
