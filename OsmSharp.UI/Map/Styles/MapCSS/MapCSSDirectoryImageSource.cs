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
    /// An image source that gets it's image from a file system directory.
    /// </summary>
    public class MapCSSDirectoryImageSource : IMapCSSImageSource
    {
        /// <summary>
        /// Holds the directory info.
        /// </summary>
        private readonly DirectoryInfo _directoryInfo;

        /// <summary>
        /// Creates a new MapCSS image source.
        /// </summary>
        public MapCSSDirectoryImageSource(string path)
        {
            _directoryInfo = new DirectoryInfo(path);
        }

        /// <summary>
        /// Returns true if the image with the given name exists and sets the output parameter with the image data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public bool TryGet(string name, out byte[] imageData)
        {
            var imageFile = new FileInfo(_directoryInfo.FullName + name);
            if (imageFile.Exists)
            {
                FileStream fileStream = imageFile.OpenRead();
                imageData = new byte[fileStream.Length];
                fileStream.Read(imageData, 0, (int)fileStream.Length);
            }
            imageData = null;
            return false;
        }
    }
}