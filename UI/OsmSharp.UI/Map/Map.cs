using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.UI.Map.Layers;

namespace OsmSharp.UI.Map
{
    /// <summary>
    /// Represents a renderable map.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Creates a new map.
        /// </summary>
        public Map()
        {
            this.Layers = new List<ILayer>();
        }

        /// <summary>
        /// Returns the child layers in this layer.
        /// </summary>
        public IList<ILayer> Layers { get; private set; }
    }
}
