using System;
#if __UNIFIED__
using CoreGraphics;
#else
using System.Drawing;
using MonoTouch.CoreGraphics;

// Type Mappings Unified to monotouch.dll
using CGRect = global::System.Drawing.RectangleF;
using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;

using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
#endif

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
		private CGRect _rectangle;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.iOS.UI.CGContextWrapper"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="rectangle">Rectangle.</param>
		public CGContextWrapper (CGContext context, CGRect rectangle)
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
				return (float)_rectangle.Width;
			}
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		public float Height {
			get{
				return (float)_rectangle.Height;
			}
		}
	}
}

