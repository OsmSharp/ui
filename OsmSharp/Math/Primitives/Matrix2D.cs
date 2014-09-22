using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Math.Primitives
{
    public struct Matrix2D
    {
        /// <summary>
        /// Gets or sets the element at row 0, column 0.
        /// </summary>
        public double E11 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 0, column 1.
        /// </summary>
        public double E12 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 1, column 0.
        /// </summary>
        public double E21 { get; set; }

        /// <summary>
        /// Gets or sets the element at row 1, column 1.
        /// </summary>
        public double E22 { get; set; }

        /// <summary>
        /// Builds a rotation matrix for the given angle in radians.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix2D Rotate(Radian angle)
        {
            return Matrix2D.RotateClockwise(angle.Value);
        }

        /// <summary>
        /// Builds a rotation matrix for the given angle in radians.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Matrix2D RotateClockwise(double radians)
        {
            var sin = System.Math.Sin(radians);
            var cos = System.Math.Cos(radians);
            return new Matrix2D()
            {
                E11 = cos,
                E12 = sin,
                E21 = -sin,
                E22 = cos
            };
        }

        /// <summary>
        /// Creates a scaling matrix.
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Matrix2D Scale(double scale)
        {
            return new Matrix2D()
            {
                E11 = scale,
                E12 = 0,
                E21 = 0,
                E22 = scale
            };
        }

        /// <summary>
        /// Calculates the inverse matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix2D Inverse()
        {
            var det = 1 / ((this.E11 * this.E22) - (this.E12 * this.E21));
            return new Matrix2D()
            {
                E11 = this.E22 * det,
                E12 = -this.E12 * det,
                E21 = -this.E21 * det,
                E22 = this.E11 * det
            };
        }

        /// <summary>
        /// Multiplies the given 2D point with the new point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void Multiply2D(double x, double y, out double newX, out double newY)
        {
            newX = x * this.E11 + y * this.E12;
            newY = x * this.E21 + y * this.E22;
        }

        /// <summary>
        /// Multiplies the given point with the given matrix (p*m).
        /// </summary>
        /// <param name="point"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static PointF2D operator *(PointF2D point, Matrix2D matrix)
        {
            return new PointF2D(
                point[0] * matrix.E11 + point[1] * matrix.E12,
                point[0] * matrix.E21 + point[1] * matrix.E22);
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2D operator *(Matrix2D left, Matrix2D right)
        {
            return new Matrix2D()
            {
                E11 = (left.E11 * right.E11) + (left.E12 * right.E21),
                E12 = (left.E11 * right.E12) + (left.E12 * right.E22),
                E21 = (left.E21 * right.E21) + (left.E22 * right.E21),
                E22 = (left.E21 * right.E22) + (left.E22 * right.E22),
            };
        }
    }
}
