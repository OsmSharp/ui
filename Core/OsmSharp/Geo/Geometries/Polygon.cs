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

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a polygon.
    /// </summary>
    public class Polygon : Geometry
    {
        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        public Polygon()
        {
            this.Holes = new List<LineairRing>();
            this.Ring = new LineairRing();
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="outline">The outline of the polygon.</param>
        public Polygon(LineairRing outline)
        {
            this.Holes = new List<LineairRing>();
            this.Ring = outline;
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="outline"></param>
        /// <param name="holes"></param>
        public Polygon(LineairRing outline, IEnumerable<LineairRing> holes)
        {
            this.Holes = holes;
            this.Ring = outline;
        }

        /// <summary>
        /// Gets the holes in this polygon.
        /// </summary>
        public IEnumerable<LineairRing> Holes { get; private set; }

        /// <summary>
        /// Gets the outer outline lineair ring of this polygon.
        /// </summary>
        public LineairRing Ring { get; set; }

        /// <summary>
        /// Returns the smallest possible bounding box containing this geometry.
        /// </summary>
        public override GeoCoordinateBox Box
        {
            get
            {
                GeoCoordinateBox box = this.Ring.Box;
                foreach (Geometry geometry in this.Holes)
                {
                    if (box == null)
                    {
                        box = geometry.Box;
                    }
                    else
                    {
                        box = box + geometry.Box;
                    }
                }
                return box;
            }
        }

        /// <summary>
        /// Returns true if this polygon is inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public override bool IsInside(GeoCoordinateBox box)
        {
            throw new NotImplementedException();
        }
    }
}