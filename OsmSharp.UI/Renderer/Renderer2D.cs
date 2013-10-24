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
using System.Linq;
using System.Text;

using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer
{
	/// <summary>
	/// An abstract renderer of 2D objects.
	/// </summary>
	public abstract class Renderer2D<TTarget>
	{
	    /// <summary>
	    /// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.Renderer2D`1"/> class.
	    /// </summary>
	    protected Renderer2D()
		{

		}

        /// <summary>
        /// Contains simpler version or even cached images of the last rendering.
        /// </summary>
	    private Scene2D _cachedScene;
		
        /// <summary>
        /// Renders the given scene on the given target for the given view.
        /// </summary>
        /// <param name="orginalTarget"></param>
        /// <param name="scene">Scene.</param>
        /// <param name="view">View.</param>
	    public bool Render(TTarget orginalTarget, Scene2D scene, View2D view)
	    {
            var scenes = new List<Scene2D>();
            scenes.Add(scene);
            return this.Render(orginalTarget, scenes, view);
	    }

	    /// <summary>
	    /// Renders the given scene on the given target for the given view.
	    /// </summary>
        /// <param name="orginalTarget"></param>
	    /// <param name="scenes">Scene.</param>
	    /// <param name="view">View.</param>
        public bool Render(TTarget orginalTarget, List<Scene2D> scenes, View2D view)
		{
			try {
	            if (scenes == null ||
	                scenes.Count == 0)
	            { // there is nothing to render!
	                return true;
	            }

				this.SetRunning (true);

				if (view == null)
					throw new ArgumentNullException ("view");
				if (scenes == null)
	                throw new ArgumentNullException("scenes");
	            if (orginalTarget == null)
	                throw new ArgumentNullException("orginalTarget");

	            // create the target wrapper.
	            Target2DWrapper<TTarget> target = this.CreateTarget2DWrapper(orginalTarget);

	            // the on before render event.
	            this.OnBeforeRender(target, scenes, view);

				// transform the target coordinates or notify the target of the
				// view coordinate system.
	            this.Transform(target, view);

	            // draw the backcolor.
	            this.DrawBackColor(target, scenes[0].BackColor);

				bool complete = true;
		        foreach (var scene in scenes)
		        {
				    // render the primitives.
					complete = complete  && 
						this.RenderPrimitives(target, scene, view);

					if (!complete) {
						break;
					}
		        }

		        // the on after render event.
	            this.OnAfterRender(target, scenes, view);

	            // build a cached version.
	            _cachedScene = this.BuildSceneCache(target, _cachedScene, scenes, view);
				
				this.SetRunning (false);

				return complete;
			}
			catch(Exception ex) {
				OsmSharp.Logging.Log.TraceEvent ("Renderer2D", System.Diagnostics.TraceEventType.Error, 
				                                 ex.Message);
				this.SetRunning (false);
			}
			return false;
		}

	    /// <summary>
	    /// Renders the cached scene on the given target for the given view.
	    /// </summary>
        /// <param name="orginalTarget"></param>
	    /// <param name="view"></param>
	    public bool RenderCache(TTarget orginalTarget, View2D view)
        {
			try {
	            if (_cachedScene != null)
	            {
					this.SetRunning (true);

	                if (view == null)
	                    throw new ArgumentNullException("view");
	                if (orginalTarget == null)
	                    throw new ArgumentNullException("orginalTarget");

	                // create the target wrapper.
	                Target2DWrapper<TTarget> target = this.CreateTarget2DWrapper(orginalTarget);

	                // transform the target coordinates or notify the target of the
	                // view coordinate system.
	                this.Transform(target, view);

	                // draw the backcolor.
	                this.DrawBackColor(target, _cachedScene.BackColor);

	                // render the primitives.
					bool complete = this.RenderPrimitives(target, _cachedScene, view);

					this.SetRunning (false);

					return complete;
	            }
				return true;
			}
			catch(Exception ex) {
				OsmSharp.Logging.Log.TraceEvent ("Renderer2D", System.Diagnostics.TraceEventType.Error, 
				                                 ex.Message);
				this.SetRunning (false);
			}
			return false;
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
		/// Gets a value indicating whether this renderer is running.
		/// </summary>
		/// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
		public bool IsRunning
		{
			get{
				return _running;
			}
		}

		#endregion

	    /// <summary>
	    /// Renders the primities for the given scene.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="scene"></param>
	    /// <param name="view"></param>
        private bool RenderPrimitives(Target2DWrapper<TTarget> target, Scene2D scene, View2D view)
        {
			try {
	            // TODO: calculate zoom.
				float zoom = (float)view.CalculateZoom(target.Width, target.Height);

	            // loop over all primitives in the scene.
	            foreach (Scene2DPrimitive scenePrimitive in scene.Get(view, zoom))
	            { // the primitive is visible.
	                IScene2DPrimitive primitive = scenePrimitive.Primitive;
					if (_cancelFlag) {
						return false; // stop rendering on cancel and return false for an incomplete rendering.
					}

					if (primitive is Point2D) {
						var point = (Point2D)(primitive);
						this.DrawPoint (target, point.X, point.Y, point.Color, 
						                              this.FromPixels (target, view, point.Size));
					} else if (primitive is Line2D) {
						var line = (Line2D)(primitive);
						this.DrawLine (target, line.X, line.Y, line.Color,
						                             this.FromPixels (target, view, line.Width), line.LineJoin, line.Dashes);
					} else if (primitive is Polygon2D) {
						var polygon = (Polygon2D)(primitive);
						this.DrawPolygon (target, polygon.X, polygon.Y, polygon.Color, 
						                                this.FromPixels (target, view, polygon.Width), polygon.Fill);
					} else if (primitive is Icon2D) {
						var icon = (Icon2D)(primitive);
						this.DrawIcon (target, icon.X, icon.Y, icon.Image);
					} else if (primitive is Image2D) {
						var image = (Image2D)(primitive);
						image.Tag = this.DrawImage (target, image.Left, image.Top, image.Right, image.Bottom, image.ImageData, 
						                                          image.Tag);
					} else if (primitive is ImageTilted2D) {
						var imageTilted = (ImageTilted2D)primitive;
						imageTilted.Tag = this.DrawImage (target, imageTilted.Bounds, imageTilted.ImageData, imageTilted.Tag);
					}
	                else if (primitive is Text2D)
	                {
	                    var text = (Text2D)(primitive);
	                    this.DrawText(target, text.X, text.Y, text.Text, text.Color,
	                        this.FromPixels(target, view, text.Size), text.HaloColor, text.HaloRadius, text.Font);
	                }
	                else if (primitive is LineText2D)
	                {
	                    var lineText = (LineText2D)(primitive);
	                    this.DrawLineText(target, lineText.X, lineText.Y, lineText.Text, lineText.Color,
						                  this.FromPixels(target, view, lineText.Size), lineText.HaloColor, lineText.HaloRadius, lineText.Font);
	                }
	            }
				return true;
			}
			catch(Exception ex) {
				OsmSharp.Logging.Log.TraceEvent ("Renderer2D", System.Diagnostics.TraceEventType.Error, 
				                                ex.Message);
			}
			return false;
        }

	    /// <summary>
	    /// Called before rendering starts.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="scenes"></param>
	    /// <param name="view"></param>
        protected virtual void OnBeforeRender(Target2DWrapper<TTarget> target, List<Scene2D> scenes, View2D view)
        {
            
        }

	    /// <summary>
	    /// Called after rendering stops.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="scenes"></param>
	    /// <param name="view"></param>
        protected virtual void OnAfterRender(Target2DWrapper<TTarget> target, List<Scene2D> scenes, View2D view)
        {

        }

	    /// <summary>
	    /// Builds the current cached scene.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="currentCache"></param>
	    /// <param name="currentScenes"></param>
	    /// <param name="view"></param>
        protected virtual Scene2D BuildSceneCache(Target2DWrapper<TTarget> target, Scene2D currentCache, List<Scene2D> currentScenes, View2D view)
	    {
	        return null;
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
		protected abstract object DrawImage(Target2DWrapper<TTarget> target, double left, double top, double right, double bottom, byte[] imageData, object tag);

		/// <summary>
		/// Draws the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="target">Target.</param>
		/// <param name="bounds">Bounds.</param>
		/// <param name="imageData">Image data.</param>
		/// <param name="tag">Tag.</param>
		protected abstract object DrawImage (Target2DWrapper<TTarget> target, RectangleF2D bounds, byte[] imageData, object tag);

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