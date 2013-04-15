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
using System.Threading;
using OsmSharp.Tools.Math.Units.Angle;

namespace OsmSharp.Osm.Renderer.Gdi.Layers
{
    public abstract class HeatLayerBoxed : HeatLayer
    {
        private int _zoom = 12;

        protected HeatLayerBoxed(int radius, byte intensity)
            : base(radius, intensity)
        {
            _coords = new List<GeoCoordinate>();

            this.InitializeTileLoading(1);
        }

        private List<GeoCoordinate> _coords;

        protected override List<OsmSharp.Tools.Math.Geo.GeoCoordinate> GetPoint(OsmSharp.Tools.Math.Geo.GeoCoordinateBox box)
        {
            // notify a new request.
            this.NotifyRequest(box);

            return this.GetLoadedPoints(box);
        }

        /// <summary>
        /// Issues the correct requests for this box.
        /// </summary>
        /// <param name="box"></param>
        private List<OsmSharp.Tools.Math.Geo.GeoCoordinate> GetLoadedPoints(GeoCoordinateBox box)
        {
            List<OsmSharp.Tools.Math.Geo.GeoCoordinate> list = new List<GeoCoordinate>();

            TileRange range = this.GetTileToLoadFor(box, _zoom);

            for (int x = range.XMin; x < range.XMax + 1; x++)
            {
                lock (_loaded_tiles)
                {
                    for (int y = range.YMin; y < range.YMax + 1; y++)
                    {
                        if (_loaded_tiles.ContainsKey(x)
                            && _loaded_tiles[x].ContainsKey(y))
                        {
                            list.AddRange(_loaded_tiles[x][y]);
                        }
                    }
                }
            }
            return list;
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
            _loaded_tiles = new Dictionary<int, Dictionary<int, List<GeoCoordinate>>>();

            _tmr = new Timer(new TimerCallback(Timer_Callback));
            _tmr.Change(10, 500);
        }

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

            List<GeoCoordinate> new_elements = this.GetExtraPoints(this.CreateBoxFor(x, y));
            
            //// raise the change event.
            //if (this.Changed != null)
            //{
            //    this.Invalidate();
            //    this.Changed(this);
            //}

            // remove from the current loading.
            lock (_tiles_to_load_stack)
            {
                _current_loading_objects.Remove(param_object);
                Dictionary<int, List<GeoCoordinate>> y_dic = null;
                if (!_loaded_tiles.TryGetValue(x, out y_dic))
                {
                    y_dic = new Dictionary<int, List<GeoCoordinate>>();
                    _loaded_tiles.Add(x, y_dic);
                }
                _loaded_tiles[x].Add(y, new_elements);
            }
        }

        protected abstract List<GeoCoordinate> GetExtraPoints(GeoCoordinateBox geoCoordinateBox);

        private GeoCoordinateBox CreateBoxFor(int x, int y)
        {
            // calculate the tiles bounding box and set its properties.
            GeoCoordinate top_left = TileToWorldPos(x, y, _zoom);
            GeoCoordinate bottom_right = TileToWorldPos(x + 1, y + 1, _zoom);

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

        /// <summary>
        /// Stack containing the tiles to load.
        /// </summary>
        private LimitedStack<ThreadParameterObject> _tiles_to_load_stack;

        /// <summary>
        /// Dictionary containing the tiles that are loaded.
        /// </summary>
        private Dictionary<int, Dictionary<int, List<GeoCoordinate>>> _loaded_tiles;

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
        /// Issues the correct requests for this box.
        /// </summary>
        /// <param name="box"></param>
        private void NotifyRequest(GeoCoordinateBox box)
        {
            TileRange range = this.GetTileToLoadFor(box, _zoom);

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
    }
}
