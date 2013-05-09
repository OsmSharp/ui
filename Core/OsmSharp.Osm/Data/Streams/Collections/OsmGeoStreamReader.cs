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

using System.Collections.Generic;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Osm.Data.Streams.Collections
{
    /// <summary>
    /// A data processor source of regular OsmBase objects.
    /// </summary>
    public class OsmGeoListDataProcessorSource : OsmStreamReader
    {
        /// <summary>
        /// Holds the list of OsmBase objects.
        /// </summary>
        private readonly IList<OsmGeo> _baseObjects;

        /// <summary>
        /// Holds the current object index.
        /// </summary>
        private int _current;

        /// <summary>
        /// Creates a new OsmBase source.
        /// </summary>
        /// <param name="baseObjects"></param>
        public OsmGeoListDataProcessorSource(IList<OsmGeo> baseObjects)
        {
            _baseObjects = baseObjects;
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
                _current < _baseObjects.Count);
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            // get the current object.
            OsmGeo osmObject = _baseObjects[_current];

            // convert the object.
            return osmObject.ToSimple();
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