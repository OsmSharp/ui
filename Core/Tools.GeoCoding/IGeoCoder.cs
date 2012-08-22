using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.GeoCoding;

namespace Tools.GeoCoding
{
    /// <summary>
    /// Describes a generic geo coder.
    /// </summary>
    public interface IGeoCoder
    {
        /// <summary>
        /// Creates a new goe coder query.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="postal_code"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="house_number"></param>
        /// <returns></returns>
        IGeoCoderQuery CreateQuery(string country,
            string postal_code,
            string commune,
            string street,
            string house_number);

        /// <summary>
        /// Geocodes the query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IGeoCoderResult Code(IGeoCoderQuery query);
    }
}
