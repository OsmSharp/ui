using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Shapes;
using Tools.Math.Geo;

namespace Osm.Map.Elements
{
    public class ElementPolygon : ElementBase
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
        /// Fills the polygon.
        /// </summary>
        private bool _fill;

        /// <summary>
        /// The width is fixed in pixels.
        /// </summary>
        private bool _fixed_width;

        /// <summary>
        /// The shape representing this line.
        /// </summary>
        private ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> _polygon;

        /// <summary>
        /// Creates a new element representing a polygon that is not filled.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="argb"></param>
        /// <param name="width"></param>
        public ElementPolygon(ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> polygon,
            int color_argb,
            double width,
            bool filled,
            bool fixed_width)
        {
            _color = color_argb;
            _width = width;
            _polygon = polygon;
            _fill = filled;
            _fixed_width = fixed_width;
        }

        /// <summary>
        /// Creates a new element representing a polygon that is not filled.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="argb"></param>
        /// <param name="width"></param>
        public ElementPolygon(ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> polygon,
            int color_argb, 
            double width)
        {
            _color = color_argb;
            _width = width;
            _polygon = polygon;
            _fill = false;
            _fixed_width = false;
        }

        /// <summary>
        /// Creates a new element representing a filled polygon.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="color_argb"></param>
        public ElementPolygon(ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> polygon,
            int color_argb)
        {
            _color = color_argb;
            _polygon = polygon;
            _fill = true;
            _fixed_width = false;
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
        /// Returns true if this polygon is filled.
        /// </summary>
        public bool Filled
        {
            get
            {
                return _fill;
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
        /// Retruns the shape of this polygon.
        /// </summary>
        public ShapePolyGonF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> Polygon
        {
            get
            {
                return _polygon;
            }
        }

        public override bool IsVisibleIn(GeoCoordinateBox box)
        {
            return this.Polygon.Inside(box);
        }

        public override double ShortestDistanceTo(GeoCoordinate coordinate)
        {
            PolygonProjectionResult<GeoCoordinate> result = this.Polygon.ProjectOn(coordinate);
            return result.Distance;
        }
    }
}
