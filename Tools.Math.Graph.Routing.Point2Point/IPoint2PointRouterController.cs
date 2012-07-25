//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Routing.Dykstra;

//namespace Tools.Math.Graph.Routing
//{
//    /// <summary>
//    /// Interface allowing some external controle over the routing process.
//    /// </summary>
//    /// <typeparam name="EdgeType"></typeparam>
//    /// <typeparam name="VertexType"></typeparam>
//    /// <typeparam name="float"></typeparam>
//    public interface IPoint2PointRouterController<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        /// <summary>
//        /// Let the listener know about the graph that is being routed on.
//        /// </summary>
//        /// <param name="graph"></param>
//        void NotifyGraph(Graph<EdgeType, VertexType> graph);

//        /// <summary>
//        /// Let the listener know what vertices are being routed.
//        /// </summary>
//        /// <param name="vertices"></param>
//        void NotifyVerticesToRoute(List<VertexType> vertices);

//        /// <summary>
//        /// Let the listener know the routing process moved to a new vertex.
//        /// </summary>
//        /// <param name="vertex"></param>
//        void NotifyVertexSelected(VertexReference<EdgeType, VertexType> vertex);

//        /// <summary>
//        /// Let the listener know the routing process found a target.
//        /// </summary>
//        /// <param name="vertex"></param>
//        void NotifyFound(GraphRoute<EdgeType, VertexType> route);

//        /// <summary>
//        /// Ask the listener if routing should continue.
//        /// </summary>
//        /// <returns></returns>
//        bool QueryStopConditions();
//    }
//}
