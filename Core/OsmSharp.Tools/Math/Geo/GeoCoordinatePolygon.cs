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

namespace OsmSharp.Tools.Math.Geo
{
    /// <summary>
    /// Class reprenting a polygon made out of geo coordinate points.
    /// </summary>
    public class GeoCoordinatePolygon : GenericPolygonF2D<GeoCoordinate>
    {
        /// <summary>
        /// Creates a geo coordinate polygon.
        /// </summary>
        /// <param name="points"></param>
        public GeoCoordinatePolygon(
            GeoCoordinate[] points)
            :base(points)
        {

        }

        #region Factory

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override GeoCoordinate CreatePoint(double[] values)
        {
            return new GeoCoordinate(values);
        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="is_segment1"></param>
        /// <param name="is_segment2"></param>
        /// <returns></returns>
        protected override GenericLineF2D<GeoCoordinate> CreateLine(
            GeoCoordinate point1,
            GeoCoordinate point2,
            bool is_segment1,
            bool is_segment2)
        {
            return new GeoCoordinateLine(point1, point2, is_segment1, is_segment2);
        }

        /// <summary>
        /// Creates a new rectangle
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override GenericRectangleF2D<GeoCoordinate> CreateRectangle(GeoCoordinate[] points)
        {
            return new GeoCoordinateBox(points);
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        protected override GenericPolygonF2D<GeoCoordinate> CreatePolygon(GeoCoordinate[] points)
        {
            return new GeoCoordinatePolygon(points);
        }

        #endregion
    }
}
