//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph;

//namespace Osm.Routing.Raw.Graphs.Sparse
//{
//    class SparseGraphInterpreter : IGraphInterpreter<SparseGraphEdge, GraphVertex>
//    {
//        #region IGraphInterpreter<SparseGraphEdge,GraphVertex> Members

//        public bool CanBeStoppedOn(SparseGraphEdge edge)
//        {
//            return true;
//        }

//        public bool CanBeTraversed(GraphVertex from, SparseGraphEdge edge_from, GraphVertex via, SparseGraphEdge edge_to, GraphVertex to)
//        {
//            return true;
//        }

//        public bool CanBeTraversed(SparseGraphEdge edge, GraphVertex from, GraphVertex to)
//        {
//            return true;
//        }

//        public bool CanBeTraversed(SparseGraphEdge edge)
//        {
//            return true;
//        }

//        public void SetGraph(Graph<SparseGraphEdge, GraphVertex> graph)
//        {

//        }

//        public float UnderestimateWeight(GraphVertex from, GraphVertex to)
//        {
//            return 0;
//        }

//        public float Weight(SparseGraphEdge edge, GraphVertex from_vertex, GraphVertex to_vertex)
//        {
//            return edge.Weight;
//        }

//        #endregion
//    }
//}
