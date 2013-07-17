using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Represents a zoom selection in a MapCSS v0.2 Selector class.
    /// </summary>
	public class SelectorZoom
	{
        /// <summary>
        /// The minimum zoom level to match.
        /// </summary>
        public int? ZoomMin { get; set; }

        /// <summary>
        /// The maximum zoom level to match.
        /// </summary>
        public int? ZoomMax { get; set; }

        /// <summary>
        /// Returns true if this zoom was 'selected'.
        /// </summary>
        /// <param name="zoom">The current zoom level.</param>
        /// <returns></returns>
        public bool Select(int zoom)
        {
            if (this.ZoomMin.HasValue && this.ZoomMin.Value > zoom)
            { // oeps; the given zoom is smaller than the min zoom.
                return false;
            }
            if (this.ZoomMax.HasValue && this.ZoomMax.Value > zoom)
            { // oeps; the given zoom is larger than the max zoom.
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a description of this zoom selector.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("z{0}-{1}", this.ZoomMin.HasValue ? this.ZoomMin.ToString() : string.Empty
                                 , this.ZoomMax.HasValue ? this.ZoomMax.ToString() : string.Empty);
        }
	}
}
