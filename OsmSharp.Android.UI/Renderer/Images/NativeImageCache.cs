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

using OsmSharp.UI.Renderer.Images;
using OsmSharp.UI.Renderer.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Android.UI.Renderer.Images
{
    /// <summary>
    /// Represents a native image cache.
    /// </summary>
    public class NativeImageCache : NativeImageCacheBase
    {
        /// <summary>
        /// Holds the unused images.
        /// </summary>
        private HashSet<INativeImage> _unusedImages;

        /// <summary>
        /// Holds the used images.
        /// </summary>
        private HashSet<INativeImage> _usedImages;

        /// <summary>
        /// Holds the cache size.
        /// </summary>
        private int _cacheSize;

        /// <summary>
        /// Creates a new native image cache.
        /// </summary>
        internal NativeImageCache()
            : this(300)
        {

        }

        /// <summary>
        /// Creates a new native image cache.
        /// </summary>
        /// <param name="cacheSize">The size of this image cache.</param>
        public NativeImageCache(int cacheSize)
        {
            _cacheSize = cacheSize;
            _unusedImages = new HashSet<INativeImage>();
            _usedImages = new HashSet<INativeImage>();
        }

        /// <summary>
        /// Obtains a new image from this cache.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override INativeImage Obtain(byte[] data)
        {
            OsmSharp.Logging.Log.TraceEvent("NativeImageCache",
                Logging.TraceEventType.Information, "Bitmap obtain: {0} {1}", _unusedImages.Count, _usedImages.Count);

            lock (_usedImages)
            { // synchronize access to this cache.
                if (_unusedImages.Count > 0)
                { // there are unused images, first recover one from there.
                    // get unused image and remove.
                    var unusedImage = _unusedImages.First();
                    _unusedImages.Remove(unusedImage);

                    var newNativeImage = global::Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length, new global::Android.Graphics.BitmapFactory.Options()
                    {
                        InBitmap = (unusedImage as NativeImage).Image,
                        InSampleSize = 1
                    });

                    // DecodeByteArray could return null: if we assign null to
                    // NativeImage.Image, NativeImage.GetHashCode crashes on a
                    // NullPointerException. So in this case, just roll back
                    // and return null. User code should handle this.
                    if (newNativeImage == null)
                    {
                        // Rollback
                        _unusedImages.Add(unusedImage);
                        return null;
                    }

                    // change native image and add to used images.
                    (unusedImage as NativeImage).Image = newNativeImage;
                    _usedImages.Add(unusedImage);
                    return unusedImage;
                }
                else if (_unusedImages.Count + _usedImages.Count < _cacheSize)
                { // there is still space to add a used iamge.
                    var newNativeImage = global::Android.Graphics.BitmapFactory.DecodeByteArray(data, 0, data.Length, new global::Android.Graphics.BitmapFactory.Options()
                    {
                        InSampleSize = 1,
                        InMutable = true
                    });

                    var usedImage = new NativeImage(newNativeImage);
                    _usedImages.Add(usedImage);
                    return usedImage;
                }
                else
                { // there are no unused images left and there is no more room.
                    throw new Exception("Cannot get a new image from cache, no image left.");
                }
            }
        }

        /// <summary>
        /// Release the given image.
        /// </summary>
        /// <param name="image">The image to release.</param>
        public override void Release(INativeImage image)
        {
            OsmSharp.Logging.Log.TraceEvent("NativeImageCache",
                Logging.TraceEventType.Information, "Bitmap release: {0} {1}", _unusedImages.Count, _usedImages.Count);

            lock (_usedImages)
            { // synchronize access to this cache.
                if (!_usedImages.Contains(image))
                { // oeps, cannot release an image that is not used!
                    OsmSharp.Logging.Log.TraceEvent("NativeImageCache",
                        Logging.TraceEventType.Information, "Bitmap release exception: {0} {1}", _unusedImages.Count, _usedImages.Count);
                    throw new Exception("Cannot release an unused image.");
                }

                _usedImages.Remove(image);
                _unusedImages.Add(image);
            }

            OsmSharp.Logging.Log.TraceEvent("NativeImageCache",
                Logging.TraceEventType.Information, "After bitmap release: {0} {1}", _unusedImages.Count, _usedImages.Count);
        }

        /// <summary>
        /// Flushes all images used.
        /// </summary>
        public override void Flush()
        {
            foreach (var images in _unusedImages)
            {
                images.Dispose();
            }
            _unusedImages.Clear();
            foreach (var images in _usedImages)
            {
                images.Dispose();
            }
            _usedImages.Clear();
        }
    }
}