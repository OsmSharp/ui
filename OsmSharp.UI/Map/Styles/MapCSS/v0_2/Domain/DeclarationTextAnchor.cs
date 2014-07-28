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
    public class DeclarationTextAnchor : Declaration<DeclarationTextAnchorEnum, TextAnchorEnum>
    {

    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum DeclarationTextAnchorEnum
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum TextAnchorEnum
    {
        Left,
        Center,
        Right,
        Above,
        Top,
        Bottom,
        Below
    }
}
