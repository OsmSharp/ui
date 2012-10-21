// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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
using Tools.Math.Geo;
using System.Drawing;
using Tools.Math.Graph.Routing;
//using Osm.Core;
using Tools.Math.Graph;
using Tools.Math.Shapes;
using Osm.Map.Layers.Custom;
using Osm.Map.Elements;

namespace Osm.Routing.Core.Route.Map
{
    public class SpacialRouteLayer : CustomLayer
    {
        public static int DEFAULT_ROUTE_WIDTH = 7;

        public GeoCoordinateBox AddRoute(OsmSharpRoute route, Color? color)
        {
            return this.AddRoute(route, color, SpacialRouteLayer.DEFAULT_ROUTE_WIDTH);
        }

        public GeoCoordinateBox AddRoute(OsmSharpRoute route, Color? color, int width)
        {
            if (route != null)
            {
                Color route_color;
                if (color != null && color.HasValue)
                {
                    route_color = color.Value;
                }
                else
                {
                    route_color = this.RandomColor();
                }

                List<GeoCoordinate> coordinates = route.GetPoints();
                if (coordinates != null && coordinates.Count > 0)
                {
                    double distance_arrow = 300;

                    this.DoForRoute(coordinates, distance_arrow, route_color, width, 16, 18);
                    this.DoForRoute(coordinates, distance_arrow * 4, route_color, width - 1, 14, 16);
                    this.DoForRoute(coordinates, distance_arrow * 8, route_color, width - 2, 12, 14);
                    this.DoForRoute(coordinates, distance_arrow * 16, route_color, width - 2, 10, 12);
                    return this.DoForRoute(coordinates, distance_arrow * 32, route_color, width - 2, 0, 10);
                    //return this.DoForRoute(coordinates, distance_arrow * 16, route_color, width - 2, 0, 10);
                }
            }

            return null;
        }

        private GeoCoordinateBox DoForRoute(List<GeoCoordinate> route, double distance_arrow, Color route_color, int width, int min_zoom, int max_zoom)
        {
            double distance = 0;

            GeoCoordinate previous_point = route[0];

            List<GeoCoordinate> points = new List<GeoCoordinate>();
            points.Add(previous_point);
            for(int idx = 1;idx < route.Count;idx++)
            {
                GeoCoordinate current_point = route[idx];
                points.Add(current_point);

                // calculate distance.
                distance = distance + previous_point.DistanceReal(
                        current_point).Value;
                
                if (distance > distance_arrow)
                {
                    ElementLine line = new ElementLine(
                        new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                            Tools.Math.Geo.Factory.PrimitiveGeoFactory.Instance,
                            points.ToArray<GeoCoordinate>()),
                            route_color.ToArgb(),
                            width, true);
                    line.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    line.MinZoom = min_zoom;
                    line.MaxZoom = max_zoom;
                    this.AddElement(line);
                    
                    GeoCoordinate last_point = points[points.Count - 1];
                    points.Clear();
                    points.Add(last_point);
                    distance = 0;
                }

                previous_point = current_point;
            }
            if (points.Count > 0)
            {
                ElementLine line = new ElementLine(
                    new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                        Tools.Math.Geo.Factory.PrimitiveGeoFactory.Instance,
                        points.ToArray<GeoCoordinate>()),
                        route_color.ToArgb(),
                        width, true);
                line.MinZoom = min_zoom;
                line.MaxZoom = max_zoom;
                line.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                this.AddElement(line);
            }
            return new GeoCoordinateBox(route);
        }

        public void AddCustomRoute(List<GeoCoordinate> route, Color? color, int width)
        {
            Color route_color;
            if (color != null && color.HasValue)
            {
                route_color = color.Value;
            }
            else
            {
                route_color = this.RandomColor();
            }

            ElementLine line = new ElementLine(
                new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(
                    Tools.Math.Geo.Factory.PrimitiveGeoFactory.Instance,
                    route.ToArray<GeoCoordinate>()),
                    route_color.ToArgb(),
                    width, true);
            line.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            this.AddElement(line);
        }

        /// <summary>
        /// Returns a random color
        /// </summary>
        /// <returns></returns>
        private System.Drawing.Color RandomColor()
        {
            Color color = Color.FromArgb(0, 0, 0); ;
            int color_indx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(16);
            switch (color_indx)
            {
                case 0:
                    color = Color.FromArgb(0, 0, 0);
                    break;
                case 1:
                    color = Color.FromArgb(128, 0, 0);
                    break;
                case 2:
                    color = Color.FromArgb(0, 128, 0);
                    break;
                case 3:
                    color = Color.FromArgb(128, 128, 0);
                    break;
                case 4:
                    color = Color.FromArgb(0, 0, 128);
                    break;
                case 5:
                    color = Color.FromArgb(128, 0, 128);
                    break;
                case 6:
                    color = Color.FromArgb(0, 128, 128);
                    break;
                case 7: //basicColorEnum.colorPaleGray:
                    color = Color.FromArgb(192, 192, 192);
                    break;
                case 8: //basicColorEnum.colorMidGray:
                    color = Color.FromArgb(128, 128, 128);
                    break;
                case 9: //basicColorEnum.colorRed:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 10: //basicColorEnum.colorGreen:
                    color = Color.FromArgb(0, 255, 0);
                    break;
                case 11: //basicColorEnum.colorYellow:
                    color = Color.FromArgb(255, 255, 0);
                    break;
                case 12: //basicColorEnum.colorBlue:
                    color = Color.FromArgb(0, 0, 255);
                    break;
                case 13: //basicColorEnum.colorMagenta:
                    color = Color.FromArgb(255, 0, 255);
                    break;
                case 14: //basicColorEnum.colorCyan:
                    color = Color.FromArgb(0, 255, 255);
                    break;
                case 15: //basicColorEnum.colorWhite:
                    color = Color.FromArgb(255, 255, 255);
                    break;
                default:
                    break;
            }

            return System.Drawing.Color.FromArgb(
                175,
                color.R,
                color.G,
                color.B);
        }
    }
}
