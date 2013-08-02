// OsmSharp - OpenStreetMap tools & library.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Math;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a lineair ring, a polygon without holes.
    /// </summary>
    public class LineairRing : LineString
    {
        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        public LineairRing()
        {

        }

        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public LineairRing(IEnumerable<GeoCoordinate> coordinates)
            : base(coordinates)
        {

        }

        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public LineairRing(params GeoCoordinate[] coordinates)
            : base(coordinates)
        {

        }

        /// <summary>
        /// Returns true if the given coordinate is contained in this ring.
        /// 
        /// See: http://geomalgorithms.com/a03-_inclusion.html
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool Contains(GeoCoordinate coordinate)
        {
            int number = 0;
            if (this.Coordinates[0] == coordinate)
            { // the given point is one of the corners.
                return true;
            }
            // loop over all edges and calculate if they possibly intersect.
            for (int idx = 0; idx < this.Coordinates.Count - 1; idx++)
            {
                if (this.Coordinates[idx + 1] == coordinate)
                { // the given point is one of the corners.
                    return true;
                }
                bool idxRight = this.Coordinates[idx].Longitude >= coordinate.Longitude;
                bool idx1Right = this.Coordinates[idx + 1].Longitude >= coordinate.Longitude;
                if (idxRight || idx1Right)
                { // at least on of the coordinates is to the right of the point to calculate for.
                    if (this.Coordinates[idx].Latitude <= coordinate.Latitude &&
                        this.Coordinates[idx + 1].Latitude >= coordinate.Latitude)
                    { // idx is lower than idx+1
                        if (idxRight && idx1Right)
                        { // no need for the left/right algorithm the result is already known.
                            number++;
                        }
                        else
                        { // one of the coordinates is not to the 'right' now we need the left/right algorithm.
                            LineF2D localLine = new LineF2D(this.Coordinates[idx], this.Coordinates[idx + 1]);
                            if (localLine.PositionOfPoint(coordinate) == LinePointPosition.Left)
                            {
                                number++;
                            }
                        }
                    }
                    else if (this.Coordinates[idx].Latitude >= coordinate.Latitude &&
                        this.Coordinates[idx + 1].Latitude <= coordinate.Latitude)
                    { // idx is higher than idx+1
                        if (idxRight && idx1Right)
                        { // no need for the left/right algorithm the result is already known.
                            number--;
                        }
                        else
                        { // one of the coordinates is not to the 'right' now we need the left/right algorithm.
                            LineF2D localLine = new LineF2D(this.Coordinates[idx], this.Coordinates[idx + 1]);
                            if (localLine.PositionOfPoint(coordinate) == LinePointPosition.Right)
                            {
                                number--;
                            }
                        }
                    }
                }
            }
            return number != 0;
        }

        /// <summary>
        /// Returns true if the given ring is contained in this ring.
        /// </summary>
        /// <param name="lineairRing"></param>
        /// <returns></returns>
        public bool Contains(LineairRing lineairRing)
        {
            // check if all points are inside this ring.
            foreach (var coordinate in lineairRing.Coordinates)
            {
                if (!this.Contains(coordinate))
                { // a coordinate ouside of this ring can never be part of a contained inner ring.
                    return false;
                }
            }
            // check if none of the points of this ring are inside the other ring.
            foreach (var coordinate in this.Coordinates)
            {
                if (lineairRing.Contains(coordinate))
                { // a coordinate ouside of this ring can never be part of a contained inner ring.
                    return false;
                }
            }
            return true;
        }
    }
}
