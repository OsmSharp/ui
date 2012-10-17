using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.Interpreter
{
    /// <summary>
    /// Interprets specifics about way tags.
    /// </summary>
    public abstract class RoutingWayInterperterBase
    {
        /// <summary>
        /// Holds a dictionary of tags.
        /// </summary>
        protected IDictionary<string, string> _way_tags;

        /// <summary>
        /// Creates a new tag interpreter.
        /// </summary>
        /// <param name="way_tags"></param>
        public RoutingWayInterperterBase(IDictionary<string, string> way_tags)
        {
            _way_tags = way_tags;
        }

        /// <summary>
        /// Returns all the way tags.
        /// </summary>
        public IDictionary<string, string> Tags
        {
            get
            {
                return _way_tags;
            }
        }
    }
}
