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

namespace Tools.Math.Graph.Routing
{
    /// <summary>
    /// An interface providing general functionality for a point to point routing algorithm.
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Calculates the route between two vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        RouteLinked Calculate(long from, long to);

        /// <summary>
        /// Calculates the weight of the route between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        float CalculateWeight(long from, long to);
        
        /// <summary>
        /// Calculates the one-to-many weights betwee all the given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        float[] CalculateOneToMany(long from, long[] to);

        /// <summary>
        /// Calculates the many-to-many weights betwee all the given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        float[][] CalculateManyToMany(long[] from, long[] to);

        /// <summary>
        /// Checks the connectivty of the given vertex to at least another number of vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="connectivity_count"></param>
        /// <returns></returns>
        bool CheckConnectivity(long from,
            int connectivity_count);
    }
}
