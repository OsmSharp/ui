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
using OsmSharp.UI.Renderer.Scene.Storage;

namespace OsmSharp.UI.Renderer.Scene
{
    /// <summary>
    /// Contains all objects that need to be rendered.
    /// </summary>
    public class Scene2DSimple : Scene2D
    {
        /// <summary>
        /// Holds all primitives indexed per layer and by id.
        /// </summary>
        private readonly SortedDictionary<int,
            List<IScene2DPrimitive>> _primitives;

        /// <summary>
        /// Holds the next primitive id.
        /// </summary>
        private uint _nextId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.Scene2DSimple"/> class.
        /// </summary>
        public Scene2DSimple()
        {
            _primitives = new SortedDictionary<int, List<IScene2DPrimitive>>();

            this.BackColor = SimpleColor.FromArgb(0, 255, 255, 255).Value; // fully transparent.
        }

        /// <summary>
        /// Clear this instance.
        /// </summary>
        public override void Clear()
        {
            _primitives.Clear();
        }

        /// <summary>
        /// Returns the number of objects in this scene.
        /// </summary>
        public override int Count
        {
            get
            {
                int count = 0;
                foreach (KeyValuePair<int, List<IScene2DPrimitive>> keyValuePair in _primitives)
                {
                    count = count + keyValuePair.Value.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// Returns the layer count.
        /// </summary>
        public int LayerCount
        {
            get
            {
                return _primitives.Count;
            }
        }

        /// <summary>
        /// Gets all objects in this scene for the specified view.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="zoom"></param>
        public override IEnumerable<Scene2DPrimitive> Get(View2D view, float zoom)
        {
            var primitivesInView = new List<Scene2DPrimitive>();

            lock (_primitives)
            {
                foreach (var layer in _primitives)
                { // loop over all layers in order.
                    foreach (IScene2DPrimitive primitivePair in layer.Value)
                    { // loop over all primitives in order.
                        if (primitivePair.IsVisibleIn(view, zoom))
                        {
                            primitivesInView.Add(new Scene2DPrimitive() 
                                { Layer = layer.Key, Primitive = primitivePair });
                        }
                    }
                }
            }
            return primitivesInView;
        }

        /// <summary>
        /// Returns the readonly flag.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the primitives with the given id if andy.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<IScene2DPrimitive> Get(uint id)
        {
            List<IScene2DPrimitive> primitives = new List<IScene2DPrimitive>();
            foreach (var layer in _primitives)
            {
                foreach (var primitive in layer.Value)
                {
                    if (primitive.Id == id)
                    {
                        primitives.Add(primitive);
                    }
                }
            }
            return primitives;
        }

        /// <summary>
        /// Removes the primitive with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool Remove(uint id)
        {
            List<IScene2DPrimitive> primitives = this.Get(id);
            bool removed = false;
            foreach (var layer in _primitives)
            {
                removed = removed || 
                    layer.Value.RemoveAll(x => primitives.Contains(x)) > 0;
            }
            return false;
        }

        /// <summary>
        /// Adds a new primitive with a given id and layer.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="id"></param>
        /// <param name="primitive"></param>
        internal void AddPrimitive(int layer, uint id, IScene2DPrimitive primitive)
        {
            primitive.Id = id;
            List<IScene2DPrimitive> layerList;
            if (!_primitives.TryGetValue(layer, out layerList))
            {
                layerList = new List<IScene2DPrimitive>();
                _primitives.Add(layer, layerList);
            }
            layerList.Add(primitive);
        }

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
        public override uint AddPoint(int layer, float minZoom, float maxZoom, double x, double y, int color, double size)
        {
            uint id = _nextId;
            _nextId++;

            lock (_primitives)
            {
                this.AddPrimitive(layer, id, new Point2D()
                {
                    Color = color,
                    X = x,
                    Y = y,
                    Size = size,
                    MinZoom = minZoom,
                    MaxZoom = maxZoom
                });
            }
            return id;
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
        public override uint AddLine(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width,
            LineJoin lineJoin, int[] dashes)
        {
            if (y == null)
                throw new ArgumentNullException("y");
            if (x == null)
                throw new ArgumentNullException("x");
            if (x.Length != y.Length)
                throw new ArgumentException("x and y arrays have different lenghts!");

            uint id = _nextId;
            _nextId++;

            lock (_primitives)
            {
                this.AddPrimitive(layer, id, new Line2D(x, y, color, width, lineJoin, dashes, minZoom, maxZoom));
            }
            return id;
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
        public override uint AddPolygon(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width, bool fill)
        {
            if (y == null)
                throw new ArgumentNullException("y");
            if (x == null)
                throw new ArgumentNullException("x");
            if (x.Length != y.Length)
                throw new ArgumentException("x and y arrays have different lenghts!");


            lock (_primitives)
            {
                uint id = _nextId;
                _nextId++;

                this.AddPrimitive(layer, id, new Polygon2D(x, y, color, width, fill, minZoom, maxZoom));
                return id;
            }
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
        public override uint AddIcon(int layer, float minZoom, float maxZoom, double x, double y, byte[] iconImage)
        {
            if (iconImage == null)
                throw new ArgumentNullException("iconImage");


            lock (_primitives)
            {
                uint id = _nextId;
                _nextId++;

                this.AddPrimitive(layer, id, new Icon2D(x, y, iconImage, minZoom, maxZoom));
                return id;
            }
        }

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
        public override uint AddImage(int layer, float minZoom, float maxZoom, double left, double top, double right, double bottom,
                             byte[] imageData)
        {
            if (imageData == null)
                throw new ArgumentNullException("imageData");


            lock (_primitives)
            {
                uint id = _nextId;
                _nextId++;

                this.AddPrimitive(layer, id, new Image2D(left, top, bottom, right, imageData, minZoom, maxZoom));
                return id;
            }
        }

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
		public override uint AddImage (int layer, float minZoom, float maxZoom, RectangleF2D rectangle, byte[] imageData, object tag)
		{
			if (imageData == null)
				throw new ArgumentNullException("imageData");


			lock (_primitives)
			{
				uint id = _nextId;
				_nextId++;

				var imageTilted2D = new ImageTilted2D (rectangle, imageData, minZoom, maxZoom);
				imageTilted2D.Tag = tag;
				this.AddPrimitive(layer, id, imageTilted2D);
				return id;
			}
		}

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
        public override uint AddImage(int layer, float minZoom, float maxZoom, double left, double top, double right, double bottom,
                             byte[] imageData, object tag)
        {
            if (imageData == null)
                throw new ArgumentNullException("imageData");

            Image2D image = new Image2D(left, top, bottom, right, imageData, minZoom, maxZoom);
            image.Tag = tag;

            lock (_primitives)
            {

                uint id = _nextId;
                _nextId++;

                this.AddPrimitive(layer, id, image);
                return id;
            }
        }

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
        public override uint AddText(int layer, float minZoom, float maxZoom, double x, double y, double size, string text, int color,
            int? haloColor, int? haloRadius, string font)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            lock (_primitives)
            {
                uint id = _nextId;
                _nextId++;

                this.AddPrimitive(layer, id, new Text2D(x, y, text, color, size, haloColor, haloRadius, font, minZoom, maxZoom));
                return id;
            }
        }

        /// <summary>
        /// Adds a text along a line to this scene.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <param name="font_size"></param>
        /// <param name="text"></param>
        public override uint AddTextLine(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, float font_size,
            string text, int? haloColor, int? haloRadius)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            lock (_primitives)
            {
                uint id = _nextId;
                _nextId++;

                this.AddPrimitive(layer, id, new LineText2D(x, y, color, font_size, text, haloColor, haloRadius, minZoom, maxZoom));
                return id;
            }
        }

        #region Serialization/Deserialization

        /// <summary>
        /// Serializes this scene2D to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compress"></param>
        public override void Serialize(Stream stream, bool compress)
        {
            this.SerializeStyled(stream, compress);
            //// build the index.
            //var index = new RTreeMemoryIndex<Scene2DEntry>();
            //foreach (var primitiveLayer in _primitives)
            //{
            //    foreach (var primitive in primitiveLayer.Value)
            //    {
            //        index.Add(primitive.GetBox(), new Scene2DEntry()
            //        {
            //            Layer = primitiveLayer.Key,
            //            Id = 0,
            //            Scene2DPrimitive = primitive
            //        });
            //    }
            //}

            //// create the serializer.
            //var serializer = new OsmSharp.UI.Renderer.Scene.Storage.Styled.Scene2DRTreeSerializer(compress);
            //serializer.Serialize(stream, index);
        }

        /// <summary>
        /// Deserialize a Scene2D from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static IScene2DPrimitivesSource Deserialize(Stream stream, bool compressed)
        {
            return Scene2DSimple.DeserializeStyled(stream, compressed);
            //// create the serializer.
            //var serializer = new OsmSharp.UI.Renderer.Scene.Storage.Styled.Scene2DRTreeSerializer(compressed);
            //ISpatialIndexReadonly<Scene2DEntry> index = serializer.Deserialize(stream);

            //return new Scene2DPrimitivesSource(index);
        }

        #endregion

        #region Serialization/Deserialization

        /// <summary>
        /// Serializes this scene2D to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compress"></param>
        public void SerializeStyled(Stream stream, bool compress)
        {
            // build the index.
            var index = new RTreeMemoryIndex<Scene2DEntry>();
            foreach (var primitiveLayer in _primitives)
            {
                foreach (var primitive in primitiveLayer.Value)
                {
                    index.Add(primitive.GetBox(), new Scene2DEntry()
                    {
                        Layer = primitiveLayer.Key,
                        Id = 0,
                        Scene2DPrimitive = primitive
                    });
                }
            }

            // create the serializer.
            var serializer = new OsmSharp.UI.Renderer.Scene.Storage.Styled.Scene2DStyledSerializer(compress);
            serializer.Serialize(stream, index);
        }

        /// <summary>
        /// Deserialize a Scene2D from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static IScene2DPrimitivesSource DeserializeStyled(Stream stream, bool compressed)
        {
            // create the serializer.
            var serializer = new OsmSharp.UI.Renderer.Scene.Storage.Styled.Scene2DStyledSerializer(compressed);
            return serializer.Deserialize(stream);
        }

        #endregion
    }
}