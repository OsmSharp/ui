using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing OSM data.
    /// </summary>
    public class OsmLayer : ILayer
    {
        /// <summary>
        /// Holds the source of the OSM raw data.
        /// </summary>
        private readonly IDataSourceReadOnly _dataSource;

        /// <summary>
        /// Holds the style interpreter.
        /// </summary>
        private readonly StyleInterpreter _styleInterpreter;

        /// <summary>
        /// Creates a new OSM data layer.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="styleInterpreter"></param>
        public OsmLayer(IDataSourceReadOnly dataSource, StyleInterpreter styleInterpreter)
        {
            _dataSource = dataSource;
            _styleInterpreter = styleInterpreter;

            this.Scene = new Scene2D();
            _interpretedObjects = new Dictionary<int, HashSet<long>>();
        }

        /// <summary>
        /// Gets the minimum zoom.
        /// </summary>
        public float? MinZoom { get; private set; }

        /// <summary>
        /// Gets the maximum zoom.
        /// </summary>
        public float? MaxZoom { get; private set; }

        /// <summary>
        /// Gets the scene of this layer containing the objects already projected.
        /// </summary>
        public Scene2D Scene { get; private set; }

        /// <summary>
        /// Called when the view on the map containing this layer has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        public void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
            this.BuildScene(map, zoomFactor, center, view);
        }

        /// <summary>
        /// Event raised when this layer's content has changed.
        /// </summary>
        public event Map.LayerChanged LayerChanged;


        #region Scene Building

        /// <summary>
        /// Holds al id's of all already interpreted objects.
        /// </summary>
        private readonly Dictionary<int,HashSet<long>> _interpretedObjects;

        /// <summary>
        /// Holds all previously requested boxes.
        /// </summary>
        private HashSet<GeoCoordinateBox> _requestedBoxes = new HashSet<GeoCoordinateBox>();
 

        /// <summary>
        /// Builds the scene.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        private void BuildScene(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
            // get the indexed object at this zoom.
            HashSet<long> interpretedObjects;
            if (!_interpretedObjects.TryGetValue((int) zoomFactor, out interpretedObjects))
            {
                interpretedObjects = new HashSet<long>();
                _interpretedObjects.Add((int)zoomFactor, interpretedObjects);
            }

            // build the boundingbox.
            var box = new GeoCoordinateBox(map.Projection.ToGeoCoordinates(view.Left, view.Top),
                map.Projection.ToGeoCoordinates(view.Right, view.Bottom));
            foreach (var requestedBox in _requestedBoxes)
            {
                if (requestedBox.IsInside(box))
                {
                    return;
                }
            }
            _requestedBoxes.Add(box);

            // set the scene backcolor.
            SimpleColor? color = _styleInterpreter.GetCanvasColor();
            this.Scene.BackColor = color.HasValue ? color.Value.Value : SimpleColor.FromArgb(0, 255, 255, 255).Value;

            // get data.
            foreach (var osmGeo in _dataSource.Get(box, null))
            { // translate each object into scene object.
                if (!interpretedObjects.Contains(osmGeo.Id))
                {
                    _styleInterpreter.Translate(this.Scene, map.Projection, zoomFactor, osmGeo);
                    interpretedObjects.Add(osmGeo.Id);
                }
            }
        }

        #endregion
    }
}