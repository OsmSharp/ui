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
using System.IO;
using System.Configuration;
using OsmSharp.Osm.Map.Elements;
using OsmSharp.Tools.Collections;
using OsmSharp.Tools.Math.Units.Angle;
using OsmSharp.Tools.Math.Geo;
using System.Net;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace OsmSharp.Osm.Map.Layers.Tiles
{

    /// <summary>
    /// Layer containing tiles from OsmSharp.Osm.
    /// </summary>
    public class TilesLayer : ILayer
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
        /// The minimum zoom factor.
        /// </summary>
        private int _min_zoom;

        /// <summary>
        /// The zoom offset.
        /// </summary>
        private float _zoom_offset = 0;

        /// <summary>
        /// The maximum zoom factor.
        /// </summary>
        private int _max_zoom;

        /// <summary>
        /// Is this layer lazy.
        /// </summary>
        private bool _is_lazy;

        /// <summary>
        /// Stack containing the tiles to load.
        /// </summary>
        private LimitedStack<ThreadParameterObject> _tiles_to_load_stack;

        /// <summary>
        /// The url to get the tiles from.
        /// </summary>
        private string _tiles_url;

        /// <summary>
        /// The transparency color.
        /// </summary>
        private Color? _transparency_color;
        
        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="is_lazy"></param>
        public TilesLayer(bool is_lazy, float zoom_offset)
            :this(is_lazy,zoom_offset,null)
        {

        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        /// <param name="is_lazy"></param>
        public TilesLayer(bool is_lazy, float zoom_offset,TimeSpan? max_tile_age)
        {
            this.Visible = true;
            this.Name = "Tiles Layer";

            _tiles_url = ConfigurationManager.AppSettings["OsmTilesUrl"];

            _valid = false;
            _is_lazy = is_lazy;
            _zoom_offset = zoom_offset;

            // holds all tiles already loaded.
            _tiles_cache = new Dictionary<int, IDictionary<int, IDictionary<int, IElement>>>();

            // initialize the stack to hold unloaded tile requests.
            _tiles_to_load_stack = new LimitedStack<ThreadParameterObject>();
            _tiles_to_load_stack.Limit = 20;

            // initialize the async tile loading.
            this.InitializeTileLoading(5);
            
            // set the min zoom.
            _min_zoom = 1;
            // set the max zoom.
            _max_zoom = 18;

            this.MinZoom = _min_zoom;
            this.MaxZoom = _max_zoom;
        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        public TilesLayer(TimeSpan? max_tile_age)
            : this(true, 0, max_tile_age)
        {

        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        public TilesLayer()
            : this(true, 0)
        {

        }


        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        public TilesLayer(float zoom_offset)
            : this(true, zoom_offset)
        {

        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        public TilesLayer(string tiles_url)
            : this(true, 0)
        {
            _tiles_url = tiles_url;
            _transparency_color = Color.FromArgb(255, 255, 255, 254);
        }

        /// <summary>
        /// Creates a new tiles layer.
        /// </summary>
        public TilesLayer(string tiles_url, float zoom_offset)
            : this(true, zoom_offset)
        {
            _tiles_url = tiles_url;
        }

        protected string TilesUrl
        {
            get
            {
                return _tiles_url;
            }
        }

        #region ILayer Members

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

        public IList<ILayer> Layers
        {
            get
            {
                return new List<ILayer>();
            }
        }

        public IList<IElement> GetElements(GeoCoordinateBox box, double zoom_factor)
        {
            zoom_factor = zoom_factor + _zoom_offset;
            if (zoom_factor > _max_zoom)
            {
                zoom_factor = _max_zoom;
            }
            else if (zoom_factor < _min_zoom)
            {
                zoom_factor = _min_zoom;
            }

            return this.GetTiles(box, zoom_factor);
        }

        public int ElementCount
        {
            get
            {
                return -1;
            }
        }

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
            throw new NotSupportedException();
        }       
        
        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(ElementDot first, GeoCoordinate dot, bool create_dot)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds a line to this layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public ElementLine AddLine(ElementLine line, GeoCoordinate dot, bool create_dot)
        {
            throw new NotSupportedException();
        }
        
        /// <summary>
        /// Remove this element from the layer.
        /// </summary>
        /// <param name="dot"></param>
        /// <returns></returns>
        public void RemoveElement(IElement element)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds an element to the layer.
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(IElement element)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Tile Calculations

        /// <summary>
        /// The cached tiles.
        /// </summary>
        private IDictionary<int, IDictionary<int, IDictionary<int, IElement>>> _tiles_cache;

        /// <summary>
        /// Returns all the tile filling the given bounding box.
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="zoom_factor"></param>
        /// <returns></returns>
        public IList<IElement> GetTiles(GeoCoordinateBox bbox,
            double zoom_factor)
        {
            int zoom = (int)System.Math.Floor(zoom_factor);

            TileRange range = this.GetTileToLoadFor(bbox, zoom);
            IList<IElement> return_list = new List<IElement>();

            double tile_size = 360.0f / (double)(System.Math.Pow(2, zoom));

            for (int x = range.XMin; x < range.XMax + 1; x++)
            {
                for (int y = range.YMin; y < range.YMax + 1; y++)
                {
                    IElement new_tile = this.GetTile(x, y, zoom);
                    if (new_tile != null)
                    {
                        return_list.Add(new_tile);
                    }
                }
            }
            return return_list;
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
        /// Returns a tile at a given position and zoom.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public IElement GetTile(int x, int y, int zoom)
        {
            // add the dictionary for this zoom level if needed.
            IDictionary<int, IDictionary<int, IElement>> tiles_per_zoom = null;

            lock(_tiles_cache)
            {
                if (!_tiles_cache.TryGetValue(zoom, out tiles_per_zoom))
                {
                    tiles_per_zoom = new Dictionary<int, IDictionary<int, IElement>>();
                    _tiles_cache.Add(zoom, tiles_per_zoom);
                }

                // add the dictionary for the x.
                IDictionary<int, IElement> requests_per_x = null;
                bool contains = false;

                if (!tiles_per_zoom.TryGetValue(x, out requests_per_x))
                {
                    requests_per_x = new Dictionary<int, IElement>();
                    tiles_per_zoom.Add(x, requests_per_x);
                }
                contains = requests_per_x.ContainsKey(y) &&
                    requests_per_x[y] != null;

                if (!contains)
                {
                    if (_is_lazy)
                    {
                        //requests_per_x.Add(y, null);

                        // start download thread.
                        this.NotifyRequest(x, y, zoom);
                        return null;
                    }
                    else
                    { // load the tile in sync.
                        ElementImage tile = this.LoadMissingTile(x, y, zoom);

                        // if the tile was found add it to the cache.
                        _tiles_cache[zoom][x].Add(y, tile);

                    }
                }
                return _tiles_cache[zoom][x][y];
            }
        }

        /// <summary>
        /// Converts the tile position to longitude and latitude coordinates.
        /// </summary>
        /// <param name="tile_x"></param>
        /// <param name="tile_y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        protected GeoCoordinate TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            double n = System.Math.PI - ((2.0 * System.Math.PI * tile_y) / System.Math.Pow(2.0, zoom));

            double longitude = (double)((tile_x / System.Math.Pow(2.0, zoom) * 360.0) - 180.0);
            double latitude = (double)(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

            return new GeoCoordinate(latitude,longitude);
        }

        #endregion

        #region Tile Caching
        
        /// <summary>
        /// Loads a tile that is no in the cache.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private ElementImage LoadMissingTile(int x, int y, int zoom)
        {
            ElementImage tile = null;
            //if (!_maximum_tile_file_age.HasValue)
            //{ // caching is turned off; there is no maximum file age.
                tile = this.LoadMissingTileFromServer(zoom, x, y);
            //}
            //else
            //{ // caching is turned on; there is a maximum file age.
            //    // check if the tile exists on file.
            //    if (!_cache_dir.Exists)
            //    {
            //        _cache_dir.Create();
            //    }

            //    // create the file name.
            //    FileInfo file = new FileInfo(_cache_dir.FullName + string.Format(@"\Osm_{0}_{1}_{2}.png", zoom, x, y));
            //    if (file.Exists && (DateTime.Now - file.LastWriteTime) > _maximum_tile_file_age)
            //    { // file too old.
            //        file.Delete();
            //        file = new FileInfo(_cache_dir.FullName + string.Format(@"\Osm_{0}_{1}_{2}.png", zoom, x, y));
            //    }
            //    // file not too old or does not exist.
            //    if (!file.Exists)
            //    {
            //        try
            //        {
            //            if (x < 0 || y < 0)
            //            {

            //            }
            //            else
            //            {
            //                tile = this.LoadMissingTileFromServer(zoom, x, y);

            //                // save the file to the cache.
            //                //tile.Image.Save(file.FullName, ImageFormat.Png);
            //            }
            //        }
            //        catch (WebException)
            //        {

            //        }
            //        file = null;
            //    }
            //    else
            //    {
            //        try
            //        {
            //            // load the bitmap
            //            Bitmap img = new Bitmap(file.OpenRead());

            //            // calculate the tiles bounding box and set its properties.
            //            GeoCoordinate top_left = TileToWorldPos(x, y, zoom);
            //            GeoCoordinate bottom_right = TileToWorldPos(x + 1, y + 1, zoom);

            //            // assign it to the tile.
            //            tile = new ElementImage(top_left, bottom_right, img);
            //        }
            //        catch (Exception)
            //        {

            //        }
            //    }
            //}

            return tile;
        }

        protected virtual ElementImage LoadMissingTileFromServer(int zoom, int x, int y)
        {
            try
            {
                // calculate the tiles bounding box and set its properties.
                GeoCoordinate top_left = TileToWorldPos(x, y, zoom);
                GeoCoordinate bottom_right = TileToWorldPos(x + 1, y + 1, zoom);

                string url = string.Format(_tiles_url,
                    zoom,
                    x,
                    y);

                // get file from tile server.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                    url);
                request.Timeout = 5000;
                //Debug.WriteLine(url);

                WebResponse myResp = request.GetResponse();

                Stream stream = myResp.GetResponseStream();
                Image img = Bitmap.FromStream(stream);

                if (_transparency_color != null)
                {
                    Bitmap transparent_map = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    Graphics transparent_map_graphics = Graphics.FromImage(transparent_map);
                    ImageAttributes imageAttributes = new ImageAttributes();
                    //substitute blue for white to achieve the expected aim
                    ColorMap colorMap = new ColorMap();
                    colorMap.OldColor = _transparency_color.Value;
                    colorMap.NewColor = Color.Transparent;
                    //set up color remapping table
                    ColorMap[] remapTable = { colorMap };
                    //set up the color info for the image
                    imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
                    transparent_map_graphics.DrawImage(img,
                        new Rectangle(0, 0, transparent_map.Width, transparent_map.Height), //target rectangle
                        0, 0, // upper left corner of the source image
                        transparent_map.Width, // width of the source image
                        transparent_map.Height, // height of the source image
                        GraphicsUnit.Pixel,
                        //color info
                        imageAttributes);
                    img = transparent_map;
                }

                ElementImage tile = new ElementImage(top_left, bottom_right, img);

                stream.Close();
                stream.Dispose();

                return tile;
            }
            catch (WebException)
            {

            }
            catch (ArgumentException)
            {

            }
            return null;
        }

        #endregion

        #region Tile Loading

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

            _current_loading_objects = new List<ThreadParameterObject>();

            _tmr = new Timer(new TimerCallback(Timer_Callback));
            _tmr.Change(10, 50);
        }

        /// <summary>
        /// Objects used a parameter to start a new thread.
        /// </summary>
        private class ThreadParameterObject
        {
            public int x { get; set; }
            public int y { get; set; }
            public int zoom { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is ThreadParameterObject)
                {
                    ThreadParameterObject other = obj as ThreadParameterObject;
                    return other.x == this.x
                        && other.y == this.y
                        && other.zoom == this.zoom;
                }
                return false;
            }

            public override int GetHashCode()
            {
                int hash = 23;
                hash = hash * 37 + x;
                hash = hash * 37 + y;
                hash = hash * 37 + zoom;
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
        /// Adds a new request to the stack if it is new.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        private void NotifyRequest(int x, int y, int zoom)
        {
            lock (_tiles_to_load_stack)
            {
                ThreadParameterObject new_tile_to_load =
                    new ThreadParameterObject()
                    {
                        x = x,
                        y = y,
                        zoom = zoom
                    };
                if (!_tiles_to_load_stack.Contains(new_tile_to_load) && !_current_loading_objects.Contains(new_tile_to_load))
                {
                    _tiles_to_load_stack.PushToTop(new_tile_to_load);
                    //Console.WriteLine("New request {0}_{1}@{2} STACK {3}",
                    //    x, y, zoom, _tiles_to_load_stack.Count);
                }
            }
        }

        /// <summary>
        /// Load a missing tile based on the ThreadPoolParameterObject.
        /// </summary>
        /// <param name="param"></param>
        private void DoLoadMissingTile(object param)
        {
            // load the missing tile.
            ThreadParameterObject param_object = param as ThreadParameterObject;
            int zoom = param_object.zoom;
            int x = param_object.x;
            int y = param_object.y;

            // initialize the tile.
            ElementImage tile = null;
            try
            { // try to load the missing tiles.
                tile = this.LoadMissingTile(x, y, zoom);
            }
            catch
            {

            }
            finally
            { // if the tile was found add it to the cache.
                if (tile != null)
                {
                    lock (_tiles_cache)
                    {
                        _tiles_cache[zoom][x][y] = tile;
                    }

                    // raise the change event.
                    if (this.Changed != null)
                    {
                        this.Invalidate();
                        this.Changed(this);
                    }
                }

                // remove from the current loading.
                lock (_tiles_to_load_stack)
                {
                    _current_loading_objects.Remove(param_object);
                }
            }
        }

        #endregion
    }
}
