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

namespace OsmSharp.Osm.Data.Core.Processor.Filter.LongIndex
{
    /// <summary>
    /// An efficient index for OSM object ids.
    /// </summary>
    public class LongIndex
    {
        /// <summary>
        /// The root of the positive ids.
        /// </summary>
        private LongIndexNode _rootPositive;

        /// <summary>
        /// The root of the negative ids.
        /// </summary>
        private LongIndexNode _rootNegative;

        /// <summary>
        /// Creates a new longindex.
        /// </summary>
        public LongIndex()
        {
            _rootPositive = new LongIndexNode(1);
            _rootNegative = new LongIndexNode(1);
        }

        /// <summary>
        /// Adds an id.
        /// </summary>
        /// <param name="number"></param>
        public void Add(long number)
        {
            if (number >= 0)
            {
                this.PositiveAdd(number);
            }
            else
            {
                this.NegativeAdd(-number);
            }
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        /// <param name="number"></param>
        public void Remove(long number)
        {
            if (number >= 0)
            {
                this.PositiveRemove(number);
            }
            else
            {
                this.NegativeAdd(-number);
            }
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool Contains(long number)
        {
            if (number >= 0)
            {
                return this.PositiveContains(number);
            }
            else
            {
                return this.NegativeContains(-number);
            }
        }

        #region Positive

        /// <summary>
        /// Adds an id.
        /// </summary>
        /// <param name="number"></param>
        private void PositiveAdd(long number)
        {
            while (number >= LongIndexNode.CalculateBaseNumber((short)(_rootPositive.Base + 1)))
            {
                LongIndexNode oldRoot = _rootPositive;
                _rootPositive = new LongIndexNode((short)(_rootPositive.Base + 1));
                _rootPositive.has_0 = oldRoot;
            }

            _rootPositive.Add(number);
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        /// <param name="number"></param>
        private void PositiveRemove(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_rootPositive.Base + 1)))
            {
                _rootPositive.Remove(number);
            }
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private bool PositiveContains(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_rootPositive.Base + 1)))
            {
                return _rootPositive.Contains(number);
            }
            return false;
        }

        #endregion

        #region Negative

        /// <summary>
        /// Adds an id.
        /// </summary>
        /// <param name="number"></param>
        private void NegativeAdd(long number)
        {
            while (number >= LongIndexNode.CalculateBaseNumber((short)(_rootNegative.Base + 1)))
            {
                LongIndexNode oldRoot = _rootNegative;
                _rootNegative = new LongIndexNode((short)(_rootNegative.Base + 1));
                _rootNegative.has_0 = oldRoot;
            }

            _rootNegative.Add(number);
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        /// <param name="number"></param>
        private void NegativeRemove(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_rootNegative.Base + 1)))
            {
                _rootNegative.Remove(number);
            }
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private bool NegativeContains(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_rootNegative.Base + 1)))
            {
                return _rootNegative.Contains(number);
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Clears this index.
        /// </summary>
        public void Clear()
        {
            _rootPositive = new LongIndexNode(1);
            _rootNegative = new LongIndexNode(1);
        }    
    }
}
