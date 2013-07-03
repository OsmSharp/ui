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
using OsmSharp.Osm.Data.Streams.OsmGeoStream;

namespace OsmSharp.Osm.Data.Streams.Collections
{
    /// <summary>
    /// Enumerable of all OsmGeo objects that can be detected in an OsmStream.
    /// </summary>
    internal class OsmGeoEnumerableStreamReader : IEnumerable<OsmGeo>
    {
        /// <summary>
        /// Holds the reader.
        /// </summary>
        private readonly OsmStreamSource _reader;

        /// <summary>
        /// Creates a new OsmGeo enumerable.
        /// </summary>
        /// <param name="reader"></param>
        public OsmGeoEnumerableStreamReader(OsmStreamSource reader)
        {
            _reader = reader;
        }
        
        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<OsmGeo> GetEnumerator()
        {
            var enumerator = new OsmGeoEnumerableStreamEnumerator();
            enumerator.RegisterSource(_reader);
            return enumerator;
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            var enumerator = new OsmGeoEnumerableStreamEnumerator();
            enumerator.RegisterSource(_reader);
            return enumerator;
        }
    }

    /// <summary>
    /// Enumerates all OsmGeo objects that can be detected in an OsmStream.
    /// </summary>
    internal class OsmGeoEnumerableStreamEnumerator : OsmStreamTargetOsmGeo, 
        IEnumerator<OsmGeo>, IOsmGeoStreamTarget
    {
        /// <summary>
        /// Holds the current object.
        /// </summary>
        private OsmGeo _current;

        /// <summary>
        /// Holds the initialized flag.
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// Returns the current object.
        /// </summary>
        public OsmGeo Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Disposes all resource associated with this enumerator.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Move next.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (!_initialized)
            {
                this.RegisterOsmGeoTarget(this);

                this.Initialize();
                this.Reader.Initialize();
                _initialized = true;
            }
            _current = null;
            while (this.PullNext())
            {
                if (_current != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Resets this enumerator.
        /// </summary>
        public void Reset()
        {
            _current = null;
            if (!this.Reader.CanReset)
            {
                throw new Exception("This enumerator cannot be reset!");
            }
            this.Reader.Reset();
            _initialized = false;
        }

        /// <summary>
        /// A complete node was detected.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Node node)
        {
            _current = node;
        }

        /// <summary>
        /// A complete wasy was detected.
        /// </summary>
        /// <param name="way"></param>
        public void AddWay(Way way)
        {
            _current = way;
        }

        /// <summary>
        /// A complete relation was detected.
        /// </summary>
        /// <param name="relation"></param>
        public void AddRelation(Relation relation)
        {
            _current = relation;
        }
    }
}