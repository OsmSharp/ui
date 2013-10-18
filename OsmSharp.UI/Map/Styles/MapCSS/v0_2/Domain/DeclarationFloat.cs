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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Eval;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public class DeclarationFloat : Declaration<DeclarationFloatEnum, float>
	{
		/// <summary>
		/// Evalues the value in this declaration or returns the regular value when there is no eval function.
        /// </summary>
        /// <param name="mapCSSObject">Map CSS object.</param>
		/// <returns></returns>
		public override float Eval (MapCSSObject mapCSSObject)
		{
			if (!string.IsNullOrWhiteSpace (this.EvalFunction)) {
				return EvalInterpreter.Instance.InterpretFloat (this.EvalFunction, mapCSSObject);
			}
			return this.Value;
		}
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public enum DeclarationFloatEnum
    {
        /// <summary>
        /// Width option.
        /// </summary>
        Width,
        /// <summary>
        /// Fill opacity option.
        /// </summary>
        FillOpacity,
        /// <summary>
        /// Opacity option.
        /// </summary>
        Opacity,
        /// <summary>
        /// Casing opacity option.
        /// </summary>
        CasingOpacity,
        /// <summary>
        /// Extrude edge opacity.
        /// </summary>
        ExtrudeEdgeOpacity,
        /// <summary>
        /// Extrude face opacity.
        /// </summary>
        ExtrudeFaceOpacity,
        /// <summary>
        /// Extrude edge width opacity.
        /// </summary>
        ExtrudeEdgeWidth,
        /// <summary>
        /// Icon opacity.
        /// </summary>
        IconOpacity,
        /// <summary>
        /// Text opacity.
        /// </summary>
        TextOpacity,
        /// <summary>
        /// Casing width opacity.
        /// </summary>
        CasingWidth
    }
}
