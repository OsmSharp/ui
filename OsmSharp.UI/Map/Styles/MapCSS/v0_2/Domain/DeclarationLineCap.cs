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
    public class DeclarationLineCap : Declaration<QualifierLineCapEnum, LineCapEnum>
    {

    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum QualifierLineCapEnum
    {
        LineCap,
        CasingLineCap
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum LineCapEnum
    {
        None,
        Round,
        Square,
        Butt
    }
}
