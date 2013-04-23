using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// An abstract representation of a layer.
    /// </summary>
    public interface ILayer
    {
        /// <summary>
        /// The minimum zoom.
        /// </summary>
        /// <remarks>
        /// The minimum zoom is the 'highest'.
        /// </remarks>
        float? MinZoom
        {
            get;
        }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        /// <remarks>
        /// The maximum zoom is the 'lowest' or most detailed view.
        /// </remarks>
        float? MaxZoom
        {
            get;
        }
    }
}
