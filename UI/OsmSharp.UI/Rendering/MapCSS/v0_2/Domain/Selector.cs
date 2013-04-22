using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp;
using OsmSharp.Osm;

namespace OsmSharp.UI.Rendering.MapCSS.v0_2.Domain
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
        public virtual bool Selects(int zoom, OsmGeo osmGeo)
        {
            if (!this.Zoom.Select(zoom))
            { // oeps: the zoom was not valid.
                return false;
            }

            // check the type.
            switch (this.Type)
            {
                case SelectorTypeEnum.Area:
                    if (osmGeo.Type == OsmType.Way)
                    { // this can be an area.
                        return osmGeo.Tags.ContainsKeyValue("area", "yes");
                    }
                    else if (osmGeo.Type == OsmType.Relation)
                    {
                        // this can be an area.
                        return osmGeo.Tags.ContainsKeyValue("area", "yes");
                    }
                    break;
                case SelectorTypeEnum.Canvas:
                    // no way the canvas can be here!
                    break;
                case SelectorTypeEnum.Line:
                    if (osmGeo.Type == OsmType.Way)
                    { // this can be an area.
                        return osmGeo.Tags.ContainsKeyValue("area", "yes");
                    }
                    else if (osmGeo.Type == OsmType.Relation)
                    {
                        // this can be an area.
                        return osmGeo.Tags.ContainsKeyValue("area", "yes");
                    }
                    break;
                case SelectorTypeEnum.Node:
                    return osmGeo.Type == OsmType.Node;
                case SelectorTypeEnum.Star:
                    return true; // any object will do!
                case SelectorTypeEnum.Way:
                    return osmGeo.Type == OsmType.Way;
                case SelectorTypeEnum.Relation:
                    return osmGeo.Type == OsmType.Relation;
            }
            return false; // all positive checks failed!
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