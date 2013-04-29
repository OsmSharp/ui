using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm;
using OsmSharp.Tools.Collections;
using OsmSharp.UI.Map.Elements;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;

namespace OsmSharp.UI.Map.Styles.MapCSS
{
    /// <summary>
    /// Represents a MapCSS interpreter.
    /// </summary>
    public class MapCSSInterpreter : StyleInterpreter
    {
        /// <summary>
        /// Holds the MapCSS file.
        /// </summary>
        private readonly MapCSSFile _mapCSSFile;

        /// <summary>
        /// Creates a new MapCSS interpreter.
        /// </summary>
        /// <param name="mapCSSFile"></param>
        public MapCSSInterpreter(MapCSSFile mapCSSFile)
        {
            _mapCSSFile = mapCSSFile;
        }

        /// <summary>
        /// Returns the canvas color.
        /// </summary>
        /// <returns></returns>
        public override SimpleColor? GetCanvasColor()
        {
            if (_mapCSSFile.CanvasFillColor.HasValue)
            {
                return new SimpleColor()
                           {
                               Value = _mapCSSFile.CanvasFillColor.Value
                           };
            }
            return null;
        }

        /// <summary>
        /// Translates OSM objects into basic renderable primitives.
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        public override IEnumerable<ElementBase> Translate(int zoom, OsmGeo osmGeo)
        {
            // instantiate the elements list.
            var elements = new List<ElementBase>();
            foreach (var rule in _mapCSSFile.Rules)
            {
                // select all elements.
                var selectedObjects = new HashSet<OsmGeo>();
                foreach (var selector in rule.Selectors)
                { // add elements that are selected.
                    foreach (var selectedObject in selector.Selects(zoom, osmGeo))
                    {
                        selectedObjects.Add(selectedObject);
                    }
                }

                if (selectedObjects.Count > 0)
                { // the object was selected. 
                    // translate it.
                    foreach (var declaration in rule.Declarations)
                    {

                    }
                }
            }
            return elements;
        }
    }
}