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
            return new Matrix2D()
            {
                E00 = factor,
                E01 = 0,
                E02 = 0,
                E10 = 0,
                E11 = factor,
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
