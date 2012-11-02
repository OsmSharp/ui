using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Raw;
using Osm.Routing.Core.Interpreter;
using System.IO;
using Osm.Routing.Core;
using Osm.Routing.Core.Constraints;
using Osm.Data.Raw.XML.OsmSource;

namespace Osm.Routing.Test.ManyToMany
{
    /// <summary>
    /// Executes many-to-many calculation performance tests using the raw data format.
    /// </summary>
    public class ManyToManyRawTests : ManyToManyTests<ResolvedPoint>
    {
        /// <summary>
        /// Creates a raw router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        protected override IRouter<ResolvedPoint> CreateRouter(
           Stream data, RoutingInterpreterBase interpreter, IRoutingConstraints constraints)
        {
            // build all the data source.
            OsmDataSource osm_data = new OsmDataSource(data);

            // build the router.
            return new Router(osm_data, interpreter, constraints);
        }
    }
}
