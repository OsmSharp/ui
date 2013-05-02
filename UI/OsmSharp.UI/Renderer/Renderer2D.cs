
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OsmSharp.UI.Renderer.Scene2DPrimitives;

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
        public void Render(TTarget orginalTarget, Scene2D scene, View2D view)
		{
			if (view == null)
				throw new ArgumentNullException ("view");
			if (scene == null)
                throw new ArgumentNullException("scene");
            if (orginalTarget == null)
                throw new ArgumentNullException("orginalTarget");

            // create the target wrapper.
            Target2DWrapper<TTarget> target = this.CreateTarget2DWrapper(orginalTarget);

            // the on before render event.
            this.OnBeforeRender(target, scene, view);

			// transform the target coordinates or notify the target of the
			// view coordinate system.
            this.Transform(target, view);

            // draw the backcolor.
            this.DrawBackColor(target, scene.BackColor);

			// render the primitives.
            this.RenderPrimitives(target, scene, view);

            // the on after render event.
            this.OnAfterRender(target, scene, view);

            // build a cached version.
            _cachedScene = this.BuildSceneCache(target, _cachedScene, scene, view);
		}

	    /// <summary>
	    /// Renders the cached scene on the given target for the given view.
	    /// </summary>
        /// <param name="orginalTarget"></param>
	    /// <param name="view"></param>
	    public void RenderCache(TTarget orginalTarget, View2D view)
        {
            if (_cachedScene != null)
            {
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
                this.RenderPrimitives(target, _cachedScene, view);
            }
        }

	    /// <summary>
	    /// Renders the primities for the given scene.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="scene"></param>
	    /// <param name="view"></param>
        private void RenderPrimitives(Target2DWrapper<TTarget> target, Scene2D scene, View2D view)
        {
            // loop over all primitives in the scene.
            foreach (IScene2DPrimitive primitive in scene.Get(view))
            {
                if (primitive is Point2D)
                {
                    Point2D point = (Point2D)(primitive);

                    this.DrawPoint(target, point.X, point.Y, point.Color, this.FromPixels(target, view, point.Size));
                }
                else if (primitive is Line2D)
                {
                    Line2D line = (Line2D)(primitive);

                    this.DrawLine(target, line.X, line.Y, line.Color, this.FromPixels(target, view, line.Width), line.LineJoin, line.Dashes);
                }
                else if (primitive is Polygon2D)
                {
                    Polygon2D polygon = (Polygon2D)(primitive);

                    this.DrawPolygon(target, polygon.X, polygon.Y, polygon.Color, this.FromPixels(target, view, polygon.Width), polygon.Fill);
                }
                else if (primitive is Icon2D)
                {
                    Icon2D icon = (Icon2D)(primitive);

                    this.DrawIcon(target, icon.X, icon.Y, icon.Image);
                }
                else if (primitive is Image2D)
                {
                    var image = (Image2D)(primitive);

                    image.Tag = this.DrawImage(target, image.Left, image.Top, image.Right, image.Bottom, image.ImageData, image.Tag);
                }
            }
        }

	    /// <summary>
	    /// Called before rendering starts.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="scene"></param>
	    /// <param name="view"></param>
        protected virtual void OnBeforeRender(Target2DWrapper<TTarget> target, Scene2D scene, View2D view)
        {
            
        }

	    /// <summary>
	    /// Called after rendering stops.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="scene"></param>
	    /// <param name="view"></param>
        protected virtual void OnAfterRender(Target2DWrapper<TTarget> target, Scene2D scene, View2D view)
        {

        }

	    /// <summary>
	    /// Builds the current cached scene.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="currentCache"></param>
	    /// <param name="currentScene"></param>
	    /// <param name="view"></param>
        protected virtual Scene2D BuildSceneCache(Target2DWrapper<TTarget> target, Scene2D currentCache, Scene2D currentScene, View2D view)
	    {
	        return currentScene;
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
        protected abstract float FromPixels(Target2DWrapper<TTarget> target, View2D view, float sizeInPixels);

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
        protected abstract void DrawPoint(Target2DWrapper<TTarget> target, float x, float y, int color, float size);

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
        protected abstract void DrawLine(Target2DWrapper<TTarget> target, float[] x, float[] y, int color, float width, LineJoin lineJoin, int[] dashes);

	    /// <summary>
	    /// Draws a polygon on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="fill">If set to <c>true</c> fill.</param>
        protected abstract void DrawPolygon(Target2DWrapper<TTarget> target, float[] x, float[] y, int color, float width, bool fill);


	    /// <summary>
	    /// Draws an icon on the target unscaled but centered at the given scene coordinates.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="x"></param>
	    /// <param name="y"></param>
	    /// <param name="imageData"></param>
        protected abstract void DrawIcon(Target2DWrapper<TTarget> target, float x, float y, byte[] imageData);

	    /// <summary>
	    /// Draws an image on the target.
	    /// </summary>
	    /// <param name="target"></param>
	    /// <param name="left"></param>
	    /// <param name="top"></param>
	    /// <param name="right"></param>
	    /// <param name="bottom"></param>
	    /// <param name="imageData"></param>
        protected abstract object DrawImage(Target2DWrapper<TTarget> target, float left, float top, float right, float bottom, byte[] imageData, object tag);
	}
}