//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Tools.Math.Geo;

//namespace OsmSharp.UI.Map.Elements
//{
//    /// <summary>
//    /// Represents a map element.
//    /// </summary>
//    public interface IElement
//    {
//        /// <summary>
//        /// Returns true if the element is visible inside the given box.
//        /// </summary>
//        /// <param name="box"></param>
//        /// <returns></returns>
//        bool IsVisibleIn(GeoCoordinateBox box);

//        /// <summary>
//        /// Returns the shortest distance to the given coordinate.
//        /// </summary>
//        /// <param name="coordinate"></param>
//        /// <returns></returns>
//        double ShortestDistanceTo(GeoCoordinate coordinate);

//        /// <summary>
//        /// Returns true if the element in visible at the given zoom level.
//        /// </summary>
//        /// <param name="zoomFactor"></param>
//        /// <returns></returns>
//        bool IsVisibleAt(double zoomFactor);

//        /// <summary>
//        /// Returns a dictionary containing descriptive tags for this element.
//        /// </summary>
//        IDictionary<string, string> Tags
//        {
//            get;
//        }
//    }
//}
