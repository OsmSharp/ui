using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private readonly OsmStreamReader _reader;

        /// <summary>
        /// Creates a new OsmGeo enumerable.
        /// </summary>
        /// <param name="reader"></param>
        public OsmGeoEnumerableStreamReader(OsmStreamReader reader)
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
    internal class OsmGeoEnumerableStreamEnumerator : OsmStreamOsmGeoWriter, IEnumerator<OsmGeo>
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
        protected override void AddNode(Node node)
        {
            _current = node;
        }

        /// <summary>
        /// A complete wasy was detected.
        /// </summary>
        /// <param name="way"></param>
        protected override void AddWay(Way way)
        {
            _current = way;
        }

        /// <summary>
        /// A complete relation was detected.
        /// </summary>
        /// <param name="relation"></param>
        protected override void AddRelation(Relation relation)
        {
            _current = relation;
        }
    }
}