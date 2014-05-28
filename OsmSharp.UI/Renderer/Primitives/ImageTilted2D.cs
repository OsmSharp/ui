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

using OsmSharp.Math.Primitives;
using System;

namespace OsmSharp.UI.Renderer.Primitives
{
    /// <summary>
    /// Represents an image that is defined by any rectangle.
    /// </summary>
    public class ImageTilted2D : Primitive2D, IDisposable
    {
        /// <summary>
        /// Holds the bounds.
        /// </summary>
        private RectangleF2D _bounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.Scene.Scene2DPrimitives.TiltedImage2D"/> class.
        /// </summary>
        /// <param name="bounds">Bounds.</param>
        /// <param name="nativeImage">Image data.</param>
        public ImageTilted2D(RectangleF2D bounds, INativeImage nativeImage, float minZoom, float maxZoom)
        {
            _bounds = bounds;
            this.NativeImage = nativeImage;

            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The identifier.</value>
        public uint Id
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the type of this primitive.
        /// </summary>
        public override Primitive2DType Primitive2DType
        {
            get { return Primitives.Primitive2DType.ImageTilted2D; }
        }

        /// <summary>
        /// Holds the native image.
        /// </summary>
        private INativeImage _nativeImage;

        /// <summary>
        /// Gets or sets the native image.
        /// </summary>
        public INativeImage NativeImage 
        { 
            get
            {
                return _nativeImage;
            }
            set
            {
                _nativeImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public RectangleF2D Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = value;
            }
        }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        public float MaxZoom { get; set; }

        #region IScene2DPrimitive implementation

        /// <summary>
        /// Returns true if the object is visible on the view.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="view">View.</param>
        /// <param name="zoom"></param>
        public override bool IsVisibleIn(View2D view, float zoom)
        {
            if (this.MinZoom > zoom || this.MaxZoom < zoom)
            { // outside of zoom bounds!
                return false;
            }

            return _bounds.Overlaps(view.OuterBox);
        }

        /// <summary>
        /// Returns true if the object is visible on the view.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="view">View.</param>
        public override bool IsVisibleIn(View2D view)
        {
            return _bounds.Overlaps(view.OuterBox);
        }

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
        public override BoxF2D GetBox()
        {
            return _bounds.BoundingBox;
        }

        #endregion

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
            if(this._nativeImage != null)
            { // dispose of the native image.
                this._nativeImage.Dispose();
            }
        }        

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~ImageTilted2D()
        {
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        #endregion
    }
}