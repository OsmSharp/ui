using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm;
using OsmSharp.UI.Renderer;
using OsmSharp;

namespace OsmSharp.UI.Map.Styles
{
    /// <summary>
    /// Represents a style interpreter.
    /// </summary>
    public abstract class StyleInterpreter
    {
        /// <summary>
        /// Returns the 
        /// </summary>
        /// <returns></returns>
        public abstract SimpleColor? GetCanvasColor();

        /// <summary>
        /// Translates the given OSM objects into corresponding geometries.
        /// </summary>
        /// <param name="projection">The projection to use.</param>
        /// <param name="zoom">The zoom factor.</param>
        /// <param name="osmGeo">The osm object.</param>
        /// <param name="scene">The scene to fill with the resulting geometries.</param>
        /// <returns></returns>
        public abstract void Translate(Scene2D scene, IProjection projection, float zoom, CompleteOsmGeo osmGeo);
    }
}