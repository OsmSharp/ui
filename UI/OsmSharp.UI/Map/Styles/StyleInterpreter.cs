using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm;
using OsmSharp.UI.Map.Elements;

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
        /// Translates the given object into corresponding elements.
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        public abstract IEnumerable<ElementBase> Translate(int zoom, OsmGeo osmGeo);
    }
}
