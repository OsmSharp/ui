using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Animations
{
    /// <summary>
    /// Parametric curve representing the progress of an animation over time.
    /// https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    /// Implementation directly taken from WebKit animation source: UnitBezier.h
    /// </summary>
    class CubicBezier
    {
        private double ax, bx, cx, ay, by, cy;

        public CubicBezier(float x1, float y1, float x2, float y2)
        {
            // Calculate the polynomial coefficients, implicit first and last control points are (0,0) and (1,1).
            cx = 3.0 * x1;
            bx = 3.0 * (x2 - x1) - cx;
            ax = 1.0 - cx - bx;

            cy = 3.0 * y1;
            by = 3.0 * (y2 - y1) - cy;
            ay = 1.0 - cy - by;
        }

        /// <summary>
        /// Compute the Y value on the curve indicating how far the animation has progressed, between 0 and 1.
        /// </summary>
        /// <param name="x">Current time between 0 and 1, relative to start and end of the animation.</param>
        /// <param name="epsilon">Percision in calculating the result. Long animations need more percise calculations to avoid ugly jumps.</param>
        /// <returns>Y value on the curve.</returns>
        public double ComputeY(double x, double epsilon)
        {
            return sampleCurveY(solveCurveX(x, epsilon));
        }

        /// <summary>
        /// Linear animation.
        /// </summary>
        /// <returns>Bezier representing a standard linear function/</returns>
        public static CubicBezier createLinear()
        {
            return new CubicBezier(0.250f, 0.250f, 0.750f, 0.750f);
        }

        /// <summary>
        /// The ease animation also found in modern WebKit browsers.
        /// </summary>
        /// <returns>Bezier representing a standard easing function/</returns>
        public static CubicBezier createEase()
        {
            return new CubicBezier(0.250f, 0.100f, 0.250f, 1.000f);
        }



        private double sampleCurveX(double t)
        {
            // `ax t^3 + bx t^2 + cx t' expanded using Horner's rule.
            return ((ax * t + bx) * t + cx) * t;
        }

        private double sampleCurveY(double t)
        {
            return ((ay * t + by) * t + cy) * t;
        }

        private double sampleCurveDerivativeX(double t)
        {
            return (3.0 * ax * t + 2.0 * bx) * t + cx;
        }

        // Given an x value, find a parametric value it came from.
        private double solveCurveX(double x, double epsilon)
        {
            double t0;
            double t1;
            double t2;
            double x2;
            double d2;
            int i;

            // First try a few iterations of Newton's method -- normally very fast.
            for (t2 = x, i = 0; i < 8; i++)
            {
                x2 = sampleCurveX(t2) - x;
                if (System.Math.Abs(x2) < epsilon)
                    return t2;
                d2 = sampleCurveDerivativeX(t2);
                if (System.Math.Abs(d2) < 1e-6)
                    break;
                t2 = t2 - x2 / d2;
            }

            // Fall back to the bisection method for reliability.
            t0 = 0.0;
            t1 = 1.0;
            t2 = x;

            if (t2 < t0)
                return t0;
            if (t2 > t1)
                return t1;

            while (t0 < t1)
            {
                x2 = sampleCurveX(t2);
                if (System.Math.Abs(x2 - x) < epsilon)
                    return t2;
                if (x > x2)
                    t0 = t2;
                else
                    t1 = t2;
                t2 = (t1 - t0) * .5 + t0;
            }

            // Failure.
            return t2;
        }
    }
}
