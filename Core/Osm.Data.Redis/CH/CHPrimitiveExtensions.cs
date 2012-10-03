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
