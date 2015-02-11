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
using OsmSharp.UI.Renderer.Primitives;
#if __UNIFIED__
using CoreGraphics;
using UIKit;
#else
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
#endif

namespace OsmSharp.iOS.UI
{
    /// <summary>
    /// Represents a native image.
    /// </summary>
    public class NativeImage : INativeImage
    {
        /// <summary>
        /// Holds the native image.
        /// </summary>
        private CGImage _image;

        /// <summary>
        /// Creates a wrapped native image.
        /// </summary>
        /// <param name="image"></param>
        public NativeImage(CGImage image)
        {
            if (image == null) { throw new ArgumentNullException("Cannot create a native image wrapper around null"); }

            _image = image;
        }

        /// <summary>
        /// Gets or sets the bitmap.
        /// </summary>
        public CGImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        #region Disposing-pattern

        /// <summary>
        /// Diposes of all resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {            
            if (disposing == true)
            {
                //someone want the deterministic release of all resources
                //Let us release all the managed resources
            }
            else
            {
                // Do nothing, no one asked a dispose, the object went out of
                // scope and finalized is called so lets next round of GC 
                // release these resources
            }

            // Release the unmanaged resource in any case as they will not be 
            // released by GC
            if(this._image != null)
            { // dispose of the native image.
                this._image.Dispose();
                this._image = null;
            }
        }     

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~NativeImage()
        {
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        #endregion
        /// <summary>
        /// Returns true when the given object equals this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = (obj as NativeImage);
            if(other != null)
            {
                return other._image.Equals(this._image);
            }
            return false;
        }

        /// <summary>
        /// Returns the hashcode of this image.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 1332480824 ^
                this._image.GetHashCode();
        }
    }
}