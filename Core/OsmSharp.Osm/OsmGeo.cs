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
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Shapes;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Base class for all the osm data that represents an object on the map.
    /// 
    /// Nodes, Ways and Relations
    /// </summary>
    public abstract class OsmGeo : OsmBase
    {
        /// <summary>
        /// Creates a new OsmGeo object.
        /// </summary>
        /// <param name="id"></param>
        internal OsmGeo(long id)
            :base(id)
        {
            this.Visible = true;
            this.UserId = null;
            this.User = null;
        }

        /// <summary>
        /// Creates a new OsmGeo object with a string table.
        /// </summary>
        /// <param name="string_table"></param>
        /// <param name="id"></param>
        internal OsmGeo(ObjectTable<string> string_table, long id)
            : base(string_table, id)
        {
            this.Visible = true;
            this.UserId = null;
            this.User = null;
        }

        /// <summary>
        /// Converts this OsmGeo object to an OsmGeoSimple object.
        /// </summary>
        /// <returns></returns>
        public abstract OsmSharp.Osm.Simple.SimpleOsmGeo ToSimple();

        #region Shape - Interpreter

        /// <summary>
        /// The interpreter for these objects.
        /// </summary>
        public static IShapeInterpreter ShapeInterperter = new SimpleShapeInterpreter(); // set a default shape interpreter.

        /// <summary>
        /// The shape this osm geo object represents.
        /// </summary>
        private ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> _shape;

        /// <summary>
        /// Returns the shape of this object represents.
        /// </summary>
        public ShapeF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> Shape
        {
            get
            {
                if (_shape == null)
                {
                    _shape = OsmGeo.ShapeInterperter.Interpret(this);
                }
                return _shape;
            }
        }

        /// <summary>
        /// Make sure the shape of this objects will be recalculated the next time it is requested.
        /// </summary>
        public void ResetShape()
        {
            _shape = null;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// The bounding box of object.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get
            {
                return this.Shape.BoundingBox;
            }
        }

        /// <summary>
        /// Gets/Sets the changeset id.
        /// </summary>
        public long? ChangeSetId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the visible flag.
        /// </summary>
        public bool Visible { get; set; }

        #endregion
    }
}
