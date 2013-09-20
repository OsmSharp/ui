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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Map.Styles.MapCSS
{
    /// <summary>
    /// An in-memory image source.
    /// </summary>
    public class MapCSSDictionaryImageSource : IMapCSSImageSource
    {
        /// <summary>
        /// Holds all images.
        /// </summary>
        private Dictionary<string, byte[]> _images;

        /// <summary>
        /// Creates a new dictionary image source.
        /// </summary>
        public MapCSSDictionaryImageSource()
        {
            _images = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// Adds a new image.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="imageData"></param>
        public void Add(string key, byte[] imageData)
        {
            _images[key] = imageData;
        }

        /// <summary>
        /// Adds a new image.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="stream"></param>
        public void Add(string key, Stream stream)
        {
            byte[] imageData = new byte[stream.Length];
            stream.Read(imageData, 0, (int)stream.Length);

            this.Add(key, imageData);
        }

        /// <summary>
        /// Returns an image for the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public bool TryGet(string name, out byte[] imageData)
        {
            return _images.TryGetValue(name, out imageData);
        }
    }
}