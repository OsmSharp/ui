// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Shapes;
using System.Drawing;

namespace OsmSharp.Osm.Map.Elements
{
    public class ElementText : ElementBase
    {
        /// <summary>
        /// The color of this dot.
        /// </summary>
        private int _color;

        /// <summary>
        /// The size of the text.
        /// </summary>
        private Font _font;

        /// <summary>
        /// The center of the text to place.
        /// </summary>
        private GeoCoordinate _center;

        /// <summary>
        /// The text to place.
        /// </summary>
        private string _text;

        /// <summary>
        /// Creates a new dot element.
        /// </summary>
        /// <param name="color"></param>
        public ElementText(int color, Font font, GeoCoordinate center, string text)
        {
            _color = color;
            _font = font;
            _center = center;
            _text = text;
        }

        /// <summary>
        /// Returns the color of this dot.
        /// </summary>
        public int Color
        {
            get
            {
                return _color;
            }
        }

        /// <summary>
        /// Returns the size.
        /// </summary>
        public Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }

        /// <summary>
        /// Returns the text.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public GeoCoordinate Center
        {
            get
            {
                return _center;
            }
        }

        #region IElement Members

        public override bool IsVisibleIn(GeoCoordinateBox box)
        {
            return box.IsInside(_center);
        }

        public override double ShortestDistanceTo(GeoCoordinate coordinate)
        {
            return coordinate.Distance(this.Center);
        }

        #endregion
    }
}
