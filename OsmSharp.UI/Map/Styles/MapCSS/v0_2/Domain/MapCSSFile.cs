using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            var input = new ANTLRStringStream(s);
            var lexer = new MapCSSLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new MapCSSParser(tokens);

            var tree = parser.stylesheet().Tree as Antlr.Runtime.Tree.CommonTree;

            // parse into domain.
            return MapCSSDomainParser.Parse(tree);
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
