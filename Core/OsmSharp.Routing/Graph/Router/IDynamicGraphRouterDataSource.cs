using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph.DynamicGraph;

namespace OsmSharp.Routing.Graph.Router
{
    /// <summary>
    /// Interface representing objects that are both and IBasicRouterDataSource and a IDynamicGraph.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public interface IDynamicGraphRouterDataSource<TEdgeData> : IDynamicGraph<TEdgeData>, IBasicRouterDataSource<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
    }
}
