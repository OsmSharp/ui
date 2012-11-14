using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Routing service implementation.
    /// </summary>
    public class RoutingRestService : IService<RoutingOperation>
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Execute(RoutingOperation request)
        {
            return RoutingOperationProcessor.GetInstance().ProcessRoutingResource(request);
        }
    }
}
