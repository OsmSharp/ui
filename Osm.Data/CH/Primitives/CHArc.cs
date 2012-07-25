//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Osm.Data.Core.CH.Primitives
//{
//    /// <summary>
//    /// Represents an arc.
//    /// </summary>
//    public class CHArc
//    {
//        /// <summary>
//        /// The vertex this arc starts at.
//        /// </summary>
//        public long VertexFromId { get; set; }

//        /// <summary>
//        /// The vertex this arc ends at.
//        /// </summary>
//        public long VertexToId { get; set; }

//        /// <summary>
//        /// The vertex that was contracted into this arc.
//        /// </summary>
//        public long ContractedVertexId { get; set; }

//        /// <summary>
//        /// The weight of this arc.
//        /// </summary>
//        public double Weight { get; set; }

//        /// <summary>
//        /// Hash of this arc.
//        /// </summary>
//        /// <returns></returns>
//        public override int GetHashCode()
//        {
//            return this.VertexFromId.GetHashCode() ^
//                this.VertexToId.GetHashCode() ^
//                this.Weight.GetHashCode();
//        }

//        /// <summary>
//        /// The actual tags.
//        /// </summary>
//        public IDictionary<string, string> Tags { get; set; }

//        /// <summary>
//        /// Returns true if the given object equals this object.
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        public override bool Equals(object obj)
//        {
//            if (obj is CHArc)
//            {
//                return this.VertexFromId == (obj as CHArc).VertexFromId &&
//                    this.VertexToId == (obj as CHArc).VertexToId &&
//                    this.Weight == (obj as CHArc).Weight;
//            }
//            return false;
//        }
//    }
//}
