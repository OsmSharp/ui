
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OsmSharp.UI.Renderer.Scene2DPrimitives;

namespace OsmSharp.UI.Renderer
{
	/// <summary>
	/// Contains all objects that need to be rendered.
	/// </summary>
	public class Scene2D
	{
		/// <summary>
		/// Holds a list of primitives.
		/// </summary>
		private List<IScene2DPrimitive> _primitives;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.Scene2D"/> class.
		/// </summary>
		public Scene2D()
		{
			_primitives = new List<IScene2DPrimitive>();
		}

		/// <summary>
		/// Gets all objects in this scene for the specified view.
		/// </summary>
		/// <param name="view">View.</param>
		internal IEnumerable<IScene2DPrimitive> Get(View2D view)
		{
			List<IScene2DPrimitive> primitivesInView = new List<IScene2DPrimitive>();
			foreach(IScene2DPrimitive primitive in _primitives)
			{
				if(primitive.IsVisibleIn(view))
				{
					primitivesInView.Add(primitive);
				}
			}
			return primitivesInView;
		}

		/// <summary>
		/// Adds a point.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		public void AddPoint(float x, float y, int color, float size)
		{
			_primitives.Add(new Point2D(){
				Color = color,
				X = x,
				Y = y,
				Size = size
			});
		}

		/// <summary>
		/// Adds a line.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		public void AddLine (float[] x, float[] y, int color, float width)
		{
			if (y == null)
				throw new ArgumentNullException ("y");
			if (x == null)
				throw new ArgumentNullException ("x");
			if(x.Length != y.Length)
				throw new ArgumentException("x and y arrays have different lenghts!");

			_primitives.Add(new Line2D(){
				Color = color,
				X = x,
				Y = y,
				Width = width
			});
		}
		/// <summary>
		/// Adds the polygon.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="fill">If set to <c>true</c> fill.</param>
		public void AddPolygon (float[] x, float[] y, int color, float width, bool fill)
		{
			if (y == null)
				throw new ArgumentNullException ("y");
			if (x == null)
				throw new ArgumentNullException ("x");
			if (x.Length != y.Length)
				throw new ArgumentException("x and y arrays have different lenghts!");

			
			_primitives.Add(new Polygon2D(){
				Color = color,
				X = x,
				Y = y,
				Width = width,
				Fill = fill
			});
		}
	}
}