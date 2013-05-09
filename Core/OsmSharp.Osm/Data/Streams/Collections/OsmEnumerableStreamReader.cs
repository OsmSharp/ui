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
    /// An OSM Stream Reader that wraps around a collection of OSM objects.
    /// </summary>
    internal class OsmEnumerableStreamReader : OsmStreamReader
    {
        /// <summary>
        /// Holds the list of SimpleOsmGeo objects.
        /// </summary>
        private readonly IEnumerable<SimpleOsmGeo> _baseObjects;

        /// <summary>
        /// Holds the current enumerator.
        /// </summary>
        private IEnumerator<SimpleOsmGeo> _baseObjectEnumerator;

        /// <summary>
        /// Creates a new OsmBase source.
        /// </summary>
        /// <param name="baseObjects"></param>
        public OsmEnumerableStreamReader(IEnumerable<SimpleOsmGeo> baseObjects)
        {
            _baseObjects = baseObjects;
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
            if (_baseObjectEnumerator == null)
            { // create the enumerator.
                _baseObjectEnumerator = _baseObjects.GetEnumerator();
            }

            // move next.
            if (!_baseObjectEnumerator.MoveNext())
            { // the move failed!
                _baseObjectEnumerator = null;
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
            SimpleOsmGeo osmObject = _baseObjectEnumerator.Current;

            // convert the object.
            return osmObject;
        }

        /// <summary>
        /// Resets this data source.
        /// </summary>
        public override void Reset()
        {
            _baseObjectEnumerator = null;
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