using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpService.Core.Routing.Primitives
{
    /// <summary>
    /// A generic response for a resolve request.
    /// </summary>
    public class ResolveResponse
    {
        /// <summary>
        /// Holds the resolve status.
        /// </summary>
        public OsmSharpServiceResponseStatusEnum Status { get; set; }

        /// <summary>
        /// Holds a message with more details about the status.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// The resolved version of the hook(s).
        /// </summary>
        public RoutingHookResolved[] ResolvedHooks { get; set; }

        /// <summary>
        /// Returns a description for this response.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Response: {0}:{1}", this.Status.ToString(), this.StatusMessage);
        }
    }
}
