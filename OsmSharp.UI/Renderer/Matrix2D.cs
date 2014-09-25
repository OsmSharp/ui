using OsmSharp.Math.Primitives;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer
{
    public struct Matrix2D
    {
        /// <summary>
        /// Gets or sets the element at row 0, column 0.
        /// </summary>
        public double E00 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 0, column 1.
        /// </summary>
        public double E01 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 0, column 2.
        /// </summary>
        public double E02 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 1, column 0.
        /// </summary>
        public double E10 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 1, column 1.
        /// </summary>
        public double E11 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 1, column 2.
        /// </summary>
        public double E12 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 2, column 0.
        /// </summary>
        public double E20 { get { return 0; } }

        /// <summary>
        /// Gets or sets the element at row 2, column 1.
        /// </summary>
        public double E21 { get { return 0; } }

        /// <summary>
        /// Gets or sets the element at row 2, column 2.
        /// </summary>
        public double E22 { get { return 1; } }

        /// <summary>
        /// Applies this transformation matrix to the given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xNew"></param>
        /// <param name="yNew"></param>
        public void Apply(double x, double y, out double xNew, out double yNew)
        {
            xNew = this.E00 * x + this.E01 * y + this.E02;
            yNew = this.E10 * x + this.E11 * y + this.E12;
        }

        /// <summary>
        /// Creates a tranformation matrix that transforms coordinates from inside the given rectangle to a viewport of given size. The bottom-left of the rectangle is assumed to be the origin.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Matrix2D FromRectangle(RectangleF2D rectangle, double width, double height)
        {
            return Matrix2D.FromRectangle(rectangle, width, height, false, false);
        }

        /// <summary>
        /// Creates a tranformation matrix that transforms coordinates from inside the given rectangle to a viewport of given size. The bottom-left of the rectangle is assumed to be the origin.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="invertX"></param>
        /// <param name="invertY"></param>
        /// <returns></returns>
        public static Matrix2D FromRectangle(RectangleF2D rectangle, double width, double height, bool invertX, bool invertY)
        {
            Matrix2D invert;
            if (!invertX && !invertY)
            { // everything normal.
                invert = Matrix2D.Scale(1, 1);
            }
            else if (!invertX && invertY)
            { // only y inverted.
                invert = Matrix2D.Scale(1, -1);
            }
            else if (invertX && !invertY)
            { // only x inverted.
                invert = Matrix2D.Scale(-1, 1);
            }
            else
            { // both inverted.
                invert = Matrix2D.Scale(-1, -1);
            }

            // translate the bottom-left of the rectangle to the origin.
            var translate = Matrix2D.Translate(-rectangle.BottomLeft[0], -rectangle.BottomLeft[1]);

            // rotate the rectangle to align with x-y axis.
            var rotate = Matrix2D.Rotate(-((Radian)rectangle.Angle).Value);

            // scale to match the width/height that was given.
            var scale = Matrix2D.Scale(width / rectangle.Width, height / rectangle.Height);

            return invert * translate * rotate * scale;
        }

        /// <summary>
        /// Creates a transformation matrix that transforms coordinates the viewport of given size to inside the given rectangle. The bottom-left of the rectangle is assumed to be the origin.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Matrix2D ToRectangle(RectangleF2D rectangle, double width, double height)
        {
            return Matrix2D.ToRectangle(rectangle, width, height, false, false);
        }

        /// <summary>
        /// Creates a transformation matrix that transforms coordinates the viewport of given size to inside the given rectangle. The bottom-left of the rectangle is assumed to be the origin.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="invertX"></param>
        /// <param name="invertY"></param>
        /// <returns></returns>
        public static Matrix2D ToRectangle(RectangleF2D rectangle, double width, double height, bool invertX, bool invertY)
        {
            // scale to match the width/height that was given.
            var scale = Matrix2D.Scale(rectangle.Width / width, rectangle.Height / height);

            // rotate to align with rectangle.
            var rotate = Matrix2D.Rotate(((Radian)rectangle.Angle).Value);

            // translate to the bottom-left of the rectangle.
            var translate = Matrix2D.Translate(rectangle.BottomLeft[0], rectangle.BottomLeft[1]);

            Matrix2D invert;
            if (!invertX && !invertY)
            { // everything normal.
                invert = Matrix2D.Scale(1, 1);
            }
            else if (!invertX && invertY)
            { // only y inverted.
                invert = Matrix2D.Scale(1, -1);
            }
            else if (invertX && !invertY)
            { // only x inverted.
                invert = Matrix2D.Scale(-1, 1);
            }
            else
            { // both inverted.
                invert = Matrix2D.Scale(-1, -1);
            }

            return scale * rotate * translate * invert;
        }

        /// <summary>
        /// Multiplies the two given matrices.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix2D operator*(Matrix2D a, Matrix2D b)
        {
            return new Matrix2D()
            {
                E00 = a.E00 * b.E00 + a.E10 * b.E01 + a.E20 * b.E02,
                E01 = a.E01 * b.E00 + a.E11 * b.E01 + a.E21 * b.E02,
                E02 = a.E02 * b.E00 + a.E12 * b.E01 + a.E22 * b.E02,
                E10 = a.E00 * b.E10 + a.E10 * b.E11 + a.E20 * b.E12,
                E11 = a.E01 * b.E10 + a.E11 * b.E11 + a.E21 * b.E12,
                E12 = a.E02 * b.E10 + a.E12 * b.E11 + a.E22 * b.E12
            };
        }

        /// <summary>
        /// Creates a transformation matrix for a scaling operation.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Matrix2D Scale(double factor)
        {
            return Matrix2D.Scale(factor, factor);
        }

        /// <summary>
        /// Creates a transformation matrix for a scaling operation.
        /// </summary>
        /// <param name="factorX"></param>
        /// <param name="factorY"></param>
        /// <returns></returns>
        public static Matrix2D Scale(double factorX, double factorY)
        {
            return new Matrix2D()
            {
                E00 = factorX,
                E01 = 0,
                E02 = 0,
                E10 = 0,
                E11 = factorY,
                E12 = 0
            };
        }

        /// <summary>
        /// Creates a transformation matrix for the given offsets.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Matrix2D Translate(double x, double y)
        {
            return new Matrix2D()
            {
                E00 = 1,
                E01 = 0,
                E02 = x,
                E10 = 0,
                E11 = 1,
                E12 = y
            };
        }

        /// <summary>
        /// Creates a transformation matrix for the give angle in clockwise direction.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Matrix2D Rotate(double radians)
        {
            var sin = System.Math.Sin(radians);
            var cos = System.Math.Cos(radians);
            return new Matrix2D()
            {
                E00 = cos,
                E01 = sin,
                E02 = 0,
                E10 = -sin,
                E11 = cos,
                E12 = 0
            };
        }
    }
}
