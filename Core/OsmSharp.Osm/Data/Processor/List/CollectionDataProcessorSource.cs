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

namespace OsmSharp.Osm.Data.Core.Processor.List
{
    /// <summary>
    /// A data processor source of regular SimpleOsmBase objects.
    /// </summary>
    public class CollectionDataProcessorSource : DataProcessorSource
    {
        /// <summary>
        /// Holds the list of SimpleOsmGeo objects.
        /// </summary>
        private IEnumerable<SimpleOsmGeo> _base_objects;

        /// <summary>
        /// Holds the current enumerator.
        /// </summary>
        private IEnumerator<SimpleOsmGeo> _base_object_enumerator;

        /// <summary>
        /// Creates a new OsmBase source.
        /// </summary>
        /// <param name="base_objects"></param>
        public CollectionDataProcessorSource(IEnumerable<SimpleOsmGeo> base_objects)
        {
            _base_objects = base_objects;
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
            if (_base_object_enumerator == null)
            { // create the enumerator.
                _base_object_enumerator = _base_objects.GetEnumerator();
            }

            // move next.
            if (!_base_object_enumerator.MoveNext())
            { // the move failed!
                _base_object_enumerator = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            // get the current object.
            SimpleOsmGeo osm_object = _base_object_enumerator.Current;

            // convert the object.
            return osm_object;
        }

        /// <summary>
        /// Resets this data source.
        /// </summary>
        public override void Reset()
        {
            _base_object_enumerator = null;
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