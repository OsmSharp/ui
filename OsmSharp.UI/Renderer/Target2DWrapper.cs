using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer
{
    /// <summary>
    /// A wrapper for the target of the renderer.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public class Target2DWrapper<TTarget>
    {
        /// <summary>
        /// Creates a new target wrapper.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Target2DWrapper(TTarget target, float width, float height)
        {
            this.Target = target;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        public TTarget Target { get; set; }

        /// <summary>
        /// Gets/sets the back target.
        /// </summary>
        public TTarget BackTarget { get; set; }

        /// <summary>
        /// Gets/sets some extra data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets the orginal width in pixels.
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        /// Gets the orignal height in pixels.
        /// </summary>
        public float Height { get; private set; }
    }
}
