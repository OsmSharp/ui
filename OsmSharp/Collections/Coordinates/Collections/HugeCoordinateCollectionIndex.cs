// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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

using OsmSharp.Collections.Arrays;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Geo.Simple;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections.Coordinates.Collections
{
    /// <summary>
    /// Represents a write-only large coordinate index.
    /// </summary>
    public class HugeCoordinateCollectionIndex : IDisposable
    {
        /// <summary>
        /// The maximum size of one collection is.
        /// </summary>
        private int MAX_COLLECTION_SIZE = ushort.MaxValue;

        /// <summary>
        /// The average estimated size.
        /// </summary>
        private int ESTIMATED_SIZE = 5;

        /// <summary>
        /// Holds the coordinates index position and count.
        /// </summary>
        private IHugeArray<ulong> _index;

        /// <summary>
        /// Holds the coordinates in linked-list form.
        /// </summary>
        private IHugeArray<float> _coordinates;

        /// <summary>
        /// Holds the next idx.
        /// </summary>
        private long _nextIdx = 0;

        /// <summary>
        /// Creates a new huge coordinate index.
        /// </summary>
        /// <param name="size"></param>
        public HugeCoordinateCollectionIndex(long size)
        {
            _index = new HugeArray<ulong>(size);
            _coordinates = new HugeArray<float>(size * 2 * ESTIMATED_SIZE);

            for (long idx = 0; idx < _index.Length; idx++)
            {
                _index[idx] = 0;
            }

            for (long idx = 0; idx < _coordinates.Length; idx++)
            {
                _coordinates[idx] = float.MinValue;
            }
        }

        /// <summary>
        /// Creates a new huge coordinate index.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="size"></param>
        public HugeCoordinateCollectionIndex(MemoryMappedFileFactory factory, long size)
        {
            _index = new MemoryMappedHugeArray<ulong>(factory, size);
            _coordinates = new MemoryMappedHugeArray<float>(factory, size * 2 * ESTIMATED_SIZE);

            for (long idx = 0; idx < _index.Length; idx++)
            {
                _index[idx] = 0;
            }

            for (long idx = 0; idx < _coordinates.Length; idx++)
            {
                _coordinates[idx] = float.MinValue;
            }
        }

        /// <summary>
        /// Returns the collection with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the collection was found.</returns>
        public bool Remove(long id)
        {
            long index, size;
            if(this.TryGetIndexAndSize(id, out index, out size))
            {
                this.DoReset(index, size);
                _index[id] = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds/updates the coordinate collection at the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coordinates"></param>
        public void Add(long id, ICoordinateCollection coordinates)
        {
            long index, size;
            if (this.TryGetIndexAndSize(id, out index, out size))
            {
                if(size == coordinates.Count * 2)
                { // just update in-place.
                    this.DoSet(index, coordinates);
                }
                else
                { // remove and add new.
                    _index[id] = this.DoAdd(coordinates);
                }
            }
            else
            { // add new coordinates.
                _index[id] = this.DoAdd(coordinates);
            }
        }

        /// <summary>
        /// Returns the coordinate collection at the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public bool Get(long id, out ICoordinateCollection coordinates)
        {
            long index, size;
            if (this.TryGetIndexAndSize(id, out index, out size))
            {
                coordinates = new HugeCoordinateCollection(_coordinates, index, size);
                return true;
            }
            coordinates = null;
            return false;
        }

        /// <summary>
        /// Gets or sets the coordinate collection at the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ICoordinateCollection this[long id]
        {
            get
            {
                ICoordinateCollection coordinates;
                if(this.Get(id, out coordinates))
                {
                    return coordinates;
                }
                return null;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    this.Remove(id);
                }
                else
                {
                    this.Add(id, value);
                }
            }
        }

        /// <summary>
        /// Trims the size of this collection index to it's smallest possible size.
        /// </summary>
        public void Compress()
        {

        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public void Resize(long size)
        {
            _index.Resize(size);

            long bestSize = size * 2 * ESTIMATED_SIZE;
            if (bestSize < _coordinates.Length)
            { // make sure all coordinate data is saved.
                bestSize = _coordinates.Length;
            }
            _coordinates.Resize(bestSize);
        }

        #region Helper Methods

        /// <summary>
        /// Increases the size of the coordinates array.
        /// To be used when the ESTIMATED_SIZE has underestimated to average coordinate collection size.
        /// </summary>
        private void IncreaseCoordinates()
        {
            _coordinates.Resize(_coordinates.Length + (1 << 20));
        }


        /// <summary>
        /// Tries to get the index and the size (if any).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private bool TryGetIndexAndSize(long id, out long index, out long size)
        {
            var data = _index[id];
            if(data == 0)
            {
                index = 0;
                size = 0;
                return false;
            }
            index = ((long)(data / (ulong)MAX_COLLECTION_SIZE)) * 2;
            size = (long)(data % (ulong)MAX_COLLECTION_SIZE) * 2;
            return true;
        }

        /// <summary>
        /// Resets all coordinates to the default.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        private void DoReset(long index, long size)
        {
            for(long idx = index; idx < index + (size); idx++)
            {
                _coordinates[idx] = float.MinValue; 
            }
        }

        /// <summary>
        /// Sets the coordinates starting at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="coordinates"></param>
        private void DoSet(long index, ICoordinateCollection coordinates)
        {
            long idx = index;
            coordinates.Reset();
            while(coordinates.MoveNext())
            {
                _coordinates[idx] = coordinates.Latitude;
                _coordinates[idx + 1] = coordinates.Longitude;
                idx = idx + 2;
            }
        }

        /// <summary>
        /// Adds the new coordinates at the end of the current coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private ulong DoAdd(ICoordinateCollection coordinates)
        {
            var newId = (ulong)(_nextIdx * MAX_COLLECTION_SIZE) + (ulong)coordinates.Count;
            coordinates.Reset();
            while(coordinates.MoveNext())
            {
                if (_coordinates.Length <= (_nextIdx * 2) + 1)
                { // make sure they fit!
                    this.IncreaseCoordinates();
                }
                _coordinates[(_nextIdx * 2)] = coordinates.Latitude;
                _coordinates[(_nextIdx * 2) + 1] = coordinates.Longitude;
                _nextIdx = _nextIdx + 1;
            }
            return newId;
        }

        #endregion

        /// <summary>
        /// Represents the huge coordinate collection.
        /// </summary>
        private class HugeCoordinateCollection : ICoordinateCollection
        {
            /// <summary>
            /// Holds the start idx.
            /// </summary>
            private long _startIdx;

            /// <summary>
            /// Holds the current idx.
            /// </summary>
            private long _currentIdx = -2;

            /// <summary>
            /// Holds the size.
            /// </summary>
            private long _size;

            /// <summary>
            /// Holds the coordinates.
            /// </summary>
            private IHugeArray<float> _coordinates;

            /// <summary>
            /// Holds the reverse flag.
            /// </summary>
            private bool _reverse;

            /// <summary>
            /// Creates a new huge coordinate collection.
            /// </summary>
            /// <param name="coordinates"></param>
            /// <param name="startIdx"></param>
            /// <param name="size"></param>
            public HugeCoordinateCollection(IHugeArray<float> coordinates, long startIdx, long size)
                : this(coordinates, startIdx, size, false)
            {

            }

            /// <summary>
            /// Creates a new huge coordinate collection.
            /// </summary>
            /// <param name="coordinates"></param>
            /// <param name="startIdx"></param>
            /// <param name="size"></param>
            /// <param name="reverse"></param>
            public HugeCoordinateCollection(IHugeArray<float> coordinates, long startIdx, long size, bool reverse)
            {
                _startIdx = startIdx;
                _size = size;
                _coordinates = coordinates;
                _reverse = reverse;
            }

            /// <summary>
            /// Returns the count.
            /// </summary>
            public int Count
            {
                get { return (int)(_size / 2); }
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<ICoordinate> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                this.Reset();
                return this;
            }

            /// <summary>
            /// Returns the current.
            /// </summary>
            public ICoordinate Current
            {
                get
                {
                    return new GeoCoordinateSimple()
                    {
                        Latitude = this.Latitude,
                        Longitude = this.Longitude
                    };
                }
            }

            /// <summary>
            /// Returns the current.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return new GeoCoordinateSimple()
                    {
                        Latitude = this.Latitude,
                        Longitude = this.Longitude
                    };
                }
            }

            /// <summary>
            /// Moves to the next coordinate.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if(_reverse)
                {
                    _currentIdx = _currentIdx - 2;
                    return (_currentIdx - _startIdx) >= 0;
                }
                else
                {
                    _currentIdx = _currentIdx + 2;
                    return (_currentIdx - _startIdx) < _size;
                }
            }

            /// <summary>
            /// Resets the current enumerator.
            /// </summary>
            public void Reset()
            {
                if(_reverse)
                {
                    _currentIdx = _startIdx + _size;
                }
                else
                {
                    _currentIdx = _startIdx - 2;
                }
            }

            /// <summary>
            /// Disposes all resources associated with this enumerable.
            /// </summary>
            public void Dispose()
            {

            }

            /// <summary>
            /// Returns the latitude.
            /// </summary>
            public float Latitude
            {
                get { return _coordinates[_currentIdx]; }
            }

            /// <summary>
            /// Returns the longitude.
            /// </summary>
            public float Longitude
            {
                get { return _coordinates[_currentIdx + 1]; }
            }

            /// <summary>
            /// Returns the reverse collection.
            /// </summary>
            /// <returns></returns>
            public ICoordinateCollection Reverse()
            {
                return new HugeCoordinateCollection(_coordinates, _startIdx, _size, !_reverse);
            }
        }

        /// <summary>
        /// Disposes of all resources associated with this coordinate collection index.
        /// </summary>
        public void Dispose()
        {
            _index.Dispose();
            _index = null;
            _coordinates.Dispose();
            _coordinates = null;
        }
    }
}