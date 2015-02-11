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
using OsmSharp.UI.Renderer.Images;
using OsmSharp.UI.Renderer.Primitives;
using System.Collections.Generic;
#if __UNIFIED__
using Foundation;
using UIKit;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
#endif

namespace OsmSharp.iOS.UI
{    
    /// <summary>
    /// Represents a native image cache.
    /// </summary>
    public class NativeImageCache : NativeImageCacheBase
    {
        /// <summary>
        /// Creates a new native image cache.
        /// </summary>
        internal NativeImageCache()
        {

        }

        /// <summary>
        /// Obtains a new image from this cache.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override INativeImage Obtain(byte[] data)
        {
            return new NativeImage(UIImage.LoadFromData(NSData.FromArray(data)).CGImage);
        }

        /// <summary>
        /// Release the given image.
        /// </summary>
        /// <param name="image">The image to release.</param>
        public override void Release(INativeImage image)
        {
            image.Dispose();
        }

        /// <summary>
        /// Flushes all images.
        /// </summary>
        public override void Flush()
        {
            // nothing to do here.
        }
    }
}