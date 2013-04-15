// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm.Renderer.Gdi
{
    public static class Extensions
    {
        public static System.Drawing.PointF[] ConvertToDrawing(
            this OsmSharp.Tools.Math.PointF2D[] points)
        {
            System.Drawing.PointF[] drawing_points = new System.Drawing.PointF[points.Length];

            for (int idx = 0; idx < points.Length; idx++)
            {
                drawing_points[idx] = new System.Drawing.PointF(
                    (float)points[idx][0],
                    (float)points[idx][1]);
            }

            return drawing_points;
        }

        public static System.Drawing.PointF ConvertToDrawing(
            this OsmSharp.Tools.Math.PointF2D point)
        {
            return new System.Drawing.PointF(
                    (float)point[0],
                    (float)point[1]);
        }
    }
}
