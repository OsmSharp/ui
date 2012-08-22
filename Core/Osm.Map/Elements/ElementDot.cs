using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math.Shapes;

namespace Osm.Map.Elements
{
    public class ElementDot : ElementBase
    {
        /// <summary>
        /// The color of this dot.
        /// </summary>
        private int _color;

        /// <summary>
        /// The radius of this dot.
        /// </summary>
        private double _radius;

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
        public ElementDot(int color, double radius, ShapeDotF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> dot, bool fixed_width)
        {
            _dot = dot;
            _color = color;
            _radius = radius;
            _fixed_width = fixed_width;
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
            set
            {
                _color = value;
            }
        }

        /// <summary>
        /// Returns the readius of this dot.
        /// </summary>
        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
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
            set
            {
                _fixed_width = value;
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
