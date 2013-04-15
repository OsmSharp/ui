using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Tools.Math.Structures
{
    /// <summary>
    /// Abstracts a data structure indexing objects by their location.
    /// </summary>
    public interface ILocatedObjectIndex<PointType, DataType>
        where PointType : PointF2D
    {
        /// <summary>
        /// Returns all objects inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        IEnumerable<DataType> GetInside(GenericRectangleF2D<PointType> box);

        /// <summary>
        /// Adds new located data.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="data"></param>
        void Add(PointType location, DataType data);
    }
}
