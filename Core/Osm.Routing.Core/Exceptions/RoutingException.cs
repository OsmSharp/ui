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
using Osm.Core;

namespace Osm.Routing.Core.Exceptions
{
    /// <summary>
    /// A routing exception.
    /// </summary>
    public class RoutingException : Exception
    {
        /// <summary>
        /// Creates a new routing exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public RoutingException(Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException exception,
            ILocationObject from, ILocationObject to)
            :base(string.Format("Route not found from {0} to {1}!",from, to))
        {
            this.From = new HashSet<ILocationObject>();
            this.From.Add(from);

            this.To = new HashSet<ILocationObject>();
            this.To.Add(to);
        }

        /// <summary>
        /// Creates a new routing exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        public RoutingException(Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException exception,
            ILocationObject from, IEnumerable<ILocationObject> tos)
            : base(string.Format("Multiple routes not found from {0}!", from))
        {
            this.From = new HashSet<ILocationObject>();
            this.From.Add(from);

            this.To = new HashSet<ILocationObject>();
            this.To.UnionWith(tos);
        }
        
        /// <summary>
        /// Creates a new routing exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        public RoutingException(Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException exception,
            IEnumerable<ILocationObject> from, IEnumerable<ILocationObject> tos)
            : base(string.Format("Multiple routes not found from {0}!", from))
        {
            this.From = new HashSet<ILocationObject>();
            this.From.UnionWith(from);

            this.To = new HashSet<ILocationObject>();
            this.To.UnionWith(tos);
        }

        /// <summary>
        /// Holds all the from points.
        /// </summary>
        public HashSet<ILocationObject> From { get; private set; }

        /// <summary>
        /// Holds all the to points.
        /// </summary>
        public HashSet<ILocationObject> To { get; private set; }
    }
}
