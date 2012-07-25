using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Tools.Math.Shapes;
using System.Drawing.Drawing2D;

namespace Osm.Map.Elements
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
