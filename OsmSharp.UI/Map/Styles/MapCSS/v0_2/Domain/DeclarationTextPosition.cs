using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public class DeclarationTextPosition : Declaration<DeclarationTextPositionEnum, TextPositionEnum>
    {

    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum DeclarationTextPositionEnum
    {
        TextPosition
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum TextPositionEnum
    {
        Line,
        Center
    }
}
