using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp;
using OsmSharp.Osm;
using OsmSharp.Geo.Geometries;

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
        /// <param name="zooms">The zooms selected.</param>
        /// <param name="mapCSSObject">The object to 'select'.</param>
        /// <returns></returns>
        public virtual bool Selects(MapCSSObject mapCSSObject, out SelectorZoom zooms)
        {
            // store the zooms.
            zooms = this.Zoom;

            // check the type.
            switch (this.Type)
            {
                case SelectorTypeEnum.Area:
                    if (mapCSSObject.MapCSSType != MapCSSType.Area)
                    {
                        return false;
                    }
                    break;
                case SelectorTypeEnum.Canvas:
                    // no way the canvas can be here!
                    break;
                case SelectorTypeEnum.Line:
                    if (mapCSSObject.MapCSSType != MapCSSType.Line)
                    {
                        return false;
                    }
                    break;
                case SelectorTypeEnum.Node:
                    if (mapCSSObject.MapCSSType != MapCSSType.Node)
                    {
                        return false;
                    }
                    break;
                case SelectorTypeEnum.Way:
                    if (mapCSSObject.MapCSSType != MapCSSType.Way)
                    {
                        return false;
                    }
                    break;
                case SelectorTypeEnum.Relation:
                    if (mapCSSObject.MapCSSType != MapCSSType.Relation)
                    {
                        return false;
                    }
                    break;
            }

            // object is of correct type: check rule.
            if (this.SelectorRule != null &&
                !this.SelectorRule.Selects(mapCSSObject))
            { // oeps: the zoom was not valid.
                return false;
            }
            return true; // object is of correct and selected by rule.
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