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

namespace OsmSharp.Osm.Map.Elements
{
    public class ElementEllipse : ElementBase
    {
        /// <summary>
        /// The color of this dot.
        /// </summary>
        private int _color;

        /// <summary>
        /// The color at the edge (when drawing a gradient!).
        /// </summary>
        private int _edge_color;

        private double _x;
        private double _y;

        /// <summary>
        /// Fills the ellipse.
        /// </summary>
        private bool _fill;

        /// <summary>
        /// The width is fixed in pixels.
        /// </summary>
        private bool _fixed_width;

        /// <summary>
        /// The shape of this dot.
        /// </summary>
        private ShapeDotF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> _dot;

        /// <summary>
        /// Creates a new dot element.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <param name="dot"></param>
        public ElementEllipse(int color, double radius, ShapeDotF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> dot, bool fixed_width)
        {
            _dot = dot;
            _color = color;
            _x = radius;
            _y = radius;
            _fixed_width = fixed_width;
            _edge_color = -1;
        }

        /// <summary>
        /// Creates a new dot element.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="edge_color"></param>
        /// <param name="radius"></param>
        /// <param name="dot"></param>
        public ElementEllipse(int color,int edge_color, double radius, ShapeDotF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> dot, bool fixed_width)
        {
            _dot = dot;
            _color = color;
            _edge_color = edge_color;
            _x = radius;
            _y = radius;
            _fixed_width = fixed_width;
            _edge_color = edge_color;
        }

        /// <summary>
        /// Returns true if this ellipse is filled.
        /// </summary>
        public bool Filled
        {
            get
            {
                return _fill;
            }
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
        /// Returns the color of this ellipse at the edge.
        /// </summary>
        public int EdgeColor
        {
            get
            {
                return _edge_color;
            }
        }

        /// <summary>
        /// Returns the x-size of this ellipse.
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }
        }

        /// <summary>
        /// Returns the y-size of this ellipse.
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
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
        /// Returns the shape of this dot.
        /// </summary>
        public ShapeDotF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> Dot
        {
            get
            {
                return _dot;
            }
        }

        #region IElement Members

        public override double ShortestDistanceTo(GeoCoordinate coordinate)
        {
            return coordinate.Distance(this.Dot.Point);
        }

        public override bool IsVisibleIn(GeoCoordinateBox box)
        {
            return box.IsInside(_dot.Point);
        }

        #endregion
    }
}
