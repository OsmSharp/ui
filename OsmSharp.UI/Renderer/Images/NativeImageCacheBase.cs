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

using OsmSharp.UI.Renderer.Primitives;

namespace OsmSharp.UI.Renderer.Images
{
    /// <summary>
    /// Abstract representation of a native image cache.
    /// </summary>
    public abstract class NativeImageCacheBase
    {
        /// <summary>
        /// Obtains one of the image from this cache created from the data in the byte array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract INativeImage Obtain(byte[] data);

        /// <summary>
        /// Release the image to this cache again.
        /// </summary>
        /// <param name="image"></param>
        public abstract void Release(INativeImage image);

        /// <summary>
        /// Flushes all images from this cache.
        /// </summary>
        public abstract void Flush();
    }
}