//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Routing.Dykstra
//{
//    public class ControllerWrapper<EdgeType, VertexType> : IPoint2PointRouterDykstraController<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        private IPoint2PointRouterController<EdgeType, VertexType> _controller;

//        public ControllerWrapper(IPoint2PointRouterController<EdgeType, VertexType> controller)
//        {
//            _controller = controller;
//        }

//        public bool NotifyFirstVertex(VertexReference<EdgeType, VertexType> first, DykstraVisitList<EdgeType, VertexType> visit_list)
//        {
//            return true;
//        }

//        //public void NotifyRoutingAlgorithm(RoutingAlgorithmsEnum algorithm)
//        //{
//        //    _controller.NotifyRoutingAlgorithm(algorithm);
//        //}

//        public void NotifyVerticesToRoute(List<VertexType> vertices)
//        {
//            _controller.NotifyVerticesToRoute(vertices);
//        }

//        public void NotifyVertexSelected(VertexReference<EdgeType, VertexType> vertex)
//        {
//            _controller.NotifyVertexSelected(vertex);
//        }

//        public bool QueryStopConditions()
//        {
//            return _controller.QueryStopConditions();
//        }
//        //public void NotifyRoutesNotFound(VertexType from, List<VertexType> to)
//        //{
//        //    _controller.NotifyRoutesNotFound(from, to);
//        //}

//        public void NotifyGraph(Graph<EdgeType, VertexType> graph)
//        {
//            _controller.NotifyGraph(graph);
//        }

//        public void NotifyFound(GraphRoute<EdgeType, VertexType> route)
//        {
//            _controller.NotifyFound(route);
//        }
//    }
//}
