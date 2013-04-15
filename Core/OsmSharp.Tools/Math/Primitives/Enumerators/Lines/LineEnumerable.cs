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
using OsmSharp.Tools.Math.Primitives.Enumerators.Lines;

namespace OsmSharp.Tools.Math.Primitives.Enumerators.Points
{
    /// <summary>
    /// An enurable for a line.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    public sealed class LineEnumerable<PointType> : IEnumerable<GenericLineF2D<PointType>>
        where PointType : PointF2D
    {
        private LineEnumerator<PointType> _enumerator;

        internal LineEnumerable(LineEnumerator<PointType> enumerator)
        {
            _enumerator = enumerator;
        }

        #region IEnumerable<PointType> Members

        /// <summary>
        /// Creates the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<GenericLineF2D<PointType>> GetEnumerator()
        {
            return _enumerator;
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Creates the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }

        #endregion
    }
}
