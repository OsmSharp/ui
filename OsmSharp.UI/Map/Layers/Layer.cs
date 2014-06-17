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

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// An abstract representation of a layer.
    /// </summary>
    public abstract class Layer
    {
        /// <summary>
        /// The minimum zoom.
        /// </summary>
        /// <remarks>
        /// The minimum zoom is the 'highest'.
        /// </remarks>
        public float? MinZoom { get; protected set; }

        /// <summary>
        /// The maximum zoom.
        /// </summary>
        /// <remarks>
        /// The maximum zoom is the 'lowest' or most detailed view.
        /// </remarks>
        public float? MaxZoom { get; protected set; }

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
                this.LayerChanged(this);
            }
        }

        /// <summary>
        /// An event raised when the content of this layer has changed.
        /// </summary>
        protected internal event OsmSharp.UI.Map.Map.LayerChanged LayerChanged;

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
        /// Closes the layer.
        /// </summary>
        /// <remarks>Stop with whatever the layer might be doing.</remarks>
        public virtual void Close()
        { // make sure no changes from this layer can still trigger rendering!
            this.LayerChanged = null;
        }
    }
}