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
    /// Represents a route.
    /// </summary>
    public interface IRoute : IEnumerable<int>, ICloneable
    {
        /// <summary>
        /// Returns true if the route is empty.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Returns true if the last customer is linked with the first one.
        /// </summary>
        bool IsRound
        {
            get;
        }

        /// <summary>
        /// Returns the amount of customers in the route.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        /// <returns></returns>
        int First
        {
            get;
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        int Last
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
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool Remove(int customer);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to.
        /// 0->1->2:ReplaceEdgeFrom(0, 2):0->2 without resetting the last customer property.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void ReplaceEdgeFrom(int from, int to);

        /// <summary>
        /// Replaces the first customer.
        /// </summary>
        /// <param name="first"></param>
        void ReplaceFirst(int first);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to->unknown.
        /// 0->1:InsertAfter(0, 2):0->2-1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void InsertAfter(int from, int to);

        /// <summary>
        /// Inserst a new first customer.
        /// </summary>
        /// <param name="first"></param>
        void InsertFirst(int first);

        /// <summary>
        /// Returns the neigbours of a customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int[] GetNeigbours(int customer);

        /// <summary>
        /// Returns the index of the given customer the first being zero.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int GetIndexOf(int customer);

        /// <summary>
        /// Returns true if the route is valid.
        /// </summary>
        /// <returns></returns>
        bool IsValid();

        /// <summary>
        /// Returns an enumerable that enumerates between the two given customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IEnumerable<int> Between(int from, int to);

        /// <summary>
        /// Returns a enumerable that enumerates all edges in this route.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Edge> Edges();

        /// <summary>
        /// Removes all customers in this route.
        /// </summary>
        void Clear();
    }
}
