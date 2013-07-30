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
        /// <param name="geometry">The object to 'select'.</param>
        /// <returns></returns>
        public virtual bool Selects(Geometry geometry, out KeyValuePair<int?, int?> zooms)
        {
            // store the zooms.
            if (this.Zoom == null)
            { // there are no zooms.
                zooms = new KeyValuePair<int?, int?>(
                    null, null);
            }
            else
            { // there are zooms.
                zooms = new KeyValuePair<int?, int?>(
                    this.Zoom.ZoomMin, this.Zoom.ZoomMax);
            }

            // check rule.
            if (this.SelectorRule != null &&
                !this.SelectorRule.Selects(geometry))
            { // oeps: the zoom was not valid.
                return false;
            }

            // check the type.
            switch (this.Type)
            {
                case SelectorTypeEnum.Area:
                    if (geometry is Polygon &&
                        geometry is LineairRing &&
                        geometry is MultiPolygon)
                    { // the geometry is an area!
                        return true;
                    }
                    break;
                case SelectorTypeEnum.Canvas:
                    // no way the canvas can be here!
                    break;
                case SelectorTypeEnum.Line:
                    if (geometry is LineString &&
                        geometry is MultiLineString)
                    { // the geometry is a line!
                        return true;
                    }
                    break;
                case SelectorTypeEnum.Star:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if this selector 'selects' the given object.
        /// </summary>
        /// <param name="zooms">The zooms selected.</param>
        /// <param name="osmGeo">The object to 'select'.</param>
        /// <returns></returns>
        public virtual bool Selects(CompleteOsmGeo osmGeo, out KeyValuePair<int?, int?> zooms)
        {
            // store the zooms.
            if (this.Zoom == null)
            { // there are no zooms.
                zooms = new KeyValuePair<int?, int?>(
                    null, null);
            }
            else
            { // there are zooms.
                zooms = new KeyValuePair<int?, int?>(
                    this.Zoom.ZoomMin, this.Zoom.ZoomMax);
            }

            // check rule.
            if (this.SelectorRule != null && 
			    !this.SelectorRule.Selects(osmGeo))
            { // oeps: the zoom was not valid.
                return false;
            }

            // check the type.
            switch (this.Type)
            {
                case SelectorTypeEnum.Area:
                    return false;
                case SelectorTypeEnum.Canvas:
                    // no way the canvas can be here!
                    break;
                case SelectorTypeEnum.Line:
                    return false;
                case SelectorTypeEnum.Node:
                    if (osmGeo.Type == CompleteOsmType.Node)
                    {
                        return true;
                    }
                    break;
                case SelectorTypeEnum.Star:
                    return true;
                case SelectorTypeEnum.Way:
                    if (osmGeo.Type == CompleteOsmType.Way)
                    {
                        return true;
                    }
                    break;
                case SelectorTypeEnum.Relation:
                    if (osmGeo.Type == CompleteOsmType.Relation)
                    {
                        return true;
                    }
                    break;
            }
            return false;
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