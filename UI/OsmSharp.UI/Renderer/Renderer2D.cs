
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
		/// <param name="target">Target.</param>
		protected Renderer2D(TTarget target)
		{
			this.Target = target;
		}

		/// <summary>
		/// Gets the target.
		/// </summary>
		/// <value>The target.</value>
		public TTarget Target {
			get;
			private set;
		}

        /// <summary>
        /// Renders all the given scenes.
        /// </summary>
        /// <param name="scenes"></param>
        /// <param name="view"></param>
        public void Render(IEnumerable<Scene2D> scenes, View2D view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (scenes == null)
                throw new ArgumentNullException("scenes");

            // transform the target coordinates or notify the target of the
            // view coordinate system.
            this.Transform(view);

            // loop over all primitives in the scene.
            foreach (var scene in scenes)
            {
                // draw the backcolor.
                this.DrawBackColor(scene.BackColor);

                // draw all visible primitives.
                foreach (IScene2DPrimitive primitive in scene.Get(view))
                {
                    if (primitive is Point2D)
                    {
                        Point2D point = (Point2D)(primitive);

                        this.DrawPoint(point.X, point.Y, point.Color, point.Size);
                    }
                    else if (primitive is Line2D)
                    {
                        Line2D line = (Line2D)(primitive);

                        this.DrawLine(line.X, line.Y, line.Color, this.FromPixels(view, line.Width), line.LineJoin);
                    }
                    else if (primitive is Polygon2D)
                    {
                        Polygon2D polygon = (Polygon2D)(primitive);

                        this.DrawPolygon(polygon.X, polygon.Y, polygon.Color, polygon.Width, polygon.Fill);
                    }
                }   
            }
        }

		/// <summary>
		/// Renders the current scene on the given target for the given view.
		/// </summary>
		/// <param name="scene">Scene.</param>
		/// <param name="view">View.</param>
		public void Render(Scene2D scene, View2D view)
		{
			if (view == null)
				throw new ArgumentNullException ("view");
			if (scene == null)
				throw new ArgumentNullException ("scene");

			// transform the target coordinates or notify the target of the
			// view coordinate system.
			this.Transform(view);

            // draw the backcolor.
            this.DrawBackColor(scene.BackColor);

			// loop over all primitives in the scene.
			foreach(IScene2DPrimitive primitive in scene.Get(view))
			{
				if(primitive is Point2D)
				{
					Point2D point = (Point2D)(primitive);

					this.DrawPoint(point.X, point.Y, point.Color, point.Size);
				}
				else if(primitive is Line2D)
				{
					Line2D line = (Line2D)(primitive);
					
					this.DrawLine(line.X, line.Y, line.Color, this.FromPixels(view, line.Width), line.LineJoin);
				}
				else if(primitive is Polygon2D)
				{
					Polygon2D polygon = (Polygon2D)(primitive);
					
					this.DrawPolygon(polygon.X, polygon.Y, polygon.Color, polygon.Width, polygon.Fill);
				}
			}
		}

	    /// <summary>
        /// Returns the width of the current target.
        /// </summary>
        public abstract float Width { get; }

        /// <summary>
        /// Returns the height of the current target.
        /// </summary>
        public abstract float Height { get; }

		/// <summary>
		/// Returns the size in pixels.
		/// </summary>
		/// <returns>The pixels.</returns>
		/// <param name="view">View.</param>
		/// <param name="sizeInPixels">Size in pixels.</param>
		protected abstract float FromPixels(View2D view, float sizeInPixels);

		/// <summary>
		/// Transforms the target using the specified view.
		/// </summary>
		/// <param name="view">View.</param>
		protected abstract void Transform (View2D view);

        /// <summary>
        /// Draws the backcolor.
        /// </summary>
        /// <param name="backColor"></param>
	    protected abstract void DrawBackColor(int backColor);

		/// <summary>
		/// Draws a point on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		protected abstract void DrawPoint (float x, float y, int color, float size);

	    /// <summary>
	    /// Draws a line on the target. The coordinates given are scene coordinates.
	    /// </summary>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="lineJoin"></param>
	    protected abstract void DrawLine(float[] x, float[] y, int color, float width, LineJoin lineJoin);

		/// <summary>
		/// Draws a polygon on the target. The coordinates given are scene coordinates.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="fill">If set to <c>true</c> fill.</param>
		protected abstract void DrawPolygon (float[] x, float[] y, int color, float width, bool fill);
	}
}