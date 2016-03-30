// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;
using OsmSharp.UI.Map.Layers.Tiles;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Primitives;
using System;
using System.Collections.Generic;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A vector tile layer.
    /// </summary>
    public class LayerTile : Layer, IDisposable
    {
        private readonly int _minZoomLevel = 0; // Holds the minimum zoom level.
        private readonly int _maxZoomLevel = 18; // Holds the maximum zoom level.
        private readonly int _maxOverzoom; // Holds the maximum overzoom.
        private const float _zoomMinOffset = 0.5f; // Holds the offset to calculate the minimum zoom.
        private readonly ITileSource _source; // the tile source.

        /// <summary>
        /// Creates a new vector tiles layer.
        /// </summary>
        public LayerTile(ITileSource source, int maxOverzoom)
        {
            _source = source;
            _maxOverzoom = maxOverzoom;

            _source.SourceChanged += _source_SourceChanged;
        }

        private void _source_SourceChanged()
        {
            this.RaiseLayerChanged();
        }

        /// <summary>
        /// Returns all primitives from this layer visible for the given parameters.
        /// </summary>
        protected internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            var primitives = new OsmSharp.Collections.SortedSet<Primitive2D>(LayerComparer.GetInstance());
            if (_source.IsPaused)
            { // just return an empty primitives list if suspended.
                return primitives;
            }
            try
            {
                // calculate the current zoom level.
                var zoomLevel = (int)System.Math.Round(_source.Projection.ToZoomLevel(zoomFactor), 0);

                if (zoomLevel > _maxOverzoom)
                {
                    zoomLevel = _maxOverzoom;
                }

                if (zoomLevel >= _minZoomLevel && zoomLevel <= _maxZoomLevel)
                {
                    // build the bounding box.
                    var viewBox = view.OuterBox;
                    var box = new GeoCoordinateBox(_source.Projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                                  _source.Projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));

                    var tileRange = TileRange.CreateAroundBoundingBox(box, zoomLevel);

                    Image2D image;
                    foreach (var tile in tileRange)
                    {
                        if (_source.TryGet(tile, out image))
                        {
                            primitives.Add(image);
                        }
                    }
                }
                return primitives;
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Error, ex.Message);
            }
            return primitives;
        }

        #region IComparer implementation

        /// <summary>
        /// Layer comparer to sort objects by layer.
        /// </summary>
        private class LayerComparer : IComparer<Primitive2D>
        {
            private static LayerComparer _instance = null;

            public static LayerComparer GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new LayerComparer();
                }
                return _instance;
            }

            public int Compare(Primitive2D x, Primitive2D y)
            {
                if (x.Layer == y.Layer)
                { // objects with same layer, assume different.
                    return -1;
                }
                return x.Layer.CompareTo(y.Layer);
            }
        }

        #endregion

        /// <summary>
        /// Notifies this layer the mapview has changed.
        /// </summary>
        protected internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            if (_source.IsPaused)
            { // do not accept trigger changes if suspended.
                return;
            }

            if (!this.IsVisible)
            { // if the map is not visible also do not accept changes.
                return;
            }

            try
            {
                // calculate the current zoom level.
                var zoomLevel = (int)System.Math.Round(map.Projection.ToZoomLevel(zoomFactor), 0);

                if (zoomLevel > _maxOverzoom)
                {
                    zoomLevel = _maxOverzoom;
                }

                if (zoomLevel >= _minZoomLevel && zoomLevel <= _maxZoomLevel)
                {
                    // build the bounding box.
                    var viewBox = view.OuterBox;
                    var box = new GeoCoordinateBox(map.Projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                                  map.Projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));

                    var tileRange = TileRange.CreateAroundBoundingBox(box, zoomLevel);
                    _source.Prepare(tileRange);
                }
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Error, ex.Message);
            }
        }

        #region Disposing-pattern

        /// <summary>
        /// Diposes of all resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Diposes of all resources associated with this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                //someone want the deterministic release of all resources
                //Let us release all the managed resources
            }
            else
            {
                // Do nothing, no one asked a dispose, the object went out of
                // scope and finalized is called so lets next round of GC 
                // release these resources
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~LayerTile()
        {
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Pauses all activity in this layer.
        /// </summary>
        public override void Pause()
        {
            _source.Pause();
        }

        /// <summary>
        /// Returns true if this flag is paused.
        /// </summary>
        public override bool IsPaused
        {
            get
            {
                return _source.IsPaused;
            }
        }

        /// <summary>
        /// Resumes the activity in this layer.
        /// </summary>
        public override void Resume()
        {
            _source.Resume();
        }

        /// <summary>
        /// Closes this layer.
        /// </summary>
        public override void Close()
        {
            base.Close();

            try
            {
                _source.Close();
            }
            catch (Exception ex)
            { // don't worry about exceptions here.
                OsmSharp.Logging.Log.TraceEvent("LayerVectorTiles", Logging.TraceEventType.Error, ex.Message);
            }
        }
    }
}
