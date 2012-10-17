using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.Interpreter.Default
{
    /// <summary>
    /// Interpretes specifics about ways tags into roads.
    /// </summary>
    public class DefaultWayInterpreter : RoutingWayInterperterBase
    {
        /// <summary>
        /// Creates a new tag interpreter.
        /// </summary>
        /// <param name="way_tags"></param>
        public DefaultWayInterpreter(IDictionary<string, string> way_tags)
            :base(way_tags)
        {

        }
    }
}
