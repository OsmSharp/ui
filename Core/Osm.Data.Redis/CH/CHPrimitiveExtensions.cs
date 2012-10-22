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

namespace Osm.Data.Redis.CH
{
    /// <summary>
    /// Extensions of Contraction Hierarchy primitives.
    /// </summary>
    internal static class CHPrimitiveExtensions
    {
        ///// <summary>
        ///// Builds a unique key for storage in redis.
        ///// </summary>
        ///// <param name="vertex"></param>
        ///// <returns></returns>
        //public static string BuildRedisKey(this CHVertex vertex)
        //{
        //    return CHPrimitiveExtensions.BuildRedisKeySparseSimpleVertex(vertex.Id);
        //}

        ///// <summary>
        ///// Builds a unique key for storage in redis.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static string BuildRedisKeySparseSimpleVertex(long id)
        //{
        //    return "c_s:" + id;
        //}
    }
}
