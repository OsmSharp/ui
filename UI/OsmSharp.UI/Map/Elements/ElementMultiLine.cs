using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Shapes;

namespace OsmSharp.UI.Map.Elements
{
    /// <summary>
    /// Represents a line.
    /// </summary>
    public class ElementMultiLine : ElementBase
    {
        /// <summary>
        /// Creates a new element representing a poly line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="colorArgb"></param>
        /// <param name="width"></param>
        public ElementMultiLine(ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line,
                           SimpleColor colorArgb,
                           double width)
        {
            this.Color = colorArgb;
            this.Width = width;
            this.Line = line;

            this.Cap = LineCap.Round;
        }

        /// <summary>
        /// Gets/sets the width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Retruns the shape of this line.
        /// </summary>
        public ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> Line { get; set; }
        
        /// <summary>
        /// Gets/sets the linecap.
        /// </summary>
        public LineCap Cap { get; set; }

        /// <summary>
        /// Gets/sets the linejoin.
        /// </summary>
        public LineJoin Join { get; set; }

        #region IElement Members

        /// <summary>
        /// Returns true if the line is visible in the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool IsVisibleIn(GeoCoordinateBox box)
        {
            return this.Line.Inside(box);
        }

        /// <summary>
        /// Returns the distances to this line.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public override double ShortestDistanceTo(GeoCoordinate coordinate)
        {
            LineProjectionResult<GeoCoordinate> result = this.Line.ProjectOn(coordinate);
            return result.Distance;
        }

        #endregion
    }

    /// <summary>
    /// An enumeration of possible linecaps.
    /// </summary>
    public enum LineCap
    {
        None,
        Round,
        Square,
        Butt
    }

    /// <summary>
    /// An enumeration of possible linejoins.
    /// </summary>
    public enum LineJoin
    {
        Round,
        Miter,
        Bevel
    }
}
