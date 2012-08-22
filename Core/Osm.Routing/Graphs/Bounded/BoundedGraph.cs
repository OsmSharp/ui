using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Data;
using Osm.Routing.Raw.Graphs.Interpreter;


// TODO: create a IPointF2D-like graph interface hierarchy to allow other-than-osm datasources.

namespace Osm.Routing.Raw.Graphs.Bounded
{
    public class BoundedGraph : Graph
    {
        public BoundedGraph(GraphInterpreterBase interpreter,
            IDataSourceReadOnly source)
            :base(interpreter, source)
        {
            if (!source.HasBoundinBox)
            {
                throw new ArgumentException("Only bounded source can be used for a bounded graph!");
            }
        }

        public IList<GraphVertex> GetVertices()
        {
            throw new NotImplementedException();
        }

        public IList<Way> GetEdges()
        {
            throw new NotImplementedException();
        }
    }
}
