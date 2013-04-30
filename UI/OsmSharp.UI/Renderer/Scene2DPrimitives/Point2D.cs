
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives
{	
	/// <summary>
	/// A simple 2D point.
	/// </summary>
	internal struct Point2D : IScene2DPrimitive
	{
		/// <summary>
		/// Gets or sets the x.
		/// </summary>
		/// <value>The x.</value>
		public float X {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
		public float Y {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public int Color {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>The size.</value>
		public float Size {
			get;
			set;
		}

		#region IScene2DPrimitive implementation

		/// <summary>
		/// Returns true if the object is visible on the view.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="view">View.</param>
		public bool IsVisibleIn (View2D view)
		{
			return view.Contains(this.X, this.Y);
		}

		#endregion
	}
}

