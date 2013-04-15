// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.Linq;
using System.Text;
using OsmSharp.Tools.Collections;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Map.Elements;
using System.Threading;
using OsmSharp.Tools.Math.Shapes;
using OsmSharp.Tools.Math.Geo.Factory;
using System.Drawing;
using OsmSharp.Tools.Math.Units.Angle;

namespace OsmSharp.Osm.Map.Layers.Custom
{
    public abstract class CustomAbstractBoxedLayer : ILayer
    {
        /// <summary>
        /// The event raised when data or elements in this layer changed.
        /// </summary>
        public event Map.LayerChangedDelegate Changed;

        /// <summary>
        /// The unique id for this layer.
        /// </summary>
        private Guid _guid;

        /// <summary>
        /// Boolean containing the valid state.
        /// </summary>
        private bool _valid;

        /// <summary>
        /// The elements in this custom layer.
        /// </summary>
        private IList<IElement> _elements;

        /// <summary>
        /// The zoom factor to use for caching.
        /// </summary>
        private int _zoom_factor;

        /// <summary>
        /// Creates a new custom layer.
        /// </summary>
        public CustomAbstractBoxedLayer(int zoom)
        {
            this.Visible = true;
            this.Name = "Custom Boxed Layer";

            _guid = Guid.NewGuid();
            _elements = new List<IElement>();
            _valid = false;

            _zoom_factor = zoom;

            this.MinZoom = -1;
            this.MaxZoom = -1;

            this.InitializeTileLoading(1);
        }


        #region ILayer Members

        /// <summary>
        /// Gets/sets the visible flag of this layer.
        /// </summary>
        public bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the name of this layer.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        public Guid Id
        {
            get
            {
                return _guid;
            }
        }

        /// <summary>
        /// Invalidates this layer.
        /// </summary>
        public void Invalidate()
        {
            _valid = false;
        }

        /// <summary>
        /// Validates this layer.
        /// </summary>
        public void Validate()
        {
            _valid = true;
        }

        /// <summary>
        /// Gets the valid flag.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return _valid;
        }

        /// <summary>
        /// Returns the child layers.
        /// </summary>
        public IList<ILayer> Layers
        {
            get
            {
                return new List<ILayer>();
            }
        }


        /// <summary>
        /// Returns the elements.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="zoom_factor"></param>
        /// <returns></returns>
        public IList<IElement> GetElements(GeoCoordinateBox box, double zoom_factor)
        {
            // notify a new request.
            this.NotifyRequest(box);

            // return the existing elements.
            return this.GetExistingElements(box, zoom_factor);
        }

        /// <summary>
        /// Returns the elements.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="zoom_factor"></param>
        /// <returns></returns>
        public abstract IList<IElement> GetExtraElements(GeoCoordinateBox box, double zoom_factor);

        /// <summary>
        /// Returns the existing elements.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="zoom_factor"></param>
        /// <returns></returns>
        protected IList<IElement> GetExistingElements(GeoCoordinateBox box, double zoom_factor)
        {
            List<IElement> elements = new List<IElement>();
            List<IElement> local_elements;
            lock (_elements)
            {
                local_elements = new List<IElement>(_elements);
            }

            foreach (IElement element in local_elements)
            {
                if (element.IsVisibleIn(box))
                {
                    elements.Add(element);
                }
            }

            return elements;
        }

        /// <summary>
        /// Returns the number of elements.
        /// </summary>
        public int ElementCount
        {
            get
            {
                return -1;
            }
        }
        
        /// <summary>
        /// The minimum zoom for this layer.
        /// </summary>
        public int MinZoom
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum zoom for this layer.
        /// </summary>
        public int MaxZoom
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a dot to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementDot AddDot(GeoCoordinate dot)
        {
            ElementDot element = new ElementDot(
                Color.Black.ToArgb(),
                0.0002f,
                new OsmSharp.Tools.Math.Shapes.ShapeDotF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(PrimitiveGeoFactory.Instance, dot),
                false);

            lock (_elements)
            {
                _elements.Add(element);
            }
            return element;
        }

        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(ElementDot first, GeoCoordinate dot, bool create_dot)
        {
            // add new dot.
            if (create_dot)
            {
                this.AddDot(dot);
            }

            // create polyline.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            coordinates.Add(first.Dot.Point);
            coordinates.Add(dot);
            ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line
                = new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                    PrimitiveGeoFactory.Instance,
                    coordinates.ToArray());

            // create the line element.
            ElementLine element = new ElementLine(
                line,
                Color.Black.ToArgb(),
                0.0002f,
                true);

            lock (_elements)
            {
                _elements.Add(element);
            }

            return element;
        }

        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(
            GeoCoordinate dot1,
            GeoCoordinate dot2,
            bool create_dot,
            double width,
            bool width_fixed,
            int color)
        {
            // add new dot.
            if (create_dot)
            {
                this.AddDot(dot1);
                this.AddDot(dot2);
            }

            // create polyline.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            coordinates.Add(dot1);
            coordinates.Add(dot2);
            ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line
                = new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                    PrimitiveGeoFactory.Instance,
                    coordinates.ToArray());

            // create the line element.
            ElementLine element = new ElementLine(
                line,
                color,
                width,
                width_fixed);

            lock (_elements)
            {
                _elements.Add(element);
            }

            return element;
        }

        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(ElementLine line, GeoCoordinate dot, bool create_dot)
        {
            lock (_elements)
            {
                // remove the old line.
                _elements.Remove(line);
            }

            // add new dot.
            if (create_dot)
            {
                this.AddDot(dot);
            }

            // create polyline.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            coordinates.AddRange(line.Line.Points);
            coordinates.Add(dot);

            ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> polyline
                = new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                    PrimitiveGeoFactory.Instance,
                    coordinates.ToArray());

            // create the line element.
            ElementLine element = new ElementLine(
                polyline,
                Color.Black.ToArgb(),
                0.0002f,
                true);

            lock (_elements)
            {
                _elements.Add(element);
            }

            return element;
        }


        /// <summary>
        /// Remove this element from the layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public void RemoveElement(IElement element)
        {
            lock (_elements)
            {
                _elements.Remove(element);
            }
        }

        /// <summary>
        /// Removes all elements from this layer.
        /// </summary>
        public void Clear()
        {
            lock (_elements)
            {
                _elements.Clear();
            }
        }

        /// <summary>
        /// Adds an element to the layer.
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(IElement element)
        {
            lock (_elements)
            {
                _elements.Add(element);
            }
        }

        public ElementImage AddImage(
            Image image,
            GeoCoordinate coordinate)
        {
            ElementImage image_element =
                new ElementImage(coordinate, image);
            lock (_elements)
            {
                _elements.Add(image_element);
            }

            return image_element;
        }

        #endregion


        #region Tile Loading

        /// <summary>
        /// Stack containing the tiles to load.
        /// </summary>
        private LimitedStack<ThreadParameterObject> _tiles_to_load_stack;

        /// <summary>
        /// Dictionary containing the tiles that are loaded.
        /// </summary>
        private Dictionary<int, Dictionary<int,bool>> _loaded_tiles;

        /// <summary>
        /// Timer used to tigger the tile loading.
        /// </summary>
        private Timer _tmr;

        /// <summary>
        /// Initialize the tile loading functionality.
        /// </summary>
        private void InitializeTileLoading(int max_threads)
        {
            _max_loading_thread_count = max_threads;

            _tiles_to_load_stack = new LimitedStack<ThreadParameterObject>();

            _current_loading_objects = new List<ThreadParameterObject>();
            _loaded_tiles = new Dictionary<int, Dictionary<int, bool>>();

            _tmr = new Timer(new TimerCallback(Timer_Callback));
            _tmr.Change(10, 500);
        }

        /// <summary>
        /// Objects used a parameter to start a new thread.
        /// </summary>
        private class ThreadParameterObject
        {
            public int x { get; set; }
            public int y { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is ThreadParameterObject)
                {
                    ThreadParameterObject other = obj as ThreadParameterObject;
                    return other.x == this.x
                        && other.y == this.y;
                }
                return false;
            }

            public override int GetHashCode()
            {
                int hash = 23;
                hash = hash * 37 + x;
                hash = hash * 37 + y;
                return hash;
            }
        }

        /// <summary>
        /// The maximum amount of thread used for loading.
        /// </summary>
        private int _max_loading_thread_count;

        /// <summary>
        /// The current thread-count.
        /// </summary>
        private IList<ThreadParameterObject> _current_loading_objects;

        /// <summary>
        /// Timer callback used to trigger the tile loading.
        /// </summary>
        private void Timer_Callback(object sender)
        {
            lock (_tiles_to_load_stack)
            {
                if (_tiles_to_load_stack.Count > 0 && _current_loading_objects.Count < _max_loading_thread_count)
                {
                    ParameterizedThreadStart start = new ParameterizedThreadStart(DoLoadMissingTile);
                    Thread thr = new Thread(start);
                    ThreadParameterObject current = _tiles_to_load_stack.Pop();
                    _current_loading_objects.Add(current);
                    thr.Start(current);
                }
            }
        }

        /// <summary>
        /// Issues the correct requests for this box.
        /// </summary>
        /// <param name="box"></param>
        private void NotifyRequest(GeoCoordinateBox box)
        {
            TileRange range = this.GetTileToLoadFor(box, _zoom_factor);

            for (int x = range.XMin; x < range.XMax + 1; x++)
            {
                for (int y = range.YMin; y < range.YMax + 1; y++)
                {
                    this.NotifyRequest(x, y);
                }
            }
        }

        /// <summary>
        /// Adds a new request to the stack if it is new.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        private void NotifyRequest(int x, int y)
        {
            lock (_tiles_to_load_stack)
            {
                // if the tile was loaded before don't do it again!
                if (_loaded_tiles.ContainsKey(x) && _loaded_tiles[x].ContainsKey(y))
                {
                    return;
                }

                // notify a new tile if it is not already on the stack.
                ThreadParameterObject new_tile_to_load =
                    new ThreadParameterObject()
                    {
                        x = x,
                        y = y
                    };

                if (!_tiles_to_load_stack.Contains(new_tile_to_load) && !_current_loading_objects.Contains(new_tile_to_load))
                {
                    _tiles_to_load_stack.PushToTop(new_tile_to_load);
                    Console.WriteLine("New request {0}_{1} STACK {2}",
                        x, y, _tiles_to_load_stack.Count);
                }
            }
        }

        public TileRange GetTileToLoadFor(GeoCoordinateBox bbox,
            int zoom)
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, zoom));

            Radian rad = new Degree(bbox.MaxLat);

            int x_tile_min = (int)(((bbox.MinLon + 180.0f) / 360.0f) * (double)n);
            int y_tile_min = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);


            rad = new Degree(bbox.MinLat);
            int x_tile_max = (int)(((bbox.MaxLon + 180.0f) / 360.0f) * (double)n);
            int y_tile_max = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            TileRange range = new TileRange();
            range.XMax = x_tile_max;
            range.XMin = x_tile_min;
            range.YMax = y_tile_max;
            range.YMin = y_tile_min;
            return range;
        }

        public class TileRange
        {
            public int XMin { get; set; }

            public int YMin { get; set; }

            public int XMax { get; set; }

            public int YMax { get; set; }
        }


        /// <summary>
        /// Load a missing tile based on the ThreadPoolParameterObject.
        /// </summary>
        /// <param name="param"></param>
        private void DoLoadMissingTile(object param)
        {
            // load the missing tile.
            ThreadParameterObject param_object = param as ThreadParameterObject;
            int x = param_object.x;
            int y = param_object.y;

            Console.WriteLine("Loading {0}_{1} STACK {2}",
                x, y, _tiles_to_load_stack.Count);

            IList<IElement> new_elements = this.GetExtraElements(this.CreateBoxFor(x, y), _zoom_factor);

            // get the extra elements
            foreach (IElement element in new_elements)
            {
                this.AddElement(element);
            }

            // raise the change event.
            if (this.Changed != null)
            {
                this.Invalidate();
                this.Changed(this);
            }

            // remove from the current loading.
            lock (_tiles_to_load_stack)
            {
                _current_loading_objects.Remove(param_object);
                Dictionary<int, bool> y_dic = null;
                if (!_loaded_tiles.TryGetValue(x,out y_dic))
                {
                    y_dic = new Dictionary<int, bool>();
                    _loaded_tiles.Add(x, y_dic);
                }
                _loaded_tiles[x].Add(y, true);
            }            
        }

        private GeoCoordinateBox CreateBoxFor(int x, int y)
        {
            // calculate the tiles bounding box and set its properties.
            GeoCoordinate top_left = TileToWorldPos(x, y, _zoom_factor);
            GeoCoordinate bottom_right = TileToWorldPos(x + 1, y + 1, _zoom_factor);

            return new GeoCoordinateBox(top_left, bottom_right);
        }

        /// <summary>
        /// Converts the tile position to longitude and latitude coordinates.
        /// </summary>
        /// <param name="tile_x"></param>
        /// <param name="tile_y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private GeoCoordinate TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            double n = System.Math.PI - ((2.0 * System.Math.PI * tile_y) / System.Math.Pow(2.0, zoom));

            double longitude = (double)((tile_x / System.Math.Pow(2.0, zoom) * 360.0) - 180.0);
            double latitude = (double)(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

            return new GeoCoordinate(latitude, longitude);
        }

        #endregion
    }
}
