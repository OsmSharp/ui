//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Finite
//{
//    public abstract class FiniteGraphInterpreter<EdgeType, VertexType> : IGraphInterpreter<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType> 
//    {
//        public void SetGraph(Graph<EdgeType, VertexType> graph)
//        {

//        }

//        public bool CanBeStoppedOn(EdgeType edge)
//        {
//            return true;
//        }

//        public bool CanBeTraversed(EdgeType edge)
//        {
//            return true;
//        }

//        public virtual bool CanBeTraversed(EdgeType edge, VertexType from, VertexType to)
//        {
//            return true;
//        }

//        public bool CanBeTraversed(VertexType from, EdgeType edge_from, VertexType via, EdgeType edge_to, VertexType to)
//        {
//            return true;
//        }

//        public abstract float Weight(EdgeType edge, VertexType from_vertex, VertexType to_vertex);

//        public float UnderestimateWeight(VertexType from, VertexType to)
//        {
//            return 0;
//        }
//    }
//}
