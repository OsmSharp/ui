// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.IO;
using Antlr.Runtime;

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

            this.DefaultPoints = false;
            this.DefaultLines = false;

            this.HasNodeIdSelector = false;
            this.HasWayIdSelector = false;
            this.HasRelationIdSelector = false;
        }

        /// <summary>
        /// Creates and parses mapcss from a string.
        /// </summary>
        /// <param name="css"></param>
        /// <returns></returns>
        public static MapCSSFile FromString(string css)
        {
            var input = new ANTLRStringStream(css);
            var lexer = new MapCSSLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new MapCSSParser(tokens);

            var tree = parser.stylesheet().Tree as Antlr.Runtime.Tree.CommonTree;

            // parse into domain.
            return MapCSSDomainParser.Parse(tree);
        }

        /// <summary>
        /// Creates and parses mapcss from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MapCSSFile FromStream(Stream stream)
        {
            // get the text from the embedded test file.
            var reader = new StreamReader(stream);
            string s = reader.ReadToEnd();

            return MapCSSFile.FromString(s);
        }

        /// <summary>
        /// The canvas fill color.
        /// </summary>
	    public int? CanvasFillColor { get; set; }

        /// <summary>
        /// Gets/sets the default points.
        /// </summary>
        public bool DefaultPoints { get; set; }

        /// <summary>
        /// Gets/sets the default lines.
        /// </summary>
        public bool DefaultLines { get; set; }
                 
        /// <summary>
        /// Gets or sets the node id selector flag.
        /// </summary>
        public bool HasNodeIdSelector { get; set; }

        /// <summary>
        /// Gets or sets the relation id selector flag.
        /// </summary>
        public bool HasRelationIdSelector { get; set; }

        /// <summary>
        /// Gets or sets the way id selector flag.
        /// </summary>
        public bool HasWayIdSelector { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Holds a list of all MapCSS rules.
        /// </summary>
	    public IList<Rule> Rules { get; private set; }
	}
}
