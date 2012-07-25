//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Routing
//{
//    public static class Extensions<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        public static GraphRoute<EdgeType, VertexType>
//            CreateRoute(IPoint2PointRouter<EdgeType, VertexType> router, List<VertexType> in_order)
//        {
//            VertexType from = in_order[0];

//            List<GraphRouteEntry<EdgeType, VertexType>> entries = new List<GraphRouteEntry<EdgeType, VertexType>>();

//            float weight = 0;
//            for (int idx = 1; idx < in_order.Count; idx++)
//            {
//                // get start-end.
//                VertexType from_in_entry = in_order[idx - 1];
//                VertexType to_in_entry = in_order[idx];

//                // calculate/get route.
//                GraphRoute<EdgeType, VertexType> route = router.Calculate(from_in_entry, to_in_entry, -1);
//                weight = route.Weight + weight;
//                entries.AddRange(route.Entries);
//            }

//            return new GraphRoute<EdgeType, VertexType>(
//                new GraphRouteEntryPoint<VertexType>(from, true), entries, weight);
//        }
//    }
//}
