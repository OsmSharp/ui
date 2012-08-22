using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Geo.Lambert
{
    /// <summary>
    /// Represents a lambert coordinate for a given projection.
    /// </summary>
    public class LambertCoordinate
    {
        /// <summary>
        /// The projection this lambert coordinate is for.
        /// </summary>
        private LambertProjectionBase _projection;

        /// <summary>
        /// The x-part of this coordinate.
        /// </summary>
        private double _x;

        /// <summary>
        /// The y-part of this coordinate.
        /// </summary>
        private double _y;

        /// <summary>
        /// Creates a new lambert coordinate.
        /// </summary>
        /// <param name="projection"></param>
        public LambertCoordinate(LambertProjectionBase projection)
        {
            _projection = projection;
        }

        /// <summary>
        /// Gets the projection for this coordinate.
        /// </summary>
        public LambertProjectionBase Projection
        {
            get
            {
                return _projection;
            }
        }

        /// <summary>
        /// Gets/Sets the x-part of this coordinate.
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        /// <summary>
        /// Gets/Sets the y-part of this coordinate.
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }
    }
}
