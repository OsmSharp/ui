using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Routing.Core.Route;
using Osm.Core;

namespace Osm.Routing.Core
{
    /// <summary>
    /// Interface representing a router.
    /// </summary>
    public interface IRouter<ResolvedType>
        where ResolvedType : ILocationObject
    {
        #region Capabilities

        /// <summary>
        /// Returns true if the given vehicle type is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        bool SupportsVehicle(VehicleEnum vehicle);

        #endregion

        #region Routing

        /// <summary>
        /// Calculates a route between two given points.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        OsmSharpRoute Calculate(ResolvedType source, ResolvedType target);

        /// <summary>
        /// Calculates the weight between two given points.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        float CalculateWeight(ResolvedType source, ResolvedType target);

        /// <summary>
        /// Calculates a route between one source and many target points.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        float[] CalculateOneToManyWeight(ResolvedType source, ResolvedType[] targets);

        /// <summary>
        /// Calculates all routes between many sources/targets.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        float[][] CalculateManyToManyWeight(ResolvedType[] sources, ResolvedType[] targets);

        #endregion

        #region Error Detection/Error Handling

        /// <summary>
        /// Returns true if the given point is connected for a radius of at least the given weight.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        bool CheckConnectivity(ResolvedType point, float weight);

        /// <summary>
        /// Returns true if the given point is connected for a radius of at least the given weight.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        bool[] CheckConnectivity(ResolvedType[] point, float weight);
        
        #endregion

        #region Resolving

        /// <summary>
        /// Resolves a point.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        ResolvedType Resolve(GeoCoordinate coordinate);

        /// <summary>
        /// Resolves all the given points.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        ResolvedType[] Resolve(GeoCoordinate[] coordinate);

        #region Search

        /// <summary>
        /// Searches for a closeby link to the road network.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        /// <remarks>Similar to resolve except no resolved point is created.</remarks>
        GeoCoordinate Search(GeoCoordinate coordinate);

        #endregion

        #endregion
    }
}
