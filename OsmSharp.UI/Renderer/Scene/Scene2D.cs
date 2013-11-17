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

using System.Collections.Generic;
using System.IO;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Scene.Scene2DPrimitives;

namespace OsmSharp.UI.Renderer.Scene
{
	/// <summary>
	/// Contains all objects that need to be rendered.
	/// </summary>
    public abstract class Scene2D : Scene2DReadonly
	{
        /// <summary>
        /// Returns true if this scene is readonly.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a point to the scene returning the id of the geometry.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract uint AddPoint(double x, double y);

        /// <summary>
        /// Adds a series of points to the scene returning the id of the geometry.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract uint AddPoints(double[] x, double[] y);

        /// <summary>
        /// Adds the given byte array as an image and returns and id.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract uint AddImage(byte[] data);

        #region Styles

        /// <summary>
        /// Adds a style to the given point.
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        public abstract uint AddStylePoint(uint pointId, uint layer, float minZoom, float maxZoom, int color, float size);

        /// <summary>
        /// Adds a style to the given point.
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        public virtual uint AddStylePoint(uint pointId, float minZoom, float maxZoom, int color, float size)
		{
            return this.AddStylePoint(0, minZoom, maxZoom, color, size);
		}

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
        public abstract uint AddIcon(uint pointId, uint layer, float minZoom, float maxZoom, uint imageId);

        /// <summary>
        /// Adds texts at the given location.
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="size"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="haloColor"></param>
        /// <param name="haloRadius"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public abstract uint AddText(uint pointId, uint layer, float minZoom, float maxZoom, float size, string text, int color,
            int? haloColor, int? haloRadius, string font);

        /// <summary>
        /// Adds the style properties to the given line with the given id.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public virtual uint AddStyleLine(uint pointsId, float minZoom, float maxZoom, int color, double width)
		{
            return this.AddStyleLine(0, minZoom, maxZoom, color, width);
		}

        /// <summary>
        /// Adds the style properties to the given line with the given id.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="lineJoin"></param>
        /// <param name="dashes"></param>
        /// <returns></returns>
        public virtual uint AddStyleLine(uint pointsId, float minZoom, float maxZoom, int color, double width, LineJoin lineJoin, int[] dashes)
		{
            return this.AddStyleLine(0, minZoom, maxZoom, color, width, lineJoin, dashes);
		}

        /// <summary>
        /// Adds the style properties to the given line with the given id.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public virtual uint AddStyleLine(uint pointsId, uint layer, float minZoom, float maxZoom, int color, float width)
		{
            return this.AddStyleLine(pointsId, layer, minZoom, maxZoom, color, width, LineJoin.None, null);
		}

        /// <summary>
        /// Adds the style properties to the given line with the given id.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="lineJoin"></param>
        /// <param name="dashes"></param>
        /// <returns></returns>
        public abstract uint AddStyleLine(uint pointsId, uint layer, float minZoom, float maxZoom, int color, float width,
            LineJoin lineJoin, int[] dashes);

        /// <summary>
        /// Adds the style properties to the given line with the given id.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="fontSize"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="haloColor"></param>
        /// <param name="haloRadius"></param>
        /// <returns></returns>
        public abstract uint AddStyleLine(uint pointsId, uint layer, float minZoom, float maxZoom, int color, float fontSize,
            string text, string font, int? haloColor, int? haloRadius);

        /// <summary>
        /// Adds the given style properties to the polygon with the given id.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fill"></param>
        /// <returns></returns>
        public virtual uint AddStylePolygon(uint pointsId, float minZoom, float maxZoom, int color, float width, bool fill)
		{
            return this.AddStylePolygon(pointsId, 0, minZoom, maxZoom, color, width, fill);
		}

        /// <summary>
        /// Adds the given style properties to the polygon with the given id.
        /// </summary>
        /// <param name="pointsId"></param>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="fill"></param>
        /// <returns></returns>
        public abstract uint AddStylePolygon(uint pointsId, uint layer, float minZoom, float maxZoom, int color, float width, bool fill);

        #endregion
        
        /// <summary>
        /// Serializes this scene2D to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compress"></param>
        public abstract void Serialize(Stream stream, bool compress);
    }
}