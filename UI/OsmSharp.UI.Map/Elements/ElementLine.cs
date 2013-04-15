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
using System.Drawing.Drawing2D;

namespace OsmSharp.Osm.Map.Elements
{
    /// <summary>
    /// Elements representing a poly line.
    /// </summary>
    public class ElementLine : ElementBase
    {
        /// <summary>
        /// The color of this line.
        /// </summary>
        private int _color;

        /// <summary>
        /// The width of this line.
        /// </summary>
        private double _width;

        /// <summary>
        /// The width is fixed in pixels.
        /// </summary>
        private bool _fixed_width;

        /// <summary>
        /// The shape representing this line.
        /// </summary>
        private ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> _line;

        /// <summary>
        /// Creates a new element representing a poly line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="argb"></param>
        /// <param name="width"></param>
        public ElementLine(ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line,
            int color_argb, 
            double width)
        {
            _color = color_argb;
            _width = width;
            _line = line;
            _fixed_width = false;
            this.Style = LineStyle.Solid;
            this.StartCap = LineCap.Round;
            this.EndCap = LineCap.Round;
        }

        /// <summary>
        /// Creates a new element representing a poly line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="argb"></param>
        /// <param name="width"></param>
        public ElementLine(ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line,
            int color_argb,
            double width,
            bool fixed_width)
        {
            _color = color_argb;
            _width = width;
            _line = line;
            _fixed_width = fixed_width;
        }

        /// <summary>
        /// Returns the color for this line.
        /// </summary>
        public int Color
        {
            get
            {
                return _color;
            }
        }

        public LineStyle Style { get; set; }

        public LineCap StartCap { get; set; }
        public LineCap EndCap { get; set; }


        /// <summary>
        /// Returns the width of this line.
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
        }

        /// <summary>
        /// Returns true if the width does not scale.
        /// </summary>
        public bool FixedWidth
        {
            get
            {
                return _fixed_width;
            }
        }

        /// <summary>
        /// Retruns the shape of this line.
        /// </summary>
        public ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> Line
        {
            get
            {
                return _line;
            }
        }

        #region IElement Members

        public override bool IsVisibleIn(GeoCoordinateBox box)
        {
            return this.Line.Inside(box);
        }

        public override double ShortestDistanceTo(GeoCoordinate coordinate)
        {
            LineProjectionResult<GeoCoordinate> result = this.Line.ProjectOn(coordinate);
            return result.Distance;
        }

        #endregion
    }
}
