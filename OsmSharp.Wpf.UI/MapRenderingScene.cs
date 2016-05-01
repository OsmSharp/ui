using System.Windows.Media.Imaging;
using OsmSharp.Math.Geo;
using OsmSharp.Units.Angle;

namespace OsmSharp.Wpf.UI
{
    public class MapRenderingScene
    {
        public MapRenderingScene(GeoCoordinate center, double zoom, Degree mapTilt)
        {
            Center = center;
            Zoom = zoom;
            Tilt = mapTilt;
        }

        public GeoCoordinate Center { get; }
        public double Zoom { get; }
        public Degree Tilt { get; }

        public MapRenderingScene PreviousScene { get; set; }

        public BitmapSource SceneImage { get; set; }
    }
}