using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Sources
{
    /// <summary>
    /// Represents any source of nodes.
    /// </summary>
    public interface INodeSource
    {
        /// <summary>
        /// Returns a node with the given id from this source.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Node GetNode(long id);
    }
}
