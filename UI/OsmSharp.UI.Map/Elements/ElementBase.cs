// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Osm.Map.Elements
{
    public abstract class ElementBase : IElement
    {
        /// <summary>
        /// The minimum zoom level this element is visible at.
        /// </summary>
        private double? _min_zoom;

        /// <summary>
        /// The maximum zoom level this element is visible at.
        /// </summary>
        private double? _max_zoom;

        /// <summary>
        /// Creates a new basic element.
        /// </summary>
        protected ElementBase()
        {
            _min_zoom = null;
            _max_zoom = null;
        }

        /// <summary>
        /// Creates a new basic element.
        /// </summary>
        /// <param name="min_zoom"></param>
        /// <param name="max_zoom"></param>
        protected ElementBase(double? min_zoom, double? max_zoom)
        {
            _min_zoom = min_zoom;
            _max_zoom = max_zoom;
        }

        public double? MinZoom 
        {
            get
            {
                return _min_zoom;
            }
            set
            {
                _min_zoom = value;
            }
        }

        public double? MaxZoom
        {
            get
            {
                return _max_zoom;
            }
            set
            {
                _max_zoom = value;
            }
        }


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
        /// <param name="zoom_factor"></param>
        /// <returns></returns>
        public bool IsVisibleAt(double zoom_factor)
        {
            if (!_min_zoom.HasValue
                && !_max_zoom.HasValue)
            {
                return true;
            }
            else
            {

                if (_min_zoom.HasValue &&
                    _max_zoom.HasValue)
                {
                    return _min_zoom.Value < zoom_factor
                        && _max_zoom.Value >= zoom_factor;
                }
                else if (_min_zoom.HasValue)
                {
                    return _min_zoom.Value < zoom_factor;
                }
                else
                {
                    return _max_zoom.Value >= zoom_factor;
                }
            }
        }

        #endregion

        #region IElement Members

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
