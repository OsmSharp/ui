using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;

namespace OsmSharp.Routing.Routers
{
    /// <summary>
    /// A version of the typedrouter using edges of type LiveEdge.
    /// </summary>
    internal class TypedRouterLiveEdge : TypedRouter<LiveEdge>
    {
        /// <summary>
        /// Creates a new type router using edges of type LiveEdge.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="router"></param>
        public TypedRouterLiveEdge(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
                           IBasicRouter<LiveEdge> router)
            :base(graph, interpreter, router)
        {
            
        }
    }
}