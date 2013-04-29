using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.UI.Map.Elements
{
    /// <summary>
    /// Represents the basics of an element.
    /// </summary>
    public abstract class ElementBase // : IElement
    {
        /// <summary>
        /// Creates a new basic element.
        /// </summary>
        protected ElementBase()
        {
            this.MinZoom = null;
            this.MaxZoom = null;
        }

        /// <summary>
        /// Creates a new basic element.
        /// </summary>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        protected ElementBase(double? minZoom, double? maxZoom)
        {
            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
        }

        /// <summary>
        /// Gets/sets the minimum zoom.
        /// </summary>
        public double? MinZoom { get; set; }

        /// <summary>
        /// Gets/sets the maximum zoom.
        /// </summary>
        public double? MaxZoom { get; set; }

        /// <summary>
        /// Gets/sets the color.
        /// </summary>
        public SimpleColor? Color { get; set; }

        #region IElement Members

        /// <summary>
        /// Returns true if the element is visible inside the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public abstract bool IsVisibleIn(GeoCoordinateBox box);

        /// <summary>
        /// Returns the shortest distance from the given point to the given coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public abstract double ShortestDistanceTo(GeoCoordinate coordinate);

        /// <summary>
        /// Returns true if the element in visible at the given zoom level.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <returns></returns>
        public bool IsVisibleAt(double zoomFactor)
        {
            if (!this.MinZoom.HasValue
                && !this.MaxZoom.HasValue)
            {
                return true;
            }
            else
            {

                if (this.MinZoom.HasValue &&
                    this.MaxZoom.HasValue)
                {
                    return this.MinZoom.Value < zoomFactor
                        && this.MaxZoom.Value >= zoomFactor;
                }
                else if (this.MinZoom.HasValue)
                {
                    return this.MinZoom.Value < zoomFactor;
                }
                else
                {
                    return this.MaxZoom.Value >= zoomFactor;
                }
            }
        }

        /// <summary>
        /// Holds the dictionary of tags.
        /// </summary>
        private IDictionary<string, string> _tags;

        /// <summary>
        /// Returns a dictionary of tags.
        /// </summary>
        public IDictionary<string, string> Tags
        {
            get
            {
                if (_tags == null)
                {
                    _tags = new Dictionary<string, string>();
                }
                return _tags;
            }
        }

        #endregion
    }
}
