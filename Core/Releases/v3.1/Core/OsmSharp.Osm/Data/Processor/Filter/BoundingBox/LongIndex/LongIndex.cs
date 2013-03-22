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
        /// The root.
        /// </summary>
        private LongIndexNode _root;

        /// <summary>
        /// Creates a new longindex.
        /// </summary>
        public LongIndex()
        {
            _root = new LongIndexNode(1);
        }

        /// <summary>
        /// Adds an id.
        /// </summary>
        /// <param name="number"></param>
        public void Add(long number)
        {
            while (number >= LongIndexNode.CalculateBaseNumber((short)(_root.Base + 1)))
            {
                LongIndexNode old_root = _root;
                _root = new LongIndexNode((short)(_root.Base + 1));
                _root.has_0 = old_root;
            }

            _root.Add(number);
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        /// <param name="number"></param>
        public void Remove(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_root.Base + 1)))
            {
                _root.Remove(number);
            }
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool Contains(long number)
        {
            if (number < LongIndexNode.CalculateBaseNumber((short)(_root.Base + 1)))
            {
                return _root.Contains(number);
            }
            return false;
        }

        /// <summary>
        /// Clears this index.
        /// </summary>
        public void Clear()
        {
            _root = new LongIndexNode(1);
        }    
    }
}
