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
    /// Represents a simple 2D image.
    /// </summary>
    public class Image2D : Primitive2D, IDisposable
    {
        /// <summary>
        /// Creates a new Image2D.
        /// </summary>
        public Image2D()
        {

        }

        /// <summary>
        /// Creates a new Image2D.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="nativeImage"></param>
        public Image2D(double left, double top, double bottom, double right, INativeImage nativeImage)
        {
            this.NativeImage = nativeImage;
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;

            this.MinZoom = float.MinValue;
            this.MaxZoom = float.MaxValue;
        }

        /// <summary>
        /// Creates a new Image2D.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="imageData"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        public Image2D(double left, double top, double bottom, double right, INativeImage nativeImage, float minZoom, float maxZoom)
        {
            this.NativeImage = nativeImage;
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;

            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Returns the type of this primitive.
        /// </summary>
        public override Primitive2DType Primitive2DType
        {
            get { return Primitives.Primitive2DType.Image2D; }
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
        /// Gets the left.
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        public double Right { get; set; }

        /// <summary>
        /// Gets the top.
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        public double Bottom { get; set; }

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

            return view.OverlapsWithBox(this.Left, this.Top, this.Right, this.Bottom);
        }

        /// <summary>
        /// Returns true if the object is visible on the view.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="view">View.</param>
        /// 
        public override bool IsVisibleIn(View2D view)
        {
            return view.OverlapsWithBox(this.Left, this.Top, this.Right, this.Bottom);
        }

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
        public override BoxF2D GetBox()
        {
            return new BoxF2D(this.Left, this.Top, this.Right, this.Bottom);
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
        ~Image2D()
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