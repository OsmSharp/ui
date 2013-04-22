using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Rendering.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Strongly typed MapCSS v0.2 Rule class.
    /// </summary>
    public class Rule
	{
        /// <summary>
        /// List of selectors.
        /// </summary>
        public List<Selector> Selectors { get; set; }

        /// <summary>
        /// List of declarations.
        /// </summary>
        public List<Declaration> Declarations { get; set; }

        /// <summary>
        /// Returns a description of this rule.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (this.Selectors != null)
            {
                foreach (var selector in this.Selectors)
                {
                    builder.AppendLine(selector.ToString());
                }
            }
            builder.AppendLine("{");
            if (this.Declarations != null)
            {
                foreach (var declarations in this.Declarations)
                {
                    builder.AppendLine(declarations.ToString());
                    builder.Append(" ");
                }
            }
            builder.AppendLine("}");
            return builder.ToString();
        }
	}
}
