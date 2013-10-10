// OsmSharp - OpenStreetMap (OSM) SDK
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
using OsmSharp.Osm.Data.Streams.Collections;
using OsmSharp.Osm;
using System.Collections;

namespace OsmSharp.Osm.Data.Streams
{
    /// <summary>
    /// Base class for any (streamable) source of osm data (Nodes, Ways and Relations).
    /// </summary>
    public abstract class OsmStreamSource : IEnumerable<OsmGeo>, IEnumerator<OsmGeo>
    {
        /// <summary>
        /// Creates a new source.
        /// </summary>
        protected OsmStreamSource()
        {

        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public abstract void Initialize();
        
        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();

        /// <summary>
        /// Returns the current item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract OsmGeo Current();

        /// <summary>
        /// Resets the source to the beginning.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanReset
        {
            get;
        }

        #region Pull Command

        /// <summary>
        /// Pulls all objects from this source into a collection.
        /// </summary>
        /// <returns></returns>
        public ICollection<OsmGeo> PullToCollection()
        {
            // create collection.
            var collection = new List<OsmGeo>();

            // create the collection target and pull the data into it.
            var collectionTarget = new OsmCollectionStreamWriter(
                collection);
            collectionTarget.RegisterSource(this);
            collectionTarget.Pull();

            return collection;
        }

        #endregion

        #region IEnumerator/IEnumerable Implementation

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<OsmGeo> GetEnumerator()
        {
            this.Initialize();

            return this;
        }

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Initialize();

            return this;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        OsmGeo IEnumerator<OsmGeo>.Current
        {
            get { return this.Current(); }
        }

        /// <summary>
        /// Disposes all resources associated with this source.
        /// </summary>
        public virtual void Dispose()
        {

        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current(); }
        }

        #endregion
    }
}