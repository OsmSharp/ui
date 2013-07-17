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
    public class DeclarationFontWeight : Declaration<QualifierFontWeightEnum, FontWeightEnum>
    {

    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum QualifierFontWeightEnum
    {
        FontWeight
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum FontWeightEnum
    {
        Bold,
        Normal
    }
}
