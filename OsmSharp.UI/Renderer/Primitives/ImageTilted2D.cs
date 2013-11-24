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

using OsmSharp.Math.Primitives;
namespace OsmSharp.UI.Renderer.Primitives
{
    /// <summary>
    /// Represents an image that is defined by any rectangle.
    /// </summary>
    public class ImageTilted2D : Primitive2D
    {
        /// <summary>
        /// Holds the bounds.
        /// </summary>
        private RectangleF2D _bounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.Scene.Scene2DPrimitives.TiltedImage2D"/> class.
        /// </summary>
        /// <param name="bounds">Bounds.</param>
        /// <param name="imageData">Image data.</param>
        public ImageTilted2D(RectangleF2D bounds, byte[] imageData, float minZoom, float maxZoom)
        {
            _bounds = bounds;
            this.ImageData = imageData;

            this.MinZoom = minZoom;
            this.MaxZoom = maxZoom;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The identifier.</value>
        public uint Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the image data.
        /// </summary>
        public byte[] ImageData { get; set; }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public RectangleF2D Bounds
        {
            get
            {
                return _bounds;
            }
        }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        public float MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        public float MaxZoom { get; set; }

        #region IScene2DPrimitive implementation

        /// <summary>
        /// Returns true if the object is visible on the view.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="view">View.</param>
        /// <param name="zoom"></param>
        public override bool IsVisibleIn(View2D view, float zoom)
        { // TODO: refine this visible check.
            return _bounds.Overlaps(view.OuterBox);
        }

        /// <summary>
        /// Returns the bounding box for this primitive.
        /// </summary>
        /// <returns></returns>
        public override BoxF2D GetBox()
        {
            return _bounds.BoundingBox;
        }

        #endregion
    }
}