//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Routing.Dykstra
//{
//    /// <summary>
//    /// Interface specifying specific dykstra controller functions.
//    /// </summary>
//    /// <typeparam name="EdgeType"></typeparam>
//    /// <typeparam name="VertexType"></typeparam>
//    /// <typeparam name="float"></typeparam>
//    public interface IPoint2PointRouterDykstraController<EdgeType, VertexType> : IPoint2PointRouterController<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        /// <summary>
//        /// Notifies the controller of a first vertex being selected.
//        /// </summary>
//        /// <param name="first"></param>
//        /// <returns></returns>
//        bool NotifyFirstVertex(VertexReference<EdgeType, VertexType> first, DykstraVisitList<EdgeType, VertexType> visit_list);
//    }
//}
