using System;
using System.Collections.Generic;
using System.IO;
using OsmSharp.Math.Primitives;

namespace OsmSharp.Collections.SpatialIndexes.Serialization.v1
{
    /// <summary>
    /// R-tree implementation of a spatial index.
    /// http://en.wikipedia.org/wiki/R-tree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class RTreeStreamIndex<T> : ISpatialIndexReadonly<T>
    {
        /// <summary>
        /// Holds the serializer.
        /// </summary>
        private readonly RTreeStreamSerializer<T> _serializer;

        /// <summary>
        /// Holds the stream.
        /// </summary>
        private readonly SpatialIndexSerializerStream _stream;

        /// <summary>
        /// Creates a new index.
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="stream"></param>
        public RTreeStreamIndex(RTreeStreamSerializer<T> serializer,
            SpatialIndexSerializerStream stream)
        {
            _serializer = serializer;
            _stream = stream;
        }

        /// <summary>
        /// Returns the data that has overlapping bounding boxes with the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public IEnumerable<T> Get(RectangleF2D box)
        {
            var results = new HashSet<T>();

            // move to the start.
            _stream.Seek(0, SeekOrigin.Begin);

            _serializer.Search(_stream, box, results);
            return results;
        }
    }
}