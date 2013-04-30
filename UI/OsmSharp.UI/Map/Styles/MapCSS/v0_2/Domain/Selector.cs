using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp;
using OsmSharp.Osm;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Strongly typed MapCSS v0.2 Selector class.
    /// </summary>
	public class Selector
	{
        /// <summary>
        /// The selector type.
        /// </summary>
        public SelectorTypeEnum Type { get; set; }

        /// <summary>
        /// The zoom selector.
        /// </summary>
        public SelectorZoom Zoom { get; set; }
        
        /// <summary>
        /// The selector rule.
        /// </summary>
        public SelectorRule SelectorRule { get; set; }

        /// <summary>
        /// Returns true if this selector 'selects' the given object.
        /// </summary>
        /// <param name="zoom">The current zoom level.</param>
        /// <param name="osmGeo">The object to 'select'.</param>
        /// <returns></returns>
        public virtual Dictionary<SelectorTypeEnum, OsmGeo> Selects(int zoom, OsmGeo osmGeo)
        {
            // osm geo list.
            var osmGeos = new Dictionary<SelectorTypeEnum, OsmGeo>();

            if (this.Zoom != null && !this.Zoom.Select(zoom))
            { // oeps: the zoom was not valid.
                return osmGeos;
            }

            // check rule.
            if (!this.SelectorRule.Selects(osmGeo))
            { // oeps: the zoom was not valid.
                return osmGeos;
            }

            // check the type.
            switch (this.Type)
            {
                case SelectorTypeEnum.Area:
                    if (osmGeo.Type == OsmType.Way)
                    { // this can be an area.
                        if (osmGeo.IsOfType(MapCSSTypes.Area))
                        {
                            osmGeos.Add(SelectorTypeEnum.Area, osmGeo);
                        }
                    }
                    else if (osmGeo.Type == OsmType.Relation)
                    {
                        // TODO: implement relation support.
                    }
                    break;
                case SelectorTypeEnum.Canvas:
                    // no way the canvas can be here!
                    break;
                case SelectorTypeEnum.Line:
                    if (osmGeo.Type == OsmType.Way)
                    { // this can be a line.
                        if (osmGeo.IsOfType(MapCSSTypes.Line))
                        {
                            osmGeos.Add(SelectorTypeEnum.Line, osmGeo);
                        }
                    }
                    else if (osmGeo.Type == OsmType.Relation)
                    {
                        // TODO: implement relation support.
                    }
                    break;
                case SelectorTypeEnum.Node:
                    if (osmGeo.Type == OsmType.Node)
                    {
                        osmGeos.Add(SelectorTypeEnum.Node, osmGeo);
                    }
                    break;
                case SelectorTypeEnum.Star:
                    osmGeos.Add(SelectorTypeEnum.Star, osmGeo);
                    break;
                case SelectorTypeEnum.Way:
                    if (osmGeo.Type == OsmType.Way)
                    {
                        osmGeos.Add(SelectorTypeEnum.Way, osmGeo);
                    }
                    break;
                case SelectorTypeEnum.Relation:
                    if (osmGeo.Type == OsmType.Relation)
                    {
                        osmGeos.Add(SelectorTypeEnum.Relation, osmGeo);
                    }
                    break;
            }
            return osmGeos; // all positive checks failed!
        }

        /// <summary>
        /// Returns a description of this selector.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Zoom != null && this.SelectorRule != null)
            {
                return string.Format("{0}|{1}{2}",
                                     this.Type.ToMapCSSString(), this.Zoom.ToString(), this.SelectorRule.ToString());
            }
            else if (this.Zoom != null)
            {
                return string.Format("{0}|{1}",
                                     this.Type.ToMapCSSString(), this.Zoom.ToString());
            }
            else if (this.SelectorRule != null)
            {
                return string.Format("{0}|{1}",
                                     this.Type.ToMapCSSString(), this.SelectorRule.ToString());
            }
            return this.Type.ToString();
        }
	}
}