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
using System.Linq;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing scene objects.
    /// </summary>
    public class LayerScene : Layer
    {
        /// <summary>
        /// Holds the scene primitives source.
        /// </summary>
        private IPrimitives2DSource _index;

        /// <summary>
        /// Holds the primitives.
        /// </summary>
        private IEnumerable<Primitive2D> _primitives;

        /// <summary>
        /// Creates a new scene layer.
        /// </summary>
        /// <param name="index"></param>
        public LayerScene(IPrimitives2DSource index)
        {
            _primitives = new List<Primitive2D>();
            _index = index;
        }

        /// <summary>
        /// Called when the view on the map containing this layer has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        /// <param name="extraView"></param>
        protected internal override void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view, View2D extraView)
        {
            this.BuildScene(map, zoomFactor, center, extraView);
        }

        /// <summary>
        /// Called when the last map view change has to be cancelled.
        /// </summary>
        protected internal override void ViewChangedCancel()
        { // cancel the current get if there is one going on.
            _index.GetCancel();
        }

        /// <summary>
        /// Returns the backcolor.
        /// </summary>
        public override int? BackColor
        {
            get
            {
                if (base.BackColor.HasValue)
                { // backcolor overridden.
                    return base.BackColor;
                }
                return _index.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        #region Scene Building

        /// <summary>
        /// Holds the last box.
        /// </summary>
        private GeoCoordinateBox _lastBox;

        /// <summary>
        /// Holds the last zoom level.
        /// </summary>
        private int _lastZoom;

        /// <summary>
        /// Builds the scene.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        private void BuildScene(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
            // build the boundingbox.
            var viewBox = view.OuterBox;
            var box = new GeoCoordinateBox(map.Projection.ToGeoCoordinates(viewBox.Min[0], viewBox.Min[1]),
                          map.Projection.ToGeoCoordinates(viewBox.Max[0], viewBox.Max[1]));
            var zoomLevel = (int)map.Projection.ToZoomLevel(zoomFactor);
            if (_lastBox != null && _lastBox.Contains(box) &&
                zoomLevel == _lastZoom)
            {
                return;
            }
            _lastBox = box;
            _lastZoom = zoomLevel;

            lock (_index)
            {
                _primitives = _index.Get(view, zoomFactor);
            }
        }

        /// <summary>
        /// Returns all objects in this scene that are visible for the given parameters.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        protected internal override IEnumerable<Primitive2D> Get(float zoomFactor, View2D view)
        {
            return _primitives.Where((primitive) =>
            {
                return primitive.IsVisibleIn(view, zoomFactor);
            });
        }

        #endregion

        /// <summary>
        /// Closes this layer.
        /// </summary>
        public override void Close()
        {
            base.Close();

            // nothing to stop, even better!
        }
    }
}