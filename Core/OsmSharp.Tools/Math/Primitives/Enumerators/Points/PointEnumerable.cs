// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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

namespace OsmSharp.Tools.Math.Primitives.Enumerators.Points
{
    /// <summary>
    /// An enumerable for a point.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    public sealed class PointEnumerable<PointType> : IEnumerable<PointType>
        where PointType : PointF2D
    {
        private PointEnumerator<PointType> _enumerator;

        internal PointEnumerable(PointEnumerator<PointType> enumerator)
        {
            _enumerator = enumerator;
        }

        #region IEnumerable<PointType> Members

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<PointType> GetEnumerator()
        {
            return _enumerator;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }

        #endregion
    }
}
