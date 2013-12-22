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

using System.Collections.Generic;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2
{
    /// <summary>
    /// Holds all possible MapCSS rule newProperties.
    /// </summary>
    internal class MapCSSRuleProperties
    {
        /// <summary>
        /// Holds the properties.
        /// </summary>
        private readonly Dictionary<string, object> _dictionary;

        /// <summary>
        /// Creates a new set of MapCSS newProperties.
        /// </summary>
        public MapCSSRuleProperties()
        {
            _dictionary = new Dictionary<string, object>();

            this.MinZoom = 0;
            this.MaxZoom = 20;
        }

        /// <summary>
        /// Creates a new set of MapCSS newProperties.
        /// </summary>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        public MapCSSRuleProperties(int minZoom, int maxZoom)
        {
            _dictionary = new Dictionary<string, object>();

            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
        }

        /// <summary>
        /// Add value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddProperty<T>(string key, T value)
        {
            _dictionary[key] = value;
        }

        /// <summary>
        /// Returns the value of the given properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetProperty<T>(string key, out T value)
        {
            object untypedValue;
            if (_dictionary.TryGetValue(key, out untypedValue))
            {
                value = (T) untypedValue;
                return true;
            }
            value = default(T);
            return false;
        }

        /// <summary>
        /// Returns all the keys.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetKeys()
        {
            return _dictionary.Keys;
        }

        /// <summary>
        /// Min zoom, the range here is [MinZoom, MaxZoom[.
        /// </summary>
        public int MinZoom { get; set; }

        /// <summary>
        /// Max zoom,the range here is [MinZoom, MaxZoom[.
        /// </summary>
        public int MaxZoom { get; set; }

        /// <summary>
        /// Returns true if the min/max zoom are equal.
        /// </summary>
        /// <param name="zoomMin"></param>
        /// <param name="zoomMax"></param>
        /// <returns></returns>
        public bool IsEqualForZoom(int zoomMin, int zoomMax)
        {
            return this.MinZoom == zoomMin &&
                   this.MaxZoom == zoomMax;
        }

        /// <summary>
        /// Returns true if the min/max zoom are equal.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public bool IsEqualForZoom(MapCSSRuleProperties properties)
        {
            return this.IsEqualForZoom(
                properties.MinZoom, properties.MaxZoom);
        }

        /// <summary>
        /// Merges the given newProperties with this one.
        /// </summary>
        /// <param name="newProperties"></param>
        /// <returns></returns>
        internal MapCSSRuleProperties Merge(MapCSSRuleProperties newProperties)
        {
            // determine the tightest zoom.
            int minZoom = this.MinZoom;
            int maxZoom = this.MaxZoom;
            if (this.MinZoom < newProperties.MinZoom)
            {
                minZoom = newProperties.MinZoom;
            }
            if (this.MaxZoom > newProperties.MaxZoom)
            {
                maxZoom = newProperties.MaxZoom;
            }

            // create a new rule and merge properties from both.
            var rule= new MapCSSRuleProperties(minZoom, maxZoom);
            foreach (var key in this.GetKeys())
            {
                object value;
                this.TryGetProperty(key, out value);
                rule.AddProperty(key, value);
            }
            foreach (var key in newProperties.GetKeys())
            {
                object value;
                newProperties.TryGetProperty(key, out value);
                rule.AddProperty(key, value);
            }
            return rule;
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        internal bool IsForZoom(int zoom)
        {
            if (this.MinZoom <= zoom &&
                this.MaxZoom >= zoom)
            {
                return true;
            }
            return false;
        }
    }
}