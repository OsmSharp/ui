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

namespace OsmSharp.UI.Renderer.Images
{
    /// <summary>
    /// Native image cache factory.
    /// 
    /// Uses dependency injection to build native images.
    /// </summary>
    public static class NativeImageCacheFactory
    {
        /// <summary>
        /// Delegate to create a native image cache.
        /// </summary>
        public delegate NativeImageCacheBase NativeImageCreate();

        /// <summary>
        /// The _native image cache create delegate.
        /// </summary>
        private static NativeImageCreate _nativeImageCacheCreateDelegate;

        /// <summary>
        /// Create a new native image cache.
        /// </summary>
        public static NativeImageCacheBase Create()
        {
            if (_nativeImageCacheCreateDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("Image creating delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _nativeImageCacheCreateDelegate.Invoke();
        }

        /// <summary>
        /// Sets the delegate.
        /// </summary>
        /// <param name="createNativeImage">Create native image.</param>
        public static void SetDelegate(NativeImageCreate createNativeImage)
        {
            _nativeImageCacheCreateDelegate = createNativeImage;
        }
    }
}