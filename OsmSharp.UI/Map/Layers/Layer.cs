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

using OsmSharp.Math.Geo;
using OsmSharp.UI.Renderer;
using OsmSharp.UI.Renderer.Scene;
using OsmSharp.UI.Renderer.Primitives;
using System.Collections.Generic;
using OsmSharp.Math.Geo.Projections;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// An abstract representation of a layer.
    /// </summary>
    public abstract class Layer
    {
        /// <summary>
        /// Creates a new layer.
        /// </summary>
        public Layer()
        {
            this.IsVisible = true;
        }

        /// <summary>
        /// The minimum zoom.
        /// </summary>
        /// <remarks>
        /// The minimum zoom is the 'highest'.
        /// </remarks>
        public virtual float? MinZoom { get; set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        /// <remarks>
        /// The maximum zoom is the 'lowest' or most detailed view.
        /// </remarks>
        public virtual float? MaxZoom { get; set; }

        /// <summary>
        /// Returns true if this layer is visible for the given project and relative zoom factor.
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="zoomFactor"></param>
        /// <returns></returns>
        public bool IsLayerVisibleFor(IProjection projection, float zoomFactor)
        {
            return this.IsLayerVisibleFor((float)projection.ToZoomLevel(zoomFactor));
        }

        /// <summary>
        /// Returns true if this layer is visible for the given zoom level.
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        public bool IsLayerVisibleFor(float zoomLevel)
        {
            return this.IsVisible &&
                    (!this.MinZoom.HasValue || this.MinZoom < zoomLevel) &&
                    (!this.MaxZoom.HasValue || this.MaxZoom >= zoomLevel);
        }

        /// <summary>
        /// Returns all primitives from this layer that exist for the given zoom factor and inside the given view.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal abstract IEnumerable<Primitive2D> Get(float zoomFactor, View2D view);

        /// <summary>
        /// Called when the view on the map has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <param name="extraView"></param>
        protected internal virtual void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            // override in layer implementation if needed.
        }

        /// <summary>
        /// Called when the last map view change has to be cancelled.
        /// </summary>
        protected internal virtual void ViewChangedCancel()
        {
            // override in layer implementation to cancel large map requests.
        }

        /// <summary>
        /// Raises the layer changed event.
        /// </summary>
        protected void RaiseLayerChanged()
        {
            if (this.LayerChanged != null)
            {
				OsmSharp.Logging.Log.TraceEvent("Layer.RaiseLayerChanged (Before)", Logging.TraceEventType.Information, 
					"RaiseLayerChanged");
				this.LayerChanged(this);
				OsmSharp.Logging.Log.TraceEvent("Layer.RaiseLayerChanged (After)", Logging.TraceEventType.Information, 
					"RaiseLayerChanged");
            }
        }

        /// <summary>
        /// An event raised when the content of this layer has changed.
        /// </summary>
        protected internal event OsmSharp.UI.Map.Map.LayerChanged LayerChanged;

        /// <summary>
        /// Holds the visible flag.
        /// </summary>
        private bool _isVisible;

        /// <summary>
        /// Gets or sets the visible flag.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if(_isVisible != value)
                { // trigger a changed event after.
                    _isVisible = value;

                    this.RaiseLayerChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Pauses all activities in this layer.
        /// </summary>
        public virtual void Pause()
        {

        }

        /// <summary>
        /// Returns true if this layer is paused.
        /// </summary>
        public virtual bool IsPaused
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Resumes all activities in this layer.
        /// </summary>
        public virtual void Resume()
        {

        }

        /// <summary>
        /// Returns the bounding rectangle of this layer (if available).
        /// </summary>
        /// <remarks>Not all layers, formats support getting an envolope. Property can return null even on some types of bounded data.</remarks>
        public virtual GeoCoordinateBox Envelope
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Holds the backcolor.
        /// </summary>
        private int? _backcolor;

        /// <summary>
        /// Gets or sets the backcolor of this layer.
        /// </summary>
        public virtual int? BackColor
        {
            get
            {
                return _backcolor;
            }
            set
            {
                _backcolor = value;
            }
        }

        /// <summary>
        /// Closes the layer.
        /// </summary>
        /// <remarks>Stop with whatever the layer might be doing.</remarks>
        public virtual void Close()
        { // make sure no changes from this layer can still trigger rendering!
            this.LayerChanged = null;
        }
    }
}