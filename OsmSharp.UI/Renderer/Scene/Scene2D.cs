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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;

namespace OsmSharp.UI.Renderer.Scene
{
	/// <summary>
	/// Contains all objects that need to be rendered.
	/// </summary>
    public abstract class Scene2D
	{
		/// <summary>
		/// Clear this instance.
		/// </summary>
        public abstract void Clear();

        /// <summary>
        /// Returns the number of objects in this scene.
        /// </summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>
        /// Gets/sets the backcolor of the scene.
        /// </summary>
        public int BackColor { get; set; }

        /// <summary>
        /// Returns true if this scene is readonly.
        /// </summary>
        public abstract bool IsReadOnly { get; }

	    /// <summary>
	    /// Gets all objects in this scene for the specified view sorted according to layer number.
	    /// </summary>
	    /// <param name="view">View.</param>
	    /// <param name="zoom"></param>
        public abstract IEnumerable<Scene2DPrimitive> Get(View2D view, float zoom);

        /// <summary>
        /// Returns the primitive with the given id if any.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract List<IScene2DPrimitive> Get(uint id);

        /// <summary>
        /// Removes all primitives associated with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool Remove(uint id);

	    /// <summary>
	    /// Adds a point.
	    /// </summary>
	    /// <param name="layer"></param>
	    /// <param name="maxZoom"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="size">Size.</param>
	    /// <param name="minZoom"></param>
        public abstract uint AddPoint(int layer, float minZoom, float maxZoom, double x, double y, int color, double size);

		/// <summary>
		/// Adds a point.
		/// </summary>
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="size">Size.</param>
		/// <param name="minZoom"></param>
		public virtual uint AddPoint(float minZoom, float maxZoom, double x, double y, int color, double size)
		{
			return this.AddPoint(0, minZoom, maxZoom, x, y, color, size);
		}

		/// <summary>
		/// Adds a line.
		/// </summary>
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
        /// <param name="minZoom"></param>
        public virtual uint AddLine(float minZoom, float maxZoom, double[] x, double[] y, int color, double width)
		{
			return this.AddLine(0, minZoom, maxZoom, x, y, color, width);
		}

		/// <summary>
		/// Adds a line.
		/// </summary>§
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="lineJoin"></param>
		/// <param name="dashes"></param>
        /// <param name="minZoom"></param>
        public virtual uint AddLine(float minZoom, float maxZoom, double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes)
		{
            return this.AddLine(0, minZoom, maxZoom, x, y, color, width, lineJoin, dashes);
		}

		/// <summary>
		/// Adds a line.
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
        /// <param name="minZoom"></param>
		/// <returns></returns>
        public virtual uint AddLine(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width)
		{
            return this.AddLine(layer, minZoom, maxZoom, x, y, color, width, LineJoin.None, null);
		}

	    /// <summary>
	    /// Adds a line.
	    /// </summary>
	    /// <param name="layer"></param>
	    /// <param name="maxZoom"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="lineJoin"></param>
	    /// <param name="dashes"></param>
        /// <param name="minZoom"></param>
        /// <param name="casingWidth"></param>
        /// <param name="casingColor"></param>
        public abstract uint AddLine(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width,
            LineJoin lineJoin, int[] dashes);

		/// <summary>
		/// Adds the polygon.
		/// </summary>
		/// <param name="maxZoom"></param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		/// <param name="width">Width.</param>
		/// <param name="fill">If set to <c>true</c> fill.</param>
		/// <param name="minZoom"></param>
		public virtual uint AddPolygon(float minZoom, float maxZoom, double[] x, double[] y, int color, double width, bool fill)
		{
			return this.AddPolygon(0, minZoom, maxZoom, x, y, color, width, fill);
		}

	    /// <summary>
	    /// Adds the polygon.
	    /// </summary>
	    /// <param name="layer"></param>
	    /// <param name="maxZoom"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="fill">If set to <c>true</c> fill.</param>
	    /// <param name="minZoom"></param>
        public abstract uint AddPolygon(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width, bool fill);

	    /// <summary>
	    /// Adds an icon.
	    /// </summary>
	    /// <param name="layer"></param>
	    /// <param name="maxZoom"></param>
	    /// <param name="x"></param>
	    /// <param name="y"></param>
	    /// <param name="iconImage"></param>
	    /// <param name="minZoom"></param>
	    /// <returns></returns>
        public abstract uint AddIcon(int layer, float minZoom, float maxZoom, double x, double y, byte[] iconImage);

	    /// <summary>
	    /// Adds an image.
	    /// </summary>
	    /// <param name="layer"></param>
	    /// <param name="minZoom"></param>
	    /// <param name="maxZoom"></param>
	    /// <param name="left"></param>
	    /// <param name="top"></param>
	    /// <param name="right"></param>
	    /// <param name="bottom"></param>
	    /// <param name="imageData"></param>
	    /// <returns></returns>
        public abstract uint AddImage(int layer, float minZoom, float maxZoom, double left, double top, double right, double bottom,
                             byte[] imageData);

		/// <summary>
		/// Adds the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="layer">Layer.</param>
		/// <param name="minZoom">Minimum zoom.</param>
		/// <param name="maxZoom">Max zoom.</param>
		/// <param name="left">Left.</param>
		/// <param name="top">Top.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		/// <param name="imageData">Image data.</param>
        public abstract uint AddImage(int layer, float minZoom, float maxZoom, double left, double top, double right, double bottom,
                             byte[] imageData, object tag);

		/// <summary>
		/// Adds the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="layer">Layer.</param>
		/// <param name="minZoom">Minimum zoom.</param>
		/// <param name="maxZoom">Max zoom.</param>
		/// <param name="rectangle">Rectangle.</param>
		/// <param name="imageData">Image data.</param>
		/// <param name="tag">Tag.</param>
		public abstract uint AddImage (int layer, float minZoom, float maxZoom, RectangleF2D rectangle, byte[] imageData, object tag);

	    /// <summary>
	    /// Adds texts.
	    /// </summary>
	    /// <param name="layer"></param>
	    /// <param name="maxZoom"></param>
	    /// <param name="x"></param>
	    /// <param name="y"></param>
	    /// <param name="size"></param>
	    /// <param name="text"></param>
	    /// <param name="minZoom"></param>
	    /// <returns></returns>
        public abstract uint AddText(int layer, float minZoom, float maxZoom, double x, double y, double size, string text, int color,
            int? haloColor, int? haloRadius, string font);

        /// <summary>
        /// Adds a text along a line to this scene.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="fontSize"></param>
        /// <param name="text"></param>
        public abstract uint AddTextLine(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, float fontSize,
            string text, int? haloColor, int? haloRadius);
        
        /// <summary>
        /// Serializes this scene2D to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compress"></param>
        public abstract void Serialize(Stream stream, bool compress);
    }
}