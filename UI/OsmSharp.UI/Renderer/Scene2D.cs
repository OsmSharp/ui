
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
        /// Holds all primitives indexed per layer and by id.
        /// </summary>
	    private readonly SortedDictionary<int, Dictionary<uint, IScene2DPrimitive>>  _primitives; 

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
	    /// Gets all objects in this scene for the specified view.
	    /// </summary>
	    /// <param name="view">View.</param>
	    /// <param name="zoom"></param>
	    internal IEnumerable<IScene2DPrimitive> Get(View2D view, float zoom)
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
	    /// Adds a point.
	    /// </summary>
	    /// <param name="layer"></param>
	    /// <param name="maxZoom"></param>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="size">Size.</param>
	    /// <param name="minZoom"></param>
	    public uint AddPoint(int layer, float minZoom, float maxZoom, float x, float y, int color, float size)
        {
            uint id = _nextId;
            _nextId++;
			
			lock(_primitives)
			{
		        Dictionary<uint, IScene2DPrimitive> layerDic;
	            if (!_primitives.TryGetValue(layer, out layerDic))
	            {
	                layerDic = new Dictionary<uint, IScene2DPrimitive>();
	                _primitives.Add(layer, layerDic);
	            }
	            layerDic.Add(id, new Point2D()
	            {
	                Color = color,
	                X = x,
	                Y = y,
	                Size = size
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
	    public uint AddPoint(float minZoom, float maxZoom, float x, float y, int color, float size)
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
	    public uint AddLine(float minZoom, float maxZoom, float[] x, float[] y, int color, float width)
        {
            return this.AddLine(0, minZoom, maxZoom, x, y, color, width);
        }

	    /// <summary>
	    /// Adds a line.
	    /// </summary>
	    /// <param name="x">The x coordinate.</param>
	    /// <param name="y">The y coordinate.</param>
	    /// <param name="color">Color.</param>
	    /// <param name="width">Width.</param>
	    /// <param name="lineJoin"></param>
	    /// <param name="dashes"></param>
	    public uint AddLine(float minZoom, float maxZoom, float[] x, float[] y, int color, float width, LineJoin lineJoin, int[] dashes)
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
	    public uint AddLine(int layer, float minZoom, float maxZoom, float[] x, float[] y, int color, float width)
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
	    public uint AddLine(int layer, float minZoom, float maxZoom, float[] x, float[] y, int color, float width, LineJoin lineJoin, int[] dashes)
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
	            Dictionary<uint, IScene2DPrimitive> layerDic;
	            if (!_primitives.TryGetValue(layer, out layerDic))
	            {
	                layerDic = new Dictionary<uint, IScene2DPrimitive>();
	                _primitives.Add(layer, layerDic);
	            }
		        layerDic.Add(id, new Line2D(x, y, color, width, lineJoin, dashes, minZoom, maxZoom));
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
	    public uint AddPolygon(float minZoom, float maxZoom, float[] x, float[] y, int color, float width, bool fill)
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
	    public uint AddPolygon(int layer, float minZoom, float maxZoom, float[] x, float[] y, int color, float width, bool fill)
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
	            Dictionary<uint, IScene2DPrimitive> layerDic;
	            if (!_primitives.TryGetValue(layer, out layerDic))
	            {
	                layerDic = new Dictionary<uint, IScene2DPrimitive>();
	                _primitives.Add(layer, layerDic);
	            }
				layerDic.Add(id, new Polygon2D(x, y, color, width, fill, minZoom, maxZoom));
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
	    public uint AddIcon(int layer, float minZoom, float maxZoom, float x, float y, byte[] iconImage)
        {
            if (iconImage == null)
                throw new ArgumentNullException("iconImage");


			lock(_primitives)
			{
				uint id = _nextId;
				_nextId++;
	            Dictionary<uint, IScene2DPrimitive> layerDic;
	            if (!_primitives.TryGetValue(layer, out layerDic))
	            {
	                layerDic = new Dictionary<uint, IScene2DPrimitive>();
	                _primitives.Add(layer, layerDic);
	            }
				layerDic.Add(id, new Icon2D(x, y, iconImage, minZoom, maxZoom));
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
	    public uint AddImage(int layer, float minZoom, float maxZoom, float left, float top, float right, float bottom, byte[] imageData)
        {
            if (imageData == null)
                throw new ArgumentNullException("imageData");


			lock(_primitives)
			{
				uint id = _nextId;
				_nextId++;

	            Dictionary<uint, IScene2DPrimitive> layerDic;
	            if (!_primitives.TryGetValue(layer, out layerDic))
	            {
	                layerDic = new Dictionary<uint, IScene2DPrimitive>();
	                _primitives.Add(layer, layerDic);
	            }
				layerDic.Add(id, new Image2D(left, top, bottom, right, imageData, minZoom, maxZoom));
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
	    public uint AddText(int layer, float minZoom, float maxZoom, float x, float y, float size, string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

			lock(_primitives)
			{
				uint id = _nextId;
				_nextId++;

	            Dictionary<uint, IScene2DPrimitive> layerDic;
	            if (!_primitives.TryGetValue(layer, out layerDic))
	            {
	                layerDic = new Dictionary<uint, IScene2DPrimitive>();
	                _primitives.Add(layer, layerDic);
	            }
				layerDic.Add(id, new Text2D(x, y, text, size, minZoom, maxZoom));
				return id;
			}
        }
    }
}