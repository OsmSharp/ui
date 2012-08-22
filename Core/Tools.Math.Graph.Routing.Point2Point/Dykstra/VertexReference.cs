//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Routing.Dykstra
//{
//    public class VertexReference<EdgeType, VertexType> : IEquatable<VertexReference<EdgeType, VertexType>>
//    {
//        /// <summary>
//        /// Creates a new node reference object.
//        /// </summary>
//        /// <param name="distance"></param>
//        /// <param name="from"></param>
//        /// <param name="node"></param>
//        public VertexReference(
//            float weight,
//            VertexReference<EdgeType, VertexType> from,
//            VertexType vertex,
//            EdgeType edge)
//        {
//            this.Weight = weight;
//            this.From = from;
//            this.Vertex = vertex;
//            this.Edge = edge;
//        }

//        /// <summary>
//        /// The weight to the node this reference is to.
//        /// </summary>
//        public float Weight { get; private set; }

//        private float? _total_weight;

//        /// <summary>
//        /// The total weight.
//        /// </summary>
//        public float TotalWeight
//        {
//            get
//            {
//                if (_total_weight == null 
//                    || !_total_weight.HasValue)
//                {
//                    if (this.From != null)
//                    {
//                        _total_weight = this.Weight + this.From.TotalWeight;
//                    }
//                    else
//                    {
//                        _total_weight = this.Weight;
//                    }
//                }
//                return _total_weight.Value;
//            }
//        }
        
//        /// <summary>
//        /// The node in the route before this node.
//        /// </summary>
//        public VertexReference<EdgeType, VertexType> From { get; private set; }

//        /// <summary>
//        /// The node this reference is for.
//        /// </summary>
//        public VertexType Vertex { get; private set; }

//        /// <summary>
//        /// The way that is followed by this reference.
//        /// </summary>
//        public EdgeType Edge { get; private set; }

//        #region IEquatable<VertexReference<EdgeType, VertexType>> Members

//        public bool Equals(VertexReference<EdgeType, VertexType> other)
//        {
//            if (other == null)
//            {
//                return false;
//            }
//            if (other.Vertex == null
//                && this.Vertex != null)
//            {
//                return false;
//            }
//            if (this.Vertex == null &&
//                other.Vertex != null)
//            {
//                return false;
//            }
//            if (other.From == null
//                && this.From != null)
//            {
//                return false;
//            }
//            if (this.From == null &&
//                other.From != null)
//            {
//                return false;
//            }
//            if (this.From == null)
//            {
//                if (this.Vertex.Equals(other.Vertex)
//                    && other.Weight == this.Weight
//                    && other.From == null)
//                {
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            if ((other.Vertex as object).Equals(this.Vertex)
//                && other.Weight == this.Weight
//                && (other.From as object).Equals(this.From))
//            {
//                return true;
//            }
//            if (other.Vertex.Equals(this.Vertex)
//                && other.Weight == this.Weight
//                && other.From.Equals(this.From))
//            {
//                return true;
//            }
//            return false;
//        }

//        #endregion

//        /// <summary>
//        /// Creates a route to this node from the source node.
//        /// </summary>
//        /// <returns></returns>
//        public GraphRoute<EdgeType, VertexType> CreateRouteTo()
//        {
//            List<GraphRouteEntry<EdgeType, VertexType>> entries = new List<GraphRouteEntry<EdgeType, VertexType>>();

//            // build the route.
//            VertexReference<EdgeType, VertexType> next_ref = this;
//            float total_weight = 0;

//            // calculate total weight.
//            float weight = this.Weight;
//            total_weight = weight + total_weight;

//            // create a route entry.
//            GraphRouteEntry<EdgeType, VertexType> entry = new GraphRouteEntry<EdgeType, VertexType>(
//                new GraphRouteEntryPoint<VertexType>(next_ref.Vertex, true),
//                next_ref.Edge,
//                weight);
//            entries.Insert(0, entry);
//            next_ref = next_ref.From;

//            // calculate the rest.
//            while (next_ref.From != null)
//            {
//                // calculate total weight.
//                weight = this.Weight;
//                total_weight = weight + total_weight;

//                // create a route entry.
//                entry = new GraphRouteEntry<EdgeType, VertexType>(
//                    new GraphRouteEntryPoint<VertexType>(next_ref.Vertex, false),
//                    next_ref.Edge,
//                    weight);
//                entries.Insert(0, entry);
//                next_ref = next_ref.From;
//            }

//            return new GraphRoute<EdgeType, VertexType>(
//                new GraphRouteEntryPoint<VertexType>(next_ref.Vertex, true), entries, total_weight);
//        }


//        /// <summary>
//        /// Creates a route to this node from the source node.
//        /// </summary>
//        /// <returns></returns>
//        public GraphRoute<EdgeType, VertexType> CreateRouteToBackward(VertexReference<EdgeType, VertexType> backward)
//        {
//            List<GraphRouteEntry<EdgeType, VertexType>> entries = new List<GraphRouteEntry<EdgeType, VertexType>>();

//            // build the route.
//            VertexReference<EdgeType, VertexType> next_ref = this;
//            float total_weight = 0;

//            // calculate total weight.
//            float weight = this.Weight;
//            total_weight = weight + total_weight;

//            // create a route entry.
//            GraphRouteEntry<EdgeType, VertexType> entry = new GraphRouteEntry<EdgeType, VertexType>(
//                new GraphRouteEntryPoint<VertexType>(next_ref.Vertex, true),
//                next_ref.Edge,
//                weight);
//            entries.Insert(0, entry);
//            next_ref = next_ref.From;

//            // calculate the rest.
//            while (next_ref.From != null)
//            {
//                // calculate total weight.
//                weight = this.Weight;
//                total_weight = weight + total_weight;

//                // create a route entry.
//                entry = new GraphRouteEntry<EdgeType, VertexType>(
//                    new GraphRouteEntryPoint<VertexType>(next_ref.Vertex, false),
//                    next_ref.Edge,
//                    weight);
//                entries.Insert(0, entry);
//                next_ref = next_ref.From;
//            }

//            // add the backwards part.
//            next_ref = backward;
//            while (next_ref != null)
//            {
//                // calculate total weight.
//                weight = this.Weight;
//                total_weight = weight + total_weight;

//                // create a route entry.
//                entry = new GraphRouteEntry<EdgeType, VertexType>(
//                    new GraphRouteEntryPoint<VertexType>(next_ref.Vertex, false),
//                    next_ref.Edge,
//                    weight);
//                entries.Add(entry);
//                next_ref = next_ref.From;
//            }

//            return new GraphRoute<EdgeType, VertexType>(
//                new GraphRouteEntryPoint<VertexType>(next_ref.Vertex, true), entries, total_weight);
//        }
//    }
//}
