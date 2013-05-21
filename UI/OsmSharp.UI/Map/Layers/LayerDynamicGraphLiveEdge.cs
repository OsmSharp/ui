using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing OSM data.
    /// </summary>
    public class LayerDynamicGraphLiveEdge : ILayer
    {
        /// <summary>
        /// Holds the source of the OSM raw data.
        /// </summary>
        private readonly IBasicRouterDataSource<LiveEdge> _dataSource;

        /// <summary>
        /// Holds the style interpreter.
        /// </summary>
        private readonly StyleInterpreter _styleInterpreter;

        /// <summary>
        /// Creates a new OSM data layer.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="styleInterpreter"></param>
        public LayerDynamicGraphLiveEdge(IBasicRouterDataSource<LiveEdge> dataSource, 
            StyleInterpreter styleInterpreter)
        {
            _dataSource = dataSource;
            _styleInterpreter = styleInterpreter;

            this.Scene = new Scene2D();
            _interpretedObjects = new Dictionary<int, HashSet<ArcId>>();
            this.Cache = false;
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
        /// Gets or sets the cache flag.
        /// </summary>
        public bool Cache { get; set; }

        /// <summary>
        /// Called when the view on the map containing this layer has changed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="center"></param>
        /// <param name="view"></param>
        public void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view)
        {
            if (!this.Cache)
            {
                _interpretedObjects.Clear();
                _requestedBoxes.Clear();
            }
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
        private readonly Dictionary<int, HashSet<ArcId>> _interpretedObjects;

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
                HashSet<ArcId> interpretedObjects;
                if (!_interpretedObjects.TryGetValue((int) zoomFactor, out interpretedObjects))
                {
                    interpretedObjects = new HashSet<ArcId>();
                    _interpretedObjects.Add((int) zoomFactor, interpretedObjects);
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
                this.Scene.BackColor = color.HasValue
                                           ? color.Value.Value
                                           : SimpleColor.FromArgb(0, 255, 255, 255).Value;

            // get data.
            foreach (var arc in _dataSource.GetArcs(box))
            {
                // translate each object into scene object.
                var arcId = new ArcId()
                                {
                                    Vertex1 = arc.Key,
                                    Vertex2 = arc.Value.Key
                                };
                if (!interpretedObjects.Contains(arcId))
                {
                    interpretedObjects.Add(arcId);

                    // create nodes.
                    float latitude, longitude;
                    _dataSource.GetVertex(arcId.Vertex1, out latitude, out longitude);
                    var node1 = Node.Create(arcId.Vertex1);
                    node1.Coordinate = new GeoCoordinate(latitude, longitude);
                    _dataSource.GetVertex(arcId.Vertex2, out latitude, out longitude);
                    var node2 = Node.Create(arcId.Vertex2);
                    node2.Coordinate = new GeoCoordinate(latitude, longitude);

                    // create way.
                    var way = Way.Create(-1);
                    if (arc.Value.Value.Forward)
                    {
                        way.Nodes.Add(node1);
                        way.Nodes.Add(node2);
                    }
                    else
                    {
                        way.Nodes.Add(node2);
                        way.Nodes.Add(node1);
                    }
                    way.Tags.AddOrReplace(_dataSource.TagsIndex.Get(arc.Value.Value.Tags));

                    _styleInterpreter.Translate(this.Scene, map.Projection, zoomFactor, way);
                    interpretedObjects.Add(arcId);
                }
            }
        }

        private struct ArcId
        {
            public uint Vertex1 { get; set; }

            public uint Vertex2 { get; set; }

            public override int GetHashCode()
            {
                return (this.Vertex1 + this.Vertex2).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is ArcId)
                {
                    var other = (ArcId)(obj);
                    return (other.Vertex1 == this.Vertex2 &&
                            other.Vertex2 == this.Vertex1) ||
                           (other.Vertex1 == this.Vertex1 &&
                            other.Vertex2 == this.Vertex2);
                }
                return false;
            }
        }

        #endregion
    }
}
