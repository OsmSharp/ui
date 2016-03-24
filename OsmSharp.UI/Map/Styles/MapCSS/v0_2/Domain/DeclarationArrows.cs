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
    public class DeclarationArrows : Declaration<DeclarationArrowsQualifier, DeclarationArrowsEnum>
    {

    }

    /// <summary>
    /// Arrows qualifier.
    /// </summary>
    public enum DeclarationArrowsQualifier
    {
        /// <summary>
        /// Arrows.
        /// </summary>
        Arrows
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum DeclarationArrowsEnum
    {
        /// <summary>
        /// Forward option.
        /// </summary>
        Forward,
        /// <summary>
        /// Backward option.
        /// </summary>
        Backward
    }
}