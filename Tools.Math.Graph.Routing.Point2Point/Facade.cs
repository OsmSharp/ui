//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Routing
//{
//    public class Facade
//    {
//        /// <summary>
//        /// Creates a new point to point router.
//        /// </summary>
//        /// <param name="graph"></param>
//        /// <returns></returns>
//        public static IPoint2PointRouter<EdgeType, VertexType> 
//            Create<EdgeType, VertexType>(
//            RoutingAlgorithmsEnum algorithm,
//            Graph<EdgeType, VertexType> graph,
//            int max)
//            where EdgeType : class
//            where VertexType : class, IEquatable<VertexType>
//        {
//            switch (algorithm)
//            {
//                case RoutingAlgorithmsEnum.Dykstra:
//                    return new Dykstra.DykstraRouting<EdgeType, VertexType>(graph, max);
//                case RoutingAlgorithmsEnum.AStar:
//                    return new AStar.AStarRouting<EdgeType, VertexType>(graph, max);
//                case RoutingAlgorithmsEnum.BidirectionalAStar:
//                    return new Bidirectional.BidirectionalAStarRouting<EdgeType, VertexType>(graph, max);
//            }

//            throw new ArgumentOutOfRangeException(string.Format("{0} not implemented!", algorithm.ToString()));
//        }

//        ///// <summary>
//        ///// Creates a new point to point router that calculates and keep all routes between all vertices given.
//        ///// </summary>
//        ///// <param name="algorithm"></param>
//        ///// <param name="graph"></param>
//        ///// <param name="nodes"></param>
//        ///// <returns></returns>
//        //public static IPoint2PointRouter<EdgeType, VertexType> CreateCachedRouter<EdgeType, VertexType>(
//        //    RoutingAlgorithmsEnum algorithm,
//        //    Graph<EdgeType, VertexType> graph,
//        //    List<VertexType> vertices,
//        //    int max)
//        //    where EdgeType : class
//        //    where VertexType : class, IEquatable<VertexType>
//        //{
//        //    return new Point2PointRouteCache<EdgeType, VertexType>(
//        //        Facade.Create(algorithm, graph, max),
//        //        vertices);
//        //}
//    }

//    public enum RoutingAlgorithmsEnum
//    {
//        Dykstra,
//        AStar,
//        BidirectionalAStar
//    }
//}
