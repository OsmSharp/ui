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
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Primitives;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Renderer.Scene.Primitives;
using OsmSharp.UI.Renderer.Scene.Styles;
using OsmSharp.Math.Geo.Projections;

namespace OsmSharp.UI.Renderer
{
	/// <summary>
	/// An abstract renderer of 2D objects.
	/// </summary>
	public abstract class Renderer2D<TTarget>
	{
	    /// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.Renderer2D"/> class.
	    /// </summary>
	    protected Renderer2D()
		{

		}

	    /// <summary>
	    /// Renders the given scene on the given target for the given view.
	    /// </summary>
        /// <param name="orginalTarget">The target to render to.</param>
	    /// <param name="primitives">The primitives to render.</param>
	    /// <param name="view">The current view.</param>
	    /// <param name="zoomFactor">The current zoom factor.</param>
        public bool Render(TTarget orginalTarget, View2D view, float zoomFactor, IEnumerable<Primitive2D> primitives)
        {
            return this.Render(orginalTarget, view, zoomFactor, primitives, null);
        }

	    /// <summary>
	    /// Renders the given scene on the given target for the given view.
	    /// </summary>
        /// <param name="orginalTarget">The target to render to.</param>
	    /// <param name="primitives">The primitives to render.</param>
	    /// <param name="view">The current view.</param>
	    /// <param name="zoomFactor">The current zoom factor.</param>
        /// <param name="backcolor">The backcolor.</param>
        public bool Render(TTarget orginalTarget, View2D view, float zoomFactor, IEnumerable<Primitive2D> primitives, int? backcolor)
		{
			try {
				this.SetRunning (true);

				if (view == null)
					throw new ArgumentNullException ("view");
                if (primitives == null)
                    throw new ArgumentNullException("primitives");
                if (orginalTarget == null)
	                throw new ArgumentNullException("orginalTarget");

	            // create the target wrapper.
	            Target2DWrapper<TTarget> target = this.CreateTarget2DWrapper(orginalTarget);

	            // the on before render event.
	            this.OnBeforeRender(target, view);

				// transform the target coordinates or notify the target of the
				// view coordinate system.
	            this.Transform(target, view);

                // render the backcolor.
                if(backcolor.HasValue)
                {
                    this.DrawBackColor(target, backcolor.Value);
                }

                // render the primitives.
                bool complete = this.RenderPrimitives(target, view, zoomFactor, primitives);

		        // the on after render event.
	            this.OnAfterRender(target, view);
				
				this.SetRunning (false);

				return complete;
			}
			catch(Exception ex)
            {
				OsmSharp.Logging.Log.TraceEvent ("Renderer2D", OsmSharp.Logging.TraceEventType.Error, 
				                                 ex.Message);
				this.SetRunning (false);
				throw ex;
			}
		}

		#region Running/Cancelling/State

		/// <summary>
		/// Holds the running flag.
		/// </summary>
		private bool _running = false;

		/// <summary>
		/// Holds the cancel flag.
		/// </summary>
		private bool _cancelFlag = false;

		/// <summary>
		/// Sets the running flag.
		/// </summary>
		/// <param name="running">If set to <c>true</c> running.</param>
		private void SetRunning(bool running)
		{
			_running = running;

			// always reset cancel-flag on new run or stopping run.
			_cancelFlag = false;
		}

		/// <summary>
		/// Cancels the current run.
		/// </summary>
		/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
		public void Cancel()
		{
			_cancelFlag = true;
		}

		/// <summary>
		/// Cancels the current run and wait until the current run is finished.
		/// </summary>
		/// <returns><c>true</c> if this instance cancel and wait; otherwise, <c>false</c>.</returns>
		public void CancelAndWait()
		{
			_cancelFlag = true;

			while (_running) { // wait and wait and wait.
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset ()
		{
			_cancelFlag = false;
			_running = false;
		}

		/// <summary>
		/// Gets a value indicating whether this renderer is running.
		/// </summary>
		/// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
		public bool IsRunning
		{
			get{
				return _running;
			}
		}

        /// <summary>
        /// Gets or sets the density.
        /// </summary>
        public virtual float Density { get; set; }

		#endregion

	    /// <summary>
	    /// Renders the primities for the given scene.
	    /// </summary>
	    /// <param name="target"></param>
        /// <param name="view"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="primitives"></param>
        private bool RenderPrimitives(Target2DWrapper<TTarget> target, View2D view, float zoomFactor, 
            IEnumerable<Primitive2D> primitives)
        {
			try {
                // calculate current simplification epsilon.
                double epsilon = Scene2D.CalculateSimplificationEpsilon(new WebMercator(), zoomFactor);

	            // loop over all primitives in the scene.
                int simplifiedLines = 0;
                int droppedLines = 0;
                foreach (Primitive2D primitive in primitives)
	            { // the primitive is visible.
					if (_cancelFlag) {
						return false; // stop rendering on cancel and return false for an incomplete rendering.
					}

                    if(primitive == null)
                    {
                        continue;
                    }
                    double[] x, y;
                    switch (primitive.Primitive2DType)
                    {
                        case Primitive2DType.Line2D:
                            Line2D line = (Line2D)primitive;

                            x = line.X;
                            y = line.Y;
                            if (x.Length > 4 && line.MaxZoom > zoomFactor * 2 && line.MaxZoom < 512)
                            { // try and simplify.
                                double[][] simplified = OsmSharp.Math.Algorithms.SimplifyCurve.Simplify(new double[][] { x, y },
                                    epsilon);
                                if (simplified[0].Length < line.X.Length)
                                {
                                    simplifiedLines++;
                                    x = simplified[0];
                                    y = simplified[1];
                                }
                                double distance = epsilon * 2;
                                if (simplified[0].Length == 2)
                                { // check if the simplified version is smaller than epsilon.
                                    distance = System.Math.Sqrt(
                                        System.Math.Pow((simplified[0][0] - simplified[0][1]), 2) +
                                        System.Math.Pow((simplified[1][0] - simplified[1][1]), 2));
                                }
                                if (distance < epsilon)
                                {
                                    droppedLines++;
                                    continue;
                                }
                            }
                            this.DrawLine(target, x, y, line.Color,
                                this.FromPixels(target, view, line.Width), line.LineJoin, line.Dashes);
                            break;
                        case Primitive2DType.Polygon2D:
                            Polygon2D polygon = (Polygon2D)primitive;

                            x = polygon.X;
                            y = polygon.Y;
                            //if (x.Length > 4 && polygon.MaxZoom > zoomFactor * 2 && polygon.MaxZoom < 512)
                            //{ // try and simplify.
                            //    double[][] simplified = OsmSharp.Math.Algorithms.SimplifyCurve.Simplify(new double[][] { x, y },
                            //        epsilon);
                            //    if (simplified[0].Length < polygon.X.Length)
                            //    {
                            //        simplifiedLines++;
                            //        x = simplified[0];
                            //        y = simplified[1];
                            //    }
                            //    double distance = epsilon * 2;
                            //    if (simplified[0].Length == 2)
                            //    { // check if the simplified version is smaller than epsilon.
                            //        distance = System.Math.Sqrt(
                            //            System.Math.Pow((simplified[0][0] - simplified[0][1]), 2) +
                            //            System.Math.Pow((simplified[1][0] - simplified[1][1]), 2));
                            //    }
                            //    //if (distance < epsilon)
                            //    //{
                            //    //    droppedLines++;
                            //    //    continue;
                            //    //}
                            //}
                            this.DrawPolygon(target, x, y, polygon.Color,
                                this.FromPixels(target, view, polygon.Width), polygon.Fill);
                            break;
                        case Primitive2DType.LineText2D:
                            LineText2D lineText = (LineText2D)primitive;
                            this.DrawLineText(target, lineText.X, lineText.Y, lineText.Text, lineText.Color,
                                this.FromPixels(target, view, lineText.Size), lineText.HaloColor, lineText.HaloRadius, lineText.Font);
                            break;
                        case Primitive2DType.Point2D:
                            Point2D point = (Point2D)primitive;
                            this.DrawPoint(target, point.X, point.Y, point.Color,
                                this.FromPixels(target, view, point.Size));
                            break;
                        case Primitive2DType.Icon2D:
                            Icon2D icon = (Icon2D)primitive;
                            this.DrawIcon(target, icon.X, icon.Y, icon.Image);
                            break;
                        case Primitive2DType.ImageTilted2D:
                            ImageTilted2D imageTilted = (ImageTilted2D)primitive;
                            this.DrawImage(target, imageTilted.Bounds, imageTilted.NativeImage);
                            break;
                        case Primitive2DType.Image2D:
                            Image2D image = (Image2D)primitive;
                            this.DrawImage(target, image.Left, image.Top, image.Right, image.Bottom, image.NativeImage);
                            break;
                        case Primitive2DType.Text2D:
                            Text2D text = (Text2D)primitive;
                            this.DrawText(target, text.X, text.Y, text.Text, text.Color,
                                this.FromPixels(target, view, text.Size), text.HaloColor, text.HaloRadius, text.Font);
                            break;
                    }
                }
				return true;
			}
			catch(Exception ex) {
				OsmSharp.Logging.Log.TraceEvent ("Renderer2D", OsmSharp.Logging.TraceEventType.Error, 
				                                 ex.Message);
				throw ex;
			}
        }

	    /// <summary>
	    /// Called before rendering starts.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="view"></param>
        protected virtual void OnBeforeRender(Target2DWrapper<TTarget> target, View2D view)
        {
            
        }

	    /// <summary>
	    /// Called after rendering is finished or was stopped.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="view"></param>
        protected virtual void OnAfterRender(Target2DWrapper<TTarget> target, View2D view)
        {

        }

	    /// <summary>
	    /// Creates a wrapper for the target making it possible to drag along some properties.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <returns></returns>
        public abstract Target2DWrapper<TTarget> CreateTarget2DWrapper(TTarget target);

	    /// <summary>
	    /// Returns the size in pixels.
	    /// </summary>
	    /// <returns>The pixels.</returns>
	    /// <param name="target"></param>
	    /// <param name="view">View.</param>
	    /// <param name="sizeInPixels">Size in pixels.</param>
		protected abstract double FromPixels(Target2DWrapper<TTarget> target, View2D view, double sizeInPixels);

	    /// <summary>
	    /// Transforms the target using the specified view.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="view">View.</param>
        protected abstract void Transform(Target2DWrapper<TTarget> target, View2D view);

	    /// <summary>
	    /// Draws the backcolor.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="backColor"></param>
        protected abstract void DrawBackColor(Target2DWrapper<TTarget> target, int backColor);

	    /// <summary>
	    /// Draws a point on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="size">Size.</param>
		protected abstract void DrawPoint(Target2DWrapper<TTarget> target, double x, double y, int color, double size);

	    /// <summary>
	    /// Draws a line on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="lineJoin"></param>
        /// <param name="dashes"></param>
		protected abstract void DrawLine(Target2DWrapper<TTarget> target, double[] x, double[] y, int color, double width, 
            LineJoin lineJoin, int[] dashes);

	    /// <summary>
	    /// Draws a polygon on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="fill">If set to <c>true</c> fill.</param>
		protected abstract void DrawPolygon(Target2DWrapper<TTarget> target, double[] x, double[] y, int color, double width, bool fill);

	    /// <summary>
	    /// Draws an icon on the target unscaled but centered at the given scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x"></param>
	    /// <param name="y"></param>
	    /// <param name="imageData"></param>
		protected abstract void DrawIcon(Target2DWrapper<TTarget> target, double x, double y, byte[] imageData);

	    /// <summary>
	    /// Draws an image on the target.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="left"></param>
	    /// <param name="top"></param>
	    /// <param name="right"></param>
	    /// <param name="bottom"></param>
	    /// <param name="imageData"></param>
        protected abstract void DrawImage(Target2DWrapper<TTarget> target, double left, double top, double right, double bottom, INativeImage tag);

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="target">Target.</param>
		/// <param name="bounds">Bounds.</param>
		/// <param name="imageData">Image data.</param>
		/// <param name="tag">Tag.</param>
        protected abstract void DrawImage(Target2DWrapper<TTarget> target, RectangleF2D bounds, INativeImage tag);

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="size"></param>
		protected abstract void DrawText(Target2DWrapper<TTarget> target, double x, double y, string text, int color, double size,
		                                 int? haloColor, int? haloRadius, string fontName);

        /// <summary>
        /// Draws text along a given line.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
		/// <param name="size"></param>
		/// <param name="text"></param>
        protected abstract void DrawLineText(Target2DWrapper<TTarget> target, double[] x, double[] y, string text, int color, 
		                                     double size, int? haloColor, int? haloRadius, string fontName);
    }
}