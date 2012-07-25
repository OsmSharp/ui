using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Renderer.Gdi.Layers;

namespace Demo.RandomHeatMap
{
    class DemoHeatLayer : HeatLayerBoxed
    {

        public DemoHeatLayer()
            :base(30, 75)
        {

        }

        protected override List<GeoCoordinate> GetExtraPoints(GeoCoordinateBox geoCoordinateBox)
        {
            List<GeoCoordinate> coords = new List<GeoCoordinate>();

            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                coords.Add(geoCoordinateBox.GenerateRandomIn(rand));
            }
            return coords;
        }
    }
}
