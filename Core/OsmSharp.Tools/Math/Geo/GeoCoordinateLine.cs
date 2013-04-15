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
    /// Class representing a geo coordinate line.
    /// </summary>
    public class GeoCoordinateLine : GenericLineF2D<GeoCoordinate>
    {
        /// <summary>
        /// Creates a geo coordinate line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        public GeoCoordinateLine(
            GeoCoordinate point1,
            GeoCoordinate point2)
            :base(point1,point2)
        {

        }

        /// <summary>
        /// Creates a geo coordinate line.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="is_segment1"></param>
        /// <param name="is_segment2"></param>
        public GeoCoordinateLine(
            GeoCoordinate point1,
            GeoCoordinate point2,
            bool is_segment1,
            bool is_segment2)
            : base(point1, point2, is_segment1, is_segment2)
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

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Point1.GetHashCode() ^
                   this.Point2.GetHashCode() ^
                   this.IsSegment1.GetHashCode() ^
                   this.IsSegment2.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is GeoCoordinateLine)
            {
                return this.Point1.Equals((obj as GeoCoordinateLine).Point1) &&
                       this.Point2.Equals((obj as GeoCoordinateLine).Point2) &&
                       this.IsSegment1.Equals((obj as GeoCoordinateLine).IsSegment1) &&
                       this.IsSegment2.Equals((obj as GeoCoordinateLine).IsSegment2);
            }
            return false;
        }
    }
}
