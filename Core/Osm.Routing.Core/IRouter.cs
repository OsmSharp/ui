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
using Tools.Math.Geo;
using Osm.Routing.Core.Route;
using Osm.Core;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.Core
{
    /// <summary>
    /// Interface representing a router.
    /// </summary>
    public interface IRouter<ResolvedType>
        where ResolvedType : IResolvedPoint
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
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <returns></returns>
        OsmSharpRoute Calculate(ResolvedType source, ResolvedType target);

        /// <summary>
        /// Calculates a route between two given points.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="max">The maximum weight to stop the calculation.</param>
        /// <returns></returns>
        OsmSharpRoute Calculate(ResolvedType source, ResolvedType target, float max);

        /// <summary>
        /// Calculates a shortest route from a given point to any of the targets points.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="targets">The target point(s).</param>
        /// <returns></returns>
        OsmSharpRoute CalculateToClosest(ResolvedType source, ResolvedType[] targets);

        /// <summary>
        /// Calculates a shortest route from a given point to any of the targets points.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="targets">The target point(s).</param>
        /// <param name="max">The maximum weight to stop the calculation.</param>
        /// <returns></returns>
        OsmSharpRoute CalculateToClosest(ResolvedType source, ResolvedType[] targets, float max);

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

        #region Range Calculation

        /// <summary>
        /// Returns true if range calculation is supported.
        /// </summary>
        bool IsCalculateRangeSupported
        {
            get;
        }

        /// <summary>
        /// Returns all points located at a given weight (distance/time) from the orgin.
        /// </summary>
        /// <param name="orgine"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        HashSet<ILocationObject> CalculateRange(ResolvedType orgin, float weight);

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
        /// Resolves a point.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        ResolvedType Resolve(GeoCoordinate coordinate, IResolveMatcher matcher);

        /// <summary>
        /// Resolves all the given points.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        ResolvedType[] Resolve(GeoCoordinate[] coordinate);

        /// <summary>
        /// Resolves all the given points.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        ResolvedType[] Resolve(GeoCoordinate[] coordinate, IResolveMatcher matcher);

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
