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

namespace OsmSharp.UI.Renderer.Scene
{
    /// <summary>
    /// Represents a scene 2D primitive enumerable that keeps primitives sorted by layer.
    /// </summary>
    public class Scene2DPrimitiveEnumerable : IEnumerable<Scene2DPrimitive>
    {
        /// <summary>
        /// Holds the enumerables.
        /// </summary>
        private IEnumerable<IEnumerable<Scene2DPrimitive>> _enumerables;

        /// <summary>
        /// Creates a new enumerable.
        /// </summary>
        /// <param name="enumerables"></param>
        public Scene2DPrimitiveEnumerable(IEnumerable<IEnumerable<Scene2DPrimitive>> enumerables)
        {
            _enumerables = enumerables;
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Scene2DPrimitive> GetEnumerator()
        {
            return new Scene2DPrimitiveEnumerator(_enumerables);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Scene2DPrimitiveEnumerator(_enumerables);
        }

        /// <summary>
        /// Represents an enumerator.
        /// </summary>
        class Scene2DPrimitiveEnumerator : IEnumerator<Scene2DPrimitive>
        {
            /// <summary>
            /// Holds the enumerables.
            /// </summary>
            private IEnumerable<IEnumerable<Scene2DPrimitive>> _enumerables;

            /// <summary>
            /// Creates a new enumerator.
            /// </summary>
            /// <param name="enumerables"></param>
            public Scene2DPrimitiveEnumerator(IEnumerable<IEnumerable<Scene2DPrimitive>> enumerables)
            {
                _enumerables = enumerables;

                this.Reset();
            }

            /// <summary>
            /// Holds the current.
            /// </summary>
            private Scene2DPrimitive? _current;

            /// <summary>
            /// Holds the current idx.
            /// </summary>
            private int _currentIdx;

            /// <summary>
            /// Holds all enumerators.
            /// </summary>
            private List<IEnumerator<Scene2DPrimitive>> _enumerators;

            /// <summary>
            /// Holds all move next flags.
            /// </summary>
            private bool[] _moveNextFlags;

            /// <summary>
            /// Returns the current object.
            /// </summary>
            public Scene2DPrimitive Current
            {
                get { return _current.Value; }
            }

            /// <summary>
            /// Returns the current object.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get { return _current.Value; }
            }

            /// <summary>
            /// Moves to the next object and 
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (_current == null)
                { // move to the first one for each.
                    for (int idx = 0; idx < _enumerators.Count; idx++)
                    {
                        if (!_enumerators[idx].MoveNext())
                        { // oeps, this one has no data.
                            _enumerators[idx] = null;
                        }
                    }
                }
                else
                { // there is already a current.
                    if (!_enumerators[_currentIdx].MoveNext()) // move the current one.
                    { // oeps, this one has not more data.
                        _enumerators[_currentIdx] = null;
                    }
                    if (_enumerators[_currentIdx] != null &&
                        _enumerators[_currentIdx].Current.Layer == _current.Value.Layer)
                    { // always ok, the current one has not changed layers, return.
                        _current = _enumerators[_currentIdx].Current;
                        return true;
                    }
                }
                // search for the one with the smallest layer.
                int layer = int.MaxValue;
                _current = null;
                for (int idx = 0; idx < _enumerators.Count; idx++)
                {
                    if (_enumerators[idx] != null &&
                        _enumerators[idx].Current.Layer < layer)
                    { // this one has data with a lower layer.
                        layer = _enumerators[idx].Current.Layer;
                        _currentIdx = idx;
                        _current = _enumerators[idx].Current;
                    }
                }
                return _current != null;
            }

            /// <summary>
            /// Disposes this enumerator.
            /// </summary>
            public void Dispose()
            {
                _current = null;
                _moveNextFlags = null;
                _enumerables = null;
                _enumerators.Clear();
                _enumerators = null;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _current = null;

                _enumerators = new List<IEnumerator<Scene2DPrimitive>>();
                foreach (IEnumerable<Scene2DPrimitive> enumerable in _enumerables)
                {
                    _enumerators.Add(enumerable.GetEnumerator());
                }
                _moveNextFlags = new bool[_enumerators.Count];
            }
        }
    }
}
