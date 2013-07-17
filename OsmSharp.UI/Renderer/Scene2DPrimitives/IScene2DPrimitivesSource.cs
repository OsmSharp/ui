using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives
{
    /// <summary>
    /// Abstracts a source of scene primitives.
    /// </summary>
    public interface IScene2DPrimitivesSource
    {
        /// <summary>
        /// Adds all primitives inside the given box for the given zoom.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="view"></param>
        /// <param name="zoomFactor"></param>
        /// <returns></returns>
        void Get(Scene2D scene, View2D view, float zoomFactor);
    }
}