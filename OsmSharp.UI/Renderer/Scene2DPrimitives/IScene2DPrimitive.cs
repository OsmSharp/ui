
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Primitives;

namespace OsmSharp.UI.Renderer.Scene2DPrimitives
{
	/// <summary>
	/// Abstract representation of a Scene2D primitive.
	/// </summary>
	public interface IScene2DPrimitive
	{
	    /// <summary>
	    /// Returns true if the object is visible on the view.
	    /// </summary>
	    /// <returns><c>true</c> if this instance is visible in the specified view; otherwise, <c>false</c>.</returns>
	    /// <param name="view">View.</param>
	    /// <param name="zoom"></param>
	    bool IsVisibleIn(View2D view, float zoom);

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
	    RectangleF2D GetBox();

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		object Tag {
			get;
			set;
		}
	}
}