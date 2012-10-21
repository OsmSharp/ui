// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph.Routing.Point2Point.Exceptions
{
    /// <summary>
    /// Exception to indicate routing errors.
    /// </summary>
    public class RoutingException : Exception
    {
        /// <summary>
        /// The points in the from list.
        /// </summary>
        private HashSet<long> _from;

        /// <summary>
        /// The poiints in the to list.
        /// </summary>
        private HashSet<long> _to;
        
        /// <summary>
        /// Creates a routing exception with one source and one target.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public RoutingException(long from, long to)
            : base(string.Format("Route not found from {0} to {1}!", from, to))
        {
            _from = new HashSet<long>();
            _from.Add(from);

            _to = new HashSet<long>();
            _to.Add(to);
        }

        /// <summary>
        /// Creates a routing exception with one source and multiple targets.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public RoutingException(long from, IEnumerable<long> to)
            : base("Multiple routes not found!")
        {
            _from = new HashSet<long>();
            _from.Add(from);

            _to = new HashSet<long>();
            _to.UnionWith(to);
        }

        /// <summary>
        /// Creates a routing exception with multiple sources and multiple targets.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public RoutingException(IEnumerable<long> from, IEnumerable<long> to)
            : base("Multiple routes not found!")
        {
            _from = new HashSet<long>();
            _from.UnionWith(from);

            _to = new HashSet<long>();
            _to.UnionWith(to);
        }

        /// <summary>
        /// Returns the from collection.
        /// </summary>
        public HashSet<long> From
        {
            get
            {
                return _from;
            }
        }

        /// <summary>
        /// Returns the to collection.
        /// </summary>
        public HashSet<long> To
        {
            get
            {
                return _to;
            }
        }
    }
}
