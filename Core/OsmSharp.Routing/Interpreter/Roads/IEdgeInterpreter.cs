using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Units.Speed;

namespace OsmSharp.Routing.Interpreter.Roads
{
    /// <summary>
    /// Interpreter for edges in the routable data.
    /// </summary>
    public interface IEdgeInterpreter
    {
        /// <summary>
        /// Returns true if in some configuration this edge is traversable.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        bool IsRoutable(IDictionary<string, string> tags);

        /// <summary>
        /// Returns true if the tags represent a oneway edge, false if oneway reverse and null if none.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        bool? IsOneWay(IDictionary<string, string> tags, VehicleEnum vehicle);

        /// <summary>
        /// Returns true if the edge with given tags can be traversed by the given vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        bool CanBeTraversedBy(IDictionary<string, string> tags, VehicleEnum vehicle);

        /// <summary>
        /// Returns true if the edge is only locally accessible.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        bool IsOnlyLocalAccessible(IDictionary<string, string> tags);

        /// <summary>
        /// Returns the weight between two points on the given edge.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        double Weight(IDictionary<string, string> tags, VehicleEnum vehicle,
            GeoCoordinate from, GeoCoordinate to);

        /// <summary>
        /// Returns the name of the edge represented by the tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        string GetName(IDictionary<string, string> tags);

        /// <summary>
        /// Returns the names of the edge represented by the tags in each available language.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        Dictionary<string, string> GetNamesInAllLanguages(IDictionary<string, string> tags);

        /// <summary>
        /// Returns true if the tags (or a subset of) represent the same edge type for the given vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="tags1"></param>
        /// <param name="tags2"></param>
        /// <returns></returns>
        bool IsEqualFor(VehicleEnum vehicle, IDictionary<string, string> tags1, Dictionary<string, string> tags2);

        /// <summary>
        /// Returns the maximum possible speed a vehicle can travel on a edge with the given properties.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        KilometerPerHour MaxSpeed(VehicleEnum vehicle, IDictionary<string, string> tags);

        /// <summary>
        /// Returns true if the edge with given properties is a roundabout.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        bool IsRoundabout(IDictionary<string, string> tags);
    }
}