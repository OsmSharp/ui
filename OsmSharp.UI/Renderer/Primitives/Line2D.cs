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

using OsmSharp.Math.Primitives;
namespace OsmSharp.UI.Renderer.Primitives
{
    /// <summary>
    /// A simple 2D line.
    /// </summary>
    public class Line2D : Primitive2D
    {
        /// <summary>
        /// Creates a new line2D.
        /// </summary>
        public Line2D(double[] x, double[] y, int color, float width, LineJoin lineJoin, int[] dashes)
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;
            this.IsArrowLine = false;

            MinX = int.MaxValue;
            MaxX = int.MinValue;
            for (int idx = 0; idx < x.Length; idx++)
            {
                if (x[idx] > MaxX)
                {
                    MaxX = x[idx];
                }
                if (x[idx] < MinX)
                {
                    MinX = x[idx];
                }
            }
            MinY = int.MaxValue;
            MaxY = int.MinValue;
            for (int idx = 0; idx < y.Length; idx++)
            {
                if (y[idx] > MaxY)
                {
                    MaxY = y[idx];
                }
                if (y[idx] < MinY)
                {
                    MinY = y[idx];
                }
            }

            this.MinZoom = float.MinValue;
            this.MaxZoom = float.MaxValue;
        }

        /// <summary>
        /// Creates a new line2D.
        /// </summary>
        public Line2D(double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes,
            float minZoom, float maxZoom)
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;
            this.IsArrowLine = false;

            MinX = int.MaxValue;
            MaxX = int.MinValue;
            for (int idx = 0; idx < x.Length; idx++)
            {
                if (x[idx] > MaxX)
                {
                    MaxX = x[idx];
                }
                if (x[idx] < MinX)
                {
                    MinX = x[idx];
                }
            }
            MinY = int.MaxValue;
            MaxY = int.MinValue;
            for (int idx = 0; idx < y.Length; idx++)
            {
                if (y[idx] > MaxY)
                {
                    MaxY = y[idx];
                }
                if (y[idx] < MinY)
                {
                    MinY = y[idx];
                }
            }

            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
        }

        /// <summary>
        /// Creates a new line2D.
        /// </summary>
        public Line2D(double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes,
            int minX, int maxX, int minY, int maxY)
        {
            this.X = x;
            this.Y = y;
            this.Color = color;
            this.Width = width;
            this.LineJoin = lineJoin;
            this.Dashes = dashes;
            this.IsArrowLine = false;

            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;

            this.MinZoom = float.MinValue;
            this.MaxZoom = float.MaxValue;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Returns the type of this primitive.
        /// </summary>
        public override Primitive2DType Primitive2DType
        {
            get { return Primitives.Primitive2DType.Line2D; }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>The x.</value>
        public double[] X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the arrow line flag.
        /// </summary>
        public bool IsArrowLine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>The y.</value>
        public double[] Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public int Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public double Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the linejoin.
        /// </summary>
        public LineJoin LineJoin { get; set; }

        /// <summary>
        /// Gets or sets the line dashses.
        /// </summary>
        public int[] Dashes { get; set; }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        public float MaxZoom { get; set; }

        #region IScene2DPrimitive implementation

        internal double MinX { get; set; }
        internal double MaxX { get; set; }
        internal double MinY { get; set; }
        internal double MaxY { get; set; }

        /// <summary>
        /// Returns true if the object is visible on the view.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="view">View.</param>
        /// <param name="zoom"></param>
        public override bool IsVisibleIn(View2D view, float zoom)
        {
            if (this.MinZoom > zoom || this.MaxZoom < zoom)
            { // outside of zoom bounds!
                return false;
            }

            if (view.OverlapsWithBox(MinX, MinY, MaxX, MaxY))
            {
                return true; // maybe a better hittest?
            }
            return false;
        }

        /// <summary>
        /// Returns true if the object is visible on the view.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="view">View.</param>
        public override bool IsVisibleIn(View2D view)
        {
            if (view.OverlapsWithBox(MinX, MinY, MaxX, MaxY))
            {
                return true; // maybe a better hittest?
            }
            return false;
        }

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
        public override BoxF2D GetBox()
        {
            return new BoxF2D(MinX, MinY, MaxX, MaxY);
        }

        #endregion
    }

    /// <summary>
    /// Enumerated the different linejoin options.
    /// </summary>
    public enum LineJoin
    {
        Round,
        Miter,
        Bevel,
        None
    }

    /// <summary>
    /// Enumerates the different linecaps.
    /// </summary>
    public enum LineCap
    {
        /// <summary>
        /// Specifies a mask used to check whether a line cap is an anchor cap.
        /// </summary>
        AnchorMask,
        /// <summary>
        /// Specifies an arrow-shaped anchor cap.
        /// </summary>
        ArrowAnchor,
        /// <summary>
        /// Specifies a custom line cap.
        /// </summary>
        Custom,
        /// <summary>
        /// Specifies a diamond anchor cap.
        /// </summary>
        DiamondAnchor,
        /// <summary>
        /// Specifies a flat line cap.
        /// </summary>
        Flat,
        /// <summary>
        /// Specifies no anchor.
        /// </summary>
        NoAnchor,
        /// <summary>
        /// Specifies a round line cap.
        /// </summary>
        Round,
        /// <summary>
        /// Specifies a round anchor cap.
        /// </summary>
        RoundAnchor,
        /// <summary>
        /// Specifies a square line cap.
        /// </summary>
        Square,
        /// <summary>
        /// Specifies a square anchor line cap.
        /// </summary>
        SquareAnchor,
        /// <summary>
        /// Specifies a triangular line cap.
        /// </summary>
        Triangle
    }
}