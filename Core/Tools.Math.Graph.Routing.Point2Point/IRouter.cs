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
