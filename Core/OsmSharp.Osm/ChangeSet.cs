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

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents a changeset.
    /// </summary>
    public class ChangeSet : OsmBase
    {
        /// <summary>
        /// Holds all the changes in this changeset.
        /// </summary>
        private IList<Change> _changes;

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <param name="id"></param>
        internal ChangeSet(long id)
            :base(id)
        {
            _changes = new List<Change>();
        }

        #region Properties

        /// <summary>
        /// Returns an ordered list of all changes in this changeset.
        /// </summary>
        public IList<Change> Changes
        {
            get
            {
                return _changes;
            }
        }

        /// <summary>
        /// Returns the list of objects that this changeset applies to.
        /// </summary>
        public IList<OsmGeo> Objects
        {
            get
            {
                IList<OsmGeo> objs = new List<OsmGeo>();

                foreach (Change change in this.Changes)
                {
                    objs.Add(change.Object);
                }

                return objs;
            }
        }

        /// <summary>
        /// Returns the bounding box of this changeset.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get 
            {
                if (this.Objects.Count > 0)
                {
                    GeoCoordinateBox box = this.Objects[0].BoundingBox;

                    for (int idx = 1; idx < this.Objects.Count; idx++)
                    {
                        box = box + this.Objects[idx].BoundingBox;
                    }

                    return box;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the changeset type.
        /// </summary>
        public override OsmType Type
        {
            get { return OsmType.ChangeSet; }
        }

        #endregion
    }
}
