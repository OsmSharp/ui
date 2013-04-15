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

namespace OsmSharp.Tools.Math.VRP.Core.Routes
{
    /// <summary>
    /// Represents an object containing multiple routes.
    /// </summary>
    public interface IMultiRoute : ICloneable
    {
        /// <summary>
        /// Returns one of the routes.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        IRoute Route(int idx);

        /// <summary>
        /// Adds a new empty route.
        /// </summary>
        /// <returns></returns>
        IRoute Add();

        /// <summary>
        /// Adds a new route with an intial customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        IRoute Add(int customer);

        /// <summary>
        /// Adds a new route by copying the given one.
        /// </summary>
        /// <param name="route"></param>
        IRoute Add(IRoute route);

        /// <summary>
        /// Removes the route at the given index.
        /// </summary>
        /// <param name="route_idx"></param>
        /// <returns></returns>
        bool Remove(int route_idx);

        /// <summary>
        /// Returns the number of routes.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the current size of this route.
        /// </summary>
        int Size
        {
            get;
        }

        /// <summary>
        /// Returns true if there is an edge in this route from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool Contains(int from, int to);

        /// <summary>
        /// Returns true if the given customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool Contains(int customer);

        /// <summary>
        /// Returns the customer next in line.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int Next(int customer);

        /// <summary>
        /// Removes the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool RemoveCustomer(int customer);
    }
}