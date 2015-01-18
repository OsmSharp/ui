using System;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace OsmSharp.iOS.UI
{
	/// <summary>
	/// A wrapper for the CG context holding also it's size.
	/// </summary>
	public class CGContextWrapper
	{
		/// <summary>
		/// Holds the CGContext.
		/// </summary>
		private CGContext _context;

		/// <summary>
		/// Holds the size of the CG context.
		/// </summary>
		private RectangleF _rectangle;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.CGContextWrapper"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="rectangle">Rectangle.</param>
		public CGContextWrapper (CGContext context, RectangleF rectangle)
		{
			_rectangle = rectangle;
			_context = context;
		}

		/// <summary>
		/// Gets the CG context.
		/// </summary>
		/// <value>The CG context.</value>
		public CGContext CGContext {
			get{
				return _context;
			}
		}

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width.</value>
		public float Width {
			get{
				return _rectangle.Width;
			}
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		public float Height {
			get{
				return _rectangle.Height;
			}
		}
	}
}

