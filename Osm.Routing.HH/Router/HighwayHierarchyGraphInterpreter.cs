//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.Graphs.Interpreter;
//using Osm.Routing.HH.Highways;
//using Osm.Routing.Graphs;
//using Tools.Math.Graph;

//namespace Osm.Routing.HH.Router
//{
//    public class HighwayHierarchyGraphInterpreter : IGraphInterpreter<HighwayEdge, GraphVertex>
//    {

//        public void SetGraph(Graph<HighwayEdge, GraphVertex> graph)
//        {

//        }

//        public bool CanBeStoppedOn(HighwayEdge edge)
//        {
//            return true;
//        }

//        public bool CanBeTraversed(HighwayEdge edge)
//        {
//            return true;
//        }

//        public bool CanBeTraversed(HighwayEdge edge, GraphVertex from, GraphVertex to)
//        {
//            return true;
//        }

//        public bool CanBeTraversed(GraphVertex from, HighwayEdge edge_from, GraphVertex via, HighwayEdge edge_to, GraphVertex to)
//        {
//            return true;
//        }

//        public float Weight(HighwayEdge edge, GraphVertex from_vertex, GraphVertex to_vertex)
//        {
//            return edge.Weight;
//        }

//        public float UnderestimateWeight(GraphVertex from, GraphVertex to)
//        {
//            return 0;
//        }
//    }
//}
