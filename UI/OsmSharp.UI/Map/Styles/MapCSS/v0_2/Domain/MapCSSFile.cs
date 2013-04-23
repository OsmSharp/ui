using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Represents one MapCSS file.
    /// </summary>
	public class MapCSSFile
	{
        /// <summary>
        /// Creates a new MapCSSFile object.
        /// </summary>
        public MapCSSFile()
        {
            this.Rules = new List<Rule>();
        }

        /// <summary>
        /// The canvas fill color.
        /// </summary>
	    public int? CanvasFillColor { get; set; }

        /// <summary>
        /// Holds a list of all MapCSS rules.
        /// </summary>
	    public IList<Rule> Rules { get; private set; }
	}
}
