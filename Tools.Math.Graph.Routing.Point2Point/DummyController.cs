//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Routing
//{
//    public class DummyController<EdgeType, VertexType> : IPoint2PointRouterController<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        public void NotifyRoutingAlgorithm(RoutingAlgorithmsEnum algorithm)
//        {

//        }

//        public void NotifyVerticesToRoute(List<VertexType> vertices)
//        {

//        }

//        public void NotifyVertexSelected(Dykstra.VertexReference<EdgeType, VertexType> vertex)
//        {

//        }

//        public void NotifyRoutesNotFound(VertexType from, List<VertexType> to)
//        {

//        }

//        public bool QueryStopConditions()
//        {
//            return false;
//        }
//        public void NotifyGraph(Graph<EdgeType, VertexType> graph)
//        {

//        }

//        public void NotifyFound(GraphRoute<EdgeType, VertexType> route)
//        {

//        }
//    }
//}
