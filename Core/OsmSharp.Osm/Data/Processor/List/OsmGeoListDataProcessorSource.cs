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
using OsmSharp.Osm;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Data.Core.Processor.ListSource
{
    /// <summary>
    /// A data processor source of regular OsmBase objects.
    /// </summary>
    public class OsmGeoListDataProcessorSource : DataProcessorSource
    {
        /// <summary>
        /// Holds the list of OsmBase objects.
        /// </summary>
        private IList<OsmGeo> _base_objects;

        /// <summary>
        /// Holds the current object index.
        /// </summary>
        private int _current;

        /// <summary>
        /// Creates a new OsmBase source.
        /// </summary>
        /// <param name="base_objects"></param>
        public OsmGeoListDataProcessorSource(IList<OsmGeo> base_objects)
        {
            _base_objects = base_objects;
            _current = int.MinValue;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            this.Reset();
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            _current++;
            return (_current >= 0 &&
                _current < _base_objects.Count);
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            // get the current object.
            OsmGeo osm_object = _base_objects[_current];

            // convert the object.
            return osm_object.ToSimple();
        }

        /// <summary>
        /// Resets this data source.
        /// </summary>
        public override void Reset()
        {
            _current = -1;
        }

        /// <summary>
        /// Returns true, this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }
    }
}
