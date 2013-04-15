using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo;

namespace OsmSharp.Tools.Math
{
    /// <summary>
    /// An object that represents a location.
    /// </summary>
    public interface ILocationObject
    {
        /// <summary>
        /// Returns the location of this object.
        /// </summary>
        GeoCoordinate Location
        {
            get;
        }
    }
}
