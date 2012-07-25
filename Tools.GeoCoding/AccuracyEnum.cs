using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.GeoCoding
{
    /// <summary>
    /// Describing the accuracy of a geocoding result.
    /// 
    /// This is the same as the Google "enum GGeoAddressAccuracy".
    /// </summary>
    public enum AccuracyEnum : int
    {
        /// <summary>
        /// Unknown location. 
        /// </summary>
        UnkownLocationLevel = 0,
        /// <summary>
        /// Country level accuracy. 
        /// </summary>
        CountryLevel = 1,
        /// <summary>
        /// Region (state, province, prefecture, etc.) level accuracy. 
        /// </summary>
        RegionLevel = 2,
        /// <summary>
        /// Sub-region (county, municipality, etc.) level accuracy.
        /// </summary>
        SubRegionLevel = 3,
        /// <summary>
        /// Town (city, village) level accuracy. 
        /// </summary>
        TownLevel = 4,
        /// <summary>
        /// 	 Post code (zip code) level accuracy.
        /// </summary>
        PostalCodeLevel = 5,
        /// <summary>
        /// Street level accuracy. 
        /// </summary>
        StreetLevel = 6,
        /// <summary>
        /// Intersection level accuracy. 
        /// </summary>
        IntersectionLevel = 7,
        /// <summary>
        /// Address level accuracy. 
        /// </summary>
        AddressLevel = 8,
        /// <summary>
        /// Premise (building name, property name, shopping center, etc.) level accuracy
        /// </summary>
        PremiseLevel = 9
    }
}
