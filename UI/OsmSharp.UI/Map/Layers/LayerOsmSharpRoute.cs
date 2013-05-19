
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Routing.Route;
using OsmSharp.UI.Renderer;

namespace OsmSharp.UI.Map.Layers
{
    /// <summary>
    /// A layer drawing OsmSharpRoute layer data.
    /// </summary>
    public class LayerOsmSharpRoute : ILayer
    {
        /// <summary>
        /// Holds the projection.
        /// </summary>
        private readonly IProjection _projection;

        /// <summary>
        /// Creates a new OsmSharpRoute layer.
        /// </summary>
        /// <param name="projection"></param>
        public LayerOsmSharpRoute(IProjection projection)
        {
            _projection = projection;

            this.Scene = new Scene2D();
            this.Scene.BackColor = 
                SimpleColor.FromKnownColor(KnownColor.Transparent).Value;
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
            // all data is pre-loaded for now.

            // when displaying huge amounts of GPX-data use another approach.
        }

        /// <summary>
        /// Event raised when this layer's content has changed.
        /// </summary>
        public event Map.LayerChanged LayerChanged;

        #region Scene Building

        /// <summary>
        /// Adds a new OsmSharpRoute.
        /// </summary>
        /// <param name="route">Stream.</param>
        public void AddRoute(OsmSharpRoute route)
        {
            if (route.Entries != null && route.Entries.Length > 0)
            { // there are entries.
                // get x/y.
                var x = new double[route.Entries.Length];
                var y = new double[route.Entries.Length];
                for (int idx = 0; idx < route.Entries.Length; idx++)
                {
                    x[idx] = _projection.LongitudeToX(
                        route.Entries[idx].Longitude);
                    y[idx] = _projection.LatitudeToY(
                        route.Entries[idx].Latitude);
                }

                // set the default color if none is given.
                SimpleColor blue = SimpleColor.FromKnownColor(KnownColor.Blue);
                SimpleColor transparantBlue = SimpleColor.FromArgb(128,
                                                                   blue.R, blue.G, blue.B);

                this.Scene.AddLine(float.MinValue, float.MaxValue, x, y,
                                   transparantBlue.Value, 8);
            }
        }

        #endregion
    }
}