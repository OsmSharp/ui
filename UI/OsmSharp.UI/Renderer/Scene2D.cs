
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Math.Primitives;
using OsmSharp.UI.Renderer.Scene2DPrimitives;
using OsmSharp.UI.Renderer.Scene2DPrimitives.Storage;

namespace OsmSharp.UI.Renderer
{
	/// <summary>
	/// Contains all objects that need to be rendered.
	/// </summary>
    public class Scene2D
	{
        /// <summary>
        /// Holds all primitives indexed per layer and by id.
        /// </summary>
        private readonly SortedDictionary<int, 
			Dictionary<uint, IScene2DPrimitive>> _primitives; 

        /// <summary>
        /// Holds the next primitive id.
        /// </summary>
	    private uint _nextId = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.UI.Renderer.Scene2D"/> class.
		/// </summary>
		public Scene2D()
		{
            _primitives = new SortedDictionary<int, Dictionary<uint, IScene2DPrimitive>>();

            this.BackColor = SimpleColor.FromArgb(0, 255, 255, 255).Value; // fully transparent.
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear ()
		{
			_primitives.Clear ();
		}

	    /// <summary>
	    /// Gets all objects in this scene for the specified view.
	    /// </summary>
	    /// <param name="view">View.</param>
	    /// <param name="zoom"></param>
	    public IEnumerable<IScene2DPrimitive> Get(View2D view, float zoom)
		{
			var primitivesInView = new List<IScene2DPrimitive>();
			
			lock(_primitives)
			{
			    foreach (var layer in _primitives)
			    { // loop over all layers in order.
                    foreach (KeyValuePair<uint, IScene2DPrimitive> primitivePair in layer.Value)
                    { // loop over all primitives in order.
                        if (primitivePair.Value.IsVisibleIn(view, zoom))
                        {
                            primitivesInView.Add(primitivePair.Value);
                        }
                    }
                    //var viewRectangle = new RectangleF2D(view.Left, view.Top, view.Right, view.Bottom);
                    //var primitivesInLayer = layer.Value.Get(viewRectangle);
                    //foreach (var scene2DPrimitive in primitivesInLayer)
                    //{			        
                    //    if (scene2DPrimitive.IsVisibleIn(view, zoom))
                    //    {
                    //        primitivesInView.Add(scene2DPrimitive);
                    //    }
                    //}
			    }
			}
		    return primitivesInView;
		}

        /// <summary>
        /// Gets/sets the backcolor of the scene.
        /// </summary>
	    public int BackColor { get; set; }

        /// <summary>
        /// Removes the primitive with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Remove(uint id)
        {
            foreach (var layer in _primitives)
            {
                if (layer.Value.Remove(id))
                {
                    return true;
                }
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
            Dictionary<uint, IScene2DPrimitive> layerDic;
            if (!_primitives.TryGetValue(layer, out layerDic))
            {
                layerDic = new Dictionary<uint, IScene2DPrimitive>();
                _primitives.Add(layer, layerDic);
            }
            layerDic.Add(id, primitive);
            //ISpatialIndex<IScene2DPrimitive> layer;
            //if (!_primitives.TryGetValue(layerIdx, out layer))
            //{
            //    layer = new RTreeMemoryIndex<IScene2DPrimitive>();
            //    _primitives.Add(layerIdx, layer);
            //}
            //layer.Add(primitive.GetBox(), primitive);
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
		public uint AddPoint(int layer, float minZoom, float maxZoom, double x, double y, int color, double size)
        {
            uint id = _nextId;
            _nextId++;
			
			lock(_primitives)
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
	    /// Adds a point.
	    /// </summary>
	    /// <param name="maxZoom"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="size">Size.</param>
	    /// <param name="minZoom"></param>
		public uint AddPoint(float minZoom, float maxZoom, double x, double y, int color, double size)
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
		public uint AddLine(float minZoom, float maxZoom, double[] x, double[] y, int color, double width)
        {
            return this.AddLine(0, minZoom, maxZoom, x, y, color, width);
        }

	    /// <summary>
	    /// Adds a line.
	    /// </summary>
	    /// <param name="maxZoom"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="lineJoin"></param>
	    /// <param name="dashes"></param>
	    /// <param name="minZoom"></param>
	    public uint AddLine(float minZoom, float maxZoom, double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes)
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
		public uint AddLine(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width)
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
		public uint AddLine(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width, LineJoin lineJoin, int[] dashes)
		{
			if (y == null)
				throw new ArgumentNullException ("y");
			if (x == null)
				throw new ArgumentNullException ("x");
			if(x.Length != y.Length)
				throw new ArgumentException("x and y arrays have different lenghts!");

            uint id = _nextId;
            _nextId++;

			lock(_primitives)
			{
                this.AddPrimitive(layer, id, new Line2D(x, y, color, width, lineJoin, dashes, minZoom, maxZoom));
			}
		    return id;
		}

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
		public uint AddPolygon(float minZoom, float maxZoom, double[] x, double[] y, int color, double width, bool fill)
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
		public uint AddPolygon(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double width, bool fill)
		{
			if (y == null)
				throw new ArgumentNullException ("y");
			if (x == null)
				throw new ArgumentNullException ("x");
			if (x.Length != y.Length)
				throw new ArgumentException("x and y arrays have different lenghts!");

			
			lock(_primitives)
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
		public uint AddIcon(int layer, float minZoom, float maxZoom, double x, double y, byte[] iconImage)
        {
            if (iconImage == null)
                throw new ArgumentNullException("iconImage");


			lock(_primitives)
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
		public uint AddImage(int layer, float minZoom, float maxZoom, double left, double top, double right, double bottom, 
		                     byte[] imageData)
        {
            if (imageData == null)
                throw new ArgumentNullException("imageData");


			lock(_primitives)
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
		/// <param name="left">Left.</param>
		/// <param name="top">Top.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		/// <param name="imageData">Image data.</param>
		public uint AddImage(int layer, float minZoom, float maxZoom, double left, double top, double right, double bottom, 
		                     byte[] imageData, object tag)
		{
			if (imageData == null)
				throw new ArgumentNullException("imageData");

			Image2D image = new Image2D (left, top, bottom, right, imageData, minZoom, maxZoom);
			image.Tag = tag;

			lock(_primitives)
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
		public uint AddText(int layer, float minZoom, float maxZoom, double x, double y, double size, string text, int color,
            int? haloColor, int? haloRadius)
        {
            if (text == null)
                throw new ArgumentNullException("text");

			lock(_primitives)
			{
				uint id = _nextId;
				_nextId++;

                this.AddPrimitive(layer, id, new Text2D(x, y, text, color, size, haloColor, haloRadius, minZoom, maxZoom));
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
        public uint AddTextLine(int layer, float minZoom, float maxZoom, double[] x, double[] y, int color, double font_size, 
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
	    public void Serialize(Stream stream, bool compress)
	    {
            // build the index.
            var index = new RTreeMemoryIndex<Scene2DEntry>();
            foreach (var primitiveLayer in _primitives)
            {
                foreach (var primitive in primitiveLayer.Value)
                {
                    index.Add(primitive.Value.GetBox(), new Scene2DEntry()
                                                            {
                                                                Layer = primitiveLayer.Key,
                                                                Id = primitive.Key,
                                                                Scene2DPrimitive = primitive.Value
                                                            });
                }
            }

            // create the serializer.
            var serializer = new Scene2DPrimitivesSerializer(compress);
            serializer.Serialize(stream, index);
	    }

	    /// <summary>
	    /// Deserialize a Scene2D from the given stream.
	    /// </summary>
	    /// <param name="stream"></param>
	    /// <param name="compressed"></param>
	    /// <returns></returns>
	    public static IScene2DPrimitivesSource Deserialize(Stream stream, bool compressed)
        {
            // create the serializer.
            var serializer = new Scene2DPrimitivesSerializer(compressed);
            ISpatialIndexReadonly<Scene2DEntry> index = serializer.Deserialize(stream);

            return new Scene2DPrimitivesSource(index);
        }

	    #endregion
    }
}